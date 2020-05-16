using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Models
{
    public class TooltipViewModel
    {
        public int TooltipId { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Localization { get; set; }
        [Display(Name = "Tenant")]
        public int? TenantId { get; set; }
        public string TenantName { get; set; }
    }
}