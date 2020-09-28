@echo off

set cwdpath=%~dp0
set curpath=%cwdpath%..

set MPCBIN=%cwdpath%mpc\win\mpc.exe

set CLIENT_PATH=%cwdpath%\CodeGen\net5.0\Fenix.CodeGen.exe

echo %CLIENT_PATH%
echo %curpath%\src\Client.App\Gen
echo %curpath%\src\Server.App\Gen

rd /s /Q "%curpath%\src\Client.App\Gen"
rd /s /Q "%curpath%\src\Server.App\Gen"
 
%CLIENT_PATH% -r %curpath%
%CLIENT_PATH% -c %curpath%
%CLIENT_PATH% -s %curpath% 

rem %MPCBIN% -i %curpath%\src\Client.App\Gen\Message\ -o %curpath%\src\Client.App\Gen\Message\Generated\ClientAppMsg.g.cs -r ClientAppResolver
rem %MPCBIN% -i %curpath%\src\Fenix.Runtime\Common\ -o %curpath%\src\Client.App\Gen\Message\Generated\FenixRuntimeMsg.g.cs -r FenixRuntimeResolver
rem %MPCBIN% -i %curpath%\src\Shared\ -o %curpath%\src\Client.App\Gen\Message\Generated\SharedMsg.g.cs -r SharedResolver

rem %MPCBIN% -i %curpath%\src\Client.App\Gen\Message\ -o %curpath%\src\Server.App\Gen\Message\Generated\ClientAppMsg.g.cs -r ClientAppResolver
rem %MPCBIN% -i %curpath%\src\Fenix.Runtime\Common\ -o %curpath%\src\Server.App\Gen\Message\Generated\FenixRuntimeMsg.g.cs -r FenixRuntimeResolver
rem %MPCBIN% -i %curpath%\src\Shared\ -o %curpath%\src\Server.App\Gen\Message\Generated\SharedMsg.g.cs -r SharedResolver

