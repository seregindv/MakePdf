using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Fonet;
using Fonet.Render.Pdf;
using MakePdf.Attributes;
using MakePdf.Configuration;
using MakePdf.Markup;
using MakePdf.Serialization;
using MakePdf.Helpers;
using MakePdf.Pooling;
using MakePdf.Pooling.Pools;
using MakePdf.XmlMakers;
using MakePdf.Galleries;

namespace MakePdf.ViewModels
{
    [XmlInclude(typeof(GalleryDocumentViewModel))]
    [XmlInclude(typeof(TextDocumentViewModel))]
    public abstract class DocumentViewModel : BaseViewModel, ISerializationControl, IHasTags
    {
        //static readonly DelegateThreadedPool<FonetDriver> _fonetDriverPool = new DelegateThreadedPool<FonetDriver>(Utils.GetFonetDriver);
        //static readonly DelegateThreadedPool<Pdfer> _pdferPool = new DelegateThreadedPool<Pdfer>(() => new Pdfer(Config.Instance.AppSettings["Template"]));

        protected static readonly ObjectPool<FonetDriver> _fonetDriverPool = ObjectPool<FonetDriver>.Get(WpfHelper.GetFonetDriver);

        protected DocumentViewModel()
        {
            Status = ProcessingStatus.New;
        }

        public AddressType AddressType { set; get; }

        public decimal ScreenWidth { set; get; }
        public decimal ScreenHeight { set; get; }
        public string Date { set; get; }

        private string _name;
        public string Name
        {
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
            get { return _name; }
        }

        private string _annotation;
        public string Annotation
        {
            set
            {
                _annotation = value;
                OnPropertyChanged("Annotation");
            }
            get { return _annotation; }
        }

        private string _sourceAddress;
        public string SourceAddress
        {
            set
            {
                _sourceAddress = value;
                OnPropertyChanged("SourceAddress");
            }
            get { return _sourceAddress; }
        }

        public string SourceName { set; get; }

        private string _contents;
        public string Contents
        {
            set
            {
                _contents = value;
                OnPropertyChanged("Contents");
            }
            get { return _contents; }
        }

        public bool SkipEmptyLines { set; get; }

        [XmlIgnore]
        public IList<Exception> Exceptions { private set; get; }
        [XmlIgnore]
        public Exception Exception
        {
            set
            {
                if (value == null)
                    Exceptions = null;
                else
                {
                    var exception = value as AggregateException;
                    Exceptions = exception != null
                        ? (IList<Exception>)exception.InnerExceptions
                        : new[] { value };
                }
                OnPropertyChanged("Exceptions");
            }
        }

        private ProcessingStatus _status;

        public ProcessingStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private string _savedContents;
        private bool _swapped;

        public List<Tag> Tags { set; get; }

        #region ISerializationController members

        public virtual void Serializing()
        {
            _swapped = !String.IsNullOrEmpty(Contents) && (Tags == null || Tags.Count == 0);
            if (!_swapped)
                return;
            _savedContents = Contents;
            Tags = TagFactory.GetParagraphTags(Contents, true);
            Contents = null;
        }

        public virtual void Serialized()
        {
            if (!_swapped)
                return;
            Tags = null;
            Contents = _savedContents;
            _savedContents = null;
        }

        public void Deserialized()
        {
        }

        #endregion


        public void Render(string directory, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                Status = ProcessingStatus.Cancelled;
                return;
            }
            ScreenWidth = Config.Instance.ScreenWidth;
            ScreenHeight = Config.Instance.ScreenHeight;
            Date = DateTime.Now.ToString("G");
            if (String.IsNullOrEmpty(SourceName) && !String.IsNullOrEmpty(SourceAddress))
            {
                var matches = StringHelper.GetHyperlinkMatches(SourceAddress);
                if (matches.Count > 0 && matches[0].Groups[1].Value.Length > 0)
                    SourceName = matches[0].Groups[1].Value;
                else
                    SourceName = SourceAddress;
            }
            RenderDocument(directory, ct);
        }

        protected abstract void RenderDocument(string directory, CancellationToken ct);

        public abstract DocumentViewModel Clone();

        public virtual void CloneTo(DocumentViewModel document)
        {
            document.Annotation = Annotation;
            document.Contents = Contents;
            document.Exceptions = Exceptions;
            document.Name = Name;
            document.SkipEmptyLines = SkipEmptyLines;
            document.SourceAddress = SourceAddress;
            document.Status = Status;
            document.AddressType = AddressType;
        }

        public static DocumentViewModel Create(AddressType addressType, string htmlContents)
        {
            DocumentViewModel result;
            /*switch (addressType)
            {
                case AddressType.LentaGallery:
                case AddressType.LentaRealtyGallery:
                case AddressType.ForbesGallery:
                case AddressType.GazetaGallery:
                case AddressType.ForbesSlideshow:
                case AddressType.NovayaGazeta:
                case AddressType.MkGallery:
                    result = new GalleryDocumentViewModel();
                    break;
                default:
                    result = new TextDocumentViewModel();
                    break;
            }*/
            result = GalleryAttribute.IsGallery(addressType)
                                ? (DocumentViewModel)new GalleryDocumentViewModel { HtmlContents = htmlContents }
                                : new TextDocumentViewModel();
            result.AddressType = addressType;
            return result;
        }

        protected string GetPdfFileName(string directory)
        {
            var name = Config.Instance.AddDeviceToPdfName
                ? String.Concat(Name, "[", Config.Instance.SelectedDevice.FileSuffix, "]")
                : Name;
            return PathHelper.GetPdfPath(directory, name);
        }
    }

    public enum ProcessingStatus
    {
        New,
        Loading,
        [Description("Cooking PDF")]
        InProcess,
        Complete,
        Error,
        Cancelled
    }
}
