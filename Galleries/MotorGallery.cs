using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MakePdf.Markup;
using MakePdf.Stuff;

namespace MakePdf.Galleries
{
    public class MotorGallery : HtmlGallery
    {
        protected override IEnumerable<GalleryItem> GetItems()
        {
            GalleryDocument.Name = Document
                .DocumentNode
                .SelectSingleNode("(descendant::div[@class='b-h1']/h1/span)[1]")
                .InnerText;
            GalleryDocument.Annotation = Utils.TruncateAfterEOL(Document
                .DocumentNode
                .SelectSingleNode("descendant::p[@class='two-words']")
                .InnerText);
            var bigPicsText = Document
                .DocumentNode
                .SelectSingleNode("descendant::div[@class='l-col l-col_x1 l-col_w6-5']/descendant::script")
                .InnerText;
            var bigPics = Regex.Matches(bigPicsText, @"url_1600\:\s*""(.+)"",")
                .OfType<Match>().Select(@match => @match.Groups[1].Value);
            return Document.CreateNavigator()
                .SelectDescendants("div", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .First(
                    @node =>
                        @node.GetAttribute("class", String.Empty) ==
                        "b-galleria b-galleria_big js-galleria b-galleria_start")
                .Select("a")
                .OfType<HtmlNodeNavigator>()
                .Select(@node =>
                {
                    var @imgNode = @node.SelectSingleNode("img");
                    var result = new GalleryItem
                    {
                        //ImageUrl = Utils.FixUrlProtocol(@node.GetAttribute("href", String.Empty)),
                        ThumbnailImageUrl = Utils.FixUrlProtocol(@imgNode.GetAttribute("src", String.Empty))
                    };
                    var text = @imgNode.GetAttribute("alt", String.Empty);
                    if (text.Length > 0)
                        result.Tags = TagFactory.GetParagraphTag(text).ToTags();
                    return result;
                }).Zip(bigPics, (@galleryItem, @bigPic) =>
                {
                    @galleryItem.ImageUrl = Utils.FixUrlProtocol(@bigPic);
                    return @galleryItem;
                });
        }
    }
}
