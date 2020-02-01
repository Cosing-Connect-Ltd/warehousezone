using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class Appointments
    {
        [Key]
        public int AppointmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int Label { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int EventType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public int? ResourceId { get; set; }
        public string ResourceIDs { get; set; }
        public int? OrderId { get; set; }
        public int? TenentId { get; set; }
        public bool IsCanceled { get; set; }
        public string CancelReason { get; set; }

        public virtual Resources AppointmentResources { get; set; }

        public virtual Order Orders { get; set; }
    }
}