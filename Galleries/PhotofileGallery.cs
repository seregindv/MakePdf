using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MakePdf.Galleries
{
    public class PhotofileGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var regex = new Regex(@"src\=""([^""]+\/small\/\d+\.jpg).*?""\s+alt\=""(.+?)""");
            return
                from Match match in regex.Matches(GalleryDocument.HtmlContents)
                select new GalleryItem(match.Groups[1].Value.Replace("/small", String.Empty), match.Groups[2].Value);
        }
    }
}
