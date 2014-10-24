using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Navigation;
using HtmlAgilityPack;
using MakePdf.Markup;

namespace MakePdf.Galleries
{
    public class RbcInterview : InterviewBase
    {
        protected override GalleryItem GetMainItem(HtmlDocument document)
        {
            var imageNode = document.DocumentNode.SelectSingleNode("//div[@class='article__main-image']/img");
            var textNode = document.DocumentNode.SelectSingleNode("//div[@class='article__overview__text']");
            return new GalleryItem(imageNode.GetAttributeValue("src", String.Empty), textNode.InnerText);
        }

        protected override HtmlNodeNavigator GetNavigator(HtmlDocument document)
        {
            return (HtmlNodeNavigator)document.DocumentNode.SelectSingleNode("//div[@class='article__text']").CreateNavigator();
        }

        protected override bool ProcessGalleryNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "p")
            {
                var imageNode = node.SelectSingleNode("div[@class='article__infographic']//img");
                if (imageNode != null)
                {
                    tags.Add(new GalleryItem(imageNode.GetAttributeValue("src", String.Empty)));
                    return true;
                }
            }

            return base.ProcessGalleryNode(node, tags);
        }

        protected override NodeProcessResult ProcessTextNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "p")
            {
                if (node.SelectSingleNode("div[@class='article__photoreport']") != null)
                    return new NodeProcessResult(true, false);

                var headerNode = node.SelectSingleNode("span/strong");
                if (headerNode != null)
                {
                    tags.Add(TagFactory.GetTextTag(headerNode.InnerText).Wrap(GetHeaderTag()));
                    return new NodeProcessResult(true, false);
                }

                var emNode = node.SelectSingleNode("em");
                if (emNode != null)
                {
                    tags.Add(TagFactory.GetTextTag(HttpUtility.HtmlDecode(emNode.InnerText)).Wrap<ParagraphTag>());
                    return new NodeProcessResult(true, false);
                }
            }

            if (node.Name == "div")
            {
                tags.Add(TagFactory.GetParagraphTag(HttpUtility.HtmlDecode(node.InnerText)));
                return new NodeProcessResult(true, false);
            }

            if (node.Name == "strong")
            {
                return new NodeProcessResult(true);
            }

            return base.ProcessTextNode(node, tags);
        }
    }
}

