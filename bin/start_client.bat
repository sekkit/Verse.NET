@echo off

set cwdpath=%~dp0
set curpath=%cwdpath%..

set CLIENT_PATH=%curpath%/src/Client.App/bin/Debug/netcoreapp3.1/Client.App/netcoreapp3.1/Client.App.exe

start %CLIENT_PATH%