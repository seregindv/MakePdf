using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Fonet.Image;

namespace MakePdf.Sizing
{
    [DebuggerDisplay("{Width}x{Height}")]
    public class ImageSize : IComparable<ImageSize>, IComparable
    {
        public int Width { set; get; }
        public int Height { set; get; }

        public ImageSize()
        {
        }

        public ImageSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public ImageSize(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        #region Implementation of IComparable<in ComparableSize>

        public int CompareTo(ImageSize other)
        {
            return ToInt().CompareTo(other.ToInt());
        }

        #endregion

        #region Implementation of IComparable

        public int CompareTo(object obj)
        {
            if (!(obj is ImageSize))
                return 1;
            return CompareTo((ImageSize)obj);
        }

        #endregion

        private int ToInt()
        {
            return Width * 100000 + Height;
        }

        public static ImageSize FromStream(Stream stream)
        {
            if (stream.Position != 0L)
                stream.Position = 0L;
            var jpi = new JpegParser(stream).Parse();
            return new ImageSize(jpi.Width, jpi.Height);
        }

        public static ImageSize FromFile(string file)
        {
            try
            {
                using (var fs = new FileStream(file, FileMode.Open))
                    return FromStream(fs);
            }
            catch
            {
                using (var image = Image.FromFile(file))
                    return new ImageSize(image.Size);
            }
        }
    }
}
