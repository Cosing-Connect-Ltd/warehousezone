using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderPTenantEmailRecipient  
    {
        [Key]
        public int OrderPTenantEmailRecipientId { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public int? PPropertyId { get; set; }
        [ForeignKey("PPropertyId")]
        public virtual PProperty PProperty { get; set; }

        public int? PTenantId { get; set; }
        [ForeignKey("PTenantId")]
        public virtual PTenant PTenant { get; set; }

        public int? LastEmailNotificationId { get; set; }
        [ForeignKey("LastEmailNotificationId")]
        public virtual TenantEmailNotificationQueue LastEmailNotification { get; set; }
        public int? AccountContactId { get; set; }

        public string EmailAddress { get; set; }

        public bool? IsDeleted { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}