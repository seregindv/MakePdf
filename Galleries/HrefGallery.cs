using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MakePdf.Configuration;
using MakePdf.Stuff;

namespace MakePdf.Galleries
{
    public class HrefGallery : HtmlSourceGallery
    {
        protected override Regex Regex
        {
            get { return new Regex(@"\<a .*?href\=""(.+?)"""); }
        }
    }
}
