using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using MakePdf.Markup;
using MakePdf.Sizing;
using MakePdf.Stuff;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MakePdf.Galleries
{
    public class LentaBeelineGallery : HtmlGallery
    {
        public LentaBeelineGallery()
        {
            HtmlEncoding = Encoding.UTF8;
        }

        protected override IEnumerable<GalleryItem> GetItems()
        {
            GalleryDocument.Tags = new List<Tag>();
            var tags = GalleryDocument.Tags;
            tags.Add(TagFactory.GetParagraphTag(
                HttpUtility.HtmlDecode(Utils.Trim(Document
                .DocumentNode
                .SelectSingleNode("descendant::div[@class='description'][1]")
                .InnerText))));
            var result = new List<GalleryItem>();
            Action<HtmlNode> processGalleryDataNode = (node) =>
            {
                var json = JToken.Parse(node.GetAttributeValue("data-json", String.Empty));
                result.AddRange(
                    json.Children().Select(child =>
                    {
                        var doc = new HtmlDocument();
                        doc.LoadHtml(
                            child.SelectToken("description").Value<string>());
                        return new GalleryItem(
                            new Uri(GalleryUri, child.SelectToken("original.url").Value<string>()).ToString(),
                            HttpUtility.HtmlDecode(doc.DocumentNode.InnerText),
                            child.SelectToken("original.width").Value<int>(),
                            child.SelectToken("original.height").Value<int>(),
                            new Uri(GalleryUri, child.SelectToken("thumbnails.small.url").Value<string>()).ToString()
                            );
                    }
            )
                );
                if (result.Any())
                {
                    if (result[result.Count - 1].Tags == null)
                        result[result.Count - 1].Tags = new List<Tag>();
                    tags = result[result.Count - 1].Tags;
                }
            };
            foreach (var node in Document
                .DocumentNode
                .SelectNodes("descendant::div[@class='b-content-page-content-block b-rich-content'][1]/*"))
            {
                switch (node.Name)
                {
                    case "h4":
                        tags.Add(TagFactory.GetTextTag(HttpUtility.HtmlDecode(node.InnerText)).Wrap<BoldTag>().Wrap<ParagraphTag>());
                        break;
                    case "p":
                        var galleryDataNode = node.SelectSingleNode("div[@class='gallery-data']");
                        if (galleryDataNode != null)
                            processGalleryDataNode(galleryDataNode);
                        else
                            tags.Add(TagFactory.GetParagraphTag(HttpUtility.HtmlDecode(node.InnerText)));
                        break;
                    case "div":
                        if (node.GetAttributeValue("class", String.Empty) == "gallery-data")
                            processGalleryDataNode(node);
                        break;
                }
            }
            return result;
        }

        private List<Tag> GetTags(ref List<Tag> tags)
        {
            return tags ?? (tags = new List<Tag>());
        }
    }
}
