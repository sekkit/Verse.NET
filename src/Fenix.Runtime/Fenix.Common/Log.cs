using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

using globalLog = Serilog.Log;

namespace Fenix.Common
{
	public class Log
	{
		public Log()
        {
			globalLog.Logger = new LoggerConfiguration()
			.MinimumLevel.Information()
			.WriteTo.Console()
			.WriteTo.File("log.txt",
				rollingInterval: RollingInterval.Day,
				rollOnFileSizeLimit: true)
			.CreateLogger();
		} 

		public static void Verbose(string message)
		{
			globalLog.Verbose(message); 
		}

		public static void Warning(string message)
		{
			globalLog.Warning(message);
		}

		public static void Info(string message)
		{
			globalLog.Information(message);
		}

		public static void Debug(string message)
		{
			globalLog.Debug(message);
		}

		public static void Error(Exception e)
		{
			globalLog.Error(e.ToString());
		}

		public static void Error(string message)
		{
			globalLog.Error(message);
		}

		public static void Fatal(Exception e)
		{
			globalLog.Fatal(e.ToString());
		}

		public static void Fatal(string message)
		{
			globalLog.Fatal(message);
		}
	}
}
