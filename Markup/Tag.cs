using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows.Annotations;
using System.Xml.Serialization;

namespace MakePdf.Markup
{

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
}
