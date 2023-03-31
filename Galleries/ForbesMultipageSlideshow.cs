using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public class ForbesMultipageSlideshow : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return GetItem(Document).ToEnumerable().Concat(GetPages().Select(GetItem));
        }

        private GalleryItem GetItem(HtmlDocument document)
        {
            var imgNode = document.DocumentNode.SelectSingleNode("descendant::li[@class='gallery-slide']/a/img");
            var textNode = document.DocumentNode.SelectSingleNode("descendant::div[contains(@class,'photoarticle')]");
            return new GalleryItem(imgNode.GetSrc(),
                HttpUtility.HtmlDecode(textNode.InnerText),
                imgNode.GetAttributeValue("width", Int32.MinValue),
                imgNode.GetAttributeValue("height", Int32.MinValue));
        }

        private IEnumerable<HtmlDocument> GetPages()
        {
            return Document.DocumentNode
                .SelectNodes("descendant::div[@class='wrapper']/ul/li[not(contains(@class,'current'))]/a")
                .Select((node, index) => GetHtmlDocument(node.GetHref(), PathHelper.GetFileName(index)));
        }
    }
}
