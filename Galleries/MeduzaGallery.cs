using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MakePdf.Markup;
using MakePdf.Stuff;

namespace MakePdf.Galleries
{
    public class MeduzaGallery : HtmlGallery
    {
        public MeduzaGallery()
        {
            HtmlEncoding = Encoding.UTF8;
        }

        protected override IEnumerable<GalleryItem> GetItems()
        {
            GalleryDocument.Name = Document.DocumentNode.SelectSingleNode(@"(//span[@class='NewsTitle-first'])[1]").InnerText;
            GalleryDocument.Annotation = Document.DocumentNode.SelectSingleNode(@"(//span[@class='NewsTitle-second'])[1]").InnerText;

            var mainItemNode = Document.DocumentNode.SelectSingleNode(@"(//div[@class='MediaMaterial-image'])[1]");
            if (mainItemNode == null)
                return null;
            var mainItemStyle = mainItemNode.GetAttributeValue("style", null);
            var mainItemSrc = Regex.Match(mainItemStyle, @"background\-image\s*:\s*url\(\s*(.+?)\s*\)").Groups[1].Value;
            var mainItem = new GalleryItem(new Uri(GalleryUri, mainItemSrc).ToString(),
                TagFactory.GetParagraphTags(Document.DocumentNode.SelectNodes(@"//div[@class='Body']/p").Select(node => node.InnerText)));
            var items = Document.DocumentNode
                .SelectNodes(@"//div[@class='Gallery']/div[@class='Gallery-item']")
                .Select(
                    node =>
                        new GalleryItem(new Uri(GalleryUri,
                            node.SelectSingleNode(@"(./div[@class='GalleryImage']/img)[1]")
                                .GetAttributeValue("src", null)).ToString(), GetGalleryImageTags(node.SelectSingleNode(@"(./div[@class='GalleryItemMeta'])[1]"))));
            return Enumerable.Repeat(mainItem, 1).Union(items);
        }

        private List<Tag> GetGalleryImageTags(HtmlNode node)
        {
            var result = TagFactory.GetParagraphTags(
                node.SelectNodes(@"./div[@class='GalleryItemMeta-wrapper']/span")
                    .Select(wrapperNode => wrapperNode.InnerText));
            result.AddRange(TagFactory.GetParagraphTags(
                node.SelectNodes(@"./p[@class='GalleryItemMeta-text']")
                    .Select(textNode => textNode.InnerText)));
            return result;
        }
    }
}
