using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class TenantEmailConfig : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Config Id")]
        public int TenantEmailConfigId { get; set; }
        [Required]
        [Display(Name = "Smtp Host")]
        public string SmtpHost { get; set; }
        [Required]
        [Display(Name = "Smtp Port")]
        public int SmtpPort { get; set; }
        [Required]
        [Display(Name = "Smtp User Email")]
        public string UserEmail { get; set; }
        [Required]
        [Display(Name = "Smtp Password")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Enable SSL")]
        public bool EnableSsl { get; set; }
        [DefaultValue(true)]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public string DailyEmailDispatchTime { get; set; }
        public bool? EnableRelayEmailServer { get; set; }
    }

    public class TenantEmailNotificationQueue
    {
        [Key]
        public int TenantEmailNotificationQueueId { get; set; }
        public int? OrderId { get; set; }
        public string EmailSubject { get; set; }
        public string AttachmentVirtualPath { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        public int TenantEmailTemplatesId { get; set; }
        [ForeignKey("TenantEmailTemplatesId")]
        public virtual TenantEmailTemplates TenantEmailTemplates { get; set; }
        public int? AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public virtual Appointments Appointment { get; set; }
        public bool? IsNotificationCancelled { get; set; }
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

        [ForeignKey("InvoiceMasterId")]
        public virtual InvoiceMaster InvoiceMaster { get; set; }
    }
}

