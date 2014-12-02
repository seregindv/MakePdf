using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Windows.Input;
using System.Windows.Media;
using Fonet;
using Fonet.Render.Pdf;
using MakePdf.Configuration;
using MakePdf.Markup;

namespace MakePdf.Stuff
{
    public static class Utils
    {
        static readonly Regex _splitRegex = new Regex(@"\b(?:(?:https?|ftp|file)://|www\.|ftp\.)([\w\.-]+)(?:\([-A-Z0-9+&@#/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#/%=~_|$?!:,.]*\)|[A-Z0-9+&@#/%=~_|$])", RegexOptions.IgnoreCase);
        public static MatchCollection GetHyperlinkMatches(string s)
        {
            return _splitRegex.Matches(s);
        }

        public static string GetFullPath(string directory, string fileName)
        {
            return Path.Combine(directory, TrimIllegalChars(fileName));
        }

        public static string GetPdfPath(string directory, string title, string extension = ".pdf")
        {
            var fileName = (title.Length > 255 ? title.Substring(0, 255) : title).Trim() + extension;
            return GetFullPath(directory, fileName);
        }

        public static string TrimIllegalChars(string path)
        {
            return String.Concat(path.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        }

        public static void CopyStream(Stream source, Stream destination)
        {
            var buffer = new byte[32768];
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, read);
            }
        }

        public static Stream GetResponseStream(string url)
        {
            var request = WebRequest.Create(url);
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            var response = request.GetResponse();
            return response.GetResponseStream();
        }

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

        public static string GetFileName(int fileNum, string format = "00000000")
        {
            return fileNum.ToString(format);
        }

        public static Tuple<TEnum, TAttr>[] GetDecoratedEnumMembers<TEnum, TAttr>()
        {
            return GetEnumMembers<TEnum, TAttr>(true);
        }

        public static Tuple<TEnum, TAttr>[] GetNonDecoratedEnumMembers<TEnum, TAttr>()
        {
            return GetEnumMembers<TEnum, TAttr>(false);
        }

        private static Tuple<TEnum, TAttr>[] GetEnumMembers<TEnum, TAttr>(bool? decorated)
        {
            var result =
                from field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)
                let attrs = field.GetCustomAttributes(typeof(TAttr), true)
                where decorated == null || (decorated.Value && attrs.Length > 0) || (!decorated.Value && attrs.Length == 0)
                select new Tuple<TEnum, TAttr>((TEnum)Enum.Parse(typeof(TEnum), field.Name), (TAttr)attrs[0]);
            return result.ToArray();
        }

        public static Tuple<TEnum, TAttr> FindFirstField<TEnum, TAttr>(Tuple<TEnum, TAttr>[] fields, Func<TAttr, bool> criteria)
        {
            return fields.FirstOrDefault(@field => criteria(@field.Item2));
        }

        public static string Trim(string s)
        {
            return s.Replace("&nbsp;", String.Empty).Trim(' ', '\r', '\n', '\t');
        }

        public static string TruncateAfterEOL(string s)
        {
            var index = s.IndexOf('\n');
            if (index >= 0)
                return s.Substring(0, index);
            return s;
        }

        public static void SaveStream(Stream s, string f)
        {
            if (!s.CanSeek)
                throw new NotSupportedException("Nonseekable streams are not supported");
            var pos = s.Position;
            s.Position = 0L;
            var sr = new StreamReader(s);
            var cnt = sr.ReadToEnd();
            File.WriteAllText(f, cnt);
            s.Position = pos;
        }

        public static bool IsNullOrEmpty<T>(T collection) where T : ICollection
        {
            return collection == null || collection.Count == 0;
        }

        public static string FixUrlProtocol(string url)
        {
            var builder = new UriBuilder(new Uri(url));
            if (builder.Scheme == Uri.UriSchemeFile)
                builder.Scheme = Uri.UriSchemeHttp;
            return builder.ToString();
        }

        public static string SubstringAfter(string s, string after)
        {
            var afterIndex = s.IndexOf(after);
            return afterIndex == -1 ? s : s.Substring(0, afterIndex);
        }

        public static string SafeSubstring(string s, int startIndex, int length)
        {
            if (s.Length <= startIndex)
                return String.Empty;
            if (startIndex + length >= s.Length)
                length = s.Length - startIndex;
            return s.Substring(startIndex, length);
        }

        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static string InsertOrDeleteAtLineStart(string s, string text, int start, int length, bool delete = false)
        {
            var result = s;
            var pos = Math.Min(start + length, s.Length - 1);
            var IsRN = (Func<char, bool>)(c => c == '\r' || c == '\n');
            var skipRN = IsRN(result[pos]);
            if (length > 0 && IsRN(result[pos - 1]))
            {
                skipRN = true;
                --pos;
            }
            var exitAfterInsert = false;
            while (true)
            {
                if (skipRN)
                {
                    if (!IsRN(result[pos]) || pos == 1)
                        skipRN = false;
                }
                else if (IsRN(result[pos]) || pos == 0)
                {
                    var picTextPos = pos == 0 ? pos : pos + 1;
                    var isPic = SafeSubstring(result, picTextPos, text.Length) == text;
                    if (delete)
                    {
                        if (isPic)
                            result = result.Remove(picTextPos, text.Length);
                    }
                    else
                    {
                        if (!isPic)
                            result = result.Insert(picTextPos, text);
                    }
                    if (exitAfterInsert || pos == 0)
                        break;
                    skipRN = true;
                }
                --pos;
                if (pos <= start)
                    exitAfterInsert = true;
            }
            return result;
        }

        public static bool IsTablet
        {
            get
            {
                return Tablet.TabletDevices.Count > 0;
            }
        }

        public static void DisableWPFTabletSupport()
        {
            // Get a collection of the tablet devices for this window.  
            var devices = Tablet.TabletDevices;
            if (devices.Count > 0)
            {
                // Get the Type of InputManager.
                var inputManagerType = typeof(InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.
                var stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, InputManager.Current, null);

                if (stylusLogic != null)
                {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.
                    var stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.
                    while (devices.Count > 0)
                        // Remove the first tablet device in the devices collection.
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, stylusLogic, new object[] { (uint)0 });
                }
            }
        }

        public static void EnableFocusTracking()
        {
            var cp = new InputPanelConfiguration();
            var icp = cp as IInputPanelConfiguration;
            if (icp != null)
                icp.EnableFocusTracking();
        }

        public static FonetDriver GetFonetDriver()
        {
            return new FonetDriver { Options = new PdfRendererOptions { FontType = FontType.Subset } };
        }

        public static string GetColorAsString(string r, string g, string b)
        {
            return "#" + (Byte.Parse(r) << 16 |
                   Byte.Parse(g) << 8 |
                   Byte.Parse(b)).ToString("X6");

        }

        static readonly object _syncRoot = new object();

        static volatile string[] _supportedImages;
        static IEnumerable<string> SupportedImages
        {
            get
            {
                if (_supportedImages == null)
                    lock (_syncRoot)
                        if (_supportedImages == null)
                            _supportedImages = Config.Instance.AppSettings["SupportedImages"].Split(',');
                return _supportedImages;
            }
        }

        public static bool IsImage(string pathToImage)
        {
            var extension = Path.GetExtension(pathToImage);
            if (extension == null)
                return false;
            extension = extension.Replace(".", String.Empty);
            return SupportedImages.Contains(extension, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
