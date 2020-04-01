using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class TerminalsLog
    {
        [Key]
        [Display(Name = "Log Id")]
        public Guid TerminalLogId { get; set; }
        [Display(Name = "Terminal Id")]
        public int? TerminalId { get; set; }
        [MaxLength(50)]
        [Display(Name = "Log Type")]
        public string TerminalLogType { get; set; }
        [Display(Name = "Request Date")]
        public DateTime DateRequest { get; set; }
        [Display(Name = "Sent Code")]
        public int? SentCount { get; set; }
        [Display(Name = "Acknowledgment")]
        public Boolean? Ack { get; set; }
        [Display(Name = "Received Count")]
        public int? RecievedCount { get; set; }
        public string Response { get; set; }
        public string ResponseText { get; set; }
        public string AdditonalInfo { get; set; }
        public string clientIp { get; set; }
        public string ServerIp { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }

        public int? SiteID { get; set; }
        public DateTime? DateUpdated { get; set; }
        [ForeignKey("TerminalId")]
        public virtual Terminals Terminals { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
    }
}