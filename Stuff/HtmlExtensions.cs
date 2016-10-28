using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace MakePdf.Stuff
{
    public static class HtmlExtensions
    {
        public static string GetHref(this HtmlNode node)
        {
            return node.GetAttributeValue("href", String.Empty);
        }

        public static string GetHref(this XPathNavigator nodeNavigator)
        {
            return nodeNavigator.GetAttribute("href", String.Empty);
        }

        public static string GetSrc(this HtmlNode node)
        {
            return node.GetAttributeValue("src", String.Empty);
        }
    }
}
