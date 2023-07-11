using System;
using System.IO;
using System.Linq;

namespace Module.Shared
{
    public static class Sanitize {

        private static char[] invalidChars = Path.GetInvalidFileNameChars().Union(new char[1]{':'}).ToArray();

        public static string SanitizeToDirName(string dirName) {
            return string.Join("_", dirName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }

        public static string SanitizeToFileName(string fileName) {
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }

    }
}