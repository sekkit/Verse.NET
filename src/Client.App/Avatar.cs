 
using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes; 
using Shared.Protocol;
using System;

namespace Client
{
    //Avatar.Client
    public partial class Avatar : ClientAvatar
    {
        public new Server.AvatarRef Server => (Server.AvatarRef)this.serverActor;
        
        //[ClientApi]
        //public void ApiTest(string uid, int match_type, Action<ErrCode> callback)
        //{
        //    Log.Info("Call=>client_api:ClientApiTest");
        //    callback(ErrCode.OK);
        //}

        [ClientApi]
        public void ApiTest(string uid, Action<ErrCode> callback)
        {
            Log.Info("Call=>client_api:ClientApiTest", uid);
            callback(ErrCode.OK);
        }

        [ClientApi]
        public void ApiTest2(string uid, int match_type)
        {
            Log.Info("Call=>client_api:ClientApiTest2");
        }
    }
}
