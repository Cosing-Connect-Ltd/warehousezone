using System;
using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public class InventoryPalletExtensions
    {
        public static void AdjustPalletRemainingCases(int palletTrackingId, int WarehouseId, int TenantId, int UserId,
            bool saveContext = true, IApplicationContext context = null)
        {
            if (context == null)
            {
                context = DependencyResolver.Current.GetService<IApplicationContext>();
            }

            var pallet = context.PalletTracking.Find(palletTrackingId);

            int transactionsCount = 0;
            decimal inStock;

            var totals = (from e in context.InventoryTransactions
                where e.ProductId == pallet.ProductId && e.WarehouseId == WarehouseId &&
                      e.PalletTrackingId == palletTrackingId &&
                      e.TenentId == TenantId && e.DontMonitorStock != true
                select new InventoryCalculationViewModel
                {
                    Quantity = e.Quantity,
                    Type = e.InventoryTransactionTypeId,
                    Order = e.Order
                });

            transactionsCount = totals.Count();
            inStock = InventoryExtensions.GetTotalInStock(totals);
            var inStockCases = inStock / pallet.ProductMaster.ProductsPerCase ?? 1;

            pallet.DateUpdated = DateTime.UtcNow;

            if (transactionsCount == 0)
            {
                pallet.Status = PalletTrackingStatusEnum.Created;
                pallet.RemainingCases = pallet.TotalCases;
            }
            else if (transactionsCount > 0 && inStock == 0)
            {
                pallet.Status = PalletTrackingStatusEnum.Completed;
                pallet.RemainingCases = 0;
            }
            else
            {
                pallet.Status = PalletTrackingStatusEnum.Active;
                pallet.RemainingCases = inStockCases;
            }

            if (saveContext)
            {
                context.SaveChanges();
            }
        }

        public static void AdjustPalletRemainingCasesAll(int WarehouseId, int TenantId, int UserId)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var pallets = context.PalletTracking.AsNoTracking()
                .Where(a => a.TenantId == TenantId && a.WarehouseId == WarehouseId).Select(x => x.PalletTrackingId)
                .ToList();

            var i = 0;

            foreach (var pallet in pallets)
            {
                AdjustPalletRemainingCases(pallet, WarehouseId, TenantId, UserId, i % 200 == 0, context);
                i++;
            }

            context.SaveChanges();
        }

        public static decimal CalculatePalletQuantity(int palletTrackingId)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var totalIn = context.InventoryTransactions.Where(e =>
                e.PalletTrackingId == palletTrackingId && e.IsDeleted != true &&
                (e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder
                 || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns
                 || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn
                 || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn)
            ).Select(I => I.Quantity).DefaultIfEmpty(0).Sum();


            var totalout = context.InventoryTransactions.Where(e =>
                e.PalletTrackingId == palletTrackingId && e.IsDeleted != true &&
                (e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder
                 || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentOut
                 || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut
                 || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales
                 || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Samples
                 || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Wastage)
            ).Select(I => I.Quantity).DefaultIfEmpty(0).Sum();

            return (totalIn - totalout);

        }

    }
}