using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class TooltipViewModel
    {
        public int TooltipId { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Localization { get; set; }
        public int? TenantId { get; set; }
    }
}