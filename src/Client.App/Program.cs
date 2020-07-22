using System;

namespace Client.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = ClientApp.Create();

            c.Login("hello", "world", (code) => { 
                
            });
        }
    }
}
