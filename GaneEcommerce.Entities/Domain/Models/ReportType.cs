using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ReportType:PersistableEntity<int>
    {
        public ReportType()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int ReportTypeId { get; set; }
        public string  TypeName { get; set; }
        public bool? AllowChargeTo { get; set; } 
        public bool? AllowReportType { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}