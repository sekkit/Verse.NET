 
@echo off

set cwdpath=%~dp0
set curpath=%cwdpath%..

set APP_CONF=%curpath%/conf/app.json

set BIN_PATH=%cwdpath%/netcoreapp3.1

echo %BIN_PATH%/Server.App.dll

start %BIN_PATH%/Server.App.exe --Config=%APP_CONF% --AppName="Login.App"
start %BIN_PATH%/Server.App.exe --Config=%APP_CONF% --AppName="Match.App"
start %BIN_PATH%/Server.App.exe --Config=%APP_CONF% --AppName="Master.App"
start %BIN_PATH%/Server.App.exe --Config=%APP_CONF% --AppName="Zone.App"