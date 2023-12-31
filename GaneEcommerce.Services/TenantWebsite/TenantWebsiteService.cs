﻿using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using WebGrease.Css.Extensions;

namespace Ganedata.Core.Services
{
    public class TenantWebsiteService : ITenantWebsiteService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;
        private readonly IEmailServices _emailServices;
        private readonly IProductServices _productServices;
        private readonly IProductPriceService _productPriceService;
        private readonly ITenantsCurrencyRateServices _tenantsCurrencyRateServices;
        private readonly IAccountServices _accountServices;

        public TenantWebsiteService(IApplicationContext currentDbContext,
                                    IMapper mapper,
                                    IEmailServices emailServices,
                                    IProductServices productServices,
                                    IProductPriceService productPriceService,
                                    ITenantsCurrencyRateServices tenantsCurrencyRateServices, IAccountServices accountServices)
        {
            _currentDbContext = currentDbContext;
            _mapper = mapper;
            _emailServices = emailServices;
            _productPriceService = productPriceService;
            _productServices = productServices;
            _tenantsCurrencyRateServices = tenantsCurrencyRateServices;
            _accountServices = accountServices;
        }

        public WebsiteNavigation GetProductCategoryByProductId(int siteId, int productId)
        {
            return _currentDbContext.ProductsWebsitesMap.FirstOrDefault(u => u.SiteID == siteId &&
                                                                             u.ProductId == productId &&
                                                                             u.IsDeleted != true &&
                                                                             u.IsActive == true)?
                                                        .ProductsNavigationMap.FirstOrDefault()?
                                                        .WebsiteNavigation;
        }

        public bool VerifyProductAgainstWebsite(int productId, int siteId)
        {
            return _currentDbContext.ProductsWebsitesMap.Any(u =>
                u.ProductId == productId && u.SiteID == siteId && u.IsDeleted != true && u.IsActive);
        }

        public IEnumerable<TenantWebsites> GetAllActiveTenantWebSites(int tenantId)
        {
            return GetAllValidTenantWebSites(tenantId).Where(u => u.IsActive == true);
        }

        public IEnumerable<TenantWebsites> GetAllValidTenantWebSites(int tenantId)
        {
            return _currentDbContext.TenantWebsites.Where(u => u.IsDeleted != true && u.TenantId == tenantId);
        }

        public WebsiteLayoutSettings GetWebsiteLayoutSettingsInfoBySiteId(int SiteId)
        {
            return _currentDbContext.WebsiteLayoutSettings.FirstOrDefault(u => u.SiteId == SiteId);
        }

