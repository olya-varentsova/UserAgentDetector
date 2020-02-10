using System;
using Microsoft.AspNetCore.Mvc;
using DeviceDetectorNET;
using UserAgentDetector.Models;
using DeviceDetectorNET.Parser;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Client;

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
            DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);

            DeviceDetector deviceDetector = new DeviceDetector(userAgent);
            deviceDetector.Parse();
            UserAgentDetectorItem userAgentItem = new UserAgentDetectorItem();
            DeviceDetectorResult deviceDetectorResult = ((DeviceDetectorResult)DeviceDetector.GetInfoFromUserAgent(userAgent).Match);

            ClientMatchResult client = deviceDetector.GetClient().Match;
            userAgentItem.client_summary = client.Name + " " + client.Version;

            userAgentItem.ua_family = deviceDetectorResult.BrowserFamily;
            userAgentItem.ua_version = client.Version;

            userAgentItem.os_family = deviceDetectorResult.Os.Name;
            userAgentItem.os_version = deviceDetectorResult.Os.Version;

            userAgentItem.ua_type = deviceDetector.GetClient().Match.Type;
            userAgentItem.bot_info = deviceDetector.IsBot();

            userAgentItem.os_meta = new Os_meta();
            userAgentItem.os_meta.name = deviceDetectorResult.Os.Name;
            userAgentItem.os_meta.short_name = deviceDetectorResult.Os.ShortName;
            userAgentItem.os_meta.version = deviceDetectorResult.Os.Version;
            userAgentItem.os_meta.platform = deviceDetectorResult.Os.Platform;

            /* String[] info = client.ToString().Split("\n");
             userAgentItem.ua_rendering_engine = info.Length > 4 ? info[4] : "";
             userAgentItem.ua_rendering_engine_version = info.Length > 5 ? info[5] : "";*/

            userAgentItem.device = new Device();
            userAgentItem.device.is_mobile = deviceDetector.IsMobile();
            userAgentItem.device.is_tablet = deviceDetector.IsTablet();
            userAgentItem.device.is_desktop = deviceDetector.IsDesktop();
            userAgentItem.device.brand = deviceDetectorResult.DeviceBrand;
            userAgentItem.device.model = deviceDetectorResult.DeviceModel;

            userAgentItem.client = new Client();
            userAgentItem.client.bot = deviceDetector.IsBot();
            userAgentItem.client.user = !deviceDetector.IsBot();
            return userAgentItem;
        }
    }
}
