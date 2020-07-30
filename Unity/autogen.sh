#!/bin/bash

dotnet-mpc -i Client.App.csproj -o ./Assets/Plugins/Fenix/Gen/
dotnet-mpc -i ../src/Fenix.Runtime/Common -o ./Assets/Plugins/Fenix/Gen/
dotnet-mpc -i ../src/Shared/ -o ./Assets/Plugins/Fenix/Gen/


