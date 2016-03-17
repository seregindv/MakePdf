using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    new GalleryItem(new Uri(GalleryUri, e.GetAttributeValue("href", String.Empty)).ToString(),
                        e.GetAttributeValue("title", String.Empty)));
        }
    }
}
