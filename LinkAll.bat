@echo off

cd /d %~dp0
cd "src/Client.App"
rd /s /Q Runtime
mkdir Runtime
cd Runtime
mklink /J Actor "%~dp0src/Fenix.Runtime/Actor"
mklink /J Bootstrap "%~dp0src/Fenix.Runtime/Bootstrap"
mklink /J Common "%~dp0src/Fenix.Runtime/Common"
mklink /J Config "%~dp0src/Fenix.Runtime/Config"
mklink /J Global "%~dp0src/Fenix.Runtime/Global"
mklink /J Host "%~dp0src/Fenix.Runtime/Host"

cd /d %~dp0
cd "src/Client.App"
rd /s /Q Shared
mkdir Shared
cd Shared

mklink /J DataModel "%~dp0src/Shared/DataModel"
mklink /J Gen "%~dp0src/Shared/Gen"

cd /d %~dp0
cd "src/Server.App"
rd /s /Q Runtime
mkdir Runtime
cd Runtime
mklink /J Actor "%~dp0src/Fenix.Runtime/Actor"
mklink /J Bootstrap "%~dp0src/Fenix.Runtime/Bootstrap"
mklink /J Common "%~dp0src/Fenix.Runtime/Common"
mklink /J Config "%~dp0src/Fenix.Runtime/Config"
mklink /J Global "%~dp0src/Fenix.Runtime/Global"
mklink /J Host "%~dp0src/Fenix.Runtime/Host"
mklink /J Redis "%~dp0src/Fenix.Runtime/Redis"

cd /d %~dp0
cd "src/Server.App"
rd /s /Q Shared
mkdir Shared
cd Shared
mklink /J DataModel "%~dp0src/Shared/DataModel"
mklink /J Gen "%~dp0src/Shared/Gen"

cd /d %~dp0