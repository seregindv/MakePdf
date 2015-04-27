using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Annotations;
using System.Xml.Serialization;
using MakePdf.Stuff;

namespace MakePdf.Markup
{
    public interface IHasTags
    {
        List<Tag> Tags { set; get; }
    }

    [XmlInclude(typeof(BoldTag)),
     XmlInclude(typeof(ItalicTag)),
     XmlInclude(typeof(TextTag)),
     XmlInclude(typeof(HrefTag)),
     XmlInclude(typeof(ParagraphTag)),
     XmlInclude(typeof(ColorTag)),
     XmlInclude(typeof(ImageTag))]
    public class Tag : IHasTags
    {
        public List<Tag> Tags { set; get; }

        public virtual T Wrap<T>() where T : Tag, new()
        {
            return new T { Tags = this.ToTags() };
        }

        public virtual Tag Wrap(Tag with)
        {
            with.Tags = this.ToTags();
            return with;
        }

        public Tag DrillDown()
        {
            var result = this;
            while (result.Tags != null && result.Tags.Any())
                result = result.Tags[0];
            return result;
        }

        [XmlIgnore]
        public List<Tag> EnsuredTags
        {
            get { return Tags ?? (Tags = new List<Tag>()); }
        }
    }

    [XmlType(TypeName = "Paragraph")]
    public class ParagraphTag : Tag
    {
        [XmlAttribute]
        public string Color { set; get; }

        [XmlAttribute]
        public string BackgroundColor { set; get; }

        [XmlAttribute]
        public string Alignment { set; get; }
    }

    [XmlType(TypeName = "Bold")]
    public class BoldTag : Tag
    {
    }

    [XmlType(TypeName = "Italic")]
    public class ItalicTag : Tag
    {
    }

    [XmlType(TypeName = "Text")]
    public class TextTag : Tag
    {
        public TextTag()
        {
        }

        public TextTag(string value)
        {
            Value = value;
        }

        [XmlText]
        public string Value { set; get; }
    }

    [XmlType(TypeName = "Ref")]
    public class HrefTag : Tag
    {
        public HrefTag()
        {
        }

        public HrefTag(string address)
        {
            Address = address;
        }

        [XmlAttribute]
        public string Address { set; get; }
    }

    [XmlType(TypeName = "Color")]
    public class ColorTag : Tag
    {
        public ColorTag()
        {
        }

        public ColorTag(string color)
        {
            Value = color;
        }

        [XmlAttribute]
        public string Value { set; get; }
    }

    [XmlType(TypeName = "Image")]
    public class ImageTag : Tag
    {
        [XmlAttribute("Source")]
        public string ImageUrl { set; get; }
        [XmlAttribute("Href")]
        public string Url { set; get; }
        [XmlAttribute]
        public string Thumb { set; get; }
        [XmlAttribute]
        public int Width { set; get; }
        [XmlAttribute]
        public int Height { set; get; }
        [XmlAttribute]
        public string LocalPath { set; get; }
        [XmlAttribute]
        public int Index { set; get; }

        public string GetFormattedIndex(string format = "00000000")
        {
            return Utils.GetFileName(Index, format);
        }
    }
}
