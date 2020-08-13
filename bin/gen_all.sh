#!/bin/bash

export cwdpath=$(pwd)
export curpath=${cwdpath}/..

export CLIENT_PATH=${curpath}/src/Fenix.Gen/bin/Debug/netcoreapp3.1/Fenix.Gen.dll

dotnet ${CLIENT_PATH} -r ${curpath}
dotnet ${CLIENT_PATH} -c ${curpath}
dotnet ${CLIENT_PATH} -s ${curpath}

chmod 777 ./mpc/osx/*

./mpc/osx/mpc -i ${curpath}/src/Client.App/Gen/Message/ -o ${curpath}/src/Client.App/Gen/Message/Generated/ClientAppMsg.g.cs -r ClientAppResolver
./mpc/osx/mpc -i ${curpath}/src/Fenix.Runtime/Common/ -o ${curpath}/src/Client.App/Gen/Message/Generated/FenixRuntimeMsg.g.cs -r FenixRuntimeResolver
./mpc/osx/mpc -i ${curpath}/src/Shared/ -o ${curpath}/src/Client.App/Gen/Message/Generated/SharedMsg.g.cs -r SharedResolver

./mpc/osx/mpc -i ${curpath}/src/Client.App/Gen/Message/ -o ${curpath}/src/Server.App/Gen/Message/Generated/ClientAppMsg.g.cs -r ClientAppResolver
./mpc/osx/mpc -i ${curpath}/src/Fenix.Runtime/Common/ -o ${curpath}/src/Server.App/Gen/Message/Generated/FenixRuntimeMsg.g.cs -r FenixRuntimeResolver
./mpc/osx/mpc -i ${curpath}/src/Shared/ -o ${curpath}/src/Server.App/Gen/Message/Generated/SharedMsg.g.cs -r SharedResolver

