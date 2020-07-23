
#if !CLIENT
using Serilog;
#endif
using System;
using System.Collections.Generic;
using System.Text;

#if !CLIENT 
using globalLog = Serilog.Log; 
#endif

namespace Fenix.Common
{
	public class Log
	{
		public Log()
        {
#if !CLIENT
			globalLog.Logger = new LoggerConfiguration()
			.MinimumLevel.Information()
			.WriteTo.Console()
			.WriteTo.File("log.txt",
				rollingInterval: RollingInterval.Day,
				rollOnFileSizeLimit: true)
			.CreateLogger();
#endif
		} 

		public static void Verbose(string message)
		{
			Console.WriteLine(message);
#if !CLIENT
			globalLog.Verbose(message); 
#endif
		}

		public static void Warning(string message)
		{
			Console.WriteLine(message);
#if !CLIENT
			globalLog.Warning(message);
#endif
		}

		public static void Info(string message)
		{
			Console.WriteLine(message);
#if !CLIENT
			globalLog.Information(message);
#endif
		}

		public static void Debug(string message)
		{
			Console.WriteLine(message);
#if !CLIENT
			globalLog.Debug(message);
#endif
		}

		public static void Error(Exception e)
		{
			Console.WriteLine(e.ToString());
#if !CLIENT
			globalLog.Error(e.ToString());
#endif
		}

		public static void Error(string message)
		{
			Console.WriteLine(message);
#if !CLIENT
			globalLog.Error(message);
#endif
		}

		public static void Fatal(Exception e)
		{
			Console.WriteLine(e.ToString());
#if !CLIENT
			globalLog.Fatal(e.ToString());
#endif
		}

		public static void Fatal(string message)
		{
			Console.WriteLine(message);
#if !CLIENT
			globalLog.Fatal(message);
#endif
		}
	}
}
