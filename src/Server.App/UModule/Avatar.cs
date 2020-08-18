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
    [PersistentData(typeof(User), DbConfig.USER)]
    public partial class Avatar : ServerAvatar
    {
        public new Client.AvatarRef Client => (Client.AvatarRef)this.clientActor;
         

        protected override void onLoad()
        {
            Log.Info("Avatar.User>", GetRuntime<User>()); 
        }

        protected override void onClientEnable()
        {
            base.onClientEnable();

            //向客户端发消息的前提是，已经绑定了ClientAvatarRef,而且一个Actor的ClientRef不是全局可见的，只能在该host进程上调用
            this.Client.client_api_test("hello", (code) =>
            {
                Log.Info("client_on_api_test", code);
            });
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
