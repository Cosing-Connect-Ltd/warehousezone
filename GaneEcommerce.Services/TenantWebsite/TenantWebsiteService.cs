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

        public IQueryable<NavigationProductsViewModel> GetAllValidWebsiteProducts(int TenantId, int SiteId)
        {
            var allProducts = _currentDbContext.ProductMaster.Where(m => m.TenantId == TenantId && m.IsDeleted != true);
            var websiteMap = _currentDbContext.ProductsWebsitesMap.Where(u => u.IsDeleted != true && u.SiteID == SiteId && u.TenantId == TenantId);



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
        //WebsiteShippingRules

        public IQueryable<WebsiteShippingRules> GetAllValidWebsiteShippingRules(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteShippingRules.Where(u => u.TenantId == TenantId && u.SiteID == SiteId && u.IsDeleted != true);
            
        }
        public WebsiteShippingRules CreateOrUpdateWebsiteShippingRules(WebsiteShippingRules websiteShippingRules, int UserId, int TenantId)
        {
            var websiteShippingRulesItems = _currentDbContext.WebsiteShippingRules.Where(x => x.TenantId == TenantId
           && x.SiteID == websiteShippingRules.SiteID && x.IsDeleted != true && x.Id==websiteShippingRules.Id).FirstOrDefault();
            if (websiteShippingRulesItems == null)
            {
                WebsiteShippingRules websiteShipping = new WebsiteShippingRules();
                websiteShipping.SiteID = websiteShippingRules.SiteID;
                websiteShipping.CountryId = websiteShippingRules.CountryId;
                websiteShipping.Courier = websiteShippingRules.Courier;
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
            return _currentDbContext.WebsiteShippingRules.FirstOrDefault(u => u.Id == Id && u.IsDeleted != true);

        }

        // website Voucher

        public IQueryable<WebsiteVouchers> GetAllValidWebsiteVoucher(int TenantId, int SiteId)
        {
            return _currentDbContext.WebsiteVouchers.Where(u => u.TenantId == TenantId && u.SiteID == SiteId && u.IsDeleted != true);

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
            return _currentDbContext.WebsiteVouchers.FirstOrDefault(u => u.Id == Id && u.IsDeleted != true);

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


            return otp.Insert(4,"-").Insert(9,"-").Insert(14,"-").ToUpper();
        }

        //WebsiteDiscountCodes
        public IEnumerable<WebsiteDiscountCodes> GetAllValidWebsiteDiscountCodes(int TenantId, int SiteId)
        {

            return _currentDbContext.WebsiteDiscountCodes.Where(u=>u.TenantId==TenantId && u.SiteID==SiteId && u.IsDeleted != true);
        }
        public IEnumerable<WebsiteDiscountProductsMap> GetAllValidWebsiteDiscountProductsMap(int TenantId, int DiscountId)
        {

            return _currentDbContext.websiteDiscountProductsMaps.Where(u => u.TenantId == TenantId && u.IsDeleted != true && u.DiscountId==DiscountId);
        }

        public WebsiteDiscountCodes CreateOrUpdateWebsiteDiscountCodes(WebsiteDiscountCodes websiteDiscount, int UserId, int TenantId)
        {
            var websiteDiscountItems = _currentDbContext.WebsiteDiscountCodes.Where(x => x.TenantId == TenantId
          && x.SiteID == websiteDiscount.SiteID && x.IsDeleted != true && x.Id == websiteDiscount.Id).FirstOrDefault();
            if (websiteDiscountItems == null)
            {
                WebsiteDiscountCodes websiteDiscountCodes  = new WebsiteDiscountCodes();
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
                        DiscountProductsMap.ProductId = item;
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
                    .Where(a => a.IsDeleted != true && a.DiscountId == websiteDiscount.Id).Select(a => a.ProductId)
                    .ToList()).ToList();
                    var toDelete = _currentDbContext.websiteDiscountProductsMaps
                    .Where(a => a.IsDeleted != true && a.DiscountId == websiteDiscount.Id).Select(a => a.ProductId).Except(productWebMapIds);
                    foreach (var item in Toadd)
                    {
                        WebsiteDiscountProductsMap DiscountProductsMap = new WebsiteDiscountProductsMap();
                        DiscountProductsMap.DiscountId = websiteDiscount.Id;
                        DiscountProductsMap.ProductId = item;
                        DiscountProductsMap.SortOrder = 1;
                        DiscountProductsMap.TenantId = TenantId;
                        DiscountProductsMap.IsActive = true;
                        DiscountProductsMap.UpdateCreatedInfo(UserId);
                        _currentDbContext.websiteDiscountProductsMaps.Add(DiscountProductsMap);

                    }
                    foreach (var item in toDelete)
                    {
                        var productsNavigation = _currentDbContext.websiteDiscountProductsMaps.FirstOrDefault(u => u.ProductId == item);
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
        public IEnumerable<ProductAllowance> GetAllValidProductAllowance(int TenantId)
        {
            return _currentDbContext.ProductAllowance.Where(u => u.TenantId == TenantId && u.IsDeleted != true);
        }
        public ProductAllowance CreateOrUpdateProductAllowance(ProductAllowance productAllowance, int UserId, int TenantId)
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
        public IEnumerable<ProductAllowanceGroup> GetAllValidProductAllowanceGroups(int TenantId)
        {
            return _currentDbContext.ProductAllowanceGroup.Where(u => u.TenantId == TenantId && u.IsDeleted != true);
        }
        //ProductAllowance CreateOrUpdateProductAllowance(ProductAllowance productAllowance, int UserId, int TenantId);
        //ProductAllowance RemoveProductAllowance(int Id, int UserId);
        //ProductAllowance GetProductAllowanceById(int Id);



    }
}