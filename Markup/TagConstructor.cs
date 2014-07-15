using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using HtmlAgilityPack;
using MakePdf.Stuff;
using MakePdf.ViewModels;

namespace MakePdf.Markup
{
    public static class LentaArticleStaticTagConstructor
    {
        public static List<Tag> GetTags(HtmlNodeNavigator navigator)
        {
            var result = new List<Tag>();
            if (!navigator.MoveToFirst())
                return null;
            do
            {
                var recognized = true;
                switch (navigator.CurrentNode.Name)
                {
                    case "p":
                        var classAttr = navigator.CurrentNode.Attributes["class"];
                        if (classAttr != null && classAttr.Value == "question")
                            result.Add(new ItalicTag().Wrap<BoldTag>().Wrap<ParagraphTag>());
                        else
                            result.Add(new ParagraphTag());
                        break;
                    case "i":
                        result.Add(new ItalicTag());
                        break;
                    case "b":
                        result.Add(new BoldTag());
                        break;
                    case "a":
                        result.Add(new HrefTag(navigator.CurrentNode.GetAttributeValue("href", String.Empty)));
                        break;
                    default:
                        var text = Utils.Trim(navigator.CurrentNode.InnerText);
                        if (text.Length > 0)
                        {
                            // keep spaces etc
                            text = navigator.CurrentNode.InnerText;
                            if (navigator.CurrentNode.Name != "#text")
                                recognized = false;
                            if (!recognized)
                            {
                                text = navigator.CurrentNode.Name + ">" + Utils.Trim(text);
                                result.Add(TagFactory.GetTextTag(text).Wrap(new ColorTag("red")).Wrap<ParagraphTag>());
                            }
                            else
                                result.Add(TagFactory.GetTextTag(text));
                        }
                        break;
                }
                if (recognized && navigator.CurrentNode.HasChildNodes)
                {
                    navigator.MoveToFirstChild();
                    if (result.Any())
                        result.Last().DrillDown().Tags = GetTags(navigator);
                    else
                        result = GetTags(navigator);
                    navigator.MoveToParent();
                }
            } while (navigator.MoveToNext());
            return result;
        }
    }
}
