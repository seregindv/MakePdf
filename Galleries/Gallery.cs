using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using MakePdf.Attributes;
using MakePdf.Sizing;
using MakePdf.Stuff;
using MakePdf.ViewModels;

namespace MakePdf.Galleries
{
    public abstract class Gallery
    {
        protected Uri GalleryUri { set; get; }
        protected GalleryDocumentViewModel GalleryDocument { set; get; }
        protected bool _galleryFolderCreated;
        protected string _galleryFolder;
        protected string GalleryFolder
        {
            set
            {
                _galleryFolder = value;
            }
            get
            {
                if (!_galleryFolderCreated)
                {
                    Directory.CreateDirectory(_galleryFolder);
                    _galleryFolderCreated = true;
                }
                return _galleryFolder;
            }
        }
        public GalleryItem[] Items { set; get; }

        protected void SetSize(GalleryItem item, Stream fileStream = null)
        {
            if (item.Size != null && item.Size.Height != 0 && item.Size.Width != 0)
                return;
            try
            {
                if (fileStream != null)
                {
                    item.Size = ImageSize.FromStream(fileStream);
                    return;
                }
            }
            catch { }
            item.Size = ImageSize.FromFile(item.LocalPath);
        }

        public void Load(GalleryDocumentViewModel galleryDocument, string galleryFolder)
        {
            GalleryDocument = galleryDocument;
            if (!String.IsNullOrEmpty(galleryDocument.SourceAddress))
                GalleryUri = new Uri(galleryDocument.SourceAddress);
            GalleryFolder = galleryFolder;
            if (Items == null)
                Items = GetItems().ToArray();
            for (var i = 0; i < Items.Length; ++i)
                Items[i].Index = i;
        }

        public virtual void LoadItem(GalleryItem item)
        {
            if (String.IsNullOrEmpty(item.ImageUrl))
                return;
            var uri = new Uri(item.ImageUrl);
            if (uri.Scheme == Uri.UriSchemeFile)
            {
                item.LocalPath = item.ImageUrl;
                SetSize(item);
            }
            else
            {
                if (String.IsNullOrEmpty(item.LocalPath))
                    item.LocalPath = Path.Combine(GalleryFolder,
                        item.GetFormattedIndex() + Utils.TrimIllegalChars(Path.GetExtension(item.ImageUrl)));
                if (File.Exists(item.LocalPath))
                {
                    SetSize(item);
                }
                else
                {
                    var responseStream = GetResponseStream(item.ImageUrl);
                    using (var fileStream = new FileStream(item.LocalPath, FileMode.Create))
                    {
                        Utils.CopyStream(responseStream, fileStream);
                        SetSize(item, fileStream);
                    }
                }
            }
        }

        protected abstract IEnumerable<GalleryItem> GetItems();

        public static Gallery Create(AddressType addressType)
        {
            return GalleryAttribute.CreateGallery(addressType);
        }

        public Stream GetResponseStream(string url)
        {
            var request = WebRequest.Create(url);
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            /*request.Proxy = new WebProxy
            {
                Address = new Uri("http://202.29.97.2:3128"),
                BypassProxyOnLocal = true//,
                //Credentials = CredentialCache.DefaultCredentials
            };*/
            var response = request.GetResponse();
            var webResponse = response as HttpWebResponse;
            var responseStream = response.GetResponseStream();
            if (responseStream == null)
                return null;

            if (webResponse != null && webResponse.ContentEncoding == "gzip")
            {
                var gz = new GZipStream(responseStream, CompressionMode.Decompress);
                return gz;
            }
            return responseStream;
        }

        public string GetUrlFromRelativePath(string path)
        {
            return new Uri(GalleryUri, path).ToString();
        }
    }
}
