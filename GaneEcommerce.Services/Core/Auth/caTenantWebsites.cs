using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    [Serializable]
    public class caTenantWebsites
    {
        public int SiteID { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public string SiteApiUrl { get; set; }
        public string ApiToken { get; set; }
        public int WarehouseId { get; set; }
        public int TenantId { get; set; }
        public WebsiteThemeEnum Theme { get; set; }

    }
}