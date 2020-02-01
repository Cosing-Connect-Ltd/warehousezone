using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Entities.Domain
{
    public class ResourceRequests : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string HolidayReason { get; set; }
        public DateTime RequestedDate { get; set; }
        public int Label { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int EventType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public string ResourceIDs { get; set; }
        public int Status { get; set; }
        
        public ResourceRequestStatusEnum RequestStatus { get; set; }
        public int? ActionedBy { get; set; }
        public string ActionReason { get; set; }
        public ResourceRequestTypesEnum RequestType { get; set; }
        public string Notes { get; set; }
        public virtual Resources Resources { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
    }
}