using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using MakePdf.Markup;

namespace MakePdf.Galleries
{
    public class DofigaGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var imageNodes = Document
                .DocumentNode
                .SelectNodes("descendant::img[@itemprop='image']");
            if (imageNodes != null && imageNodes.Count > 0)
                return imageNodes.Select(img =>
                {
                    var imgSrc = new Uri(GalleryUri, img.GetAttributeValue("src", String.Empty)).ToString();
                    var url = img.ParentNode.Name == "a"
                        ? new Uri(GalleryUri, img.ParentNode.GetAttributeValue("href", String.Empty)).ToString()
                        : null;

                    return new GalleryItem
                    {
                        ImageUrl = imgSrc.Replace("tn_", String.Empty),
                        ThumbnailImageUrl = imgSrc,
                        Url = url
                    };
                });

            var textDiv = Document.DocumentNode.SelectSingleNode("//div[@itemprop='text']");
            if (textDiv != null)
                return textDiv.SelectNodes("//img[@border]").Select(node =>
                    new GalleryItem(new Uri(GalleryUri, node.GetAttributeValue("src", String.Empty)).ToString(), null, node.GetAttributeValue("width", 0), node.GetAttributeValue("height", 0)));
            return Enumerable.Empty<GalleryItem>();
        }

        public override void LoadItem(GalleryItem item)
        {
            if (item.HasUrl)
            {
                var doc = GetItemHtmlDocument(item);
                var aboutNode = doc.DocumentNode.SelectSingleNode("//div[@class='about_image']");
                if (aboutNode != null)
                    item.Tags = TagFactory.GetParagraphTags(aboutNode.InnerText);
            }
            base.LoadItem(item);
        }
    }
}
