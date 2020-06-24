using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class AbandonedCartSetting : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsite { get; set; }
        public string NotificationEmailTemplate { get; set; }
        public string NotificationEmailSubjectTemplate { get; set; }
        public int NotificationDelay { get; set; }
        public bool IsNotificationEnabled { get; set; }
    }
}