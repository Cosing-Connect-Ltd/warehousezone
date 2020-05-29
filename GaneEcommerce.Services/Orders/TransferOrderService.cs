using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;

namespace Ganedata.Core.Services
{

    public class TransferOrderService : ITransferOrderService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IOrderService _orderService;
        private readonly IVanSalesService _vanSalesService;

        public TransferOrderService(IApplicationContext currentDbContext, IOrderService orderService, IVanSalesService vanSalesService)
        {
            _currentDbContext = currentDbContext;
            _orderService = orderService;
            _vanSalesService = vanSalesService;
        }

        public List<ProductMarketReplenishModel> AutoTransferOrdersForMobileLocations(int tenantId)
        {
            var allMobileLocations = _currentDbContext.TenantWarehouses.Where(m => m.TenantId == tenantId && m.IsDeleted != true && m.IsMobile == true).ToList();

            var results = new List<ProductMarketReplenishModel>();

            foreach (var mLocation in allMobileLocations)
            {
                var mMarketSchedules = _currentDbContext.MarketRouteSchedules.Where(m => m.StartTime <= DateTime.UtcNow && DateTime.UtcNow <= m.EndTime && m.WarehouseId == mLocation.WarehouseId);

                var mRouteMarkets = mMarketSchedules.Select(x => x.MarketRoute).SelectMany(m => m.MarketRouteMap).Select(x => x.Market);

                var mMarketProductToStocks = _currentDbContext.ProductMarketStockLevel.Where(x => mRouteMarkets.Any(m => m.Id == x.MarketId));

                var distinctProductStocks = mMarketProductToStocks.GroupBy(x => x.ProductMasterID).Select(p => new ProductMarketReplenishModel() { ProductId = p.Key, RequiredQuantity = p.Sum(x => x.MinStockQuantity) });

                var finalVanStocksRequired = (from m in distinctProductStocks
                                              join i in _currentDbContext.InventoryStocks on m.ProductId equals i.ProductId
                                              where i.WarehouseId == mLocation.ParentWarehouseId && i.InStock > m.RequiredQuantity && m.RequiredQuantity > 0
                                              select new ProductMarketReplenishModel() { ProductId = m.ProductId, RequiredQuantity = m.RequiredQuantity }).ToList();
                if (finalVanStocksRequired.Any())
                {
                    var orderDetails = (finalVanStocksRequired.Select(o => new OrderDetail
                    {
                        ProductId = o.ProductId,
                        TenentId = tenantId,
                        DateCreated = DateTime.UtcNow,
                        CreatedBy = 0,
                        Qty = o.RequiredQuantity
                    })).ToList();
                    var tInOrder = new Order() { TransferWarehouseId = mLocation.ParentWarehouseId, WarehouseId = mLocation.WarehouseId, AuthorisedUserID = 0, DateCreated = DateTime.UtcNow, CreatedBy = 0, InventoryTransactionTypeId = InventoryTransactionTypeEnum.TransferIn, OrderNumber = _orderService.GenerateNextOrderNumber(InventoryTransactionTypeEnum.TransferIn, tenantId) };
                    CreateTransferOrder(tInOrder, orderDetails, tenantId, mLocation.WarehouseId, 0);
                }
                results.AddRange(finalVanStocksRequired);
            }

            return results;
        }


