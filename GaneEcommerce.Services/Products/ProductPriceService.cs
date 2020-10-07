using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
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

        public ProductSaleQueryResponse GetProductSalePriceById(int productId)
        {
            return GetProductMasterWithSpecialPrice(productId, 0);
        }

        public ProductSaleQueryResponse GetProductPriceThresholdByAccountId(int productId, int? accountId = null)
        {
            if (productId == 0)
            {
                var thresholdInfos = new ProductSaleQueryResponse() { MinimumThresholdPrice = 0, SellPrice = 0, LandingCost = 0, LandingCostWithMargin = 0, PriceGroupID = 0, PriceGroupPercent = 0, ProfitMargin = 0 };
                return thresholdInfos;
            }

            return GetProductMasterWithSpecialPrice(productId, accountId ?? 0);
        }

        public ProductSaleQueryResponse CanTheProductBeSoldAtPriceToAccount(int productId, int accountId, decimal sellingPrice)
        {
            var thresholdInfo = GetProductPriceThresholdByAccountId(productId, accountId);

            return GetProductMasterWithSpecialPrice(productId, accountId);
            //var product
            //var result = sellingPrice > minSellprice && (product.AllowZeroSale == true || sellingPrice > 0);

            //if (!result)
            //{
            //    thresholdInfo.Success = false;
            //    thresholdInfo.FailureMessage = (product.AllowZeroSale != true && sellingPrice <= 0) ? "Zero price sale not permitted for this product. Contact administrator." : "Selling Price cannot be less than the minimum threshold price.";
            //}

            //return LoadThresholdInfo(thresholdInfo, product);
        }

        public ProductSaleQueryResponse CanTheProductBeSoldAtPercentageDiscountToAccount(int productId, int accountId, decimal customDiscountPercent)
        {
            var thresholdInfo = GetProductPriceThresholdByAccountId(productId, accountId);

            var minSellprice = thresholdInfo.MinimumThresholdPrice;

            return GetProductMasterWithSpecialPrice(productId, accountId);

            //var sellingPrice = GetPercentageDiscountedPrice(product.SellPrice, customDiscountPercent);

            //var result = sellingPrice > minSellprice && (product.AllowZeroSale == true || sellingPrice > 0);
            //if (!result)
            //{
            //    thresholdInfo.Success = false;
            //    thresholdInfo.FailureMessage = (product.AllowZeroSale != true && sellingPrice <= 0) ? "Zero price sale not permitted for this product. Contact administrator." : "Selling Price cannot be less than the minimum threshold price.";
            //}
            //return LoadThresholdInfo(thresholdInfo, product);
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

        public decimal GetLastProductPriceForAccount(int productId, int accountid, InventoryTransactionTypeEnum transactionType)
        {
            if (transactionType == InventoryTransactionTypeEnum.PurchaseOrder)
            {
                var product = _productService.GetProductMasterById(productId);
                return ConvertBaseRates(accountid, (product.BuyPrice ?? 0));
            }

            if (transactionType == InventoryTransactionTypeEnum.SalesOrder)
            {
                return GetProductMasterWithSpecialPrice(productId, accountid).SellPrice;
            }
            return 0.0m;
        }

        public decimal GetLastSoldProductPriceForAccount(int productId, int accountid)
        {
            return GetLastProductPriceForAccount(productId, accountid, InventoryTransactionTypeEnum.SalesOrder);
        }

        public decimal GetLastPurchaseProductPriceForAccount(int productId, int accountId)
        {
            return GetLastProductPriceForAccount(productId, accountId, InventoryTransactionTypeEnum.PurchaseOrder);
        }

        public decimal GetTaxAmountProductPriceForAccount(int productId, int accountId)
        {
            return GetLastProductPriceForAccount(productId, accountId, InventoryTransactionTypeEnum.SalesOrder);
            //var product = GetProductMasterWithSpecialPrice(productId, accountId);
            //if (product.GlobalTax == null || product.GlobalTax.PercentageOfAmount == 0) return 0;

            //return (lastProductPrice / 100) * product.GlobalTax.PercentageOfAmount;
        }

        private static decimal GetPercentageDiscountedPrice(decimal? price, decimal customDiscountPercent)
        {
            if (!price.HasValue) return 0;

            if (customDiscountPercent == 0) return price.Value;

            return price.Value - ((price.Value / 100) * customDiscountPercent);
        }

        public decimal GetPercentageMarginPrice(decimal? price, decimal customMarginPercent)
        {
            if (!price.HasValue) return 0;

            if (customMarginPercent == 0) return price.Value;

            return price.Value + ((price.Value / 100) * customMarginPercent);
        }

        private ProductSaleQueryResponse GetProductMasterWithSpecialPrice(int productId, int? accountId)
        {
            var product = _productService.GetProductMasterById(productId);
            var priceGroupId = 0;
            var account = _accountServices.GetAccountsById(accountId ?? 0);
            if (account != null && account.PriceGroupID > 0)
            {
                priceGroupId = account.PriceGroupID;

                var specialPrice = _context.ProductSpecialPrices.FirstOrDefault(m => m.ProductID == productId && priceGroupId == m.PriceGroupID && (!m.StartDate.HasValue || m.StartDate < DateTime.UtcNow) && (!m.EndDate.HasValue || m.EndDate > DateTime.UtcNow));
                if (specialPrice != null && specialPrice.SpecialPrice > 0)
                {
                    if (account.TenantPriceGroups.ApplyDiscountOnSpecialPrice && account.TenantPriceGroups.Percent > 0)
                    {
                        product.SellPrice = Math.Round((specialPrice.SpecialPrice - ((specialPrice.SpecialPrice * account.TenantPriceGroups.Percent) / 100)), 2);
                    }
                    else
                    {
                        product.SellPrice = specialPrice.SpecialPrice;
                    }
                }
                else
                {
                    if (account.TenantPriceGroups.ApplyDiscountOnTotal && !account.TenantPriceGroups.ApplyDiscountOnTotal)
                        if (account.TenantPriceGroups.Percent > 0)
                        {
                            product.SellPrice = Math.Round(((product.SellPrice ?? 0) - (((product.SellPrice ?? 0) * account.TenantPriceGroups.Percent) / 100)), 2);
                        }
                }
            }
            decimal? SellPrice = ConvertBaseRates(accountId ?? 0, product.SellPrice ?? 0) > 0 ? ConvertBaseRates(accountId ?? 0, product.SellPrice ?? 0) : 0;
            var minSellPrice = ConvertBaseRates(accountId ?? 0, (GetPercentageMarginPrice(product.BuyPrice, product.PercentMargin) + (product.LandedCost ?? 0)));
            var minThresholdPrice = product.MinThresholdPrice ?? (minSellPrice <= 0 ? SellPrice : minSellPrice) ?? 0;
            var finalThresholdPrice = new[] { minThresholdPrice, minSellPrice }.Min();
            finalThresholdPrice = ConvertBaseRates(accountId ?? 0, finalThresholdPrice);
            var LandingCost = ConvertBaseRates(accountId ?? 0, product.LandedCost ?? 0);
            var thresholdInfo = new ProductSaleQueryResponse()
            {
                MinimumThresholdPrice = finalThresholdPrice,
                SellPrice = (SellPrice ?? 0),
                LandingCost = ConvertBaseRates(accountId ?? 0, product.LandedCost ?? 0),
                LandingCostWithMargin = minSellPrice,
                PriceGroupID = account?.PriceGroupID ?? 0,
                PriceGroupPercent = account?.TenantPriceGroups?.Percent ?? 0,
                ProfitMargin = product.PercentMargin,
                MinimumSellPrice = minSellPrice
            };
            return LoadThresholdInfo(thresholdInfo, product);
        }

        private decimal GetProductPriceByAccountId(int productId, int? accountId = null)
        {
            if (!accountId.HasValue || accountId == 0)
            {
                return GetProductSalePriceById(productId).SellPrice;
            }
            var product = GetProductMasterWithSpecialPrice(productId, accountId ?? 0);

            var account = _accountServices.GetAccountsById(accountId.Value);

            var sellPrice = GetPercentageDiscountedPrice(ConvertBaseRates(accountId ?? 0, product.SellPrice), account.TenantPriceGroups.Percent);

            return sellPrice;
        }

        private ProductSaleQueryResponse LoadThresholdInfo(ProductSaleQueryResponse thresholdInfo, ProductMaster product)
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

        private decimal ConvertBaseRates(int accountId, decimal price)
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

        public bool DeleteProductGroupById(int priceGroupId, int userId)
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

        public decimal? GetPurchasePrice(int productId, InvoiceDetail invoiceDetail = null)
        {
            var product = _context.ProductMaster.FirstOrDefault(u => u.ProductId == productId && u.IsDeleted != true);

            var targetDate = invoiceDetail?.DateCreated;
            var targetOrderId = invoiceDetail?.OrderDetail?.OrderID;

            var targetOrderDetailQuery = _context.OrderDetail.Where(u => (u.DateCreated < targetDate || targetDate == null) &&
                                                                    u.ProductId == productId &&
                                                                    u.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder &&
                                                                    u.Order.OrderStatusID == OrderStatusEnum.Complete &&
                                                                    u.Order.IsDeleted != true &&
                                                                    u.IsDeleted != true);

            OrderDetail targetOrderDetail = null;

            if (targetOrderDetailQuery.Any(u => u.Order.BaseOrderID == targetOrderId && targetOrderId != null))
            {
                targetOrderDetailQuery = targetOrderDetailQuery.Where(u => u.Order.BaseOrderID == targetOrderId && targetOrderId != null);
            }
            else
            {
                var palletTrackingId = _context.InventoryTransactions.FirstOrDefault(p => p.OrderID == targetOrderId && p.ProductId == productId)?.PalletTrackingId;

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
    }
}