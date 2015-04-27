using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace MakePdf.Galleries
{
    public abstract class ClipboardSourceHtmlGallery : HtmlGallery
    {
        protected override HtmlDocument Document
        {
            get
            {
                if (_document == null)
                    _document = CreateHtmlDocument(GalleryDocument.HtmlContents);
                return _document;
            }
        }
    }
}
