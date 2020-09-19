
/*
 * (c)2020 Sekkit.com
 * Fenix是一个基于Actor网络模型的分布式游戏服务器
 * server端通信都是走tcp
 * server/client之间可以走tcp/kcp/websockets
 */
 
using Fenix;
using Fenix.Config;  
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Server.Config;
using CommandLine;
using MessagePack.Resolvers;
using MessagePack;
using System.Linq;
using DotNetty.Common;
using static DotNetty.Common.ResourceLeakDetector;

namespace Server
{
    public class Options
    {
        [Option('a', "AppName", Required = false, HelpText = "AppName")]
        public string AppName { get; set; }

        [Option('c', "Config", Required = false, HelpText = "Config")]
        public string Config { get; set; }
    }

    class App
    { 
        static void Main(string[] args)
        {
            ResourceLeakDetector.Level = DetectionLevel.Disabled;
#if ENABLE_IL2CPP
            StaticCompositeResolver.Instance.Register( 
                 MessagePack.Resolvers.ClientAppResolver.Instance,
                 MessagePack.Resolvers.FenixRuntimeResolver.Instance,
                 MessagePack.Resolvers.SharedResolver.Instance,
                 MessagePack.Unity.UnityResolver.Instance,
                 MessagePack.Unity.Extension.UnityBlitResolver.Instance,
                 MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
                 MessagePack.Resolvers.AttributeFormatterResolver.Instance,
                 MessagePack.Resolvers.BuiltinResolver.Instance,
                 MessagePack.Resolvers.StandardResolver.Instance
            );
#endif
            if (args.Length == 0)
            {
                var cfgList = new List<RuntimeConfig>();

                var obj = new RuntimeConfig();
                obj.ExternalIP = "auto";
                obj.InternalIP = "0.0.0.0";
                obj.Port = 17777; //auto
                obj.AppName = "Login.App";
                obj.HeartbeatIntervalMS = 5000;
                obj.ClientNetwork = NetworkType.TCP;
                obj.DefaultActorNames = new List<string>()
                {
                    "LoginService"
                };

                cfgList.Add(obj);

                obj = new RuntimeConfig();
                obj.ExternalIP = "auto";
                obj.InternalIP = "0.0.0.0";
                obj.Port = 17778; //auto
                obj.AppName = "Match.App";
                obj.HeartbeatIntervalMS = 5000;
                obj.ClientNetwork = NetworkType.TCP;
                obj.DefaultActorNames = new List<string>()
                {
                    "MatchService"
                };

                cfgList.Add(obj);

                obj = new RuntimeConfig();
                obj.ExternalIP = "auto";
                obj.InternalIP = "0.0.0.0";
                obj.Port = 17779; //auto
                obj.AppName = "Master.App";
                obj.HeartbeatIntervalMS = 5000;
                obj.ClientNetwork = NetworkType.TCP;
                obj.DefaultActorNames = new List<string>()
                {
                    "MasterService"
                };

                cfgList.Add(obj);

                obj = new RuntimeConfig();
                obj.ExternalIP = "auto";
                obj.InternalIP = "0.0.0.0";
                obj.Port = 17780; //auto
                obj.AppName = "Zone.App";
                obj.HeartbeatIntervalMS = 5000;
                obj.ClientNetwork = NetworkType.TCP;
                obj.DefaultActorNames = new List<string>()
                {
                    "ZoneService"
                };

                cfgList.Add(obj);
                
                using (var sw = new StreamWriter("app.json", false, Encoding.UTF8))
                {
                    var content = JsonConvert.SerializeObject(cfgList, Formatting.Indented);
                    sw.Write(content);
                }

                //for Debug purpose
                Environment.SetEnvironmentVariable("AppPath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../"));
                Bootstrap.StartSingleProcess(new Assembly[] { typeof(UModule.Avatar).Assembly }, cfgList, OnInit); //单进程模式
                
            //    foreach (var cfg in cfgList)
            //       if (cfg.AppName == "Login.App")
            //           Bootstrap.StartMultiProcess(new Assembly[] { typeof(UModule.Avatar).Assembly }, cfg, OnInit); //分布式
            }
            else
            {
                Environment.SetEnvironmentVariable("AppPath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../"));

                Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   { 
                       Environment.SetEnvironmentVariable("AppName", o.AppName);

                       using (var sr = new StreamReader(o.Config))
                       {
                           var cfgList = JsonConvert.DeserializeObject<List<RuntimeConfig>>(sr.ReadToEnd());
                           foreach (var cfg in cfgList)
                               if(cfg.AppName == o.AppName)
                                   Bootstrap.StartMultiProcess(new Assembly[] { typeof(UModule.Avatar).Assembly }, cfg, OnInit, 
                                       cfgList.Where(m=>m.AppName != o.AppName).ToList()); //分布式
                       }
                   });
            }
        }

        static void OnInit()
        {
            DbConfig.Init();

            foreach (var cfg in DbConfig.Instance.CfgDic)
            {
                Fenix.Global.DbManager.LoadDb(cfg.Value);
            }
        }
    }
}
