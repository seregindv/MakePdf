using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MakePdf.Stuff;

namespace MakePdf.Dialog
{
    public class ModalDialogViewModel:INotifyPropertyChanged
    {
        private bool _isVisible;
        public DelegateCommand OkCommand { set; get; }
        public DelegateCommand CancelCommand { set; get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsVisible
        {
            set
            {
                if (value.Equals(_isVisible)) return;
                _isVisible = value;
                OnPropertyChanged();
            }
            get { return _isVisible; }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
