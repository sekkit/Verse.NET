using System;
using System.Collections;
using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;
using UnityEngine;

public static class UnityLogUtil
{
    private static object locker = new object();

    public static NLog.Logger GetLogger()
    {
        return GetLogger(new Dictionary<string, object>());
    }

    public static NLog.Logger GetLogger(Dictionary<string, object> eventProperties)
    {
        return GetLogger(System.IO.Path.Combine(Application.dataPath, "Scripts", "NLog.config"), eventProperties);
    }

    public static NLog.Logger GetLogger(string nlogConfigFile)
    {
        return GetLogger(nlogConfigFile, new Dictionary<string, object>());
    }

    public static NLog.Logger GetLogger(string name, string nlogConfigFile)
    {
        return GetLogger(name, nlogConfigFile, new Dictionary<string, object>());
    }

    public static NLog.Logger GetLogger(string nlogConfigFile, Dictionary<string, object> eventProperties)
    {
        // create logger with caller name
        var sf = new System.Diagnostics.StackFrame(1);
        return GetLogger(sf.GetMethod().ReflectedType.FullName, nlogConfigFile, eventProperties);
    }

    public static NLog.Logger GetLogger(string name, string nlogConfigFile, Dictionary<string, object> eventProperties)
    {
        // configure if not configured
        lock (locker)
        {
            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new XmlLoggingConfiguration(nlogConfigFile);
            }
        }

        // create logger with caller name
        var logger = LogManager.GetLogger(name);

        // add eventProperties
        foreach (var ep in eventProperties)
        {
            logger.SetProperty(ep.Key, ep.Value);
        }

        return logger;
    }

    public static NLog.Logger GetLogger(string name, Action<NLog.Config.LoggingConfiguration> configure)
    {
        // configure if not configured
        lock (locker)
        {
            if (LogManager.Configuration == null)
            {
                // create new config
                var config = new NLog.Config.LoggingConfiguration();
                // configure
                configure(config);
                // update config if configuration finished successfully
                LogManager.Configuration = config;
            }
        }

        // return logger with caller name
        if (name == null)
        {
            Console.WriteLine("create_default_log:" + name);
            var sf = new System.Diagnostics.StackFrame(1);
            return LogManager.GetLogger(sf.GetMethod().ReflectedType.FullName);
        }
        else
        {
            Console.WriteLine("create_log:" + name);
            return LogManager.GetLogger(name);
        }
    }

    public static UnityConsoleTarget CreateUnityConsoleTarget()
    {
        return new UnityConsoleTarget();
    }

    public static UnityConsoleTarget CreateUnityConsoleTarget(string name)
    {
        return new UnityConsoleTarget(name);
    }

    public static UnityConsoleTarget CreateUnityConsoleTarget(string name, string layout)
    {
        return new UnityConsoleTarget(name, layout);
    }

    public static WebServiceTarget CreateJsonWebServiceTarget(string uri)
    {
        return CreateJsonWebServiceTarget(null, uri);
    }

    public static WebServiceTarget CreateJsonWebServiceTarget(string name, string uri)
    {
        var items = new Dictionary<string, string>()
        {
            {"SequenceId", "${ticks}.${sequenceid:padCharacter=0:padding=6}"},
            {"LocalTimestamp", "${date:format=yyyy-MM-ddTHH:mm:ss.fffffff}"},
            {"Level", "${level:uppercase=true}"},
            {"Callsite", "${callsite}"},
            {"Message", "${message}"},
            {"error", "${exception:format=Message, ToString:separator=\n}"}
        };

        return CreateJsonWebServiceTarget(name, uri, items);
    }

    public static WebServiceTarget CreateJsonWebServiceTarget(string uri, Dictionary<string, string> items)
    {
        return CreateJsonWebServiceTarget(null, uri, items);
    }

    public static WebServiceTarget CreateJsonWebServiceTarget(string name, string uri, Dictionary<string, string> items)
    {
        // create and setup
        var jwsTarget = name == null ? new WebServiceTarget() : new WebServiceTarget(name);
        jwsTarget.Url = new Uri(uri);
        jwsTarget.Protocol = WebServiceProtocol.JsonPost;

        // add JSON attributes
        foreach (var item in items)
        {
            jwsTarget.Parameters.Add(new MethodCallParameter(item.Key, item.Value));
        }

        return jwsTarget;
    }
}