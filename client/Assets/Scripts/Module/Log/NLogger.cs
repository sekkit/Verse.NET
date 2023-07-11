using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Module.Shared;
using NLog.Targets;
using UnityEngine;

public class NLogger: ILog
{
    private readonly NLog.Logger logger;

    public NLogger(string name, int process, string configPath)
    { 
        string logFolder = Path.Combine(UnityEngine.Application.persistentDataPath, "logs");
		if (!Directory.Exists(logFolder))
			Directory.CreateDirectory(logFolder);
		string logFilePath = Path.Combine(logFolder, name+".log");
		Console.WriteLine("CreateLogFile: " + logFilePath);
		var logfile = new NLog.Targets.FileTarget("logfile")
		{
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

        UnityConsoleTarget logconsole;// = new NLog.Targets.ColoredConsoleTarget("logconsole");

        if (isClassLogger)
			logconsole = UnityLogUtil.CreateUnityConsoleTarget("unityconsole", "${longdate} [${level:uppercase=true}] [${threadid}] ${callsite} - ${message}");
        else
			logconsole = UnityLogUtil.CreateUnityConsoleTarget("unityconsole", "${longdate} [${level:uppercase=true}] [${threadid}] ${logger} - ${message}");

        if (isClassLogger)
		{
			this.logger = UnityLogUtil.GetLogger(null, (cfg) =>
			{
				cfg.AddTarget("logfile", logfile);
				cfg.AddTarget("logconsole", logconsole);

				cfg.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);
				cfg.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logfile);
			});
		}
		else
		{
			this.logger = UnityLogUtil.GetLogger(name, (cfg) =>
			{
				cfg.AddTarget("logfile", logfile);
				cfg.AddTarget("logconsole", logconsole); 

				cfg.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);
				cfg.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logfile);
			});
		}
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