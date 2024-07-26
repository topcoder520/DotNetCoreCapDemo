using DotNetCore.CAP;

namespace TestCap.services
{
    public interface ISubscriberService
    {
        public void CheckReceivedMessage(string msg);
    }

    public class CheckMsgService : ISubscriberService, ICapSubscribe
    {
        [CapSubscribe(AppConsts.PublishMsgEvent,Group ="group2")]
        public void CheckReceivedMessage(string msg)
        {
            Console.WriteLine("sub interface msg: "+msg);
        }
    }
}
