
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
#if !UNITY_5_3_OR_NEWER
			Console.WriteLine(message);
#else
			UnityEngine.Debug.Log(message);
#endif
#if !CLIENT
			globalLog.Verbose(message); 
#endif
		}

		public static void Warning(string message)
		{
#if !UNITY_5_3_OR_NEWER
			Console.WriteLine(message);
#else
			UnityEngine.Debug.LogWarning(message);
#endif
#if !CLIENT
			globalLog.Warning(message);
#endif
		}

		public static void Info(string message)
		{
#if !UNITY_5_3_OR_NEWER
			Console.WriteLine(message);
#else
			UnityEngine.Debug.Log(message);
#endif
#if !CLIENT
			globalLog.Information(message);
#endif
		}

		public static void Debug(string message)
		{
#if !UNITY_5_3_OR_NEWER
			Console.WriteLine(message);
#else
			UnityEngine.Debug.Log(message);
#endif
#if !CLIENT
			globalLog.Debug(message);
#endif
		}

		public static void Error(Exception e)
		{
#if !UNITY_5_3_OR_NEWER
			Console.WriteLine(e.ToString());
#else
			UnityEngine.Debug.LogError(e.ToString());
#endif
#if !CLIENT
			globalLog.Error(e.ToString());
#endif
		}

		public static void Error(string message)
		{
#if !UNITY_5_3_OR_NEWER
			Console.WriteLine(message);
#else
			UnityEngine.Debug.LogError(message);
#endif
#if !CLIENT
			globalLog.Error(message);
#endif
		}

		public static void Fatal(Exception e)
		{
#if !UNITY_5_3_OR_NEWER
			Console.WriteLine(e.ToString());
#else
			UnityEngine.Debug.LogError(e.ToString());
#endif
#if !CLIENT
			globalLog.Fatal(e.ToString());
#endif
		}

		public static void Fatal(string message)
		{
#if !UNITY_5_3_OR_NEWER
			Console.WriteLine(message);
#else
			UnityEngine.Debug.LogError(message);
#endif
#if !CLIENT
			globalLog.Fatal(message);
#endif
		}
	}
}
