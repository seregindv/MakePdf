using MakePdf.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MakePdf.Helpers
{
    public static class PathHelper
    {
        static readonly object _syncRoot = new object();
        static volatile string[] _supportedImages;
        static IEnumerable<string> SupportedImages
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

        public static string GetFileName(int fileNum, string format = "00000000")
        {
            return fileNum.ToString(format);
        }

        public static string GetFullPath(string directory, string fileName)
        {
            return Path.Combine(directory, TrimIllegalChars(fileName));
        }

        public static string GetPdfPath(string directory, string title, string extension = ".pdf")
        {
            var fileName = (title.Length > 255 ? title.Substring(0, 255) : title).Trim() + extension;
            return GetFullPath(directory, fileName);
        }

        public static bool IsImage(string pathToImage)
        {
            var extension = Path.GetExtension(pathToImage);
            if (extension == null)
                return false;
            extension = extension.Replace(".", String.Empty);
            return SupportedImages.Contains(extension, StringComparer.InvariantCultureIgnoreCase);
        }

        public static string TrimIllegalChars(string path)
        {
            return string.Concat(path.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
