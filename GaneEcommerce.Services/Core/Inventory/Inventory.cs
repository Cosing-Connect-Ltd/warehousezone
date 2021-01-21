using AutoMapper;
using Elmah;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    public class Inventory
    {
        public static bool StockTransaction(int productId, InventoryTransactionTypeEnum transType, decimal quantity,
            int? orderID, int? locationId = null, string transactionRef = null, int? serialId = null,
            int? pallettrackingId = null, int? orderprocessId = null, int? orderProcessDetailId = null)
        {
            Boolean status = false;
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            caTenant tenant = caCurrent.CurrentTenant();
            caUser user = caCurrent.CurrentUser();
            TenantLocations warehouse = caCurrent.CurrentWarehouse();
            int location = GetLocation(tenant.TenantId, warehouse.WarehouseId, user.UserId, locationId);

            if (user == null || tenant == null || warehouse == null || !ValidateProduct(productId) || quantity < 0)
            {
                return false;
            }

            if (serialId != null)
            {
                var serial = context.ProductSerialization.Find(serialId);
                if (serial == null)
                {
                    return false;
                }

                serial.CurrentStatus = (InventoryTransactionTypeEnum)transType;
            }

            // if DontMonitorStock flag is true then make that flag true in inventory as well
            bool dontMonitorStock = CheckDontStockMonitor(productId, null, orderID);


            InventoryTransaction transaction = new InventoryTransaction()
            {

                InventoryTransactionTypeId = transType,
                OrderID = orderID > 0 ? orderID : null,
                ProductId = productId,
                WarehouseId = warehouse.WarehouseId,
                TenentId = tenant.TenantId,
                Quantity = GetUomQuantity(productId, quantity),
                OrderProcessId = orderprocessId.HasValue ? orderprocessId : null,
                OrderProcessDetailId = orderProcessDetailId.HasValue ? orderProcessDetailId : null,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                CreatedBy = user.UserId,
                UpdatedBy = user.UserId,
                IsActive = true,
                LocationId = location,
                InventoryTransactionRef = transactionRef,
                SerialID = serialId,
                DontMonitorStock = dontMonitorStock,
                LastQty = CalculateLastQty(productId, tenant.TenantId, warehouse.WarehouseId, GetUomQuantity(productId, quantity), transType,
                    dontMonitorStock),
                PalletTrackingId = pallettrackingId

            };


            //add changes to context
            context.InventoryTransactions.Add(transaction);

            if (context.SaveChanges() > 0)
            {
                StockRecalculate(productId, warehouse.WarehouseId, tenant.TenantId, user.UserId);
                status = true;
                AdjustRecipeItemsInventory(transaction);

                //calculate location stock
                if (!dontMonitorStock)
                {
                    AdjustStockLocations(transaction.ProductId, 0, location, transaction.Quantity, transaction.WarehouseId, transaction.TenentId, transaction.CreatedBy, pallettrackingId, serialId, false);
                }
            }

            return status;
        }

        public static int StockTransaction(GoodsReturnRequestSync goodsReturnRequestSync, int? cons_Type,
            string groupToken = null, AccountShipmentInfo shipmentInfo = null)
        {
            try
            {
                int location = GetLocation(goodsReturnRequestSync.tenantId, goodsReturnRequestSync.warehouseId, goodsReturnRequestSync.userId, goodsReturnRequestSync.LocationId);

                if (goodsReturnRequestSync.MissingTrackingNo == true)
                {
                    return StockTransaction(goodsReturnRequestSync, groupToken);
                }

                var context = DependencyResolver.Current.GetService<IApplicationContext>();
                var orderservice = DependencyResolver.Current.GetService<IOrderService>();
                int user = goodsReturnRequestSync.userId;
                int? locationId = goodsReturnRequestSync.LocationId;
                locationId = locationId == 0 ? null : locationId;
                if (goodsReturnRequestSync.OrderId <= 0 &&
                    (goodsReturnRequestSync.InventoryTransactionType == InventoryTransactionTypeEnum.Wastage
                     || goodsReturnRequestSync.InventoryTransactionType == InventoryTransactionTypeEnum.AdjustmentIn ||
                     goodsReturnRequestSync.InventoryTransactionType == InventoryTransactionTypeEnum.AdjustmentOut))
                {
                    int? orderId = goodsReturnRequestSync.OrderId;
                    orderId = orderId == 0 ? null : orderId;

                    Inventory.StockTransactionApi(goodsReturnRequestSync.ProductId,
                        goodsReturnRequestSync.InventoryTransactionType ?? 0, GetUomQuantity(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.Quantity ?? 0),
                        orderId, goodsReturnRequestSync.tenantId, goodsReturnRequestSync.warehouseId, user, null, null,
                        null, groupToken);
                    return 0;

                }

                var cOrder = context.Order.Find(goodsReturnRequestSync.OrderId);
                if (goodsReturnRequestSync.OrderId <= 0 && goodsReturnRequestSync.InventoryTransactionType > 0)
                {
                    cOrder = orderservice.CreateOrderByOrderNumber(goodsReturnRequestSync.OrderNumber,
                        goodsReturnRequestSync.ProductId, goodsReturnRequestSync.tenantId,
                        goodsReturnRequestSync.warehouseId, goodsReturnRequestSync.InventoryTransactionType ?? 0, user,
                        GetUomQuantity(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.Quantity ?? 1));
                }

                if (goodsReturnRequestSync.OrderDetailID == null || goodsReturnRequestSync.OrderDetailID < 1)
                {
                    goodsReturnRequestSync.OrderDetailID = cOrder.OrderDetails
                        .Where(u => u.ProductId == goodsReturnRequestSync.ProductId).FirstOrDefault()?.OrderDetailID;
                }

                var xopr = orderservice.GetOrderProcessByDeliveryNumber(cOrder.OrderID,
                    goodsReturnRequestSync.InventoryTransactionType ?? 0, goodsReturnRequestSync.deliveryNumber, user,
                    warehouseId: goodsReturnRequestSync.warehouseId);
                var orderprocess = new OrderProcessDetail()
                {
                    CreatedBy = user,
                    DateCreated = DateTime.UtcNow,
                    OrderProcessId = xopr.OrderProcessID,
                    ProductId = goodsReturnRequestSync.ProductId,
                    TenentId = goodsReturnRequestSync.userId,
                    QtyProcessed = goodsReturnRequestSync.ProductSerials.Count,
                    OrderDetailID = goodsReturnRequestSync.OrderDetailID,
                };
                context.OrderProcessDetail.Add(orderprocess);
                context.SaveChanges();
                var orderDetail = context.OrderDetail.First(m =>
                    m.OrderID == cOrder.OrderID && m.ProductId == goodsReturnRequestSync.ProductId);
                var warrantyInfo = orderDetail.Warranty;
                foreach (var item in goodsReturnRequestSync.ProductSerials)
                {
                    ProductSerialis serial = null;
                    serial = context.ProductSerialization.FirstOrDefault(a =>
                        a.SerialNo == item && a.CurrentStatus !=
                        (InventoryTransactionTypeEnum)goodsReturnRequestSync.InventoryTransactionType);

                    if (serial != null)
                    {
                        serial.CurrentStatus =
                            (InventoryTransactionTypeEnum)goodsReturnRequestSync.InventoryTransactionType;
                        serial.DateUpdated = DateTime.UtcNow;
                        serial.UpdatedBy = user;
                        serial.TenentId = goodsReturnRequestSync.tenantId;
                        serial.LocationId = location;
                        serial.WarehouseId = goodsReturnRequestSync.warehouseId;
                    }
                    else
                    {
                        serial = new ProductSerialis
                        {
                            CreatedBy = user,
                            DateCreated = DateTime.UtcNow,
                            SerialNo = item,
                            TenentId = goodsReturnRequestSync.tenantId,
                            ProductId = goodsReturnRequestSync.ProductId,
                            LocationId = location,
                            CurrentStatus =
                                (InventoryTransactionTypeEnum)goodsReturnRequestSync.InventoryTransactionType,
                            WarehouseId = goodsReturnRequestSync.warehouseId
                        };
                    }

                    if (warrantyInfo != null)
                    {
                        serial.SoldWarrantyStartDate = DateTime.UtcNow;
                        serial.SoldWarrentyEndDate = DateTime.UtcNow.AddDays(warrantyInfo.WarrantyDays);
                        serial.DeliveryMethod = warrantyInfo.DeliveryMethod;
                        serial.SoldWarrantyIsPercent = warrantyInfo.IsPercent;
                        serial.SoldWarrantyName = warrantyInfo.WarrantyName;
                        serial.SoldWarrantyPercentage = warrantyInfo.PercentageOfPrice;
                        serial.SoldWarrantyFixedPrice = warrantyInfo.FixedPrice;
                    }

                    if (context.Entry(serial).State == EntityState.Detached)
                    {
                        context.Entry(serial).State = EntityState.Added;
                    }
                    else
                    {
                        context.Entry(serial).State = EntityState.Modified;
                    }

                    // if DontMonitorStock flag is true then make that flag true in inventory as well
                    bool dontMonitorStock = CheckDontStockMonitor(goodsReturnRequestSync.ProductId,
                        goodsReturnRequestSync.OrderDetailID, goodsReturnRequestSync.OrderId);

                    InventoryTransaction tans = new InventoryTransaction
                    {
                        LocationId = location,
                        OrderID = cOrder.OrderID,
                        ProductId = goodsReturnRequestSync.ProductId,
                        CreatedBy = user,
                        DateCreated = DateTime.UtcNow,
                        InventoryTransactionTypeId = goodsReturnRequestSync.InventoryTransactionType ?? 0,
                        ProductSerial = serial,
                        Quantity = 1,
                        InventoryTransactionRef = groupToken,
                        TenentId = goodsReturnRequestSync.tenantId,
                        WarehouseId = goodsReturnRequestSync.warehouseId,
                        DontMonitorStock = dontMonitorStock,
                        OrderProcessId = xopr?.OrderProcessID,
                        OrderProcessDetailId = orderprocess?.OrderProcessDetailID,
                        LastQty = CalculateLastQty(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.tenantId,
                            goodsReturnRequestSync.warehouseId, 1, goodsReturnRequestSync.InventoryTransactionType ?? 0,
                            dontMonitorStock)
                    };

                    context.InventoryTransactions.Add(tans);

                    context.SaveChanges();

                    StockRecalculate(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.warehouseId,
                        goodsReturnRequestSync.tenantId, user);

                    //calculate location stock
                    if (!dontMonitorStock)
                    {
                        AdjustStockLocations(tans.ProductId, 0, location, tans.Quantity, tans.WarehouseId, tans.TenentId, tans.CreatedBy, null, tans.SerialID, false);
                    }
                }

                return cOrder.OrderID;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return -1;
            }
        }

        public static int StockTransaction(GoodsReturnRequestSync goodsReturnRequestSync, string groupToken = null,
            int? userId = null)
        {
            try
            {
                int location = GetLocation(goodsReturnRequestSync.tenantId, goodsReturnRequestSync.warehouseId, goodsReturnRequestSync.userId, goodsReturnRequestSync.LocationId);
                var context = DependencyResolver.Current.GetService<IApplicationContext>();
                var orderservice = DependencyResolver.Current.GetService<IOrderService>();
                var UserId = goodsReturnRequestSync.userId;

                if (goodsReturnRequestSync.OrderId <= 0 &&
                    (goodsReturnRequestSync.InventoryTransactionType ==
                        InventoryTransactionTypeEnum.Wastage || goodsReturnRequestSync.InventoryTransactionType ==
                                                             InventoryTransactionTypeEnum.AdjustmentIn
                                                             || goodsReturnRequestSync.InventoryTransactionType ==
                                                             InventoryTransactionTypeEnum.AdjustmentOut))
                {
                    int? orderId = goodsReturnRequestSync.OrderId;
                    orderId = orderId == 0 ? null : orderId;

                    Inventory.StockTransactionApi(goodsReturnRequestSync.ProductId,
                        goodsReturnRequestSync.InventoryTransactionType ?? 0, GetUomQuantity(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.Quantity ?? 0),
                        orderId, goodsReturnRequestSync.tenantId, goodsReturnRequestSync.warehouseId, UserId, null,
                        null, null, groupToken);
                    return 0;

                }


                var cOrder = context.Order.Find(goodsReturnRequestSync.OrderId);

                if (goodsReturnRequestSync.OrderId <= 0 && goodsReturnRequestSync.InventoryTransactionType > 0)
                {
                    cOrder = orderservice.CreateOrderByOrderNumber(goodsReturnRequestSync.OrderNumber,
                        goodsReturnRequestSync.ProductId, goodsReturnRequestSync.tenantId,
                        goodsReturnRequestSync.warehouseId, goodsReturnRequestSync.InventoryTransactionType ?? 0,
                        UserId, GetUomQuantity(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.Quantity ?? 1));
                }

                if (goodsReturnRequestSync.OrderDetailID == null || goodsReturnRequestSync.OrderDetailID < 1)
                {
                    goodsReturnRequestSync.OrderDetailID = cOrder.OrderDetails
                        .Where(u => u.ProductId == goodsReturnRequestSync.ProductId).FirstOrDefault()?.OrderDetailID;
                }

                var oprocess = orderservice.GetOrderProcessByDeliveryNumber(cOrder.OrderID,
                    goodsReturnRequestSync.InventoryTransactionType ?? 0, goodsReturnRequestSync.deliveryNumber, UserId,
                    warehouseId: goodsReturnRequestSync.warehouseId);
                var xopr = new OrderProcessDetail()
                {
                    CreatedBy = UserId,
                    DateCreated = DateTime.UtcNow,
                    OrderProcessId = oprocess.OrderProcessID,
                    ProductId = goodsReturnRequestSync.ProductId,
                    TenentId = goodsReturnRequestSync.tenantId,
                    QtyProcessed = GetUomQuantity(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.Quantity ?? 1),
                    OrderDetailID = goodsReturnRequestSync.OrderDetailID
                };

                context.OrderProcessDetail.Add(xopr);
                context.SaveChanges();

                // if DontMonitorStock flag is true then make that flag true in inventory as well
                bool dontMonitorStock = CheckDontStockMonitor(goodsReturnRequestSync.ProductId,
                    goodsReturnRequestSync.OrderDetailID, goodsReturnRequestSync.OrderId);

                InventoryTransaction trans = new InventoryTransaction
                {
                    CreatedBy = UserId,
                    OrderID = cOrder.OrderID,
                    DateCreated = DateTime.UtcNow,
                    InventoryTransactionTypeId = goodsReturnRequestSync.InventoryTransactionType ?? 0,
                    ProductId = goodsReturnRequestSync.ProductId,
                    Quantity = GetUomQuantity(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.Quantity ?? 1),
                    InventoryTransactionRef = groupToken,
                    DontMonitorStock = dontMonitorStock,
                    OrderProcessId = oprocess?.OrderProcessID,
                    OrderProcessDetailId = xopr?.OrderProcessDetailID,
                    LastQty = CalculateLastQty(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.tenantId,
                        goodsReturnRequestSync.warehouseId, GetUomQuantity(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.Quantity ?? 1),
                        goodsReturnRequestSync.InventoryTransactionType ?? 0, dontMonitorStock),
                    TenentId = goodsReturnRequestSync.tenantId,
                    WarehouseId = goodsReturnRequestSync.warehouseId,
                    IsCurrentLocation = true,
                    LocationId = location
                };

                context.InventoryTransactions.Add(trans);
                context.SaveChanges();

                StockRecalculate(goodsReturnRequestSync.ProductId, goodsReturnRequestSync.warehouseId,
                    goodsReturnRequestSync.tenantId, UserId);

                //calculate location stock
                if (!dontMonitorStock)
                {
                    AdjustStockLocations(trans.ProductId, 0, location, trans.Quantity, trans.WarehouseId, trans.TenentId, trans.CreatedBy, null, null, false);
                }

                return cOrder.OrderID;

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return -1;
            }

        }

        public static void StockTransaction(InventoryTransaction model, InventoryTransactionTypeEnum type,
            int? cons_type, string delivery, int? Line_Id, List<CommonLocationViewModel> stockLocations = null,
            AccountShipmentInfo shipmentInfo = null)
        {
            int location = GetLocation(model.TenentId, model.WarehouseId, model.CreatedBy, model.LocationId);
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var orderservice = DependencyResolver.Current.GetService<IOrderService>();
            InventoryTransaction AutoTransferInventoryTransaction = new InventoryTransaction();
            bool reverseInventoryTransaction = false;

            var tenantConfig = context.TenantConfigs.First(m => m.TenantId == model.TenentId);
            int warehouseId = model.WarehouseId;
            var order = context.Order.FirstOrDefault(x => x.OrderID == model.OrderID);
            if (stockLocations != null && stockLocations.Count > 0)
            {
                model.Quantity = stockLocations.Sum(m => m.Quantity);
            }

            var consolidateOrderProcess = false;

            if (warehouseId > 0)
            {
                consolidateOrderProcess = context.TenantWarehouses
                    .FirstOrDefault(x => x.WarehouseId == warehouseId && x.IsDeleted != true).ConsolidateOrderProcesses;

            }

            var oprocess = context.OrderProcess.Where(m => m.OrderID == model.OrderID && m.IsDeleted != true).ToList()
                .FirstOrDefault(m => consolidateOrderProcess == true ||
                                     (!string.IsNullOrEmpty(m.DeliveryNO) && !string.IsNullOrEmpty(delivery) &&
                                      m.DeliveryNO.Trim().Equals(delivery.Trim(), StringComparison.OrdinalIgnoreCase)));

            if (oprocess == null)
            {
                OrderProcess opr = new OrderProcess
                {
                    DeliveryNO = delivery,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy,
                    OrderID = model.OrderID,
                    TenentId = model.TenentId,
                    WarehouseId = model.WarehouseId,
                    DeliveryMethod = cons_type > 0 ? (DeliveryMethods)cons_type : (DeliveryMethods?)null,
                    InventoryTransactionTypeId = type
                };

                if (shipmentInfo != null)
                {
                    opr.ShipmentAddressName = shipmentInfo.ShipmentAddressName;
                    opr.ShipmentAddressLine1 = shipmentInfo.ShipmentAddressLine1;
                    opr.ShipmentAddressLine2 = shipmentInfo.ShipmentAddressLine2;
                    opr.ShipmentAddressLine3 = shipmentInfo.ShipmentAddressLine3;
                    opr.ShipmentAddressTown = shipmentInfo.ShipmentAddressTown;
                    opr.ShipmentAddressPostcode = shipmentInfo.ShipmentAddressPostcode;
                    opr.ShipmentCountryId = shipmentInfo.ShipmentCountryId;
                }
                else
                {
                    opr.ShipmentAddressName = order.ShipmentAddressName;
                    opr.ShipmentAddressLine1 = order.ShipmentAddressLine1;
                    opr.ShipmentAddressLine2 = order.ShipmentAddressLine2;
                    opr.ShipmentAddressLine3 = order.ShipmentAddressLine3;
                    opr.ShipmentAddressTown = order.ShipmentAddressTown;
                    opr.ShipmentAddressPostcode = order.ShipmentAddressPostcode;
                    opr.ShipmentCountryId = order.ShipmentCountryId;
                }

                var orderDetail =
                    order.OrderDetails.FirstOrDefault(m => m.ProductId == model.ProductId && m.Qty >= m.ProcessedQty);

                OrderProcessDetail odet = new OrderProcessDetail
                {
                    CreatedBy = model.CreatedBy,
                    DateCreated = DateTime.UtcNow,
                    OrderProcessId = opr.OrderProcessID,
                    ProductId = model.ProductId,
                    TenentId = model.TenentId,
                    BatchNumber = model.BatchNumber ?? null,
                    ExpiryDate = model.ExpiryDate ?? null,
                    QtyProcessed = GetUomQuantity(model.ProductId, model.Quantity),
                    OrderDetailID = orderDetail?.OrderDetailID
                };


                opr.OrderProcessDetail.Add(odet);
                context.OrderProcess.Add(opr);
                context.SaveChanges();
                model.OrderProcessDetailId = odet?.OrderProcessDetailID;
                model.OrderProcessId = opr?.OrderProcessID;
                if (tenantConfig.AutoTransferStockEnabled == true &&
                    order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut)
                {
                    var cOrder = context.Order.Find(model.OrderID);
                    var altOrder = context.Order.FirstOrDefault(m =>
                        m.OrderID != model.OrderID && m.OrderGroupToken.HasValue &&
                        m.OrderGroupToken == cOrder.OrderGroupToken);

                    if (altOrder != null && altOrder.Warehouse.AutoTransferOrders == true)
                    {
                        var AltOrderDetail = altOrder.OrderDetails.FirstOrDefault(m =>
                            m.ProductId == model.ProductId && m.Qty >= m.ProcessedQty);

                        var xopr = new OrderProcess
                        {
                            DeliveryNO = delivery,
                            DateCreated = DateTime.UtcNow,
                            CreatedBy = model.CreatedBy,
                            OrderID = altOrder?.OrderID,
                            TenentId = model.TenentId,

                            WarehouseId = altOrder.WarehouseId ?? 0,
                            DeliveryMethod = cons_type > 0 ? (DeliveryMethods)cons_type : (DeliveryMethods?)null,
                            InventoryTransactionTypeId = altOrder.InventoryTransactionTypeId,
                            ShipmentAddressName = altOrder.ShipmentAddressName,
                            ShipmentAddressLine1 = altOrder.ShipmentAddressLine1,
                            ShipmentAddressLine2 = altOrder.ShipmentAddressLine2,
                            ShipmentAddressLine3 = altOrder.ShipmentAddressLine3,
                            ShipmentAddressTown = altOrder.ShipmentAddressTown,
                            ShipmentAddressPostcode = altOrder.ShipmentAddressPostcode,
                            ShipmentCountryId = altOrder.ShipmentCountryId
                        };

                        var orderprocessdet = new OrderProcessDetail
                        {
                            CreatedBy = model.CreatedBy,
                            DateCreated = DateTime.UtcNow,
                            OrderProcessId = xopr.OrderProcessID,
                            ProductId = model.ProductId,
                            TenentId = model.TenentId,
                            QtyProcessed = GetUomQuantity(model.ProductId, model.Quantity),
                            OrderDetailID = AltOrderDetail?.OrderDetailID,
                            BatchNumber = model.BatchNumber ?? null,
                            ExpiryDate = model.ExpiryDate ?? null,

                        };
                        xopr.OrderProcessDetail.Add(orderprocessdet);
                        context.OrderProcess.Add(xopr);
                        context.SaveChanges();

                        //create an inventory transaction for other warehouse
                        var altTransaction = new InventoryTransaction();
                        bool dontMonitorStockAlt = CheckDontStockMonitor(model.ProductId, AltOrderDetail.OrderDetailID,
                            model.OrderID);

                        altTransaction.CreatedBy = model.CreatedBy;
                        altTransaction.DateCreated = DateTime.UtcNow;
                        altTransaction.InventoryTransactionTypeId = altOrder.InventoryTransactionTypeId;
                        altTransaction.TenentId = model.TenentId;
                        altTransaction.DontMonitorStock = dontMonitorStockAlt;
                        altTransaction.WarehouseId = altOrder.WarehouseId == null ? model.WarehouseId : (int)altOrder.WarehouseId;
                        altTransaction.Quantity = GetUomQuantity(model.ProductId, model.Quantity);
                        altTransaction.OrderProcessId = xopr.OrderProcessID;
                        altTransaction.OrderProcessDetailId = orderprocessdet?.OrderProcessDetailID;
                        altTransaction.ProductId = model.ProductId;
                        altTransaction.OrderID = altOrder.OrderID;
                        altTransaction.LocationId = location;
                        AutoTransferInventoryTransaction = altTransaction;
                        reverseInventoryTransaction = true;
                    }
                }
            }
            else
            {
                oprocess.DateUpdated = DateTime.UtcNow;
                oprocess.UpdatedBy = model.CreatedBy;
                var odet = new OrderProcessDetail
                {
                    CreatedBy = model.CreatedBy,
                    DateCreated = DateTime.UtcNow,
                    OrderProcessId = oprocess.OrderProcessID,
                    ProductId = model.ProductId,
                    TenentId = model.TenentId,
                    QtyProcessed = GetUomQuantity(model.ProductId, model.Quantity),
                    OrderDetailID = Line_Id,
                };

                model.OrderProcessDetailId = odet?.OrderProcessDetailID;
                model.OrderProcessId = oprocess?.OrderProcessID;
                var orderDetailLines = oprocess.Order.OrderDetails.Where(x => x.ProductId == model.ProductId).ToList();
                var totalRequiredQuantity = orderDetailLines.Sum(m => m.Qty);

                var orderDetailProcessedSoFar = oprocess.Order.OrderProcess
                    .SelectMany(m => m.OrderProcessDetail)
                    .Where(x => x.IsDeleted != true && x.ProductId == model.ProductId)
                    .Sum(m => m.QtyProcessed);
                //Here if total req quantity is less than the expected outgoing, process quantity = totalrequiredquantity - order.

                var remainingQty = model.Quantity;
                if (totalRequiredQuantity < (orderDetailProcessedSoFar + model.Quantity))
                {
                    remainingQty = totalRequiredQuantity - orderDetailProcessedSoFar + model.Quantity;
                }

                var allOrderDetailLines = orderDetailLines.Where(m => m.OrderDetailID == Line_Id).ToList();

                foreach (var od in allOrderDetailLines)
                {
                    if (remainingQty < 1 && (type == InventoryTransactionTypeEnum.SalesOrder ||
                                             type == InventoryTransactionTypeEnum.TransferOut ||
                                             type == InventoryTransactionTypeEnum.WorksOrder)) break;

                    odet = new OrderProcessDetail
                    {
                        CreatedBy = model.CreatedBy,
                        DateCreated = DateTime.UtcNow,
                        OrderProcessId = oprocess.OrderProcessID,
                        ProductId = model.ProductId,
                        TenentId = model.TenentId,
                        OrderDetailID = od.OrderDetailID,
                        BatchNumber = model.BatchNumber ?? null,
                        ExpiryDate = model.ExpiryDate ?? null,

                    };

                    var odAvailable = od.Qty - od.ProcessedQty;

                    if (odAvailable >= remainingQty)
                    {
                        odet.QtyProcessed = remainingQty;
                        remainingQty -= remainingQty;
                    }
                    else
                    {
                        var spareQuantity = remainingQty - odAvailable;
                        odet.QtyProcessed = spareQuantity;
                        remainingQty -= spareQuantity;
                    }

                    oprocess.OrderProcessDetail.Add(odet);
                    if (shipmentInfo != null)
                    {
                        oprocess.ShipmentAddressName = shipmentInfo.ShipmentAddressName ?? "";
                        oprocess.ShipmentAddressLine1 = shipmentInfo.ShipmentAddressLine1 ?? "";
                        oprocess.ShipmentAddressLine2 = shipmentInfo.ShipmentAddressLine2 ?? "";
                        oprocess.ShipmentAddressLine3 = shipmentInfo.ShipmentAddressLine3 ?? "";
                        oprocess.ShipmentAddressTown = shipmentInfo.ShipmentAddressTown ?? "";
                        oprocess.ShipmentAddressPostcode = shipmentInfo.ShipmentAddressPostcode ?? "";
                        oprocess.ShipmentCountryId = shipmentInfo.ShipmentCountryId;
                    }
                }


                if (tenantConfig.AutoTransferStockEnabled == true &&
                    order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut)
                {

                    var cOrder = context.Order.Find(model.OrderID);
                    var altOrder = context.Order.FirstOrDefault(m =>
                        m.OrderID != model.OrderID && m.OrderGroupToken.HasValue &&
                        m.OrderGroupToken == cOrder.OrderGroupToken);

                    if (altOrder != null && altOrder.Warehouse.AutoTransferOrders == true)
                    {
                        var AltOrderDetail = altOrder.OrderDetails.FirstOrDefault(m =>
                            m.ProductId == model.ProductId && m.Qty >= m.ProcessedQty);
                        var targetWarehouseProcess = context.OrderProcess
                            .Where(m => m.OrderID == model.OrderID && m.IsDeleted != true).ToList().FirstOrDefault(m =>
                                consolidateOrderProcess == true ||
                                (!string.IsNullOrEmpty(m.DeliveryNO) && !string.IsNullOrEmpty(delivery) && m.DeliveryNO
                                    .Trim().Equals(delivery.Trim(), StringComparison.OrdinalIgnoreCase)));

                        if (targetWarehouseProcess == null)
                        {
                            var xopr = new OrderProcess
                            {
                                DeliveryNO = delivery,
                                DateCreated = DateTime.UtcNow,
                                CreatedBy = model.CreatedBy,
                                OrderID = altOrder?.OrderID,
                                TenentId = model.TenentId,
                                WarehouseId = altOrder.WarehouseId ?? 0,
                                DeliveryMethod = cons_type > 0 ? (DeliveryMethods)cons_type : (DeliveryMethods?)null,
                                InventoryTransactionTypeId = altOrder.InventoryTransactionTypeId,
                                ShipmentAddressName = altOrder.ShipmentAddressName,
                                ShipmentAddressLine1 = altOrder.ShipmentAddressLine1,
                                ShipmentAddressLine2 = altOrder.ShipmentAddressLine2,
                                ShipmentAddressLine3 = altOrder.ShipmentAddressLine3,
                                ShipmentAddressTown = altOrder.ShipmentAddressTown,
                                ShipmentAddressPostcode = altOrder.ShipmentAddressPostcode,
                                ShipmentCountryId = altOrder.ShipmentCountryId
                            };

                            var orderProcessDet = new OrderProcessDetail
                            {
                                CreatedBy = model.CreatedBy,
                                DateCreated = DateTime.UtcNow,
                                OrderProcessId = xopr.OrderProcessID,
                                ProductId = model.ProductId,
                                TenentId = model.TenentId,
                                QtyProcessed = GetUomQuantity(model.ProductId, model.Quantity),
                                BatchNumber = model.BatchNumber ?? null,
                                ExpiryDate = model.ExpiryDate ?? null,
                                OrderDetailID = AltOrderDetail?.OrderDetailID
                            };
                            xopr.OrderProcessDetail.Add(orderProcessDet);
                            context.OrderProcess.Add(xopr);
                            context.SaveChanges();

                            //create an inventory transaction for other warehouse
                            var altTransaction = new InventoryTransaction();
                            bool dontMonitorStockAlt = CheckDontStockMonitor(model.ProductId,
                                AltOrderDetail.OrderDetailID, model.OrderID);

                            altTransaction.CreatedBy = model.CreatedBy;
                            altTransaction.DateCreated = DateTime.UtcNow;
                            altTransaction.InventoryTransactionTypeId = altOrder.InventoryTransactionTypeId;
                            altTransaction.TenentId = model.TenentId;
                            altTransaction.DontMonitorStock = dontMonitorStockAlt;
                            altTransaction.WarehouseId = altOrder.WarehouseId == null ? model.WarehouseId : (int)altOrder.WarehouseId;
                            altTransaction.Quantity = GetUomQuantity(model.ProductId, model.Quantity);
                            altTransaction.OrderProcessId = xopr.OrderProcessID;
                            altTransaction.OrderProcessDetailId = orderProcessDet?.OrderProcessDetailID;
                            altTransaction.ProductId = model.ProductId;
                            altTransaction.OrderID = altOrder.OrderID;
                            altTransaction.LocationId = location;
                            AutoTransferInventoryTransaction = altTransaction;
                            reverseInventoryTransaction = true;

                        }
                        else
                        {
                            var det = new OrderProcessDetail
                            {
                                CreatedBy = model.CreatedBy,
                                DateCreated = DateTime.UtcNow,
                                OrderProcessId = targetWarehouseProcess.OrderProcessID,
                                ProductId = model.ProductId,
                                TenentId = model.TenentId,
                                QtyProcessed = GetUomQuantity(model.ProductId, model.Quantity),
                                BatchNumber = model.BatchNumber ?? null,
                                ExpiryDate = model.ExpiryDate ?? null,
                                OrderDetailID = AltOrderDetail.OrderDetailID
                            };


                            targetWarehouseProcess.DateUpdated = DateTime.UtcNow;
                            targetWarehouseProcess.UpdatedBy = model.CreatedBy;
                            context.OrderProcessDetail.Add(odet);
                            context.SaveChanges();

                            //create an inventory transaction for other warehouse
                            var altTransaction = new InventoryTransaction();
                            bool dontMonitorStockAlt = CheckDontStockMonitor(model.ProductId,
                                AltOrderDetail.OrderDetailID, model.OrderID);

                            altTransaction.CreatedBy = model.CreatedBy;
                            altTransaction.DateCreated = DateTime.UtcNow;
                            altTransaction.InventoryTransactionTypeId = altOrder.InventoryTransactionTypeId;
                            altTransaction.TenentId = model.TenentId;
                            altTransaction.DontMonitorStock = dontMonitorStockAlt;
                            altTransaction.WarehouseId = altOrder.WarehouseId == null ? model.WarehouseId : (int)altOrder.WarehouseId;
                            altTransaction.Quantity = GetUomQuantity(model.ProductId, model.Quantity);
                            altTransaction.OrderProcessId = targetWarehouseProcess.OrderProcessID;
                            altTransaction.OrderProcessDetailId = det?.OrderProcessDetailID;
                            altTransaction.ProductId = model.ProductId;
                            altTransaction.OrderID = altOrder.OrderID;
                            altTransaction.LocationId = location;
                            AutoTransferInventoryTransaction = altTransaction;
                            reverseInventoryTransaction = true;

                        }
                    }
                }
            }

            // if DontMonitorStock flag is true then make that flag true in inventory as well
            bool dontMonitorStock = CheckDontStockMonitor(model.ProductId, Line_Id, model.OrderID);

            model.CreatedBy = model.CreatedBy;
            model.DateCreated = DateTime.UtcNow;
            model.InventoryTransactionTypeId = type;
            model.WarehouseId = model.WarehouseId;
            model.TenentId = model.TenentId;
            model.DontMonitorStock = dontMonitorStock;

            if (oprocess != null)
            {
                model.OrderProcessId = oprocess.OrderProcessID;

                oprocess.DateCreated = DateTime.UtcNow;
                context.Entry(oprocess).State = EntityState.Modified;
                context.SaveChanges();
                model.OrderProcessDetailId = oprocess?.OrderProcessDetail
                    ?.FirstOrDefault(u => u.ProductId == model.ProductId)?.OrderProcessDetailID;

            }


            if (stockLocations != null && stockLocations.Count > 0)
            {
                foreach (var item in stockLocations)
                {
                    if (context.ChangeTracker.Entries<InventoryTransaction>().All(e =>
                        e.Entity.InventoryTransactionId != model.InventoryTransactionId))
                    {
                        context.Entry(model).State = EntityState.Detached;
                    }

                    model.Location = context.Locations.FirstOrDefault(m => m.LocationCode == item.LocationCode);
                    model.BatchNumber = item.BatchNumber;
                    model.Quantity = GetUomQuantity(model.ProductId, item.Quantity);
                    model.LastQty = CalculateLastQty(model.ProductId, model.TenentId, model.WarehouseId, GetUomQuantity(model.ProductId, item.Quantity),
                        type, dontMonitorStock);
                    context.InventoryTransactions.Add(model);
                    context.SaveChanges();
                    StockRecalculate(model.ProductId, model.WarehouseId, model.TenentId, model.CreatedBy);

                    //calculate location stock
                    if (!dontMonitorStock)
                    {
                        AdjustStockLocations(model.ProductId, 0, location, item.Quantity, model.WarehouseId, model.TenentId, model.CreatedBy, model.PalletTrackingId, model.SerialID, false);
                    }
                }
            }
            else
            {
                model.LastQty = CalculateLastQty(model.ProductId, model.TenentId, model.WarehouseId, GetUomQuantity(model.ProductId, model.Quantity),
                    type, dontMonitorStock);


                context.InventoryTransactions.Add(model);
                context.SaveChanges();
                StockRecalculate(model.ProductId, model.WarehouseId, model.TenentId, model.CreatedBy);

                //calculate location stock
                if (!dontMonitorStock)
                {
                    AdjustStockLocations(model.ProductId, 0, location, model.Quantity, model.WarehouseId, model.TenentId, model.CreatedBy, model.PalletTrackingId, model.SerialID, false);
                }
            }

            if (reverseInventoryTransaction == true)
            {
                model.LastQty = CalculateLastQty(AutoTransferInventoryTransaction.ProductId,
                    AutoTransferInventoryTransaction.TenentId, AutoTransferInventoryTransaction.WarehouseId,
                    GetUomQuantity(model.ProductId, AutoTransferInventoryTransaction.Quantity),
                    AutoTransferInventoryTransaction.InventoryTransactionTypeId,
                    AutoTransferInventoryTransaction.DontMonitorStock ?? false);
                context.InventoryTransactions.Add(AutoTransferInventoryTransaction);
                context.SaveChanges();
                StockRecalculate(AutoTransferInventoryTransaction.ProductId,
                    AutoTransferInventoryTransaction.WarehouseId, model.TenentId, model.CreatedBy);

                //calculate location stock
                if (!dontMonitorStock)
                {
                    AdjustStockLocations(AutoTransferInventoryTransaction.ProductId, 0, location, AutoTransferInventoryTransaction.Quantity, AutoTransferInventoryTransaction.WarehouseId,
                        AutoTransferInventoryTransaction.TenentId, AutoTransferInventoryTransaction.CreatedBy, AutoTransferInventoryTransaction.PalletTrackingId, AutoTransferInventoryTransaction.SerialID, false);
                }
            }
        }

        public static InventoryTransaction StockTransactionApi(int productId, InventoryTransactionTypeEnum transType,
            decimal quantity, int? orderID, int tenantId, int warehouseId, int userId, int? locationId = null,
            int? pallettrackingId = null, int? terminalId = null, string GroupToken = null, int? orderProcessId = null,
            int? OrderProcessDetialId = null)
        {
            int location = GetLocation(tenantId, warehouseId, userId, locationId);
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            InventoryTransaction transaction = new InventoryTransaction();


            //validate parameters if they exist in current context and are valid
            if (!ValidateProduct(productId) || quantity < 0)
            {
                return transaction;
            }

            // if DontMonitorStock flag is true then make that flag true in inventory as well
            bool dontMonitorStock = CheckDontStockMonitor(productId, null, orderID);

            transaction.InventoryTransactionTypeId = transType;
            transaction.OrderID = orderID;
            transaction.ProductId = productId;
            transaction.WarehouseId = warehouseId;
            transaction.TenentId = tenantId;
            transaction.Quantity = GetUomQuantity(productId, quantity);
            transaction.LastQty =
                CalculateLastQty(productId, tenantId, warehouseId, GetUomQuantity(productId, quantity), transType, dontMonitorStock);
            transaction.DateCreated = DateTime.UtcNow;
            transaction.DateUpdated = DateTime.UtcNow;
            transaction.CreatedBy = userId;
            transaction.UpdatedBy = userId;
            transaction.InventoryTransactionRef = GroupToken;
            transaction.LocationId = location;
            transaction.IsActive = true;
            transaction.DontMonitorStock = dontMonitorStock;
            transaction.PalletTrackingId = pallettrackingId;
            transaction.TerminalId = terminalId;
            transaction.OrderProcessId = orderProcessId;
            transaction.OrderProcessDetailId = OrderProcessDetialId;

            context.InventoryTransactions.Add(transaction);

            if (context.SaveChanges() > 0)
            {
                StockRecalculate(productId, warehouseId, tenantId, userId);
                AdjustRecipeItemsInventory(transaction);

                //calculate location stock
                if (!dontMonitorStock)
                {
                    AdjustStockLocations(transaction.ProductId, 0, location, transaction.Quantity, transaction.WarehouseId, transaction.TenentId, transaction.CreatedBy, transaction.PalletTrackingId, transaction.SerialID, false);
                }
            }

            return transaction;
        }

        public static List<InventoryTransaction> StockTransactionApi(List<string> serials, int? order, int product,
            InventoryTransactionTypeEnum typeId, int? locationId, int tenantId, int warehouseId, int userId,
            int? orderProcessId = null, int? OrderProcessDetialId = null)
        {
            int location = GetLocation(tenantId, warehouseId, userId, locationId);
            var context = DependencyResolver.Current.GetService<IApplicationContext>();


            var lstTemp = new List<InventoryTransaction>();
            serials.Sort();
            foreach (var ser in serials)
            {
                ProductSerialis serial = context.ProductSerialization.FirstOrDefault(a => a.SerialNo == ser);

                // if DontMonitorStock flag is true then make that flag true in inventory as well
                bool dontMonitorStock = CheckDontStockMonitor(product, null, order);

                if (serial != null)
                {
                    serial.CurrentStatus = typeId;
                    serial.DateUpdated = DateTime.UtcNow;
                    serial.SoldWarrantyStartDate = null;
                    serial.SoldWarrentyEndDate = null;
                    context.Entry(serial).State = EntityState.Modified;

                    InventoryTransaction trans = new InventoryTransaction
                    {
                        CreatedBy = userId,
                        OrderID = order != 0 ? order : null,
                        DateCreated = DateTime.UtcNow,
                        InventoryTransactionTypeId = typeId,
                        ProductId = product,
                        Quantity = 1,
                        LastQty = CalculateLastQty(product, tenantId, warehouseId, 1, typeId, dontMonitorStock),
                        TenentId = tenantId,
                        WarehouseId = warehouseId,
                        LocationId = location,
                        OrderProcessId = orderProcessId,
                        OrderProcessDetailId = OrderProcessDetialId,
                        IsCurrentLocation = true,
                        SerialID = serial.SerialID,
                        DontMonitorStock = dontMonitorStock
                    };

                    context.InventoryTransactions.Add(trans);
                    lstTemp.Add(trans);

                    context.SaveChanges();
                    StockRecalculate(product, warehouseId, tenantId, userId);

                    //calculate location stock
                    if (!dontMonitorStock)
                    {
                        AdjustStockLocations(trans.ProductId, 0, location, trans.Quantity, trans.WarehouseId, trans.TenentId, trans.CreatedBy, trans.PalletTrackingId, trans.SerialID, false);
                    }
                }
            }

            context.SaveChanges();
            return lstTemp;
        }

        public static decimal GetTotalInStock(IQueryable<InventoryCalculationViewModel> query)
        {
            decimal totalIn;
            decimal totalOut;
            decimal inStock;
            decimal adjustmentIn;
            decimal adjustmentOut;
            decimal totalReturns;
            decimal transferIn;
            decimal transferOut;
            decimal worksOut;
            decimal samples;
            decimal directSales;
            decimal exchnageOut;
            decimal wastages;

            totalIn = query.Where(e => e.Type == InventoryTransactionTypeEnum.PurchaseOrder).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            totalOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.SalesOrder).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            totalReturns = query.Where(e => e.Type == InventoryTransactionTypeEnum.Returns).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            adjustmentIn = query.Where(e => e.Type == InventoryTransactionTypeEnum.AdjustmentIn)
                .Select(I => I.Quantity).DefaultIfEmpty(0).Sum();
            adjustmentOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.AdjustmentOut)
                .Select(I => I.Quantity).DefaultIfEmpty(0).Sum();
            transferIn = query.Where(e => e.Type == InventoryTransactionTypeEnum.TransferIn).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            transferOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.TransferOut).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            worksOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.WorksOrder).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            samples = query.Where(e => e.Type == InventoryTransactionTypeEnum.Samples).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            directSales = query.Where(e => e.Type == InventoryTransactionTypeEnum.DirectSales).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            exchnageOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.Exchange).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            wastages = query.Where(e => e.Type == InventoryTransactionTypeEnum.Wastage).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();

            inStock = (totalIn + totalReturns + adjustmentIn + transferIn) - adjustmentOut - totalOut - transferOut -
                      worksOut - samples - directSales - exchnageOut - wastages;

            return inStock;
        }

        public static bool StockRecalculate(int productId, int warehouseId, int tenantId, int userId,
            bool saveContext = true, IApplicationContext context = null)
        {
            if (context == null)
            {
                context = DependencyResolver.Current.GetService<IApplicationContext>();
            }

            decimal available;
            decimal inStock;

            var totals = (from e in context.InventoryTransactions
                          where e.ProductId == productId && e.WarehouseId == warehouseId &&
                                e.TenentId == tenantId && e.DontMonitorStock != true && e.IsDeleted != true
                          select new InventoryCalculationViewModel
                          {
                              Quantity = e.Quantity,
                              Type = e.InventoryTransactionTypeId,
                              Order = e.Order
                          });

            inStock = GetTotalInStock(totals);

            //On Order
            var itemsOrdered = context.Order
                .Where(m => (m.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder ||
                             m.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn) &&
                            m.WarehouseId == warehouseId &&
                            m.OrderStatusID != OrderStatusEnum.Complete &&
                            m.OrderStatusID != OrderStatusEnum.Cancelled &&
                            m.OrderStatusID != OrderStatusEnum.PostedToAccounts &&
                            m.OrderStatusID != OrderStatusEnum.Invoiced &&
                            m.ShipmentPropertyId == null && m.IsDeleted != true && m.IsCancel != true &&
                            m.DirectShip != true)
                .Select(m =>
                    m.OrderDetails.Where(p => p.ProductId == productId && p.IsDeleted != true).Select(x => x.Qty)
                        .DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

            var itemsReceived = context.Order
                .Where(m => (m.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder ||
                             m.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn) &&
                            m.WarehouseId == warehouseId &&
                            m.OrderStatusID != OrderStatusEnum.Complete &&
                            m.OrderStatusID != OrderStatusEnum.Cancelled &&
                            m.OrderStatusID != OrderStatusEnum.PostedToAccounts &&
                            m.OrderStatusID != OrderStatusEnum.Invoiced
                            && m.ShipmentPropertyId == null && m.IsDeleted != true && m.IsCancel != true &&
                            m.DirectShip != true)
                .Select(m => m.OrderProcess.Where(u => u.IsDeleted != true).Select(o => o.OrderProcessDetail.Where(p =>
                        p.ProductId == productId && p.IsDeleted != true &&
                        p.OrderDetail.DontMonitorStock != true).Select(q => q.QtyProcessed).DefaultIfEmpty(0)
                    .Sum()).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

            var itemsOnOrder = itemsOrdered - itemsReceived;

            if (itemsOnOrder < 1)
            {
                itemsOnOrder = 0;
            }

            // Allocated
            var itemsOnSalesOrders = context.Order
                .Where(m =>
                    (m.InventoryTransactionTypeId ==
                        InventoryTransactionTypeEnum.SalesOrder || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.WorksOrder
                                                                || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Loan ||
                                                                m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Samples
                                                                || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Exchange ||
                                                                m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.TransferOut
                                                                || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Wastage) &&
                    m.WarehouseId == warehouseId &&
                    m.OrderStatusID != OrderStatusEnum.Complete && m.OrderStatusID != OrderStatusEnum.Cancelled &&
                    m.OrderStatusID != OrderStatusEnum.PostedToAccounts && m.OrderStatusID != OrderStatusEnum.Invoiced
                    && m.IsDeleted != true && m.IsCancel != true && m.DirectShip != true)
                .Select(m =>
                    m.OrderDetails.Where(p => p.ProductId == productId && p.IsDeleted != true).Select(x => x.Qty)
                        .DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

            var itemsDispatched = context.Order
                .Where(m =>
                    (m.InventoryTransactionTypeId ==
                        InventoryTransactionTypeEnum.SalesOrder || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.WorksOrder
                                                                || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Loan ||
                                                                m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Samples
                                                                || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Exchange ||
                                                                m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.TransferOut ||
                                                                m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Wastage) &&
                    m.WarehouseId == warehouseId &&
                    m.OrderStatusID != OrderStatusEnum.Complete && m.OrderStatusID != OrderStatusEnum.Cancelled &&
                    m.OrderStatusID != OrderStatusEnum.PostedToAccounts && m.OrderStatusID != OrderStatusEnum.Invoiced
                    && m.IsDeleted != true && m.IsCancel != true && m.DirectShip != true)
                .Select(m => m.OrderProcess.Where(u => u.IsDeleted != true).Select(o => o.OrderProcessDetail
                    .Where(p => p.ProductId == productId && p.IsDeleted != true).Select(q => q.QtyProcessed)
                    .DefaultIfEmpty(0)
                    .Sum()).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

            var itemsAllocated = itemsOnSalesOrders - itemsDispatched;

            if (itemsAllocated < 1)
            {
                itemsAllocated = 0;
            }

            available = inStock - itemsAllocated;

            InventoryStock oldStock = context.InventoryStocks.Where(e =>
                e.ProductId == productId && e.WarehouseId == warehouseId && e.TenantId == tenantId &&
                e.IsDeleted != true).FirstOrDefault();

            if (oldStock != null)
            {
                if (oldStock.Available <= 0 && available > 0)
                {
                    AddProductToNotifyQueue(productId, tenantId, warehouseId, context);
                }

                if (available <= 0 && oldStock.Available > 0)
                {
                    RemoveProductFromNotifyQueue(productId, tenantId, warehouseId, context);
                }

                oldStock.InStock = inStock;
                oldStock.Allocated = itemsAllocated;
                oldStock.OnOrder = itemsOnOrder;
                oldStock.Available = available;
                oldStock.DateUpdated = DateTime.UtcNow;
                oldStock.UpdatedBy = userId;
                oldStock.IsActive = true;
                context.Entry(oldStock).State = EntityState.Modified;
            }
            else
            {
                InventoryStock newStock = new InventoryStock()
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    TenantId = tenantId,
                    InStock = inStock,
                    Allocated = itemsAllocated,
                    OnOrder = itemsOnOrder,
                    Available = available,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    UpdatedBy = userId,
                    CreatedBy = userId,
                    IsActive = true
                };

                AddProductToNotifyQueue(productId, tenantId, warehouseId, context);
                context.InventoryStocks.Add(newStock);
            }

            if (saveContext)
            {
                context.SaveChanges();
            }

            return true;
        }

        public static bool LocationStockRecalculate(int locationId, int warehouseId, int tenantId, int userId,
            bool saveContext = true, IApplicationContext context = null)
        {
            if (context == null)
            {
                context = DependencyResolver.Current.GetService<IApplicationContext>();
            }

            var totals = (from e in context.InventoryTransactions
                          where e.LocationId == locationId && e.WarehouseId == warehouseId
                                && e.TenentId == tenantId && e.DontMonitorStock != true && e.IsDeleted != true
                                && e.IsCurrentLocation == true && e.LocationId != null
                          && (e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder
                          || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns
                          || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn
                          || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn
                          || e.InventoryTransactionTypeId == InventoryTransactionTypeEnum.MovementIn)
                          group e by new { e.LocationId, e.ProductId, e.TenentId, e.WarehouseId, e.Quantity } into g
                          select new
                          {
                              Quantity = g.Key.Quantity,
                              ProductId = g.Key.ProductId,
                              LocationId = g.Key.LocationId
                          }).ToList();

            foreach (var item in totals)
            {
                ProductLocationStocks oldLocationStock = context.ProductLocationStocks.Where(e =>
                e.ProductId == item.ProductId && e.LocationId == item.LocationId && e.WarehouseId == warehouseId && e.TenantId == tenantId &&
                e.IsDeleted != true).FirstOrDefault();

                if (oldLocationStock != null)
                {
                    oldLocationStock.Quantity = item.Quantity;
                    oldLocationStock.DateUpdated = DateTime.UtcNow;
                    oldLocationStock.UpdatedBy = userId;
                    oldLocationStock.IsActive = true;
                    context.Entry(oldLocationStock).State = EntityState.Modified;
                }
                else
                {
                    ProductLocationStocks newStock = new ProductLocationStocks()
                    {
                        ProductId = item.ProductId,
                        WarehouseId = warehouseId,
                        TenantId = tenantId,
                        Quantity = item.Quantity,
                        LocationId = item.LocationId ?? 0,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        UpdatedBy = userId,
                        CreatedBy = userId,
                        IsActive = true
                    };

                    context.ProductLocationStocks.Add(newStock);
                }

            }

            if (saveContext)
            {
                context.SaveChanges();
            }

            return true;
        }

        public static Boolean LocationStockRecalculateAll(int warehouseId, int tenantId, int userId)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var locations = context.Locations.AsNoTracking()
                .Where(a => a.TenantId == tenantId && a.IsDeleted != true).Select(x => x.LocationId).ToList();

            var i = 0;

            foreach (var location in locations)
            {
                LocationStockRecalculate(location, warehouseId, tenantId, userId, i % 200 == 0, context);
                i++;
            }

            context.SaveChanges();
            return true;
        }

        private static void AddProductToNotifyQueue(int productId, int tenantId, int warehouseId, IApplicationContext context = null)
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

        private static void RemoveProductFromNotifyQueue(int productId, int tenantId, int warehouseId, IApplicationContext context = null)
        {
            var productAvailabilityNotify = context.ProductAvailabilityNotifyQueue.FirstOrDefault(n => n.ProductId == productId &&
                                                                 n.TenantId == tenantId &&
                                                                 n.WarehouseId == warehouseId);
            if (productAvailabilityNotify != null)
            {
                context.ProductAvailabilityNotifyQueue.Remove(productAvailabilityNotify);
            }
        }

        public static Boolean StockRecalculateAll(int WarehouseId, int TenantId, int UserId)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var products = context.ProductMaster.AsNoTracking()
                .Where(a => a.TenantId == TenantId && a.IsDeleted != true).Select(x => x.ProductId).ToList();
            var i = 0;

            foreach (var product in products)
            {
                StockRecalculate(product, WarehouseId, TenantId, UserId, i % 200 == 0, context);
                i++;
            }

            context.SaveChanges();
            return true;
        }

        public static Boolean StockRecalculateByOrderId(int OrderId, int WarehouseId, int TenantId, int UserId,
            bool isdeleted = false)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var orders = context.Order.AsNoTracking().FirstOrDefault(a => a.OrderID == OrderId && a.IsDeleted != true);
            if (isdeleted)
            {
                orders = context.Order.AsNoTracking().FirstOrDefault(a => a.OrderID == OrderId);
            }

            var i = 0;

            foreach (var orderdetail in orders.OrderDetails)
            {
                var product = orderdetail.ProductId;
                StockRecalculate(product, WarehouseId, TenantId, UserId, i % 200 == 0, context);
                i++;
            }

            context.SaveChanges();
            return true;
        }

        public static bool ValidateProduct(int productId)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            Boolean status = false;

            // get all products in specific warehouse
            var product = (from e in context.ProductMaster
                           where e.ProductId == productId
                                 && e.IsDeleted != true
                           select new
                           {
                               Id = e.ProductId

                           }).ToList();

            if (product.Any())
            {
                status = true;
            }

            return status;
        }

        public static void AdjustRecipeItemsInventory(InventoryTransaction parentTransaction)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var _mapper = DependencyResolver.Current.GetService<IMapper>();
            var productService = DependencyResolver.Current.GetService<IProductServices>();
            var productMaster = productService.GetProductMasterById(parentTransaction.ProductId);


            productMaster.ProductKitMap.Where(x => x.ProductKitType == ProductKitTypeEnum.Recipe).ToList().ForEach(p =>
            {
                // if DontMonitorStock flag is true then make that flag true in inventory as well
                bool dontMonitorStock = CheckDontStockMonitor(p.ProductId, null, null);

                var transaction = new InventoryTransaction();
                transaction.ProductId = p.KitProductId;
                transaction.CreatedBy = parentTransaction.CreatedBy;
                transaction.InventoryTransactionTypeId = parentTransaction.InventoryTransactionTypeId;
                transaction.OrderID = parentTransaction.OrderID;
                transaction.OrderProcessId = parentTransaction.OrderProcessId;
                transaction.OrderProcessDetailId = parentTransaction.OrderProcessDetailId;
                transaction.DateCreated = DateTime.UtcNow;
                transaction.TerminalId = parentTransaction.TerminalId;
                transaction.Quantity = parentTransaction.Quantity * p.Quantity;
                transaction.LastQty = CalculateLastQty(p.KitProductId, parentTransaction.TenentId,
                    parentTransaction.WarehouseId, (parentTransaction.Quantity * p.Quantity),
                    parentTransaction.InventoryTransactionTypeId, dontMonitorStock);
                context.InventoryTransactions.Add(transaction);
                context.SaveChanges();
                StockRecalculate(p.KitProductId, parentTransaction.WarehouseId, parentTransaction.TenentId,
                    parentTransaction.CreatedBy);
            });

        }

        private static decimal CalculateLastQty(int productId, int tenantId, int warehouseId, decimal newStock,
            InventoryTransactionTypeEnum transType, bool dontMonitorStock)
        {
            decimal totalStock = 0;

            if (dontMonitorStock == true)
            {
                newStock = 0;
            }

            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            decimal currentStock = context.InventoryStocks.AsNoTracking().FirstOrDefault(x =>
                x.ProductId == productId && x.TenantId == tenantId &&
                x.WarehouseId == warehouseId && x.IsDeleted != true)?.InStock ?? 0;


            if (transType == InventoryTransactionTypeEnum.AdjustmentIn ||
                transType == InventoryTransactionTypeEnum.PurchaseOrder ||
                transType == InventoryTransactionTypeEnum.Returns
                || transType == InventoryTransactionTypeEnum.TransferIn)
            {
                totalStock = currentStock + newStock;
            }
            else if (transType == InventoryTransactionTypeEnum.AdjustmentOut ||
                     transType == InventoryTransactionTypeEnum.DirectSales ||
                     transType == InventoryTransactionTypeEnum.Loan
                     || transType == InventoryTransactionTypeEnum.SalesOrder ||
                     transType == InventoryTransactionTypeEnum.Samples ||
                     transType == InventoryTransactionTypeEnum.TransferOut
                     || transType == InventoryTransactionTypeEnum.WorksOrder ||
                     transType == InventoryTransactionTypeEnum.Wastage)
            {
                totalStock = currentStock - newStock;
            }
            else
            {
                totalStock = currentStock;
            }

            return totalStock;
        }

        private static bool CheckDontStockMonitor(int productId, int? orderDetailId, int? orderId)
        {
            bool status = false;
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            status = context.ProductMaster.AsNoTracking().Any(x =>
                x.ProductId == productId && x.DontMonitorStock == true && x.IsDeleted != true);

            if (status != true)
            {
                if (orderDetailId != null && orderDetailId > 0)
                {
                    status = context.OrderDetail.AsNoTracking().Any(x =>
                        x.OrderDetailID == orderDetailId && x.DontMonitorStock == true && x.IsDeleted != true);
                }

                if (orderId != null && orderId > 0 && status != true)
                {
                    var order = context.Order.AsNoTracking().Where(x => x.OrderID == orderId).FirstOrDefault();
                    if ((order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder &&
                         order.ShipmentPropertyId != null) || order.DirectShip == true)
                    {
                        status = true;
                    }
                }
            }

            return status;
        }

        // correct pallet remaining cases
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
            inStock = GetTotalInStock(totals);
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

        public static void UpdateInvetoryTransaction(int orderId, int orderProcessId, int orderProcessDetailId)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var inventoryTransaction = context.InventoryTransactions.Where(u => u.OrderID == orderId).ToList();
            if (inventoryTransaction.Count > 0)
            {
                foreach (var item in inventoryTransaction)
                {
                    var transaction = context.InventoryTransactions.FirstOrDefault(u =>
                        u.InventoryTransactionId == item.InventoryTransactionId);
                    if (transaction != null)
                    {
                        transaction.DateUpdated = DateTime.UtcNow;
                        transaction.OrderProcessDetailId = orderProcessId;
                        transaction.OrderProcessDetailId = orderProcessDetailId;
                        context.InventoryTransactions.Attach(transaction);
                        context.Entry(transaction).State = EntityState.Modified;
                    }
                }

                context.SaveChanges();

            }
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

        public static bool AdjustStockMovementTransactions(StockMovementViewModel stockMovement)
        {
            bool status = false;
            var productService = DependencyResolver.Current.GetService<IProductServices>();
            var product = productService.GetProductMasterById(stockMovement.ProductId);

            if (stockMovement.PalletSerials.Count > 0)
            {
                foreach (var item in stockMovement.PalletSerials)
                {

                    status = AdjustStockLocations(stockMovement.ProductId, stockMovement.FromLocation, stockMovement.ToLocation,
                        GetUomQuantity(product.ProductId, (item.Cases * product.ProductsPerCase ?? 1)), stockMovement.WarehouseId, stockMovement.TenentId,
                        stockMovement.UserId, item.PalletSerialId, null, true);
                }
            }
            else if (stockMovement.SerialIds.Count > 0)
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
                            var location = GetLocation(item.TenentId, warehouseId, userId, item.LocationId);
                            InventoryTransactionForStockMovement(item, userId, tenantId, warehouseId, ProductMovementId, location, quantityToMove);
                            LocationStockRecalculate(location, warehouseId, tenantId, userId);
                        }

                        InventoryTransactionForStockMovement(item, userId, tenantId, warehouseId, ProductMovementId, toLocation, quantity);
                        LocationStockRecalculate(toLocation, warehouseId, tenantId, userId);
                        status = true;
                        break;
                    }
                    else
                    {
                        quantityToMove = quantityToMove - item.Quantity;
                        InventoryTransactionForStockMovement(item, userId, tenantId, warehouseId, ProductMovementId, toLocation, item.Quantity);
                        LocationStockRecalculate(toLocation, warehouseId, tenantId, userId);
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
                    LastQty = CalculateLastQty(inventoryTransaction.ProductId, Tenantid,
                        inventoryTransaction.WarehouseId, Qty, InventoryTransactionTypeEnum.MovementIn,
                        (inventoryTransaction.DontMonitorStock ?? false))
                };

                context.InventoryTransactions.Add(tans);
                context.SaveChanges();
            }
        }

        public static string GetProductAttributesValueToDisplay(ICollection<ProductAttributeValuesMap> productAttributeValuesMap)
        {
            return string.Join(", ", productAttributeValuesMap.Where(a => a.IsDeleted != true)
                                            .OrderBy(m => m.ProductAttributeValues.ProductAttributes.SortOrder)
                                            .ThenBy(m => m.ProductAttributeValues.SortOrder)
                                            .Select(a => a.ProductAttributeValues.Value));
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

        public static decimal GetUomQuantity(int productId, decimal quantity)
        {
            var productService = DependencyResolver.Current.GetService<IProductServices>();
            var product = productService.GetProductMasterById(productId);
            int? unit = product?.UOMId;

            if (unit == 1)
            {
                quantity = Math.Round(quantity);
            }

            return quantity;
        }

        public static int GetLocation(int tenantId, int warehouseId, int userId, int? locationId = null)
        {
            var lookupService = DependencyResolver.Current.GetService<LookupServices>();
            var location = lookupService.GetLocationById(locationId ?? 0, tenantId);

            if (location == null)
            {
                location = lookupService.GetAllLocations(tenantId, null, false).FirstOrDefault();
            }

            if (location == null)
            {
                var newLocation = new Locations
                {
                    LocationName = "Default",
                    LocationCode = "DEF",
                    WarehouseId = warehouseId,
                    TenantId = tenantId,
                    IsActive = true,
                    CreatedBy = userId,
                    DateCreated = DateTime.Now

                };

                location = lookupService.CreateLocation(newLocation, null, userId, tenantId, warehouseId);
            }

            return location.LocationId;

        }
    }
}
