using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MakePdf.Markup;

namespace MakePdf.Galleries
{
    public class VillageGallery : InterviewBase
    {
        public VillageGallery()
        {
            HtmlEncoding = Encoding.UTF8;
        }

        protected override GalleryItem GetMainItem(HtmlDocument document)
        {
            return new GalleryItem(document
                .DocumentNode
                .SelectSingleNode(@"(//div[@class='cover-image']/img)[1]")
                .GetAttributeValue("src", String.Empty));
        }

        protected override HtmlNodeNavigator GetNavigator(HtmlDocument document)
        {
            return (HtmlNodeNavigator)document
                .DocumentNode
                .SelectSingleNode(@"(//div[@class='article-text'])[1]")
                .CreateNavigator();
        }

        protected override bool ProcessGalleryNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "p")
            {
                var slideshowNode = node.SelectSingleNode(@"(./span[@class='img-with-caption'])[1]");
                if (slideshowNode == null)
                    return false;
                tags.AddRange(from imgNode in node.SelectNodes(@".//img")
                    let dataOriginal = imgNode.GetAttributeValue("data-original", null)
                    select new GalleryItem(dataOriginal ?? imgNode.GetAttributeValue("src", null)));
                return true;
            }
            return false;
        }

        protected override NodeProcessResult ProcessTextNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "div")
            {
                var divClassName = node.GetAttributeValue("class", String.Empty);
                if (Regex.Matches(divClassName, @"\bx(1|2)\b").Count > 0)
                    return new NodeProcessResult(true, false);
                return new NodeProcessResult(true);
            }
            if (node.Name == "p" && node.GetAttributeValue("class", String.Empty) == "mb-4")
                return new NodeProcessResult(true, false);
            if (node.Name == "strong")
                return new NodeProcessResult(true, false);
            if (node.Name == "em")
                return new NodeProcessResult(true);
            if (node.Name == "img" && node.GetAttributeValue("src", String.Empty).StartsWith("data:"))
                return new NodeProcessResult(true, false);

            return base.ProcessTextNode(node, tags);
        }
    }
}