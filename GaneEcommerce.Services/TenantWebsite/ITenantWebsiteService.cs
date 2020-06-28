﻿using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
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
        TenantWebsites GetTenantWebSiteBySiteId(int SiteId);
        TenantWebsites CreateOrUpdateTenantWebsite(TenantWebsites tenantWebsites, int UserId, int TenantId);
        bool RemoveTenantWebsite(int siteID, int UserId);

        // WebsiteContentPages//

        IEnumerable<WebsiteContentPages> GetAllValidWebsiteContentPages(int TenantId, int SiteId);
        WebsiteContentPages CreateOrUpdateWebsiteContentPages(WebsiteContentPages WebsiteContentPages, int UserId, int TenantId);
        WebsiteContentPages RemoveWebsiteContentPages(int Id, int UserId);
        WebsiteContentPages GetWebsiteContentById(int Id);
        WebsiteContentPages GetWebsiteContentByUrl(int siteId, string url);

        //WebsiteWarehouses
        bool CreateOrUpdateWebsiteWarehouse(WebsiteWarehousesViewModel websiteWarehouseData, int UserId, int TenantId);
        IQueryable<WebsiteWarehousesViewModel> GetAllValidWebsiteWarehouses(int TenantId, int SiteId);


        //ProductsWebsitesMaps
        IEnumerable<ProductsWebsitesMap> GetAllValidProductsWebsitesMap(int TenantId);

        //WebsiteSliders
        IEnumerable<WebsiteSlider> GetAllValidWebsiteSlider(int TenantId, int SiteId);
        WebsiteSlider CreateOrUpdateProductswebsiteSlider(WebsiteSlider websiteSlider, int UserId, int TenantId);
        WebsiteSlider RemoveWebsiteSlider(int Id, int UserId);
        WebsiteSlider GetWebsiteSliderById(int Id);
        //WebsiteNavigation

        IEnumerable<WebsiteNavigation> GetAllValidWebsiteNavigation(int TenantId, int? SiteId);
        IQueryable<NavigationProductsViewModel> GetAllValidWebsiteNavigations(int TenantId, int SiteId, int navigationId);
        WebsiteNavigation CreateOrUpdateWebsiteNavigation(WebsiteNavigation websiteNavigation, int UserId, int TenantId);
        WebsiteNavigation RemoveWebsiteNavigation(int Id, int UserId);
        WebsiteNavigation GetWebsiteNavigationId(int NavigationId);
        bool CreateOrUpdateWebsiteNavigationProducts(NavigationProductsViewModel navigationProduct, int UserId, int TenantId);
        IQueryable<NavigationProductsViewModel> GetAllValidWebsiteProducts(int TenantId, int SiteId);
        bool CreateOrUpdateWebsiteProducts(NavigationProductsViewModel websiteProduct, int UserId, int TenantId);
        IQueryable<WebsiteNavigation> GetAllValidWebsiteNavigationCategory(int TenantId, int? SiteId);
        IQueryable<ProductMaster> GetProductByNavigationId(int navigationId);

        //WebsiteShippingRules

        IQueryable<WebsiteShippingRules> GetAllValidWebsiteShippingRules(int TenantId, int SiteId);
        WebsiteShippingRules CreateOrUpdateWebsiteShippingRules(WebsiteShippingRules websiteShippingRules, int UserId, int TenantId);
        WebsiteShippingRules RemoveWebsiteShippingRules(int Id, int UserId);
        WebsiteShippingRules GetWebsiteShippingRulesById(int Id);

        // website Voucher

        IQueryable<WebsiteVouchers> GetAllValidWebsiteVoucher(int TenantId, int SiteId);

        WebsiteVouchers CreateOrUpdateWebsiteVouchers(WebsiteVouchers websiteVouchers, int UserId, int TenantId);

        WebsiteVouchers RemoveWebsiteVoucher(Guid Id, int UserId);
        WebsiteVouchers GetWebsiteVoucherById(Guid Id);
        string GenerateVoucherCode();

        //WebsiteDiscountCodes
        IEnumerable<WebsiteDiscountCodes> GetAllValidWebsiteDiscountCodes(int TenantId, int SiteId);
        WebsiteDiscountCodes CreateOrUpdateWebsiteDiscountCodes(WebsiteDiscountCodes websiteDiscount, int UserId, int TenantId);
        WebsiteDiscountCodes RemoveWebsiteDiscountCodes(int Id, int UserId);
        WebsiteDiscountCodes GetWebsiteDiscountCodesById(int discountId);
        IEnumerable<WebsiteDiscountProductsMap> GetAllValidWebsiteDiscountProductsMap(int TenantId, int DiscountId);
        IQueryable<NavigationProductsViewModel> GetAllValidWebsiteProductsMap(int TenantId, int SiteId);
        //ProductAllowances
        IEnumerable<ProductAllowance> GetAllValidProductAllowance(int TenantId, int SiteId);
        ProductAllowance CreateOrUpdateProductAllowance(ProductAllowance productAllowance, string Reason, int UserId, int TenantId);
        ProductAllowance RemoveProductAllowance(int Id, int UserId);
        ProductAllowance GetProductAllowanceById(int Id);


        //ProductAllowancesGroups
        IEnumerable<ProductAllowanceGroup> GetAllValidProductAllowanceGroups(int TenantId, int SiteId);
        ProductAllowanceGroup CreateOrUpdateProductGroupAllowance(ProductAllowanceGroup productAllowanceGroup, int UserId, int TenantId);
        ProductAllowanceGroup RemoveProductAllowanceGroup(int Id, int UserId);
        ProductAllowanceGroup GetProductAllowanceGroupById(int Id);
        IEnumerable<ProductAllowanceGroupMap> GetAllValidProductAllowanceGroupMap(int TenantId, int productAllownceGroupId);



        // WebsiteSearching Realted Queries
        IQueryable<ProductMaster> GetAllValidProductWebsiteSearch(int siteId, string category = "", string ProductName = "");
        Dictionary<string, List<ProductAttributeValues>> GetAllValidProductAttributeValuesByProductIds(IQueryable<ProductMaster> product);
        List<Tuple<string, string>> AllPriceListAgainstGroupAndDept(IQueryable<ProductMaster> productMasters);
        IEnumerable<ProductManufacturer> GetAllValidProductManufacturerGroupAndDeptByName(IQueryable<ProductMaster> productMasters);
        string CategoryAndSubCategoryBreedCrumb(int siteId, int? productId = null, string Category = "", string SubCategory = "");

        // WebsiteCartAndWishlist

        IEnumerable<WebsiteCartItem> GetAllValidCartItemsList(int siteId, int UserId);
        IEnumerable<KitProductCartSession> GetAllValidKitCartItemsList(int KitProductId);
        IEnumerable<WebsiteWishListItem> GetAllValidWishListItemsList(int siteId, int UserId);

        int AddOrUpdateCartItems(int SiteId, int UserId, int TenantId, List<OrderDetailSessionViewModel> orderDetail);

        int AddOrUpdateWishListItems(int SiteId, int UserId, int TenantId, List<OrderDetailSessionViewModel> orderDetails);

        int RemoveCartItem(int ProductId, int SiteId, int UserId);
        int RemoveWishListItem(int ProductId, int SiteId, int UserId);

        void SendNotificationForAbandonedCarts();
    }
}