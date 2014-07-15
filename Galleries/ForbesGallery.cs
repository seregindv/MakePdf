using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace MakePdf.Galleries
{
    public class ForbesGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var fullImageAddress = Document
                .CreateNavigator()
                .SelectDescendants("li", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .First(itemsNode => itemsNode.GetAttribute("class", String.Empty) == "gallery-slide")
                .SelectSingleNode("a/img")
                .GetAttribute("src", String.Empty);

            return Document
                .CreateNavigator()
                .SelectDescendants("div", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .Where(itemsNode => itemsNode.GetAttribute("class", String.Empty) == "wrapper")
                .SelectMany(itemsNode => itemsNode
                    .SelectDescendants("img", String.Empty, false)
                    .OfType<HtmlNodeNavigator>()
                    .Select(imgNode =>
                        {
                            var src = imgNode.GetAttribute("src", String.Empty);
                            var imagePath =
                                Path.Combine(Path.GetDirectoryName(fullImageAddress), Path.GetFileName(src))
                                    .Replace('\\', '/').Replace("http:/", "http://");
                            return new GalleryItem(imagePath,
                                            imgNode.GetAttribute("title", String.Empty),
                                            Int32.MinValue, Int32.MinValue,
                                            src);
                        }
                    )
                );
        }
    }
}
