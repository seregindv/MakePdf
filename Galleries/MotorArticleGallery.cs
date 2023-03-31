using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Xml.XPath;
using HtmlAgilityPack;
using MakePdf.Markup;
using MakePdf.Helpers;
using MakePdf.ViewModels;

namespace MakePdf.Galleries
{
    public class MotorArticleGallery : InterviewBase
    {
        List<GalleryItem> _items = new List<GalleryItem>();
        List<Tag> _tags = new List<Tag>();

        protected override IEnumerable<GalleryItem> GetItems()
        {
            var result = GetItemsV1();
            if (result == null)
            {
                HtmlEncoding = Encoding.UTF8;
                ReloadDocument();
                result = GetItemsV2();
            }
            return result;
        }

        #region V2

        private IEnumerable<GalleryItem> GetItemsV2()
        {
            GalleryDocument.Name = Document
                .DocumentNode
                .SelectSingleNode(@"//meta[@property='og:title']")
                .GetAttributeValue("content", String.Empty);
            GalleryDocument.Annotation = Document
                .DocumentNode
                .SelectSingleNode(@"//meta[@property='og:description']")
                .GetAttributeValue("content", String.Empty);
            return base.GetItems();
        }

        protected override GalleryItem GetMainItem(HtmlDocument document)
        {
            return new GalleryItem(Document
                .DocumentNode
                .SelectSingleNode(@"//meta[@property='og:image']")
                .GetAttributeValue("content", String.Empty));
        }

        protected override HtmlNodeNavigator GetNavigator(HtmlDocument document)
        {
            return (HtmlNodeNavigator)document.DocumentNode.SelectSingleNode("//p").ParentNode.CreateNavigator();
        }

        protected override bool ProcessGalleryNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name != "div")
                return base.ProcessGalleryNode(node, tags);

            var nodeClass = node.GetAttributeValue("class", String.Empty);
            if (!nodeClass.StartsWith("widget "))
                return base.ProcessGalleryNode(node, tags);

            var imgNodes = node.SelectNodes(".//img");
            if (imgNodes == null)
                return base.ProcessGalleryNode(node, tags);

            var isRight = nodeClass.EndsWith("widget--right");

            foreach (var imgNode in imgNodes)
            {
                var galleryItem = new GalleryItem(imgNode.GetSrc());
                tags.Add(galleryItem);
                var pNodes = imgNode.SelectNodes("./following-sibling::*");
                if (pNodes != null && pNodes.Count > 0)
                    GetItems((HtmlNodeNavigator)pNodes.First().CreateNavigator(), galleryItem);
                if (isRight)
                    galleryItem.EnsuredTags.Add(TagFactory.GetTextTag("* * *").Wrap(new ParagraphTag { Alignment = "center" }));
            }

