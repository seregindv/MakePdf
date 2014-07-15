using System.Xml;
using MakePdf.Galleries;

namespace MakePdf.XmlMakers
{
    public class GalleryDocumentXmlMaker : XmlMaker
    {
        public GalleryDocumentXmlMaker(string templatePath)
            : base(templatePath)
        {
        }

        public HtmlGallery Gallery { set; get; }

        public override XmlDocument GetXml(string title, string annotation, string text, bool removeEmptyStrings, string sourceAddress)
        {
            var result = base.GetXml(title, annotation, text, removeEmptyStrings, sourceAddress);
            return result;
        }
    }
}