        public Order CreateTransferOrder(Order Order, List<OrderDetail> orderDetails, int tenantId, int warehouseId, int userId)
        {
            var t_Type = Order.InventoryTransactionTypeId;
            if (t_Type == InventoryTransactionTypeEnum.TransferIn)
            {
                Order.IsShippedToTenantMainLocation = true;
            }

            Order.OrderNumber = Order.OrderNumber.Trim();
            var duplicateOrder = _currentDbContext.Order.FirstOrDefault(m => m.OrderNumber.Equals(Order.OrderNumber, StringComparison.CurrentCultureIgnoreCase));
            if (duplicateOrder != null)
            {
                throw new Exception($"Order Number {Order.OrderNumber} already associated with another Order. Please regenerate order number.", new Exception("Duplicate Order Number"));
            }
            Order.IssueDate = DateTime.UtcNow;
            Order.DateCreated = DateTime.UtcNow;
            Order.DateUpdated = DateTime.UtcNow;
            Order.TenentId = tenantId;
            Order.CreatedBy = userId;
            Order.UpdatedBy = userId;
            
            Order.WarehouseId = warehouseId;
            if (!caCurrent.CurrentWarehouse().AutoAllowProcess) { Order.OrderStatusID = _currentDbContext.OrderStatus.AsNoTracking().FirstOrDefault(a => a.OrderStatusID == (int)OrderStatusEnum.Hold).OrderStatusID; }
            else { Order.OrderStatusID = _currentDbContext.OrderStatus.AsNoTracking().FirstOrDefault(a => a.OrderStatusID == (int)OrderStatusEnum.Active).OrderStatusID; }

            List<OrderDetail> ToDetails = null;
            if (orderDetails != null)
            {
                decimal? ordTotal = 0;
                ToDetails = new List<OrderDetail>();
                foreach (var item in orderDetails)
                {
                    int? taxId = item.TaxID;
                    int? warrantyId = item.WarrantyID;
                    int productId = item.ProductId;

                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.OrderID = Order.OrderID;
                    item.TenentId = tenantId;
                    item.WarehouseId = warehouseId;
                    item.SortOrder = item.ProductMaster?.ProductGroup?.SortOrder ?? 0;
                    ordTotal = ordTotal + ((item.Price * item.Qty) + item.TaxAmount);
                    // fix navigation issue
                    item.ProductMaster = null;
                    item.TaxName = null;
                    item.Warranty = null;

                    var newItem = new OrderDetail();

                    newItem.DateCreated = DateTime.UtcNow;
                    newItem.CreatedBy = userId;
                    newItem.WarehouseId = warehouseId;
                    newItem.WarrantyID = warrantyId;
                    newItem.TaxID = taxId;
                    newItem.ProductId = productId;
                    newItem.Qty = item.Qty;
                    newItem.SortOrder = item.SortOrder;
                    ToDetails.Add(newItem);

                    item.ProductId = productId;
                    item.TaxID = taxId;
                    item.WarrantyID = warrantyId;

                    Order.OrderDetails.Add(item);
                }

                Order.OrderTotal = (decimal)ordTotal;

            }
            Order.OrderGroupToken = Guid.NewGuid();
            _currentDbContext.Order.Add(Order);
            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(Order.OrderID, Order.WarehouseId??0, tenantId, caCurrent.CurrentUser().UserId);

            // create an alternate order
            InventoryTransactionTypeEnum InventoryTransactionTypeId = 0;
            bool shipmentTobool = false;

            if (t_Type == InventoryTransactionTypeEnum.TransferIn)
            {
                InventoryTransactionTypeId = InventoryTransactionTypeEnum.TransferOut;
            }

            else if (t_Type == InventoryTransactionTypeEnum.TransferOut)
            {
                InventoryTransactionTypeId = InventoryTransactionTypeEnum.TransferIn;
                shipmentTobool = true;
            }
            
            var inOrder = new Order
            {
                IssueDate = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                TenentId = tenantId,
                CreatedBy = userId,
                UpdatedBy = userId,
                OrderNumber = _orderService.GenerateNextOrderNumber(InventoryTransactionTypeEnum.TransferIn, tenantId),
                OrderStatusID = _currentDbContext.OrderStatus.First(a => a.OrderStatusID == (int)OrderStatusEnum.Active).OrderStatusID,
            TransferWarehouseId = Order.WarehouseId,
                Note = Order.Note,
                OrderGroupToken = Order.OrderGroupToken,
                OrderTotal = 0,
                IsShippedToTenantMainLocation=shipmentTobool,
                ExpectedDate = Order.ExpectedDate,
                InventoryTransactionTypeId = InventoryTransactionTypeId,
                WarehouseId = Order.TransferWarehouseId
            };
        
            foreach (var item in ToDetails)
            {
                int? taxId = item.TaxID;
                int? warrantyId = item.WarrantyID;
                int productId = item.ProductId;
                item.DateCreated = DateTime.UtcNow;
                item.CreatedBy = userId;
                item.TenentId = tenantId;
                item.WarehouseId = Order.TransferWarehouseId ?? warehouseId;
                // fix navigation issue
                item.ProductMaster = null;
                item.TaxName = null;
                item.Warranty = null;

                item.ProductId = productId;
                item.TaxID = taxId;
                item.WarrantyID = warrantyId;

                inOrder.OrderDetails.Add(item);
            }
            _currentDbContext.Order.Add(inOrder);
            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(inOrder.OrderID, inOrder.WarehouseId??0, tenantId, caCurrent.CurrentUser().UserId);

            return Order;
        }

