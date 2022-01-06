using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IProductPriceService
    {

        decimal GetPercentageMarginPrice(decimal? price, decimal customMarginPercent);

        ProductSaleQueryResponse GetProductPriceThresholdByAccountId(int productId, int? accountId = null, int siteId = 0);

        List<ProductPriceHistoryModel> GetProductPriceHistoryForAccount(int productId, int accountid);
        decimal GetProductLastPurchasePriceForAccount(int productId, int accountid);

        decimal GetProductLastSellPriceForAccount(int productId, int accountid);

        ProductSpecialPriceViewModel SaveSpecialProductPrice(int productId, decimal price, int priceGroupId, DateTime? startDate = null, DateTime? endDate = null, int currentTenantId = 0, int userId = 0);

        ProductSpecialPriceViewModel GetSpecialProductPriceById(int specialProductPriceId);

        bool DeleteSpecialProductPriceById(int specialProductPriceId, int userId);

        List<ProductSpecialPriceViewModel> GetAllSpecialProductPrices(int tenantId, int priceGroupId = 0);

        TenantPriceGroups SavePriceGroup(int priceGroupId, string name, decimal percent, int tenantId, int currentUserId, bool ApplyDiscountOnTotal, bool ApplyDiscountOnSpecialPrice);

        bool DeletePriceGroupById(int priceGroupId, int userId);

        TenantPriceGroups GetTenantPriceGroupById(int priceGroupId);

        IQueryable<TenantPriceGroups> GetAllTenantPriceGroups(int tenantId, bool includeIsDeleted = false);

        IQueryable<TenantPriceGroupDetail> GetAllTenantPriceGroupDetails(int tenantId, bool includeIsDeleted = false);

        decimal? GetPurchasePrice(int productId, int tenantId, DateTime? date = null, int? orderId = null);
        List<InvoiceProductPriceModel> GetInvoiceDetailsProductPrices(List<InvoiceDetail> invoiceDetails, int tenantId);
        decimal? GetAveragePurchasePrice(int productId, int tenantId);
    }
}