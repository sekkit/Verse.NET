 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace Fenix.Common
{
	public class Log
	{
		private Log()
        { 
		} 
		 
		public static Logger GetLogger(string name)
        {
			var obj = new Logger(name); 
			return obj;
        } 
		 
		public static void Verbose(params object[] args)
		{
			Logger.Instance.Verbose(args);
		}

		public static void Warning(params object[] args)
		{
			Logger.Instance.Warning(args);
		} 

		public static void Info(params object[] args)
		{
			Logger.Instance.Info(args);
		}

		public static void Debug(params object[] args)
		{
			Logger.Instance.Debug(args);
		} 

		public static void Error(params object[] args)
		{
			Logger.Instance.Error(args);
		} 
		public static void Fatal(params object[] args)
		{
			Logger.Instance.Fatal(args);
		}
	}

	public class Logger
    {
		public Logger(string name)
		{
			var appname = Environment.GetEnvironmentVariable("AppName");

			if (name == null && appname != null)
				Name = appname;
			else
				Name = name;
 
        }

        string Name = null;

		public static Logger Instance = new Logger(null); 

		string Format(object[] objs, string logLevel)
		{
			string output = string.Join(" ", objs.Select(m => m?.ToString()));

			var appname = Environment.GetEnvironmentVariable("AppName");

			if (Name == null && appname != null)
				Name = appname;

#if !UNITY_5_3_OR_NEWER
    //#if !CLIENT
			    return string.Format("({0}) {1}",  Name, output);
    //#else
			 //   return string.Format("{0} [{1}]({2}) {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logLevel, Name, output); 
    //#endif
#else
			return string.Format("[{0}]({1}) {2}", logLevel, Name, output); 
#endif

		}

		public void Verbose(params object[] args)
		{
			var message = Format(args, "VERB");
			 
            Console.WriteLine(message); 
        }

        public void Warning(params object[] args)
		{
			var message = Format(args, "WARN"); 
            Console.WriteLine(message); 
        }

        public void Info(params object[] args)
		{
			var message = Format(args, "INFO");  
            Console.WriteLine(message); 

        }

        public void Debug(params object[] args)
		{
			var message = Format(args, "DEBUG"); 
            Console.WriteLine(message); 
        }

        public void Error(params object[] args)
		{
			var message = Format(args, "ERROR"); 
            Console.WriteLine(message); 
        }
        public void Fatal(params object[] args)
		{
			var message = Format(args, "FATAL"); 
            Console.WriteLine(message); 
        }
	}
}
