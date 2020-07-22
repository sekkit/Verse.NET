using System;
using DotNetty.Buffers;
using DotNetty.Common;
using DotNetty.KCP.thread;

namespace DotNetty.KCP
{
    public class ReadTask : ITask
    {
        private Ukcp kcp;

        private static readonly ThreadLocalPool<ReadTask> RECYCLER =
            new ThreadLocalPool<ReadTask>(handle => new ReadTask(handle));

        private readonly ThreadLocalPool.Handle recyclerHandle;

        private ReadTask(ThreadLocalPool.Handle recyclerHandle)
        {
            this.recyclerHandle = recyclerHandle;
        }

        public static ReadTask New(Ukcp kcp)
        {
            ReadTask readTask = RECYCLER.Take();
            readTask.kcp = kcp;
            return readTask;
        }

        public override void execute()
        {
            //long last_ts = DateTime.Now.Ticks;
            CodecOutputList<IByteBuffer> bufList = null;
            try {
                //Thread.sleep(1000);
                //查看连接状态
                if (!kcp.isActive()) {
                    return;
                }
                bool hasKcpMessage = false;
                long current = kcp.currentMs();
                var readQueue = kcp.ReadQueue;
                IByteBuffer byteBuf = null;
                for (;;)
                {
                    if (!readQueue.TryDequeue(out byteBuf))
                    {
                        break;
                    }
                    hasKcpMessage = true;
                    kcp.input(byteBuf, current);
                    byteBuf.Release();
                }
                if (!hasKcpMessage) {
                    return;
                }
                if (kcp.isStream()) {
                    while (kcp.canRecv()) {
                        if (bufList == null) {
                            bufList = CodecOutputList<IByteBuffer>.NewInstance();
                        }
                        kcp.receive(bufList);
                    }
                    int size = bufList.Count;
                    for (int i = 0; i < size; i++)
                    {
                        byteBuf = bufList[i];
                        //Console.WriteLine(string.Format("(READBYTE2){0}", (DateTime.Now.Ticks-last_ts)/10000.0));
                        readBytebuf(byteBuf,current);
                    }
                } else {
                    while (kcp.canRecv()) {
                        IByteBuffer recvBuf = kcp.mergeReceive();
                        //Console.WriteLine(string.Format("(READBYTE3){0}", (DateTime.Now.Ticks-last_ts)/10000.0));
                        readBytebuf(recvBuf,current);
                    }
                }
                //判断写事件
                if (!kcp.WriteQueue.IsEmpty&&kcp.canSend(false)) {
                    kcp.notifyWriteEvent();
                }
            } catch (Exception e) {
                kcp.KcpListener.handleException(kcp, e);
                Console.WriteLine(e);
            } finally {
                release();
                bufList?.Return();
            }
        }


        private void readBytebuf(IByteBuffer buf,long current)
        {
            kcp.LastRecieveTime = current;
            try
            {
                
                kcp.getKcpListener().handleReceive(buf, kcp);
            }
            catch (Exception throwable)
            {
                kcp.getKcpListener().handleException(kcp, throwable);
            }
            finally
            {
                buf.Release();
            }
        }

        private void release()
        {
            kcp.ReadProcessing.Set(false);
            kcp = null;
            recyclerHandle.Release(this);
        }
    }
}