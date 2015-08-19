using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace SEGarden.Extensions {

    public static class StringExtensions {

        public static readonly char[] InvalidFileChars = Path.GetInvalidFileNameChars().
            Concat(Path.GetInvalidPathChars()).ToArray();

        public static String CleanFileName(this String s) {
            foreach (char c in InvalidFileChars) {
                s = s.Replace(c.ToString(), "");
            }

            return s;
        }

    }

}
