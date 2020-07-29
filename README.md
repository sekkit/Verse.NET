
![opencraft](https://user-images.githubusercontent.com/25851211/88820973-97f71800-d1f4-11ea-93cc-7be9bad7d147.png)

   
# To Warcraft 3 fans:
OpenCraft is not Warcraft III, but for those who's been waiting for Warcraft IV for many years,
this project is for you. Targeting Next Generation RTS games while learning from Blizzard's design.

Fenix is part of OpenCraft project, a distributed server designed for https://github.com/sekkit/OpenCraft


# Fenix Project
 
A distributed game server featuring asynchronous event-driven networking, KCP/TCP/Websockets support.
From MMORPG to small projects, Fenix is designed to be stable, simple, and super easy to scale.
With the power of .NetCore, Fenix can be run on MacOS/Linux/Windows.

Fenix is in a early stage of development, but is also being used in commercial Game projects.


## Get Started

1. Run Unity3D and open Unity project at root path

2. Open Fenix.sln with Visual Studio 2019(.NetCore 3.1+ SDK installed)

3. Build solution

3. Go to ./bin folder, run start_redis.bat and start_server.bat

4. (Unity3d) Press play button in Unity OR (Native) Now build and run Client.App

5. Unity3D support

6. Any stars or contributions are welcomed

## Features

1. RPC decleration
to tag a funtion with ServerApi, ServerOnly, ClientOnly you can create a RPC protocol without effort.
```csharp
    [RuntimeData(typeof(MatchData))]
    public partial class MatchService : Service
    {
        public MatchService(string name): base(name)
        {
        }

        public void onLoad()
        {
        }

        //public new string UniqueName => nameof(MatchService);

        [ServerApi] 
        public void JoinMatch(string uid, int match_type, Action<ErrCode> callback)
        {
            Log.Info("Call=>server_api:JoinMatch");
            callback(ErrCode.OK);
        } 

        [ServerOnly]
        [CallbackArgs("code", "user")]
        public void FindMatch(string uid, Action<ErrCode, Account> callback)
        {
            callback(ErrCode.OK, null);
        }
    }
```
    
2. Switch between KCP/TCP/websockets super easy (just open conf/app.json)

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
3. Messagepack/Zeroformatter/Protobuf are easily supported, AutoGen takes care of serialization&deserializtion

4. Able to call Actors and Hosts through ActorRef anywhere(reference of real net objects)
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
5. Architecture specifically designed for Game developers, easier than any other distributable server framework.
 ```
A Host is a container(Process) for many/single actors.
An Actor is an entity with state, able to make rpc calls, has its own lifecycle within the Host.

Scaling is achieved through multiprocesses, interprocess communication are mainly through Global cache or TCP/KCP networking.

Design principle comes from Bigworld, and microservices. 
The simple, the better.
 ```

## Contribute

We gladly accept community contributions.

* Issues: Please report bugs using the Issues section of GitHub
* Source Code Contributions: 
* See [C# Coding Style](https://github.com/Azure/DotNetty/wiki/C%23-Coding-Style) for reference on coding style.
