
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
using Server.Config.Db;
using CommandLine;

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
            if (args.Length == 0)
            {
                var cfgList = new List<RuntimeConfig>();

                var obj = new RuntimeConfig();
                obj.ExternalIp = "auto";
                obj.InternalIp = "auto";
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
                obj.ExternalIp = "auto";
                obj.InternalIp = "auto";
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
                obj.ExternalIp = "auto";
                obj.InternalIp = "auto";
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
                obj.ExternalIp = "auto";
                obj.InternalIp = "auto";
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

                Environment.SetEnvironmentVariable("AppName", "Login.App");

                //Bootstrap.Start(new Assembly[] { typeof(UModule.Avatar).Assembly}, cfgList, OnInit, isMultiProcess:true); //单进程模式

                Bootstrap.Start(new Assembly[] { typeof(UModule.Avatar).Assembly }, cfgList, OnInit, isMultiProcess: false); //单进程模式
            }
            else
            {
                Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       //将命令行参数，设置到进程的环境变量
                       Environment.SetEnvironmentVariable("AppName", o.AppName);

                       using (var sr = new StreamReader(o.Config))
                       {
                           var cfgList = JsonConvert.DeserializeObject<List<RuntimeConfig>>(sr.ReadToEnd());
                           Bootstrap.Start(new Assembly[] { typeof(UModule.Avatar).Assembly }, cfgList, OnInit, isMultiProcess: true); //分布式
                       }
                   });
            }
        }

        static void OnInit()
        {
            Global.DbManager.LoadDb(DbConfig.account_db);
        }
    }
}
