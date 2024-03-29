﻿ 
using CommandLine;
using Helper;
using Module.Log;
using Module.Shared;
using MongoDB.Bson;
using Newtonsoft.Json;
using Service.Db;
using Service.Entity;
using Service.Id;
using Service.Login;
using Service.Timer;

public sealed class Init
{
    public static void Start()
    {
        WinPeriod.Init();
        
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Log.Error(e.ExceptionObject.ToString());
        };
        
        Parser.Default.ParseArguments<Options>(Environment.GetCommandLineArgs())
            .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
            .WithParsed(Global.AddSingleton);
        
        Global.AddSingleton<Logger>().ILog = new NLogger(Options.Instance.AppType, Options.Instance.Process, "");
        Log.Debug("Hello");

        Global.AddSingleton<EnvironmentV2>();
        Global.AddSingleton<ProtocolProvider>(); 
        Global.AddSingleton<MainThreadSynchronizationContext>();
        Global.AddSingleton<DbService>();
        Global.AddSingleton<IdService>();
        Global.AddSingleton<EntityService>();
        Global.AddSingleton<LoginService>(); 
        Global.AddSingleton<TimerService>();
        
        Global.Start();

        Log.Info(JsonConvert.SerializeObject(EnvironmentV2.Instance.systemInfo, Formatting.Indented));
    }

    public static void Update()
    {
        //
        Global.Update();
    }

    public static void Destroy()
    {
        //
        Global.Close();
    }

    public static void LateUpdate()
    {
        //
        Global.LateUpdate();
    }

    public static void FrameFinishUpdate()
    {
        //
        Global.FrameFinishedUpdate();
    }
}