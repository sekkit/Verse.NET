 
@echo off

set cwdpath=%~dp0
set curpath=%cwdpath%..

set APP_CONF=%curpath%/conf/app.json

set BIN_PATH=%cwdpath%/netcoreapp3.1

echo %BIN_PATH%/Server.App.dll

start dotnet %BIN_PATH%/Server.App.dll --Config=%APP_CONF% --AppName="Account.App"
rem start dotnet %BIN_PATH%/Server.App.dll --Config=%APP_CONF% --AppName="Match.App"