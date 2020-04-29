using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public interface ITenantWebsiteService
    {
        // TenantWebsite//
        IEnumerable<TenantWebsites> GetAllValidTenantWebSite(int TenantId);
        TenantWebsites CreateOrUpdateTenantWebsite(TenantWebsites tenantWebsites, int UserId, int TenantId);
        bool RemoveTenantWebsite(int siteID, int UserId);

        // WebsiteContentPages//

        IEnumerable<WebsiteContentPages> GetAllValidWebsiteContentPages(int TenantId);
        WebsiteContentPages CreateOrUpdateWebsiteContentPages(WebsiteContentPages WebsiteContentPages, int UserId, int TenantId);
        bool RemoveWebsiteContentPages(int Id, int UserId);


        //ProductsWebsitesMaps
        IEnumerable<ProductsWebsitesMap> GetAllValidProductsWebsitesMap(int TenantId);
        bool CreateOrUpdateProductsWebsitesMap(List<int> ProductIds,int SiteId, int UserId, int TenantId);
        bool RemoveProductsWebsitesMap(int Id, int UserId);

        //WebsiteSliders
        IEnumerable<WebsiteSlider> GetAllValidWebsiteSlider(int TenantId);
        WebsiteSlider CreateOrUpdateProductswebsiteSlider(WebsiteSlider websiteSlider, int UserId, int TenantId);
        bool RemoveWebsiteSlider(int Id, int UserId);

        //WebsiteNavigation

        IEnumerable<WebsiteNavigation> GetAllValidWebsiteNavigation(int TenantId);
        IQueryable<object> GetAllValidWebsiteNavigations(int TenantId);
        WebsiteNavigation CreateOrUpdateWebsiteNavigation(WebsiteNavigation websiteNavigation, int UserId, int TenantId);
        bool RemoveWebsiteNavigation(int Id, int UserId);

    }
}