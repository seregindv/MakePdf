using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MakePdf.Helpers;

namespace MakePdf.Galleries.Processors
{
    public class LentaPictureProcessor
    {
        public LentaPictureProcessor(HtmlDocument document)
        {
            Document = document;
        }

        public HtmlDocument Document { set; get; }

        private GalleryItem PictureFromNode(HtmlNode node)
        {
            return new GalleryItem(node.GetSrc(),
                node.GetAttributeValue("alt", String.Empty),
                Int32.Parse(node.GetAttributeValue("width", String.Empty)),
                Int32.Parse(node.GetAttributeValue("height", String.Empty)));
        }

        public HtmlNodeCollection GetPictureNodes()
        {
            return Document.DocumentNode.SelectNodes("descendant::img[@width > 200]");
        }

        public GalleryItem GetMainPicture()
        {
            var node = Document.DocumentNode.SelectSingleNode("descendant::img[@class = 'g-picture']");
            return PictureFromNode(node);
        }

        public IEnumerable<GalleryItem> GetPictures()
        {
            var pictureNodes = GetPictureNodes();
            if (pictureNodes == null)
                return Enumerable.Empty<GalleryItem>();
            return pictureNodes.Select(PictureFromNode);
        }
    }
}
