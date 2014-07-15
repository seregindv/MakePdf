using System;
using System.Collections.Generic;
using System.Linq;

namespace MakePdf.Galleries
{
    public class DofigaGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return Document
                .DocumentNode
                .SelectNodes("descendant::img[@itemprop='image']")
                .Select(img =>
                {
                    var imgSrc = new Uri(GalleryUri, img.GetAttributeValue("src", String.Empty)).ToString();
                    return new GalleryItem
                    {
                        ImageUrl = imgSrc.Replace("tn_", String.Empty),
                        ThumbnailImageUrl = imgSrc
                    };
                });
        }
    }
}
