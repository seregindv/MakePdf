using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MakePdf.Configuration;
using MakePdf.Markup;
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public class TextGallery : Gallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var splitted = Regex.Split(GalleryDocument.Contents, Config.Instance.AppSettings["TextGallerySplit"], RegexOptions.Multiline);
            if (splitted.Length == 0)
                yield break;
            GalleryDocument.Contents = String.Empty;
            GalleryDocument.Tags = TagFactory.GetParagraphTags(splitted[0], parseHyperlinks: true);
            for (var i = 1; i < splitted.Length; i = i + 2)
                yield return new GalleryItem(StringHelper.Trim(splitted[i]), TagFactory.GetParagraphTags(splitted[i + 1], parseHyperlinks: true));
        }
    }
}
