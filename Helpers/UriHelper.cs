using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MakePdf.Helpers
{
    public static class UriHelper
    {

        public static void FixUriTrailingDotBug()
        {
            var getSyntax = typeof(UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
            var flagsField = typeof(UriParser).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
            if (getSyntax == null || flagsField == null)
                return;
            foreach (string scheme in new[] { "http", "https" })
            {
                var parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                if (parser == null)
                    continue;
                var flagsValue = (int)flagsField.GetValue(parser);
                // Clear the CanonicalizeAsFilePath attribute
                if ((flagsValue & 0x1000000) != 0)
                    flagsField.SetValue(parser, flagsValue & ~0x1000000);
            }
        }

        public static string FixUrlProtocol(string url)
        {
            var builder = new UriBuilder(new Uri(url));
            if (builder.Scheme == Uri.UriSchemeFile)
                builder.Scheme = Uri.UriSchemeHttp;
            return builder.ToString();
        }
    }
}
