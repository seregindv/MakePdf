using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace MakePdf.Configuration
{
    public class Config : IConfig
    {
        public Config()
        {
            DeviceConfiguration = GenericConfigurationSection<DeviceArray>.Get().Value;
            if (DeviceConfiguration != null)
                SelectedDevice =
                    DeviceConfiguration.Devices.FirstOrDefault(
                        device => device.Name == DeviceConfiguration.DefailtDeviceName);
        }

        #region IConfig

        public DeviceArray DeviceConfiguration { private set; get; }

        public NameValueCollection AppSettings
        {
            get
            {
                return ConfigurationManager.AppSettings;
            }
        }

        private Device _selectedDevice;
        private decimal _screenHeight;
        private decimal _screenWidth;

        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                ScreenWidth = Decimal.MinusOne;
                ScreenHeight = Decimal.MinusOne;
            }
        }

        public decimal ScreenHeight
        {
            get
            {
                EnsureScreenDimensions();
                return _screenHeight;
            }
            private set { _screenHeight = value; }
        }

        public decimal ScreenWidth
        {
            get
            {
                EnsureScreenDimensions();
                return _screenWidth;
            }
            private set { _screenWidth = value; }
        }

        public string GetTemplate(TemplateType type)
        {
            if (SelectedDevice != null && SelectedDevice.Templates != null)
            {
                var template = SelectedDevice.Templates.FirstOrDefault(tpl => tpl.Type == type);
                if (template != null)
                    return template.Path;
            }
            return AppSettings[type + "Template"];
        }

        public bool SaveIntermediateFiles
        {
            get
            {
                return ToBoolean("SaveIntermediateFiles");
            }
        }

        public bool AddDeviceToPdfName
        {
            get
            {
                return ToBoolean("AddDeviceToPdfName");
            }
        }

        #endregion

        private bool ToBoolean(string s)
        {
            return String.Compare(AppSettings[s], "true", StringComparison.OrdinalIgnoreCase) == 0
                || AppSettings[s] == "1";
        }

        private void EnsureScreenDimensions()
        {
            if ((_screenHeight != Decimal.MinusOne && _screenWidth != Decimal.MinusOne)
                || SelectedDevice == null
                || SelectedDevice.ResolutionX == 0
                || SelectedDevice.ResolutionY == 0
                || SelectedDevice.Diagonal == 0)
                return;
            var pixelsPerMillimeter =
                Math.Sqrt(SelectedDevice.ResolutionX * SelectedDevice.ResolutionX +
                          SelectedDevice.ResolutionY * SelectedDevice.ResolutionY) / 25.4 / (double)SelectedDevice.Diagonal;
            ScreenWidth = Decimal.Round((decimal)(SelectedDevice.ResolutionX / pixelsPerMillimeter), 2);
            ScreenHeight = Decimal.Round((decimal)(SelectedDevice.ResolutionY / pixelsPerMillimeter), 2);
        }

        private static IConfig _instance;

        public static IConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Config();
                return _instance;
            }
        }
    }
}
