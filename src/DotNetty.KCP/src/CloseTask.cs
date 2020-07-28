using DotNetty.KCP.thread;

namespace DotNetty.KCP
{
    public class CloseTask:ITask
    {
        private Ukcp _ukcp;

        public CloseTask(Ukcp ukcp)
        {
            _ukcp = ukcp;
        }

        public override void execute()
        {
            _ukcp.internalClose();
        }
    }
}