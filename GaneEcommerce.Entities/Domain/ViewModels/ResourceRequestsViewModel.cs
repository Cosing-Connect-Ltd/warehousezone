using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Models
{

    public class ResourceRequestsViewModel
    {
        public int Id { get; set; }
        [DisplayName("Resource")]
        public int ResourceId { get; set; }
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        [DisplayName("Subject / Reason"),Required(ErrorMessage =" Reason is Required")]
        public string HolidayReason { get; set; }
        public DateTime RequestedDate { get; set; }
        public int Label { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int EventType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public int Status { get; set; }
        
        [Required]
        [DisplayName("Request Status")]
        public ResourceRequestStatusEnum RequestStatus { get; set; }
        public int? ActionedBy { get; set; }
        public string ActionReason { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Request type is required.")]
        [DisplayName("Request Type")]
        public ResourceRequestTypesEnum RequestType { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public string ResourceName { get; set; }
        public string RequestNotes { get; set; }
        public string RequestPeriod { get; set; }

        public string ApprovedDetails { get; set; }
    }


    public class ApproveRequestViewModel
    {
        public int ResourceHolidayRequestId { get; set; }

        public bool IsApproved { get; set; }

        public bool AcknowledgedWarning { get; set; }

        public string Reason { get; set; }
    }

    public class HolidayRequestResult
    {
        public bool HasWarning { get; set; }

        public string WarningMessage { get; set; }

        public int StaffsOnHoliday { get; set; }

        public int StaffsOnMeetings { get; set; }
    }

}