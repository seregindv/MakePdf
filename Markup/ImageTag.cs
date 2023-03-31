using System.Xml.Serialization;
using MakePdf.Helpers;

namespace MakePdf.Markup
{
    [XmlType(TypeName = "Image")]
    public class ImageTag : Tag
    {
        [XmlAttribute("Source")]
        public string ImageUrl { set; get; }
        [XmlAttribute("Href")]
        public string Url { set; get; }
        [XmlAttribute]
        public string Thumb { set; get; }
        [XmlAttribute]
        public int Width { set; get; }
        [XmlAttribute]
        public int Height { set; get; }
        [XmlAttribute]
        public string LocalPath { set; get; }
        [XmlAttribute]
        public int Index { set; get; }

        public string GetFormattedIndex(string format = "00000000")
        {
            return PathHelper.GetFileName(Index, format);
        }
    }
}
