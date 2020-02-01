namespace Ganedata.Core.Entities.Domain
{
    using Ganedata.Core.Entities.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // This table will hold different loan types. eg. short term loan, long term loan and loan period will be defined in days. 
    public partial class TenantWebsitesSyncLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime RequestTime { get; set; }
        public string RequestUrl { get; set; }
        public int ErrorCode { get; set; }
        public bool Synced { get; set; }
        public int ResultCount { get; set; }
        public DateTime ResponseTime { get; set; }
        public string request { get; set; }
        public int SiteID { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
    }
}
