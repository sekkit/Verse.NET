using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared.Protocol;
using System;
using Shared.DataModel;

namespace Server.UModule
{

    [ActorType(AType.SERVER)] 
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    [RuntimeData(typeof(Account))]
    public partial class Avatar : Actor
    {
        public Client.AvatarRef Client => (Client.AvatarRef)this.client;

        public Avatar()
        {
            
        }

        public Avatar(string uid) : base(uid)
        { 
        }

        public override void onLoad()
        {
            
        }

        public override void onClientEnable()
        {
            this.Client.client_on_api_test("", 1, (code) =>
            {
                Log.Info("client_on_api_test " + code.ToString());
            });
        }

        //public override void Update()
        //{
        //    base.Update();

        //    //var svc = GetService("MatchService");
        //    //svc.rpc_join_match("", 1, new Action<uint>((code) =>
        //    //{
        //    //    Log.Info(code.ToString());
        //    //}));
        //}

        [ServerApi]
        public void ChangeName(string name, Action<ErrCode> callback)
        {
            callback(ErrCode.OK);
        }

    }
}