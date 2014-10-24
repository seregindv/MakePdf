﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MakePdf.Markup;

namespace MakePdf.Galleries
{
    public class MedusaInterview : InterviewBase
    {
        public MedusaInterview()
        {
            HtmlEncoding = Encoding.UTF8;
        }

        protected override GalleryItem GetMainItem(HtmlDocument document)
        {
            var imageNode = document.DocumentNode.SelectSingleNode("//link[@rel='image_src']");
            var textNode = document.DocumentNode.SelectSingleNode("//div[@class='Lead']/p");
            return new GalleryItem(imageNode.GetAttributeValue("href", String.Empty), textNode.InnerText);
        }

        protected override HtmlNodeNavigator GetNavigator(HtmlDocument document)
        {
            return (HtmlNodeNavigator)document.DocumentNode.SelectSingleNode("//div[@class='Body']").CreateNavigator();
        }

        protected override bool ProcessGalleryNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "figure")
            {
                var imageNode = node.SelectSingleNode("//img");
                if (imageNode != null)
                {
                    tags.Add(new GalleryItem(new Uri(GalleryUri, imageNode.GetAttributeValue("src", String.Empty)).ToString(), node.InnerText));
                    return true;
                }
            }

            return base.ProcessGalleryNode(node, tags);
        }

        protected override NodeProcessResult ProcessTextNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "br" || node.Name == "nobr")
                return new NodeProcessResult(true);

            return base.ProcessTextNode(node, tags);
        }
    }
}
