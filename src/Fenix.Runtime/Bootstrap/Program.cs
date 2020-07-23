#if RUNTIME
using System;
using System.Reflection;

namespace Fenix
{
    class Program
    { 
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Bootstrap.Start(new Assembly[] { typeof(Program).Assembly }, null); //单进程模式
            }
            else
            {
                //将命令行参数，设置到进程的环境变量
                Environment.SetEnvironmentVariable("HostType", "AccountService");

                Bootstrap.Start(new Assembly[] { typeof(Program).Assembly }, null, isMultiProcess: true); //分布式
            }
        }
    }
}
#endif