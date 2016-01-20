using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MakePdf.Stuff
{
    public static class HtmlUtils
    {
        public static string GetAddress(string htmlData)
        {
            using (var lineReader = new NonEmptyStringReader(htmlData))
            {
                var lentaEx = new Regex(@"^SourceURL.+?(https?:\/\/(www\.)?.+)$");
                string line;
                while ((line = lineReader.ReadLine()) != null)
                {
                    var match = lentaEx.Match(line);
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            return String.Empty;
        }

        public static string GetHtml(string htmlData)
        {
            using (var lineReader = new NonEmptyStringReader(htmlData))
            {
                string line;
                while ((line = lineReader.ReadLine()) != null)
                {
                    if (line.StartsWith("<"))
                        return line + Environment.NewLine + lineReader.ReadToEnd();
                }
            }
            return String.Empty;
        }

        public static HtmlDocument CreateHtmlDocument(string s = null)
        {
            var result = new HtmlDocument { OptionFixNestedTags = true };
            if (s != null)
                result.LoadHtml(s);
            return result;
        }
    }
}
