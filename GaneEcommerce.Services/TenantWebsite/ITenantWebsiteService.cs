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

        IEnumerable<WebsiteContentPages> GetAllValidWebsiteContentPages(int TenantId,int SiteId);
        WebsiteContentPages CreateOrUpdateWebsiteContentPages(WebsiteContentPages WebsiteContentPages, int UserId, int TenantId);
        WebsiteContentPages RemoveWebsiteContentPages(int Id, int UserId);
        WebsiteContentPages GetWebsiteContentById(int Id);


        //ProductsWebsitesMaps
        IEnumerable<ProductsWebsitesMap> GetAllValidProductsWebsitesMap(int TenantId);
        bool CreateOrUpdateProductsWebsitesMap(List<int> ProductIds,int SiteId, int UserId, int TenantId);
        bool RemoveProductsWebsitesMap(int Id, int UserId);

        //WebsiteSliders
        IEnumerable<WebsiteSlider> GetAllValidWebsiteSlider(int TenantId,int SiteId);
        WebsiteSlider CreateOrUpdateProductswebsiteSlider(WebsiteSlider websiteSlider, int UserId, int TenantId);
        WebsiteSlider RemoveWebsiteSlider(int Id, int UserId);
        WebsiteSlider GetWebsiteSliderById(int Id);
        //WebsiteNavigation

        IEnumerable<WebsiteNavigation> GetAllValidWebsiteNavigation(int TenantId, int? SiteId);
        IQueryable<object> GetAllValidWebsiteNavigations(int TenantId, int? SiteId);
        WebsiteNavigation CreateOrUpdateWebsiteNavigation(WebsiteNavigation websiteNavigation, int UserId, int TenantId);
        WebsiteNavigation RemoveWebsiteNavigation(int Id, int UserId);
        WebsiteNavigation GetWebsiteNavigationId(int NavigationId);

    }
}