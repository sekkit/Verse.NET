#!/bin/bash

export CWD=$(pwd)/

cd ${CWD}

cd "src/Client.App"
rm -rf Shared
mkdir Shared
cd Shared

ln -s "${CWD}src/Shared/DataModel" DataModel
ln -s "${CWD}src/Shared/Gen" Gen

cd ${CWD}
cd "src/Client.App"
rm -rf Runtime
mkdir Runtime
cd Runtime
ln -s "${CWD}src/Fenix.Runtime/Actor" Actor
ln -s "${CWD}src/Fenix.Runtime/Bootstrap" Bootstrap
ln -s "${CWD}src/Fenix.Runtime/Common" Common
ln -s "${CWD}src/Fenix.Runtime/Config" Config
ln -s "${CWD}src/Fenix.Runtime/Global" Global
ln -s "${CWD}src/Fenix.Runtime/Host" Host

cd ${CWD}
cd "src/Server.App"
rm -rf Runtime
mkdir Runtime
cd Runtime
ln -s "${CWD}src/Fenix.Runtime/Actor" Actor
ln -s "${CWD}src/Fenix.Runtime/Bootstrap" Bootstrap
ln -s "${CWD}src/Fenix.Runtime/Common" Common
ln -s "${CWD}src/Fenix.Runtime/Config" Config
ln -s "${CWD}src/Fenix.Runtime/Global" Global
ln -s "${CWD}src/Fenix.Runtime/Host" Host
ln -s "${CWD}src/Fenix.Runtime/Redis" Redis

cd ${CWD}
cd "src/Server.App"
rm -rf Shared
mkdir Shared
cd Shared
ln -s "${CWD}src/Shared/DataModel" DataModel
ln -s "${CWD}src/Shared/Gen" Gen

cd ${CWD}
cd "src/Fenix.Gen"
rm -rf Common
ln -s "${CWD}src/Fenix.Runtime/Common" Common
rm -rf Protocol
ln -s "${CWD}src/Shared/Gen/Protocol" Protocol

cd ${CWD} 

cd ${CWD}
cd "src/Unity/Assets/Scripts"

rm -rf Client.App
mkdir Client.App

cd Client.App 

rm -rf Shared
mkdir Shared
cd Shared

ln -s "${CWD}src/Shared/DataModel" DataModel
ln -s "${CWD}src/Shared/Gen" Gen

cd ${CWD}
cd "src/Unity/Assets/Scripts/Client.App"
rm -rf Stub
find . -name '*.cs' -type f -delete

ln -s "${CWD}src/Client.App/Stub" Stub
ln "${CWD}src/Client.App/App.cs" App.cs 
ln "${CWD}src/Client.App/Game.cs" Game.cs
ln "${CWD}src/Client.App/Avatar.cs" Avatar.cs

cd ${CWD}
cd "src/Unity/Assets/Scripts/Client.App"


rm -rf Runtime
mkdir Runtime
cd Runtime
ln -s "${CWD}src/Fenix.Runtime/Actor" Actor
ln -s "${CWD}src/Fenix.Runtime/Bootstrap" Bootstrap
ln -s "${CWD}src/Fenix.Runtime/Common" Common
ln -s "${CWD}src/Fenix.Runtime/Config" Config
ln -s "${CWD}src/Fenix.Runtime/Global" Global
ln -s "${CWD}src/Fenix.Runtime/Host" Host

cd ${CWD}
cd "src/Unity/Assets/Plugins"
rm -rf DotNetty.KCP
rm -rf DotNetty.TCP

mkdir DotNetty.KCP
mkdir DotNetty.TCP

cd DotNetty.KCP 
ln -s "${CWD}src/DotNetty.KCP/kcp" kcp
ln -s "${CWD}src/DotNetty.KCP/queue" queue
ln -s "${CWD}src/DotNetty.KCP/thread" thread
ln -s "${CWD}src/DotNetty.KCP/src" src 

cd ../DotNetty.TCP
ln -s "${CWD}src/DotNetty.TCP/src" src
