using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakePdf.Helpers
{
    public static class StreamHelper
    {
        public static void CopyStream(Stream source, Stream destination)
        {
            var buffer = new byte[32768];
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, read);
            }
        }

        public static void SaveStream(Stream s, string f)
        {
            if (!s.CanSeek)
                throw new NotSupportedException("Nonseekable streams not supported");
            var pos = s.Position;
            s.Position = 0L;
            var sr = new StreamReader(s);
            var cnt = sr.ReadToEnd();
            File.WriteAllText(f, cnt);
            s.Position = pos;
        }
    }
}
