using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media;
using System.Windows.Navigation;
using HtmlAgilityPack;
using MakePdf.Markup;
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public class RbcInterview : InterviewBase
    {
        protected override GalleryItem GetMainItem(HtmlDocument document)
        {
            var imageNode = document.DocumentNode.SelectSingleNode("//div[@class='article__main-image']/img");
            var textNode = document.DocumentNode.SelectSingleNode("//div[@class='article__overview__text']");
            return new GalleryItem(imageNode.GetSrc(), textNode.InnerText);
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
                    tags.Add(new GalleryItem(imageNode.GetSrc()));
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
                if (node.GetAttributeValue("class", String.Empty).Contains("banner"))
                    return new NodeProcessResult(true, false);
                var match = Regex.Match(node.GetAttributeValue("style", String.Empty),
                    @"background:\s*rgb\((\d+),\s*(\d+),\s*(\d+)\)");
                var tag = TagFactory.GetParagraphTag(HttpUtility.HtmlDecode(node.InnerText));
                tag.BackgroundColor = match.Success
                    ? StringHelper.GetColorAsString(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value)
                    : null;
                tags.Add(tag);
                return new NodeProcessResult(true, false);
            }

            if (node.Name == "strong")
                return new NodeProcessResult(true);

            return base.ProcessTextNode(node, tags);
        }
    }
}

