using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Ganedata.Core.Services
{
    public interface ITenantWebsiteService
    {
        // TenantWebsite//
        IEnumerable<TenantWebsites> GetAllValidTenantWebSite(int TenantId);
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
        IQueryable<WebsiteNavigation> GetAllValidWebsiteNavigationTopCategory(int TenantId, int? SiteId);
        IQueryable<ProductMaster> GetProductByNavigationId(int navigationId);

        //WebsiteShippingRules

        IQueryable<WebsiteShippingRules> GetAllValidWebsiteShippingRules(int TenantId, int SiteId);
        List<WebsiteShippingRulesViewModel> GetShippingRulesByShippingAddress(int tenantId, int siteId, int shippingAddressId, double parcelWeightInGrams);
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
        Tuple<string, string> AllPriceListAgainstGroupAndDept(IQueryable<ProductMaster> productMasters);
        IEnumerable<ProductManufacturer> GetAllValidProductManufacturerGroupAndDeptByName(IQueryable<ProductMaster> productMasters);
        string CategoryAndSubCategoryBreedCrumb(int siteId, int? productId = null, string Category = "", string SubCategory = "");

        // WebsiteCartAndWishlist

        IEnumerable<WebsiteCartItem> GetAllValidCartItemsList(int siteId, int? UserId, string SessionKey);
        IEnumerable<KitProductCartSession> GetAllValidKitCartItemsList(int KitProductId);
        IEnumerable<WebsiteWishListItem> GetAllValidWishListItemsList(int siteId, int UserId);


        OrderDetailSessionViewModel SetCartItem(int productId, decimal quantity, decimal? currencyRate, int? currencyId);

        int AddOrUpdateCartItem(int siteId, int? userId, int tenantId, string sessionKey, int productId, decimal quantity, List<KitProductCartSession> kitProductCartItems = null);
        bool UpdateCartItemQuantity(int siteId, int? userId, int tenantId, string sessionKey, int cartId, decimal quantity);

        IEnumerable<WebsiteCartItemViewModel> GetAllValidCartItems(int siteId, int? userId, int tenantId, string sessionKey, int? cartId = null);

        void UpdateUserIdInCartItem(string sessionKey, int userId, int siteId);

        int AddOrUpdateWishListItems(int SiteId, int UserId, int TenantId, List<OrderDetailSessionViewModel> wishListDetail);
        bool RemoveCartItem(int cartId, int siteId, int? userId, string sessionKey);
        int RemoveWishListItem(int ProductId, bool notification, int SiteId, int UserId);
        bool GetWishlistNotificationStatus(int productId, int siteId, int userId);
        bool ChangeWishListStatus(int productId, bool notification, int siteId, int userId);


        void SendNotificationForAbandonedCarts();
        CheckoutViewModel SetCheckOutProcessModel(CheckoutViewModel checkoutViewModel, int siteId, int tenantId, int userId, string sessionKey);

        // deliveryInfoNavigation//
        IEnumerable<WebsiteDeliveryNavigation> GetAllValidWebsiteDeliveryNavigations(int tenantId, int siteId, bool includeInactive = false);
        WebsiteDeliveryNavigation GetWebsiteDeliveryNavigationById(int id);
        WebsiteDeliveryNavigation CreateOrUpdateWebsiteDeliveryNavigation(WebsiteDeliveryNavigation websiteDeliveryNavigation, int userId, int tenantId);
        int RemoveWebsiteDeliveryNavigation(int id, int userId);

        decimal GetPriceForProduct(int productId, int siteId);



    }
}