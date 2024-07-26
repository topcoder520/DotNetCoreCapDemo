using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestCap.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublisherConsumerController : ControllerBase
    {
        private readonly ICapPublisher publisher;


        public PublisherConsumerController(ICapPublisher publisher)
        {
            this.publisher = publisher;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            publisher.Publish(AppConsts.PublishMsgEvent,"hello ya");
            return new String[] { "ok","not"};
        }

        [NonAction]
        [CapSubscribe(AppConsts.PublishMsgEvent,Group ="group1")]
        public void ConsumeMsg(string data) {
            Console.WriteLine("Consumer: "+data);
        }

    }
}
