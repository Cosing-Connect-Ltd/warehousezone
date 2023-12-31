﻿using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface ITenantWebsiteService
    {
        // TenantWebsite//
        bool VerifyProductAgainstWebsite(int productId, int siteId);
        IEnumerable<TenantWebsites> GetAllActiveTenantWebSites(int TenantId);
        IEnumerable<TenantWebsites> GetAllValidTenantWebSites(int tenantId);
        WebsiteLayoutSettings GetWebsiteLayoutSettingsInfoBySiteId(int SiteId);
        TenantWebsites GetTenantWebSiteBySiteId(int SiteId);
        TenantWebsites CreateOrUpdateTenantWebsite(TenantWebsites tenantWebsites, int UserId, int TenantId);
        WebsiteLayoutSettings SaveWebsiteLayoutSettings(WebsiteLayoutSettings websiteLayoutSettings, int UserId, int TenantId);
        bool RemoveTenantWebsite(int siteID, int UserId);

        // WebsiteContentPages//

        IEnumerable<WebsiteContentPages> GetAllValidWebsiteContentPages(int TenantId, int SiteId);
        WebsiteContentPages CreateOrUpdateWebsiteContentPages(WebsiteContentPages WebsiteContentPages, int UserId, int TenantId);
        WebsiteContentPages RemoveWebsiteContentPages(int Id, int UserId);
        WebsiteContentPages GetWebsiteContentById(int Id);
        WebsiteContentPages GetWebsiteContentByUrl(int? siteId, string url);

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
        WebsiteNavigation GetWebsiteNavigationId(int navigationId);
        bool CreateOrUpdateWebsiteNavigationProducts(NavigationProductsViewModel navigationProduct, int UserId, int TenantId);
        IQueryable<NavigationProductsViewModel> GetAllValidWebsiteProducts(int tenantId, int SiteId);
        bool CreateOrUpdateWebsiteProducts(NavigationProductsViewModel websiteProduct, int UserId, int TenantId);
        IQueryable<WebsiteNavigation> GetAllValidWebsiteNavigationCategory(int tenantId, int? SiteId);
        IQueryable<WebsiteNavigation> GetAllValidWebsiteNavigationTopCategory(int TenantId, int? SiteId);
        IQueryable<ProductMaster> GetProductByNavigationId(int navigationId);

        //WebsiteShippingRules

        IQueryable<WebsiteShippingRules> GetAllValidWebsiteShippingRules(int TenantId, int SiteId);
        List<WebsiteShippingRulesViewModel> GetShippingRulesByShippingAddressId(int tenantId, int siteId, int shippingAddressId, double parcelWeightInGrams);
        List<WebsiteShippingRulesViewModel> GetShippingRulesByPostCode(int tenantId, int siteId, string postCode, double parcelWeightInGrams);
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
        List<ProductSearchResultViewModel> SearchWebsiteProducts(int siteId, int resultCount, string productName = "");
        IQueryable<ProductMaster> GetWebsiteProducts(int siteId, string category = "", string ProductName = "", int? categoryId = null);
        IQueryable<ProductMaster> GetAllValidProductForDynamicFilter(int siteId, List<int> productIds);
        Dictionary<ProductAttributes, List<ProductAttributeValues>> GetAllValidProductAttributeValuesByProductIds(IQueryable<ProductMaster> products);
        Tuple<string, string> GetAvailablePricesRange(List<int> products,int siteId, int? accountId);
        Dictionary<int, string> GetAllValidProductManufacturers(List<int> productIds);
        WebsiteNavigation GetProductCategoryByProductId(int siteId, int productId);

        // WebsiteCartAndWishlist

        IEnumerable<WebsiteCartItem> GetAllValidCartItemsList(int siteId, int? UserId, string SessionKey);
        IEnumerable<KitProductCartSession> GetAllValidKitCartItemsList(int KitProductId);
        IEnumerable<WebsiteWishListItem> GetAllValidWishListItemsList(int siteId, int UserId);
        int GetAllValidWishListItemsCount(int siteId, int UserId);

        int AddOrUpdateCartItem(int siteId, int? userId, int tenantId, string sessionKey, int productId, decimal quantity, List<KitProductCartSession> kitProductCartItems = null);
        bool UpdateCartItemQuantity(int siteId, int? userId, int tenantId, string sessionKey, int cartId, decimal quantity);

        IEnumerable<WebsiteCartItemViewModel> GetAllValidCartItems(int siteId, int? userId, int tenantId, string sessionKey, int? cartId = null);

        void UpdateUserIdInCartItem(string sessionKey, int userId, int siteId);

        int AddOrUpdateWishListItems(int siteId, int userId, int tenantId, int productId);
        int AddOrUpdateNotifyListItems(int siteId, int userId, int tenantId, int productId, string emailId);
        bool RemoveCartItem(int cartId, int siteId, int? userId, string sessionKey);
        int RemoveWishListItem(int ProductId, int siteId, int userId);
        int RemoveNotifyItem(int ProductId, int siteId, int userId);
        bool GetWishlistNotificationStatus(int productId, int siteId, int userId);
        bool ChangeWishListStatus(int productId, bool notification, int siteId, int userId);


        void SendNotificationForAbandonedCarts();
        void SendProductAvailabilityNotifications();
        CheckoutViewModel SetCheckOutProcessModel(CheckoutViewModel checkoutViewModel, int siteId, int tenantId, int userId, string sessionKey);

        // deliveryInfoNavigation//
        IEnumerable<WebsiteDeliveryNavigation> GetAllValidWebsiteDeliveryNavigations(int tenantId, int siteId, bool includeInactive = false);
        WebsiteDeliveryNavigation GetWebsiteDeliveryNavigationById(int id);
        WebsiteDeliveryNavigation CreateOrUpdateWebsiteDeliveryNavigation(WebsiteDeliveryNavigation websiteDeliveryNavigation, int userId, int tenantId);
        int RemoveWebsiteDeliveryNavigation(int id, int userId);

        decimal? GetPriceForProduct(int productId, int siteId, int? accountId);

        List<ProductPriceViewModel> GetPricesForProducts(List<int> productIds, int siteId, int? accountId);

        decimal? GetProductByAttributeAvailableCount(int productId, List<int> warehouseIds);
        List<ProductManufacturer> GetWebsiteProductManufacturers(int siteId);

    }
}