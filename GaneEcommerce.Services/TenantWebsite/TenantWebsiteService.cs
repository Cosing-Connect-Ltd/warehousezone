using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
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
            return _currentDbContext.TenantWebsites.Where(u => u.IsDeleted != true && u.TenantId == TenantId);
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
        public IEnumerable<WebsiteContentPages> GetAllValidWebsiteContentPages(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteContentPages.Where(u => u.IsDeleted != true && u.TenantId == TenantId && u.SiteID == SiteId);
        }
        public WebsiteContentPages CreateOrUpdateWebsiteContentPages(WebsiteContentPages WebsiteContentPages, int UserId, int TenantId)
        {
            WebsiteContentPages.TenantId = TenantId;
            if (WebsiteContentPages.Id <= 0)
            {
                WebsiteContentPages.TenantId = TenantId;
                WebsiteContentPages.UpdateCreatedInfo(UserId);
                _currentDbContext.WebsiteContentPages.Add(WebsiteContentPages);
                _currentDbContext.SaveChanges();
            }
            else
            {
                var pages = _currentDbContext.WebsiteContentPages.AsNoTracking().FirstOrDefault(u => u.Id == WebsiteContentPages.Id);
                if (pages != null)
                {
                    WebsiteContentPages.CreatedBy = pages.CreatedBy;
                    WebsiteContentPages.DateCreated = pages.DateCreated;
                    WebsiteContentPages.TenantId = TenantId;
                    WebsiteContentPages.UpdateUpdatedInfo(UserId);
                    _currentDbContext.Entry(WebsiteContentPages).State = System.Data.Entity.EntityState.Modified;
                    _currentDbContext.SaveChanges();

                }

            }
            return WebsiteContentPages;
        }
        public WebsiteContentPages RemoveWebsiteContentPages(int Id, int UserId)
        {
            var pages = _currentDbContext.WebsiteContentPages.FirstOrDefault(u => u.Id == Id);
            if (pages != null)
            {
                pages.IsDeleted = true;
                pages.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(pages).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();

            }
            return pages;
        }

        public WebsiteContentPages GetWebsiteContentById(int Id)
        {
            return _currentDbContext.WebsiteContentPages.FirstOrDefault(u => u.Id == Id);
        }


        //ProductsWebsitesMaps
        public IEnumerable<ProductsWebsitesMap> GetAllValidProductsWebsitesMap(int TenantId)
        {
            return _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true);
        }
        public bool CreateOrUpdateProductsWebsitesMap(List<int> ProductIds, int siteId, int UserId, int TenantId)
        {

            var Toadd = ProductIds.Except(_currentDbContext.ProductsWebsitesMap
                    .Where(a => a.IsDeleted != true && a.SiteID == siteId).Select(a => a.ProductId)
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
        public IEnumerable<WebsiteSlider> GetAllValidWebsiteSlider(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteSliders.Where(u => u.IsDeleted != true && u.TenantId == TenantId && u.SiteID == SiteId);
        }
        public WebsiteSlider CreateOrUpdateProductswebsiteSlider(WebsiteSlider websiteSlider, int UserId, int TenantId)
        {
            if (websiteSlider.Id <= 0)
            {
                websiteSlider.TenantId = TenantId;
                websiteSlider.UpdateCreatedInfo(UserId);
                _currentDbContext.WebsiteSliders.Add(websiteSlider);
                _currentDbContext.SaveChanges();
            }
            else
            {
                var slider = _currentDbContext.WebsiteContentPages.AsNoTracking().FirstOrDefault(u => u.Id == websiteSlider.Id);
                if (slider != null)
                {
                    websiteSlider.CreatedBy = slider.CreatedBy;
                    websiteSlider.DateCreated = slider.DateCreated;
                    websiteSlider.TenantId = TenantId;
                    websiteSlider.UpdateUpdatedInfo(UserId);
                    _currentDbContext.Entry(websiteSlider).State = System.Data.Entity.EntityState.Modified;
                    _currentDbContext.SaveChanges();

                }

            }
            return websiteSlider;
        }
        public WebsiteSlider RemoveWebsiteSlider(int Id, int UserId)
        {
            var slider = _currentDbContext.WebsiteSliders.FirstOrDefault(u => u.Id == Id);
            if (slider != null)
            {
                slider.IsDeleted = true;
                slider.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(slider).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();

            }
            return slider;
        }

        public WebsiteSlider GetWebsiteSliderById(int Id)
        {
            return _currentDbContext.WebsiteSliders.FirstOrDefault(u => u.Id == Id);
        }

        //WebsiteNavigation

        public IEnumerable<WebsiteNavigation> GetAllValidWebsiteNavigation(int TenantId, int? SiteId)
        {
            return _currentDbContext.WebsiteNavigations.Where(u => u.IsDeleted != true && (!SiteId.HasValue || u.SiteID == SiteId) && u.TenantId == TenantId);
        }

        public IQueryable<NavigationProductsViewModel> GetAllValidWebsiteNavigations(int TenantId, int SiteId, int navigationId)
        {

            var websiteMap = _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true && u.SiteID == SiteId && u.TenantId == TenantId);
            var allProducts = _currentDbContext.ProductMaster.Where(m => m.TenantId == TenantId && m.IsDeleted != true);
            var navigationMap = _currentDbContext.ProductsNavigationMaps.Where(x => x.NavigationId == navigationId && x.IsDeleted != true);


            var res = (from w in websiteMap
                       join p in allProducts on w.ProductId equals p.ProductId
                       join nm in navigationMap on w.Id equals nm.ProductWebsiteMapId into tempNavigation
                       from d in tempNavigation.DefaultIfEmpty()
                       select new NavigationProductsViewModel()
                       {
                           Id = w.Id,
                           NavigationId = navigationId,
                           SiteID = w.SiteID,
                           ProductId = w.ProductId,
                           Name = p.Name,
                           SKUCode = p.SKUCode,
                           Description = p.Description,
                           ProductGroupName = p.ProductGroup.ProductGroup,
                           ProductCategoryName = p.ProductCategory.ProductCategoryName,
                           DepartmentName = p.TenantDepartment.DepartmentName,
                           IsActive = d.IsActive,
                           SortOrder = d.SortOrder
                       });

            return res;

        }
        public WebsiteNavigation CreateOrUpdateWebsiteNavigation(WebsiteNavigation websiteNavigation, int UserId, int TenantId)
        {
            if (websiteNavigation.Id <= 0)
            {
                websiteNavigation.TenantId = TenantId;
                websiteNavigation.UpdateCreatedInfo(UserId);
                _currentDbContext.WebsiteNavigations.Add(websiteNavigation);
                _currentDbContext.SaveChanges();
            }
            else
            {
                var updatedRecored = _currentDbContext.WebsiteNavigations.AsNoTracking().FirstOrDefault(u => u.Id == websiteNavigation.Id);
                if (updatedRecored != null)
                {
                    websiteNavigation.CreatedBy = updatedRecored.CreatedBy;
                    websiteNavigation.DateCreated = updatedRecored.DateCreated;
                    websiteNavigation.TenantId = TenantId;
                    websiteNavigation.UpdateUpdatedInfo(UserId);
                    _currentDbContext.Entry(websiteNavigation).State = System.Data.Entity.EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }
            }

            return websiteNavigation;
        }
        public WebsiteNavigation RemoveWebsiteNavigation(int Id, int UserId)
        {
            var navigation = _currentDbContext.WebsiteNavigations.FirstOrDefault(u => u.Id == Id);
            if (navigation != null)
            {
                navigation.IsDeleted = true;
                navigation.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(navigation).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();

            }
            return navigation;
        }
        public WebsiteNavigation GetWebsiteNavigationId(int NavigationId)
        {
            return _currentDbContext.WebsiteNavigations.FirstOrDefault(u => u.IsDeleted != true && u.Id == NavigationId);
        }

        public bool CreateOrUpdateWebsiteNavigationProducts(NavigationProductsViewModel navigationProduct, int UserId, int TenantId)
        {
            var navItem = _currentDbContext.ProductsNavigationMaps.Where(x => x.ProductWebsiteMapId == navigationProduct.Id
            && x.NavigationId == navigationProduct.NavigationId && x.TenantId == TenantId).FirstOrDefault();

            if (navItem == null)
            {
                ProductsNavigationMap productsNavigation = new ProductsNavigationMap();
                productsNavigation.NavigationId = navigationProduct.NavigationId;
                productsNavigation.ProductWebsiteMapId = navigationProduct.Id;
                productsNavigation.SortOrder = navigationProduct.SortOrder ?? 1;
                productsNavigation.TenantId = TenantId;
                productsNavigation.IsActive = navigationProduct.IsActive ?? true;
                productsNavigation.IsDeleted = false;
                productsNavigation.UpdateCreatedInfo(UserId);
                _currentDbContext.ProductsNavigationMaps.Add(productsNavigation);
            }
            else
            {
                navItem.IsActive = navigationProduct.IsActive ?? true;
                navItem.SortOrder = navigationProduct.SortOrder ?? 1;
                navItem.IsDeleted = false;
                navItem.UpdateUpdatedInfo(UserId);
            }

            _currentDbContext.SaveChanges();
            return true;
        }
    }
}