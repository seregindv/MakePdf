using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace MakePdf.XmlMakers
{
    public abstract class XmlMaker
    {
        readonly XmlDocument _templateDocument;
        readonly XmlNode _textParent;
        readonly XmlNode _textNode;
        readonly XmlNode _titleNode;
        readonly XmlNode _annotationNode;
        readonly XmlNode _linkNode;
        readonly Regex _linkRegex;

        public XmlMaker(string templatePath)
        {
            _templateDocument = new XmlDocument();
            _templateDocument.Load(templatePath);
            var textPlaceholder = _templateDocument.GetElementsByTagName("TextPlaceholder");
            _textParent = textPlaceholder[0].ParentNode;
            _textNode = textPlaceholder[0].ChildNodes[0];
            _titleNode = GetHeaderNode("TitlePlaceholder");
            _annotationNode = GetHeaderNode("AnnotationPlaceholder");
            _linkNode = PullOutNodeUnder("LinkTemplate");
            _linkRegex = new Regex(@"\b(?:(?:https?|ftp|file)://|www\.|ftp\.)([\w\.-]+)(?:\([-A-Z0-9+&@#/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#/%=~_|$?!:,.]*\)|[A-Z0-9+&@#/%=~_|$])", RegexOptions.IgnoreCase);
        }

        private XmlNode GetHeaderNode(string tag)
        {
            var placeholder = _templateDocument.GetElementsByTagName(tag);
            if (placeholder.Count == 0)
                return null;
            var result = placeholder[0].ParentNode;
            if (result != null)
                result.RemoveChild(placeholder[0]);
            return result;
        }

        private XmlNode PullOutNodeUnder(string tag)
        {
            var placeholder = _templateDocument.GetElementsByTagName(tag);
            if (placeholder.Count == 0)
                return null;
            var parent = placeholder[0].ParentNode;
            var result = placeholder[0].HasChildNodes ? placeholder[0].ChildNodes[0] : null;
            if (parent != null)
                parent.RemoveChild(placeholder[0]);
            return result;
        }

        private void SetHeaderText(XmlNode node, string text)
        {
            if (node != null)
                node.InnerText = text;
        }

        private XmlNode MakeLink(string address, string text)
        {
            const string EXT_DEST = "external-destination";
            if (_linkNode == null)
                return _templateDocument.CreateTextNode(text + " [" + address + "]");
            var result = _linkNode.Clone();
            var destAttr = result.Attributes[EXT_DEST] ??
                       result.Attributes.Append(_templateDocument.CreateAttribute(EXT_DEST));
            destAttr.Value = address;
            result.InnerText = text;
            return result;
        }

        private XmlNode GetTextNode(string text)
        {
            var result = _textNode.Clone();
            var matches = _linkRegex.Matches(text);
            var startPosition = 0;
            foreach (Match match in matches)
            {
                // from startPosition to Index
                if (startPosition != match.Index)
                    result.AppendChild(_templateDocument.CreateTextNode(text.Substring(startPosition, match.Index - startPosition)));
                // link
                result.AppendChild(MakeLink(match.Value, match.Groups[1].Value));
                // shift startPosition
                startPosition += match.Index + match.Length;
            }
            if (startPosition < text.Length)
                result.AppendChild(_templateDocument.CreateTextNode(text.Substring(startPosition, text.Length - startPosition)));
            return result;
        }

        public virtual XmlDocument GetXml(string title, string annotation, string text, bool removeEmptyStrings, string sourceAddress)
        {
            SetHeaderText(_titleNode, title);
            SetHeaderText(_annotationNode, annotation);
            while (_textParent.HasChildNodes)
                _textParent.RemoveChild(_textParent.FirstChild);
            using (var reader = new StringReader(text))
                do
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;
                    if (removeEmptyStrings && String.IsNullOrWhiteSpace(line))
                        continue;
                    _textParent.AppendChild(GetTextNode(line));
                } while (true);
            if (!String.IsNullOrEmpty(sourceAddress))
                _textParent.AppendChild(GetTextNode("Источник " + sourceAddress));
            return _templateDocument;
        }
    }
}
