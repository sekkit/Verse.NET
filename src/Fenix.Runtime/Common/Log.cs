
#if !UNITY_5_3_OR_NEWER
using Serilog;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.IO;
using NLog.Targets.Wrappers;
using NLog.Targets;

//#if !UNITY_5_3_OR_NEWER 
//using globalLog = Serilog.Log;
//#endif



namespace Fenix.Common
{
	public class Log
	{
		static NLog.Logger _logger = new Logger(false).InternalLogger;

		private Log()
        {
		}
		 
		public static Logger GetLogger()
        {
			var obj = new Logger();
			return obj;
        } 
		 
		//public static void Verbose(params object[] args)
		//{
		//	Logger.Instance.Verbose(args);
		//}

		public static void Warn(params object[] args)
		{
			_logger.Warn(string.Join(" ", args.Select(m => m?.ToString())));
		} 

		public static void Info(params object[] args)
		{
			_logger.Info(string.Join(" ", args.Select(m => m?.ToString())));
		}

		public static void Debug(params object[] args)
		{
			_logger.Debug(string.Join(" ", args.Select(m => m?.ToString())));
		} 

		public static void Error(params object[] args)
		{
			_logger.Error(string.Join(" ", args.Select(m => m?.ToString())));
		} 
		public static void Fatal(params object[] args)
		{
			_logger.Fatal(string.Join(" ", args.Select(m => m?.ToString())));
		}
	}

	public class Logger
    {
		private readonly NLog.Logger NLogger;// = NLog.LogManager.GetLogger("DefaultLog");

		public NLog.Logger InternalLogger => NLogger;

		public Logger(bool isClassLogger=true)
		{
			var appName = Environment.GetEnvironmentVariable("AppName");

#if !UNITY_5_3_OR_NEWER
			var config = new NLog.Config.LoggingConfiguration();
			string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../logs");
			if (!Directory.Exists(logFolder))
				Directory.CreateDirectory(logFolder);
			string logFilePath = Path.Combine(logFolder, appName+".log");
			Console.WriteLine("CreateLogFile: " + logFilePath); 
			var logfile = new NLog.Targets.FileTarget("logfile") { 
				FileName = logFilePath,
				Layout = new NLog.Layouts.SimpleLayout("${longdate} [${level:uppercase=true}] [${threadid}] ${callsite} - ${message}"),
				ArchiveFileName = "${basedir}/logs/archived/" + appName + ".{#####}.log",
				ArchiveAboveSize = 100000000 /* 100 MB */,
				ArchiveNumbering = ArchiveNumberingMode.Sequence,
				ConcurrentWrites = true,
				KeepFileOpen = false,
				MaxArchiveFiles = 10,
			}; 
			
			if (!isClassLogger)
            {
				logfile.Layout = new NLog.Layouts.SimpleLayout("${longdate} [${level:uppercase=true}] [${threadid}] ${logger} - ${message}");
			}

			var logconsole = new NLog.Targets.ColoredConsoleTarget("logconsole");
			 
			AsyncTargetWrapper wrapper = new AsyncTargetWrapper();
			wrapper.WrappedTarget = logfile;
			wrapper.QueueLimit = 5000;
			wrapper.OverflowAction = AsyncTargetWrapperOverflowAction.Grow;

			AsyncTargetWrapper wrapper2 = new AsyncTargetWrapper();
			wrapper2.WrappedTarget = logconsole;
			wrapper2.QueueLimit = 5000;
			wrapper2.OverflowAction = AsyncTargetWrapperOverflowAction.Grow;
			 

			config.AddTarget("asyncFile", wrapper);
			config.AddTarget("asyncConsole", wrapper2);

			config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, wrapper2);
			config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, wrapper);
			NLog.LogManager.Configuration = config;
			if (isClassLogger)
				NLogger = NLog.LogManager.GetCurrentClassLogger();
			else
				NLogger = NLog.LogManager.GetLogger(appName);
#else
			string logFolder = Path.Combine(Unity.Common.IOUtil.persistentDataPath, "logs");
			if (!Directory.Exists(logFolder))
				Directory.CreateDirectory(logFolder);
			string logFilePath = Path.Combine(logFolder, appName+".log");
			Console.WriteLine("CreateLogFile: " + logFilePath);
			var logfile = new NLog.Targets.FileTarget("logfile")
			{
				FileName = logFilePath,
				Layout = new NLog.Layouts.SimpleLayout("${longdate} [${level:uppercase=true}] [${threadid}] ${callsite} - ${message}"),
				ArchiveFileName = "${basedir}/logs/archived/" + appName + ".{#####}.log",
				ArchiveAboveSize = 100000000 /* 100 MB */,
				ArchiveNumbering = ArchiveNumberingMode.Sequence,
				ConcurrentWrites = true,
				KeepFileOpen = false,
				MaxArchiveFiles = 10,
			};

			if (!isClassLogger)
            {
				logfile.Layout = new NLog.Layouts.SimpleLayout("${longdate} [${level:uppercase=true}] [${threadid}] ${logger} - ${message}");
			}

			AsyncTargetWrapper wrapper = new AsyncTargetWrapper();
			wrapper.WrappedTarget = logfile;
			wrapper.QueueLimit = 5000;
			wrapper.OverflowAction = AsyncTargetWrapperOverflowAction.Grow;  

			AsyncTargetWrapper wrapperUnity = new AsyncTargetWrapper();
			if(isClassLogger)
				wrapperUnity.WrappedTarget = Unity.Common.LogUtil.CreateUnityConsoleTarget("unityconsole", "${longdate} [${level:uppercase=true}] [${threadid}] ${callsite} - ${message}");
			else
				wrapperUnity.WrappedTarget = Unity.Common.LogUtil.CreateUnityConsoleTarget("unityconsole", "${longdate} [${level:uppercase=true}] [${threadid}] ${logger} - ${message}");
			wrapperUnity.QueueLimit = 5000;
			wrapperUnity.OverflowAction = AsyncTargetWrapperOverflowAction.Grow;

			if (isClassLogger)
			{
				NLogger = Unity.Common.LogUtil.GetLogger(null, (cfg) =>
				{ 
					cfg.AddTarget("asyncFile", wrapper);
					cfg.AddTarget("asyncConsole", wrapperUnity);

					cfg.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, wrapperUnity);
					cfg.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, wrapper);
				});
			}
			else
			{
				NLogger = Unity.Common.LogUtil.GetLogger(appName, (cfg) =>
				{ 
					cfg.AddTarget("asyncFile", wrapper);
					cfg.AddTarget("asyncConsole", wrapperUnity);

					cfg.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, wrapperUnity);
					cfg.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, wrapper);
				});
			}
#endif
		}

		public void Warning(params object[] args)
		{
			NLogger.Warn(string.Join(" ", args.Select(m => m?.ToString()))); 
        }

        public void Info(params object[] args)
		{
			NLogger.Info(string.Join(" ", args.Select(m => m?.ToString())));
		}

        public void Debug(params object[] args)
		{
			NLogger.Debug(string.Join(" ", args.Select(m => m?.ToString()))); 
		}

        public void Error(params object[] args)
		{
			NLogger.Error(string.Join(" ", args.Select(m => m?.ToString()))); 
		}
        public void Fatal(params object[] args)
		{
			NLogger.Fatal(string.Join(" ", args.Select(m => m?.ToString()))); 
		}
	}
}
