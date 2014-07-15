using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MakePdf.Markup
{
    public class TagList : IList<Tag>
    {
        Regex _splitRegex = new Regex(@"\b(?:(?:https?|ftp|file)://|www\.|ftp\.)([\w\.-]+)(?:\([-A-Z0-9+&@#/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#/%=~_|$?!:,.]*\)|[A-Z0-9+&@#/%=~_|$])", RegexOptions.IgnoreCase);

        List<Tag> _container = new List<Tag>();

        public IEnumerator<Tag> GetEnumerator()
        {
            return _container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _container.GetEnumerator();
        }

        public void JustAdd(Tag item)
        {
            _container.Add(item);
        }

        public void Add(Tag item)
        {
            var textItem = item as TextTag;
            if (textItem != null)
            {
            }
            _container.Add(item);
        }

        private void ProcessTextTags(TextTag tag, IHasTags parent)
        {
            var startPosition = 0;
            int tagIndex = 0;
            var matches = _splitRegex.Matches(tag.Value);
            if (matches.Count > 0)
            {
                tagIndex = parent.Tags.IndexOf(tag);
                if (tagIndex == -1)
                    throw new ArgumentException("tag is not child of parent");
                parent.Tags.RemoveAt(tagIndex);
            }
            foreach (Match match in matches)
            {
                // from startPosition to Index
                if (startPosition != match.Index)
                {
                    parent.Tags.Insert(tagIndex,
                        TagFactory.GetTextTag(tag.Value.Substring(startPosition, match.Index - startPosition)));
                    ++tagIndex;
                }
                // link
                parent.Tags.Insert(tagIndex, TagFactory.GetTextTag(match.Groups[1].Value).Wrap(new HrefTag(match.Value)));
                ++tagIndex;
                // shift startPosition
                startPosition += match.Index + match.Length;
            }
            if (startPosition < tag.Value.Length)
                parent.Tags.Insert(tagIndex, TagFactory.GetTextTag(tag.Value.Substring(startPosition, tag.Value.Length - startPosition)));
        }

        public void Clear()
        {
            _container.Clear();
        }

        public bool Contains(Tag item)
        {
            return _container.Contains(item);
        }

        public void CopyTo(Tag[] array, int arrayIndex)
        {
            _container.CopyTo(array, arrayIndex);
        }

        public bool Remove(Tag item)
        {
            return _container.Remove(item);
        }

        public int Count
        {
            get
            {
                return _container.Count;
            }
        }

        public bool IsReadOnly { get { return false; } }

        public int IndexOf(Tag item)
        {
            return _container.IndexOf(item);
        }

        public void Insert(int index, Tag item)
        {
            _container.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _container.RemoveAt(index);
        }

        public Tag this[int index]
        {
            get { return _container[index]; }
            set { _container[index] = value; }
        }
    }
}
