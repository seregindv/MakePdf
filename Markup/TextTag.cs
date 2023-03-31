using System.Xml.Serialization;

namespace MakePdf.Markup
{
    [XmlType(TypeName = "Text")]
    public class TextTag : Tag
    {
        public TextTag()
        {
        }

        public TextTag(string value)
        {
            Value = value;
        }

        [XmlText]
        public string Value { set; get; }
    }
}
