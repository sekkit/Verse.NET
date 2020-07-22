using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared.Protocol;
using System;

namespace Server.UModule
{

    [ActorType(AType.SERVER)] 
    [AccessLevel(ALevel.CLIENT_AND_SERVER)]
    [RuntimeData(typeof(Account))]
    public partial class Avatar : Actor
    {
        //public Client.AvatarRef Client;

        public Avatar(string uid) : base(uid)
        {
            //Client.client_on_api_test("", 1, (code)=> { 
                
            //});
        }

        public override  void Update()
        {
            base.Update();

            var svc = GetService("MatchService");
            //svc.rpc_join_match("", 1, new Action<uint>((code) =>
            //{
            //    Log.Info(code.ToString());
            //}));
        } 
    }
}
