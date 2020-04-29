using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class TenantWebsiteService : ITenantWebsiteService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;

        public TenantWebsiteService(IApplicationContext currentDbContext, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _mapper = mapper;
        }
        public IEnumerable<TenantWebsites> GetAllValidTenantWebSite(int TenantId)
        {
            return _currentDbContext.TenantWebsites.Where(u => u.IsDeleted != true);
        }
        public TenantWebsites CreateOrUpdateTenantWebsite(TenantWebsites tenantWebsites, int UserId, int TenantId)
        {
            tenantWebsites.TenantId = TenantId;
            if (tenantWebsites.SiteID <= 0)
            {
                tenantWebsites.UpdateCreatedInfo(UserId);
                _currentDbContext.TenantWebsites.Add(tenantWebsites);
            }
            else
            {
                var website = _currentDbContext.TenantWebsites.AsNoTracking().FirstOrDefault(u => u.SiteID == tenantWebsites.SiteID);
                tenantWebsites.CreatedBy = website.CreatedBy;
                tenantWebsites.DateCreated = website.DateCreated;
                tenantWebsites.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(tenantWebsites).State = System.Data.Entity.EntityState.Modified;
            }
            _currentDbContext.SaveChanges();
            return tenantWebsites;
        }
        public bool RemoveTenantWebsite(int siteID, int UserId)
        {
            var website = _currentDbContext.TenantWebsites.Find(siteID);
            website.IsDeleted = true;
            website.UpdateUpdatedInfo(UserId);
            _currentDbContext.Entry(website).State = System.Data.Entity.EntityState.Modified;
            _currentDbContext.SaveChanges();
            return true;

        }

        //WebsiteContentPages
        public IEnumerable<WebsiteContentPages> GetAllValidWebsiteContentPages(int TenantId)
        {
            return _currentDbContext.WebsiteContentPages.Where(u => u.IsDeleted != true);
        }
        public WebsiteContentPages CreateOrUpdateWebsiteContentPages(WebsiteContentPages WebsiteContentPages, int UserId, int TenantId)
        {
            return default;
        }
        public bool RemoveWebsiteContentPages(int Id, int UserId)
        {
            return true;
        }


        //ProductsWebsitesMaps
        public IEnumerable<ProductsWebsitesMap> GetAllValidProductsWebsitesMap(int TenantId)
        {
            return _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true);
        }
        public bool CreateOrUpdateProductsWebsitesMap(List<int> ProductIds, int siteId, int UserId, int TenantId)
        {
            var Toadd = ProductIds.Except(_currentDbContext.ProductsWebsitesMap
                    .Where(a => a.IsDeleted != true).Select(a => a.ProductId)
                    .ToList()).ToList();
            if (Toadd.Count > 0)
            {
                foreach (var item in Toadd)
                {
                    ProductsWebsitesMap websitesMap = new ProductsWebsitesMap();
                    websitesMap.SiteID = siteId;
                    websitesMap.ProductId = item;
                    websitesMap.SortOrder = 1;
                    websitesMap.TenantId = TenantId;
                    websitesMap.UpdateCreatedInfo(UserId);
                    _currentDbContext.ProductsWebsitesMap.Add(websitesMap);
                }
                _currentDbContext.SaveChanges();
                return true;
            }

            return false;
        }
        public bool RemoveProductsWebsitesMap(int Id, int UserId)
        {
            return true;
        }

        //WebsiteSliders
        public IEnumerable<WebsiteSlider> GetAllValidWebsiteSlider(int TenantId)
        {
            return _currentDbContext.WebsiteSliders.Where(u => u.IsDeleted != true);
        }
        public WebsiteSlider CreateOrUpdateProductswebsiteSlider(WebsiteSlider websiteSlider, int UserId, int TenantId)
        {
            return default;
        }
        public bool RemoveWebsiteSlider(int Id, int UserId)
        {
            return true;
        }

        //WebsiteNavigation

        public IEnumerable<WebsiteNavigation> GetAllValidWebsiteNavigation(int TenantId)
        {
            return _currentDbContext.WebsiteNavigations.Where(u => u.IsDeleted != true);
        }

        public IQueryable<object> GetAllValidWebsiteNavigations(int TenantId)
        {
            return _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true).Select(prd => new
            {
                ProductId = prd.ProductId,
                Name = prd.ProductMaster.Name,
                SKUCode = prd.ProductMaster.SKUCode,
                Description = prd.ProductMaster.Description,
                BarCode = prd.ProductMaster.BarCode,
                Serialisable = prd.ProductMaster.Serialisable,
                BarCode2 = prd.ProductMaster.BarCode2,
                ProductGroupName = prd.ProductMaster.ProductGroup == null ? "" : prd.ProductMaster.ProductGroup.ProductGroup,
                ProductCategoryName = prd.ProductMaster.ProductCategory == null ? "" : prd.ProductMaster.ProductCategory.ProductCategoryName,
                DepartmentName = prd.ProductMaster.TenantDepartment.DepartmentName,
                Location = prd.ProductMaster.ProductLocationsMap.Where(a => a.IsDeleted != true).Select(x => x.Locations.LocationCode).FirstOrDefault().ToString(),
               

            });

        }
        public WebsiteNavigation CreateOrUpdateWebsiteNavigation(WebsiteNavigation websiteNavigation, int UserId, int TenantId)
        {
            return default;
        }
        public bool RemoveWebsiteNavigation(int Id, int UserId)
        {
            return true;
        }



    }
}