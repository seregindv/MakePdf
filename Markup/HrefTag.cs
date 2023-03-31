using System.Xml.Serialization;

namespace MakePdf.Markup
{
    [XmlType(TypeName = "Ref")]
    public class HrefTag : Tag
    {
        public HrefTag()
        {
        }

        public HrefTag(string address)
        {
            Address = address;
        }

        [XmlAttribute]
        public string Address { set; get; }
    }
}
