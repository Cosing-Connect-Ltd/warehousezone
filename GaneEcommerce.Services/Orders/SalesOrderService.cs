using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IGaneConfigurationsHelper _configHelper;
        private readonly IAccountServices _accountServices;
        private readonly IOrderService _orderService;

        public SalesOrderService(IApplicationContext currentDbContext, IGaneConfigurationsHelper configHelper, IAccountServices accountServices, OrderService orderService)
        {
            _currentDbContext = currentDbContext;
            _configHelper = configHelper;
            _accountServices = accountServices;
            _orderService = orderService;
        }

        public Order CreateSalesOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            order.OrderNumber = order.OrderNumber.Trim();
            double? AccountCredit = 0;
            var duplicateOrder = _currentDbContext.Order.FirstOrDefault(m => m.OrderNumber.Equals(order.OrderNumber, StringComparison.CurrentCultureIgnoreCase));
            if (duplicateOrder != null)
            {
                throw new Exception($"Order Number {order.OrderNumber} already associated with another Order. Please regenerate order number.", new Exception("Duplicate Order Number"));
            }

            order.IssueDate = DateTime.UtcNow;
            order.OrderStatusID = (int)OrderStatusEnum.Active;
            order.DateCreated = DateTime.UtcNow;
            order.DateUpdated = DateTime.UtcNow;
            order.TenentId = tenantId;
            order.CreatedBy = userId;
            order.UpdatedBy = userId;
            order.WarehouseId = warehouseId;
            if (order.AccountID > 0)
            {
                var account = _currentDbContext.Account.Find(order.AccountID);
                if (account != null)
                {
                    order.AccountCurrencyID = account.CurrencyID;
                    AccountCredit = account.CreditLimit;
                }
            }
            if (!caCurrent.CurrentWarehouse().AutoAllowProcess)
            {
                order.OrderStatusID = _currentDbContext.OrderStatus.First(a => a.OrderStatusID == (int)OrderStatusEnum.Hold).OrderStatusID;

            }
            else
            {
                order.OrderStatusID = _currentDbContext.OrderStatus.First(a => a.OrderStatusID == (int)OrderStatusEnum.Active).OrderStatusID;
            }
            _currentDbContext.Order.Add(order);


            if (orderDetails != null)
            {
                if (orderDetails.Any(m => m.OrderDetailStatusId == (int)OrderStatusEnum.AwaitingAuthorisation))
                {
                    order.OrderStatusID = (int)OrderStatusEnum.AwaitingAuthorisation;
                }

                decimal? ordTotal = 0;
                foreach (var item in orderDetails)
                {
                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.OrderID = order.OrderID;
                    item.TenentId = tenantId;
                    item.SortOrder = item.ProductMaster?.ProductGroup?.SortOrder ?? 0;
                    item.ProductMaster = null;
                    item.TaxName = null;
                    item.WarehouseId = warehouseId;
                    order.OrderDetails.Add(item);
                    ordTotal = ordTotal + ((item.Price * item.Qty) + item.TaxAmount + item.WarrantyAmount);

                }
                order.OrderCost = ordTotal;
                order.OrderDiscount = _orderService.GetDiscountOnTotalCost(order.AccountID??0, ordTotal??0);
                order.OrderTotal = (decimal)ordTotal-order.OrderDiscount;

                if (_orderService.CheckOrdersAuthroization(order.OrderTotal, order.InventoryTransactionTypeId, tenantId, userId, AccountCredit))
                {
                    order.OrderStatusID = (int)OrderStatusEnum.AwaitingAuthorisation;
                }


            }
            if (orderNotes != null)
            {
                foreach (var item in orderNotes)
                {
                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.OrderID = order.OrderID;
                    item.TenantId = tenantId;
                    order.OrderNotes.Add(item);
                }
            }
            if (shipmentAndRecipientInfo.AddAddressToAccount == true && order.AccountID > 0 && (!order.ShipmentAccountAddressId.HasValue || order.ShipmentAccountAddressId < 1))
            {
                var account = _accountServices.GetAccountsById(order.AccountID.Value);
                var accountAddress = new AccountAddresses()
                {
                    AccountID = order.AccountID.Value,
                    AddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1,
                    AddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2,
                    AddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3,
                    Town = shipmentAndRecipientInfo.ShipmentAddressLine4,
                    PostCode = shipmentAndRecipientInfo.ShipmentAddressPostcode,
                    AddTypeShipping = true,
                    CountryID = account.CountryID,
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                    Name = account.CompanyName
                };
                _currentDbContext.AccountAddresses.Add(accountAddress);
            }
            if (order.ShipmentAccountAddressId < 1)
            {
                order.ShipmentAccountAddressId = (int?)null;
            }

            if (order.ConsignmentTypeId == 4)
            {
                order.ShipmentAddressLine1 = _currentDbContext.ConsignmentTypes.FirstOrDefault(u => u.ConsignmentTypeId == 4)?.ConsignmentType;
                order.ShipmentAddressLine2 = null;
                order.ShipmentAddressLine3 = null;
                order.ShipmentAddressLine4 = null;
                order.ShipmentAddressPostcode = null;
                order.ShipmentAccountAddressId = (int?)null;

            }


            #region SendEmailWithAttachment
            if (shipmentAndRecipientInfo.SendEmailWithAttachment)
            {
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
            #endregion

            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);

            return order;
        }

        public Order SaveSalesOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            decimal total = 0;
            double? AccountCredit = 0;
            if (orderDetails != null)
            {
                var toAdd = orderDetails.Where(a => a.OrderDetailID < 0).ToList();
                var cItems = orderDetails.Where(a => a.OrderDetailID > 0).ToList();

                var toDelete = _currentDbContext.OrderDetail.Where(a => a.OrderID == order.OrderID && a.IsDeleted != true).Select(a => a.OrderDetailID).Except(cItems.Select(a => a.OrderDetailID).ToList());
                foreach (var item in toDelete)
                {
                    var dItem = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == item);
                    dItem.IsDeleted = true;
                }

                foreach (var item in toAdd)
                {
                    int? taxId = item.TaxID;
                    int? warrantyId = item.WarrantyID;
                    int productId = item.ProductId;
                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.TenentId = tenantId;
                    item.OrderID = order.OrderID;
                    total = total + item.TotalAmount;
                    item.TenentId = tenantId;
                    item.WarehouseId = warehouseId;
                    item.IsDeleted = null;

                    if (item.WarrantyID > 0)
                    {
                      
                        var warranty = _currentDbContext.TenantWarranty.AsNoTracking().FirstOrDefault(x => x.WarrantyID == item.WarrantyID);
                        if (warranty != null)
                        {
                             
                            if (warranty.IsPercent)
                            {
                                item.WarrantyAmount = (item.TotalAmount / 100) * item.Warranty.PercentageOfPrice;
                            }
                            else
                            {
                                item.WarrantyAmount = warranty.FixedPrice;
                            }
                        }
                        
                    }

                    // fix navigation issue
                    item.ProductMaster = null;
                    item.TaxName = null;
                    item.Warranty = null;

                    item.ProductId = productId;
                    item.TaxID = taxId;
                    item.WarrantyID = warrantyId;

                    if (item.OrderDetailID > 0)
                    {
                        _currentDbContext.Entry(item).State = EntityState.Modified;
                    }
                    else
                    {
                        _currentDbContext.OrderDetail.Add(item);
                    }

                    _currentDbContext.SaveChanges();
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

                _currentDbContext.SaveChanges();

            }
            else
            {
                foreach (var item in _currentDbContext.OrderDetail.Where(a => a.OrderID == order.OrderID && a.IsDeleted != true))
                {
                    item.IsDeleted = true;
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
                        _currentDbContext.OrderNotes.Add(m);
                        _currentDbContext.SaveChanges();
                    });
                }
            }
           
            order.OrderCost = total;
            order.OrderDiscount = _orderService.GetDiscountOnTotalCost(order.AccountID ?? 0, total);
            order.OrderTotal = (decimal)total - order.OrderDiscount;
            if (shipmentAndRecipientInfo.AddAddressToAccount == true && shipmentAndRecipientInfo.ShipmentAddressLine1 != null && order.AccountID > 0 && (!order.ShipmentAccountAddressId.HasValue || order.ShipmentAccountAddressId < 1))
            {
                var account = _accountServices.GetAccountsById(order.AccountID.Value);

                var accountAddress = new AccountAddresses()
                {
                    AccountID = order.AccountID.Value,
                    AddressLine1 = shipmentAndRecipientInfo.ShipmentAddressLine1,
                    AddressLine2 = shipmentAndRecipientInfo.ShipmentAddressLine2,
                    AddressLine3 = shipmentAndRecipientInfo.ShipmentAddressLine3,
                    Town = shipmentAndRecipientInfo.ShipmentAddressLine4,
                    PostCode = shipmentAndRecipientInfo.ShipmentAddressPostcode,
                    AddTypeShipping = true,
                    CountryID = account.CountryID,
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                    Name = account.CompanyName
                };
                _currentDbContext.AccountAddresses.Add(accountAddress);
            }
            var obj = _currentDbContext.Order.Find(order.OrderID);
            if (obj != null)
            {
                _currentDbContext.Entry(obj).State = System.Data.Entity.EntityState.Detached;
                order.CreatedBy = obj.CreatedBy;
                order.DateCreated = obj.DateCreated;
                //order.OrderStatusID = obj.OrderStatusID;
                order.TenentId = obj.TenentId;
                order.WarehouseId = obj.WarehouseId;
                if (order.ShipmentAccountAddressId < 1)
                {
                    order.ShipmentAccountAddressId = (int?)null;
                }
                order.InventoryTransactionTypeId = obj.InventoryTransactionTypeId;
                obj.OrderNumber = order.OrderNumber.Trim();
                obj.DateUpdated = DateTime.UtcNow;
                obj.UpdatedBy = userId;
                obj.WarehouseId = warehouseId;
                if (order.ConsignmentTypeId == 4)
                {
                    order.ShipmentAddressLine1 = _currentDbContext.ConsignmentTypes.FirstOrDefault(u => u.ConsignmentTypeId == 4)?.ConsignmentType;
                    order.ShipmentAddressLine2 = null;
                    order.ShipmentAddressLine3 = null;
                    order.ShipmentAddressLine4 = null;
                    order.ShipmentAddressPostcode = null;
                    order.ShipmentAccountAddressId = (int?)null;

                }

                #region SendEmailWithAttachment
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

                #endregion

                AccountCredit = _accountServices.GetAccountsById(order.AccountID.Value)?.CreditLimit;
                if (_orderService.CheckOrdersAuthroization(order.OrderTotal, order.InventoryTransactionTypeId, tenantId, userId, AccountCredit))
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
            }

            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return obj;

        }

        public bool DeleteSalesOrderDetailById(int orderDetailId, int userId)
        {
            var model = _currentDbContext.OrderDetail.Find(orderDetailId);

            if (model == null) return false;

            model.IsDeleted = true;
            model.UpdatedBy = userId;
            model.DateUpdated = DateTime.UtcNow;
            _currentDbContext.SaveChanges();
            return true;
        }

        public IQueryable<OrderProcess> GetAllSalesConsignments(int tenantId, int warehouseId, int? InventoryTransactionId = null, int? orderstatusId = null)
        {
            if (InventoryTransactionId.HasValue)
            {
                if (InventoryTransactionId == (int)InventoryTransactionTypeEnum.Returns)
                {
                    return _currentDbContext.OrderProcess.Where(a => (a.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Returns || a.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WastedReturn)
                    && (a.WarehouseId == warehouseId || a.Order.Warehouse.ParentWarehouseId == warehouseId) && a.TenentId == tenantId && a.IsDeleted != true);
                }
                else
                {
                    return _currentDbContext.OrderProcess.Where(a => a.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Wastage && a.WarehouseId == warehouseId && a.TenentId == tenantId && a.IsDeleted != true);
                }

            }


            return _currentDbContext.OrderProcess.Where(a => (a.InventoryTransactionTypeId != (int)InventoryTransactionTypeEnum.Returns &&
            a.InventoryTransactionTypeId != (int)InventoryTransactionTypeEnum.WastedReturn &&
            a.InventoryTransactionTypeId != (int)InventoryTransactionTypeEnum.Wastage) &&

            (a.Order.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder
            || a.Order.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Loan
            || a.Order.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder
            || a.Order.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Samples)
            && a.WarehouseId == warehouseId && (!orderstatusId.HasValue || a.OrderProcessStatusId == orderstatusId) && a.TenentId == tenantId && a.IsDeleted != true)

            .Include(x => x.Order);
        }

        public IQueryable<SalesOrderViewModel> GetAllActiveSalesOrdersIq(int tenantId, int warehouseId, int? statusId = null)
        {

            var result = _currentDbContext.Order.AsNoTracking().Where(o =>
                    o.TenentId == tenantId && o.WarehouseId == warehouseId && (!statusId.HasValue || o.OrderStatusID == statusId.Value) && (o.OrderStatusID == (int)OrderStatusEnum.Active || o.OrderStatusID == (int)OrderStatusEnum.Hold || o.OrderStatusID == (int)OrderStatusEnum.BeingPicked || o.OrderStatusID == (int)OrderStatusEnum.AwaitingAuthorisation) && (o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder
                            || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Proforma || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Quotation ||
                            o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Samples || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Loan) && o.IsDeleted != true)
                .OrderByDescending(x => x.DateCreated)
                .Select(p => new SalesOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    DateUpdated = p.DateUpdated,
                    DateCreated = p.DateCreated,
                    POStatus = p.OrderStatus.Status,
                    OrderStatusID = p.OrderStatusID,
                    Account = p.Account.AccountCode,
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                    OrderType = p.TransactionType.OrderType,
                    AccountName = p.Account.CompanyName,
                    Currecny = p.AccountCurrency.CurrencyName,
                    OrderTotal = p.OrderTotal,
                    EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                    {
                        OrderNoteId = s.OrderNoteId,
                        Notes = s.Notes,
                        NotesByName = p.Tenant.AuthUsers.FirstOrDefault(x => x.UserId == s.CreatedBy).UserName,
                        NotesDate = s.DateCreated
                    }).ToList()
                });

            return result;
        }

        public IQueryable<SalesOrderViewModel> GetAllCompletedSalesOrdersIq(int tenantId, int warehouseId, int? type = null)
        {


            var result = _currentDbContext.Order.AsNoTracking().Where(o =>
                    o.TenentId == tenantId && o.WarehouseId == warehouseId && ((type.HasValue && o.OrderStatusID == type) || !type.HasValue) && (o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder
                            || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Proforma || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Quotation ||
                            o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Samples || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Loan) && o.IsDeleted != true)
                .OrderByDescending(x => x.DateCreated)
                .Select(p => new SalesOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    DateUpdated = p.DateUpdated,
                    DateCreated = p.DateCreated,
                    POStatus = p.OrderStatus.Status,
                    OrderStatusID = p.OrderStatusID,
                    Account = p.Account.AccountCode,
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                    OrderType = p.TransactionType.OrderType,
                    AccountName = p.Account.CompanyName,
                    Currecny = p.AccountCurrency.CurrencyName,
                    OrderTotal = p.OrderTotal,
                    EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                    {
                        OrderNoteId = s.OrderNoteId,
                        Notes = s.Notes,
                        NotesByName = p.Tenant.AuthUsers.FirstOrDefault(x => x.UserId == s.CreatedBy).UserName,
                        NotesDate = s.DateCreated
                    }).ToList()
                });

            return result;
        }

        public IQueryable<SalesOrderViewModel> GetAllDirectSalesOrdersIq(int tenantId, int warehouseId, int? statusId = null)
        {

            IQueryable<TenantLocations> childWarehouseIds = _currentDbContext.TenantWarehouses.Where(x => x.ParentWarehouseId == warehouseId);

            var result = _currentDbContext.Order.AsNoTracking().Where(o =>
                     o.TenentId == tenantId && (o.WarehouseId == warehouseId || childWarehouseIds.Any(x => x.ParentWarehouseId == warehouseId)) && (!statusId.HasValue || o.OrderStatusID == statusId.Value) && (o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.DirectSales) && o.IsDeleted != true)
                .OrderByDescending(x => x.DateCreated)
                .Select(p => new SalesOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    DateUpdated = p.DateUpdated,
                    DateCreated = p.DateCreated,
                    POStatus = p.OrderStatus.Status,
                    Account = p.Account != null ? p.Account.AccountCode : "",
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    OrderDiscount = p.OrderDiscount,
                    OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                    OrderType = p.TransactionType.OrderType,
                    AccountName = p.Account != null ? p.Account.CompanyName : p.Note,
                    Currecny = p.AccountCurrency != null ? p.AccountCurrency.CurrencyName : "",
                    OrderTotal = p.OrderTotal,
                    SaleNotes = p.Note,
                    EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                    {
                        OrderNoteId = s.OrderNoteId,
                        Notes = s.Notes,
                        NotesByName = p.Tenant.AuthUsers.FirstOrDefault(x => x.UserId == s.CreatedBy).UserName,
                        NotesDate = s.DateCreated
                    }).ToList()

                });

            return result;

        }
        public IQueryable<SalesOrderViewModel> GetAllReturnOrders(int tenantId, int warehouseId, int? statusId = null)
        {

            IQueryable<TenantLocations> childWarehouseIds = _currentDbContext.TenantWarehouses.Where(x => x.ParentWarehouseId == warehouseId);

            var result = _currentDbContext.Order.AsNoTracking().Where(o =>
                     o.TenentId == tenantId && (o.WarehouseId == warehouseId || childWarehouseIds.Any(x => x.ParentWarehouseId == warehouseId)) && (!statusId.HasValue || o.OrderStatusID == statusId.Value) && (o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Returns || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WastedReturn) && o.IsDeleted != true)
                .OrderByDescending(x => x.DateCreated)
                .Select(p => new SalesOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    DateUpdated = p.DateUpdated,
                    DateCreated = p.DateCreated,
                    POStatus = p.OrderStatus.Status,
                    Account = p.Account != null ? p.Account.AccountCode : "",
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    OrderDiscount = p.OrderDiscount,
                    OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                    OrderType = p.TransactionType.OrderType,
                    AccountName = p.Account != null ? p.Account.CompanyName : p.Note,
                    Currecny = p.AccountCurrency != null ? p.AccountCurrency.CurrencyName : "",
                    OrderTotal = p.OrderTotal,
                    SaleNotes = p.Note,
                    EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                    {
                        OrderNoteId = s.OrderNoteId,
                        Notes = s.Notes,
                        NotesByName = p.Tenant.AuthUsers.FirstOrDefault(x => x.UserId == s.CreatedBy).UserName,
                        NotesDate = s.DateCreated
                    }).ToList()

                });

            return result;

        }



        public List<SalesOrderViewModel> GetAllSalesOrdersForPalletsByAccount(int tenantId, int accountId)
        {
            var dispatchInitialDate = DateTime.Today.AddDays(-5);

            return _currentDbContext.Order.AsNoTracking().Where(o => o.AccountID == accountId &&
                    o.TenentId == tenantId && (o.OrderStatusID == (int)OrderStatusEnum.Active || o.OrderStatusID == (int)OrderStatusEnum.Complete) &&
                    (o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferOut
                    || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Loan || o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Samples)
                    && o.IsDeleted != true && o.DateUpdated > dispatchInitialDate)
                .OrderByDescending(x => x.DateCreated)
                .Select(p => new SalesOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    DateUpdated = p.DateUpdated,
                    POStatus = p.OrderStatus.Status,
                    Account = p.Account.AccountCode,
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                    OrderType = p.TransactionType.OrderType
                }).ToList();
        }

        public bool AuthoriseSalesOrder(int orderId, int userId, string notes, bool unauthorize = false)
        {
            var order = _currentDbContext.Order.First(m => m.OrderID == orderId);
            order.AuthorisedDate = DateTime.UtcNow;
            order.DateUpdated = DateTime.UtcNow;
            order.AuthorisedUserID = userId;
            order.AuthorisedNotes = notes;
            if (!unauthorize)
            {
                order.OrderStatusID = (int)OrderStatusEnum.Approved;
                if (order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder)
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
                foreach (var item in order.OrderDetails)
                {
                    if (item.OrderDetailStatusId == (int)OrderStatusEnum.AwaitingAuthorisation)
                    {
                        item.OrderDetailStatusId = (int)OrderStatusEnum.Active;
                        item.UpdatedBy = userId;
                        item.DateUpdated = DateTime.UtcNow;
                    }
                    _currentDbContext.Entry(item).State = EntityState.Modified;
                }
            }
            else
            {

                order.OrderStatusID = (int)OrderStatusEnum.AwaitingAuthorisation;
                _currentDbContext.Entry(order).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return false;

            }
            _currentDbContext.Entry(order).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return true;
        }

        public PalletTracking GetSerialByPalletTrackingScheme(int productId, int palletTrackingSchemeId, int tenantId, int warehouseId)
        {
            if (palletTrackingSchemeId == 1)
            {
                var values = _currentDbContext.PalletTracking.Where(u => u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0 && u.Status == PalletTrackingStatusEnum.Active).OrderBy(u => u.PalletTrackingId).FirstOrDefault();
                return values;
            }
            else if (palletTrackingSchemeId == 2)
            {
                return _currentDbContext.PalletTracking.Where(u => u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0 && u.Status == PalletTrackingStatusEnum.Active).OrderByDescending(u => u.PalletTrackingId).FirstOrDefault();
            }
            else if (palletTrackingSchemeId == 3)
            {
                return _currentDbContext.PalletTracking.Where(u => u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0 && u.Status == PalletTrackingStatusEnum.Active).OrderBy(u => u.ExpiryDate).FirstOrDefault();
            }
            else if (palletTrackingSchemeId == 4)
            {
                return _currentDbContext.PalletTracking.Where(u => u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0 && u.Status == PalletTrackingStatusEnum.Active).OrderBy(u => u.ExpiryDate).FirstOrDefault();
            }
            else
            {
                return null;
            }

        }
        public PalletTracking GetUpdatedSerial(int productId, int palletTrackingSchemeId, int tenantId, int warehouseId, List<string> serial)
        {
            if (palletTrackingSchemeId == 1)
            {
                var values = _currentDbContext.PalletTracking.Where(u => u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0 && u.Status == PalletTrackingStatusEnum.Active && !serial.Contains(u.PalletSerial)).OrderBy(u => u.PalletTrackingId).FirstOrDefault();
                return values;
            }
            else if (palletTrackingSchemeId == 2)
            {
                return _currentDbContext.PalletTracking.Where(u => u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0 && u.Status == PalletTrackingStatusEnum.Active && !serial.Contains(u.PalletSerial)).OrderByDescending(u => u.PalletTrackingId).FirstOrDefault();
            }
            else if (palletTrackingSchemeId == 3)
            {
                return _currentDbContext.PalletTracking.Where(u => u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0 && u.Status == PalletTrackingStatusEnum.Active && !serial.Contains(u.PalletSerial)).OrderBy(u => u.ExpiryDate).FirstOrDefault();
            }
            else if (palletTrackingSchemeId == 4)
            {
                return _currentDbContext.PalletTracking.Where(u => u.ProductId == productId && u.TenantId == tenantId && u.WarehouseId == warehouseId && u.RemainingCases > 0 && u.Status == PalletTrackingStatusEnum.Active && !serial.Contains(u.PalletSerial)).OrderBy(u => u.ExpiryDate).FirstOrDefault();
            }
            else
            {
                return null;
            }

        }

        public List<Order> GetDirectSaleOrders(int? orderId)
        {
            return _currentDbContext.Order.Where(u => ((!orderId.HasValue) || (u.OrderID == orderId)) && u.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder && u.DirectShip == true && u.IsDeleted != true).ToList();

        }
    }
}