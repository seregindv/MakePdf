using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using MakePdf.Attributes;
using MakePdf.ViewModels;
using MakePdf.Controls;

namespace MakePdf.Helpers
{
    public class PasteProcessor
    {
        public void ProcessPaste(PasteParameter parameter)
        {
            const string HTML_FORMAT = "HTML Format";
            const string TEXT_FORMAT = "UnicodeText";
            if (parameter.ClipboardData.GetDataPresent(HTML_FORMAT))
            {
                var htmlData = parameter.ClipboardData.GetData(HTML_FORMAT).ToString();
                var address = HtmlHelper.GetAddress(htmlData);
                var addressType = GetAddressType(address);
                Document = DocumentViewModel.Create(addressType, HtmlHelper.GetHtml(htmlData));
                Document.SourceAddress = address;

                string clipboardText = null;
                if (parameter.ClipboardData.GetDataPresent(TEXT_FORMAT))
                    clipboardText = parameter.ClipboardData.GetData(TEXT_FORMAT) as string;
                if (clipboardText == null)
                    return;

                if (addressType == AddressType.LentaArticle
                 || addressType == AddressType.LentaNews
                 || addressType == AddressType.LentaGallery
                 || addressType == AddressType.LentaRealtyGallery)
                {
                    using (var lineReader = new NonEmptyStringReader(clipboardText))
                    {
                        Document.Name = lineReader.ReadLine();
                        Document.Annotation = addressType == AddressType.LentaNews
                            ? "Новость"
                            : lineReader.ReadLine();
                        Document.Contents = lineReader.ReadToEnd();
                    }
                }
                else
                    Document.Contents = clipboardText;
                parameter.Processed = true;
            }
        }

        private AddressType GetAddressType(string address)
        {
            return GalleryAttribute.GetAddressType(address);
        }

        public DocumentViewModel Document { get; set; }
    }
}
