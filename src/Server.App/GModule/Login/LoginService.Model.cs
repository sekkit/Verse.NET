using Fenix;
using Fenix.Redis;
using Server.Config.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.GModule
{
    public partial class LoginService
    {
        private RedisDb AccountDb;
        private RedisDb SeqDb;

        protected override void onLoad()
        {
            this.AccountDb = Global.DbManager.GetDb(DbConfig.ACCOUNT);
            this.SeqDb = Global.DbManager.GetDb(DbConfig.SEQ);
        }

        public string CreateUid()
        {
            //19位uid
            long uid = this.SeqDb.NewSeqId(DbConfig.key_seq_uid);
            return "U"+ uid.ToString().PadLeft(19, '0');
        }
    }
}
