using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public class InventoryStockMoveExtensions
    {

        public static bool AdjustStockMovementTransactions(StockMovementViewModel stockMovement)
        {
            bool status = false;
            var productService = DependencyResolver.Current.GetService<IProductServices>();
            var product = productService.GetProductMasterById(stockMovement.ProductId);

            if (stockMovement.PalletSerials != null && stockMovement.PalletSerials.Count > 0)
            {
                foreach (var item in stockMovement.PalletSerials)
                {

                    status = AdjustStockLocations(stockMovement.ProductId, stockMovement.FromLocation, stockMovement.ToLocation,
                        Inventory.GetUomQuantity(product.ProductId, (item.Cases * product.ProductsPerCase ?? 1)), stockMovement.WarehouseId, stockMovement.TenentId,
                        stockMovement.UserId, item.PalletSerialId, null, true);
                }
            }
            else if (stockMovement.SerialIds != null && stockMovement.SerialIds.Count > 0)
            {
                foreach (var item in stockMovement.SerialIds)
                {
                    status = AdjustStockLocations(stockMovement.ProductId, stockMovement.FromLocation, stockMovement.ToLocation,
                        1, stockMovement.WarehouseId, stockMovement.TenentId,
                        stockMovement.UserId, item, null, true);
                }
            }
            else
            {
                status = AdjustStockLocations(stockMovement.ProductId, stockMovement.FromLocation, stockMovement.ToLocation,
                    stockMovement.Qty, stockMovement.WarehouseId, stockMovement.TenentId,
                    stockMovement.UserId, null, null, true);
            }

            return status;
        }

        public static bool AdjustStockLocations(int productId, int fromLocation, int toLocation, decimal quantity, int warehouseId, int tenantId,
            int userId, int? palletId = null, int? serialId = null, bool isStockMovement = false)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var lookupService = DependencyResolver.Current.GetService<ILookupServices>();
            var productService = DependencyResolver.Current.GetService<IProductServices>();
            var product = productService.GetProductMasterById(productId);
            Guid? ProductMovementId = null;
            var status = false;

            if (quantity > 0)
            {
                var stockListfromLocation = context.InventoryTransactions.Where(u =>
                    u.ProductId == productId && (fromLocation == 0 || u.LocationId == fromLocation)
                                             && u.IsCurrentLocation == true && (serialId == null || u.SerialID == serialId)
                                             && (palletId == null || u.PalletTrackingId == palletId)
                                             && (u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder
                                                 || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns
                                                 || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn
                                                 || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn
                                                 || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.MovementIn)).ToList();

                status = stockListfromLocation.Count > 0 ? true : false;

                if (isStockMovement)
                {
                    ProductMovementId = lookupService.CreateStockMovement(userId, tenantId, warehouseId);
                }

                var quantityToMove = quantity;

                foreach (var item in stockListfromLocation)
                {

                    InventoryTransactionForStockMovement(item, userId, tenantId, warehouseId, ProductMovementId, CurrentLocationstatus: false);

                    if (item.Quantity >= quantityToMove)
                    {
                        quantityToMove = item.Quantity - quantityToMove;

                        if (quantityToMove > 0)
                        {
                            var location = Inventory.GetLocation(item.TenentId, warehouseId, userId, item.LocationId);
                            InventoryTransactionForStockMovement(item, userId, tenantId, warehouseId, ProductMovementId, location, quantityToMove);
                            Inventory.LocationStockRecalculate(location, warehouseId, tenantId, userId);
                        }

                        InventoryTransactionForStockMovement(item, userId, tenantId, warehouseId, ProductMovementId, toLocation, quantity);
                        Inventory.LocationStockRecalculate(toLocation, warehouseId, tenantId, userId);
                        status = true;
                        break;
                    }
                    else
                    {
                        quantityToMove = quantityToMove - item.Quantity;
                        InventoryTransactionForStockMovement(item, userId, tenantId, warehouseId, ProductMovementId, toLocation, item.Quantity);
                        Inventory.LocationStockRecalculate(toLocation, warehouseId, tenantId, userId);
                    }
                }
            }

            return status;
        }

        private static void InventoryTransactionForStockMovement(InventoryTransaction inventoryTransaction, int UserId,
            int Tenantid, int WarehouseId, Guid? StocktMovementId = null, int LocationIdTo = 0, decimal Qty = 0,
            bool CurrentLocationstatus = true)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            if (!CurrentLocationstatus)
            {
                var stockItem = context.InventoryTransactions.FirstOrDefault(u =>
                    u.InventoryTransactionId == inventoryTransaction.InventoryTransactionId);
                if (stockItem != null)
                {
                    stockItem.IsCurrentLocation = CurrentLocationstatus;
                    stockItem.UpdatedBy = UserId;
                    stockItem.TenentId = Tenantid;
                    stockItem.DateUpdated = DateTime.UtcNow;
                    stockItem.StockMovementId = StocktMovementId;
                    context.Entry(stockItem).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else
            {
                InventoryTransaction tans = new InventoryTransaction
                {
                    LocationId = LocationIdTo,
                    ProductId = inventoryTransaction.ProductId,
                    CreatedBy = UserId,
                    DateCreated = DateTime.UtcNow,
                    InventoryTransactionTypeId = InventoryTransactionTypeEnum.MovementIn,
                    Quantity = Qty,
                    TenentId = Tenantid,
                    WarehouseId = WarehouseId,
                    StockMovementId = StocktMovementId,
                    IsCurrentLocation = true,
                    LastQty = Inventory.CalculateLastQty(inventoryTransaction.ProductId, Tenantid,
                        inventoryTransaction.WarehouseId, Qty, InventoryTransactionTypeEnum.MovementIn,
                        (inventoryTransaction.DontMonitorStock ?? false))
                };

                context.InventoryTransactions.Add(tans);
                context.SaveChanges();
            }
        }

    }
}