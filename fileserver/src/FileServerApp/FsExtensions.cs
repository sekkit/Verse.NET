using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FileServerApp
{
    public static class FsExtensions
    {
        public static Dictionary<string, object> GetFsEntries (string relPath, string rootPath) {
            if (!relPath.EndsWith('/')) relPath += '/';
            string path = rootPath + relPath;
            IEnumerable<FsDirectoryEntry> dirs = null;
            IEnumerable<FsFileEntry> files = null;
            var baseDir = new DirectoryInfo(path);
            var rootDir = new DirectoryInfo(rootPath);
            string parent = "";

            if (baseDir.Exists) {
                // Get subdirectories and files
                dirs = baseDir.EnumerateDirectories().Select(MakeFsDirectoryEntry);
                files = baseDir.EnumerateFiles().Select(MakeFsFileEntry);
                parent = GetRelativePath(baseDir.Parent, rootDir);
            }
            
            // Make a string:string dictionary
            var resultDict = new Dictionary<string, object> {
                { "Base", relPath },
                { "Parent", parent },
                { "Dirs", dirs ?? new FsDirectoryEntry[0] },
                { "Files", files ?? new FsFileEntry[0] }
            }; 

            return resultDict;
        }

        public static string GetRelativePath (DirectoryInfo directory, DirectoryInfo root)
        {
            string rootPath = root.FullName.Replace('\\', '/');
            string dirPath = directory.FullName.Replace('\\', '/');
            if (rootPath.Length > dirPath.Length) {
                return "";
            }
            else {
                return dirPath.Replace(rootPath, "");
            }
        }

        public static FsFileEntry MakeFsFileEntry (FileInfo file) 
        {
            return new FsFileEntry(file.Name, file.Length, file.LastWriteTimeUtc);
        }

        public static FsDirectoryEntry MakeFsDirectoryEntry (DirectoryInfo dir) 
        {
            return new FsDirectoryEntry(dir.Name, dir.LastWriteTimeUtc);
        }
    }
}