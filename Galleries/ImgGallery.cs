using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MakePdf.Galleries
{
    public class ImgGallery:HtmlSourceGallery
    {
        protected override Regex Regex
        {
            get { return new Regex(@"\<img .*?src\=""(.+?)"""); }
        }
    }
}
