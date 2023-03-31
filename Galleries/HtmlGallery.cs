using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using MakePdf.Attributes;
using MakePdf.Sizing;
using MakePdf.Helpers;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace MakePdf.Galleries
{
    public abstract class HtmlGallery : Gallery
    {
        protected const string GALLERY_HTML_FILE = "gallery.html";

        protected Encoding HtmlEncoding { set; get; }

        protected HtmlDocument _document;
        protected virtual HtmlDocument Document
        {
            get
            {
                if (_document == null)
                    _document = LoadHtmlFile(GalleryUri.ToString(), GALLERY_HTML_FILE);
                return _document;
            }
        }

        protected void ReloadDocument()
        {
            _document = null;
        }

        protected string LoadFile(string url, string fileName)
        {
            var file = Path.Combine(GalleryFolder, fileName);
            if (!File.Exists(file))
            {
                using (var fileStream = new FileStream(file, FileMode.Create))
                    StreamHelper.CopyStream(GetResponseStream(url), fileStream);
            }
            return file;
        }

        protected string LoadString(string url, string fileName, Encoding encoding = null)
        {
            var file = LoadFile(url, fileName);
            return encoding == null ? File.ReadAllText(file) : File.ReadAllText(file, encoding);
        }

        protected HtmlDocument LoadHtmlFile(string url, string fileName)
        {
            var file = LoadFile(url, fileName);
            var result = HtmlHelper.CreateHtmlDocument();
            if (HtmlEncoding == null)
                result.DetectEncodingAndLoad(file);
            else
                result.Load(file, HtmlEncoding);
            return result;
        }

        protected HtmlDocument GetItemHtmlDocument(GalleryItem item)
        {
            return GetHtmlDocument(item.Url, item.GetFormattedIndex());
        }

        protected HtmlDocument GetHtmlDocument(string url, string fileName)
        {
            return LoadHtmlFile(url, fileName + ".html");
        }
    }
}
