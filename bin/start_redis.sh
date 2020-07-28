#!/bin/bash

#export KBE_ROOT=$(cd ../src/; pwd)
#export KBE_RES_PATH="$KBE_ROOT/kbe/res/:$KBE_ROOT/assets:$KBE_ROOT/assets/res/:$KBE_ROOT/assets/scripts/"
#export KBE_BIN_PATH="$KBE_ROOT/kbe/bin/server/"

export SERVER_ROOT=$(cd ../; pwd)
#export SERVER_SRC_PATH="${SERVER_ROOT}/src/assets/scripts/"

export REDIS_ACC_CONF=${SERVER_ROOT}/conf/redis_account.conf
export REDIS_LOGIN_CONF=${SERVER_ROOT}/conf/redis_login.conf
export REDIS_CONF=${SERVER_ROOT}/conf/redis.conf

#echo KBE_ROOT = \"${KBE_ROOT}\"
#echo KBE_RES_PATH = \"${KBE_RES_PATH}\"
#echo KBE_BIN_PATH = \"${KBE_BIN_PATH}\" 
#echo REDIS_ACC_CONF = \"${REDIS_ACC_CONF}\"

mkdir -p ${SERVER_ROOT}/db/account/

nohup redis-server ${REDIS_LOGIN_CONF} --port 7379 > /dev/null 2>&1  &
nohup redis-server ${REDIS_ACC_CONF} --port 7380 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7381 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7382 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7383 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7384 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7385 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7386 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7387 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7388 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7389 > /dev/null 2>&1  &
nohup redis-server ${REDIS_CONF} --port 7390 > /dev/null 2>&1  &
