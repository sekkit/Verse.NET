using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using Shared.Protocol;
using System;

namespace UModule
{
    [RuntimeData(typeof(Account))]
    public partial class Avatar : User
    {
        public Avatar(string uid): base(uid)
        {

        }

        public override void Update()
        {
            base.Update();

            var svc = GetService("MatchService");
            svc.rpc_join_match("", 1, new Action<uint>((code) => 
            {
                Log.Info(code.ToString());
            }));

            //var svc2 = GetService<MatchServiceRef>("");
            //svc2.rpc_join_match("", 1, (code) =>
            //{
            //    Log.Info(code.ToString());
            //});
        }

        [ClientApi]
        public void ClientApiTest(string uid, int match_type, Action<ErrCode> callback)
        {
            Log.Info("Call=>client_api:ClientApiTest");
            callback(ErrCode.OK);
        }
    }
}
