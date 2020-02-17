using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAgentDetector.Models;
using UserAgentDetector.Services;
using Microsoft.Extensions.DependencyInjection;

namespace UserAgentDetector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        // GET: api/Main
        [HttpGet("/useragent")]
        public UserAgentDetectorItem detectUserAgent([FromQuery(Name = "ua")]string userAgent = "")
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                userAgent = Request.Headers["User-Agent"];
            }
            IDeviceInfoService deviceInfoService = Request.HttpContext.RequestServices.GetService<IDeviceInfoService>();
            
            return deviceInfoService.getInfo(userAgent);
        }

        [HttpGet("/useragent/test-result")]
        public string getTestResult()
        {
            ITestService testService = Request.HttpContext.RequestServices.GetService<ITestService>();

            return testService.runTest();
        }
    }
}
