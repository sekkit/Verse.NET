#!/bin/bash

dotnet-mpc -i ./Client.App.csproj -o ./Assets/Scripts/Client/Gen/Message/Generated/ClientAppMsg.g.cs -r ClientAppResolver
dotnet-mpc -i ../src/Fenix.Runtime/Common -o ./Assets/Scripts/Client/Gen/Message/Generated/FenixRuntimeMsg.g.cs -r FenixRuntimeResolver
dotnet-mpc -i ../src/Shared/ -o ./Assets/Scripts/Client/Gen/Message/Generated/SharedMsg.g.cs -r SharedResolver


