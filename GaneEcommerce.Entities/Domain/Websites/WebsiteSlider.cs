using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteSlider : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        public string Image { get; set; }
        public string ImageAltTag { get; set; }
        public string Text { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLinkUrl { get; set; }
        public string TextColor { get; set; }
        public string ForeColor { get; set; }
        public decimal ForeColorOpacity { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
    }
}