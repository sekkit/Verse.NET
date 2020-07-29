
![image](https://github.com/sekkit/Fenix/tree/master/src/images/opencraft.png)

   
# To Warcraft 3 fans:
OpenCraft is not Warcraft III, but for those who's been waiting for Warcraft IV for many years,
this project is aimed at making Next Generation RTS games, learning from Blizzard's design.

Fenix is part of OpenCraft project, it is a distributed server designed for https://github.com/sekkit/OpenCraft


# Fenix Project
 
A distributed game server featuring asynchronous event-driven networking, KCP/TCP/Websockets support.
From MMORPG to small projects, Fenix is designed to be stable, simple, and super easy to scale.
With the power of .NetCore, Fenix can be run on MacOS/Linux/Windows.

Fenix is in a early stage of development, but is also being used in commercial Game projects.


## Get Started

1. Click LinkAll.bat to automatically creat symbol links to Client.App and Server.App projects

2. Open Fenix.sln with Visual Studio 2019(.netcore 3.1 SDK installed)

3. Build solution

3. Go to ./bin folder, run start_redis.bat and start_server.bat

4. Now build and run Client.App to see what happens.

5. Unity3D support

6. Any stars or contributions are welcomed.

## Features

1. RPC calls are super easy
    
2. Switch between KCP/TCP/websockets super easy

3. Messagepack/Zeroformatter/Protobuf are easily supported

4. Able to call Actors and Hosts through ActorRef anywhere(reference of real net objects)
 
5. Architecture specifically designed for Game developers, easier than any other distributable server framework.

## Contribute

We gladly accept community contributions.

* Issues: Please report bugs using the Issues section of GitHub
* Source Code Contributions: 
 * See [C# Coding Style](https://github.com/Azure/DotNetty/wiki/C%23-Coding-Style) for reference on coding style.
