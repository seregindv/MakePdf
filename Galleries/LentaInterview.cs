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
    public class LentaInterview : InterviewBase
    {
        protected override GalleryItem GetMainItem(HtmlDocument document)
        {
            return new LentaPictureProcessor(document).GetMainPicture();
        }

        protected override HtmlNodeNavigator GetNavigator(HtmlDocument document)
        {
            return (HtmlNodeNavigator)document.DocumentNode.SelectSingleNode("//div[@itemprop='articleBody']").CreateNavigator();
        }

        protected override HtmlDocument GetDocument()
        {
            if (!GalleryDocument.SourceAddress.Contains("lenta.ru/features"))
                return base.GetDocument();

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

        protected override NodeProcessResult ProcessTextNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "p")
            {
                var classAttr = node.Attributes["class"];
                if (classAttr != null && classAttr.Value == "question")
                {
                    tags.Add(new ItalicTag().Wrap<BoldTag>().Wrap<ParagraphTag>());
                    return new NodeProcessResult(true);
                }
            }

            if (node.Name == "blockquote")
            {
                tags.Add(new ParagraphTag());
                return new NodeProcessResult(true);
            }

            return base.ProcessTextNode(node, tags);
        }

        protected override bool ProcessGalleryNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "aside" && node.GetAttributeValue("class", String.Empty).Contains("b-inline-gallery-box"))
            {
                var data = node.GetAttributeValue("data-box", String.Empty);
                if (!String.IsNullOrEmpty(data))
                {
                    var json = JToken.Parse(HttpUtility.HtmlDecode(node.GetAttributeValue("data-box", String.Empty)));
                    var images = json.Children()
                        .Select(jsonItem =>
                            new GalleryItem(jsonItem.Value<string>("original_url"),
                                jsonItem.Value<string>("caption"),
                                jsonItem.Value<int>("original_width"),
                                jsonItem.Value<int>("original_height")));
                    var oldCount = tags.Count;
                    tags.AddRange(images);
                    return oldCount != tags.Count;
                }
            }
            return base.ProcessGalleryNode(node, tags);
        }
    }
}
