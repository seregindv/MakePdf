using System.Xml.Serialization;

namespace MakePdf.Markup
{
    [XmlType(TypeName = "Color")]
    public class ColorTag : Tag
    {
        public ColorTag()
        {
        }

        public ColorTag(string color)
        {
            Value = color;
        }

        [XmlAttribute]
        public string Value { set; get; }
    }
}
