using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MakePdf.Helpers
{
    public static class StringHelper
    {
        static readonly Regex _splitRegex = new Regex(@"\b(?:(?:https?|ftp|file)://|www\.|ftp\.)([\w\.-]+)(?:\([-A-Z0-9+&@#/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#/%=~_|$?!:,.]*\)|[A-Z0-9+&@#/%=~_|$])", RegexOptions.IgnoreCase);

        public static string GetColorAsString(string r, string g, string b)
        {
            return "#" + (Byte.Parse(r) << 16 |
                   Byte.Parse(g) << 8 |
                   Byte.Parse(b)).ToString("X6");

        }
        public static MatchCollection GetHyperlinkMatches(string s)
        {
            return _splitRegex.Matches(s);
        }

        public static string InsertOrDeleteAtLineStart(string s, string text, int start, int length, bool delete = false)
        {
            var result = s;
            var pos = Math.Min(start + length, s.Length - 1);
            var IsRN = (Func<char, bool>)(c => c == '\r' || c == '\n');
            var skipRN = IsRN(result[pos]);
            if (length > 0 && IsRN(result[pos - 1]))
            {
                skipRN = true;
                --pos;
            }
            var exitAfterInsert = false;
            while (true)
            {
                if (skipRN)
                {
                    if (!IsRN(result[pos]) || pos == 1)
                        skipRN = false;
                }
                else if (IsRN(result[pos]) || pos == 0)
                {
                    var picTextPos = pos == 0 ? pos : pos + 1;
                    var isPic = SafeSubstring(result, picTextPos, text.Length) == text;
                    if (delete)
                    {
                        if (isPic)
                            result = result.Remove(picTextPos, text.Length);
                    }
                    else
                    {
                        if (!isPic)
                            result = result.Insert(picTextPos, text);
                    }
                    if (exitAfterInsert || pos == 0)
                        break;
                    skipRN = true;
                }
                --pos;
                if (pos <= start)
                    exitAfterInsert = true;
            }
            return result;
        }

        public static string SafeSubstring(string s, int startIndex, int length)
        {
            if (s.Length <= startIndex)
                return String.Empty;
            if (startIndex + length >= s.Length)
                length = s.Length - startIndex;
            return s.Substring(startIndex, length);
        }

        public static string SubstringAfter(string s, string after)
        {
            var afterIndex = s.IndexOf(after);
            return afterIndex == -1 ? s : s.Substring(0, afterIndex);
        }

        public static string Trim(string s)
        {
            return s.Replace("&nbsp;", String.Empty).Trim(' ', '\r', '\n', '\t');
        }

        public static string TruncateAfterEOL(string s)
        {
            var index = s.IndexOf('\n');
            if (index >= 0)
                return s.Substring(0, index);
            return s;
        }
    }
}
