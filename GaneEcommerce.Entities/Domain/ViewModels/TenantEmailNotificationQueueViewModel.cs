using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class TenantEmailNotificationQueueViewModel
    {
        public int TenantEmailNotificationQueueId { get; set; }
        public int? OrderId { get; set; }
        public string EmailSubject { get; set; }
        public string AttachmentVirtualPath { get; set; }
        public int TenantEmailTemplatesId { get; set; }
        public int? AppointmentId { get; set; }
        public bool? IsNotificationCancelled { get; set; }
        public virtual int TenantEmailConfigId { get; set; }
        public bool ScheduledProcessing { get; set; }
        public DateTime ScheduledProcessingTime { get; set; }
        public DateTime? ActualProcessingTime { get; set; }
        public DateTime? WorkOrderStartTime { get; set; }
        public DateTime? WorkOrderEndTime { get; set; }
        public string WorksOrderResourceName { get; set; }
        public bool ProcessedImmediately { get; set; }
        public string CustomRecipients { get; set; }
        public string CustomCcRecipients { get; set; }
        public string CustomBccRecipients { get; set; }
        public string CustomEmailMessage { get; set; }
        public int? InvoiceMasterId { get; set; }

    }
}