ps -ef|grep dotnet|grep Login.App|awk '{print $2}'|xargs kill -9
ps -ef|grep dotnet|grep Master.App|awk '{print $2}'|xargs kill -9
ps -ef|grep dotnet|grep Match.App|awk '{print $2}'|xargs kill -9
ps -ef|grep dotnet|grep Zone.App|awk '{print $2}'|xargs kill -9 