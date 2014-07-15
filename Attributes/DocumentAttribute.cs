using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakePdf.Attributes
{
    public abstract class DocumentAttribute : Attribute
    {
        public Type Type { set; get; }
        public string Regex { set; get; }
    }
}
