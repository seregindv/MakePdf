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
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public class RbcGallery : InterviewBase
    {
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
            var result = new GalleryItem(node.GetHref())
            {
                ThumbnailImageUrl = node.SelectSingleNode("img").GetSrc()
            };
            var descText = HttpUtility.HtmlDecode(node.GetAttributeValue("data-rbc-description", String.Empty));
            var descDoc = HtmlHelper.CreateHtmlDocument(descText);
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
    }
}
