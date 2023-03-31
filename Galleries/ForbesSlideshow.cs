using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using MakePdf.Markup;
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public class ForbesSlideshow : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return Document
                    .CreateNavigator()
                    .SelectDescendants("div", String.Empty, false)
                    .OfType<HtmlNodeNavigator>()
                    .First(@node =>
                        {
                            var @attr = @node.GetAttribute("class", String.Empty);
                            return @attr != null && @attr.StartsWith("wrapper");
                        })
                    .Select("ul/li")
                    .OfType<HtmlNodeNavigator>()
                    .Select(@node =>
                        {
                            var @aNode = @node.SelectSingleNode("a");
                            var @imgNode = @node.SelectSingleNode("a/img");
                            return new GalleryItem
                                {
                                    ThumbnailImageUrl = @imgNode.GetAttribute("src", String.Empty),
                                    Url = @aNode.GetHref()
                                };
                        });
        }

        public override void LoadItem(GalleryItem item)
        {
            var doc = GetItemHtmlDocument(item);
            item.ImageUrl = doc.CreateNavigator()
                .SelectDescendants("a", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .First(@node => @node.GetAttribute("class", String.Empty) == "fancybox")
                .GetHref();
            item.Tags = TagFactory.GetParagraphTags(HttpUtility.HtmlDecode(doc.CreateNavigator()
                .SelectDescendants("div", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .First(@node =>
                {
                    var classAttr = @node.GetAttribute("class", String.Empty);
                    return classAttr != null && classAttr.StartsWith("photoarticle");
                })
                .Value));
            base.LoadItem(item);
        }
    }
}
