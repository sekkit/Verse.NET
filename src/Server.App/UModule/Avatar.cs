using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared.Protocol;
using System;
using Shared.DataModel;
using Server.DataModel;
using System.Collections.Generic;
using Server.Config;

namespace Server.UModule
{
    [RequireModule(typeof(ItemModule))]
    [PersistentData(typeof(User), DbConfig.USER)]
    public partial class Avatar : ServerAvatar
    {
        protected Avatar self => this;

        public User User => GetPersist<User>();

        public new Client.AvatarRef Client => (Client.AvatarRef)this.clientActor;
        
        protected override void onLoad()
        {
            Log.Info("Avatar.User>", GetPersist<User>());

            if(User.Exp == 0)
            {
                User.Exp = 1; 
            }
        }

        protected override void onClientEnable()
        {
            base.onClientEnable();

            //向客户端发消息的前提是，已经绑定了ClientAvatarRef,
            //而且一个Actor的ClientRef不是全局可见的，只能在该host进程上调用 

            this.SyncAll();
        }

        public void Save() //保存user到db
        {
            SaveDataToDb(DbConfig.USER, typeof(User), self.User);
        }

        public void Sync()
        {
            //this.Client.client_on_sync_user(User.PackChanged());
        }

        public void SyncAll()
        {
            this.Client?.client_on_sync_user(User.Pack());
        }

        [ServerApi]
        public void ChangeName(string name, Action<ErrCode> callback)
        { 
            callback(ErrCode.OK);
        }

        [ServerOnly]
        public void OnMatchOk()
        {

        }
    }
}
