using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Documents;
using HtmlAgilityPack;
using MakePdf.Markup;

namespace MakePdf.Galleries
{
    public class RbcGallery : InterviewBase
    {
        private readonly Regex _addressRegex = new Regex(@"(\d+)(?=\.shtml)");

        protected override IEnumerable<GalleryItem> GetItems()
        {
            var titleNode = Document.DocumentNode.SelectSingleNode("(//div[@class='article__overview__title']/span)[1]");
            if (titleNode != null)
                GalleryDocument.Name = titleNode.InnerText;
            var annotationNode = Document.DocumentNode.SelectSingleNode("//div[@class='article__overview__text']");
            if (annotationNode != null)
                GalleryDocument.Tags = TagFactory.GetParagraphTags(HttpUtility.HtmlDecode(annotationNode.InnerText));
            return Document
                .DocumentNode
                .SelectNodes("//div[@class='fotorama']/a")
                .Select(GetGalleryItem);
        }

        protected override GalleryItem GetMainItem(HtmlDocument document)
        {
            throw new NotImplementedException();
        }

        protected override HtmlNodeNavigator GetNavigator(HtmlDocument document)
        {
            throw new NotImplementedException();
        }

        private GalleryItem GetGalleryItem(HtmlNode node, int index)
        {
            var result = new GalleryItem(node.GetAttributeValue("href", String.Empty))
            {
                ThumbnailImageUrl = node.SelectSingleNode("img").GetAttributeValue("src", String.Empty)
            };
            var descText = HttpUtility.HtmlDecode(node.GetAttributeValue("data-rbc-description", String.Empty));
            var descDoc = CreateHtmlDocument();
            descDoc.LoadHtml(descText);
            var baseDescNode = descDoc.DocumentNode.SelectSingleNode("/p");

            var titleNode = baseDescNode.SelectSingleNode("b");
            if (titleNode != null)
            {
                result.EnsuredTags.Add(
                    TagFactory.GetTextTag(baseDescNode.SelectSingleNode("b").InnerText)
                        .Wrap<BoldTag>()
                        .Wrap<ParagraphTag>());
                result.EnsuredTags.Add(
                    TagFactory.GetParagraphTag(HttpUtility.HtmlDecode(baseDescNode.SelectSingleNode("span").InnerText)));
            }
            else
            {
                result.Tags = GetItems((HtmlNodeNavigator)descDoc.DocumentNode.ChildNodes[0].CreateNavigator());
            }
            return result;
        }

        protected override NodeProcessResult ProcessTextNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "strong" || node.Name == "span")
            {
                tags.Add(TagFactory.GetTextTag(HttpUtility.HtmlDecode(node.InnerText)).Wrap<BoldTag>());
                return new NodeProcessResult(true, false);
            }

            return base.ProcessTextNode(node, tags);
        }

        /*
                private GalleryItem GetGalleryItem(HtmlNode node, int index)
                {
                    var address = _addressRegex.Replace(GalleryDocument.SourceAddress, (index + 1).ToString());
                    return new GalleryItem
                    {
                        ThumbnailImageUrl = node.SelectSingleNode("//img").GetAttributeValue("src", String.Empty),
                        Index = index,
                        Url = address
                    };
                }

                public override void LoadItem(GalleryItem item)
                {
                    if (item.HasUrl)
                    {
                        var doc = GetItemHtmlDocument(item);

                        var picNode = doc.DocumentNode.SelectSingleNode("//div[@class='fotorama__stage__frame fotorama__loaded fotorama__loaded--img fotorama__active']//img");
                        if (picNode != null)
                            item.ImageUrl = picNode.GetAttributeValue("src", String.Empty);

                        var textNode = doc.DocumentNode.SelectSingleNode("//div[@class='gallery__text js-photoreport-text']/p");
                        if (textNode != null)
                        {
                            var texts = new[]
                            {
                                textNode.SelectSingleNode("b").InnerText,
                                textNode.SelectSingleNode("span").InnerText
                            };
                            item.Tags = TagFactory.GetParagraphTags(texts);
                        }
                    }
                    base.LoadItem(item);
                }
         */
    }
}
