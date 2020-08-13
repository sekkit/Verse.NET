@echo off

set cwdpath=%~dp0
set curpath=%cwdpath%..

set CLIENT_PATH=%curpath%/src/Fenix.Gen/bin/Debug/netcoreapp3.1/Fenix.Gen.exe

%CLIENT_PATH% -r %curpath%
%CLIENT_PATH% -c %curpath%
%CLIENT_PATH% -s %curpath%

dotnet-mpc -i %curpath%\src\Client.App\Gen\Message\ -o %curpath%\src\Client.App\Gen\Message\Generated\ClientAppMsg.g.cs -r ClientAppResolver
dotnet-mpc -i %curpath%\src\Fenix.Runtime\Common\ -o %curpath%\src\Client.App\Gen\Message\Generated\FenixRuntimeMsg.g.cs -r FenixRuntimeResolver
dotnet-mpc -i %curpath%\src\Shared\ -o %curpath%\src\Client.App\Gen\Message\Generated\SharedMsg.g.cs -r SharedResolver

dotnet-mpc -i %curpath%\src\Client.App\Gen\Message\ -o %curpath%\src\Server.App\Gen\Message\Generated\ClientAppMsg.g.cs -r ClientAppResolver
dotnet-mpc -i %curpath%\src\Fenix.Runtime\Common\ -o %curpath%\src\Server.App\Gen\Message\Generated\FenixRuntimeMsg.g.cs -r FenixRuntimeResolver
dotnet-mpc -i %curpath%\src\Shared\ -o %curpath%\src\Server.App\Gen\Message\Generated\SharedMsg.g.cs -r SharedResolver

