using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using MakePdf.Markup;

namespace MakePdf.Galleries
{
    public class PhotoshareGallery : HtmlGallery
    {
        public PhotoshareGallery()
        {
            HtmlEncoding = Encoding.UTF8;
        }

        protected override IEnumerable<GalleryItem> GetItems()
        {
            var navNode = Document.DocumentNode.SelectSingleNode("//iframe[@id='album_nav']");
            var navUri = navNode.GetAttributeValue("src", String.Empty);
            var navUrl = new Uri(GalleryUri, navUri).ToString();
            var navDoc = LoadHtmlFile(navUrl, "album_nav.html");

            return navDoc.DocumentNode.SelectNodes("//a").Select(
                node => new GalleryItem { Url = node.GetAttributeValue("href", String.Empty) });
        }

        public override void LoadItem(GalleryItem item)
        {
            var doc = GetItemHtmlDocument(item);
            var node = doc.DocumentNode.SelectSingleNode("//a[@itemprop='contentURL']");
            item.ImageUrl = node.GetAttributeValue("href", String.Empty);
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@itemprop='name']");
            var title = titleNode.InnerText;
            if (!Regex.IsMatch(title, @"^\s*(IMG|DSC)_?\d+\s*$"))
            {
                title = HttpUtility.HtmlDecode(title);
                item.Tags = TagFactory.GetTextTag(title).Wrap<ParagraphTag>().ToTags();
            }
            base.LoadItem(item);
        }
    }
}
