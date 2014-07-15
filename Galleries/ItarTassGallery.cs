using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MakePdf.Markup;
using MakePdf.Sizing;

namespace MakePdf.Galleries
{
    public class ItarTassGallery : HtmlGallery
    {
        public ItarTassGallery()
        {
            HtmlEncoding = Encoding.UTF8;
        }

        protected override IEnumerable<GalleryItem> GetItems()
        {
            GalleryDocument.Annotation = HttpUtility.HtmlDecode(Document.DocumentNode.SelectSingleNode("descendant::span[@itemprop='description']").InnerText);

            GalleryDocument.Tags = TagFactory.GetParagraphTags(Document.DocumentNode.SelectNodes("descendant::div[@class='b-material-text__l']/*").Select(node => HttpUtility.HtmlDecode(node.InnerText)));

            var galleryItems = Document
                .DocumentNode
                .SelectNodes(
                    "descendant::div[@class='b-gallery__content']/descendant::div[@class='b-gallery-slider__item']/div[@class='b-gallery-item']/img");
            if (galleryItems != null)
                return galleryItems
                    .Select(node => new GalleryItem
                    {
                        ImageUrl = node.Attributes["src"].Value,
                        Size = new ImageSize(Int32.Parse(node.Attributes["width"].Value), Int32.Parse(node.Attributes["height"].Value)),
                        Tags = TagFactory.GetParagraphTag(HttpUtility.HtmlDecode(node.Attributes["alt"].Value)).ToTags()
                    });
            return Enumerable.Empty<GalleryItem>();
        }
    }
}
