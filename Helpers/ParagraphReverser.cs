using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MakePdf.Helpers
{
    public class ParagraphReverser
    {
        private IEnumerable<string> GetLines(string s)
        {
            string line;
            using (var sr = new StringReader(s))
                while ((line = sr.ReadLine()) != null)
                    yield return line;
        }

        public string ReverseParagraphs(string text)
        {
            var paragraphStartRegEx = new Regex(@"^\d{1,2}:\d{2}\b");
            var paragraphs = new List<string>();
            var nonReversableCount = 0;
            foreach (var line in GetLines(text))
            {
                var lastParagraphIndex = paragraphs.Count - 1;
                if (paragraphStartRegEx.Matches(line).Count > 0)
                    paragraphs.Add(line);
                else
                {
                    if (lastParagraphIndex == -1)
                    {
                        paragraphs.Add(line);
                        nonReversableCount = 1;
                    }
                    else
                        paragraphs[lastParagraphIndex] = paragraphs[lastParagraphIndex]
                                                         + Environment.NewLine
                                                         + line;
                }
            }
            return String.Join(Environment.NewLine, paragraphs.Take(nonReversableCount)
                .Concat(paragraphs.Skip(nonReversableCount).Reverse()));
        }
    }
}
