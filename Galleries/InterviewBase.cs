using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using MakePdf.Galleries.Helpers;
using MakePdf.Markup;
using MakePdf.Stuff;
using Newtonsoft.Json.Linq;

namespace MakePdf.Galleries
{
    public abstract class InterviewBase : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var document = GetDocument();
            var navigator = GetNavigator(document);
            navigator.MoveToFirstChild();
            GalleryDocument.Contents = String.Empty;
            return GetItems(navigator, GetMainItem(document)).Cast<GalleryItem>();
        }

        protected virtual bool ProcessTextNode(HtmlNode node, List<Tag> tags)
        {
            switch (node.Name)
            {
                case "p":
                    tags.Add(new ParagraphTag());
                    break;
                case "i":
                    tags.Add(new ItalicTag());
                    break;
                case "b":
                    tags.Add(new BoldTag());
                    break;
                case "a":
                    tags.Add(new HrefTag(node.GetAttributeValue("href", String.Empty)));
                    break;
                case "h1":
                    tags.Add(new ColorTag("grey").Wrap<ParagraphTag>());
                    break;
                default:
                    return false;
            }
            return true;
        }

        protected virtual bool ProcessGalleryNode(HtmlNode node, List<Tag> tags)
        {
            return false;
        }

        protected abstract GalleryItem GetMainItem(HtmlDocument document);

        protected abstract HtmlNodeNavigator GetNavigator(HtmlDocument document);

        protected virtual HtmlDocument GetDocument()
        {
            return Document;
        }

        private List<Tag> GetItems(HtmlNodeNavigator navigator, Tag mainItem = null)
        {
            List<Tag> result = null;
            List<Tag> tags;
            if (mainItem != null)
            {
                result = mainItem.ToTags();
                tags = mainItem.EnsuredTags;
            }
            else
                tags = new List<Tag>();

            if (!navigator.MoveToFirst())
                return null;
            do
            {
                var processed = ProcessGalleryNode(navigator.CurrentNode, result);
                if(processed)
                    tags = result.Last().EnsuredTags;
var processChildren = !processed;
                if (!processed)
                    processed = ProcessTextNode(navigator.CurrentNode, tags);
                if (!processed)
                {
                    var text = Utils.Trim(navigator.CurrentNode.InnerText);
                    if (text.Length > 0)
                    {
                        // keep spaces etc
                        text = navigator.CurrentNode.InnerText;
                        if (navigator.CurrentNode.Name != "#text")
                            processChildren = false;
                        if (!processChildren)
                        {
                            text = navigator.CurrentNode.Name + ">" + Utils.Trim(text);
                            tags.Add(TagFactory.GetTextTag(text).Wrap(new ColorTag("red")).Wrap<ParagraphTag>());
                        }
                        else
                            tags.Add(TagFactory.GetTextTag(HttpUtility.HtmlDecode(text)));
                    }
                }
                if (processChildren && navigator.CurrentNode.HasChildNodes)
                {
                    navigator.MoveToFirstChild();
                    var gotTags = GetItems(navigator);
                    if (tags.Any())
                        if (gotTags.Count > 0 && gotTags.First() is ParagraphTag)
                            tags.AddRange(gotTags);
                        else
                            tags.Last().DrillDown().Tags = gotTags;
                    else
                        tags = gotTags;
                    navigator.MoveToParent();
                }
            } while (navigator.MoveToNext());
            return mainItem == null ? tags : result;
        }
    }
}
