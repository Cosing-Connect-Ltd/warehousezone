using System;
using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public class InventoryAvailabilityExtensions
    {
        public static bool IsProductAvailableToSell(ProductMaster product, int siteId)
        {
            return product.SellPrice > 0 && product.SellPrice.HasValue && (GetAvailableProductCount(product, siteId) ?? 20) > 0;
        }

        public static bool IsProductInWishList(ProductMaster product, int userId)
        {
            return IsInWishListOrNotifyList(product, userId, false);
        }

        public static bool IsProductInNotifyList(ProductMaster product, int userId)
        {
            return IsInWishListOrNotifyList(product, userId, true);
        }

        public static bool IsProductInWishList(int productId, int userId)
        {
            var productService = DependencyResolver.Current.GetService<IProductServices>();
            var product = productService.GetProductMasterById(productId);
            return IsInWishListOrNotifyList(product, userId, false);
        }

        public static bool IsProductInNotifyList(int productId, int userId)
        {
            var productService = DependencyResolver.Current.GetService<IProductServices>();
            var product = productService.GetProductMasterById(productId);
            return IsInWishListOrNotifyList(product, userId, true);
        }

        private static bool IsInWishListOrNotifyList(ProductMaster product, int userId, bool isNoftification)
        {
            if (product.ProductType != ProductKitTypeEnum.ProductByAttribute)
            {
                return product.WebsiteWishListItems.Any(u => u.IsDeleted != true && u.UserId == userId && u.IsNotification == isNoftification);
            }
            else
            {
                return product.ProductKitItems.Any(u => u.IsDeleted != true &&
                                                        u.IsActive &&
                                                        u.KitProductMaster.WebsiteWishListItems.Any(a => a.IsDeleted != true && a.UserId == userId && a.IsNotification == isNoftification));
            }
        }

        public static decimal? GetAvailableProductCount(ProductMaster product, int siteId)
        {
            decimal? availableProductCount = 0;
            var tenantWebsiteService = DependencyResolver.Current.GetService<ITenantWebsiteService>();

            var tenantWebsite = tenantWebsiteService.GetTenantWebSiteBySiteId(siteId);
            var warehouseIds = tenantWebsiteService.GetAllValidWebsiteWarehouses(tenantWebsite.TenantId, siteId).Select(u => u.Id).ToList();

            if (product != null)
            {
                if ((product.DontMonitorStock == true && product.IsStockItem == true))
                {
                    return null;
                }

                switch (product.ProductType)
                {
                    case ProductKitTypeEnum.ProductByAttribute:
                        availableProductCount = tenantWebsiteService.GetProductByAttributeAvailableCount(product.ProductId, warehouseIds);
                        break;
                    case ProductKitTypeEnum.Simple:
                        if (product.InventoryStocks != null && product.InventoryStocks.Count > 0)
                        {
                            availableProductCount += product.InventoryStocks.Where(u => warehouseIds.Contains(u.WarehouseId)).Select(q => q.Available).DefaultIfEmpty(0).Sum(); ;
                        }
                        break;

                    default:
                        availableProductCount = 0;
                        break;

                }
            }

            return availableProductCount > 0 ? availableProductCount : (product.IsPreOrderAccepted == true ? (decimal?)null : 0);
        }

        public static void AddProductToNotifyQueue(int productId, int tenantId, int warehouseId, IApplicationContext context = null)
        {
            if (!context.ProductAvailabilityNotifyQueue.Any(n => n.ProductId == productId &&
                                                                 n.TenantId == tenantId &&
                                                                 n.WarehouseId == warehouseId))
            {
                var productAvailabilityNotify = new ProductAvailabilityNotifyQueue
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    TenantId = tenantId,
                    DateCreated = DateTime.Now
                };
                context.ProductAvailabilityNotifyQueue.Add(productAvailabilityNotify);
            }
        }

        public static void RemoveProductFromNotifyQueue(int productId, int tenantId, int warehouseId, IApplicationContext context = null)
        {
            var productAvailabilityNotify = context.ProductAvailabilityNotifyQueue.FirstOrDefault(n => n.ProductId == productId &&
                                                                                                       n.TenantId == tenantId &&
                                                                                                       n.WarehouseId == warehouseId);
            if (productAvailabilityNotify != null)
            {
                context.ProductAvailabilityNotifyQueue.Remove(productAvailabilityNotify);
            }
        }

    }
}