using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MakePdf.Configuration
{
    [XmlType("devices")]
    public class DeviceArray
    {
        [XmlAttribute("default")]
        public string DefailtDeviceName { set; get; }
        [XmlElement("device")]
        public Device[] Devices { set; get; }
    }

    public class Device
    {
        private string _fileSuffix;

        [XmlAttribute("name")]
        public string Name { set; get; }
        [XmlAttribute("file-suffix")]
        public string FileSuffix
        {
            set
            {
                _fileSuffix = value;
            }
            get
            {
                return _fileSuffix ?? Name;
            }
        }
        [XmlArray("templates")]
        public Template[] Templates { set; get; }
        [XmlAttribute("x")]
        public int ResolutionX { set; get; }
        [XmlAttribute("y")]
        public int ResolutionY { set; get; }
        [XmlAttribute("diagonal")]
        public decimal Diagonal { set; get; }
    }

    /*var devices = new DeviceArray
    {
        Devices = new Device
        {
            Name = "iPod",
            Templates = new[]
            {
                new Template
                {
                    Type=TemplateType.Article,
                    Path= "Templates\\iPodTextTemplate.xml"
                }
            }
        }
    };
    var ser = new XmlSerializer(typeof(DeviceArray));
    ser.Serialize(System.IO.File.Create("d:\\deviceArray.xml"), devices);*/
}