            return true;
        }

        protected override NodeProcessResult ProcessTextNode(HtmlNode node, List<Tag> tags)
        {
            if (node.Name == "img" || node.GetAttributeValue("class", String.Empty) == "article__adverts-container")
                return new NodeProcessResult(true, false);
            if (node.Name == "div" && node.GetAttributeValue("class", String.Empty).StartsWith("adverts--hide"))
                return new NodeProcessResult(true, false);
            return base.ProcessTextNode(node, tags);
        }

        #endregion

        private IEnumerable<GalleryItem> GetItemsV1()
        {
            var navigator = Document.CreateNavigator();
            var titleNode = navigator
                .SelectDescendants("div", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .SingleOrDefault(@node => @node.GetAttribute("class", String.Empty) == "header");
            if (titleNode == null)
                return null;
            var h1 = titleNode.SelectDescendants("h1", String.Empty, false).OfType<HtmlNodeNavigator>().First();
            var summary = h1.SelectSingleNode("span[@class='summary']");
            if (summary == null)
            {
                var name = StringHelper.TruncateAfterEOL(
StringHelper.Trim(
                            h1.SelectChildren(XPathNodeType.All)
                              .OfType<HtmlNodeNavigator>()
                              .Where(@node => @node.Name == "#text" || @node.Name == "br")
                              .Aggregate(new StringBuilder(), (@sb, @node) =>
                              {
                                  var val = StringHelper.Trim(@node.Value);
                                  if (val.Length > 0)
                                  {
                                      @sb.Append(val);
                                      @sb.Append(" ");
                                  }
                                  return @sb;
                              }, @sb => @sb.ToString().Trim())));
                if (name.Length > 0)
                    GalleryDocument.Name = name;
                else
                    GalleryDocument.Name = StringHelper.Trim(StringHelper.SubstringAfter(Document.DocumentNode.SelectSingleNode("html/head/title").InnerText, "-"));
            }
            else
                GalleryDocument.Name = summary.Value;
            GalleryDocument.Annotation = StringHelper.Trim(titleNode.SelectDescendants("p", String.Empty, false).OfType<HtmlNodeNavigator>().First().Value);

            var headerImage = Document.DocumentNode.SelectSingleNode(@"//div[contains(@class,'image_first')]/img");
            if (headerImage != null)
                _items.Add(new GalleryItem(
                    headerImage.GetSrc(),
                    null,
                    headerImage.GetAttributeValue("width", 0),
                    headerImage.GetAttributeValue("height", 0)));

            var contentNavigator = navigator
                .SelectDescendants("span", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .Single(@node => @node.GetAttribute("class", String.Empty) == "description");
            contentNavigator.MoveToFirstChild();
            do
            {
                switch (contentNavigator.CurrentNode.Name)
                {
                    case "p":
                        _tags.Add(TagFactory.GetParagraphTag(contentNavigator.Value));
                        break;
                    case "h3":
                        _tags.Add(TagFactory.GetTextTag(contentNavigator.Value).Wrap<BoldTag>().Wrap<ParagraphTag>());
                        break;
                    case "div":
                        var className = contentNavigator.GetAttribute("class", String.Empty);
                        switch (className)
                        {
                            case "b-slideshow-incut":
                                AddImages(contentNavigator);
                                break;
                            case "image image_incut":
                                AddImageWithDescription(contentNavigator);
                                break;
                            case "image":
                                if (!AddImage(contentNavigator))
                                    AddVideoLink(contentNavigator);
                                break;
                            case "note note_dark":
                                AddTable(contentNavigator);
                                break;
                            case "tech":
                                AddTechTable(contentNavigator);
                                break;
                            case "g-align-center":
                            case "question":
                                _tags.Add(TagFactory.GetTextTag(contentNavigator.CurrentNode.InnerText).Wrap<BoldTag>().Wrap<ParagraphTag>());
                                break;
                            case "incut":
                                AddIncut(contentNavigator);
                                break;
                            default:
                                _tags.Add(TagFactory.GetParagraphTag(contentNavigator.CurrentNode.InnerText));
                                break;
                        }
                        break;
                }
            } while (contentNavigator.MoveToNext());
            Flush();
            return _items;
        }

        private void Flush()
        {
            if (_tags.Count == 0)
                return;
            var currentItem = _items.LastOrDefault();
            if (currentItem == null)
                GalleryDocument.Tags = _tags;
            else
            {
                if (CollectionHelper.IsNullOrEmpty(currentItem.Tags))
                    currentItem.Tags = _tags;
                else
                    currentItem.Tags.AddRange(_tags);
            }
            _tags = new List<Tag>();
        }

        private void AddImages(HtmlNodeNavigator contentNavigator)
        {
            Flush();
            _items.AddRange(contentNavigator
                .CurrentNode
                .SelectNodes("div[@class='b-slideshow-incut__photos-list']/div")
                .Select(
                    @node =>
                    {
                        var textNode = @node.SelectSingleNode("div/div");
                        return
                            new GalleryItem(
UriHelper.FixUrlProtocol(@node.SelectSingleNode("img").GetSrc()),
                                textNode == null ? null : @node.SelectSingleNode("div/div").InnerText
                                );
                    }
                )
             );
        }

        private bool AddImage(HtmlNodeNavigator contentNavigator)
        {
            var imageNode = contentNavigator.CurrentNode.SelectSingleNode("img[@width or @class='photo']");
            if (imageNode == null)
                return false;
            Flush();
            var imageSource =
                imageNode.GetSrc();
            var item = new GalleryItem(UriHelper.FixUrlProtocol(imageSource));
            var captionNode = contentNavigator.CurrentNode.SelectSingleNode("div[@class='caption']");
            if (captionNode != null)
                item.Tags = TagFactory.GetParagraphTags(captionNode.InnerText);
            _items.Add(item);
            return true;
        }

        private void AddVideoLink(HtmlNodeNavigator contentNavigator)
        {
            var videoNode = contentNavigator.CurrentNode.SelectSingleNode("iframe[@class='youtube-player' and @src]");
            if (videoNode == null)
                return;
            var tubeID = videoNode.GetSrc();
            var address = Regex.Replace(tubeID, @"http:\/\/(?:www\.)?youtube\.com\/embed\/(.+?)(?:\/|\?|$).*", "http://www.youtube.com/watch?v=$1");
            _tags.Add(TagFactory.GetTextTag("Видео>>").Wrap(new HrefTag(address)).Wrap<ParagraphTag>());
        }

        private void AddImageWithDescription(HtmlNodeNavigator contentNavigator)
        {
            Flush();
            var imageSource =
                contentNavigator.CurrentNode.SelectSingleNode("img")
                    .GetSrc();
            var lines = contentNavigator
                .CurrentNode
                .SelectNodes("div/div/p")
                .Select(@node => @node.InnerText)
                .Where(@text => !String.IsNullOrEmpty(@text));
            var item = new GalleryItem(UriHelper.FixUrlProtocol(imageSource), TagFactory.GetParagraphTags(lines));
            _items.Add(item);
        }

        private void AddTable(HtmlNodeNavigator contentNavigator)
        {
            _tags.Add(TagFactory.GetParagraphTag(contentNavigator.SelectSingleNode("h2").Value));
            Flush();
            _items.AddRange(
                contentNavigator
                .CurrentNode
                .SelectNodes("div/div")
                .Select(@node =>
                    new GalleryItem(@node.SelectSingleNode("div/a").GetHref(),
                        @node.SelectSingleNode("div/p").InnerText))
                );
        }

        private void AddTechTable(HtmlNodeNavigator contentNavigator)
        {
            _tags.Add(TagFactory.GetParagraphTag(contentNavigator.SelectSingleNode("h2").Value));
            Flush();
            _tags.AddRange(
                contentNavigator
                .CurrentNode
                .SelectNodes("table/tbody/tr")
                .Select(@node =>
                    TagFactory.GetParagraphTag(
                    String.Join(" = ", @node.SelectNodes("td").Select(@tdNode => @tdNode.InnerText)))));
        }

        private void AddIncut(HtmlNodeNavigator contentNavigator)
        {
            var div = contentNavigator.CurrentNode.SelectSingleNode("div");
            var header = div.SelectSingleNode("h2").InnerText;
            var text = div.SelectSingleNode("p").InnerText;
            _tags.Add(TagFactory.GetTextTag(header).Wrap<BoldTag>().Wrap<ParagraphTag>());
            _tags.Add(TagFactory.GetParagraphTag(text));
        }
    }
}
