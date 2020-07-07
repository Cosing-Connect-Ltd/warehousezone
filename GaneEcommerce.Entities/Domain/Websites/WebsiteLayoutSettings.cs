using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteLayoutSettings : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        [Display(Name = "Subscribtion Panel Title")]
        public string SubscriptionPanelTitle { get; set; }
        [Display(Name = "Subscribtion Panel Description")]
        public string SubscriptionPanelDescription { get; set; }
        [Display(Name = "Subscribtion Image")]
        public string SubscriptionPanelImageUrl { get; set; }
        [Display(Name = "Subscribtion Handler URL")]
        public string SubscriptionHandlerUrl { get; set; }
        [Display(Name = "Show Supscription Panel")]
        public bool ShowSubscriptionPanel { get; set; }
        [ForeignKey("SiteId")]
        public virtual TenantWebsites TenantWebsites { get; set; }
    }
}