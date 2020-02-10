using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAgentDetector.Models
{
    public class Device
    {
        public bool is_mobile { get; set; }
        public bool is_tablet { get; set; }
        public bool is_desktop { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
    }
}
