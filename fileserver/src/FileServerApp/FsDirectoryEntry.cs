using System;
using System.IO;

namespace FileServerApp
{
    public class FsDirectoryEntry
    {
        public string Name { get; set; }
        public string LastModified { get; set; }

        public FsDirectoryEntry () { }

        public FsDirectoryEntry (string name, DateTime lastModified)
        {
            this.Name = name;
            this.LastModified = lastModified.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
