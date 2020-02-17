using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public class CommonDbServices : ICommonDbServices
    {
        private readonly IApplicationContext _currentDbContext;

        public CommonDbServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }
        public List<ProductLocationsDetailResponse> ProductsByLocationDetails(int locationId)
        {
            var data = (from prd in _currentDbContext.InventoryTransactions.Where(a => a.LocationId == locationId &&
                                                                                       a.ProductSerial == null &&
                                                                                       a.InventoryTransactionType
                                                                                           .InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder && a
                                                                                           .IsCurrentLocation && a.IsDeleted != true)
                        .GroupBy(g => g.ProductId)
                        select new ProductLocationsDetailResponse
                        {
                            Id = prd.FirstOrDefault().InventoryTransactionId,
                            Quantity = (int)prd.Sum(s => s.Quantity),
                            Serial = null,
                            ProductName = prd.FirstOrDefault().ProductMaster.Name,
                            ProductId = (int)prd.FirstOrDefault().ProductId,
                            IsSerializable = prd.FirstOrDefault().ProductMaster.Serialisable ? "YES" : "NO",


                        })
                .Union
                (from prd1 in _currentDbContext.InventoryTransactions.Where(
                        a => a.LocationId == locationId && a.ProductSerial != null && a.IsCurrentLocation &&
                             a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder)
                 select new ProductLocationsDetailResponse
                 {
                     Id = prd1.InventoryTransactionId,
                     Quantity = 1,
                     Serial = prd1.ProductSerial.SerialNo,
                     ProductName = prd1.ProductMaster.Name,
                     ProductId = (int)prd1.ProductId,
                     IsSerializable = prd1.ProductMaster.Serialisable ? "YES" : "NO"
                 });
            return data.ToList();
        }

        public ProductLocationsResponse LocationsByProductDetails(int productId, int warehouseId)
        {
            var product = _currentDbContext.ProductMaster.First(m => m.ProductId == productId && m.IsDeleted != true);
            var results = (from prd in product.InventoryTransactions.Where(a => a.ProductSerial == null && a.WarehouseId == warehouseId).GroupBy(g => g.LocationId).ToList()
                           select new ProductLocationsDetailResponse()
                           {
                               Id = prd.First().InventoryTransactionId,
                               Quantity = prd.Where(a =>
                                                  (a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder ||
                                                   a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.AdjustmentIn ||
                                                   a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferIn ||
                                                   a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Returns))
                                              .Select(s => s.Quantity).ToList().Sum() -
                                          prd.Where(a => (a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder ||
                                                          a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferOut ||
                                                          a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.AdjustmentOut ||
                                                          a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder ||
                                                          a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Proforma ||
                                                          a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Quotation ||
                                                          a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Loan))
                                              .Select(s => s.Quantity).ToList().Sum(),
                               Batches = (from m in prd where !string.IsNullOrEmpty(m.BatchNumber) select new ProductLocationBatchResponse() { Quantity = m.Quantity, BatchNumber = m.BatchNumber, ExpiryDate = (m.ExpiryDate.HasValue ? m.ExpiryDate.Value : DateTime.MaxValue), ExpiryDateString = m.ExpiryDate?.ToString("dd/MM/yyyy") ?? "", LocationId = m.LocationId ?? 0 }).ToList(),
                               LocationCode = (prd.FirstOrDefault() != null && prd.First().Location != null) ? prd.First().Location.LocationCode : "UNKNOWN",
                               Location = (prd.FirstOrDefault() != null && prd.First().Location != null) ? prd.First().Location.LocationName : "UNKNOWN",
                               ProductName = product.Name,
                               ProductId = product.ProductId,
                               IsSerializable = product.Serialisable ? "YES" : "NO",
                           }).Where(m => m.Quantity > 0).ToList();


            var result = new ProductLocationsResponse();

            result.ProductDetails = results.OrderBy(m => m.Batches.Where(b => b.ExpiryDate.HasValue).Min(e => e.ExpiryDate)).ToList();

            if (product.Serialisable)
            {
                result.Serialised = true;
            }
            else if (results.Any(m => m.Batches.Any()))
            {
                result.ContainsBatches = true;
            }
            if (product.RequiresExpiryDateOnReceipt == true)
            {
                result.ContainsExpiryDate = true;
            }

            return result;
        }

        public List<CommonLocationViewModel> ProductsByLocations(int tenant, int warehouse)
        {
            var data = (from locs in _currentDbContext.Locations
                    .Where(a => a.WarehouseId == warehouse && a.TenentId == tenant && a.IsDeleted != true)

                        select new CommonLocationViewModel
                        {
                            LocationCode = locs.LocationCode,
                            Decription = locs.Description,
                            LocationId = locs.LocationId,
                        });

            return data.ToList();
        }
        public OrderDetail SetDetails(OrderDetail model, int? accountId, string urlReferer, string processBy)
        {
            var packCount = 1;
            model.ProductMaster = _currentDbContext.ProductMaster.AsNoTracking().FirstOrDefault(x => x.ProductId == model.ProductId && x.IsDeleted != true);

            packCount = (processBy == "Case") ? model.ProductMaster.ProductsPerCase ?? 1
                : (processBy == "Pallet" ? (model.ProductMaster.CasesPerPallet ?? 1) : 1);

            model.Qty = model.Qty * packCount;

            if ((model.TaxID == null || model.TaxID == 0) && model.ProductMaster?.EnableTax == true)
            {
                model.TaxID = model.ProductMaster.TaxID;
            }

            if (model.TaxID.HasValue && model.TaxID > 0)
            {
                model.TaxName = _currentDbContext.GlobalTax.AsNoTracking().First(a => a.TaxID == model.TaxID);
            }

            if (model.WarrantyID.HasValue && model.WarrantyID > 0 && model.ProductMaster.EnableWarranty == true)
            {
                model.Warranty = _currentDbContext.TenantWarranty.AsNoTracking().FirstOrDefault(a => a.WarrantyID == model.WarrantyID);
            }
            if (urlReferer.Contains("PurchaseOrders") || urlReferer.Contains("SalesOrders") || urlReferer.Contains("WorksOrders") || urlReferer.Contains("DirectSales"))
            {

                if (model.Warranty != null)
                {
                    if (model.Warranty.IsPercent)
                    {
                        model.WarrantyAmount = Math.Round(model.Warranty.PercentageOfPrice * ((model.Price * model.Qty) / 100), 2);

                    }
                    else
                    {
                        model.WarrantyAmount = Math.Round(model.Warranty.FixedPrice * model.Qty, 2);

                    }
                }
                else if (model.WarrantyAmount > 0)
                {
                    model.WarrantyAmount = model.WarrantyAmount;
                }
                if (model.TaxID.HasValue && model.TaxID > 0)
                {
                    model.TaxAmount = Math.Round(((model.Price * model.Qty) / 100) * (model.TaxName != null ? model.TaxName.PercentageOfAmount : 0), 2);
                    if (accountId.HasValue)
                    {
                        var accounttax = _currentDbContext.Account.FirstOrDefault(u => u.AccountID == accountId && (u.TaxID == 4 || u.TaxID == 5));
                        if (accounttax != null)
                        {
                            model.TaxAmount = 0;
                        }
                    }
                }
                if (accountId.HasValue && accountId > 0)
                {
                    var account = _currentDbContext.Account.FirstOrDefault(u => u.AccountID == accountId);
                    if (account.GlobalTax?.PercentageOfAmount <= 0)
                    {
                        model.TaxID = account.TaxID;
                        model.TaxAmount = 0;
                        model.TaxName.PercentageOfAmount = 0;
                    }



                }
                model.TotalAmount = Math.Round((model.Price * model.Qty), 2) + model.TaxAmount + model.WarrantyAmount;
               


            }

            return model;
        }
        public int GetQuantityInLocation(InventoryTransaction transaction)
        {

            int quantity = (int)_currentDbContext.InventoryTransactions
                .Where(a => a.LocationId == transaction.LocationId &&
                            a.ProductId == transaction.ProductId && a.IsDeleted != true
                            && a.IsCurrentLocation && a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder).Sum(a => a.Quantity);
            return quantity;
        }

    }
}