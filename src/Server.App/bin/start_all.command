#!/bin/bash

cd "`dirname "$0"`"

sh stop_redis.sh
sh start_redis.sh
 