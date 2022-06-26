using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Xml.Xsl;
using MakePdf.Attributes;
using MakePdf.Configuration;
using MakePdf.Galleries;
using MakePdf.Markup;
using MakePdf.Pooling.Pools;
using MakePdf.Serialization;
using MakePdf.Stuff;
using MakePdf.XmlMakers;
using MakePdf.Galleries.Loaders;
using System.Xml;

namespace MakePdf.ViewModels
{
    [Serializable]
    [XmlRoot("Gallery")]
    public class GalleryDocumentViewModel : DocumentViewModel
    {
        public GalleryDocumentViewModel()
        {
            _totalCount = 1;
        }

        public Gallery Gallery { set; get; }

        public string HtmlContents { get; set; }

        private string GetFonetSafeFolderName(string name)
        {
            return Regex.Replace(name, @"\s*[\(\)]\s*", " ").Trim();
        }

        protected override void RenderDocument(string directory, CancellationToken ct)
        {
            try
            {
                Exception = null;

                Status = ProcessingStatus.Loading;
                if (Gallery == null)
                    Gallery = Gallery.Create(AddressType);
                Gallery.Load(this, Utils.GetFullPath(directory, GetFonetSafeFolderName(Name)));
#if DEBUG
                var galleryLoader = new SingleThreadedGalleryLoader(ct);
#else
                var galleryLoader = new ParallelGalleryLoader(ct);
#endif
                galleryLoader.GalleryItemLoaded += galleryLoader_GalleryItemLoaded;
                galleryLoader.LoadGallery(Gallery);

                if (ct.IsCancellationRequested)
                {
                    Status = ProcessingStatus.Cancelled;
                    return;
                }

                Status = ProcessingStatus.InProcess;
                using (var foStream = new MemoryStream())
                {
                    using (var serializedThis = new MemoryStream())
                    {
                        var ser = new XmlSerializerWrapper(GetType(), GalleryAttribute.GetTypes());
                        ser.Serialize(serializedThis, this);
                        if (Config.Instance.SaveIntermediateFiles)
                        {
                            serializedThis.Position = 0L;
                            using (
                                var fileStream =
                                    File.Create(Utils.GetPdfPath(Config.Instance.AppSettings["Directory"], Name, ".xml"))
                                )
                                Utils.CopyStream(serializedThis, fileStream);
                        }
                        var transform = new XslCompiledTransform(false);
                        transform.Load(Config.Instance.GetTemplate(TemplateType.Gallery));

                        serializedThis.Position = 0L;
                        using (var xmlReader = XmlReader.Create(serializedThis))
                            transform.Transform(xmlReader, new XsltArgumentList(), foStream);
                        if (Config.Instance.SaveIntermediateFiles)
                        {
                            foStream.Position = 0L;
                            using (
                                var fileStream =
                                    File.Create(Utils.GetPdfPath(Config.Instance.AppSettings["Directory"], Name, ".fo"))
                                )
                                Utils.CopyStream(foStream, fileStream);
                        }
                    }

                    using (var outStream = new FileStream(GetPdfFileName(directory), FileMode.Create))
                    using (var driver = _fonetDriverPool.FetchDisposable())
                    {
                        foStream.Position = 0L;
                        driver.Object.Render(foStream, outStream);
                    }
                }

                Status = ProcessingStatus.Complete;
            }
            catch (Exception ex)
            {
                Status = ProcessingStatus.Error;
                Exception = ex;
            }
        }

        void galleryLoader_GalleryItemLoaded(object sender, GalleryItemLoadedEventArgs e)
        {
            LoadedCount = e.LoadedCount;
            TotalCount = e.TotalCount;
        }

        public override DocumentViewModel Clone()
        {
            var result = new GalleryDocumentViewModel();
            CloneTo(result);
            return result;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "Status")
                IsLoading = Status == ProcessingStatus.Loading;
            base.OnPropertyChanged(propertyName);
        }

        private bool _isLoading;
        [XmlIgnore]
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        private int _totalCount;
        [XmlIgnore]
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                if (_totalCount != value)
                {
                    _totalCount = value;
                    OnPropertyChanged("TotalCount");
                }
            }
        }

        private int _loadedCount;
        [XmlIgnore]
        public int LoadedCount
        {
            get { return _loadedCount; }
            set
            {
                if (_loadedCount != value)
                {
                    _loadedCount = value;
                    OnPropertyChanged("LoadedCount");
                }
            }
        }

        public override void CloneTo(DocumentViewModel document)
        {
            base.CloneTo(document);
            ((GalleryDocumentViewModel)document).Gallery = Gallery;
            ((GalleryDocumentViewModel)document).HtmlContents = HtmlContents;
        }
    }
}
