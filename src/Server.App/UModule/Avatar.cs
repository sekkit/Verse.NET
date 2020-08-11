using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared.Protocol;
using System;
using Shared.DataModel;
using Server.DataModel;

namespace Server.UModule
{
    [ActorType(AType.SERVER)] 
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    [PersistentData(typeof(User))]
    public partial class Avatar : Actor
    {
        public new Client.AvatarRef Client => (Client.AvatarRef)this.clientActor;

        public Avatar()
        {
            
        }

        public Avatar(string uid) : base(uid)
        {

        }

        protected override void onLoad()
        {
            
        }

        protected override void onClientEnable()
        {
            base.onClientEnable();

            //向客户端发消息的前提是，已经绑定了ClientAvatarRef,而且一个Actor的ClientRef不是全局可见的，只能在该host进程上调用
            this.Client.client_on_api_test("hello", (code) =>
            {
                Log.Info("client_on_api_test", code);
            });
        }

        [ServerApi]
        public void ChangeName(string name, Action<ErrCode> callback)
        {
            Get<Account>().uid = name;

            callback(ErrCode.OK);
        }

        [ServerOnly]
        public void OnMatchOk()
        {

        }
    }
}
