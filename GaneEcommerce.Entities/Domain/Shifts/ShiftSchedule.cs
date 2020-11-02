using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ShiftSchedule
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string RecurrenceInfo { get; set; }
        [Display(Name = "Employee")]
        public int ResourceId { get; set; }
        public int? TenantId { get; set; }
        [Display(Name = "Time Breaks")]
        public TimeSpan? TimeBreaks { get; set; }
        public int? LocationsId { get; set; }
        public bool IsCanceled { get; set; }
        [ForeignKey("ResourceId")]
        public virtual Resources Resources { get; set; }

        public virtual TimeSpan? ExpectedHours
        {
            get
            {
                return EndTime - StartTime;
            }
        }
    }
}