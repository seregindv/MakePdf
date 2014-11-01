using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MakePdf.Configuration;

namespace MakePdf.Galleries
{
    public abstract class HtmlSourceGallery : Gallery
    {
        protected static readonly object _syncRoot = new object();

        protected static volatile string[] _supportedImages;
        protected static IEnumerable<string> SupportedImages
        {
            get
            {
                if (_supportedImages == null)
                    lock (_syncRoot)
                        if (_supportedImages == null)
                            _supportedImages = Config.Instance.AppSettings["SupportedImages"].Split(',');
                return _supportedImages;
            }
        }

        protected static bool IsImage(string pathToImage)
        {
            var extension = Path.GetExtension(pathToImage);
            if (extension == null)
                return false;
            extension = extension.Replace(".", String.Empty);
            return SupportedImages.Contains(extension);
        }

        protected override IEnumerable<GalleryItem> GetItems()
        {
            var matches = Regex.Matches(GalleryDocument.HtmlContents);
            return from Match match in matches
                   select match.Groups[1].Value into href
                   where IsImage(href)
                   select new GalleryItem(href);
        }

        protected abstract Regex Regex { get; }
    }
}
