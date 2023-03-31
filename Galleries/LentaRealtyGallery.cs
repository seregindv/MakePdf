using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using MakePdf.Markup;
using MakePdf.Helpers;
using Newtonsoft.Json.Linq;

namespace MakePdf.Galleries
{
    public class LentaRealtyGallery : HtmlGallery
    {
        #region Overrides of HtmlGallery

        protected override IEnumerable<GalleryItem> GetItems()
        {
            GalleryDocument.Tags = TagFactory.GetParagraphTags(
                Document
                .DocumentNode
                .SelectSingleNode("descendant::div[@id='toptext']")
                .InnerText, true);
            var items = Document
                .DocumentNode
                .SelectNodes("descendant::table[@class='thumbs']/descendant::td/descendant::img[1]")
                .Select(@imgNode => new GalleryItem(imgNode.GetSrc()));
            var bigImageRegex = new Regex(@"\-\-\d+");
            return Regex.Matches(LoadString(GalleryUri.ToString(), GALLERY_HTML_FILE, Document.Encoding)
                , @"img.+src\=(.+?)\s+alt\=""(.+)""\s+title.+width.+height")
                .Cast<Match>()
                .Join(items, match => match.Groups[1].Value, item => item.ImageUrl
                , (match, item) =>
                {
                    item.ImageUrl = bigImageRegex.Replace(item.ImageUrl, String.Empty);
                    item.Tags = TagFactory.GetParagraphTag(match.Groups[2].Value).ToTags();
                    return item;
                });
        }

        #endregion
    }
}
