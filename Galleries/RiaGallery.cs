using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using HtmlAgilityPack;
using MakePdf.Markup;

namespace MakePdf.Galleries
{
    public class RiaGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {

            //class="main_items touchcarousel-container"
            //class="riaSlidesContainer touchcarousel-container main_items"
            return Document
                .CreateNavigator()
                .SelectDescendants("ul", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .Single(
                    @node =>
                        @node.GetAttribute("class", String.Empty) ==
                        "riaSlidesContainer touchcarousel-container main_items")
                .Select("li[@class='mainSlide touchcarousel-item']")
                .OfType<HtmlNodeNavigator>()
                .Select(@item =>
                    new GalleryItem(@item.GetAttribute("data-mainview", String.Empty))
                        {
                            ThumbnailImageUrl = @item.GetAttribute("data-preview", String.Empty),
                            Tags = GetTags(@item.GetAttribute("data-description", String.Empty))
                        });
        }

        private List<Tag> GetTags(string desc)
        {
            return Regex
                .Split(desc.Replace("\\", String.Empty), @"(&lt;br\s*\/?&gt;)+\s*")
                .Where((@token, @index) => @index % 2 == 0)
                .Select(tag => (Tag)TagFactory.GetParagraphTag(tag))
                .ToList();
        }
    }
}
