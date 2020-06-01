using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("UISettingItems")]
    public class UISettingItem : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public string DefaultValue { get; set; }
        public int DisplayOrder { get; set; }
        public UISettingItemInputType InputType { get; set; }
        public int? WebsiteThemeId { get; set; }
        [NotMapped]
        [DisplayFormat(NullDisplayText = "No Theme")]
        public WebsiteThemeEnum? WebsiteTheme
        {
            get
            {
                return WebsiteThemeId.HasValue ? (WebsiteThemeEnum?)WebsiteThemeId.Value : null;
            }
        }
        public int? WarehouseThemeId { get; set; }
        [NotMapped]
        [DisplayFormat(NullDisplayText = "No Theme")]
        public WarehouseThemeEnum? WarehouseTheme
        {
            get
            {
                return WarehouseThemeId.HasValue ? (WarehouseThemeEnum?)WarehouseThemeId.Value : null;
            }
        }
    }
}