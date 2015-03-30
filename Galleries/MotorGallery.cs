using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MakePdf.Galleries
{
    public class MotorGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var galleryItemsNode = Document.DocumentNode.SelectSingleNode("//script[contains(.,'gallery_images')]");
            var jsonMatch = Regex.Match(galleryItemsNode.InnerText, @"gallery_images\=(.+);");
            var jsonToken = JToken.Parse(jsonMatch.Groups[1].Value);

            return jsonToken.Children().Select(child =>
            {
                var imageUrl = new Uri(GalleryUri, child.SelectToken("versions.original.rel_url").Value<string>()).ToString();
                var thumbUrl = new Uri(GalleryUri, child.SelectToken("versions.thumbnail.rel_url").Value<string>()).ToString();
                return new GalleryItem(imageUrl,
                    child.SelectToken("caption").Value<string>(),
                    child.SelectToken("versions.original.width").Value<int>(),
                    child.SelectToken("versions.original.height").Value<int>(),
                    thumbUrl);
            });
        }
    }
}
