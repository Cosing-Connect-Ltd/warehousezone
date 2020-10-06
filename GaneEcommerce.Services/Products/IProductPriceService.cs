using System;
using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public interface IProductPriceService
    {
        ProductSaleQueryResponse GetProductSalePriceById(int productId);


        decimal GetPercentageMarginPrice(decimal? price, decimal customMarginPercent);
        ProductSaleQueryResponse GetProductPriceThresholdByAccountId(int productId, int? accountId = null);

        ProductSaleQueryResponse CanTheProductBeSoldAtPriceToAccount(int productId, int accountId, decimal sellingPrice);

        ProductSaleQueryResponse CanTheProductBeSoldAtPercentageDiscountToAccount(int productId, int accountId, decimal customDiscountPercent);

        List<ProductPriceHistoryModel> GetProductPriceHistoryForAccount(int productId, int accountid);

        decimal GetLastProductPriceForAccount(int productId, int accountId, InventoryTransactionTypeEnum transactionType);
        decimal GetLastSoldProductPriceForAccount(int productId, int accountId);
        decimal GetLastPurchaseProductPriceForAccount(int productId, int accountId);

        decimal GetTaxAmountProductPriceForAccount(int productId, int accountId);

        ProductSpecialPriceViewModel SaveSpecialProductPrice(int productId, decimal price, int priceGroupId, DateTime? startDate = null, DateTime? endDate = null, int currentTenantId = 0, int userId = 0);

        ProductSpecialPriceViewModel GetSpecialProductPriceById(int specialProductPriceId);

        bool DeleteSpecialProductPriceById(int specialProductPriceId, int userId);

        List<ProductSpecialPriceViewModel> GetAllSpecialProductPrices(int tenantId, int priceGroupId = 0);

        TenantPriceGroups SavePriceGroup(int priceGroupId, string name, decimal percent, int tenantId, int currentUserId, bool ApplyDiscountOnTotal, bool ApplyDiscountOnSpecialPrice);

        bool DeleteProductGroupById(int priceGroupId, int userId);

        TenantPriceGroups GetTenantPriceGroupById(int priceGroupId);
        IQueryable<TenantPriceGroups> GetAllTenantPriceGroups(int tenantId, bool includeIsDeleted = false);
        IQueryable<TenantPriceGroupDetail> GetAllTenantPriceGroupDetails(int tenantId, bool includeIsDeleted = false);
        decimal? GetPurchasePrice(int productId, InvoiceDetail invoiceDetail = null);

    }
}