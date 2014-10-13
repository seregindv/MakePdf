using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MakePdf.Stuff;

namespace MakePdf.Galleries
{
    public class RbcPhotoreportGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return Document
                .DocumentNode
                .SelectNodes("//div[@class='gallery']/div[@class='fotorama']/a")
                .Select(node => new GalleryItem(node.Attributes["href"].Value, Utils.GetInnerText(node.Attributes["data-rbc-description"].Value)) { ThumbnailImageUrl = node.SelectSingleNode("img").Attributes["src"].Value });
        }
    }
}
