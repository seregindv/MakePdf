﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Web;
using HtmlAgilityPack;
using MakePdf.Controls;
using MakePdf.Galleries.Processors;
using MakePdf.Markup;
using MakePdf.Helpers;
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
                    var @href = @nav.GetHref();
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
                if (node.ParentNode.Name == "blockquote")
                    return new NodeProcessResult(true);

                var classAttr = node.Attributes["class"];
                if (classAttr != null && classAttr.Value == "question")
                {
                    tags.Add(new ItalicTag().Wrap<BoldTag>().Wrap<ParagraphTag>());
                    return new NodeProcessResult(true);
                }
            }

            if (node.Name == "blockquote")
            {
                if (node.ParentNode.Name != "p")
                    tags.Add(new ParagraphTag());
                return new NodeProcessResult(true);
            }

            return base.ProcessTextNode(node, tags);
        }

        protected override bool ProcessGalleryNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "aside")
            {
                var className = node.GetAttributeValue("class", String.Empty);
                if (className.Contains("b-inline-gallery-box"))
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
                else if (className.Contains("b-inline-image-box"))
                {
                    var imageNode = node.SelectSingleNode("img");
                    if (imageNode != null)
                    {
                        tags.Add(new GalleryItem(imageNode.GetSrc(),
                            imageNode.GetAttributeValue("alt", String.Empty),
                            imageNode.GetAttributeValue("width", 0),
                            imageNode.GetAttributeValue("height", 0)));
                        return true;
                    }
                    var divNode = node.SelectSingleNode("div[@class='picture js-zoom']");
                    if (divNode != null)
                    {
                        tags.Add(new GalleryItem(divNode.GetAttributeValue("data-url", String.Empty),
                            divNode.GetAttributeValue("data-alt", String.Empty)));
                        return true;
                    }
                }
            }
            else if (node.Name == "p" && node.HasChildNodes)
            {
                var childNode = node.ChildNodes[0];
                if (childNode.Name == "img" && node.ChildNodes.Count == 1)
                {
                    tags.Add(new GalleryItem(childNode.GetSrc(),
                        childNode.GetAttributeValue("alt", String.Empty),
                        childNode.GetAttributeValue("width", 0),
                        childNode.GetAttributeValue("height", 0)));
                    return true;
                }

                childNode = node.SelectSingleNode("./blockquote[@class='instagram-media']");
                if (childNode != null)
                {
                    tags.Add(new GalleryItem
                    {
                        Url = childNode.SelectSingleNode(".//a").GetHref(),
                        Tags = TagFactory.GetParagraphTag(childNode.InnerText).ToTags()
                    });
                    return true;
                }

                //childNode = node.SelectSingleNode("./div[@class='fb-post']");
                //if (childNode != null)
                //{
                //    tags.Add(new GalleryItem
                //    {
                //        Url = childNode.GetAttributeValue("data-href", String.Empty),
                //        Tags = TagFactory.GetParagraphTag(node.InnerText).ToTags()
                //    });
                //}
            }
            return base.ProcessGalleryNode(node, tags);
        }

        public override void LoadItem(GalleryItem item)
        {
            if (item.Url != null)
            {
                var uri = new Uri(item.Url);
                var document = GetItemHtmlDocument(item);
                if (uri.Host == "instagram.com")
                    item.ImageUrl = document
                        .DocumentNode
                        .SelectSingleNode("//meta[@property='og:image']")
                        .GetAttributeValue("content", String.Empty);
                //else if (uri.Host == "www.facebook.com")
                //    item.ImageUrl = document
                //        .DocumentNode
                //        .SelectSingleNode("//img[@id='fbPhotoImage']")
                //        .GetSrc();
            }
            base.LoadItem(item);
        }
    }
}
