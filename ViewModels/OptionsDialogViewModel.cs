using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakePdf.Configuration;
using MakePdf.Dialog;
using Microsoft.Practices.ServiceLocation;

namespace MakePdf.ViewModels
{
    public class OptionsDialogViewModel : ModalDialogViewModel
    {
        private IConfig _config;

        public OptionsDialogViewModel()
        {
            _config = ServiceLocator.Current.GetInstance<IConfig>();
        }

        public string Path
        {
            set { _config.AppSettings["Directory"] = value; }
            get { return _config.AppSettings["Directory"]; }
        }

        public bool OpenExplorer
        {
            set { _config.AppSettings["OpenExplorer"] = value.ToString(); }
            get { return Boolean.Parse(_config.AppSettings["OpenExplorer"]); }
        }

        public Device[] Devices
        {
            get { return _config.DeviceConfiguration.Devices; }
        }

        public Device DefaultDevice
        {
            get
            {
                return
                    _config.DeviceConfiguration.Devices.FirstOrDefault(
                        device => device.Name == _config.DeviceConfiguration.DefailtDeviceName);
            }
            set { _config.DeviceConfiguration.DefailtDeviceName = value.Name; }
        }

        public string TextGallerySplit
        {
            set { _config.AppSettings["TextGallerySplit"] = value; }
            get { return _config.AppSettings["TextGallerySplit"]; }
        }

        public bool SaveIntermediateFiles
        {
            set { _config.AppSettings["SaveIntermediateFiles"] = value.ToString(); }
            get { return Boolean.Parse(_config.AppSettings["SaveIntermediateFiles"]); }
        }

        public bool AddDeviceToPdfName
        {
            set { _config.AppSettings["AddDeviceToPdfName"] = value.ToString(); }
            get { return Boolean.Parse(_config.AppSettings["AddDeviceToPdfName"]); }
        }
    }
}