        public Order SaveTransferOrder(Order Order, List<OrderDetail> orderDetails, int tenantId, int warehouseId, int userId)
        {
            Order.OrderNumber = Order.OrderNumber.Trim();
            var group = _currentDbContext.Order.FirstOrDefault(u => u.OrderNumber == Order.OrderNumber)?.OrderGroupToken;
            var Trnsferout = _currentDbContext.Order.FirstOrDefault(u => u.OrderNumber != Order.OrderNumber && u.OrderGroupToken == group);
            Order.DateUpdated = DateTime.UtcNow;
            Trnsferout.DateUpdated = DateTime.UtcNow;
            Order.UpdatedBy = userId;
            Trnsferout.UpdatedBy = userId;
            decimal total = 0;

            Order.WarehouseId = warehouseId;
            Trnsferout.WarehouseId = warehouseId;
            if (orderDetails != null)
            {
                var toAdd = orderDetails.Where(a => a.OrderDetailID < 0).ToList();
                var cItems = orderDetails.Where(a => a.OrderDetailID > 0).ToList();
                foreach (var item in toAdd)
                {
                    item.ProductMaster = null;
                    item.TaxName = null;
                    item.Warranty = null;
                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.TenentId = tenantId;
                    item.OrderID = Order.OrderID;
                    item.TenentId = tenantId;
                    item.WarehouseId = warehouseId;
                    total = total + item.TotalAmount;
                    _currentDbContext.OrderDetail.Add(item);
                }
                foreach (var item in toAdd)
                {
                    item.ProductMaster = null;
                    item.TaxName = null;
                    item.Warranty = null;
                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.TenentId = tenantId;
                    item.OrderID = Trnsferout.OrderID;
                    item.TenentId = tenantId;
                    item.WarehouseId = warehouseId;
                    total = total + item.TotalAmount;
                    _currentDbContext.OrderDetail.Add(item);
                }

                var toDelete = _currentDbContext.OrderDetail.Where(a => a.OrderID == Order.OrderID && a.IsDeleted != true).Select(a => a.OrderDetailID).Except(cItems.Select(a => a.OrderDetailID).ToList());
                foreach (var item in toDelete)
                {
                    var dItem = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == item);
                    dItem.IsDeleted = true;
                }
                var toDeleteor = _currentDbContext.OrderDetail.Where(a => a.OrderID == Trnsferout.OrderID && a.IsDeleted != true).Select(a => a.OrderDetailID).Except(cItems.Select(a => a.OrderDetailID).ToList());
                foreach (var items in toDelete)
                {
                    var dItem = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == items);
                    dItem.IsDeleted = true;
                }
                foreach (var eItem in cItems)
                {
                    var dItem = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == eItem.OrderDetailID);
                    var itemdetail = _currentDbContext.OrderDetail.FirstOrDefault(u => u.OrderID == Trnsferout.OrderID)?.OrderDetailID;
                    var dItems = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == itemdetail);
                    if (dItem != null)
                    {
                        dItem.Qty = eItem.Qty;
                        dItem.ProductId = eItem.ProductId;
                        dItem.Price = eItem.Price;
                        dItem.Notes = eItem.Notes;
                        _currentDbContext.Entry(dItem).State = EntityState.Modified;
                    }
                    if (dItems != null)
                    {
                        dItems.Qty = eItem.Qty;
                        dItems.ProductId = eItem.ProductId;
                        dItems.Price = eItem.Price;
                        dItems.Notes = eItem.Notes;
                        _currentDbContext.Entry(dItems).State = EntityState.Modified;
                    }
                }
            }
            else
            {
                foreach (var item in _currentDbContext.OrderDetail.Where(a => a.OrderID == Order.OrderID && a.IsDeleted != true))
                {
                    item.IsDeleted = true;
                }
                foreach (var item in _currentDbContext.OrderDetail.Where(a => a.OrderID == Trnsferout.OrderID && a.IsDeleted != true))
                {
                    item.IsDeleted = true;
                }

            }
            Order.OrderTotal = total;
            Trnsferout.OrderTotal = total;
            var obj = _currentDbContext.Order.Find(Order.OrderID);
            if (Order.OrderStatusID == 0)
            {
                Order.OrderStatusID = obj.OrderStatusID;
                Trnsferout.OrderStatusID = obj.OrderStatusID;
            }
            
            _currentDbContext.Entry(obj).State = System.Data.Entity.EntityState.Detached;
            _currentDbContext.Entry(Trnsferout).State = System.Data.Entity.EntityState.Detached;

            _currentDbContext.Order.Attach(Order);
          
            var entry = _currentDbContext.Entry(Order);

            entry.Property(e => e.OrderID).IsModified = true;
            entry.Property(e => e.OrderNumber).IsModified = true;
            entry.Property(e => e.ExpectedDate).IsModified = true;
            entry.Property(e => e.Note).IsModified = true;
            entry.Property(e => e.OrderStatusID).IsModified = true;

            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            entry.Property(e => e.OrderTotal).IsModified = true;
            entry.Property(e => e.DepartmentId).IsModified = true;
          
            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(Order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            Inventory.StockRecalculateByOrderId(Trnsferout.OrderID, Trnsferout.WarehouseId ?? warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return Order;
        }


        public Order DeleteTransferOrder(int orderId, int tenantId, int warehouseId, int userId)
        {
            var order= _currentDbContext.Order.FirstOrDefault(u => u.OrderID == orderId);
            var group = _currentDbContext.Order.FirstOrDefault(u => u.OrderNumber == order.OrderNumber)?.OrderGroupToken;
            var Trnsferout = _currentDbContext.Order.FirstOrDefault(u => u.OrderNumber != order.OrderNumber && u.OrderGroupToken == group);
            order.DateUpdated = DateTime.UtcNow;
            order.IsDeleted = true;
            order.IsCancel = true;
            Trnsferout.IsCancel = true;
            Trnsferout.IsDeleted = true;
            Trnsferout.DateUpdated = DateTime.UtcNow;
            order.UpdatedBy = userId;
            Trnsferout.UpdatedBy = userId;
            _currentDbContext.Entry(order).State = EntityState.Modified;
            _currentDbContext.Entry(Trnsferout).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(orderId, caCurrent.CurrentWarehouse().WarehouseId, caCurrent.CurrentTenant().TenantId, caCurrent.CurrentUser().UserId, true);
            Inventory.StockRecalculateByOrderId(Trnsferout.OrderID, caCurrent.CurrentWarehouse().WarehouseId, caCurrent.CurrentTenant().TenantId, caCurrent.CurrentUser().UserId, true);

            return order;
        
    }
}
}