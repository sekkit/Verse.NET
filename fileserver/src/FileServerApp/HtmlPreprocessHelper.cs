using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace FileServerApp
{
    public static class HtmlPreprocessHelper
    {
        // Cache
        static IDictionary<string, string> 
            cachedFileContents = new SortedDictionary<string, string>();

        // Last write info
        static IDictionary<string, DateTime>
            cachedAccessTimes = new SortedDictionary<string, DateTime>();

        // Silly Html preprocessor. 
        // Replaces @@@<some key> with corresponding value.
        // Does nothing if key is not in replacement table.
        public static string GetPreprocessedHtml (
            string filepath, 
            IDictionary<string, object> replacements,
            bool useCache = false) 
        {
            string data = null; 
            if (useCache) {
                DateTime lastAccessTime;
                if (cachedAccessTimes.TryGetValue(filepath, out lastAccessTime)) {
                    if (File.GetLastWriteTimeUtc(filepath) < lastAccessTime) {
                        data = cachedFileContents[filepath];
                    }
                }
            }
            if (data == null) {
                data = File.ReadAllText(filepath);
                cachedFileContents[filepath] = data;
                cachedAccessTimes[filepath] = DateTime.UtcNow;
            } 
            StringBuilder builder = new StringBuilder(data);
            foreach (var kvPair in replacements) {
                builder.Replace("@@@" + kvPair.Key, kvPair.Value.ToString());
            }
            return builder.ToString();
        }
    }
}
