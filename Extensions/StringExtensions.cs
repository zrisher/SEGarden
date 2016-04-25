using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace SEGarden.Extensions {

    public static class StringExtensions {

        public static readonly char[] InvalidFileChars = Path.GetInvalidFileNameChars().
            Concat(Path.GetInvalidPathChars()).ToArray();

        public static bool LooseContains(this string bigString, string smallString) {
            VRage.Exceptions.ThrowIf<ArgumentNullException>(bigString == null, "bigString");
            VRage.Exceptions.ThrowIf<ArgumentNullException>(smallString == null, "smallString");

            bigString = bigString.LowerRemoveWhitespace();
            smallString = smallString.LowerRemoveWhitespace();
            return bigString.Contains(smallString);
        }

        public static bool LooseContains(this IEnumerable<String> strings, String target) {
            VRage.Exceptions.ThrowIf<ArgumentNullException>(strings == null, "strings");
            VRage.Exceptions.ThrowIf<ArgumentNullException>(target == null, "target");

            return strings.Any(x =>
                x.LowerRemoveWhitespace().Contains(target.LowerRemoveWhitespace())
            );
        }

        /// <remarks>
        /// From http://stackoverflow.com/a/20857897
        /// </remarks>
        public static string RemoveWhitespace(this string input) {
            int j = 0, inputlen = input.Length;
            char[] newarr = new char[inputlen];

            for (int i = 0; i < inputlen; ++i) {
                char tmp = input[i];

                if (!char.IsWhiteSpace(tmp)) {
                    newarr[j] = tmp;
                    ++j;
                }
            }

            return new String(newarr, 0, j);
        }

        /// <summary>
        /// Convert a string to lower case and remove whitespace.
        /// </summary>
        public static string LowerRemoveWhitespace(this string input) {
            int outIndex = 0;
            char[] output = new char[input.Length];

            for (int inIndex = 0; inIndex < input.Length; inIndex++) {
                char current = input[inIndex];

                if (!char.IsWhiteSpace(current)) {
                    output[outIndex] = char.ToLower(current);
                    outIndex++;
                }
            }

            return new String(output, 0, outIndex);
        }

        public static String CleanFileName(this String s) {
            foreach (char c in InvalidFileChars) {
                s = s.Replace(c.ToString(), "");
            }

            return s;
        }

    }

}
