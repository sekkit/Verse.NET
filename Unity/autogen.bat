@echo off

dotnet-mpc -i Client.App.csproj -o Assets\Plugins\Fenix\Gen\ClientAppMsg.g.cs -r ClientAppResolver
dotnet-mpc -i ..\src\Fenix.Runtime\Common -o Assets\Plugins\Fenix\Gen\FenixRuntimeMsg.g.cs -r FenixRuntimeResolver
dotnet-mpc -i ..\src\Shared\ -o Assets\Plugins\Fenix\Gen\SharedMsg.g.cs -r SharedResolver

