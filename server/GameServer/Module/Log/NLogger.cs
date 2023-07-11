using Module.Shared;

using System;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers; 

namespace Module.Log;

public class NLogger: ILog
{
    private readonly NLog.Logger logger;

    public NLogger(string name, int process, string configPath)
    { 
        var config = new NLog.Config.LoggingConfiguration();
        string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../logs");
        if (!Directory.Exists(logFolder))
            Directory.CreateDirectory(logFolder);
        string logFilePath = Path.Combine(logFolder, name+".log");
        Console.WriteLine("CreateLogFile: " + logFilePath); 
        var logfile = new NLog.Targets.FileTarget("logfile") { 
            FileName = logFilePath,
            Layout = new NLog.Layouts.SimpleLayout("${longdate} [${level:uppercase=true}] [${threadid}] ${callsite} - ${message}"),
            ArchiveFileName = "${basedir}/logs/archived/" + name + ".{#####}.log",
            ArchiveAboveSize = 100000000 /* 100 MB */,
            ArchiveNumbering = ArchiveNumberingMode.Sequence,
            ConcurrentWrites = true,
            KeepFileOpen = false,
            MaxArchiveFiles = 10,
        }; 
        bool isClassLogger = false;
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
        //LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(configPath);
        LogManager.Configuration.Variables["appIdFormat"] = $"{process:000000}";
        LogManager.Configuration.Variables["currentDir"] = Environment.CurrentDirectory;
        //this.logger = LogManager.GetLogger(name);

        if (isClassLogger)
            this.logger = NLog.LogManager.GetCurrentClassLogger();
        else
            this.logger = NLog.LogManager.GetLogger(name); 
    }

    public void Trace(string message)
    {
        this.logger.Trace(message);
    }

    public void Warning(string message)
    {
        this.logger.Warn(message);
    }

    public void Info(string message)
    {
        this.logger.Info(message);
    }

    public void Debug(string message)
    {
        this.logger.Debug(message);
    }

    public void Error(string message)
    {
        this.logger.Error(message);
    }

    public void Fatal(string message)
    {
        this.logger.Fatal(message);
    }

    public void Trace(string message, params object[] args)
    {
        this.logger.Trace(message, args);
    }

    public void Warning(string message, params object[] args)
    {
        this.logger.Warn(message, args);
    }

    public void Info(string message, params object[] args)
    {
        this.logger.Info(message, args);
    }

    public void Debug(string message, params object[] args)
    {
        this.logger.Debug(message, args);
    }

    public void Error(string message, params object[] args)
    {
        this.logger.Error(message, args);
    }

    public void Fatal(string message, params object[] args)
    {
        this.logger.Fatal(message, args);
    }
} 