using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class Shifts : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? WeekNumber { get; set; }
        public int? WeekDay { get; set; }
        public TimeSpan? ExpectedHours { get; set; }
        public TimeSpan? TimeBreaks { get; set; }
        public int? LocationsId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public virtual Resources Resources { get; set; }
        public virtual TenantLocations TenantLocations { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
    }
}