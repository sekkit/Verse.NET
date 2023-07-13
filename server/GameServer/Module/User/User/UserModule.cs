using System.Text;
using DataModel.Shared.Message;
using MemoryPack;
using Module.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Module.User;

public class UserModule : EntityModule 
{
    public async Task SyncField(string fieldName)
    {
        object value = self.User[fieldName];
        //var bytes = MemoryPackSerializer.Serialize(value);
        // using (MemoryStream ms = new MemoryStream())
        // {
        //     using (BsonDataWriter datawriter = new BsonDataWriter(ms))
        //     {
        //         JsonSerializer serializer = new JsonSerializer();
        //         serializer.Serialize(datawriter, value);
        //     }
        //     var ntf = new SyncFieldNtf
        //     {
        //         Key = fieldName,
        //         Value = ms.ToArray(),
        //     };
        //     Shared.Log.Info("SyncField "+fieldName);
        //     await self.Get<RpcModule>().Notify(ProtoCode.ON_SYNC_FIELD, ntf);
        // }

        string json = JsonConvert.SerializeObject(value);
        var bytes = Encoding.UTF8.GetBytes(json);
        var ntf = new SyncFieldNtf
        {
            Key = fieldName,
            Value = bytes,
        };
        Shared.Log.Info("SyncField "+fieldName);
        await self.Get<RpcModule>().Notify(ProtoCode.ON_SYNC_FIELD, ntf);
    }
    
    public override void Start()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void LateUpdate()
    {
        
    }

    public override void FrameFinishedUpdate()
    {
        
    }

    public override void Destroy()
    {
        
    }
}