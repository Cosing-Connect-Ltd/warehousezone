using Ganedata.Core.Entities.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    public class TenantEmailTemplates : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Template Id")]
        public int TemplateId { get; set; }
        [Required]
        [Display(Name = "Template Name")]
        public string EventName { get; set; }
        public WorksOrderNotificationTypeEnum NotificationType { get; set; }
        [Display(Name = "Select Site")]
        public int? SiteId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        [Display(Name = " Footer")]
        public string HtmlFooter { get; set; }
        [Required]
        [Display(Name = " Header")]
        public string HtmlHeader { get; set; }
        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(Name = " Body")]
        public string Body { get; set; }

        [ForeignKey("SiteId")]
        public virtual TenantWebsites TenantWebsites { get; set; }
    }

    public class TenantEmailTemplateVariable
    {
        [Key]
        [Display(Name = "Variable Id")]
        public int TenantEmailVariableId { get; set; }
        [Required]
        [Display(Name = "Variable Name")]
        public string VariableName { get; set; }

        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
    }
}