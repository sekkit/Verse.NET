using DotNetty.KCP.thread;

namespace DotNetty.KCP
{
    public class ConnectTask : ITask
    {
        private Ukcp _ukcp;

        public ConnectTask(Ukcp ukcp)
        {
            _ukcp = ukcp;
        }

        public override void execute()
        {
            _ukcp.close();
        }
    }
}