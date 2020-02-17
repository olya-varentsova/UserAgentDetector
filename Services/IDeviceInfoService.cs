using UserAgentDetector.Models;

namespace UserAgentDetector.Services
{
    interface IDeviceInfoService
    {
        public UserAgentDetectorItem getInfo(string userAgent);
    }
}