        public TenantWebsites GetTenantWebSiteBySiteId(int SiteId)
        {
            return _currentDbContext.TenantWebsites.FirstOrDefault(u => u.IsDeleted != true && u.SiteID == SiteId);
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
                _currentDbContext.Entry(tenantWebsites).State = EntityState.Modified;
            }
            _currentDbContext.SaveChanges();
            return tenantWebsites;
        }

        public WebsiteLayoutSettings SaveWebsiteLayoutSettings(WebsiteLayoutSettings websiteLayoutSettings, int UserId, int TenantId)
        {
            websiteLayoutSettings.TenantId = TenantId;
            if (websiteLayoutSettings.Id <= 0)
            {
                websiteLayoutSettings.UpdateCreatedInfo(UserId);
                _currentDbContext.WebsiteLayoutSettings.Add(websiteLayoutSettings);
            }
            else
            {
                var website = _currentDbContext.TenantWebsites.AsNoTracking().FirstOrDefault(u => u.SiteID == websiteLayoutSettings.SiteId);
                websiteLayoutSettings.CreatedBy = website.CreatedBy;
                websiteLayoutSettings.DateCreated = website.DateCreated;
                websiteLayoutSettings.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(websiteLayoutSettings).State = EntityState.Modified;
            }
            _currentDbContext.SaveChanges();
            return websiteLayoutSettings;
        }

        public bool RemoveTenantWebsite(int siteID, int UserId)
        {
            var website = _currentDbContext.TenantWebsites.Find(siteID);
            website.IsDeleted = true;
            website.UpdateUpdatedInfo(UserId);
            _currentDbContext.Entry(website).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return true;
        }

        //WebsiteContentPages
        public IEnumerable<WebsiteContentPages> GetAllValidWebsiteContentPages(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteContentPages.Where(u => u.IsDeleted != true && u.TenantId == TenantId && u.SiteID == SiteId && u.IsActive == true);
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

        public WebsiteContentPages GetWebsiteContentByUrl(int? siteId, string url)
        {
            return _currentDbContext.WebsiteContentPages.FirstOrDefault(u => u.pageUrl == url && (u.SiteID == siteId || (siteId == null && u.TenantWebsites.IsInternalWebsite)));
        }

        //ProductsWebsitesMaps
        public IEnumerable<ProductsWebsitesMap> GetAllValidProductsWebsitesMap(int TenantId)
        {
            return _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true && u.IsActive);
        }

        //WebsiteSliders
        public IEnumerable<WebsiteSlider> GetAllValidWebsiteSlider(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteSliders.Where(u => u.IsDeleted != true && u.TenantId == TenantId && u.SiteID == SiteId && u.IsActive);
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
                var slider = _currentDbContext.WebsiteSliders.AsNoTracking().FirstOrDefault(u => u.Id == websiteSlider.Id);
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

        public IEnumerable<WebsiteNavigation> GetAllValidWebsiteNavigation(int tenantId, int? siteId)
        {
            return _currentDbContext.WebsiteNavigations.Where(u => u.IsDeleted != true && (!siteId.HasValue || u.SiteID == siteId) && u.TenantId == tenantId && u.IsActive);
        }

        public IQueryable<WebsiteNavigation> GetAllValidWebsiteNavigationCategory(int TenantId, int? SiteId)
        {
            return _currentDbContext.WebsiteNavigations.Where(u => u.IsDeleted != true &&
                                                                   (!SiteId.HasValue || u.SiteID == SiteId) &&
                                                                   u.TenantId == TenantId &&
                                                                   u.Type == WebsiteNavigationType.Category &&
                                                                   u.IsActive);
        }

        public IQueryable<WebsiteNavigation> GetAllValidWebsiteNavigationTopCategory(int tenantId, int? siteId)
        {
            return _currentDbContext.WebsiteNavigations.Where(u => u.IsDeleted != true &&
                                                                   (!siteId.HasValue || u.SiteID == siteId) &&
                                                                   u.TenantId == tenantId &&
                                                                   u.ShowInTopCategory == true &&
                                                                   u.IsActive);
        }

        public IQueryable<NavigationProductsViewModel> GetAllValidWebsiteNavigations(int tenantId, int siteId, int navigationId)
        {
            var websiteMap = _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true && u.SiteID == siteId && u.TenantId == tenantId && u.IsActive);
            var allProducts = _currentDbContext.ProductMaster.Where(m => m.TenantId == tenantId && m.IsDeleted != true);
            var navigationMap = _currentDbContext.ProductsNavigationMaps.Where(x => x.NavigationId == navigationId && x.IsDeleted != true && x.IsActive);

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

        public IQueryable<NavigationProductsViewModel> GetAllValidWebsiteProducts(int TenantId, int SiteId)
        {
            var allProducts = _currentDbContext.ProductMaster.Where(m => m.TenantId == TenantId && m.IsDeleted != true && (m.SiteId == SiteId || m.SiteId == null) && m.IsActive);
            var websiteMap = _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true && u.SiteID == SiteId && u.TenantId == TenantId && u.IsActive);
            var res = (from p in allProducts
                       join w in websiteMap on p.ProductId equals w.ProductId into tempMap
                       from d in tempMap.DefaultIfEmpty()
                       select new NavigationProductsViewModel()
                       {
                           Id = p.ProductId,
                           SiteID = SiteId,
                           ProductId = p.ProductId,
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

        public IQueryable<NavigationProductsViewModel> GetAllValidWebsiteProductsMap(int TenantId, int SiteId)
        {
            var websiteMap = _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true && u.SiteID == SiteId && u.TenantId == TenantId && u.IsActive)
                .Select(u => new NavigationProductsViewModel()
                {
                    Id = u.Id,
                    ProductId = u.ProductId,
                    Name = u.ProductMaster.Name,
                    SKUCode = u.ProductMaster.SKUCode,
                });

            return websiteMap;
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

        public bool CreateOrUpdateWebsiteProducts(NavigationProductsViewModel websiteProduct, int UserId, int TenantId)
        {
            var navItem = _currentDbContext.ProductsWebsitesMap.Where(x => x.ProductId == websiteProduct.ProductId
            && x.SiteID == websiteProduct.SiteID && x.TenantId == TenantId).FirstOrDefault();

            if (navItem == null)
            {
                ProductsWebsitesMap productsNavigation = new ProductsWebsitesMap();
                productsNavigation.ProductId = websiteProduct.ProductId;
                productsNavigation.SiteID = websiteProduct.SiteID;
                productsNavigation.SortOrder = websiteProduct.SortOrder ?? 1;
                productsNavigation.TenantId = TenantId;
                productsNavigation.IsActive = websiteProduct.IsActive ?? true;
                productsNavigation.IsDeleted = false;
                productsNavigation.UpdateCreatedInfo(UserId);
                _currentDbContext.ProductsWebsitesMap.Add(productsNavigation);
            }
            else
            {
                navItem.IsActive = websiteProduct.IsActive ?? true;
                navItem.SortOrder = websiteProduct.SortOrder ?? 1;
                navItem.IsDeleted = false;
                navItem.UpdateUpdatedInfo(UserId);
            }

            _currentDbContext.SaveChanges();
            return true;
        }

        public IQueryable<WebsiteWarehousesViewModel> GetAllValidWebsiteWarehouses(int tenantId, int siteId)
        {
            var allWarehouses = _currentDbContext.TenantWarehouses.Where(m => m.TenantId == tenantId && m.IsDeleted != true && m.IsActive);
            var websiteWarehouses = _currentDbContext.WebsiteWarehouses.Where(u => u.IsDeleted != true && u.SiteID == siteId && u.TenantId == tenantId && u.IsActive);
            var result = (from p in allWarehouses
                          join w in websiteWarehouses on p.WarehouseId equals w.WarehouseId into tempMap
                          from d in tempMap.DefaultIfEmpty()
                          select new WebsiteWarehousesViewModel()
                          {
                              Id = p.WarehouseId,
                              SiteID = siteId,
                              WarehouseId = p.WarehouseId,
                              WarehouseName = p.WarehouseName,
                              WarehouseAddress = p.AddressLine1,
                              WarehouseCity = p.City,
                              Description = p.Description,
                              IsActive = d.IsActive,
                              SortOrder = d.SortOrder
                          });

            return result;
        }

        public bool CreateOrUpdateWebsiteWarehouse(WebsiteWarehousesViewModel websiteWarehouseData, int UserId, int TenantId)
        {
            var navItem = _currentDbContext.WebsiteWarehouses.FirstOrDefault(x => x.WarehouseId == websiteWarehouseData.WarehouseId && x.TenantId == TenantId);

            if (navItem == null)
            {
                WebsiteWarehouses websiteWarehouse = new WebsiteWarehouses();
                websiteWarehouse.WarehouseId = websiteWarehouseData.WarehouseId;
                websiteWarehouse.SiteID = websiteWarehouseData.SiteID;
                websiteWarehouse.SortOrder = websiteWarehouseData.SortOrder ?? 1;
                websiteWarehouse.TenantId = TenantId;
                websiteWarehouse.IsActive = websiteWarehouseData.IsActive ?? true;
                websiteWarehouse.IsDeleted = false;
                websiteWarehouse.UpdateCreatedInfo(UserId);
                _currentDbContext.WebsiteWarehouses.Add(websiteWarehouse);
            }
            else
            {
                navItem.IsActive = websiteWarehouseData.IsActive ?? true;
                navItem.SortOrder = websiteWarehouseData.SortOrder ?? 1;
                navItem.IsDeleted = false;
                navItem.UpdateUpdatedInfo(UserId);
            }

            _currentDbContext.SaveChanges();
            return true;
        }

        public IQueryable<ProductMaster> GetProductByNavigationId(int navigationId)
        {
            var productwebsiteMap = _currentDbContext.ProductsNavigationMaps.Where(u => u.NavigationId == navigationId).OrderBy(u => u.SortOrder).Select(u => u.ProductsWebsitesMap);
            return productwebsiteMap.OrderBy(u => u.SortOrder).Select(u => u.ProductMaster);
        }

        //WebsiteShippingRules

        public IQueryable<WebsiteShippingRules> GetAllValidWebsiteShippingRules(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteShippingRules.Where(u => u.TenantId == TenantId && u.SiteID == SiteId && u.IsDeleted != true && u.IsActive);
        }

        public List<WebsiteShippingRulesViewModel> GetShippingRulesByPostCode(int tenantId, int siteId, string postCode, double parcelWeightInGrams)
        {
            if (string.IsNullOrEmpty(postCode?.Trim()))
            {
                return null;
            }

            return GetShippingRulesByShippingAddress(tenantId, siteId, new AccountAddresses { PostCode = postCode }, parcelWeightInGrams);
        }

        public List<WebsiteShippingRulesViewModel> GetShippingRulesByShippingAddressId(int tenantId, int siteId, int shippingAddressId, double parcelWeightInGrams)
        {
            var address = _currentDbContext.AccountAddresses.Find(shippingAddressId);

            if (address == null)
            {
                return null;
            }

            return GetShippingRulesByShippingAddress(tenantId, siteId, address, parcelWeightInGrams);
        }

        private List<WebsiteShippingRulesViewModel> GetShippingRulesByShippingAddress(int tenantId, int siteId, AccountAddresses address, double parcelWeightInGrams)
        {
            var getTenantCurrency = _tenantsCurrencyRateServices.GetTenantCurrencies(tenantId).FirstOrDefault();
            var currencyRate = _tenantsCurrencyRateServices.GetCurrencyRateByTenantid(getTenantCurrency?.TenantCurrencyID ?? 0);

            var allRules = GetAllValidWebsiteShippingRules(tenantId, siteId)
                .Where(r => r.IsActive == true && (r.CountryId == address.CountryID || address.CountryID == 0) && r.WeightinGrams >= parcelWeightInGrams);

            var rules = allRules.Where(r => address.PostCode.StartsWith(r.PostalArea) ||
                                            (!address.PostCode.StartsWith(r.PostalArea) &&
                                            (address.Town.Contains(r.Region) ||
                                            address.AddressLine1.Contains(r.Region) ||
                                            address.AddressLine2.Contains(r.Region) ||
                                            address.AddressLine3.Contains(r.Region) ||
                                            address.AddressLine3.Contains(r.Region))));

            if (!rules.Any())
            {
                rules = allRules.Where(r => (string.IsNullOrEmpty(r.Region.Trim()) && string.IsNullOrEmpty(r.PostalArea.Trim())));
            }

            var shippingRules = rules.ToList()
                                .GroupBy(r => (r.Courier, r.Description))
                                .Select(r => r.OrderBy(rp => rp.WeightinGrams).First())
                                .OrderByDescending(r => r.SortOrder)
                                .ToList();
            shippingRules.ForEach(r =>
            {
                r.Price = Math.Round(((r.Price) * ((currencyRate <= 0) ? 1 : currencyRate)), 2);
            });
            return _mapper.Map(shippingRules, new List<WebsiteShippingRulesViewModel>());
        }

        public WebsiteShippingRules CreateOrUpdateWebsiteShippingRules(WebsiteShippingRules websiteShippingRules, int UserId, int TenantId)
        {
            var websiteShippingRulesItems = _currentDbContext.WebsiteShippingRules.Where(x => x.TenantId == TenantId
           && x.SiteID == websiteShippingRules.SiteID && x.IsDeleted != true && x.Id == websiteShippingRules.Id && x.IsActive).FirstOrDefault();
            if (websiteShippingRulesItems == null)
            {
                WebsiteShippingRules websiteShipping = new WebsiteShippingRules();
                websiteShipping.SiteID = websiteShippingRules.SiteID;
                websiteShipping.CountryId = websiteShippingRules.CountryId;
                websiteShipping.Courier = websiteShippingRules.Courier;
                websiteShipping.Description = websiteShippingRules.Description;
                websiteShipping.PostalArea = websiteShippingRules.PostalArea;
                websiteShipping.Region = websiteShippingRules.Region;
                websiteShipping.Price = websiteShippingRules.Price;
                websiteShipping.SortOrder = websiteShippingRules.SortOrder;
                websiteShipping.WeightinGrams = websiteShippingRules.WeightinGrams;
                websiteShipping.UpdateCreatedInfo(UserId);
                websiteShipping.TenantId = TenantId;
                websiteShipping.IsActive = websiteShippingRules.IsActive;
                _currentDbContext.WebsiteShippingRules.Add(websiteShipping);
            }
            else
            {
                websiteShippingRulesItems.CountryId = websiteShippingRules.CountryId;
                websiteShippingRulesItems.Courier = websiteShippingRules.Courier;
                websiteShippingRulesItems.Description = websiteShippingRules.Description;
                websiteShippingRulesItems.PostalArea = websiteShippingRules.PostalArea;
                websiteShippingRulesItems.Region = websiteShippingRules.Region;
                websiteShippingRulesItems.Price = websiteShippingRules.Price;
                websiteShippingRulesItems.SortOrder = websiteShippingRules.SortOrder;
                websiteShippingRulesItems.WeightinGrams = websiteShippingRules.WeightinGrams;
                websiteShippingRulesItems.IsActive = websiteShippingRules.IsActive;
                websiteShippingRulesItems.SortOrder = websiteShippingRules.SortOrder;
                websiteShippingRulesItems.TenantId = TenantId;
                websiteShippingRulesItems.IsDeleted = false;
                websiteShippingRulesItems.UpdateUpdatedInfo(UserId);
            }
            _currentDbContext.SaveChanges();
            return websiteShippingRules;
        }

        public WebsiteShippingRules RemoveWebsiteShippingRules(int Id, int UserId)
        {
            var WebsiteShipping = _currentDbContext.WebsiteShippingRules.FirstOrDefault(u => u.Id == Id);
            if (WebsiteShipping != null)
            {
                WebsiteShipping.IsDeleted = true;
                WebsiteShipping.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(WebsiteShipping).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return WebsiteShipping;
        }

        public WebsiteShippingRules GetWebsiteShippingRulesById(int Id)
        {
            return _currentDbContext.WebsiteShippingRules.FirstOrDefault(u => u.Id == Id && u.IsDeleted != true && u.IsActive);
        }

        // website Voucher

        public IQueryable<WebsiteVouchers> GetAllValidWebsiteVoucher(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteVouchers.Where(u => u.TenantId == TenantId && u.SiteID == SiteId && u.IsDeleted != true && u.IsActive);
        }

        public WebsiteVouchers CreateOrUpdateWebsiteVouchers(WebsiteVouchers websiteVouchers, int UserId, int TenantId)
        {
            var websiteVouchersItems = _currentDbContext.WebsiteVouchers.Where(x => x.TenantId == TenantId
           && x.SiteID == websiteVouchers.SiteID && x.IsDeleted != true && x.Id == websiteVouchers.Id).FirstOrDefault();
            if (websiteVouchersItems == null)
            {
                WebsiteVouchers websiteVouchers1 = new WebsiteVouchers();
                websiteVouchers1.Id = Guid.NewGuid();
                websiteVouchers1.SiteID = websiteVouchers.SiteID;
                websiteVouchers1.Code = websiteVouchers.Code;
                websiteVouchers1.Value = websiteVouchers.Value;
                websiteVouchers1.Shared = websiteVouchers.Shared;
                websiteVouchers1.UserId = websiteVouchers.UserId;
                websiteVouchers1.UpdateCreatedInfo(UserId);
                websiteVouchers1.TenantId = TenantId;
                websiteVouchers1.IsActive = websiteVouchers.IsActive;
                _currentDbContext.WebsiteVouchers.Add(websiteVouchers1);
            }
            else
            {
                websiteVouchersItems.SiteID = websiteVouchers.SiteID;
                websiteVouchersItems.Code = websiteVouchers.Code;
                websiteVouchersItems.Value = websiteVouchers.Value;
                websiteVouchersItems.Shared = websiteVouchers.Shared;
                websiteVouchersItems.UserId = websiteVouchers.UserId;
                websiteVouchersItems.IsActive = websiteVouchers.IsActive;
                websiteVouchersItems.TenantId = TenantId;
                websiteVouchersItems.IsDeleted = false;
                websiteVouchersItems.UpdateUpdatedInfo(UserId);
            }
            _currentDbContext.SaveChanges();
            return websiteVouchers;
        }

        public WebsiteVouchers RemoveWebsiteVoucher(Guid Id, int UserId)
        {
            var WebsiteShipping = _currentDbContext.WebsiteVouchers.FirstOrDefault(u => u.Id == Id);
            if (WebsiteShipping != null)
            {
                WebsiteShipping.IsDeleted = true;
                WebsiteShipping.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(WebsiteShipping).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return WebsiteShipping;
        }

        public WebsiteVouchers GetWebsiteVoucherById(Guid Id)
        {
            return _currentDbContext.WebsiteVouchers.FirstOrDefault(u => u.Id == Id && u.IsDeleted != true && u.IsActive);
        }

        public string GenerateVoucherCode()
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";
            string characters = alphabets + small_alphabets + numbers;
            string otp = string.Empty;
            for (int i = 0; i < 16; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            var duplicate = _currentDbContext.WebsiteVouchers.FirstOrDefault(u => u.Code == otp && u.IsDeleted != true);
            if (duplicate != null)
            {
                return GenerateVoucherCode();
            }

            return otp.Insert(4, "-").Insert(9, "-").Insert(14, "-").ToUpper();
        }

        //WebsiteDiscountCodes
        public IEnumerable<WebsiteDiscountCodes> GetAllValidWebsiteDiscountCodes(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteDiscountCodes.Where(u => u.TenantId == TenantId && u.SiteID == SiteId && u.IsDeleted != true && u.IsActive);
        }

        public IEnumerable<WebsiteDiscountProductsMap> GetAllValidWebsiteDiscountProductsMap(int TenantId, int DiscountId)
        {
            return _currentDbContext.websiteDiscountProductsMaps.Where(u => u.TenantId == TenantId && u.IsDeleted != true && u.DiscountId == DiscountId && u.IsActive);
        }

        public WebsiteDiscountCodes CreateOrUpdateWebsiteDiscountCodes(WebsiteDiscountCodes websiteDiscount, int UserId, int TenantId)
        {
            var websiteDiscountItems = _currentDbContext.WebsiteDiscountCodes.Where(x => x.TenantId == TenantId
          && x.SiteID == websiteDiscount.SiteID && x.IsDeleted != true && x.Id == websiteDiscount.Id).FirstOrDefault();
            if (websiteDiscountItems == null)
            {
                WebsiteDiscountCodes websiteDiscountCodes = new WebsiteDiscountCodes();
                websiteDiscountCodes.SiteID = websiteDiscount.SiteID;
                websiteDiscountCodes.Code = websiteDiscount.Code;
                websiteDiscountCodes.FromDate = websiteDiscount.FromDate;
                websiteDiscountCodes.ToDate = websiteDiscount.ToDate;
                websiteDiscountCodes.DiscountType = websiteDiscount.DiscountType;
                websiteDiscountCodes.DiscountPercent = websiteDiscount.DiscountPercent;
                websiteDiscountCodes.FreeShippig = websiteDiscount.FreeShippig;
                websiteDiscountCodes.SingleUse = websiteDiscount.SingleUse;
                websiteDiscountCodes.Title = websiteDiscount.Title;
                websiteDiscountCodes.MinimumBasketValue = websiteDiscount.MinimumBasketValue;
                websiteDiscountCodes.UpdateCreatedInfo(UserId);
                websiteDiscountCodes.TenantId = TenantId;
                websiteDiscountCodes.IsActive = websiteDiscount.IsActive;
                _currentDbContext.WebsiteDiscountCodes.Add(websiteDiscountCodes);
                _currentDbContext.SaveChanges();
                if (!string.IsNullOrEmpty(websiteDiscount.SelectedProductIds))
                {
                    List<int> productWebMapId = websiteDiscount.SelectedProductIds.Split(',').Select(Int32.Parse).ToList();
                    foreach (var item in productWebMapId)
                    {
                        WebsiteDiscountProductsMap DiscountProductsMap = new WebsiteDiscountProductsMap();
                        DiscountProductsMap.DiscountId = websiteDiscountCodes.Id;
                        DiscountProductsMap.ProductsWebsitesMapId = item;
                        DiscountProductsMap.SortOrder = 1;
                        DiscountProductsMap.TenantId = TenantId;
                        DiscountProductsMap.IsActive = true;
                        DiscountProductsMap.UpdateCreatedInfo(UserId);
                        _currentDbContext.websiteDiscountProductsMaps.Add(DiscountProductsMap);
                    }
                    _currentDbContext.SaveChanges();
                }
            }
            else
            {
                websiteDiscountItems.Title = websiteDiscount.Title;
                websiteDiscountItems.MinimumBasketValue = websiteDiscount.MinimumBasketValue;
                websiteDiscountItems.SiteID = websiteDiscount.SiteID;
                websiteDiscountItems.Code = websiteDiscount.Code;
                websiteDiscountItems.FromDate = websiteDiscount.FromDate;
                websiteDiscountItems.ToDate = websiteDiscount.ToDate;
                websiteDiscountItems.DiscountType = websiteDiscount.DiscountType;
                websiteDiscountItems.DiscountPercent = websiteDiscount.DiscountPercent;
                websiteDiscountItems.FreeShippig = websiteDiscount.FreeShippig;
                websiteDiscountItems.SingleUse = websiteDiscount.SingleUse;
                websiteDiscountItems.TenantId = TenantId;
                websiteDiscountItems.IsActive = websiteDiscount.IsActive;
                websiteDiscountItems.UpdateUpdatedInfo(UserId);
                if (!string.IsNullOrEmpty(websiteDiscount.SelectedProductIds))
                {
                    List<int> productWebMapIds = websiteDiscount.SelectedProductIds.Split(',').Select(Int32.Parse).ToList();
                    var Toadd = productWebMapIds.Except(_currentDbContext.websiteDiscountProductsMaps
                    .Where(a => a.IsDeleted != true && a.DiscountId == websiteDiscount.Id).Select(a => a.ProductsWebsitesMapId)
                    .ToList()).ToList();
                    var toDelete = _currentDbContext.websiteDiscountProductsMaps
                    .Where(a => a.IsDeleted != true && a.DiscountId == websiteDiscount.Id).Select(a => a.ProductsWebsitesMapId).Except(productWebMapIds);
                    foreach (var item in Toadd)
                    {
                        WebsiteDiscountProductsMap DiscountProductsMap = new WebsiteDiscountProductsMap();
                        DiscountProductsMap.DiscountId = websiteDiscount.Id;
                        DiscountProductsMap.ProductsWebsitesMapId = item;
                        DiscountProductsMap.SortOrder = 1;
                        DiscountProductsMap.TenantId = TenantId;
                        DiscountProductsMap.IsActive = true;
                        DiscountProductsMap.UpdateCreatedInfo(UserId);
                        _currentDbContext.websiteDiscountProductsMaps.Add(DiscountProductsMap);
                    }
                    foreach (var item in toDelete)
                    {
                        var productsNavigation = _currentDbContext.websiteDiscountProductsMaps.FirstOrDefault(u => u.ProductsWebsitesMapId == item);
                        if (productsNavigation != null)
                        {
                            productsNavigation.IsDeleted = true;
                            productsNavigation.UpdateUpdatedInfo(UserId);
                            _currentDbContext.Entry(productsNavigation).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
            }

            _currentDbContext.SaveChanges();
            return websiteDiscount;
        }

        public WebsiteDiscountCodes RemoveWebsiteDiscountCodes(int Id, int UserId)
        {
            var WebsiteShipping = _currentDbContext.WebsiteDiscountCodes.FirstOrDefault(u => u.Id == Id);
            if (WebsiteShipping != null)
            {
                WebsiteShipping.IsDeleted = true;
                WebsiteShipping.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(WebsiteShipping).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return WebsiteShipping;
        }

        public WebsiteDiscountCodes GetWebsiteDiscountCodesById(int discountId)
        {
            return _currentDbContext.WebsiteDiscountCodes.Find(discountId);
        }

        //ProductAllowances
        public IEnumerable<ProductAllowance> GetAllValidProductAllowance(int TenantId, int SiteId)
        {
            return _currentDbContext.ProductAllowance.Where(u => u.TenantId == TenantId && u.IsDeleted != true && u.SiteID == SiteId);
        }

        public ProductAllowance CreateOrUpdateProductAllowance(ProductAllowance productAllowance, string Reason, int UserId, int TenantId)
        {
            var websiteProductAllownce = _currentDbContext.ProductAllowance.Where(x => x.TenantId == TenantId
                 && x.Id == productAllowance.Id && x.IsDeleted != true).FirstOrDefault();
            if (websiteProductAllownce == null)
            {
                ProductAllowance productAllowances = new ProductAllowance();
                productAllowances.PerXDays = productAllowance.PerXDays;
                productAllowances.RolesId = productAllowance.RolesId;
                productAllowances.Quantity = productAllowance.Quantity;
                productAllowances.ProductId = productAllowance.ProductId;
                productAllowances.AllowanceGroupId = productAllowance.AllowanceGroupId;
                productAllowances.UpdateCreatedInfo(UserId);
                productAllowances.TenantId = TenantId;
                _currentDbContext.ProductAllowance.Add(productAllowances);
                _currentDbContext.SaveChanges();
            }
            else
            {
                websiteProductAllownce.PerXDays = productAllowance.PerXDays;
                websiteProductAllownce.RolesId = productAllowance.RolesId;
                websiteProductAllownce.Quantity = productAllowance.Quantity;
                websiteProductAllownce.ProductId = productAllowance.ProductId;
                websiteProductAllownce.AllowanceGroupId = productAllowance.AllowanceGroupId;
                websiteProductAllownce.TenantId = TenantId;
                websiteProductAllownce.UpdateUpdatedInfo(UserId);
                if (!string.IsNullOrEmpty(Reason))
                {
                    ProductAllowanceAdjustmentLog adjustmentLog = new ProductAllowanceAdjustmentLog();
                    adjustmentLog.Reason = Reason;
                    adjustmentLog.TenantId = TenantId;
                    adjustmentLog.AllowanceId = productAllowance.Id;
                    adjustmentLog.UpdateCreatedInfo(UserId);
                    _currentDbContext.ProductAllowanceAdjustmentLog.Add(adjustmentLog);
                }
            }

            _currentDbContext.SaveChanges();
            return productAllowance;
        }

        public ProductAllowance RemoveProductAllowance(int Id, int UserId)
        {
            var WebsiteShipping = _currentDbContext.ProductAllowance.FirstOrDefault(u => u.Id == Id);
            if (WebsiteShipping != null)
            {
                WebsiteShipping.IsDeleted = true;
                WebsiteShipping.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(WebsiteShipping).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return WebsiteShipping;
        }

        public ProductAllowance GetProductAllowanceById(int Id)
        {
            return _currentDbContext.ProductAllowance.Find(Id);
        }

        //ProductAllowancesGroups
        public IEnumerable<ProductAllowanceGroup> GetAllValidProductAllowanceGroups(int TenantId, int SiteId)
        {
            return _currentDbContext.ProductAllowanceGroup.Where(u => u.TenantId == TenantId && u.IsDeleted != true && u.SiteID == SiteId);
        }

        public ProductAllowanceGroup CreateOrUpdateProductGroupAllowance(ProductAllowanceGroup productAllowanceGroup, int UserId, int TenantId)
        {
            var websiteDiscountItems = _currentDbContext.ProductAllowanceGroup.Where(x => x.TenantId == TenantId
                && x.SiteID == productAllowanceGroup.SiteID && x.IsDeleted != true && x.Id == productAllowanceGroup.Id).FirstOrDefault();
            if (websiteDiscountItems == null)
            {
                ProductAllowanceGroup productAllowanceGroups = new ProductAllowanceGroup();
                productAllowanceGroups.SiteID = productAllowanceGroup.SiteID;
                productAllowanceGroups.Name = productAllowanceGroup.Name;
                productAllowanceGroups.Notes = productAllowanceGroup.Notes;
                productAllowanceGroups.UpdateCreatedInfo(UserId);
                productAllowanceGroups.TenantId = TenantId;
                _currentDbContext.ProductAllowanceGroup.Add(productAllowanceGroups);
                _currentDbContext.SaveChanges();
                if (!string.IsNullOrEmpty(productAllowanceGroup.SelectedProductIds))
                {
                    List<int> productWebMapId = productAllowanceGroup.SelectedProductIds.Split(',').Select(Int32.Parse).ToList();
                    foreach (var item in productWebMapId)
                    {
                        ProductAllowanceGroupMap DiscountProductsMap = new ProductAllowanceGroupMap();
                        DiscountProductsMap.AllowanceGroupId = productAllowanceGroups.Id;
                        DiscountProductsMap.ProductwebsiteMapId = item;
                        DiscountProductsMap.TenantId = TenantId;
                        DiscountProductsMap.UpdateCreatedInfo(UserId);
                        _currentDbContext.ProductAllowanceGroupMap.Add(DiscountProductsMap);
                    }
                    _currentDbContext.SaveChanges();
                }
            }
            else
            {
                websiteDiscountItems.SiteID = productAllowanceGroup.SiteID;
                websiteDiscountItems.Name = productAllowanceGroup.Name;
                websiteDiscountItems.Notes = productAllowanceGroup.Notes;
                websiteDiscountItems.TenantId = TenantId;

                websiteDiscountItems.UpdateUpdatedInfo(UserId);
                if (!string.IsNullOrEmpty(productAllowanceGroup.SelectedProductIds))
                {
                    List<int> productWebMapIds = productAllowanceGroup.SelectedProductIds.Split(',').Select(Int32.Parse).ToList();
                    var Toadd = productWebMapIds.Except(_currentDbContext.ProductAllowanceGroupMap
                    .Where(a => a.IsDeleted != true && a.AllowanceGroupId == productAllowanceGroup.Id).Select(a => a.ProductwebsiteMapId)
                    .ToList()).ToList();
                    var toDelete = _currentDbContext.ProductAllowanceGroupMap
                    .Where(a => a.IsDeleted != true && a.AllowanceGroupId == productAllowanceGroup.Id).Select(a => a.ProductwebsiteMapId).Except(productWebMapIds);
                    foreach (var item in Toadd)
                    {
                        ProductAllowanceGroupMap DiscountProductsMap = new ProductAllowanceGroupMap();
                        DiscountProductsMap.AllowanceGroupId = productAllowanceGroup.Id;
                        DiscountProductsMap.ProductwebsiteMapId = item;
                        DiscountProductsMap.TenantId = TenantId;
                        DiscountProductsMap.UpdateCreatedInfo(UserId);
                        _currentDbContext.ProductAllowanceGroupMap.Add(DiscountProductsMap);
                    }
                    foreach (var item in toDelete)
                    {
                        var productsNavigation = _currentDbContext.ProductAllowanceGroupMap.FirstOrDefault(u => u.ProductwebsiteMapId == item);
                        if (productsNavigation != null)
                        {
                            productsNavigation.IsDeleted = true;
                            productsNavigation.UpdateUpdatedInfo(UserId);
                            _currentDbContext.Entry(productsNavigation).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
            }

            _currentDbContext.SaveChanges();
            return productAllowanceGroup;
        }

        public ProductAllowanceGroup RemoveProductAllowanceGroup(int Id, int UserId)
        {
            var websiteDiscountItems = _currentDbContext.ProductAllowanceGroup.Where(x => x.IsDeleted != true && x.Id == Id).FirstOrDefault();
            if (websiteDiscountItems == null)
            {
                websiteDiscountItems.IsDeleted = true;
                websiteDiscountItems.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(websiteDiscountItems).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return websiteDiscountItems;
        }

        public ProductAllowanceGroup GetProductAllowanceGroupById(int Id)
        {
            return _currentDbContext.ProductAllowanceGroup.Find(Id);
        }

        public IEnumerable<ProductAllowanceGroupMap> GetAllValidProductAllowanceGroupMap(int TenantId, int productAllownceGroupId)
        {
            return _currentDbContext.ProductAllowanceGroupMap.Where(u => u.TenantId == TenantId && u.IsDeleted != true && u.AllowanceGroupId == productAllownceGroupId);
        }

        // WebsiteSearching Realted Queries
        public IQueryable<ProductMaster> GetWebsiteProducts(int siteId, string category = "", string ProductName = "", int? categoryId = null)
        {
            if (categoryId == null && category == null)
            {
                var productIds = GetProductsNavigationMapsSearch(siteId, ProductName)
                        .OrderBy(n => n.SortOrder)
                        .Select(x => x.ProductsWebsitesMap.ProductId)
                        .Distinct()
                        .ToList();

                return _currentDbContext.ProductMaster.Where(p => p.IsActive && p.IsDeleted != true && productIds.Contains(p.ProductId)).OrderBy(p => p.ProductId);
            }

            return GetProductsNavigationMapsSearch(siteId, ProductName)
                    .Where(u => (u.NavigationId == categoryId || (categoryId == null && u.WebsiteNavigation.Name == category) || (categoryId == null && category == null)))
                    .OrderBy(n => n.SortOrder)
                    .Select(x => x.ProductsWebsitesMap.ProductMaster);
        }

        public List<ProductManufacturer> GetWebsiteProductManufacturers(int siteId)
        {
            var manufacturersIds = GetProductsNavigationMapsSearch(siteId)
                                    .Where(n => n.ProductsWebsitesMap.ProductMaster.ManufacturerId != null)
                                    .Select(n => n.ProductsWebsitesMap.ProductMaster.ManufacturerId)
                                    .Distinct()
                                    .ToList();

            manufacturersIds.AddRange(_currentDbContext.ProductKitMaps.Where(k => k.IsActive &&
                                                                                k.IsDeleted != true &&
                                                                                k.KitProductMaster.IsActive &&
                                                                                k.KitProductMaster.IsDeleted != true &&
                                                                                k.ProductKitTypes != null && 
                                                                                k.ProductKitTypes.UseInParentCalculations == true &&
                                                                                k.ProductKitType != ProductKitTypeEnum.RelatedProduct &&
                                                                                k.KitProductMaster.ManufacturerId != null)
                                                                    .Select(k => k.KitProductMaster.ManufacturerId)
                                                                    .Distinct()
                                                                    .ToList());

            var manufacturers = _currentDbContext.ProductManufacturers.Where(p => manufacturersIds.Contains(p.Id) && p.IsDeleted != true).ToList();

            return manufacturers;
        }

        public List<ProductSearchResultViewModel> SearchWebsiteProducts(int siteId, int resultCount, string productName = "")
        {
            var productIds = GetProductsNavigationMapsSearch(siteId, productName).Select(p => p.ProductsWebsitesMap.ProductId)
                                                                                 .Distinct()
                                                                                 .Take(resultCount);
            return _currentDbContext.ProductMaster
                            .Where(p => productIds.Contains(p.ProductId))
                            .Select(x =>
                                        new ProductSearchResultViewModel
                                        {
                                            Id = x.ProductId,
                                            Name = x.Name,
                                            DefaultImage = x.ProductFiles.OrderByDescending(f => f.DefaultImage).FirstOrDefault(a => a.IsDeleted != true).FilePath,
                                            SkuCode = x.SKUCode,
                                            SearchKey = productName
                                        })
                            .OrderBy(u => u.Id)
                            .ToList();
        }

        private IQueryable<ProductsNavigationMap> GetProductsNavigationMapsSearch(int siteId, string ProductName = "")
        {
            ProductName = ProductName?.Trim();

            return _currentDbContext.ProductsNavigationMaps.Where(u => u.IsDeleted != true &&
                                                                        u.IsActive == true &&
                                                                        u.ProductsWebsitesMap.ProductMaster.IsActive == true &&
                                                                        u.ProductsWebsitesMap.ProductMaster.IsDeleted != true &&
                                                                        u.ProductsWebsitesMap.IsActive == true &&
                                                                        u.ProductsWebsitesMap.IsDeleted != true &&
                                                                        u.ProductsWebsitesMap.SiteID == siteId &&
                                                                        u.WebsiteNavigation.IsActive == true &&
                                                                        u.WebsiteNavigation.IsDeleted != true &&
                                                                        u.WebsiteNavigation.SiteID == siteId &&
                                                                        (string.IsNullOrEmpty(ProductName) ||
                                                                            (u.ProductsWebsitesMap.ProductMaster.Name.Contains(ProductName) ||
                                                                            u.ProductsWebsitesMap.ProductMaster.SKUCode.Contains(ProductName) ||
                                                                            u.ProductsWebsitesMap.ProductMaster.Description.Contains(ProductName)))
                                                                        );
        }

        public IQueryable<ProductMaster> GetAllValidProductForDynamicFilter(int siteId, List<int> productIds)
        {
            return _currentDbContext.ProductsWebsitesMap.Where(a => a.IsActive == true && a.IsDeleted != true && a.SiteID == siteId && productIds.Contains(a.ProductId)).Select(u => u.ProductMaster);
        }

        public Dictionary<ProductAttributes, List<ProductAttributeValues>> GetAllValidProductAttributeValuesByProductIds(IQueryable<ProductMaster> products)
        {
            var allProducts = products.Where(u => u.ProductType != ProductKitTypeEnum.ProductByAttribute).ToList();

            allProducts.AddRange(_productServices.GetAllProductInKitsByProductIds(products.Select(p => p.ProductId).ToList()));

            return allProducts.SelectMany(a => a.ProductAttributeValuesMap.Where(p => p.IsDeleted != true &&
                                                                                          p.ProductAttributeValues.IsDeleted != true &&
                                                                                          p.ProductAttributeValues.ProductAttributes.IsDeleted != true)
                                                                          .Select(k => k.ProductAttributeValues))
                              .OrderBy(u => u.ProductAttributes.SortOrder).ThenBy(a => a.ProductAttributes.AttributeName)
                              .GroupBy(a => a.AttributeId)
                              .ToDictionary(g => g.First().ProductAttributes, g => g.OrderBy(av => av.SortOrder).ThenBy(a => a.Value)
                              .GroupBy(av => av.AttributeValueId)
                              .Select(av => av.First())
                              .ToList());
        }

        public Tuple<string, string> GetAvailablePricesRange(List<int> productIds, int siteId, int? accountId)
        {
            var productsPrices = productIds.Select(u => GetPriceForProduct(u, siteId, accountId)).Where(p => p != null).ToList();

            return new Tuple<string, string>(productsPrices.Min(u => u).ToString(), productsPrices.Max(u => u).ToString());
        }

        public Dictionary<int, string> GetAllValidProductManufacturers(List<int> productIds)
        {
            var manufatcurers = _currentDbContext.ProductKitMaps.Where(a => productIds.Contains(a.ProductId) &&
                                                                a.ProductKitTypes != null && 
                                                                a.ProductKitTypes.UseInParentCalculations == true &&
                                                                a.IsDeleted != true &&
                                                                a.IsActive &&
                                                                a.KitProductMaster.ProductManufacturer != null)
                                                 .GroupBy(a => a.KitProductMaster.ProductManufacturer)
                                                 .Select(a => a.Key)
                                                 .ToList();

            manufatcurers.AddRange(_currentDbContext.ProductMaster.Where(u => productIds.Contains(u.ProductId) &&
                                                                              u.IsDeleted != true &&
                                                                              u.ProductManufacturer != null)
                                                                  .GroupBy(a => a.ProductManufacturer)
                                                                  .Select(a => a.Key)
                                                                  .ToList());

            return manufatcurers
                    .GroupBy(m => m)
                    .Select(m => m.Key)
                    .OrderBy(m => m.Name)
                    .ToDictionary(m => m.Id, m => m.Name);
        }

        public IEnumerable<WebsiteCartItem> GetAllValidCartItemsList(int siteId, int? UserId, string SessionKey)
        {
            return _currentDbContext.WebsiteCartItems.Where(u => u.SiteID == siteId && ((u.UserId == UserId) || u.SessionKey.Equals(SessionKey, StringComparison.InvariantCultureIgnoreCase)) && u.IsDeleted != true);
        }

        public IEnumerable<WebsiteCartItemViewModel> GetAllValidCartItems(int siteId, int? userId, int tenantId, string sessionKey, int? cartId = null)
        {
            var model = _currentDbContext.WebsiteCartItems.Where(u => u.IsDeleted != true &&
                                                                      u.SiteID == siteId &&
                                                                      u.TenantId == tenantId &&
                                                                      (u.UserId == userId || u.SessionKey.Equals(sessionKey, StringComparison.InvariantCultureIgnoreCase)) &&
                                                                      (!cartId.HasValue || u.Id == cartId))
                                                           .ToList()
                                                           .Select(u =>
                                                           {
                                                               var productMaster = _mapper.Map(u.ProductMaster, new ProductMasterViewModel());

                                                               if (productMaster.ProductType == ProductKitTypeEnum.Simple && productMaster.DefaultImage == null)
                                                               {
                                                                   productMaster.DefaultImage = GetDefaultImageFromParentProduct(u.ProductId);
                                                               }

                                                               return new WebsiteCartItemViewModel
                                                               {
                                                                   Id = u.Id,
                                                                   ProductMaster = productMaster,
                                                                   TaxPercent = u.ProductMaster.EnableTax == true ? u.ProductMaster.GlobalTax.PercentageOfAmount : 0,
                                                                   Weight = (u.ProductMaster.Weight + u.ProductMaster.ProductKitItems.Where(p => p.ProductKitType == ProductKitTypeEnum.Kit).Sum(k => k.ProductMaster.Weight)) * (double)u.Quantity,
                                                                   Price = u.UnitPrice,
                                                                   Quantity = u.Quantity,
                                                                   ProductId = u.ProductId,
                                                                   CartId = u.Id,
                                                                   KitProductCartItems = GetAllValidKitCartItemsList(u.Id).ToList()
                                                               };
                                                           });
            return model;
        }

        private string GetDefaultImageFromParentProduct(int productId)
        {
            return _productServices.GetParentProductsByKitProductId(productId)
                                    .FirstOrDefault(k => k.IsActive == true &&
                                                            k.ProductType == ProductKitTypeEnum.ProductByAttribute &&
                                                            k.IsDeleted != true)?.DefaultImage;
        }

        public IEnumerable<KitProductCartSession> GetAllValidKitCartItemsList(int cartId)
        {
            var data = _currentDbContext.KitProductCartItems.Where(u => u.CartId == cartId && u.IsDeleted != true).ToList()
                .Select(u => new KitProductCartSession
                {
                    SimpleProductId = u.SimpleProductId,
                    KitProductId = u.KitProductId,
                    Quantity = u.Quantity,
                    SimpleProductMaster = _mapper.Map(u.SimpleProductMaster, new ProductMasterViewModel())
                }).ToList();

            return data;
        }

        public int GetAllValidWishListItemsCount(int siteId, int UserId)
        {
            return _currentDbContext.WebsiteWishListItems.Count(u => u.SiteID == siteId && u.UserId == UserId && u.IsDeleted != true && !u.IsNotification);
        }

        public IEnumerable<WebsiteWishListItem> GetAllValidWishListItemsList(int siteId, int UserId)
        {
            var wishlistItems = _currentDbContext.WebsiteWishListItems
                                                .Include(w => w.ProductMaster)
                                                .Where(u => u.SiteID == siteId && u.UserId == UserId && u.IsDeleted != true && !u.IsNotification);

            wishlistItems.ForEach(w =>
            {
                if (w.ProductMaster != null && w.ProductMaster?.ProductType == ProductKitTypeEnum.Simple && w.ProductMaster?.DefaultImage == null)
                {
                    var parentProduct = _productServices.GetParentProductsByKitProductId(w.ProductId)
                                                        .FirstOrDefault(k => k.IsActive == true &&
                                                                             k.ProductType == ProductKitTypeEnum.ProductByAttribute &&
                                                                             k.IsDeleted != true);

                    var file = parentProduct?.ProductFiles.Where(x => x.DefaultImage == true && x.IsDeleted != true).FirstOrDefault();
                    if (file == null)
                    {
                        file = parentProduct?.ProductFiles.Where(x => x.IsDeleted != true).FirstOrDefault();
                    }

                    w.ProductMaster.ProductFiles.Add(file);
                }
            });

            return wishlistItems;
        }

        public int AddOrUpdateCartItem(int siteId, int? userId, int tenantId, string sessionKey, int productId, decimal quantity, List<KitProductCartSession> kitProductCartItems = null)
        {
            var getTenantCurrency = _tenantsCurrencyRateServices.GetTenantCurrencies(tenantId).FirstOrDefault();
            var currencyRate = _tenantsCurrencyRateServices.GetCurrencyRateByTenantid(getTenantCurrency?.TenantCurrencyID ?? 0);
            var productMaster = _productServices.GetProductMasterById(productId);
            var productKitType = productMaster.ProductType == ProductKitTypeEnum.Kit ? true : false;

            var cartProduct = _currentDbContext.WebsiteCartItems.FirstOrDefault(u => u.ProductId == productId &&
                                                                         u.IsDeleted != true &&
                                                                         productKitType == false &&
                                                                         u.SiteID == siteId &&
                                                                         u.TenantId == tenantId &&
                                                                         (u.UserId == userId || u.SessionKey.Equals(sessionKey, StringComparison.InvariantCultureIgnoreCase)));
            if (cartProduct == null)
            {
                var accountId = _currentDbContext.AuthUsers.FirstOrDefault(a => a.UserId == userId)?.AccountId;
                cartProduct = new WebsiteCartItem();
                cartProduct.ProductId = productId;
                cartProduct.Quantity = quantity;
                cartProduct.UnitPrice = Math.Round(GetPriceForProduct(productId, siteId, accountId) ?? 0, 2);
                cartProduct.UserId = userId == 0 ? null : userId;
                cartProduct.TenantId = tenantId;
                cartProduct.SiteID = siteId;
                cartProduct.SessionKey = userId > 0 ? null : sessionKey;
                cartProduct.CreatedBy = userId;
                cartProduct.DateCreated = DateTime.UtcNow;
                _currentDbContext.WebsiteCartItems.Add(cartProduct);
                _currentDbContext.SaveChanges();
                if (kitProductCartItems != null && kitProductCartItems.Count > 0)
                {
                    var distinctAttributes = kitProductCartItems.GroupBy(u => u.SimpleProductId).Select(grp => grp.ToList()).ToList();
                    if (distinctAttributes != null && distinctAttributes.Count > 0)
                    {
                        foreach (var attribute in distinctAttributes)
                        {
                            KitProductCartItem kitProductCart = new KitProductCartItem();
                            kitProductCart.KitProductId = attribute.FirstOrDefault().KitProductId;
                            kitProductCart.SimpleProductId = attribute.FirstOrDefault().SimpleProductId;
                            kitProductCart.Quantity = attribute.Count;
                            kitProductCart.CartId = cartProduct.Id;
                            kitProductCart.UpdateCreatedInfo(userId ?? 0);
                            _currentDbContext.KitProductCartItems.Add(kitProductCart);
                        }
                        _currentDbContext.SaveChanges();
                    }
                    _currentDbContext.SaveChanges();
                }
            }
            else
            {
                cartProduct.Quantity += quantity;
                cartProduct.SessionKey = userId > 0 ? null : sessionKey;
                cartProduct.UpdatedBy = userId == 0 ? null : userId;
                cartProduct.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(cartProduct).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
            return cartProduct.Id;
        }

        public bool UpdateCartItemQuantity(int siteId, int? userId, int tenantId, string sessionKey, int cartId, decimal quantity)
        {
            var cartProduct = _currentDbContext.WebsiteCartItems.FirstOrDefault(u => u.Id == cartId &&
                                                                                     u.IsDeleted != true &&
                                                                                     u.SiteID == siteId &&
                                                                                     u.TenantId == tenantId &&
                                                                                     (u.UserId == userId || u.SessionKey.Equals(sessionKey, StringComparison.InvariantCultureIgnoreCase)));

            if (cartProduct != null)
            {
                cartProduct.IsDeleted = quantity <= 0;
                cartProduct.Quantity = quantity;
                cartProduct.SessionKey = userId > 0 ? null : sessionKey;
                cartProduct.UpdatedBy = userId == 0 ? null : userId;
                cartProduct.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(cartProduct).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
            return true;
        }

        public void UpdateUserIdInCartItem(string sessionKey, int userId, int siteId)
        {
            var cartItems = _currentDbContext.WebsiteCartItems.Where(u => u.IsDeleted != true &&
                                                                          u.SiteID == siteId &&
                                                                          !u.UserId.HasValue &&
                                                                          u.SessionKey.Equals(sessionKey, StringComparison.InvariantCultureIgnoreCase))
                                                              .ToList();
            if (cartItems.Count <= 0)
            {
                return;
            }

            foreach (var item in cartItems)
            {
                var sameProduct = _currentDbContext.WebsiteCartItems.FirstOrDefault(u => u.UserId == userId &&
                                                                                         u.ProductId == item.ProductId &&
                                                                                         u.IsDeleted != true);
                if (sameProduct != null)
                {
                    sameProduct.Quantity += item.Quantity;
                    sameProduct.SessionKey = userId > 0 ? null : sessionKey;
                    item.IsDeleted = true;
                    sameProduct.UpdateUpdatedInfo(userId);
                    _currentDbContext.Entry(sameProduct).State = EntityState.Modified;
                    continue;
                }

                item.UserId = userId;
                item.SessionKey = userId > 0 ? null : sessionKey;
                item.UpdateUpdatedInfo(userId);
                _currentDbContext.Entry(item).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
        }

        public int AddOrUpdateWishListItems(int siteId, int userId, int tenantId, int productId)
        {
            return AddOrUpdateWishListNotifyListItems(siteId, userId, tenantId, productId, false);
        }

        public int AddOrUpdateNotifyListItems(int siteId, int userId, int tenantId, int productId, string emailId)
        {
            return AddOrUpdateWishListNotifyListItems(siteId, userId, tenantId, productId, true, emailId);
        }

        private int AddOrUpdateWishListNotifyListItems(int siteId, int userId, int tenantId, int productId, bool isNotificationItem, string emailId = null)
        {
            var wishListItems = _currentDbContext.WebsiteWishListItems
                .Where(u => u.ProductId == productId && u.SiteID == siteId)
                .Where(u => (!string.IsNullOrEmpty(emailId)? u.EmailId == emailId : u.UserId == userId))
                .Where(u => u.IsDeleted != true)                
                .ToList();

            if (!wishListItems.Any(w => !w.IsNotification))
            {
                AddWishlistItem(siteId, userId, tenantId, productId, false, emailId);
            } 

            if (isNotificationItem && !wishListItems.Any(w => w.IsNotification))
            {
                AddWishlistItem(siteId, userId, tenantId, productId, true, emailId);
            }

            _currentDbContext.SaveChanges();

            return GetAllValidWishListItemsList(siteId, userId).Count();
        }

        private void AddWishlistItem(int siteId, int userId, int tenantId, int productId, bool isNotification, string emailId = null)
        {
            var wishListItem = new WebsiteWishListItem
            {
                ProductId = productId,
                IsNotification = isNotification,
                SiteID = siteId,
                UserId = userId <= 0? (int?)null : userId,
                TenantId = tenantId,
                EmailId = emailId
            };
            wishListItem.UpdateCreatedInfo(userId);
            _currentDbContext.WebsiteWishListItems.Add(wishListItem);
        }

        public bool RemoveCartItem(int cartId, int siteId, int? userId, string sessionKey)
        {
            var cartProduct = _currentDbContext.WebsiteCartItems.FirstOrDefault(u => u.Id == cartId &&
                                                                                     u.IsDeleted != true &&
                                                                                     u.SiteID == siteId &&
                                                                                     ((((!userId.HasValue || userId == 0) || u.UserId == userId) ||
                                                                                       (string.IsNullOrEmpty(sessionKey) ||
                                                                                       u.SessionKey.Equals(sessionKey, StringComparison.InvariantCultureIgnoreCase)))));
            if (cartProduct != null)
            {
                cartProduct.IsDeleted = true;
                cartProduct.UpdateUpdatedInfo(userId ?? 0);
                _currentDbContext.Entry(cartProduct).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return true;
        }

        public bool GetWishlistNotificationStatus(int productId, int siteId, int userId)
        {
            return _currentDbContext.WebsiteWishListItems
                .FirstOrDefault(u => u.ProductId == productId && u.SiteID == siteId && u.UserId == userId)
                .IsNotification;
        }

        public bool ChangeWishListStatus(int productId, bool notification, int siteId, int userId)
        {
            var wishListItem = _currentDbContext.WebsiteWishListItems
                .Where(u => u.ProductId == productId && u.SiteID == siteId && u.UserId == userId).ToList();
            if (wishListItem.Count > 0)
            {
                foreach (var item in wishListItem)
                {
                    item.IsNotification = notification;
                    item.UpdateUpdatedInfo(userId);
                    _currentDbContext.Entry(item).State = EntityState.Modified;
                }
                _currentDbContext.SaveChanges();
            }

            return true;
        }

        public int RemoveWishListItem(int ProductId, int siteId, int userId)
        {
            return RemoveWishListOrNotifyListItem(ProductId, false, siteId, userId);
        }

        public int RemoveNotifyItem(int ProductId, int siteId, int userId)
        {
            return RemoveWishListOrNotifyListItem(ProductId, true, siteId, userId);
        }

        private int RemoveWishListOrNotifyListItem(int ProductId, bool onlyNotification, int SiteId, int UserId)
        {
            var wishListProduct = _currentDbContext.WebsiteWishListItems.Where(u => u.ProductId == ProductId && u.SiteID == SiteId && u.UserId == UserId && (u.IsNotification == onlyNotification || !onlyNotification)).ToList();
            if (wishListProduct.Count > 0)
            {
                foreach (var item in wishListProduct)
                {
                    item.IsDeleted = true;
                    item.UpdateUpdatedInfo(UserId);
                }

                _currentDbContext.SaveChanges();
            }
            return GetAllValidWishListItemsCount(SiteId, UserId);
        }

        public CheckoutViewModel SetCheckOutProcessModel(CheckoutViewModel checkoutViewModel, int siteId, int tenantId, int userId, string sessionKey)
        {
            var tenantCurrency = _tenantsCurrencyRateServices.GetTenantCurrencies(tenantId).FirstOrDefault();
            var currencyRate = _tenantsCurrencyRateServices.GetCurrencyRateByTenantid(tenantCurrency?.TenantCurrencyID ?? 0);
            List<AccountAddresses> accountAddresses = new List<AccountAddresses>();
            if ((DeliveryMethod?)checkoutViewModel.DeliveryMethodId == DeliveryMethod.ToShipmentAddress)
            {
                accountAddresses.Add(_accountServices.GetAccountAddressById(checkoutViewModel.ShippingAddressId ?? 0));
                var shippingRule = GetWebsiteShippingRulesById(checkoutViewModel.ShipmentRuleId ?? 0);
                shippingRule.Price = Math.Round(((shippingRule.Price) * ((currencyRate <= 0) ? 1 : currencyRate)), 2);
                checkoutViewModel.ShippingRule = _mapper.Map(shippingRule, new WebsiteShippingRulesViewModel());
            }
            accountAddresses.Add(_accountServices.GetAccountAddressById(checkoutViewModel.BillingAddressId ?? 0));
            checkoutViewModel.Addresses = _mapper.Map(accountAddresses, new List<AddressViewModel>());
            checkoutViewModel.CartItems = GetAllValidCartItems(siteId, userId, tenantId, sessionKey).ToList();
            if ((DeliveryMethod?)checkoutViewModel.DeliveryMethodId == DeliveryMethod.ToPickupPoint)
            {
                checkoutViewModel.CollectionPoint =
                    _mapper.Map((_currentDbContext.TenantWarehouses.FirstOrDefault(e => e.WarehouseId == (checkoutViewModel.CollectionPointId ?? 0) && e.IsDeleted != true && e.IsActive == true)), new CollectionPointViewModel());
            }

            return checkoutViewModel;
        }

        public void SendNotificationForAbandonedCarts()
        {
            var abandonedCartSettings = _currentDbContext.AbandonedCartSettings
                                                        .AsNoTracking()
                                                        .Where(s => s.IsNotificationEnabled &&
                                                                    s.IsDeleted != true &&
                                                                    s.TenantWebsite.IsDeleted != true &&
                                                                    s.TenantWebsite.IsActive == true);

            foreach (var settings in abandonedCartSettings)
            {
                var targetTime = DateTime.Now.AddMinutes(-1 * settings.NotificationDelay);

                var websiteCartItems = _currentDbContext.WebsiteCartItems.Where(i => i.IsDeleted != true &&
                                                                                    i.SiteID == settings.SiteID &&
                                                                                    (i.DateUpdated ?? i.DateCreated) < targetTime &&
                                                                                    i.AuthUser.IsActive == true &&
                                                                                    i.AuthUser.IsDeleted != true)
                                                                          .OrderByDescending(o => o.DateUpdated)
                                                                          .ThenByDescending(o => o.DateCreated)
                                                                          .ToList();

                var emailconfig = _emailServices.GetAllActiveTenantEmailConfigurations(settings.TenantId).FirstOrDefault();

                foreach (var item in websiteCartItems.GroupBy(i => i.UserId)
                                                    .Select(g =>
                                                    {
                                                        var item = g.First();
                                                        return new
                                                        {
                                                            Date = item.DateUpdated ?? item.DateCreated,
                                                            item.AuthUser
                                                        };
                                                    }))
                {
                    if (!_currentDbContext.AbandonedCartNotifications.Any(a => a.SiteId == settings.SiteID && a.UserId == item.AuthUser.UserId && a.SendDate >= (item.Date)))
                    {
                        var (body, subject) = GetEmailContent(settings, item.AuthUser);

                        SendNotification(item.AuthUser.UserEmail, subject, body, emailconfig);

                        _currentDbContext.AbandonedCartNotifications.Add(new AbandonedCartNotification
                        {
                            SendDate = DateTime.Now,
                            SiteId = settings.SiteID,
                            UserId = item.AuthUser.UserId
                        });
                    }
                }
            }

            _currentDbContext.SaveChanges();
        }

        public void SendProductAvailabilityNotifications()
        {
            var productAvailabilityNotificationSettings = _currentDbContext.ProductAvailabilityNotificationSettings
                                                                            .AsNoTracking()
                                                                            .Where(s => s.IsNotificationEnabled &&
                                                                                        s.IsDeleted != true &&
                                                                                        s.TenantWebsite.IsDeleted != true &&
                                                                                        s.TenantWebsite.IsActive == true);

            foreach (var settings in productAvailabilityNotificationSettings)
            {
                var emailconfig = _emailServices.GetAllActiveTenantEmailConfigurations(settings.TenantId).FirstOrDefault();

                var productAvailabilityNotifys = _currentDbContext.ProductAvailabilityNotifyQueue.Where(p => p.TenantId == settings.TenantId).ToList();

                if (productAvailabilityNotifys == null || productAvailabilityNotifys.Count <= 0)
                {
                    continue;
                }

                foreach (var productAvailabilityNotify in productAvailabilityNotifys)
                {
                    var notificationListItems = _currentDbContext.WebsiteWishListItems.Where(w => w.IsNotification &&
                                                                                                      w.IsDeleted != true &&
                                                                                                      w.ProductId == productAvailabilityNotify.ProductId &&
                                                                                                      w.TenantId == productAvailabilityNotify.TenantId &&
                                                                                                      w.SiteID == settings.SiteID);
                    //TODO add check for associated warehouses to the website

                    foreach (var item in notificationListItems)
                    {
                        var user = item.AuthUser ?? new AuthUser { UserFirstName = item.EmailId, UserEmail = item.EmailId };
                        var (body, subject) = GetEmailContent(settings, user, productAvailabilityNotify.Product);

                        SendNotification(user.UserEmail, subject, body, emailconfig);

                        item.IsDeleted = true;
                    }

                    _currentDbContext.ProductAvailabilityNotifyQueue.Remove(productAvailabilityNotify);
                }
            }

            _currentDbContext.SaveChanges();
        }

        public bool SendNotification(string to, string subject, string body, TenantEmailConfig emailconfig)
        {
            var emailSender = new EmailSender(to, emailconfig.UserEmail, body, subject, string.Empty, emailconfig.SmtpHost, emailconfig.SmtpPort, emailconfig.UserEmail, emailconfig.Password);

            return emailSender.SendMail(true);
        }

        private Tuple<string, string> GetEmailContent(AbandonedCartSetting settings, AuthUser user)
        {
            var body = PrepareNotificationTemplate(settings.NotificationEmailTemplate, settings.TenantWebsite, user, null);

            var subject = PrepareNotificationTemplate(settings.NotificationEmailSubjectTemplate, settings.TenantWebsite, user, null);

            return new Tuple<string, string>(body, subject);
        }

        private Tuple<string, string> GetEmailContent(ProductAvailabilityNotificationSetting settings, AuthUser user, ProductMaster product)
        {
            var body = PrepareNotificationTemplate(settings.NotificationEmailTemplate, settings.TenantWebsite, user, product);

            var subject = PrepareNotificationTemplate(settings.NotificationEmailSubjectTemplate, settings.TenantWebsite, user, product);

            return new Tuple<string, string>(body, subject);
        }

        private string PrepareNotificationTemplate(string template, TenantWebsites tenantWebsite, AuthUser user, ProductMaster product)
        {
            var productSkuCode = product?.SKUCode;
            var productDefaultImage = product?.DefaultImage;
            var productDescription = product?.Description;
            if (product?.ProductType == ProductKitTypeEnum.Simple)
            {
                var parentProduct = _productServices.GetParentProductsByKitProductId(product.ProductId)
                                    .FirstOrDefault(k => k.IsActive == true &&
                                                            k.ProductType == ProductKitTypeEnum.ProductByAttribute &&
                                                            k.IsDeleted != true);

                productSkuCode = parentProduct?.SKUCode ?? productSkuCode;
                productDefaultImage = parentProduct?.DefaultImage ?? productDefaultImage;
                productDescription = parentProduct?.Description ?? productDescription;
            }

            template = Regex.Replace(template, "{CUSTOMERFULLNAME}", $"{user?.UserFirstName} {user?.UserLastName}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{CUSTOMERFIRSTNAME}", $"{user?.UserFirstName}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{CUSTOMERLASTNAME}", $"{user?.UserLastName}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{PRODUCTNAME}", $"{product?.Name}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{PRODUCTSKUCODE}", $"{productSkuCode}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{PRODUCTDEFAULTIMAGE}", $"{tenantWebsite?.BaseFilePath + productDefaultImage}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{PRODUCTDESCRIPTION}", $"{productDescription}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{PRODUCTID}", $"{product?.ProductId}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{WEBSITENAME}", $"{tenantWebsite?.SiteName}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{WEBSITEURL}", $"http://{tenantWebsite?.HostName}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{WEBSITECONTACTEMAIL}", $"{tenantWebsite?.WebsiteContactEmail}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{WEBSITECONTACTPHONE}", $"{tenantWebsite?.WebsiteContactPhone}", RegexOptions.IgnoreCase);
            template = Regex.Replace(template, "{WEBSITECONTACTADDRESS}", $"{tenantWebsite?.WebsiteContactAddress}", RegexOptions.IgnoreCase);

            return template;
        }

        public IEnumerable<WebsiteDeliveryNavigation> GetAllValidWebsiteDeliveryNavigations(int tenantId, int siteId, bool includeInActive = false)
        {
            return _currentDbContext.WebsiteDeliveryNavigations.Where(u => u.TenantId == tenantId && u.IsDeleted != true
            && u.SiteId == siteId && (includeInActive || u.IsActive));
        }

        public WebsiteDeliveryNavigation GetWebsiteDeliveryNavigationById(int id)
        {
            return _currentDbContext.WebsiteDeliveryNavigations.Find(id);
        }

        public WebsiteDeliveryNavigation CreateOrUpdateWebsiteDeliveryNavigation(
            WebsiteDeliveryNavigation websiteDeliveryNavigation, int userId, int tenantId)
        {
            if (websiteDeliveryNavigation.Id <= 0)
            {
                websiteDeliveryNavigation.TenantId = tenantId;
                websiteDeliveryNavigation.UpdateCreatedInfo(userId);
                _currentDbContext.WebsiteDeliveryNavigations.Add(websiteDeliveryNavigation);
                _currentDbContext.SaveChanges();
            }
            else
            {
                var updatedRecored = _currentDbContext.WebsiteDeliveryNavigations.AsNoTracking().FirstOrDefault(u => u.Id == websiteDeliveryNavigation.Id);
                if (updatedRecored != null)
                {
                    websiteDeliveryNavigation.CreatedBy = updatedRecored.CreatedBy;
                    websiteDeliveryNavigation.DateCreated = updatedRecored.DateCreated;
                    websiteDeliveryNavigation.TenantId = tenantId;
                    websiteDeliveryNavigation.UpdateUpdatedInfo(userId);
                    _currentDbContext.Entry(websiteDeliveryNavigation).State = System.Data.Entity.EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }
            }

            return websiteDeliveryNavigation;
        }

        public int RemoveWebsiteDeliveryNavigation(int id, int userId)
        {
            var updatedRecored = _currentDbContext.WebsiteDeliveryNavigations.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (updatedRecored != null)
            {
                updatedRecored.IsDeleted = true;
                updatedRecored.UpdateUpdatedInfo(userId);
                _currentDbContext.Entry(updatedRecored).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }

            return updatedRecored.SiteId;
        }

        public decimal? GetPriceForProduct(int productId, int siteId, int? accountId = null)
        {
            var product = _currentDbContext.ProductMaster.AsNoTracking().FirstOrDefault(u => u.ProductId == productId);

            if ((product.SellPrice == null || product.SellPrice <= 0) && (product.ProductType == ProductKitTypeEnum.ProductByAttribute || product.ProductType == ProductKitTypeEnum.Grouped))
            {
                product = _currentDbContext.ProductKitMaps.Where(p => p.ProductId == productId &&
                                                                      p.ProductKitTypes != null && 
                                                                      p.ProductKitTypes.UseInParentCalculations == true &&
                                                                      p.IsActive &&
                                                                      p.IsDeleted != true &&
                                                                      p.KitProductMaster.SellPrice > 0)
                                                          .OrderBy(p => p.ProductKitTypes.SortOrder)
                                                          .ThenBy(p => p.KitProductMaster.SellPrice)
                                                          .FirstOrDefault()?.KitProductMaster ?? product;
            }

            if (product.SellPrice == null)
                return null;

            var calculateTax = GetTenantWebSiteBySiteId(siteId).ShowPricesIncludingTax;
            var sellPrice = _productPriceService.GetProductPriceThresholdByAccountId(product.ProductId, accountId, siteId).SellPrice;
            if (calculateTax)
            {
                if (product != null && product.TaxID > 0 && product.EnableTax == true)
                {
                    var taxPercentage = _currentDbContext.GlobalTax.AsNoTracking().FirstOrDefault(a => a.TaxID == product.TaxID)?.PercentageOfAmount;
                    if (taxPercentage.HasValue && taxPercentage > 0)
                    {
                        var taxAmount = product.SellPrice.HasValue
                            ? Math.Round(d: ((product.SellPrice.Value) / 100) * (taxPercentage.Value), 2)
                            : 0;
                        sellPrice = sellPrice + taxAmount;
                    }
                }
            }

            return sellPrice;
        }

        public List<ProductPriceViewModel> GetPricesForProducts(List<int> productIds, int siteId, int? accountId)
        {
            return productIds.Select(productId =>
            {
                var calculateTax = GetTenantWebSiteBySiteId(siteId).ShowPricesIncludingTax;
                var productPrice = new ProductPriceViewModel
                {
                    FinalSellPrice = _productPriceService.GetProductPriceThresholdByAccountId(productId, accountId, siteId).SellPrice,
                    ProductId = productId
                };
                if (calculateTax)
                {
                    var product = _currentDbContext.ProductMaster.AsNoTracking().FirstOrDefault(u => u.ProductId == productId);

                    if (product != null && product.TaxID > 0 && product.EnableTax == true)
                    {
                        var taxPercentage = _currentDbContext.GlobalTax.AsNoTracking().FirstOrDefault(a => a.TaxID == product.TaxID)?.PercentageOfAmount;
                        if (taxPercentage.HasValue && taxPercentage > 0)
                        {
                            var taxAmount = product.SellPrice.HasValue
                                ? Math.Round(d: ((product.SellPrice.Value) / 100) * (taxPercentage.Value), 2)
                                : 0;
                            productPrice.TaxAmount = taxAmount;
                            productPrice.TaxPercentage = taxPercentage.Value;
                        }
                    }
                }

                return productPrice;
            }).ToList();
        }

        public decimal? GetProductByAttributeAvailableCount(int productId, List<int> warehouseIds)
        {
            var product = _productServices.GetProductMasterById(productId);
            var kitProductMaster = product.ProductKitItems.Where(u => u.IsDeleted != true && u.IsActive)
                .Select(a => a.KitProductMaster.ProductId).Distinct().ToList();
            foreach (var item in kitProductMaster)
            {
                var childProduct = _productServices.GetProductMasterById(item);
                var stock = childProduct?.InventoryStocks?.Where(u => warehouseIds.Contains(u.WarehouseId)).ToList();
                if (childProduct != null)
                {
                    if ((childProduct.IsStockItem == true && childProduct.DontMonitorStock == true))
                    {
                        return null;
                    }

                    if (stock != null && stock.Count > 0)
                    {
                        var stockCount = stock.Select(q => q.Available).DefaultIfEmpty(0).Sum();
                        if (stockCount > 0) return stockCount;
                        if (childProduct.IsPreOrderAccepted == true) return null;
                    }
                }
            }

            return 0;
        }
    }
}