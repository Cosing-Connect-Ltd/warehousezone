using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{
    /// <summary>
    /// TODO:GANE Any product with no sell price or threshold will return 0. Be advised.
    /// </summary>
    public class ProductPriceService : IProductPriceService
    {
        private readonly IProductServices _productService;
        private readonly IAccountServices _accountServices;
        private readonly IApplicationContext _context;
        private readonly IMapper _mapper;

        public ProductPriceService(IProductServices productService, IAccountServices accountServices, IApplicationContext context, IMapper mapper)
        {
            _productService = productService;
            _accountServices = accountServices;
            _context = context;
            _mapper = mapper;
        }

        public ProductSaleQueryResponse GetProductPriceThresholdByAccountId(int productId, int? accountId = null, int siteId = 0)
        {
            if (productId == 0)
            {
                var thresholdInfos = new ProductSaleQueryResponse() { MinimumThresholdPrice = 0, SellPrice = 0, LandingCost = 0, LandingCostWithMargin = 0, PriceGroupID = 0, PriceGroupPercent = 0, ProfitMargin = 0 };
                return thresholdInfos;
            }

            return GetProductMasterWithSpecialPrice(productId, accountId ?? 0, siteId);
        }

        public List<ProductPriceHistoryModel> GetProductPriceHistoryForAccount(int productId, int accountid)
        {
            return (from odets in _context.OrderDetail.Where(a => a.ProductId == productId && a.IsDeleted != true && a.Order.AccountID == accountid).OrderByDescending(m => m.DateCreated).ToList()
                    select new ProductPriceHistoryModel()
                    {
                        Id = odets.OrderDetailID,
                        Price = odets.Price,
                        Product = odets.ProductMaster.Name,
                        Timestamp = odets.DateCreated.ToString("dd/MM/yyyy HH:mm"),
                        CurrencySymbol = odets.Order.Account.GlobalCurrency.Symbol,
                        PriceWithDate = odets.DateCreated.ToString("dd/MM/yyyy HH:mm - ") + odets.Order.Account.GlobalCurrency.Symbol + odets.Price.ToString("0.00"),
                        TypeIdentifier = (int)odets.Order.InventoryTransactionTypeId
                    }).Take(5).ToList();
        }

        public decimal GetProductLastPurchasePriceForAccount(int productId, int accountId)
        {
            var product = _productService.GetProductMasterById(productId);
            return ConvertPriceBaseRates(accountId, (product.BuyPrice ?? 0));
        }

        public decimal GetProductLastSellPriceForAccount(int productId, int accountId)
        {
            return GetProductMasterWithSpecialPrice(productId, accountId).SellPrice;
        }

        public decimal GetPercentageMarginPrice(decimal? price, decimal customMarginPercent)
        {
            if (!price.HasValue) return 0;

            if (customMarginPercent == 0) return price.Value;

            return price.Value + ((price.Value / 100) * customMarginPercent);
        }

        private ProductSaleQueryResponse GetProductMasterWithSpecialPrice(int productId, int? accountId, int siteId = 0)
        {
            var product = _productService.GetProductMasterById(productId);
            var accountPriceGroup = _accountServices.GetAccountsById(accountId ?? 0)?.TenantPriceGroups;
            var siteDefaultPriceGroup = _context.TenantWebsites.FirstOrDefault(u => u.IsDeleted != true && u.SiteID == siteId && u.IsActive == true)?.DefaultPriceGroup;

            if (accountPriceGroup != null)
            {
                var specialPrice = _context.ProductSpecialPrices.FirstOrDefault(m => m.ProductID == productId && accountPriceGroup.PriceGroupID == m.PriceGroupID && (!m.StartDate.HasValue || m.StartDate < DateTime.UtcNow) && (!m.EndDate.HasValue || m.EndDate > DateTime.UtcNow));
                if (specialPrice != null && specialPrice?.SpecialPrice > 0)
                {
                    if (siteDefaultPriceGroup != null && siteDefaultPriceGroup.ApplyDiscountOnSpecialPrice && siteDefaultPriceGroup.Percent > 0)
                    {
                        specialPrice.SpecialPrice = Math.Round(specialPrice.SpecialPrice - ((specialPrice.SpecialPrice * siteDefaultPriceGroup.Percent) / 100), 2);
                    }
                    else if (accountPriceGroup != null && accountPriceGroup.ApplyDiscountOnSpecialPrice && accountPriceGroup.Percent > 0)
                    {
                        specialPrice.SpecialPrice = Math.Round(specialPrice.SpecialPrice - ((specialPrice.SpecialPrice * accountPriceGroup.Percent) / 100), 2);
                    }

                    product.SellPrice = specialPrice.SpecialPrice;
                }
                else
                {
                    if (siteDefaultPriceGroup != null && siteDefaultPriceGroup.ApplyDiscountOnTotal && siteDefaultPriceGroup.Percent > 0)
                    {
                        product.SellPrice = Math.Round((product.SellPrice ?? 0) - (((product.SellPrice ?? 0) * siteDefaultPriceGroup.Percent) / 100), 2);
                    }
                    else if (accountPriceGroup != null && accountPriceGroup.ApplyDiscountOnTotal && accountPriceGroup.Percent > 0)
                    {
                        product.SellPrice = Math.Round((product.SellPrice ?? 0) - (((product.SellPrice ?? 0) * accountPriceGroup.Percent) / 100), 2);
                    }
                }
            }
            decimal? SellPrice = ConvertPriceBaseRates(accountId ?? 0, product.SellPrice ?? 0) > 0 ? ConvertPriceBaseRates(accountId ?? 0, product.SellPrice ?? 0) : 0;
            var minSellPrice = ConvertPriceBaseRates(accountId ?? 0, (GetPercentageMarginPrice(product.BuyPrice, product.PercentMargin) + (product.LandedCost ?? 0)));
            var minThresholdPrice = product.MinThresholdPrice ?? (minSellPrice <= 0 ? SellPrice : minSellPrice) ?? 0;
            var finalThresholdPrice = new[] { minThresholdPrice, minSellPrice }.Min();
            finalThresholdPrice = ConvertPriceBaseRates(accountId ?? 0, finalThresholdPrice);
            var LandingCost = ConvertPriceBaseRates(accountId ?? 0, product.LandedCost ?? 0);
            var thresholdInfo = new ProductSaleQueryResponse()
            {
                MinimumThresholdPrice = finalThresholdPrice,
                SellPrice = (SellPrice ?? 0),
                LandingCost = ConvertPriceBaseRates(accountId ?? 0, product.LandedCost ?? 0),
                LandingCostWithMargin = minSellPrice,
                PriceGroupID = accountPriceGroup?.PriceGroupID ?? siteDefaultPriceGroup?.PriceGroupID ?? 0,
                PriceGroupPercent = accountPriceGroup?.Percent ?? siteDefaultPriceGroup?.Percent ?? 0,
                ProfitMargin = product.PercentMargin,
                MinimumSellPrice = minSellPrice
            };

            return SetThresholdInfo(thresholdInfo, product);
        }

        private ProductSaleQueryResponse SetThresholdInfo(ProductSaleQueryResponse thresholdInfo, ProductMaster product)
        {
            var tenantConfig = _context.TenantConfigs.FirstOrDefault(m => m.TenantId == product.TenantId);
            if (tenantConfig != null)
            {
                thresholdInfo.ShowWarning = tenantConfig.AlertMinimumProductPrice;
                thresholdInfo.CanProceed = !tenantConfig.EnforceMinimumProductPrice;
                thresholdInfo.FailureMessage = tenantConfig.AlertMinimumPriceMessage;
                thresholdInfo.StopMessage = tenantConfig.EnforceMinimumPriceMessage;
            }
            return thresholdInfo;
        }

        private decimal ConvertPriceBaseRates(int accountId, decimal price)
        {
            if (accountId == 0) return price;
            var account = _context.Account.FirstOrDefault(a => a.AccountID == accountId);
            var accountCurrencyId = account.CurrencyID;
            var tenantCurrencyId = _context.Tenants.FirstOrDefault(m => m.TenantId == account.TenantId).CurrencyID;
            if (accountCurrencyId == tenantCurrencyId)
            {
                return price;
            }

            var rate = 1m;

            var currency = _context.TenantCurrenciesExRates.Where(m => m.TenantCurrencies.CurrencyID == accountCurrencyId).OrderByDescending(x => x.ExchnageRateID).FirstOrDefault();

            if (currency != null)
            {
                rate = currency.Rate;
            }

            if (rate <= 0)
            {
                return price;
            }
            else
            {
                return rate * price;
            }
        }

        public ProductSpecialPriceViewModel SaveSpecialProductPrice(int productId, decimal price, int priceGroupId, DateTime? startDate = null, DateTime? endDate = null, int currentTenantId = 0, int userId = 0)
        {
            TenantPriceGroupDetail productPrice = _context.ProductSpecialPrices.FirstOrDefault(x => x.ProductID == productId && x.PriceGroupID == priceGroupId);

            if (productPrice == null)
            {
                productPrice = new TenantPriceGroupDetail()
                {
                    PriceGroupID = priceGroupId,
                    StartDate = startDate,
                    EndDate = endDate,
                    ProductID = productId,
                    SpecialPrice = price,
                    TenantId = currentTenantId
                };
                productPrice.UpdateCreatedInfo(userId);
                _context.ProductSpecialPrices.Add(productPrice);
            }
            else
            {
                productPrice.SpecialPrice = price;
                productPrice.StartDate = startDate;
                productPrice.EndDate = endDate;
                productPrice.ProductID = productId;
                productPrice.PriceGroupID = priceGroupId;
                productPrice.UpdateUpdatedInfo(userId);
                _context.Entry(productPrice).State = EntityState.Modified;
            }
            _context.SaveChanges();
            return _mapper.Map(productPrice, new ProductSpecialPriceViewModel());
        }

        public ProductSpecialPriceViewModel GetSpecialProductPriceById(int specialProductPriceId)
        {
            var specialPrice = _context.ProductSpecialPrices.FirstOrDefault(m => m.PriceGroupDetailID == specialProductPriceId);

            return specialPrice == null ? null : _mapper.Map(specialPrice, new ProductSpecialPriceViewModel());
        }

        public bool DeleteSpecialProductPriceById(int specialProductPriceId, int userId)
        {
            var specialPrice = _context.ProductSpecialPrices.Find(specialProductPriceId);
            if (specialPrice != null)
            {
                specialPrice.IsDeleted = true;
                specialPrice.UpdateUpdatedInfo(userId);
                _context.Entry(specialPrice).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<ProductSpecialPriceViewModel> GetAllSpecialProductPrices(int tenantId, int priceGroupId)
        {
            var priceGroup = _context.TenantPriceGroups.Find(priceGroupId);
            var allProducts = _context.ProductMaster.Where(m => m.TenantId == tenantId && m.IsDeleted != true);
            var allSpecialPrices = _context.ProductSpecialPrices.Where(m => m.PriceGroupID == priceGroupId);

            var levels =
                (from p in allProducts
                 join a in allSpecialPrices on p.ProductId equals a.ProductID into tmpGroups
                 from d in tmpGroups.DefaultIfEmpty()
                 select new ProductSpecialPriceViewModel()
                 {
                     ProductName = p.Name,
                     DateCreated = d.DateCreated,
                     PriceGroupID = d.PriceGroupID,
                     StartDate = d.StartDate,
                     EndDate = d.EndDate,
                     PriceGroupName = d.PriceGroup.Name,
                     ProductID = p.ProductId,
                     PriceGroupDetailID = d.PriceGroupDetailID,
                     SpecialPrice = d.SpecialPrice,
                     SkuCode = p.SKUCode
                 });

            return levels.ToList();
        }

        public TenantPriceGroups SavePriceGroup(int priceGroupId, string name, decimal percent, int tenantId, int currentUserId, bool ApplyDiscountOnTotal, bool ApplyDiscountOnSpecialPrice)
        {
            var pg = new TenantPriceGroups();
            if (priceGroupId > 0)
            {
                pg = _context.TenantPriceGroups.FirstOrDefault(m => m.PriceGroupID == priceGroupId);
                pg.Name = name;
                pg.Percent = percent;
                pg.TenantId = tenantId;
                pg.UpdatedBy = currentUserId;
                pg.ApplyDiscountOnSpecialPrice = ApplyDiscountOnSpecialPrice;
                pg.ApplyDiscountOnTotal = ApplyDiscountOnTotal;
                pg.DateUpdated = DateTime.UtcNow;
                _context.Entry(pg).State = EntityState.Modified;
            }
            else
            {
                var priceGroup = _context.TenantPriceGroups.FirstOrDefault(m => m.Name.Equals(name) && m.IsDeleted != true);

                if (priceGroup != null) return null;

                pg = new TenantPriceGroups()
                {
                    CreatedBy = currentUserId,
                    DateCreated = DateTime.UtcNow,
                    Name = name,
                    Percent = percent,
                    TenantId = tenantId,
                    ApplyDiscountOnTotal = ApplyDiscountOnTotal,
                    ApplyDiscountOnSpecialPrice = ApplyDiscountOnSpecialPrice
                };
                _context.Entry(pg).State = EntityState.Added;
            }
            _context.SaveChanges();
            return pg;
        }

        public bool DeletePriceGroupById(int priceGroupId, int userId)
        {
            var priceGroup = _context.TenantPriceGroups.FirstOrDefault(m => m.PriceGroupID == priceGroupId);
            if (priceGroup != null)
            {
                if (_context.Account.Any(m => m.PriceGroupID == priceGroupId))
                {
                    return false;
                }
                priceGroup.IsDeleted = true;
                priceGroup.UpdateUpdatedInfo(userId);
                _context.Entry(priceGroup).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public TenantPriceGroups GetTenantPriceGroupById(int priceGroupId)
        {
            return _context.TenantPriceGroups.FirstOrDefault(x => x.PriceGroupID == priceGroupId);
        }

        public IQueryable<TenantPriceGroups> GetAllTenantPriceGroups(int tenantId, bool includeIsDeleted = false)
        {
            return _context.TenantPriceGroups.Where(x => x.TenantId == tenantId && (includeIsDeleted || x.IsDeleted != true));
        }

        public IQueryable<TenantPriceGroupDetail> GetAllTenantPriceGroupDetails(int tenantId, bool includeIsDeleted = false)
        {
            return _context.ProductSpecialPrices.Where(x => x.TenantId == tenantId && (includeIsDeleted || x.IsDeleted != true));
        }

        public decimal? GetPurchasePrice(int productId, DateTime? date = null, int? orderId = null)
        {
            var product = _context.ProductMaster.FirstOrDefault(u => u.ProductId == productId && u.IsDeleted != true);

            var targetOrderDetailQuery = _context.OrderDetail.Where(u => (u.DateCreated < date || date == null) &&
                                                                    u.ProductId == productId &&
                                                                    u.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder &&
                                                                    (u.Order.OrderStatusID == OrderStatusEnum.Complete ||
                                                                      u.Order.OrderStatusID == OrderStatusEnum.PostedToAccounts ||
                                                                      u.Order.OrderStatusID == OrderStatusEnum.Invoiced) &&
                                                                    u.Order.IsDeleted != true &&
                                                                    u.IsDeleted != true);

            OrderDetail targetOrderDetail = null;

            if (targetOrderDetailQuery.Any(u => u.Order.BaseOrderID == orderId && orderId != null))
            {
                targetOrderDetailQuery = targetOrderDetailQuery.Where(u => u.Order.BaseOrderID == orderId && orderId != null);
            }
            else
            {
                var palletTrackingId = _context.InventoryTransactions.FirstOrDefault(p => p.OrderID == orderId && p.ProductId == productId)?.PalletTrackingId;

                if (palletTrackingId != null)
                {
                    var relatedPurchaseOrder = _context.InventoryTransactions.FirstOrDefault(p => p.PalletTrackingId == palletTrackingId &&
                                                                                                  p.ProductId == productId &&
                                                                                                  p.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder)?.Order;

                    if (relatedPurchaseOrder != null)
                    {
                        targetOrderDetail = relatedPurchaseOrder.OrderDetails
                                                                .Where(u => u.ProductId == productId && u.IsDeleted != true)
                                                                .OrderByDescending(u => u.OrderDetailID)
                                                                .FirstOrDefault();
                    }
                }
            }

            if (targetOrderDetail == null)
            {
                targetOrderDetail = targetOrderDetailQuery
                                        .OrderByDescending(u => u.OrderDetailID)
                                        .FirstOrDefault();
            }

            var rebatePercentage = targetOrderDetail != null ? (_context.ProductAccountCodes.FirstOrDefault(u => u.AccountID == targetOrderDetail.Order.AccountID &&
                                                                                    (u.RebatePercentage ?? 0) > 0 &&
                                                                                    (u.ProductId ?? 0) == productId &&
                                                                                    u.IsDeleted != true)?.RebatePercentage ?? 0) : 0;

            var buyPrice = targetOrderDetail?.Price ?? product?.BuyPrice;

            if (buyPrice == null)
            {
                return null;
            }

            buyPrice -= Math.Round((buyPrice.Value / 100) * (rebatePercentage), 2);

            buyPrice += (product.LandedCost ?? 0);

            return buyPrice;
        }

        public List<InvoiceProductPriceModel> GetInvoiceDetailsProductPrices(List<InvoiceDetail> invoiceDetails)
        {
            var invoiceProductsPrices = new List<InvoiceProductPriceModel>();
            var productIds = invoiceDetails.Select(i => i.ProductId).ToList();
            var products = _context.ProductMaster.Where(u => productIds.Contains(u.ProductId) && u.IsDeleted != true).Select(p => new { p.BuyPrice, p.LandedCost }).ToList();

            var targetOrderDetails = _context.OrderDetail.Where(u => productIds.Contains(u.ProductId) &&
                                                                     u.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder &&
                                                                     (u.Order.OrderStatusID == OrderStatusEnum.Complete ||
                                                                      u.Order.OrderStatusID == OrderStatusEnum.PostedToAccounts ||
                                                                      u.Order.OrderStatusID == OrderStatusEnum.Invoiced) &&
                                                                     u.Order.IsDeleted != true &&
                                                                     u.IsDeleted != true)
                                                         .ToList();

            var productsPrices = targetOrderDetails.Where(u => invoiceDetails.Any(i => i.ProductId == u.ProductId && i.OrderDetail.OrderID == u.Order?.BaseOrder?.OrderID && u.DateCreated <= i.DateCreated))
                                                   .Select(a => new InvoiceProductPrice { ProductId = a.ProductId, Price = a.Price, OrderId = a.Order?.BaseOrder?.OrderID })
                                                   .ToList();

            var remainingInvoices = invoiceDetails.Where(i => !productsPrices.Any(p => p.ProductId == i.ProductId && p.OrderId == i.OrderDetail.OrderID));

            var tempProductIds = remainingInvoices.Select(r => r.ProductId).ToList();
            var tempOrderIds = remainingInvoices.Select(r => r.OrderDetail.OrderID).ToList();

            var inventoryTransactions = _context.InventoryTransactions.Where(p => tempProductIds.Contains(p.ProductId) &&
                                                                                  tempOrderIds.Contains(p.OrderID ?? 0) &&
                                                                                  p.IsDeleted != true)
                                                                      .ToList()?
                                                                      .Where(p => remainingInvoices.Any(i => i.ProductId == p.ProductId && p.OrderID == i.OrderDetail.OrderID))
                                                                      .ToList();

            tempProductIds = inventoryTransactions.Select(r => r.ProductId).ToList();
            var tempPalletTrackingIds = inventoryTransactions.Select(r => r.PalletTrackingId).ToList();

            var relatedPurchaseOrdersByPallet = _context.InventoryTransactions.Where(p => tempProductIds.Contains(p.ProductId) &&
                                                                                          tempPalletTrackingIds.Contains(p.PalletTrackingId) &&
                                                                                          p.IsDeleted != true &&
                                                                                          p.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder)
                                                                              .ToList()?
                                                                              .Where(p => inventoryTransactions.Any(i => p.PalletTrackingId == i.PalletTrackingId && p.ProductId == i.ProductId))
                                                                              .ToList();

            inventoryTransactions.Where(i => relatedPurchaseOrdersByPallet.Any(p => p.PalletTrackingId == i.PalletTrackingId && p.ProductId == i.ProductId))
                                 .ToList()
                                 .ForEach(i => {
                                     productsPrices.Add(new InvoiceProductPrice
                                     {
                                         ProductId = i.ProductId,
                                         OrderId = i.OrderID,
                                         Price = relatedPurchaseOrdersByPallet.First(p => p.PalletTrackingId == i.PalletTrackingId && p.ProductId == i.ProductId)
                                                                                                           .Order.OrderDetails
                                                                                                           .Where(u => u.ProductId == i.ProductId && u.IsDeleted != true)
                                                                                                           .OrderByDescending(u => u.OrderDetailID)
                                                                                                           .FirstOrDefault().Price
                                     });
                                 });

            remainingInvoices = invoiceDetails.Where(i => !productsPrices.Any(p => p.ProductId == i.ProductId && p.OrderId == i.OrderDetail.OrderID));


            productsPrices.AddRange(targetOrderDetails.Where(u => remainingInvoices.Any(i => i.ProductId == u.ProductId && u.DateCreated <= i.DateCreated))
                                                      .Select(a => new InvoiceProductPrice { ProductId = a.ProductId, Price = a.Price, OrderId = a.Order?.BaseOrder?.OrderID })
                                                      .ToList());

            remainingInvoices = invoiceDetails.Where(i => !productsPrices.Any(p => p.ProductId == i.ProductId));

            productsPrices.AddRange(remainingInvoices.Select(a => new InvoiceProductPrice { ProductId = a.ProductId, Price = a.Product.BuyPrice, OrderId = a.OrderDetail?.OrderID }));

            productsPrices = productsPrices.OrderByDescending(p => p.OrderId).ToList();

            return invoiceDetails.Select(i => {
                var buyPrice = productsPrices.FirstOrDefault(p => p.ProductId == i.ProductId && p.OrderId == i.OrderDetail.OrderID)?.Price ??
                               productsPrices.FirstOrDefault(p => p.ProductId == i.ProductId)?.Price ?? 0;
                return new InvoiceProductPriceModel
                {
                    InvoiceId = i.InvoiceMasterId,
                    ProductId = i.ProductId,
                    ProductName = i.Product.NameWithCode,
                    Quantity = i.Quantity,
                    BuyPrice = buyPrice,
                    SellPrice = i.Price,
                    TotalBuyPrice = buyPrice * i.Quantity,
                    TotalSellPrice = i.Price * i.Quantity

                };
            }).ToList();
        }

        private class InvoiceProductPrice
        {
            public int ProductId { get; set; }
            public int? OrderId { get; set; }
            public decimal? Price { get; set; }
        }
    }
}