using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAgentDetector.Models
{
    public class UserAgentDetectorItem
    {
        /*Response*/

        public string client_summary { get; set; }
        public string ua_family { get; set; }
        public string ua_version { get; set; }
        public string os_family { get; set; }
        public string os_version { get; set; }
        public string ua_type { get; set; }
        public bool bot_info { get; set; }
        public Os_meta os_meta { get; set; }
        public Device device { get; set; }
        public Client client { get; set; }
    }
}

