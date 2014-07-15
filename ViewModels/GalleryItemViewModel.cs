using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using MakePdf.Galleries;
using Newtonsoft.Json.Linq;

namespace MakePdf.ViewModels
{
    [Obsolete("Just not used")]
    public class GalleryItemViewModel : BaseViewModel
    {
        public string Url { set; get; }

        private Image _image;
        public Image Image
        {
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged("Image");
                }
            }
            get { return _image; }
        }

        private bool _loaded;
        public bool Loaded
        {
            set
            {
                if (_loaded != value)
                {
                    _loaded = value;
                    OnPropertyChanged("Loaded");
                }
            }
            get { return _loaded; }
        }

        public string Text { set; get; }
    }
}
