using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class AttLogsViewModel
    {
        public int Id { get; set; }
        public string DeviceID { get; set; }
        public string UserId { get; set; }
        public DateTime? Time { get; set; }
        public int Status { get; set; }
        public int Verify { get; set; }
        public string EventCode { get; set; }
    }
}