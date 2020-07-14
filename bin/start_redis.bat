chcp 437
@echo off

set cwdpath=%~dp0
set curpath=%cwdpath%../

set REDIS_PATH=%cwdpath%redis/
set REDIS_BIN=%REDIS_PATH%redis-server.exe
set REDIS_ACC_CONF=%cwdpath%../conf/redis_account.conf
set REDIS_LOGIN_CONF=%cwdpath%../conf/redis_login.conf
set REDIS_CONF=%cwdpath%../conf/redis.conf

taskkill /f /im redis-server.exe

start %REDIS_BIN% %REDIS_LOGIN_CONF% --port 7379

start %REDIS_BIN% %REDIS_ACC_CONF% --port 7380

start %REDIS_BIN% %REDIS_CONF% --port 7381

start %REDIS_BIN% %REDIS_CONF% --port 7382

start %REDIS_BIN% %REDIS_CONF% --port 7383

start %REDIS_BIN% %REDIS_CONF% --port 7384

start %REDIS_BIN% %REDIS_CONF% --port 7385

start %REDIS_BIN% %REDIS_CONF% --port 7386

start %REDIS_BIN% %REDIS_CONF% --port 7387

start %REDIS_BIN% %REDIS_CONF% --port 7388

start %REDIS_BIN% %REDIS_CONF% --port 7389

start %REDIS_BIN% %REDIS_CONF% --port 7390

