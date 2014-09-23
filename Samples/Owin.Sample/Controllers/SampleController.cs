using System.Collections.Generic;
using System.Web.Http;

namespace Owin.Sample.Controllers
{
    [RoutePrefix("sample")]
    public class SampleController : ApiController
    {
        [Route("test/{x}/{i}")]
        public IEnumerable<string> Get(string x, int i)
        {
            return new[] { "value1", "value2" };
        }

        [Route("{value}")]
        public IEnumerable<string> Get(string value)
        {
            return new[] { value, "value2" };
        }
    }
}
