 
using Fenix;
using Fenix.Common;
using Fenix.Common.Attributes;
using MessagePack;
using Shared.DataModel;
using Shared.Protocol;
using System;

namespace Client
{
    //Avatar.Client
    [PersistentData(typeof(User))]
    public partial class Avatar : ClientAvatar
    {
        public new Server.AvatarRef Server => (Server.AvatarRef)this.serverActor;
#if ENABLE_IL2CPP || !DEBUG
        protected Avatar self => this;
#else
        protected dynamic self => this;
#endif

        public User User => GetPersist<User>();

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

        [ClientApi]
        public void OnSyncUser(byte[] data)
        {
            this.User.UnPack(data);
            Log.Info("sync", this.User); 
        }
    }
}
