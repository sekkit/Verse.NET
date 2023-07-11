 
using CommandLine;
using Helper;
using Module.Log;
using Module.Shared; 
using Service.Id;
using Service.Login;
using Service.Message;

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
        
        Global.AddSingleton<TypeService>();
        Global.AddSingleton<MainThreadSynchronizationContext>();
        Global.AddSingleton<IdService>();
        Global.AddSingleton<LoginService>();
        
        Global.Start();
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