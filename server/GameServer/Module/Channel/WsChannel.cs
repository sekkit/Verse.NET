using DataModel.Shared.Message;
using Helper;
using Module.Shared;
using Module.User; 
using Service.Id; 
using WebSocketSharp;
using WebSocketSharp.Server;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace Module.Channel; 

/*
 * Channel is a bridge between Entity and Network interface
 */
public class WsChannel : WebSocketBehavior, IChannel
{
    protected Entity? entity { get; set; }
    
    protected override void OnError(ErrorEventArgs e)
    {
        base.OnError(e);
        
        Shared.Log.Error(e.ToString());
    }

    protected override void OnOpen()
    {
        base.OnOpen();
        
        if (entity == null)
        {
            entity = EntityHelper.createEntityWithLogin();
            entity.Attach(this);
        }
    }

    protected override void OnClose(CloseEventArgs e)
    {
        if (entity != null)
        {
            entity.Dispose();
        }
        
        base.OnClose(e);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        if (e.IsBinary)
        {
            if (e.RawData.Length <= 4)
            {
                return;
            }

            if (entity == null)
            {
                return;
            }
          
            var byteParts = ByteHelper.SplitBytes(e.RawData, 4);
            using (var ms = new MemoryStream(byteParts.FirstOrDefault()))
            {
                using (var reader = new EndianBinaryReader(EndianBitConverter.Little, ms))
                {
                    uint protoCodeNum = reader.ReadUInt32();
                    ProtoCode protoCode = Enum.Parse<ProtoCode>(protoCodeNum.ToString()); 
                    entity.Get<RpcModule>().Call(protoCode, byteParts.LastOrDefault(), this);
                }
            }
        }
    }
    
    public void SendMsg(IMessage msg)
    {
        Send(msg.Pack());
    }

    public void BroadcastMsg(string[] uids, IMessage msg)
    {
        var sids = IdService.Instance.GetSidByUids(uids);
        foreach (var sid in sids)
        { 
            Sessions[sid].WebSocket.Send(msg.Pack());
        }
    }

    public void BroadcastAll(IMessage msg)
    {
        Sessions.Broadcast(msg.Pack());
    }
} 
