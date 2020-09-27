using Fenix.Common;
using Fenix.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fenix.Redis
{
    public class LockData<T> : IDisposable
    {
        protected RedisDb Db;

        protected string Key;

        protected string Token;

        protected T Data;

        protected bool NoSave = false;

        protected bool hasData = false;

        public LockData(RedisDb db, string key)
        {
            Db = db;
            Key = key;
            Enter();
        }

        protected virtual bool Enter()
        {
            Log.Info("begin_enter_key", Db.Name, Key);
            var result = Db.Lock(Key);
            if (!result.Item1)
            {
                NoSave = true;
                throw new Exception("LockDataException");
            }

            Token = result.Item2;

            hasData = Db.HasKey(Key);

            Data = Db.Get<T>(Key);
            Log.Info("end_enter_key", Db.Name, Key);
            return true;
        }

        public virtual T GetValue()
        {
            return Data;
        }

        public virtual bool SetValue(T value)
        {
            var result = Db.SetWithoutLock(Key, value);
            hasData = true;
            Data = value;
            return result;
        }

        public virtual bool Delete()
        {
            var result = Db.Delete(Key);
            hasData = false;
            return result;
        }

        public virtual bool Flush()
        {
            bool result = false;
            try
            {
                if (NoSave || !hasData)
                    result = true;
                else
                    result = Db.SetWithoutLock(Key, Data);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        protected virtual bool Exit()
        {
            Log.Info("begin_exit_key", Db.Name, Key);
            var result = Flush();
            result = Db.Unlock(Key, Token) && result;
            Log.Info("end_exit_key", Db.Name, Key, result);            
            return result;
        }

        public void Dispose()
        {
            Exit();
        }
    }
}
