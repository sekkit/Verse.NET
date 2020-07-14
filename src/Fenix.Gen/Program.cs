using System;
using System.IO;

namespace Fenix.Gen
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootFolder = Directory.GetCurrentDirectory();
            var path = File.ReadAllBytes(Path.Combine(rootFolder, "Server.App.dll"));
            Console.WriteLine("Hello World!");
        }
    }
}
