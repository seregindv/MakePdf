using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MakePdf.Configuration
{
    public class GenericConfigurationSection<T> : ConfigurationSection
    {
        private readonly string _rootElementName;

        public GenericConfigurationSection()
        {
        }

        public GenericConfigurationSection(string rootElementName)
        {
            _rootElementName = rootElementName;
        }

        public T Value { private set; get; }

        protected override void DeserializeSection(XmlReader reader)
        {
            var serializer = CreateSerializer();
            Value = (T)serializer.Deserialize(reader);
        }

        private XmlSerializer CreateSerializer()
        {
            if (_rootElementName != null)
                return new XmlSerializer(typeof(T), new XmlRootAttribute(_rootElementName));
            return new XmlSerializer(typeof(T));
        }

        public static GenericConfigurationSection<T> Get(string sectionName = null)
        {
            if (sectionName == null)
            {
                var xmlType = typeof(T).GetCustomAttributes(typeof(XmlTypeAttribute), true);
                sectionName = xmlType.Length > 0
                    ? ((XmlTypeAttribute)xmlType[0]).TypeName
                    : typeof(T).Name;
            }
            return (GenericConfigurationSection<T>)ConfigurationManager.GetSection(sectionName);
        }
    }
}