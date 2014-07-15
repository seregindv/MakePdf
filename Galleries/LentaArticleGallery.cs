using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakePdf.Galleries.Helpers;

namespace MakePdf.Galleries
{
    public class LentaArticleGallery : ClipboardSourceHtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return new LentaPictureProcessor(Document).GetPictures();
        }
    }
}
