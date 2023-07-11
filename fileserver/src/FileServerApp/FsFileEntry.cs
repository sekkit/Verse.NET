using System;
using System.IO;

namespace FileServerApp
{
    public class FsFileEntry
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string LastModified { get; set; }

        public FsFileEntry () { }

        public FsFileEntry (string name, long size, DateTime lastModified)
        {
            this.Name = name;
            this.Size = size;
            this.LastModified = lastModified.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
