﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MakePdf.Attributes;
using MakePdf.Configuration;
using MakePdf.Controls;
using MakePdf.Galleries.Processors;
using MakePdf.Helpers;
using System.Linq;
using System.Windows;

namespace MakePdf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IDataErrorInfo
    {
        private IConfig _config;
        private CancellationTokenSource _tokenSource;

        public MainWindowViewModel(IConfig config)
        {
            _config = config;
            UriHelper.FixUriTrailingDotBug();
            Clear();
            Documents = new ObservableCollection<DocumentViewModel>();
            Documents.CollectionChanged += Documents_CollectionChanged;
            SkipEmptyLines = true;
            AddCommand = new DelegateCommand(OnAdd, _ => IsContentPresent() || IsTitlePresent());
            RenderCommand = new DelegateCommand(OnRender, _ => !DirectoryError);
            RemoveCommand = new DelegateCommand(OnRemove, IsDocumentSelected);
            ChangeCommand = new DelegateCommand(OnChange, IsDocumentSelected);
            SaveCommand = new DelegateCommand(OnSave, _ => !DirectoryError);
            LoadCommand = new DelegateCommand(OnLoad, _ => !DirectoryError);
            PasteCommand = new DelegateCommand(OnPaste);
            ClearCommand = new DelegateCommand(OnClear);
            RemoveAllCommand = new DelegateCommand(OnRemoveAll, _ => Documents.Count > 0);
            ReverseParagraphsCommand = new DelegateCommand(OnReverseParagraphs, IsContentPresent);
            PictureCommand = new DelegateCommand(OnPicture);
            UnpictureCommand = new DelegateCommand(OnUnpicture);
            RenderFilesCommand = new DelegateCommand(OnRenderFiles);
            DropCommand = new DelegateCommand(OnDrop);
            OpenExplorer = Boolean.Parse(_config.AppSettings["OpenExplorer"]);
            IsTablet = WpfHelper.IsTablet;

            AddMenuItems = new List<MenuItemViewModel> {
                new MenuItemViewModel(AddCommand)
                {
                    AddressType = null,
                    IsDefault = true,
                    Title = "Auto"
                }
            };
            AddMenuItems.AddRange(
                (from addressType in GalleryAttribute.GetDecorated()
                 let groupName = GalleryAttribute.GetGroup(addressType)
                 where groupName != null
                 select groupName)
                .Distinct()
                .Select(groupName => new MenuItemViewModel(AddCommand)
                {
                    Group = groupName,
                    Title = groupName
                })
            );
            AddMenuItems.AddRange(
                from addressType in GalleryAttribute.GetDecorated()
                let title = GalleryAttribute.GetTitle(addressType)
                where title != null && GalleryAttribute.GetGroup(addressType) == null
                select new MenuItemViewModel(AddCommand)
                {
                    AddressType = @addressType,
                    Title = title
                }
            );
            DeviceConfiguration = _config.DeviceConfiguration;
            SelectedDevice = _config.SelectedDevice;
            if (Environment.OSVersion.Version.Major > 6 && _config.TabletAutoOnScreenKeyboard)
            {
                try
                {
                    WpfHelper.DisableWPFTabletSupport();
                    WpfHelper.EnableFocusTracking();
                }
                catch { }
            }
        }

        private void OnPicture(object obj)
        {
            PictureUnpicture(false);
        }

        private void OnUnpicture(object obj)
        {
            PictureUnpicture(true);
        }

        private void PictureUnpicture(bool unpicture)
        {
            if (DisplayedDocument != null && DisplayedDocument.Contents != null)
                DisplayedDocument.Contents = StringHelper.InsertOrDeleteAtLineStart(DisplayedDocument.Contents, "pic:", SelectionStart, SelectionLength, unpicture);
            //Debug.WriteLine(Utils.InsertOrDeleteAtLineStart(DisplayedDocument.Contents, "pic:", SelectionStart, SelectionLength, unpicture));
        }

        void Documents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseCanExecuteChanged(RemoveAllCommand);
        }

        public DelegateCommand AddCommand { private set; get; }
        public DelegateCommand RenderCommand { private set; get; }
        public DelegateCommand RemoveCommand { private set; get; }
        public DelegateCommand ChangeCommand { private set; get; }
        public DelegateCommand SaveCommand { private set; get; }
        public DelegateCommand LoadCommand { private set; get; }
        public DelegateCommand PasteCommand { private set; get; }
        public DelegateCommand ClearCommand { private set; get; }
        public DelegateCommand RemoveAllCommand { private set; get; }
        public DelegateCommand ReverseParagraphsCommand { private set; get; }
        public DelegateCommand PictureCommand { private set; get; }
        public DelegateCommand UnpictureCommand { private set; get; }
        public DelegateCommand RenderFilesCommand { private set; get; }
        public DelegateCommand DropCommand { private set; get; }
        public List<MenuItemViewModel> AddMenuItems { private set; get; }
        public string Directory
        {
            set
            {
                _config.AppSettings["Directory"] = value;
            }
            get
            {
                return _config.AppSettings["Directory"];
            }
        }

        private bool _directoryError;
        public bool DirectoryError
        {
            set
            {
                if (_directoryError == value)
                    return;
                _directoryError = value;
                RaiseCanExecuteChanged(SaveCommand);
                RaiseCanExecuteChanged(LoadCommand);
                RaiseCanExecuteChanged(RenderCommand);
            }
            get { return _directoryError; }
        }

        public DeviceArray DeviceConfiguration { private set; get; }

        private Device _selectedDevice;
        public Device SelectedDevice
        {
            set
            {
                if (_selectedDevice == value)
                    return;
                _selectedDevice = value;
                _config.SelectedDevice = _selectedDevice;
                OnPropertyChanged("SelectedDevice");
            }
            get
            {
                return _selectedDevice;
            }
        }

        private ObservableCollection<DocumentViewModel> _documents;
        public ObservableCollection<DocumentViewModel> Documents
        {
            private set
            {
                var renderChanged = _documents != null;
                _documents = value;
                OnPropertyChanged("Documents");
                if (renderChanged)
                    RaiseCanExecuteChanged(RenderCommand);
                RaiseCanExecuteChanged(RemoveAllCommand);
            }
            get { return _documents; }
        }

        private DocumentViewModel _displayedDocument;
        public DocumentViewModel DisplayedDocument
        {
            set
            {
                if (_displayedDocument != value)
                {
                    if (_displayedDocument != null)
                        _displayedDocument.PropertyChanged -= DisplayedDocument_PropertyChanged;
                    _displayedDocument = value;
                    DisplayedDocumentChanged();
                    _displayedDocument.PropertyChanged += DisplayedDocument_PropertyChanged;
                    OnPropertyChanged("DisplayedDocument");
                }
            }
            get { return _displayedDocument; }
        }

        private DocumentViewModel _selectedDocument;
        public DocumentViewModel SelectedDocument
        {
            set
            {
                var raiseCanExecuteChanged = (_selectedDocument == null || value == null) && _selectedDocument != value;
                _selectedDocument = value;
                if (IsDocumentSelected())
                    DisplayedDocument = SelectedDocument.Clone();
                else
                    Clear();
                if (raiseCanExecuteChanged)
                {
                    OnPropertyChanged("SelectedDocument");
                    RaiseCanExecuteChanged(ChangeCommand);
                    RaiseCanExecuteChanged(RemoveCommand);
                }
            }
            get { return _selectedDocument; }
        }

        public int SelectionStart { get; set; }
        public int SelectionLength { get; set; }

        public bool IsTablet { get; private set; }

        void DisplayedDocument_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Contents")
                DisplayedDocumentChanged();
            if (e.PropertyName == "Name")
                RaiseCanExecuteChanged(AddCommand);
        }

        void DisplayedDocumentChanged()
        {
            RaiseCanExecuteChanged(AddCommand);
            RaiseCanExecuteChanged(ReverseParagraphsCommand);
        }

        private void Clear()
        {
            if (IsDocumentSelected())
                SelectedDocument = null;
            DisplayedDocument = new TextDocumentViewModel();
        }

        public bool AllContentSelected { set; get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        private ProcessingStatus _dragDropActionStatus;
        public ProcessingStatus DragDropActionStatus
        {
            set
            {
                _dragDropActionStatus = value;
                OnPropertyChanged("DragDropActionStatus");
            }
            get { return _dragDropActionStatus; }
        }

        public bool SkipEmptyLines { set; get; }

        public bool OpenExplorer { set; get; }

        private void OnRemove(object data)
        {
            if (IsDocumentSelected())
            {
                Documents.Remove(SelectedDocument);
                Clear();
            }
        }

        private void OnRemoveAll(object data)
        {
            Documents.Clear();
            Clear();
        }

        private void OnChange(object data)
        {
            if (IsDocumentSelected())
            {
                DisplayedDocument.CloneTo(SelectedDocument);
                Clear();
            }
        }

        private void OnAdd(object data)
        {
            var menuItem = data as MenuItemViewModel;
            AddressType? addressType = null;
            if (menuItem != null)
            {
                if (menuItem.AddressType != null)
                    addressType = menuItem.AddressType;
                else if (menuItem.Group != null)
                    addressType = GalleryAttribute.GetAddressType(DisplayedDocument.SourceAddress, menuItem.Group);
            }
            AddItem(addressType);
        }

        private void AddItem(AddressType? addressType)
        {
            if (String.IsNullOrWhiteSpace(DisplayedDocument.Name))
            {
                using (var reader = new NonEmptyStringReader(DisplayedDocument.Contents))
                {
                    DisplayedDocument.Name = reader.ReadLine();
                    DisplayedDocument.Contents = reader.ReadToEnd();
                }
            }
            DocumentViewModel newDocument;
            var displayedIsGallery = GalleryAttribute.IsGallery(DisplayedDocument.AddressType);

            if (addressType == null)
                newDocument = displayedIsGallery
                    ? DisplayedDocument.Clone()
                    : DisplayedDocumentToGallery(AddressType.TextGallery);
            else
                if (!GalleryAttribute.IsGallery(addressType.Value))
                if (displayedIsGallery)
                {
                    newDocument = new TextDocumentViewModel();
                    DisplayedDocument.CloneTo(newDocument);
                }
                else
                    newDocument = DisplayedDocument.Clone();
            else
                    if (displayedIsGallery)
            {
                newDocument = DisplayedDocument.Clone();
                newDocument.AddressType = addressType.Value;
            }
            else
                newDocument = DisplayedDocumentToGallery(addressType.Value);
            newDocument.Exception = null;
            newDocument.Status = ProcessingStatus.New;
            Documents.Add(newDocument);
            Clear();
        }

        private DocumentViewModel DisplayedDocumentToGallery(AddressType type)
        {
            var result = new GalleryDocumentViewModel();
            DisplayedDocument.CloneTo(result);
            result.AddressType = type;
            return result;
        }

        private void OnRender(object data)
        {
            if (IsBusy)
            {
                if (!_tokenSource.IsCancellationRequested)
                    _tokenSource.Cancel();
                return;
            }
            IsBusy = true;
            Task.Run(Render).ContinueWith(_ =>
            {
                if (OpenExplorer)
                    ExecuteExplorer();
                IsBusy = false;
            });
        }

        private void OnSave(object data)
        {
            try
            {
                using (var stream = new FileStream(PathHelper.GetFullPath(Directory, "articles.xml"), FileMode.Create))
                    CreateSerializer().Serialize(stream, Documents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnLoad(object data)
        {
            using (var stream = new FileStream(PathHelper.GetFullPath(Directory, "articles.xml"), FileMode.Open))
            {
                Documents = (ObservableCollection<DocumentViewModel>)CreateSerializer().Deserialize(stream);
                Documents.CollectionChanged += Documents_CollectionChanged;
            }
        }

        private void OnPaste(object data)
        {
            if (!((String.IsNullOrWhiteSpace(DisplayedDocument.Contents) || AllContentSelected) && String.IsNullOrWhiteSpace(DisplayedDocument.Name) && String.IsNullOrWhiteSpace(DisplayedDocument.Annotation)))
                return;
            var parameter = data as PasteParameter;
            if (parameter == null)
                return;
            var pasteProcessor = new PasteProcessor();
            pasteProcessor.ProcessPaste(parameter);
            if (parameter.Processed)
                DisplayedDocument = pasteProcessor.Document;
        }

        private void OnClear(object data)
        {
            Clear();
        }

        private void OnRenderFiles(object data)
        {
            var files = data as string[];
            if (files == null)
                return;
            DragDropActionStatus = ProcessingStatus.InProcess;
            Task.Factory.StartNew(() => files.AsParallel().ForAll(file =>
WpfHelper.GetFonetDriver().Render(file, file + ".pdf")), TaskCreationOptions.PreferFairness)
                .ContinueWith(t =>
                {
                    try
                    {
                        t.Wait();
                        DragDropActionStatus = ProcessingStatus.Complete;
                    }
                    catch (AggregateException ex)
                    {
                        DragDropActionStatus = ProcessingStatus.Error;
                        var contents = DisplayedDocument.Contents;
                        DisplayedDocument.Contents = ex.InnerExceptions.Aggregate(new StringBuilder(contents),
                            (sb, exc) =>
                            {
                                sb.AppendLine(exc.Message);
                                if (exc.InnerException != null)
                                    sb.AppendLine(exc.InnerException.Message);
                                return sb;
                            },
                            b => b.ToString());
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnDrop(object data)
        {
            var dataArray = data as object[];
            if (dataArray == null)
                return;
            foreach (var datum in dataArray)
            {
                var datumArray = datum as object[];
                if (datumArray == null)
                    continue;
                var datumType = datumArray[0] as string;
                if (datumType == DataFormats.FileDrop)
                    OnDropFiles(datumArray[1] as string[]);
                else if (datumType == "HTML Format")
                    OnDropHtml(datumArray[1] as string);
            }
        }

        private void OnDropHtml(string htmlData)
        {
            DisplayedDocument.Contents = HtmlHelper
                .CreateHtmlDocument(HtmlHelper.GetHtml(htmlData))
                .DocumentNode
                .SelectNodes("//img")
                .Select(node => "pic:" + node.GetSrc())
                .Aggregate(new StringBuilder(DisplayedDocument.Contents ?? String.Empty), (sb, s) => sb.AppendLine(s), sb => sb.ToString());
        }

        private void OnDropFiles(string[] files)
        {
            if (files == null)
                return;
            var processor = new FileListProcessor();
            var fileSets = processor.Process(files);
            var setCount = 0;
            foreach (var fileSet in fileSets)
            {
                if (setCount != 0)
                    AddItem(null);
                DisplayedDocument.Contents = fileSet.Files
                    .Aggregate(new StringBuilder(DisplayedDocument.Contents ?? String.Empty),
                        (sb, s) => sb.AppendLine("pic:" + s),
                        sb => sb.ToString());
                if (!String.IsNullOrEmpty(fileSet.Title) && !String.IsNullOrEmpty(DisplayedDocument.Contents))
                    DisplayedDocument.Name = fileSet.Title;
                ++setCount;
            }
            if (setCount > 1)
                AddItem(null);
        }

        private void OnReverseParagraphs(object data)
        {
            DisplayedDocument.Contents = new ParagraphReverser().ReverseParagraphs(DisplayedDocument.Contents);
        }

        private XmlSerializer CreateSerializer()
        {
            return new XmlSerializer(typeof(ObservableCollection<DocumentViewModel>)
                , new XmlAttributeOverrides()
                , GalleryAttribute.GetTypes()
                , new XmlRootAttribute("Documents"), String.Empty);
        }

        private object Render()
        {
            _tokenSource = new CancellationTokenSource();
            try
            {
#if DEBUG
                Documents.AsParallel()
                    .WithDegreeOfParallelism(1)
                    .WithCancellation(_tokenSource.Token)
                    .ForAll(doc => doc.Render(Directory, _tokenSource.Token));
#else
                Documents.AsParallel()
                    .WithCancellation(_tokenSource.Token)
                    .ForAll(doc => doc.Render(Directory, _tokenSource.Token));
#endif
            }
            catch (OperationCanceledException)
            {
            }
            return null;
        }

        private bool IsDocumentSelected(object _ = null)
        {
            return SelectedDocument != null;
        }

        private bool IsContentPresent(object _ = null)
        {
            return DisplayedDocument != null && !String.IsNullOrWhiteSpace(DisplayedDocument.Contents);
        }

        private bool IsTitlePresent(object _ = null)
        {
            return DisplayedDocument != null && !String.IsNullOrWhiteSpace(DisplayedDocument.Name);
        }

        private void ExecuteExplorer()
        {
            Process.Start(Directory);
        }

        public string Error
        {
            get { return String.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == "Directory")
                    if (!System.IO.Directory.Exists(Directory))
                    {
                        result = "Directory doesn't exist";
                        DirectoryError = true;
                    }
                    else
                        DirectoryError = false;
                return result;
            }
        }
    }
}