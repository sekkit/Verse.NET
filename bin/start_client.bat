@echo off

set cwdpath=%~dp0
set curpath=%cwdpath%..

set CLIENT_PATH=%cwdpath%/Client.App/net5.0/Client.App.exe

start %CLIENT_PATH%