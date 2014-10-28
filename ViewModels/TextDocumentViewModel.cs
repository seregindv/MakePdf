using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using MakePdf.Configuration;
using MakePdf.Pooling.Pools;
using MakePdf.Stuff;
using MakePdf.XmlMakers;

namespace MakePdf.ViewModels
{
    [XmlRoot("TextDocument")]
    public class TextDocumentViewModel : DocumentViewModel
    {
        readonly static ObjectPool<TextDocumentXmlMaker> _textXmlMaker = ObjectPool<TextDocumentXmlMaker>.Get(() => new TextDocumentXmlMaker(Config.Instance.GetTemplate(TemplateType.Text)));

        #region Overrides of DocumentViewModel

        public override DocumentViewModel Clone()
        {
            var result = new TextDocumentViewModel();
            CloneTo(result);
            return result;
        }

        protected override void RenderDocument(string directory, CancellationToken ct)
        {
            Status = DocumentStatus.InProcess;
            Exception = null;
            try
            {
                using (var outStream = new FileStream(GetPdfFileName(directory), FileMode.Create))
                using (var driver = _fonetDriverPool.FetchDisposable())
                using (var pdfer = _textXmlMaker.FetchDisposable())
                {
                    var xml = pdfer.Object.GetXml(Name, Annotation, Contents, SkipEmptyLines, SourceAddress);
                    driver.Object.Render(xml, outStream);
                }
                Status = DocumentStatus.Complete;
            }
            catch (Exception ex)
            {
                Status = DocumentStatus.Error;
                Exception = ex;
            }
        }

        #endregion
    }
}
