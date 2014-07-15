using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace MakePdf.Galleries
{
    public class GazetaGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var scriptNode = Document
                .CreateNavigator()
                .SelectDescendants("script", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .FirstOrDefault(node => node.Value.IndexOf("pic_original") != -1);
            if (scriptNode == null)
                yield break;
            var matches = Regex.Matches(scriptNode.Value, @"pic_original:""(.+)""");
            foreach (Match match in matches)
            {
                yield return new GalleryItem(match.Groups[1].Value);
            }
        }
    }
}
