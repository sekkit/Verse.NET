@echo off

set CWD=%~dp0

cd /d %CWD%
cd "src/Client.App"
rd /s /Q Shared
mkdir Shared
cd Shared

mklink /J DataModel "%CWD%src/Shared/DataModel"
mklink /J Gen "%CWD%src/Shared/Gen"

cd /d %CWD%
cd "src/Client.App"
rd /s /Q Runtime
mkdir Runtime
cd Runtime
mklink /J Actor "%CWD%src/Fenix.Runtime/Actor"
mklink /J Bootstrap "%CWD%src/Fenix.Runtime/Bootstrap"
mklink /J Common "%CWD%src/Fenix.Runtime/Common"
mklink /J Config "%CWD%src/Fenix.Runtime/Config"
mklink /J Global "%CWD%src/Fenix.Runtime/Global"
mklink /J Host "%CWD%src/Fenix.Runtime/Host"

cd /d %CWD%
cd "src/Server.App"
rd /s /Q Runtime
mkdir Runtime
cd Runtime
mklink /J Actor "%CWD%src/Fenix.Runtime/Actor"
mklink /J Bootstrap "%CWD%src/Fenix.Runtime/Bootstrap"
mklink /J Common "%CWD%src/Fenix.Runtime/Common"
mklink /J Config "%CWD%src/Fenix.Runtime/Config"
mklink /J Global "%CWD%src/Fenix.Runtime/Global"
mklink /J Host "%CWD%src/Fenix.Runtime/Host"
mklink /J Redis "%CWD%src/Fenix.Runtime/Redis"

cd /d %CWD%
cd "src/Server.App"
rd /s /Q Shared
mkdir Shared
cd Shared
mklink /J DataModel "%CWD%src/Shared/DataModel"
mklink /J Gen "%CWD%src/Shared/Gen"

cd /d %CWD%
cd "src/Fenix.Gen"
rd /s /Q Common
mklink /J Common "%CWD%src/Fenix.Runtime/Common"
rd /s /Q Protocol
mklink /J Protocol "%CWD%src/Shared/Gen/Protocol"

cd /d %CWD%

rem Unity 目录链接

cd /d %CWD%
cd "src/Unity/Assets/Scripts"

rd /s /Q Client.App
mkdir Client.App

cd Client.App 

rd /s /Q Shared
mkdir Shared
cd Shared

mklink /J DataModel "%CWD%src/Shared/DataModel"
mklink /J Gen "%CWD%src/Shared/Gen"

cd /d %CWD%
cd "src/Unity/Assets/Scripts/Client.App"
rd /s /Q Stub
del *.cs

mklink /J Stub "%CWD%src/Client.App/Stub"
rem for /f "delims==" %%k in ('dir "%CWD%src/Client.App/*.cs" /s /b') do (mklink /H "%%~nxk" "%%~k")
mklink /H App.cs "%CWD%src/Client.App/App.cs"
mklink /H Game.cs "%CWD%src/Client.App/Game.cs"
mklink /H Avatar.cs "%CWD%src/Client.App/Avatar.cs"

cd /d %CWD%
cd "src/Unity/Assets/Scripts/Client.App"


rd /s /Q Runtime
mkdir Runtime
cd Runtime
mklink /J Actor "%CWD%src/Fenix.Runtime/Actor"
mklink /J Bootstrap "%CWD%src/Fenix.Runtime/Bootstrap"
mklink /J Common "%CWD%src/Fenix.Runtime/Common"
mklink /J Config "%CWD%src/Fenix.Runtime/Config"
mklink /J Global "%CWD%src/Fenix.Runtime/Global"
mklink /J Host "%CWD%src/Fenix.Runtime/Host"

cd /d %CWD%
cd "src/Unity/Assets/Plugins"
rd /s /Q DotNetty.KCP
rd /s /Q DotNetty.TCP

mkdir DotNetty.KCP
mkdir DotNetty.TCP

cd DotNetty.KCP 
mklink /J kcp "%CWD%src/DotNetty.KCP/kcp"
mklink /J queue "%CWD%src/DotNetty.KCP/queue"
mklink /J thread "%CWD%src/DotNetty.KCP/thread"
mklink /J src "%CWD%src/DotNetty.KCP/src"

cd ../DotNetty.TCP
mklink /J src "%CWD%src/DotNetty.TCP/src"
