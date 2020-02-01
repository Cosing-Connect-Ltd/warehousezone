using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    public class ResourceShifts : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeShiftID { get; set; }
        public DateTime Date { get; set; }
        public int WeekNumber { get; set; }
        public int? ShiftStatusId { get; set; }
        public string StatusType { get; set; }
        [Required]
        public int? ResourceId { get; set; }
        public int TerminalId { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        public virtual Resources Resources { get; set; }
        public virtual Terminals Terminals { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}