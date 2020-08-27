for i in {1..1000}
do
    nohup dotnet ../src/Client.App/bin/Debug/netcoreapp3.1/Client.App.dll 2 >&1 &
    sleep 1
done