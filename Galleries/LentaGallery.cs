using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Web;

namespace MakePdf.Galleries
{
    public class LentaGallery : InterviewBase
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
                    var tags = GetTags(json.GetValue("credits").Value<string>(),
                        json.GetValue("caption").Value<string>(),
                        json.GetValue("alt").Value<string>());
                    return new GalleryItem(json.GetValue("url").Value<string>(),
                        tags,
                        json.GetValue("width").Value<int>(),
                        json.GetValue("height").Value<int>(),
                        imgNode.GetAttributeValue("src", String.Empty));
                });
        }

        protected override GalleryItem GetMainItem(HtmlDocument document)
        {
            throw new NotImplementedException();
        }

        protected override HtmlNodeNavigator GetNavigator(HtmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
