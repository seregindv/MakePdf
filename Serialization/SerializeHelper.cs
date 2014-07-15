using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MakePdf.Serialization
{
    public class SerializeHelper
    {
        public static void SerializeToFile<T>(T o, string f)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var fs = new FileStream(f, FileMode.Create))
                ser.Serialize(fs, o);
        }

        public static string SerializeToString<T>(T o)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var s = new MemoryStream())
            using (var sr = new StreamReader(s))
            {
                ser.Serialize(s, o);
                s.Position = 0L;
                return sr.ReadToEnd();
            }
        }
    }
}
