
![opencraft](https://user-images.githubusercontent.com/25851211/88820973-97f71800-d1f4-11ea-93cc-7be9bad7d147.png)

   
# To Warcraft 3 fans:
OpenCraft is not Warcraft III, but for those who's been waiting for Warcraft IV for many years,
this project is for you. Targeting Next Generation RTS games while learning from Blizzard's design.

Fenix is part of OpenCraft project, a distributed server designed for https://github.com/sekkit/OpenCraft

##### QQ Group: 1150593832

# Fenix Project

A distributed game server featuring asynchronous event-driven networking, KCP/TCP/Websockets support.
From MMORPG to small projects, Fenix is designed to be stable, simple, and super easy to scale.
With the power of .NetCore, Fenix can be run on MacOS/Linux/Windows.

Fenix is in a early stage of development, but is also being used in reallife Game projects.

#### [RELEASES] https://github.com/sekkit/Fenix/releases<br>

## Status
   

· [![status](https://img.shields.io/badge/Passed%3F-yes-green.svg)]()&nbsp;&nbsp;&nbsp;&nbsp;macOS/Linux/Windows supported (for client sdk supports macOS/Linux/Windows/Android/iOS)<br>
· [![status](https://img.shields.io/badge/Passed%3F-yes-green.svg)]()&nbsp;&nbsp;&nbsp;&nbsp;Unity Mono/IL2CPP compilation passed<br>
· [![status](https://img.shields.io/badge/Passed%3F-yes-green.svg)]()&nbsp;&nbsp;&nbsp;&nbsp;KCP/TCP benchmark passed (Recommend to use IL2CPP compilation for client, maybe server also**UNTESTED)<br>
· [![status](https://img.shields.io/pypi/status/ansicolortags.svg)]()&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Not all feature completed currently.<br>

## Get Started

1. Run Unity3D and open Unity project at root path

2. Open Fenix.sln with Visual Studio 2019(.NetCore 3.1+ SDK installed)

3. Build solution

3. Go to ./bin folder, run start_redis.bat and start_server.bat

4. (Unity3d) Press play button in Unity OR (Native) Now build and run Client.App 

## Features

1. RPC decleration

```csharp
    //to tag a function with ServerApi, ServerOnly, ClienApi you can create a RPC protocol without effort.
    [RuntimeData(typeof(MatchData))]
    public partial class MatchService : Service
    {  
        //public new string UniqueName => nameof(MatchService);

        [ServerApi] 
        public async Task JoinMatch(string uid, int match_type, Action<ErrCode> callback)
        {
            Log.Info("Call=>server_api:JoinMatch");
            var result = await JoinMatchAsync(uid, match_type);
            if(result == true)
               callback(ErrCode.OK);
            else
               callback(ErrCode.ERROR);
        } 

        [ServerOnly]
        [CallbackArgs("code", "user")]
        public void FindMatch(string uid, Action<ErrCode, Account> callback)
        {
            callback(ErrCode.OK, null);
        }
    }
```

2. Unified DB access API for both cache and storage with Distributed Lock. <br>
Redis for cache and Rocksdb for persistency(also in redis protocol^^)

```csharp
   var db = Global.DbManager.LoadDb(DbConfig.account_db); //Create and get db
   db.Set("PLAYER0001", userData);
```
3. Switch between KCP/TCP/websockets super easy (just open conf/app.json)

```json
[
    {
       "AppName": "Login.App",
       "ExternalIp": "auto",
       "InternalIp": "auto",
       "Port": 17777,
       "DefaultActorNames": [
         "LoginService"
       ],
       "HeartbeatIntervalMS": 5000,
       "ClientNetwork": "NetworkType.KCP"
    }
]
  ```
4. Messagepack/Zeroformatter/Protobuf are easily supported <br>AutoGen takes care of serialization&deserializtion
```csharp
//AUTOGEN, do not modify it! 
namespace Shared.Message
{
    [MessageType(ProtocolCode.CHANGE_NAME_REQ)]
    [MessagePackObject]
    public class ChangeNameReq : IMessageWithCallback
    {
        [Key(0)]
        public String name { get; set; }


        [Key(199)]
        public Callback callback
        {
            get => _callback as Callback;
            set => _callback = value;
        } 

        [MessagePackObject]
        public class Callback
        {
            [Key(0)]
            [DefaultValue(ErrCode.ERROR)]
            public ErrCode code { get; set; } = ErrCode.ERROR;

        }

    }
}
```

5. Able to call Actors and Hosts through ActorRef anywhere
 ```csharp
var svc = this.GetActorRef<LoginServiceRef>(); 
svc.rpc_login("username", "password", (code2, uid, hostId, hostName, hostAddress) =>
{
     if (code2 != ErrCode.OK)
     {
          Log.Error("login_failed"); 
          return;
     }
     Log.Info("login_ok");
     //...
});
 ```
 
6. Architecture specifically designed for Game developers<br>Liteweight and easy to use than any other distributable server framework.
 ```
A Host is a container(Process) for many/single actors.
An Actor is an entity with state, able to make rpc calls, has its own lifecycle within the Host.

Scaling is achieved through multiprocesses, interprocess communication are mainly through Global cache or TCP/KCP networking.

Design principle comes from Bigworld, and microservices. 
The simple, the better.
 ```
 
7. State/Stateless Actor & Disaster Recovery & Access Control
  ```csharp
using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared.Protocol;
using System;
using Shared.DataModel;

namespace Server.UModule
{
       [RequireModule(typeof(UserModule))]
       [RequireModule(typeof(ItemModule))]
       [RequireModule(typeof(RoomModule))]
       [RequireModule(typeof(MatchModule))]
       [RequireModule(typeof(FightModule))]
       [PersistentData(typeof(User), DbConfig.USER)] //Tag an actor with RuntimeData/PersistData, the DB saving process is taken care of.
       [ActorType(AType.SERVER)] //this actor exists only in server side
       [AccessLevel(ALevel.CLIENT_AND_SERVER)] //this actor can be accessed from both client/server 
       //When disaster happens, the actor can be recovered from previous state
       public partial class Avatar : Actor
       {
           public Client.AvatarRef Client => (Client.AvatarRef)this.client; 

           public override void onLoad()
           { 
           }

           public override void onClientEnable()
           {
              //向客户端发消息的前提是，已经绑定了ClientAvatarRef,而且一个Actor的ClientRef不是全局可见的，只能在该host进程上调用
              this.Client.client_on_api_test("", (code) =>
              {
                  Log.Info("client_on_api_test", code);
              });
           }

           [ServerApi]
           public void ChangeName(string name, Action<ErrCode> callback)
           {
              Get<Account>().uid = name;

              callback(ErrCode.OK);
           }

           [ServerOnly]
           public void OnMatchOk()
           { 
           }
       }
}

  ```
 

## Contribute

We gladly accept community contributions.

* Issues: Please report bugs using the Issues section of GitHub
* Source Code Contributions: 
* See [C# Coding Style](https://github.com/Azure/DotNetty/wiki/C%23-Coding-Style) for reference on coding style.
