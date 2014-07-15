using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MakePdf.Stuff;

namespace MakePdf.Galleries
{
    public class MkGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            var videoContainer = GetVideoContainer(Document);
            var pages = GetPages(videoContainer);
            return GetItems(videoContainer)
                .Concat(pages.SelectMany((@page, @num) =>
                    GetItems(GetVideoContainer(GetHtmlDocument(@page, Utils.GetFileName(@num))))));
        }

        private IEnumerable<GalleryItem> GetItems(HtmlNodeNavigator videoContainerNode)
        {
            return videoContainerNode
                .Select("div[@class='img']/a/img")
                .OfType<HtmlNodeNavigator>()
                .Select(@node =>
                    {
                        var thumbnailImageUrl = @node.GetAttribute("src", String.Empty);
                        thumbnailImageUrl = new Uri(GalleryUri, thumbnailImageUrl).ToString();
                        return new GalleryItem
                        {
                            ThumbnailImageUrl = thumbnailImageUrl,
                            ImageUrl = thumbnailImageUrl.Replace("90_", "495_")
                        };
                    });
        }

        private HtmlNodeNavigator GetVideoContainer(HtmlDocument document)
        {
            return document
                .CreateNavigator()
                .SelectDescendants("div", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .First(@node => @node.GetAttribute("id", String.Empty) == "videoContainer");

        }

        private IEnumerable<string> GetPages(HtmlNodeNavigator videoContainerNode)
        {
            return videoContainerNode
                .Select("div[@class='MainPager']/a[not(@style)]")
                .OfType<HtmlNodeNavigator>()
                .Select(@node =>
                    new Uri(GalleryUri, @node.GetAttribute("href", String.Empty)).ToString());
        }
    }
}
