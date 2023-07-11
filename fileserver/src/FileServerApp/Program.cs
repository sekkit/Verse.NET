using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileServerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(withMyStartupClass)
                .Build()
                .Run();
        }

        static void withMyStartupClass (IWebHostBuilder webBuilder) {
            webBuilder.UseStartup<Startup>();
        }
    }
}
