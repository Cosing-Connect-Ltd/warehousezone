using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class AssetLogViewModel
    {
        public string piAddress { get; set; }
        public string uuid { get; set; }
        public int major { get; set; }
        public int minor { get; set; }
        public short measuredPower { get; set; }
        public short rssi { get; set; }
        public double accuracy { get; set; }
        public string proximity { get; set; }
        public string address { get; set; }
    }
}