using DataModel.Shared.Message;
using Helper;
using Module.Shared;
using Module.User;
using Service.Entity;
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
    }

    protected override void OnClose(CloseEventArgs e)
    {
        if (entity != null)
        {
            entity.Dispose();
            entity = null;
        }
        
        base.OnClose(e);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        if (e.IsBinary)
        {
            if (e.RawData.Length < 4)
            {
                return;
            }
          
            //var byteParts = ByteHelper.SplitBytes(e.RawData, 4);
            using (var ms = new MemoryStream(e.RawData))//byteParts.FirstOrDefault()))
            {
                BinaryReader br = new BinaryReader(ms);
                    
                using (var reader = new EndianBinaryReader(EndianBitConverter.Little, ms))
                {
                    uint protoCodeNum = reader.ReadUInt32();
                    ProtoCode code = Enum.Parse<ProtoCode>(protoCodeNum.ToString());
                    
                    if (entity == null)
                    {
                        if (code == ProtoCode.LOGIN)
                        {
                            entity = EntityHelper.createEntityWithLogin();
                            entity.Attach(this);
                        }
                        else
                        { 
                            this.Close(CloseStatusCode.PolicyViolation, "");
                        }
                    }
                    else
                    {
                        if (code == ProtoCode.LOGIN)
                        {
                            Shared.Log.Warning("duplicate login!");
                            return;
                        }
                    }

                    if (entity == null)
                    {
                        Shared.Log.Error("user not logged in");
                        return;
                    }
                        
                    //entity.Get<RpcModule>().Call(protoCode, byteParts.LastOrDefault(), this);
                    var data = Ext.SubArray(e.RawData, 4, e.RawData.Length-4);// reader.ReadBytes(e.RawData.Length-4);
                    
                    entity.Get<RpcModule>().Call(code, data, this);
                }
            }
        }
    }

    public string GetChannelId()
    {
        return ID;
    }

    public async Task Reply(ProtoCode code, Msg msg)
    {
        using (var ms = new MemoryStream())
        {
            using (var bw = new EndianBinaryWriter(EndianBitConverter.Little, ms))
            {
                //var code = type.GetCustomAttribute<ProtocolAttribute>().Code; 
                bw.Write((uint)code);
                bw.Write(msg.Pack());
                
                Send(ms.ToArray());
            }
        }
    }

    public async Task Notify(ProtoCode code, string[] uids, Msg msg)
    {
        var type = msg.GetType();
        bool isNotify = type.Name.EndsWith("Ntf");  
        if (isNotify)
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new EndianBinaryWriter(EndianBitConverter.Little, ms))
                {
                    //var code = type.GetCustomAttribute<ProtocolAttribute>().Code; 
                    bw.Write((uint)code);
                    bw.Write(msg.Pack());
                    
                    var task = Task.Run(() =>
                    {
                        var sids = EntityService.Instance.GetSidByUids(uids);
                        foreach (var sid in sids)
                        { 
                            Sessions[sid].WebSocket.Send(ms.ToArray());
                        }
                    });

                    await task;
                }
            }
        }
        else
        {
            var tcs = new TaskCompletionSource<Msg>();
            MainThreadSynchronizationContext.Instance.Post(() => { tcs.SetCanceled(); });
        }
    }

    public async Task NotifyAll(ProtoCode code, Msg msg)
    {
        var type = msg.GetType();
        bool isNotify = type.Name.EndsWith("Ntf");  
        if (isNotify)
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new EndianBinaryWriter(EndianBitConverter.Little, ms))
                {
                    //var code = type.GetCustomAttribute<ProtocolAttribute>().Code; 
                    bw.Write((uint)code);
                    bw.Write(msg.Pack());
                    
                    var task = Task.Run(() =>
                    {
                        Sessions.Broadcast(ms.ToArray());
                    });

                    await task;
                }
            }
        }
        else
        {
            var tcs = new TaskCompletionSource<Msg>();
            MainThreadSynchronizationContext.Instance.Post(() => { tcs.SetCanceled(); });
        } 
    } 
} 
