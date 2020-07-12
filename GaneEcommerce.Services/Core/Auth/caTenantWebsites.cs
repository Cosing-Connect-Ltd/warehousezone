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
        public int TenantId { get; set; }
        public int SiteID { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public WebsiteThemeEnum Theme { get; set; }
        public int DefaultWarehouseId { get; set; }
        public string HostName { get; set; }
        public string BaseFilePath { get; set; }
        public string Logo { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string InstaGramUrl { get; set; }
        public string FooterText { get; set; }
        public string ContactPageUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsCollectionAvailable { get; set; }
        public bool IsDeliveryAvailable { get; set; }
        public string WebsiteContactAddress { get; set; }
        public string WebsiteContactPhone { get; set; }
        public string WebsiteContactEmail { get; set; }
    }
}