#!/bin/bash

cd "`dirname "$0"`"

sh stop_server.sh
sh stop_redis.sh 

sh start_redis.sh
sh start_server.sh