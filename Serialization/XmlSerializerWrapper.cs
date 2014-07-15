using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Win32.SafeHandles;

namespace MakePdf.Serialization
{
    public class XmlSerializerWrapper
    {
        private readonly XmlSerializer _serializer;

        public XmlSerializerWrapper(Type type, Type[] types)
        {
            _serializer = new XmlSerializer(type, types);
        }

        public XmlSerializerWrapper(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root,
            string defaultNamespace)
        {
            _serializer = new XmlSerializer(type, overrides, types, root, defaultNamespace);
        }


        public void Serialize(Stream s, object o)
        {
            var controller = o as ISerializationControl;
            if (controller != null)
                controller.Serializing();
            _serializer.Serialize(s, o);
            if (controller != null)
                controller.Serialized();
        }

        public object Deserialize(Stream s)
        {
            var result = _serializer.Deserialize(s);
            var controller = result as ISerializationControl;
            if (controller != null)
                controller.Deserialized();
            return result;
        }
    }
}
