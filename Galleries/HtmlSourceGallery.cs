using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MakePdf.Configuration;
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public abstract class HtmlSourceGallery : Gallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var matches = Regex.Matches(GalleryDocument.HtmlContents);
            return from Match match in matches
                   select match.Groups[1].Value into href
                   where PathHelper.IsImage(href)
                   select new GalleryItem(href);
        }

        protected abstract Regex Regex { get; }
    }
}
