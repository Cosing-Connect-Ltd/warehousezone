using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Models
{
    public class UISettingItemViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Selector { get; set; }
        public string DisplayName { get; set; }
        public int DisplayOrder { get; set; }
        public string DefaultValue { get; set; }
        public UISettingItemInputType InputType { get; set; }
    }
}