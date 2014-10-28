using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using System.Windows;
using System.Windows.Media.Converters;
using MakePdf.Stuff;

namespace MakePdf.Markup
{
    public static class TagFactory
    {
        private static IEnumerable<string> StringToLines(string s)
        {
            return s.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static ParagraphTag GetParagraphTag(string text, bool parseHyperlinks = false)
        {
            if (!parseHyperlinks)
                return GetTextTag(text).Wrap<ParagraphTag>();

            var result = new List<Tag>();
            var startPosition = 0;
            var matches = Utils.GetHyperlinkMatches(text);
            foreach (Match match in matches)
            {
                // from startPosition to Index
                if (startPosition != match.Index)
                    result.Add(GetTextTag(text.Substring(startPosition, match.Index - startPosition)));
                // link
                result.Add(GetTextTag(match.Groups[1].Value).Wrap(new HrefTag(match.Value)));
                // shift startPositionD:\LocalFO\c#\MakePdf\Stuff\DelegateCommand.cs
                startPosition += match.Index + match.Length;
            }
            if (startPosition < text.Length)
                result.Add(GetTextTag(text.Substring(startPosition, text.Length - startPosition)));
            return result.Wrap<ParagraphTag>();
        }

        public static List<Tag> GetParagraphTags(IEnumerable<string> lines, bool parseHyperlinks = false)
        {
            return lines.Select(line => (Tag)GetParagraphTag(line, parseHyperlinks)).ToList();
        }

        public static List<Tag> GetParagraphTags(string s, bool trim = false, bool parseHyperlinks = false)
        {
            var lines = StringToLines(s);
            if (trim)
                lines = lines.Select(Utils.Trim).Where(@line => @line.Length > 0);
            return GetParagraphTags(lines, parseHyperlinks);
        }

        public static TextTag GetTextTag(string s)
        {
            return new TextTag(s);
        }

        public static T Wrap<T>(this List<Tag> tags) where T : Tag, new()
        {
            return new T { Tags = tags };
        }

        public static Tag Wrap(this List<Tag> tags, Tag with)
        {
            with.Tags = tags;
            return with;
        }

        public static List<Tag> ToTags(this Tag tag)
        {
            return new List<Tag> { tag };
        }

        /*
        public static Tag DrillTo<T>(this Tag tag) where T : Tag
        {
            if (tag is T)
                return tag;
            if (Utils.IsNullOrEmpty(tag.Tags))
                return null;
            if(Utils.
        }*/
    }
}
