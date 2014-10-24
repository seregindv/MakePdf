using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MakePdf.Configuration
{
    public interface IConfig
    {
        DeviceArray DeviceConfiguration { get; }
        Device SelectedDevice { set; get; }
        NameValueCollection AppSettings { get; }
        bool SaveIntermediateFiles { get; }
        decimal ScreenWidth { get; }
        decimal ScreenHeight { get; }
        bool AddDeviceToPdfName { get; }
        bool TabletAutoOnScreenKeyboard { get; }

        string GetTemplate(TemplateType type);
    }
}
