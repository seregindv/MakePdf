using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakePdf.Markup;
using MakePdf.Helpers;

namespace MakePdf.Galleries
{
    public class InterfaxGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            return Document
                .DocumentNode
                .SelectNodes("//div[@class='psBlock']//a")
                .Select(node => new GalleryItem
                {
                    Url = GetUrlFromRelativePath(node.GetHref()),
                    ThumbnailImageUrl = GetUrlFromRelativePath(node.SelectSingleNode("img").GetSrc())
                });
        }

        public override void LoadItem(GalleryItem item)
        {
            var doc = GetItemHtmlDocument(item);
            var imgNode = doc.DocumentNode.SelectSingleNode("//img[@itemprop='contentUrl']");
            item.ImageUrl = GetUrlFromRelativePath(imgNode.GetSrc());
            item.Tags = TagFactory.GetParagraphTag(imgNode.GetAttributeValue("alt", String.Empty)).ToTags();
            base.LoadItem(item);
        }
    }
}
