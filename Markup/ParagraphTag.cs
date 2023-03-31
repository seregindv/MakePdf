using System.Xml.Serialization;

namespace MakePdf.Markup
{
    [XmlType(TypeName = "Paragraph")]
    public class ParagraphTag : Tag
    {
        [XmlAttribute]
        public string Color { set; get; }

        [XmlAttribute]
        public string BackgroundColor { set; get; }

        [XmlAttribute]
        public string Alignment { set; get; }
    }
}
