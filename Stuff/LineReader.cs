using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MakePdf.Stuff
{
    public class NonEmptyStringReader : StringReader
    {

        public NonEmptyStringReader(string s)
            : base(s)
        {
        }

        public override string ReadLine()
        {
            string line;
            do
            {
                line = base.ReadLine();
            } while (line != null && String.IsNullOrWhiteSpace(line));
            return line;
        }
    }
}
