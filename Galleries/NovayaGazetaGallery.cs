using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace MakePdf.Galleries
{
    public class NovayaGazetaGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return Document
                .CreateNavigator()
                .SelectDescendants("a", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .Where(@node =>
                {
                    var attr = @node.GetAttribute("onclick", String.Empty);
                    return attr != null && attr.StartsWith("return showPhotoGallery");
                })
                .Select(@node =>
                    {
                        var match = Regex.Match(@node.GetAttribute("onClick", String.Empty), @"\('(\d+)', '(\d+)'\)");
                        var url =
                            String.Format(
                                @"http://www.novayagazeta.ru/ajax/photo/item.html?gallery_id={0}&photo_id={1}",
                                match.Groups[1].Value, match.Groups[2].Value);
                        return new GalleryItem
                        {
                            Url = url,
                            ThumbnailImageUrl = @node.SelectSingleNode("img").GetAttribute("src", String.Empty)
                        };
                    });
        }

        public override void LoadItem(GalleryItem item)
        {
            var jsonString = LoadString(item.Url, item.GetFormattedIndex() + ".json");
            var json = JObject.Parse(jsonString);
            item.ImageUrl = new Uri(GalleryUri, json.GetValue("img").Value<string>()).ToString();
            base.LoadItem(item);
        }
    }
}
