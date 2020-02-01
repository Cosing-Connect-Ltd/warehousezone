using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class AttLogs
    {
        [Key]
        public int Id { get; set; }
        public string DeviceSerialNo { get; set; }
        public string UserPIN { get; set; }
        public DateTime? Time { get; set; }
        public int Status { get; set; }
        public int Verify { get; set; }
        public string EventCode { get; set; }
    }
}