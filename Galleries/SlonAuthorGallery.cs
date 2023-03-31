using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using MakePdf.Markup;
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public class SlonAuthorGallery : ClipboardSourceHtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return Document.CreateNavigator()
                .SelectDescendants("a", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .Where(@node => @node.GetAttribute("class", String.Empty) == "b-article__title g-proxima")
                .Select(@node =>
                    new GalleryItem
                    {
                        Url = @node.GetHref()
                    })
                .Reverse();
        }

        public override void LoadItem(GalleryItem item)
        {
            var doc = GetItemHtmlDocument(item);
            var navigator = doc.CreateNavigator();
            var title = navigator
                .SelectDescendants("h1", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .First()
                .Value;
            item.Tags = TagFactory.GetTextTag(title).Wrap<BoldTag>().Wrap<ParagraphTag>().ToTags();
            var removePicSpan =
                navigator
                    .SelectDescendants("span", String.Empty, false)
                    .OfType<HtmlNodeNavigator>()
                    .FirstOrDefault(@node => @node.GetAttribute("class", String.Empty) == "remove_pic_class")
                ??
                navigator
                    .SelectDescendants("div", String.Empty, false)
                    .OfType<HtmlNodeNavigator>()
                    .FirstOrDefault(@node => @node.GetAttribute("id", String.Empty) == "content");

            var imageTag = removePicSpan
                .SelectDescendants("img", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .FirstOrDefault();
            if (imageTag != null)
            {
                item.ImageUrl = imageTag.GetAttribute("src", String.Empty);
                item.ImageUrl = new Uri(GalleryUri, item.ImageUrl).ToString();
            }
            var text = HttpUtility.HtmlDecode(removePicSpan.Value);
            item.Tags.AddRange(TagFactory.GetParagraphTags(text, true));
            try
            {
                base.LoadItem(item);
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if (response == null || response.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }
        }
    }
}
