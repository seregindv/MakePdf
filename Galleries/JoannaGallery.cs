using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public class JoannaGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return Document
                .DocumentNode
                .SelectNodes("//div[@class='galleryInnerImageHolder']/a")
                .Select(e =>
                    new GalleryItem(new Uri(GalleryUri, e.GetHref()).ToString(),
                        e.GetAttributeValue("title", String.Empty)));
        }
    }
}
