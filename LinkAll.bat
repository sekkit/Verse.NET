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