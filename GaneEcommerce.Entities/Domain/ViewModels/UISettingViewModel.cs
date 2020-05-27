using Ganedata.Core.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Models
{
    public class UISettingViewModel
    {
        public int Id { get; set; }
        public int? SiteId { get; set; }
        [Required]
        public UISettingItemViewModel UISettingItem { get; set; }
        [Required]
        public string Value { get; set; }
    }
}