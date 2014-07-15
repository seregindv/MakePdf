using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MakePdf.Configuration
{
    [XmlType("template")]
    public class Template
    {
        [XmlAttribute("type")]
        public TemplateType Type { set; get; }
        [XmlAttribute("path")]
        public string Path { set; get; }
    }
}
