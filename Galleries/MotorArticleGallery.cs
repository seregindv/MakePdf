using System;
using System.Collections.Generic;
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
using MakePdf.Stuff;
using MakePdf.ViewModels;

namespace MakePdf.Galleries
{
    public class MotorArticleGallery : HtmlGallery
    {
        List<GalleryItem> _items = new List<GalleryItem>();
        List<Tag> _tags = new List<Tag>();


        protected override IEnumerable<GalleryItem> GetItems()
        {
            var navigator = Document.CreateNavigator();
            var titleNode = navigator
                .SelectDescendants("div", String.Empty, false)
                .OfType<HtmlNodeNavigator>()
                .Single(@node => @node.GetAttribute("class", String.Empty) == "header");
            var h1 = titleNode.SelectDescendants("h1", String.Empty, false).OfType<HtmlNodeNavigator>().First();
            var summary = h1.SelectSingleNode("span[@class='summary']");
            if (summary == null)
            {
                var name = Utils.TruncateAfterEOL(
                        Utils.Trim(
                            h1.SelectChildren(XPathNodeType.All)
                              .OfType<HtmlNodeNavigator>()
                              .Where(@node => @node.Name == "#text" || @node.Name == "br")
                              .Aggregate(new StringBuilder(), (@sb, @node) =>
                              {
                                  var val = Utils.Trim(@node.Value);
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
                    GalleryDocument.Name = Utils.Trim(Utils.SubstringAfter(Document.DocumentNode.SelectSingleNode("html/head/title").InnerText, "-"));
            }
            else
                GalleryDocument.Name = summary.Value;
            GalleryDocument.Annotation = Utils.Trim(titleNode.SelectDescendants("p", String.Empty, false).OfType<HtmlNodeNavigator>().First().Value);

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
                                _tags.Add(TagFactory.GetTextTag(contentNavigator.CurrentNode.InnerText).Wrap<BoldTag>().Wrap<ParagraphTag>());
                                break;
                            case "incut":
                                AddIncut(contentNavigator);
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
                if (Utils.IsNullOrEmpty(currentItem.Tags))
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
                                Utils.FixUrlProtocol(@node.SelectSingleNode("img").GetAttributeValue("src", String.Empty)),
                                textNode == null ? null : @node.SelectSingleNode("div/div").InnerText
                                );
                    }
                )
             );
        }

        private bool AddImage(HtmlNodeNavigator contentNavigator)
        {
            var imageNode = contentNavigator.CurrentNode.SelectSingleNode("img[@width]");
            if (imageNode == null)
                return false;
            Flush();
            var imageSource =
                imageNode.GetAttributeValue("src", String.Empty);
            var item = new GalleryItem(Utils.FixUrlProtocol(imageSource));
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
            var tubeID = videoNode.GetAttributeValue("src", String.Empty);
            var address = Regex.Replace(tubeID, @"http:\/\/(?:www\.)?youtube\.com\/embed\/(.+?)(?:\/|\?|$).*", "http://www.youtube.com/watch?v=$1");
            _tags.Add(TagFactory.GetTextTag("Видео>>").Wrap(new HrefTag(address)).Wrap<ParagraphTag>());
        }

        private void AddImageWithDescription(HtmlNodeNavigator contentNavigator)
        {
            Flush();
            var imageSource =
                contentNavigator.CurrentNode.SelectSingleNode("img")
                    .GetAttributeValue("src", String.Empty);
            var lines = contentNavigator
                .CurrentNode
                .SelectNodes("div/div/p")
                .Select(@node => @node.InnerText)
                .Where(@text => !String.IsNullOrEmpty(@text));
            var item = new GalleryItem(Utils.FixUrlProtocol(imageSource), TagFactory.GetParagraphTags(lines));
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
                    new GalleryItem(@node.SelectSingleNode("div/a").GetAttributeValue("href", String.Empty),
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
