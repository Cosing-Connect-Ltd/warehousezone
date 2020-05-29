using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        //Chnaged to public by saad
        private readonly IApplicationContext _currentDbContext;
        private readonly IOrderService _orderService;
        private readonly IProductServices _productServices;

        public PurchaseOrderService(IApplicationContext currentDbContext, IOrderService orderService, IProductServices productServices)
        {
            _currentDbContext = currentDbContext;
            _orderService = orderService;
            _productServices = productServices;
        }

        public IEnumerable<OrderPTenantEmailRecipient> GetAccountContactId(int OrderId)
        {

            return _currentDbContext.OrderPTenantEmailRecipients.Where(o => o.OrderId == OrderId && o.IsDeleted != true);

        }

        public List<PurchaseOrderViewModel> GetAllPurchaseOrders(int tenantId)
        {
            return _currentDbContext.Order.Where(o => o.TenentId == tenantId && o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder && o.IsDeleted != true).Select(p => new PurchaseOrderViewModel()
            {
                OrderID = p.OrderID,
                OrderNumber = p.OrderNumber,
                IssueDate = p.IssueDate,
                DateUpdated = p.DateUpdated,
                Property = _currentDbContext.PProperties.Where(s => s.PPropertyId == p.ShipmentPropertyId)
                    .Select(s => s.AddressLine1)
                    .FirstOrDefault(),
                POStatus = _currentDbContext.OrderStatus.Where(s => s.OrderStatusID == p.OrderStatusID).Select(s => s.Status).FirstOrDefault(),
                Account = _currentDbContext.Account.Where(s => s.AccountID == p.AccountID).Select(s => s.CompanyName).FirstOrDefault(),
            }).ToList();
        }

        public Order CreatePurchaseOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId, int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {

            order.OrderNumber = order.OrderNumber.Trim();

            var duplicateOrder = _currentDbContext.Order.FirstOrDefault(m => m.OrderNumber.Equals(order.OrderNumber, StringComparison.CurrentCultureIgnoreCase));
            if (duplicateOrder != null)
            {
                throw new Exception($"Order Number {order.OrderNumber} already associated with another Order. Please regenerate order number.", new Exception("Duplicate Order Number"));
            }

            order.IssueDate = DateTime.UtcNow;
            order.DateCreated = DateTime.UtcNow;
            order.DateUpdated = DateTime.UtcNow;
            order.TenentId = tenantId;
            order.CreatedBy = userId;
            order.WarehouseId = warehouseId;
            order.UpdatedBy = userId;

            order.InventoryTransactionTypeId = InventoryTransactionTypeEnum.PurchaseOrder;
            if (!caCurrent.CurrentWarehouse().AutoAllowProcess)
            {
                order.OrderStatusID = (int)OrderStatusEnum.Hold;
            }
            else
            {
                order.OrderStatusID = (int)OrderStatusEnum.Active;
            }


            if (order.AccountID > 0)
            {
                var account = _currentDbContext.Account.Find(order.AccountID);
                if (account != null)
                {
                    order.AccountCurrencyID = account.CurrencyID;
                }
            }

            if (shipmentAndRecipientInfo.SendEmailWithAttachment)
            {
                order.CustomEmailRecipient = shipmentAndRecipientInfo.CustomRecipients;
                order.CustomCCEmailRecipient = shipmentAndRecipientInfo.CustomCCRecipients;
                order.CustomBCCEmailRecipient = shipmentAndRecipientInfo.CustomBCCRecipients;
                if (shipmentAndRecipientInfo.AccountEmailContacts != null && shipmentAndRecipientInfo.AccountEmailContacts.Length > 0)
                {
                    foreach (var item in shipmentAndRecipientInfo.AccountEmailContacts)
                    {
                        string email = "";

                        var secondryAddress = _currentDbContext.AccountContacts.Where(u => u.AccountContactId == item).AsNoTracking().FirstOrDefault();
                        if (secondryAddress != null)
                            email = secondryAddress.ContactEmail;

                        var recipient = new OrderPTenantEmailRecipient()
                        {
                            OrderId = order.OrderID,
                            EmailAddress = email,
                            AccountContactId = item,
                            UpdatedBy = userId,
                            DateUpdated = DateTime.UtcNow
                        };
                        _currentDbContext.OrderPTenantEmailRecipients.Add(recipient);

                    }
                }
            }

            if (shipmentAndRecipientInfo.ShipmentDestination.Equals("warehouse",
                StringComparison.CurrentCultureIgnoreCase))
            {
                order.AccountAddressId = null;
                if (shipmentAndRecipientInfo.TenantAddressID.Contains("TenantLocMain"))
                {
                    var currentTenant = _currentDbContext.Tenants.First(m => m.TenantId == tenantId);
                    order.IsShippedToTenantMainLocation = true;
                    order.ShipmentAddressLine1 = currentTenant.TenantAddress1;
                    order.ShipmentAddressLine2 = currentTenant.TenantAddress2;
                    order.ShipmentAddressLine3 = currentTenant.TenantAddress3;
                    order.ShipmentAddressLine4 = currentTenant.TenantAddress4;
                    order.ShipmentAddressPostcode = currentTenant.TenantPostalCode;
                    order.ShipmentAccountAddressId = null;
                }
                else
                {
                    order.ShipmentWarehouseId = int.Parse(shipmentAndRecipientInfo.TenantAddressID.Replace("TenantLoc", ""));
                    var location = _currentDbContext.TenantWarehouses.First(m => m.WarehouseId == order.ShipmentWarehouseId);
                    order.ShipmentAddressLine1 = location.AddressLine1;
                    order.ShipmentAddressLine2 = location.AddressLine2;
                    order.ShipmentAddressLine3 = location.AddressLine3;
                    order.ShipmentAddressPostcode = location.PostalCode;
                    order.ShipmentAccountAddressId = null;

                }
            }
            else if (shipmentAndRecipientInfo.ShipmentDestination.Equals("property", StringComparison.CurrentCultureIgnoreCase))
            {
                order.AccountAddressId = null;
                order.ShipmentPropertyId = shipmentAndRecipientInfo.PPropertyID;
                order.ShipmentAddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1;
                order.ShipmentAddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2;
                order.ShipmentAddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3;
                order.ShipmentAddressLine4 = shipmentAndRecipientInfo.ShipmentAddressLine4;
                order.ShipmentAddressPostcode = shipmentAndRecipientInfo.ShipmentAddressPostcode;
                order.ShipmentAccountAddressId = null;
            }
            else if (shipmentAndRecipientInfo.ShipmentDestination.Equals("account", StringComparison.CurrentCultureIgnoreCase))
            {
                order.AccountAddressId = shipmentAndRecipientInfo.AccountAddressId;
                order.ShipmentAddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1;
                order.ShipmentAddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2;
                order.ShipmentAddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3;
                order.ShipmentAddressLine4 = shipmentAndRecipientInfo.ShipmentAddressLine4;
                order.ShipmentAddressPostcode = shipmentAndRecipientInfo.ShipmentAddressPostcode;
                order.PPropertyId = null;
            }
            else if (order.IsCollectionFromCustomerSide)
            {
                order.AccountAddressId = null;
                order.IsCollectionFromCustomerSide = true;
                order.ShipmentAddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1;
                order.ShipmentAddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2;
                order.ShipmentAddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3;
                order.ShipmentAddressLine4 = shipmentAndRecipientInfo.ShipmentAddressLine4;
                order.ShipmentAddressPostcode = shipmentAndRecipientInfo.ShipmentAddressPostcode;
                order.PPropertyId = null;

            }
            else
            {
                order.AccountAddressId = null;
                order.ShipmentAddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1;
                order.ShipmentAddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2;
                order.ShipmentAddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3;
                order.ShipmentAddressLine4 = shipmentAndRecipientInfo.ShipmentAddressLine4;
                order.ShipmentAddressPostcode = shipmentAndRecipientInfo.ShipmentAddressPostcode;
                order.PPropertyId = null;
                order.ShipmentAccountAddressId = null;
            }

            if (orderDetails != null)
            {
                decimal? ordTotal = 0;
                foreach (var item in orderDetails)
                {
                    item.DateCreated =
                        DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.OrderID = order.OrderID;
                    item.TenentId = tenantId;
                    item.SortOrder = item.ProductMaster?.ProductGroup?.SortOrder ?? 0;
                    item.ProductMaster = null;
                    item.TaxName = null;
                    item.Warranty = null;
                    item.WarehouseId = warehouseId;
                    order.OrderDetails.Add(item);
                    ordTotal = ordTotal + ((item.Price * item.Qty) + item.TaxAmount);
                }



                order.OrderTotal = (decimal)ordTotal;
                if (_orderService.CheckOrdersAuthroization(order.OrderTotal, order.InventoryTransactionTypeId, tenantId, userId))
                {
                    order.OrderStatusID = (int)OrderStatusEnum.AwaitingAuthorisation;
                }

            }

            if (orderNotes != null)
            {
                if (orderNotes.Count == 0)
                {
                    var cItems = _currentDbContext.OrderNotes.Where(a => a.OrderID == order.OrderID && a.IsDeleted != true && a.TenantId == tenantId).ToList();
                    cItems.ForEach(m =>
                    {
                        m.DateUpdated = DateTime.UtcNow;
                        m.UpdatedBy = userId;
                        m.IsDeleted = true;
                    });
                }
                else
                {
                    orderNotes.Where(a => a.OrderNoteId < 0).ToList().ForEach(m =>
                    {
                        m.DateCreated = DateTime.UtcNow;
                        m.CreatedBy = userId;
                        m.TenantId = tenantId;
                        m.OrderID = order.OrderID;
                        order.OrderNotes.Add(m);
                    });
                }
            }







            _currentDbContext.Order.Add(order);
            _currentDbContext.SaveChanges();

            Inventory.StockRecalculateByOrderId(order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return order;
        }

        public Order SavePurchaseOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId, int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {

            order.OrderNumber = order.OrderNumber.Trim();

            order.DateUpdated = DateTime.UtcNow;
            order.UpdatedBy = userId;
            decimal total = 0;

            order.WarehouseId = warehouseId;

            var currentTenant = _currentDbContext.Tenants.Find(tenantId);
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
                    item.OrderID = order.OrderID;
                    item.WarehouseId = warehouseId;
                    total = total + item.TotalAmount;
                    _currentDbContext.OrderDetail.Add(item);
                }

                if (shipmentAndRecipientInfo.ShipmentDestination != null && shipmentAndRecipientInfo.TenantAddressID != null)
                {
                    if (shipmentAndRecipientInfo.ShipmentDestination.Equals("warehouse", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (shipmentAndRecipientInfo.TenantAddressID.Contains("TenantLocMain"))
                        {
                            order.IsShippedToTenantMainLocation = true;
                            order.ShipmentAddressLine1 = currentTenant.TenantAddress1;
                            order.ShipmentAddressLine2 = currentTenant.TenantAddress2;
                            order.ShipmentAddressLine3 = currentTenant.TenantAddress3;
                            order.ShipmentAddressLine4 = currentTenant.TenantAddress4;
                            order.ShipmentAddressPostcode = currentTenant.TenantPostalCode;
                            order.PPropertyId = null;
                            order.ShipmentAccountAddressId = null;
                        }
                        else
                        {
                            order.ShipmentWarehouseId =
                            int.Parse(shipmentAndRecipientInfo.TenantAddressID.Replace(shipmentAndRecipientInfo.TenantAddressID.Contains("TenantLocMain") ? "TenantLocMain" : "TenantLoc", ""));
                            var location = _currentDbContext.TenantWarehouses.First(m => m.WarehouseId == order.ShipmentWarehouseId);
                            order.ShipmentAddressLine1 = location.AddressLine1;
                            order.ShipmentAddressLine2 = location.AddressLine2;
                            order.ShipmentAddressLine3 = location.AddressLine3;
                            order.ShipmentAddressLine4 = location.AddressLine4;
                            order.ShipmentAddressPostcode = location.PostalCode;
                            order.PPropertyId = null;
                            order.ShipmentAccountAddressId = null;
                        }
                    }
                    else if (shipmentAndRecipientInfo.ShipmentDestination.Equals("property",
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        order.ShipmentPropertyId = shipmentAndRecipientInfo.PPropertyID;
                        order.ShipmentAddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1;
                        order.ShipmentAddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2;
                        order.ShipmentAddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3;
                        order.ShipmentAddressLine4 = shipmentAndRecipientInfo.ShipmentAddressLine4;
                        order.ShipmentAddressPostcode = shipmentAndRecipientInfo.ShipmentAddressPostcode;
                        order.ShipmentAccountAddressId = null;
                    }
                    else if (shipmentAndRecipientInfo.ShipmentDestination.Equals("account", StringComparison.CurrentCultureIgnoreCase))
                    {
                        order.AccountAddressId = shipmentAndRecipientInfo.AccountAddressId;
                        order.ShipmentAddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1;
                        order.ShipmentAddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2;
                        order.ShipmentAddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3;
                        order.ShipmentAddressLine4 = shipmentAndRecipientInfo.ShipmentAddressLine4;
                        order.ShipmentAddressPostcode = shipmentAndRecipientInfo.ShipmentAddressPostcode;
                        order.PPropertyId = null;
                        order.ShipmentAccountAddressId = null;
                    }
                    else
                    {
                        order.ShipmentAddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1;
                        order.ShipmentAddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2;
                        order.ShipmentAddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3;
                        order.ShipmentAddressLine4 = shipmentAndRecipientInfo.ShipmentAddressLine4;
                        order.ShipmentWarehouseId = null;
                        order.AccountAddressId = null;
                        order.ShipmentAddressPostcode = shipmentAndRecipientInfo.ShipmentAddressPostcode;
                        order.PPropertyId = null;
                        if (!order.IsCollectionFromCustomerSide)
                        {
                            order.ShipmentAccountAddressId = null;
                        }
                    }
                }
                if (order.IsCollectionFromCustomerSide)
                {
                    order.AccountAddressId = null;
                    order.IsCollectionFromCustomerSide = true;
                    order.ShipmentAddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1;
                    order.ShipmentAddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2;
                    order.ShipmentAddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3;
                    order.ShipmentAddressLine4 = shipmentAndRecipientInfo.ShipmentAddressLine4;
                    order.ShipmentAddressPostcode = shipmentAndRecipientInfo.ShipmentAddressPostcode;
                    order.PPropertyId = null;
                   
                    
                }


                var toDelete = _currentDbContext.OrderDetail.Where(a => a.OrderID == order.OrderID && a.IsDeleted != true).Select(a => a.OrderDetailID).Except(cItems.Select(a => a.OrderDetailID).ToList());
                foreach (var item in toDelete)
                {
                    var dItem = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == item);
                    dItem.IsDeleted = true;
                }
                foreach (var item in cItems)
                {
                    var orderDetail = _currentDbContext.OrderDetail.Find(item.OrderDetailID);

                    orderDetail.ProductId = item.ProductId;
                    orderDetail.ExpectedDate = item.ExpectedDate;
                    orderDetail.Qty = item.Qty;
                    orderDetail.Price = item.Price;
                    orderDetail.WarrantyID = item.WarrantyID;
                    orderDetail.TaxID = item.TaxID;
                    orderDetail.Notes = item.Notes;
                    orderDetail.TaxAmount = item.TaxAmount;
                    orderDetail.TotalAmount = item.TotalAmount;
                    orderDetail.WarrantyAmount = item.WarrantyAmount;

                    _currentDbContext.Entry(orderDetail).State = EntityState.Modified;
                    total = total + item.TotalAmount;
                }

            }
            else
            {
                foreach (var item in _currentDbContext.OrderDetail.Where(a => a.OrderID == order.OrderID && a.IsDeleted != true))
                {
                    item.IsDeleted = true;
                }
            }
            order.OrderTotal = total;

            if (order.AccountID > 0)
            {
                var account = _currentDbContext.Account.Find(order.AccountID);
                if (account != null)
                {
                    order.AccountCurrencyID = account.CurrencyID;
                }
            }

            var obj = _currentDbContext.Order.Find(order.OrderID);
            if (obj != null)
            {
                _currentDbContext.Entry(obj).State = System.Data.Entity.EntityState.Detached;
                order.CreatedBy = obj.CreatedBy;
                order.DateCreated = obj.DateCreated;
                if (order.OrderStatusID == 0)
                {
                    order.OrderStatusID = obj.OrderStatusID;
                }
                order.TenentId = obj.TenentId;
                order.WarehouseId = obj.WarehouseId;
                order.InventoryTransactionTypeId = obj.InventoryTransactionTypeId;
            }

            if (orderNotes != null)
            {
                if (orderNotes.Count == 0)
                {
                    var cItems = _currentDbContext.OrderNotes.Where(a => a.OrderID == order.OrderID && a.IsDeleted != true && a.TenantId == tenantId).ToList();
                    cItems.ForEach(m =>
                    {
                        m.DateUpdated = DateTime.UtcNow;
                        m.UpdatedBy = userId;
                        m.IsDeleted = true;
                    });
                }
                else
                {
                    orderNotes.Where(a => a.OrderNoteId < 0).ToList().ForEach(m =>
                    {
                        m.DateCreated = DateTime.UtcNow;
                        m.CreatedBy = userId;
                        m.TenantId = tenantId;
                        m.OrderID = order.OrderID;
                        _currentDbContext.OrderNotes.Add(m);
                        _currentDbContext.SaveChanges();
                    });
                }
            }
            if (shipmentAndRecipientInfo.SendEmailWithAttachment)
            {
                List<OrderPTenantEmailRecipient> accountids = _currentDbContext.OrderPTenantEmailRecipients.Where(a => a.OrderId == order.OrderID && a.IsDeleted != false).ToList();

                if (accountids.Count > 0)
                {
                    accountids.ForEach(u => u.IsDeleted = true);

                }

                if (shipmentAndRecipientInfo.AccountEmailContacts != null && shipmentAndRecipientInfo.AccountEmailContacts.Length > 0)
                {
                    foreach (var item in shipmentAndRecipientInfo.AccountEmailContacts)
                    {
                        string email = "";

                        var secondryAddress = _currentDbContext.AccountContacts.Where(u => u.AccountContactId == item).AsNoTracking().FirstOrDefault();
                        if (secondryAddress != null)
                        {
                            email = secondryAddress.ContactEmail;
                        }


                        var recipient = new OrderPTenantEmailRecipient()
                        {
                            OrderId = order.OrderID,
                            EmailAddress = email,
                            AccountContactId = item,
                            UpdatedBy = userId,
                            DateUpdated = DateTime.UtcNow,



                        };
                        _currentDbContext.OrderPTenantEmailRecipients.Add(recipient);

                    }

                }

            }
            else
            {
                List<OrderPTenantEmailRecipient> accountids = _currentDbContext.OrderPTenantEmailRecipients.Where(a => a.OrderId == order.OrderID && a.IsDeleted != false).ToList();

                if (accountids.Count > 0)
                {
                    accountids.ForEach(u => u.IsDeleted = true);

                }
            }

            if (_orderService.CheckOrdersAuthroization(order.OrderTotal, order.InventoryTransactionTypeId, tenantId, userId))
            {
                order.OrderStatusID = (int)OrderStatusEnum.AwaitingAuthorisation;
            }
            else
            {
                if (order.OrderStatusID == (int)OrderStatusEnum.AwaitingAuthorisation)
                {
                    if (!caCurrent.CurrentWarehouse().AutoAllowProcess)
                    {
                        order.OrderStatusID = _currentDbContext.OrderStatus.First(a => a.OrderStatusID == (int)OrderStatusEnum.Hold).OrderStatusID;

                    }
                    else
                    {
                        order.OrderStatusID = _currentDbContext.OrderStatus.First(a => a.OrderStatusID == (int)OrderStatusEnum.Active).OrderStatusID;
                    }
                }
            }





            _currentDbContext.Order.Attach(order);
            _currentDbContext.Entry(order).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return order;
        }

        public IQueryable<OrderProcess> GetAllPurchaseOrderProcesses(int tenantId, int warehouseId)
        {
            return _currentDbContext.OrderProcess
                .Where(a => a.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder && a.IsDeleted != true && a.WarehouseId == warehouseId && a.TenentId == tenantId)
                .Include(x => x.Order);
        }

        public IEnumerable<OrderProcessDetail> GetOrderProcessDetailByOrderProcessId(int orderProcessId)
        {
            return _currentDbContext.OrderProcessDetail
                .Where(o => o.OrderProcessId == orderProcessId && o.IsDeleted != true)
                .Include(x => x.ProductMaster);
        }

        public IQueryable<OrderReceiveCount> GetAllGoodsReceiveNotes(int tenantId, int warehouseId)
        {
            return _currentDbContext.OrderReceiveCount
                .Where(a => a.WarehouseId == warehouseId && a.TenantId == tenantId && a.IsDeleted != true)
                .Include(x => x.Order);
        }

        public IQueryable<OrderReceiveCountDetail> GetGoodsReceiveNoteDetailsById(Guid id)
        {
            return _currentDbContext.OrderReceiveCountDetail
                .Where(o => o.ReceiveCountId == id && o.IsDeleted != true)
                .Include(x => x.ProductMaster);


        }


        public Order CreateBlindShipmentOrder(List<BSDto> bsList, int accountId, string deliveryNumber, string poNumber, int tenantId, int warehouseId,
            int userId, InventoryTransactionTypeEnum transType, AccountShipmentInfo accountShipmentInfo = null)
        {

            Order order = new Order
            {
                AccountID = accountId,
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                InventoryTransactionTypeId = transType,
                IssueDate = DateTime.Today,
                WarehouseId = warehouseId,
                Note = "Blind order",
                OrderStatusID = (int)OrderStatusEnum.Complete,
                TenentId = tenantId,
                OrderNumber = poNumber,
                ShipmentWarehouseId = warehouseId

            };
            if (accountShipmentInfo != null)
            {
                order.ShipmentAddressLine1 = accountShipmentInfo.ShipmentAddressLine1;
                order.ShipmentAddressLine2 = accountShipmentInfo.ShipmentAddressLine2;
                order.ShipmentAddressLine3 = accountShipmentInfo.ShipmentAddressLine3;
                order.ShipmentAddressLine4 = accountShipmentInfo.ShipmentAddressLine4;
                order.ShipmentAddressPostcode = accountShipmentInfo.ShipmentAddressPostcode;
            }
            _currentDbContext.Order.Add(order);
            _currentDbContext.SaveChanges();
            foreach (var item in bsList)
            {


                OrderDetail detail = new OrderDetail
                {
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    Notes = "Blind order",
                    ProductId = item.ProductId,
                    TenentId = tenantId,
                    WarehouseId = warehouseId,
                    ProductGroupId = item.ProductGroupId
                };
                if (item.IsSerial == true) { detail.Qty = (item.Quantity ?? 1); }
                else
                {

                    int productcasess = 1;
                    var datas = _currentDbContext.ProductMaster.FirstOrDefault(u => u.ProductId == item.ProductId && u.ProcessByPallet == true);
                    if (datas != null)
                    {
                        productcasess = datas.ProductsPerCase ?? 1;
                        detail.Qty = (item.Quantity ?? 1) * productcasess;
                    }
                    else { detail.Qty = (item.Quantity ?? 1); }

                }

                order.OrderDetails.Add(detail);


                // _currentDbContext.SaveChanges();
                // process order at same time
                OrderProcess oprocess = null;
                caUser user = caCurrent.CurrentUser();
                oprocess = _orderService.GetOrderProcessByDeliveryNumber(order.OrderID, transType, deliveryNumber, userId, warehouseId: warehouseId, shipmentInfo: accountShipmentInfo);

                int productcases = 1;
                var data = _currentDbContext.ProductMaster.FirstOrDefault(u => u.ProductId == item.ProductId && u.ProcessByPallet == true);
                if (data != null)
                {
                    productcases = data.ProductsPerCase ?? 1;
                }
                int? serialId = null;
                if (item.LocationId >= 0) { item.LocationId = null; }

                OrderProcessDetail odet = new OrderProcessDetail
                {
                    CreatedBy = user.UserId,
                    DateCreated = DateTime.UtcNow,
                    OrderProcessId = oprocess.OrderProcessID,
                    ProductId = item.ProductId,
                    TenentId = user.TenantId,
                    OrderDetailID = detail.OrderDetailID,
                    ID = item.ProductDesc,
                    FscPercent = item.FscPercent



                };
                if (item.IsSerial == true) { odet.QtyProcessed = (item.Quantity ?? 1); }
                else
                {
                    if (data != null)
                    {
                        odet.QtyProcessed = (item.Quantity ?? 1) * productcases;
                    }
                    else { odet.QtyProcessed = (item.Quantity ?? 1); }

                }
                _currentDbContext.OrderProcessDetail.Add(odet);
                _currentDbContext.SaveChanges();

                if (item.IsSerial == true)
                {
                    ProductSerialis serial = null;
                    serial = _currentDbContext.ProductSerialization.FirstOrDefault(a => a.SerialNo == item.Serial && a.CurrentStatus != (InventoryTransactionTypeEnum)transType);
                    if (serial == null)
                    {

                        serial = new ProductSerialis
                        {
                            CreatedBy = caCurrent.CurrentUser().UserId,
                            DateCreated = DateTime.UtcNow,
                            CurrentStatus = (InventoryTransactionTypeEnum)transType,
                            LocationId = item.LocationId,
                            ProductId = item.ProductId,
                            TenentId = user.TenantId,
                            WarehouseId = warehouseId,
                            SerialNo = item.Serial,
                        };

                        if (serial != null)
                        {
                            var orderDetail = _currentDbContext.OrderDetail.First(m => m.OrderID == order.OrderID && m.ProductId == item.ProductId);
                            var warrantyInfo = orderDetail.Warranty;
                            serial.SoldWarrantyStartDate = DateTime.UtcNow;
                            serial.DateUpdated = DateTime.UtcNow;
                            if (warrantyInfo != null)
                            {
                                serial.SoldWarrentyEndDate = DateTime.UtcNow.AddDays(warrantyInfo.WarrantyDays);
                                serial.PostageTypeId = warrantyInfo.OrderConsignmentTypes.ConsignmentTypeId;
                                serial.SoldWarrantyIsPercent = warrantyInfo.IsPercent;
                                serial.SoldWarrantyName = warrantyInfo.WarrantyName;
                                serial.SoldWarrantyPercentage = warrantyInfo.PercentageOfPrice;
                                serial.SoldWarrantyFixedPrice = warrantyInfo.FixedPrice;
                            }

                        }

                        _currentDbContext.ProductSerialization.Add(serial);
                        _currentDbContext.SaveChanges();
                    }
                    else
                    {
                        serial.CurrentStatus = (InventoryTransactionTypeEnum)transType;
                        serial.DateUpdated = DateTime.UtcNow;
                        serial.UpdatedBy = userId;
                        serial.TenentId = user.TenantId;
                        serial.LocationId = item.LocationId;
                        serial.WarehouseId = warehouseId;
                        _currentDbContext.ProductSerialization.Attach(serial);
                        _currentDbContext.Entry(serial).State = EntityState.Modified;

                        _currentDbContext.SaveChanges();
                    }
                    serialId = serial.SerialID;
                    //create inventory transaction
                    Inventory.StockTransaction(item.ProductId, transType, item.Quantity ?? 1, order.OrderID, item.LocationId, deliveryNumber, serialId, orderprocessId: (oprocess?.OrderProcessID), orderProcessDetailId: (odet?.OrderProcessDetailID));

                }

                else
                {
                    if (data != null)
                    {
                        int PalletTrackingId = 0;

                        if (!string.IsNullOrEmpty(item.Serial))
                        {
                            PalletTrackingId = int.Parse(item.Serial);

                        }
                        var newpallet = _currentDbContext.PalletTracking.Find(PalletTrackingId);
                        if (newpallet != null)
                        {
                            if (transType == InventoryTransactionTypeEnum.PurchaseOrder)
                            {
                                newpallet.Status = PalletTrackingStatusEnum.Active;
                                newpallet.RemainingCases = newpallet.RemainingCases;
                            }
                            else
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases - newpallet.RemainingCases;

                                if (transType == InventoryTransactionTypeEnum.SalesOrder && newpallet.RemainingCases == 0)
                                {
                                    newpallet.Status = PalletTrackingStatusEnum.Completed;
                                }

                            }
                        }

                        _currentDbContext.Entry(newpallet).State = EntityState.Modified;
                        _currentDbContext.SaveChanges();

                        Inventory.StockTransaction(item.ProductId, transType, item.Quantity ?? 1, order.OrderID, item.LocationId, deliveryNumber, serialId, orderprocessId: (oprocess?.OrderProcessID), orderProcessDetailId: (odet?.OrderProcessDetailID));
                    }
                    else
                    {
                        Inventory.StockTransaction(item.ProductId, transType, item.Quantity ?? 1, order.OrderID, item.LocationId, deliveryNumber, serialId, orderprocessId: (oprocess?.OrderProcessID), orderProcessDetailId: (odet?.OrderProcessDetailID));
                    }
                }

            }

            return order;
        }

        public List<OrderDetailsViewModel> GetPurchaseOrderDetailsById(int orderId, int tenantId)
        {
            var model = _currentDbContext.OrderDetail.Where(a => a.OrderID == orderId && a.IsDeleted != true && a.TenentId == tenantId).ToList();

            return model.Select(ord => new OrderDetailsViewModel()
            {
                OrderID = ord.OrderID,
                ProductId = ord.ProductId,
                ProductMaster = ord.ProductMaster,
                OrderDetailID = ord.OrderDetailID,
                DirectShip = ord.Order.DirectShip,
                Product = ord.ProductMaster.Name,
                SkuCode = ord.ProductMaster.SKUCode,
                Barcode = ord.ProductMaster.BarCode,
                BarCode2 = ord.ProductMaster.BarCode2,
                Qty = ord.Qty,
                Price = ord.Price,
                TotalWarrantyAmount = ord.WarrantyAmount * ord.Qty,
                WarrantyAmount = ord.WarrantyAmount,
                TaxName = ord.TaxName,
                TaxAmount = ord.TaxAmount,
                ExpectedDate = ord.ExpectedDate,
                TotalAmount = ord.TotalAmount,
                TransType = ord.Order.InventoryTransactionTypeId,
                EnableGlobalProcessByPallet = ord.TenantWarehouse.EnableGlobalProcessByPallet,
                QtyReceived = ord.ProcessedQty,
                DirectPostAllowed = !ord.ProductMaster.Serialisable && !ord.ProductMaster.ProcessByPallet && ord.ProductMaster.RequiresBatchNumberOnReceipt != true && ord.ProductMaster.RequiresExpiryDateOnReceipt != true
            }).ToList();
        }

        public InventoryTransaction SubmitReceiveInventoryTransaction(InventoryTransaction model, string deliveryNumber, int tenantId, int warehouseId, int userId)
        {
            int? serialId = null;
            var oprocess = _orderService.GetOrderProcessByDeliveryNumber(model.OrderID ?? 0, model.InventoryTransactionTypeId, deliveryNumber, userId, null, warehouseId);

            var odet = new OrderProcessDetail
            {
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                OrderProcessId = oprocess.OrderProcessID,
                ProductId = model.ProductId,
                TenentId = tenantId,
                QtyProcessed = model.Quantity,
            };
            _currentDbContext.OrderProcessDetail.Add(odet);



            if (model.SerialID.HasValue && model.SerialID > 0)
            {
                var serial = _currentDbContext.ProductSerialization.Find(model.SerialID.Value);
                if (serial != null)
                {
                    serialId = serial.SerialID;
                    serial.CurrentStatus = (InventoryTransactionTypeEnum)model.InventoryTransactionTypeId;
                    serial.DateUpdated = DateTime.UtcNow;
                    serial.UpdatedBy = userId;
                    _currentDbContext.Entry(serial).State = EntityState.Modified;
                }
            }

            _currentDbContext.SaveChanges();

            Inventory.StockTransaction(model.ProductId, model.InventoryTransactionTypeId, model.Quantity, model.OrderID, model.LocationId, deliveryNumber, serialId, orderprocessId: (oprocess?.OrderProcessID), orderProcessDetailId: (odet?.OrderProcessDetailID));

            return model;
        }

        public Order UpdatePurchaseOrderStatus(int orderId, int orderStatusId, int userId)
        {
            var order = _orderService.GetOrderById(orderId);

            var status = _orderService.GetOrderstatusById(orderStatusId);

            if (status.OrderStatusID == (int)OrderStatusEnum.Complete)
            {
                var pd = order.OrderDetails.Where(x => x.IsDeleted != true);
                foreach (var item in pd)
                {

                    decimal qtyrec = _currentDbContext.OrderProcessDetail.Where(x => x.OrderProcessDetailID == item.OrderDetailID && x.IsDeleted != true).Sum(x => (decimal?)x.QtyProcessed) ?? 0;

                    if (item.Qty > qtyrec)  //////update POLines
                    {   //////////////  
                        item.DateUpdated = DateTime.UtcNow;
                        item.UpdatedBy = userId;

                        /////////////
                        _currentDbContext.OrderDetail.Attach(item);
                        var entry = _currentDbContext.Entry(item);
                        entry.Property(e => e.DateUpdated).IsModified = true;
                        entry.Property(e => e.UpdatedBy).IsModified = true;
                        _currentDbContext.SaveChanges();

                    }  //end of inner if


                } //end of loop


            }

            order.OrderStatusID = orderStatusId;
            order.DateUpdated = DateTime.UtcNow;
            order.UpdatedBy = userId;
            _currentDbContext.Order.Attach(order);
            var entry1 = _currentDbContext.Entry(order);
            entry1.Property(e => e.OrderStatusID).IsModified = true;
            entry1.Property(e => e.DateUpdated).IsModified = true;
            entry1.Property(e => e.UpdatedBy).IsModified = true;

            _currentDbContext.SaveChanges();
            return order;
        }

        public Order CancelPurchaseOrder(int orderId, int userId, int warehouseId)
        {
            var order = _orderService.GetOrderById(orderId);
            order.DateUpdated = DateTime.UtcNow;
            order.UpdatedBy = userId;
            order.CancelDate = DateTime.UtcNow;
            order.IsCancel = true;
            order.CancelBy = userId;
            order.OrderStatusID = (int)OrderStatusEnum.NotScheduled;
            order.WarehouseId = warehouseId;
            _currentDbContext.Order.Attach(order);
            var entry1 = _currentDbContext.Entry(order);

            entry1.Property(e => e.DateUpdated).IsModified = true;
            entry1.Property(e => e.UpdatedBy).IsModified = true;
            entry1.Property(e => e.CancelDate).IsModified = true;
            entry1.Property(e => e.CancelBy).IsModified = true;
            entry1.Property(e => e.IsCancel).IsModified = true;
            entry1.Property(e => e.OrderStatusID).IsModified = true;
            _currentDbContext.SaveChanges();
            return order;
        }

        public IQueryable<PurchaseOrderViewModel> GetAllPurchaseOrdersInProgress(int tenantId, int warehouseId)
        {
            return _currentDbContext.Order.AsNoTracking().Where(o => o.TenentId == tenantId && o.WarehouseId == warehouseId && o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder
            && (o.OrderStatusID == (int)OrderStatusEnum.Active || o.OrderStatusID == (int)OrderStatusEnum.Hold || o.OrderStatusID == (int)OrderStatusEnum.BeingPicked || o.OrderStatusID == (int)OrderStatusEnum.AwaitingAuthorisation) && o.IsDeleted != true)
                .Select(p => new PurchaseOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    DateCreated = p.DateCreated,
                    DateUpdated = p.DateUpdated,
                    POStatus = p.OrderStatus.Status,
                    OrderStatusID = p.OrderStatusID,
                    Account = p.Account.AccountCode,
                    AccountName = p.Account.CompanyName,
                    PickerName=p.Picker==null?"" :p.Picker.UserLastName + ", " + p.Picker.UserFirstName,
                    Currecny = p.AccountCurrency.CurrencyName,
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                    Property = _currentDbContext.PProperties.Where(s => s.PPropertyId == p.ShipmentPropertyId)
                        .Select(s => s.AddressLine1)
                        .FirstOrDefault(),
                    OrderTotal = p.OrderTotal,


                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                    {
                        OrderNoteId = s.OrderNoteId,
                        Notes = s.Notes,
                        NotesByName = p.Tenant.AuthUsers.FirstOrDefault(x => x.UserId == s.CreatedBy).UserName,
                        NotesDate = s.DateCreated
                    }).ToList(),


                });

        }

        public IQueryable<PurchaseOrderViewModel> GetAllPurchaseOrdersCompleted(int tenantId, int warehouseId, int? type = null)
        {

            return _currentDbContext.Order.AsNoTracking().Where(o => o.TenentId == tenantId && o.WarehouseId == warehouseId && ((type.HasValue && o.OrderStatusID == type) || !type.HasValue) && o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder
             && o.IsDeleted != true)
           .Select(p => new PurchaseOrderViewModel()
           {
               OrderID = p.OrderID,
               OrderNumber = p.OrderNumber,
               IssueDate = p.IssueDate,
               DateCreated = p.DateCreated,
               DateUpdated = p.DateUpdated,
               POStatus = p.OrderStatus.Status,
               OrderStatusID = p.OrderStatusID,
               Account = p.Account.AccountCode,
               AccountName = p.Account.CompanyName,
               Currecny = p.AccountCurrency.CurrencyName,
               InvoiceNo = p.InvoiceNo,
               EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
               InvoiceDetails = p.InvoiceDetails,
               OrderCost = p.OrderCost,
               OrderTotal = p.OrderTotal,
               Property = _currentDbContext.PProperties.Where(s => s.PPropertyId == p.ShipmentPropertyId)
                   .Select(s => s.AddressLine1)
                   .FirstOrDefault(),
               OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
               {
                   OrderNoteId = s.OrderNoteId,
                   Notes = s.Notes,
                   NotesByName = p.Tenant.AuthUsers.FirstOrDefault(x => x.UserId == s.CreatedBy).UserName,
                   NotesDate = s.DateCreated
               }).ToList()

           });

        }

        public OrderDetail GetOrderDetailByOrderId(int orderId, int TenentId)
        {
            return _currentDbContext.OrderDetail.AsNoTracking().FirstOrDefault(o => o.OrderID == orderId && o.TenentId == TenentId && o.IsDeleted != true);

        }

        public PalletTracking GetVerifedPallet(string serial, int productId, int tenantId, int warehouseId, int? type = null, int? palletTrackingId = null, DateTime? dates = null, int? orderId = null)
        {
            if (type.HasValue)
            {
                switch (type)
                {
                    case (int)InventoryTransactionTypeEnum.AdjustmentOut:
                    case (int)InventoryTransactionTypeEnum.AdjustmentIn:
                        if (type == (int)InventoryTransactionTypeEnum.AdjustmentOut)
                        { return _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0); }
                        return _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId);
                    case (int)InventoryTransactionTypeEnum.Returns:
                        return _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.Status != PalletTrackingStatusEnum.Created);
                    case (int)InventoryTransactionTypeEnum.Samples:
                        return _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.Status == PalletTrackingStatusEnum.Created);

                }
                var shelflife = _currentDbContext.Order.Find(orderId)?.Account?.AcceptedShelfLife;
                var expiryDate = _currentDbContext.PalletTracking.FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId)?.ExpiryDate;
                var days =  (expiryDate.Value - DateTime.Today).TotalDays;
                if (days <= shelflife)
                {
                    PalletTracking palletTracking = new PalletTracking();
                    palletTracking.Comments = "[#Expired#]";
                    return palletTracking;
                }
                if (palletTrackingId.HasValue && palletTrackingId == (int)PalletTrackingSchemeEnum.ByExpiryMonth)
                {

                    var month = dates.Value.Month;
                    var year = dates.Value.Year;
                    return _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && (u.OrderId == null || (orderId.HasValue && u.OrderId == orderId)) && u.Status == PalletTrackingStatusEnum.Active && u.ExpiryDate.Value.Month == month && u.ExpiryDate.Value.Year == year);
                }
                if (palletTrackingId.HasValue && palletTrackingId == (int)PalletTrackingSchemeEnum.ByExpiryDate)
                {
                    return _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && (u.OrderId == null || (orderId.HasValue && u.OrderId == orderId)) && u.Status == PalletTrackingStatusEnum.Active && u.ExpiryDate == dates);
                }
                else
                {
                    return _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && (u.OrderId == null || (orderId.HasValue && u.OrderId == orderId)) && u.Status == PalletTrackingStatusEnum.Active && u.RemainingCases > 0);
                }
            }
            return _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == serial && u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.Status == PalletTrackingStatusEnum.Created);

        }



        public int ProcessPalletTrackingSerial(GoodsReturnRequestSync serials, string groupToken = null, bool process = false)
        {
            try
            {
                if (serials.MissingTrackingNo == true)
                {

                    return Inventory.StockTransaction(serials, groupToken);
                }
                caCurrent caCurrent = new caCurrent();
                decimal totalquantity = 0;
                var product = _productServices.GetProductMasterById(serials.ProductId);
                if ((serials.OrderId <= 0 && process && serials.InventoryTransactionType == InventoryTransactionTypeEnum.Wastage) || serials.InventoryTransactionType == InventoryTransactionTypeEnum.AdjustmentIn || serials.InventoryTransactionType == InventoryTransactionTypeEnum.AdjustmentOut)
                {
                    int? orderId = serials.OrderId;
                    orderId = orderId == 0 ? null : orderId;
                    if (serials.PalleTrackingProcess.Count <= 0 && serials.PalletTrackingId > 0)
                    {

                        var newpallet = _currentDbContext.PalletTracking.Find(serials.PalletTrackingId);
                        var products = _currentDbContext.ProductMaster.Find(newpallet.ProductId);

                        if (serials.InventoryTransactionType == InventoryTransactionTypeEnum.AdjustmentIn)
                        {
                            if (newpallet.Status == PalletTrackingStatusEnum.Created)
                            {
                                newpallet.Status = PalletTrackingStatusEnum.Active;
                            }
                            newpallet.DateUpdated = DateTime.UtcNow;
                            _currentDbContext.PalletTracking.Add(newpallet);
                            _currentDbContext.Entry(newpallet).State = EntityState.Modified;
                            _currentDbContext.SaveChanges();
                            Inventory.StockTransactionApi(newpallet.ProductId, serials.InventoryTransactionType ?? 0, (newpallet.RemainingCases * (products.ProductsPerCase ?? 1)), orderId, serials.tenantId, serials.warehouseId, serials.userId, null, serials.PalletTrackingId, null, groupToken);
                        }

                        return 0;
                    }

                    foreach (var pallet in serials.PalleTrackingProcess)
                    {
                        decimal qty = 0;

                        int? PalletTrackingId = 0;
                        PalletTrackingId = pallet.PalletTrackingId;
                        qty = pallet.ProcessedQuantity;
                        var newpallet = _currentDbContext.PalletTracking.Find(pallet.PalletTrackingId);
                        if (serials.InventoryTransactionType == InventoryTransactionTypeEnum.AdjustmentIn)
                        {
                            if (newpallet.Status == PalletTrackingStatusEnum.Active)
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases + qty;
                            }
                            else
                            {
                                newpallet.RemainingCases = qty;
                                newpallet.Status = PalletTrackingStatusEnum.Active;
                            }
                        }
                        else
                        {
                            if (newpallet.RemainingCases > qty)
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases - qty;


                                newpallet.Status = PalletTrackingStatusEnum.Active;


                            }
                            else
                            {
                                newpallet.RemainingCases = 0;
                                newpallet.Status = PalletTrackingStatusEnum.Completed;
                            }
                        }

                        newpallet.DateUpdated = DateTime.UtcNow;
                        _currentDbContext.PalletTracking.Add(newpallet);
                        _currentDbContext.Entry(newpallet).State = EntityState.Modified;
                        _currentDbContext.SaveChanges();
                        Inventory.StockTransactionApi(serials.ProductId, serials.InventoryTransactionType ?? 0, (qty * (product.ProductsPerCase ?? 1)), orderId, serials.tenantId, serials.warehouseId, serials.userId, null, PalletTrackingId, null, groupToken);

                    }
                    return serials.OrderId;

                }
                else if (process)
                {
                    int? lineId = 0;
                    var cOrder = _currentDbContext.Order.Find(serials.OrderId);
                    if (serials.PalleTrackingProcess.Count > 0)
                    {
                        totalquantity = serials.PalleTrackingProcess.Sum(u => u.ProcessedQuantity);
                    }
                    if (product != null)
                    {
                        serials.Quantity = totalquantity * product.ProductsPerCase ?? 1;
                    }
                    else { serials.Quantity = totalquantity; }

                    if (serials.OrderId <= 0 && serials.InventoryTransactionType > 0)
                    {
                        cOrder = _orderService.CreateOrderByOrderNumber(serials.OrderNumber, serials.ProductId, serials.tenantId, serials.warehouseId, serials.InventoryTransactionType ?? 0, serials.userId,(serials.Quantity??1));
                    }

                    if (serials.OrderDetailID == null || serials.OrderDetailID < 1)
                    {
                        serials.OrderDetailID = cOrder.OrderDetails.Where(u => u.ProductId == serials.ProductId).FirstOrDefault()?.OrderDetailID;
                    }
                    if (cOrder.OrderID > 0)
                    {
                        


                        var oprocess = _orderService.GetOrderProcessByDeliveryNumber(cOrder.OrderID, serials.InventoryTransactionType ?? 0, serials.deliveryNumber, serials.userId, warehouseId: serials.warehouseId);

                        if (lineId == null || lineId < 1)
                        {
                            lineId = serials.OrderDetailID;
                        }
                        var xopr = new OrderProcessDetail()
                        {
                            CreatedBy = serials.userId,
                            DateCreated = DateTime.UtcNow,
                            OrderProcessId = oprocess.OrderProcessID,
                            ProductId = serials.ProductId,
                            TenentId = serials.tenantId,
                            QtyProcessed = serials.Quantity ?? 0,
                            OrderDetailID = lineId
                        };

                        _currentDbContext.OrderProcessDetail.Add(xopr);
                        _currentDbContext.SaveChanges();
                        foreach (var pallet in serials.PalleTrackingProcess)
                        {
                            decimal qty = 0;

                            int PalletTrackingId = 0;
                            PalletTrackingId = pallet.PalletTrackingId;
                            qty = pallet.ProcessedQuantity;

                            var newpallet = _currentDbContext.PalletTracking.Find(PalletTrackingId);
                            newpallet.Status = PalletTrackingStatusEnum.Active;
                            if (serials.InventoryTransactionType == InventoryTransactionTypeEnum.AdjustmentIn || serials.InventoryTransactionType == InventoryTransactionTypeEnum.AdjustmentOut)
                            {
                                if (newpallet.TotalCases < qty)
                                {
                                    newpallet.TotalCases = newpallet.TotalCases + qty;
                                }
                            }


                            else if (serials.InventoryTransactionType == InventoryTransactionTypeEnum.Returns)
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases + qty;
                            }
                            else if (serials.InventoryTransactionType == InventoryTransactionTypeEnum.WastedReturn) { }
                            else
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases - qty;
                            }

                            totalquantity += qty;

                            newpallet.DateUpdated = DateTime.UtcNow;
                            _currentDbContext.PalletTracking.Add(newpallet);
                            _currentDbContext.Entry(newpallet).State = EntityState.Modified;
                            _currentDbContext.SaveChanges();
                            Inventory.StockTransactionApi(serials.ProductId, serials.InventoryTransactionType ?? 0, (qty * (product.ProductsPerCase ?? 1)), cOrder.OrderID, serials.tenantId, serials.warehouseId, serials.userId, null, PalletTrackingId, null, groupToken, orderProcessId: (oprocess?.OrderProcessID), OrderProcessDetialId: (xopr?.OrderProcessDetailID));


                        }
                        return cOrder.OrderID;


                    }

                    else
                    {
                        if (serials.PalleTrackingProcess.Count > 0)
                        {
                            totalquantity = serials.PalleTrackingProcess.Sum(u => u.ProcessedQuantity);
                        }
                        if (product != null)
                        {
                            serials.Quantity = totalquantity * (product.ProductsPerCase ?? 1);
                        }
                        else { serials.Quantity = totalquantity; }

                        if (lineId == null || lineId < 1)
                        {
                            lineId = cOrder.OrderDetails.FirstOrDefault(u => u.ProductId == serials.ProductId)?.OrderDetailID;
                        }

                        var oprocess = _orderService.GetOrderProcessByDeliveryNumber(cOrder.OrderID, serials.InventoryTransactionType ?? 0, serials.deliveryNumber, serials.userId, warehouseId: serials.warehouseId);


                        var xopr = new OrderProcessDetail()
                        {
                            CreatedBy = serials.userId,
                            DateCreated = DateTime.UtcNow,
                            OrderProcessId = oprocess.OrderProcessID,
                            ProductId = serials.ProductId,
                            TenentId = serials.tenantId,
                            QtyProcessed = serials.Quantity ?? 0,
                            OrderDetailID = lineId
                        };

                        _currentDbContext.OrderProcessDetail.Add(xopr);
                        _currentDbContext.SaveChanges();


                        foreach (var pallet in serials.PalleTrackingProcess)
                        {
                            decimal qty = 0;

                            int PalletTrackingId = 0;

                            PalletTrackingId = pallet.PalletTrackingId;
                            qty = pallet.ProcessedQuantity;

                            var newpallet = _currentDbContext.PalletTracking.Find(PalletTrackingId);
                            newpallet.Status = PalletTrackingStatusEnum.Active;

                            if (serials.InventoryTransactionType == InventoryTransactionTypeEnum.Returns)
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases + qty;
                            }



                            totalquantity += qty;

                            newpallet.DateUpdated = DateTime.UtcNow;
                            _currentDbContext.PalletTracking.Add(newpallet);
                            _currentDbContext.Entry(newpallet).State = EntityState.Modified;
                            _currentDbContext.SaveChanges();
                            Inventory.StockTransactionApi(serials.ProductId, serials.InventoryTransactionType ?? 0, (qty * (product.ProductsPerCase ?? 1)), cOrder.OrderID, serials.tenantId, serials.warehouseId, serials.userId, null, PalletTrackingId, null, groupToken, orderProcessId: (oprocess?.OrderProcessID), OrderProcessDetialId: (xopr?.OrderProcessDetailID));


                        }

                        return cOrder.OrderID;
                    }

                }
                else if (!process)
                {
                    var orderProcess = _orderService.GetOrderProcessByDeliveryNumber(serials.OrderId, serials.InventoryTransactionType ?? 0, serials.deliveryNumber, serials.tenantId, null, serials.warehouseId);
                    foreach (var pallet in serials.PalleTrackingProcess)
                    {
                        decimal quantity = 0;
                        int PalletTrackingId = 0;
                        PalletTrackingId = pallet.PalletTrackingId;
                        quantity = pallet.ProcessedQuantity;
                        var newpallet = _currentDbContext.PalletTracking.Find(PalletTrackingId);

                        if (!serials.InventoryTransactionType.HasValue)
                        {
                            newpallet.RemainingCases = quantity;
                        }
                        else
                        {
                            newpallet.RemainingCases = newpallet.RemainingCases - quantity;
                        }
                        if (serials.InventoryTransactionType.HasValue && newpallet.RemainingCases == 0)
                        {


                            newpallet.Status = PalletTrackingStatusEnum.Completed;
                        }

                        else { newpallet.Status = PalletTrackingStatusEnum.Active; }
                        newpallet.DateUpdated = DateTime.UtcNow;
                        _currentDbContext.PalletTracking.Add(newpallet);
                        _currentDbContext.Entry(newpallet).State = EntityState.Modified;

                        var o = new OrderProcessDetail()
                        {
                            CreatedBy = serials.userId,
                            DateCreated = DateTime.UtcNow,
                            UpdatedBy = serials.userId,

                            ProductId = serials.ProductId,
                            QtyProcessed = Math.Round(quantity * (product.ProductsPerCase ?? 1),2),
                            TenentId = serials.tenantId,
                            OrderProcessId = orderProcess.OrderProcessID,
                            OrderDetailID = serials.OrderDetailID,
                            IsDeleted = false

                        };


                        _currentDbContext.OrderProcessDetail.Add(o);
                        _currentDbContext.Entry(o).State = EntityState.Added;
                        _currentDbContext.SaveChanges();
                        if (!serials.InventoryTransactionType.HasValue)
                        {
                            Inventory.StockTransactionApi(serials.ProductId, InventoryTransactionTypeEnum.PurchaseOrder, Math.Round(quantity * (product.ProductsPerCase ?? 1),2), serials.OrderId,
                                serials.tenantId, serials.warehouseId, serials.userId, null, PalletTrackingId, null, groupToken,
                                orderProcessId: orderProcess?.OrderProcessID, OrderProcessDetialId: o?.OrderProcessDetailID);
                        }
                        else
                        {
                            if (serials.InventoryTransactionType == InventoryTransactionTypeEnum.WorksOrder)
                            {
                                Inventory.StockTransactionApi(serials.ProductId, InventoryTransactionTypeEnum.WorksOrder, Math.Round(quantity * (product.ProductsPerCase ?? 1),2), serials.OrderId, serials.tenantId, serials.warehouseId,
                                    serials.userId, null, PalletTrackingId, null, groupToken,
                                    orderProcessId: orderProcess?.OrderProcessID, OrderProcessDetialId: o?.OrderProcessDetailID);

                            }
                            else
                            {
                                Inventory.StockTransactionApi(serials.ProductId, InventoryTransactionTypeEnum.SalesOrder, Math.Round(quantity * (product.ProductsPerCase ?? 1),2), serials.OrderId, serials.tenantId, serials.warehouseId, serials.userId, null, PalletTrackingId, null,
                                    groupToken, orderProcessId: orderProcess?.OrderProcessID, OrderProcessDetialId: o?.OrderProcessDetailID);
                            }
                        }
                    }

                    return serials.OrderId;
                }

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
