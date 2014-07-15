using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using HtmlAgilityPack;
using MakePdf.Galleries.Helpers;
using MakePdf.Markup;
using MakePdf.Stuff;
using Newtonsoft.Json.Linq;

namespace MakePdf.Galleries
{
    public class LentaInterview : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var document = GetDocument();
            var navigator = document
                .CreateNavigator()
                .SelectDescendants("div", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .First(@nav => @nav.GetAttribute("itemprop", String.Empty) == "articleBody");
            navigator.MoveToFirstChild();
            GalleryDocument.Contents = String.Empty;
            return GetItems(navigator, new LentaPictureProcessor(document).GetMainPicture());
        }

        private HtmlDocument GetDocument()
        {
            if (GalleryDocument.SourceAddress.Contains("lenta.ru/features"))
            {
                var navigator = Document.CreateNavigator();
                var url = navigator
                    .SelectDescendants("a", String.Empty, false)
                    .OfType<HtmlNodeNavigator>()
                    .First(@nav =>
                    {
                        var @href = @nav.GetAttribute("href", String.Empty);
                        return @nav.GetAttribute("class", String.Empty) == "item"
                               && @href != null
                               && GalleryDocument.SourceAddress.Contains(@href);
                    })
                    .GetAttribute("data-url", String.Empty);
                url = new Uri(GalleryUri, url).ToString();
                return GetHtmlDocument(url, "interview");
            }
            return Document;
        }

        private static List<Tag> GetTags(HtmlNodeNavigator navigator)
        {
            var tags = new List<Tag>();
            if (!navigator.MoveToFirst())
                return null;
            do
            {
                var recognized = true;
                switch (navigator.CurrentNode.Name)
                {
                    case "p":
                        var classAttr = navigator.CurrentNode.Attributes["class"];
                        if (classAttr != null && classAttr.Value == "question")
                            tags.Add(new ItalicTag().Wrap<BoldTag>().Wrap<ParagraphTag>());
                        else
                            tags.Add(new ParagraphTag());
                        break;
                    case "i":
                        tags.Add(new ItalicTag());
                        break;
                    case "b":
                        tags.Add(new BoldTag());
                        break;
                    case "a":
                        tags.Add(new HrefTag(navigator.CurrentNode.GetAttributeValue("href", String.Empty)));
                        break;
                    case "h1":
                        tags.Add(new ColorTag("grey").Wrap<ParagraphTag>());
                        break;
                    default:
                        var text = Utils.Trim(navigator.CurrentNode.InnerText);
                        if (text.Length > 0)
                        {
                            // keep spaces etc
                            text = navigator.CurrentNode.InnerText;
                            if (navigator.CurrentNode.Name != "#text")
                                recognized = false;
                            if (!recognized)
                            {
                                text = navigator.CurrentNode.Name + ">" + Utils.Trim(text);
                                tags.Add(TagFactory.GetTextTag(text).Wrap(new ColorTag("red")).Wrap<ParagraphTag>());
                            }
                            else
                                tags.Add(TagFactory.GetTextTag(text));
                        }
                        break;
                }
                if (recognized && navigator.CurrentNode.HasChildNodes)
                {
                    navigator.MoveToFirstChild();
                    if (tags.Any())
                        tags.Last().DrillDown().Tags = GetTags(navigator);
                    else
                        tags = GetTags(navigator);
                    navigator.MoveToParent();
                }
            } while (navigator.MoveToNext());
            return tags;
        }

        private static List<GalleryItem> GetItems(HtmlNodeNavigator navigator, GalleryItem mainItem)
        {
            var result = new List<GalleryItem> { mainItem };
            var tags = mainItem.EnsuredTags;
            if (!navigator.MoveToFirst())
                return null;
            do
            {
                var processChildren = true;
                switch (navigator.CurrentNode.Name)
                {
                    case "p":
                        var classAttr = navigator.CurrentNode.Attributes["class"];
                        if (classAttr != null && classAttr.Value == "question")
                            tags.Add(new ItalicTag().Wrap<BoldTag>().Wrap<ParagraphTag>());
                        else
                            tags.Add(new ParagraphTag());
                        break;
                    case "i":
                        tags.Add(new ItalicTag());
                        break;
                    case "b":
                        tags.Add(new BoldTag());
                        break;
                    case "a":
                        tags.Add(new HrefTag(navigator.CurrentNode.GetAttributeValue("href", String.Empty)));
                        break;
                    case "h1":
                        tags.Add(new ColorTag("grey").Wrap<ParagraphTag>());
                        break;
                    case "aside":
                        if (ProcessAside(navigator, result))
                        {
                            tags = result.Last().EnsuredTags;
                            processChildren = false;
                        }
                        break;
                    default:
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
                                tags.Add(TagFactory.GetTextTag(text));
                        }
                        break;
                }
                if (processChildren && navigator.CurrentNode.HasChildNodes)
                {
                    navigator.MoveToFirstChild();
                    var gotTags = GetTags(navigator);
                    if (tags.Any())
                        if (gotTags.First() is ParagraphTag)
                            tags.AddRange(gotTags);
                        else
                            tags.Last().DrillDown().Tags = gotTags;
                    else
                        tags = gotTags;
                    navigator.MoveToParent();
                }
            } while (navigator.MoveToNext());
            return result;
        }

        private static bool ProcessAside(HtmlNodeNavigator navigator, List<GalleryItem> galleryItems)
        {
            if (navigator.GetAttribute("class", String.Empty).Contains("b-inline-gallery-box"))
            {
                var data = navigator.GetAttribute("data-box", String.Empty);
                if (!String.IsNullOrEmpty(data))
                {
                    var json = JToken.Parse(HttpUtility.HtmlDecode(navigator.GetAttribute("data-box", String.Empty)));
                    var images = json.Children()
                        .Select(jsonItem =>
                            new GalleryItem(jsonItem.Value<string>("original_url"),
                                jsonItem.Value<string>("caption"),
                                jsonItem.Value<int>("original_width"),
                                jsonItem.Value<int>("original_height")));
                    var oldCount = galleryItems.Count;
                    galleryItems.AddRange(images);
                    return oldCount != galleryItems.Count;
                }
            }
            return false;
        }
    }
}
