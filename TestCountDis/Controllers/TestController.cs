using Microsoft.AspNetCore.Mvc;

namespace TestCountDis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly HyperLogLog _hyperlog;

        public TestController(HyperLogLog hyperlog)
        {
            _hyperlog = hyperlog;
        }

        [HttpPost]
        public ActionResult AddKey(string key)
        {
            _hyperlog.Add(key);
            return Ok();
        }


        [HttpGet]
        public ActionResult GetCount()
        {
            return Ok(_hyperlog.Count());
        }
    }
}
