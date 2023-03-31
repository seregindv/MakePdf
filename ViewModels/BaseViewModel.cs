using System.ComponentModel;
using MakePdf.Helpers;

namespace MakePdf.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseCanExecuteChanged(DelegateCommand command)
        {
            if (command != null)
                command.RaiseCanExecuteChanged();
        }
    }
}
