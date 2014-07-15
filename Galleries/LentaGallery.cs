using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Web;

namespace MakePdf.Galleries
{
    public class LentaGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return Document
                .DocumentNode
                .SelectNodes("descendant::div[@class='items']/descendant::img")
                .Select(imgNode =>
                {

                    var jsonNode = imgNode.GetAttributeValue("data-image", String.Empty);
                    jsonNode = HttpUtility.HtmlDecode(jsonNode);
                    var json = JObject.Parse(jsonNode);
                    return new GalleryItem(json.GetValue("url").Value<string>(),
                        json.GetValue("caption").Value<string>() + Environment.NewLine +
                        json.GetValue("alt").Value<string>(),
                        json.GetValue("width").Value<int>(),
                        json.GetValue("height").Value<int>(),
                        imgNode.GetAttributeValue("src", String.Empty));
                });
        }
    }
}
