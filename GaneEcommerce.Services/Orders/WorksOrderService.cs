using System;
using System.Collections.Generic;
using System.Data.Entity;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using System.Linq;
using System.Web.WebPages;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public class WorksOrderService : IWorksOrderService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IOrderService _orderService;

        public WorksOrderService(IApplicationContext currentDbContext, IOrderService orderService)
        {
            _currentDbContext = currentDbContext;
            _orderService = orderService;
        }
        public Order CreateWorksOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null)
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
            order.WarehouseId = warehouseId;
            order.CreatedBy = userId;
            order.UpdatedBy = userId;
            order.InventoryTransactionTypeId = InventoryTransactionTypeEnum.WorksOrder;
            order.OrderStatusID = OrderStatusEnum.NotScheduled;
            order.ShipmentPropertyId = shipmentAndRecipientInfo.PPropertyID;



            _currentDbContext.Order.Add(order);

            if (orderDetails != null)
            {
                decimal? ordTotal = 0;
                foreach (var item in orderDetails)
                {
                    int? taxId = item.TaxID;
                    int? warrantyId = item.WarrantyID;
                    int productId = item.ProductId;
                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.OrderID = order.OrderID;
                    item.TenentId = tenantId;
                    item.WarehouseId = warehouseId;
                    item.SortOrder = item.ProductMaster?.ProductGroup?.SortOrder ?? 0;
                    // fix navigation issue
                    item.ProductMaster = null;
                    item.TaxName = null;
                    item.Warranty = null;

                    item.ProductId = productId;
                    item.TaxID = taxId;
                    item.WarrantyID = warrantyId;

                    order.OrderDetails.Add(item);
                    ordTotal = ordTotal + ((item.Price * item.Qty) + item.TaxAmount);

                }

                order.OrderTotal = (decimal)ordTotal;
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
            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return order;
        }

        public Order SaveWorksOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId, int userId, bool isOrderComplete, List<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null)
        {
            order.OrderNumber = order.OrderNumber.Trim();
            order.DateUpdated = DateTime.UtcNow;
            order.UpdatedBy = userId;
            order.WarehouseId = warehouseId;

            decimal total = 0;
            if (orderDetails != null)
            {
                var toAdd = orderDetails.Where(a => a.OrderDetailID < 0).ToList();
                var cItems = orderDetails.Where(a => a.OrderDetailID > 0).ToList();

                var recentlyUpdatedIds = new List<int>();
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
                    total = total + item.TotalAmount;

                    if (item.WarrantyID > 0)
                    {
                        var warranty = _currentDbContext.TenantWarranty.Find(item.WarrantyID);
                        if (warranty.IsPercent)
                        {
                            item.WarrantyAmount = (item.TotalAmount / 100) * item.Warranty.PercentageOfPrice;
                        }
                        else
                        {
                            item.WarrantyAmount = item.Warranty.FixedPrice;
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
                    recentlyUpdatedIds.Add(item.OrderDetailID);
                }

                var toDelete = _currentDbContext.OrderDetail.Where(a => a.OrderID == order.OrderID && a.IsDeleted != true)
                    .Select(a => a.OrderDetailID)
                    .Except(cItems.Select(a => a.OrderDetailID).ToList().Union(recentlyUpdatedIds));

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
            if (orderNotes != null)
            {
                if (orderNotes.Count() == 0)
                {
                    var cItems = _currentDbContext.OrderNotes
                        .Where(
                            a => a.OrderID == order.OrderID && a.IsDeleted != true && a.TenantId == tenantId)
                        .ToList();
                    cItems.ForEach(m =>
                    {
                        m.DateUpdated = DateTime.UtcNow;
                        m.UpdatedBy = userId;
                        m.IsDeleted = true;
                    });
                }
                else
                {
                    orderNotes.Where(a => a.OrderNoteId < 0)
                        .ToList()
                        .ForEach(m =>
                        {
                            m.DateCreated =
                                DateTime.UtcNow;
                            m.CreatedBy = userId;
                            m.TenantId = tenantId;
                            m.OrderID = order.OrderID;
                            _currentDbContext.OrderNotes.Add(m);
                        });
                }
            }

            order.OrderTotal = total;
            var obj = _currentDbContext.Order.Find(order.OrderID);
            if (obj != null)
            {
                _currentDbContext.Entry(obj).State = EntityState.Detached;
                order.CreatedBy = obj.CreatedBy;
                order.DateCreated = obj.DateCreated;
                if (order.OrderStatusID == 0)
                {
                    order.OrderStatusID = obj.OrderStatusID;
                }
                order.TenentId = obj.TenentId;
                order.WarehouseId = obj.WarehouseId;
                order.InventoryTransactionTypeId = obj.InventoryTransactionTypeId;

                obj.OrderNumber = order.OrderNumber.Trim();
                obj.DateUpdated = DateTime.UtcNow;
                obj.UpdatedBy = userId;
                obj.WarehouseId = warehouseId;
                _currentDbContext.Order.Attach(order);
                var entry = _currentDbContext.Entry(order);
                entry.State = EntityState.Modified;
            }

            if (!isOrderComplete && shipmentAndRecipientInfo.PropertyTenantIds != null)
            {
                foreach (var item in shipmentAndRecipientInfo.PropertyTenantIds)
                {
                    var ptenantId = item.AsInt();
                    if (!_currentDbContext.OrderPTenantEmailRecipients.Any(m => m.PTenantId == ptenantId && m.OrderId == order.OrderID))
                    {
                        var recipient = new OrderPTenantEmailRecipient()
                        {
                            OrderId = order.OrderID,
                            DateUpdated = DateTime.UtcNow,
                            PPropertyId = order.ShipmentPropertyId,
                            PTenantId = item.AsInt()
                        };
                        _currentDbContext.Entry(recipient).State = EntityState.Added;
                        _currentDbContext.SaveChanges();
                    }
                }
            }

            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return order;
        }


        public Order SaveWorksOrderBulkSingle(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId, int userId, List<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null)
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
            order.WarehouseId = warehouseId;
            order.CreatedBy = userId;
            order.UpdatedBy = userId;

            order.InventoryTransactionTypeId = InventoryTransactionTypeEnum.WorksOrder;
            order.OrderStatusID = OrderStatusEnum.NotScheduled;
            order.ShipmentPropertyId = shipmentAndRecipientInfo.PPropertyID;

            _currentDbContext.Order.Add(order);


            if (orderDetails != null)
            {
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
                    item.Warranty = null;
                    item.WarehouseId = warehouseId;
                    order.OrderDetails.Add(item);
                    ordTotal = ordTotal + ((item.Price * item.Qty) + item.TaxAmount);

                }

                order.OrderTotal = (decimal)ordTotal;

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
            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return order;
        }

        public Order UpdateWorksOrderBulkSingle(Order Order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId,
            int warehouseId, int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            Order.OrderNumber = Order.OrderNumber.Trim();
            Order.IssueDate = DateTime.UtcNow;
            Order.DateCreated = DateTime.UtcNow;
            Order.DateUpdated = DateTime.UtcNow;
            Order.TenentId = tenantId;
            Order.WarehouseId = warehouseId;
            Order.CreatedBy = userId;
            Order.UpdatedBy = userId;
            Order.InventoryTransactionTypeId = InventoryTransactionTypeEnum.WorksOrder;
            Order.OrderStatusID = OrderStatusEnum.NotScheduled;
            Order.ShipmentPropertyId = shipmentAndRecipientInfo.PPropertyID;

            _currentDbContext.Order.Attach(Order);

            _currentDbContext.Entry(Order).State = EntityState.Modified;

            decimal total = 0;
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
                    total = total + item.TotalAmount;
                    item.TenentId = tenantId;
                    item.WarehouseId = warehouseId;
                    _currentDbContext.OrderDetail.Add(item);
                }

                var toDelete = _currentDbContext.OrderDetail.Where(a => a.OrderID == Order.OrderID && a.IsDeleted != true).Select(a => a.OrderDetailID).Except(cItems.Select(a => a.OrderDetailID).ToList());
                foreach (var item in toDelete)
                {
                    var dItem = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == item);
                    dItem.IsDeleted = true;
                }
                foreach (var item in cItems)
                {
                    total = total + item.TotalAmount;
                    var dItem = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == item.OrderDetailID);
                    if (dItem != null)
                    {
                        dItem.Qty = item.Qty;
                        dItem.Notes = item.Notes;
                        dItem.Price = item.Price;
                        dItem.TaxID = item.TaxID;
                        dItem.TaxAmount = item.TaxAmount;
                        dItem.WarrantyID = item.WarrantyID;
                        dItem.WarrantyAmount = item.WarrantyAmount;
                        dItem.TotalAmount = item.TotalAmount;
                        dItem.DateUpdated = DateTime.UtcNow;
                        dItem.UpdatedBy = userId;
                        _currentDbContext.Entry(dItem).State = EntityState.Modified;
                    }
                }
            }
            else
            {
                foreach (var item in _currentDbContext.OrderDetail.Where(a => a.OrderID == Order.OrderID && a.IsDeleted != true))
                {
                    item.IsDeleted = true;
                }
            }
            if (orderNotes != null)
            {

                if (orderNotes.Count() == 0)
                {
                    var cItems = _currentDbContext.OrderNotes.Where(a => a.OrderID == Order.OrderID && a.IsDeleted != true && a.TenantId == tenantId).ToList();
                    cItems.ForEach(m =>
                    {
                        m.DateUpdated = DateTime.UtcNow;
                        m.UpdatedBy = userId;
                        m.IsDeleted = true;
                        _currentDbContext.Entry(m).State = EntityState.Modified;
                    });

                }
                else
                {
                    orderNotes.Where(a => a.OrderNoteId < 0).ToList().ForEach(m =>
                    {
                        m.DateCreated = DateTime.UtcNow;
                        m.CreatedBy = userId;
                        m.TenantId = tenantId;
                        m.OrderID = Order.OrderID;
                        _currentDbContext.OrderNotes.Add(m);
                    });
                }
            }

            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(Order.OrderID, warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return Order;
        }

        public List<Order> GetAllOrdersByGroupToken(Guid groupToken, int tenantId)
        {
            return _currentDbContext.Order.Where(m => m.OrderGroupToken == groupToken && m.Tenant.TenantId == tenantId && m.IsDeleted != true).ToList();
        }

        public List<WorksOrderViewModel> GetAllPendingWorksOrders(int tenantId, Guid? groupToken = null)
        {

            return _currentDbContext.Order
                .Where(o => o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID != OrderStatusEnum.Complete && (!groupToken.HasValue || o.OrderGroupToken == groupToken) && o.IsDeleted != true)
                .OrderByDescending(m => m.SLAPriorityId).ThenByDescending(x => x.DateCreated)
                .Include(x => x.Appointmentses)
                .Select(p => new WorksOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    JobNotes = p.Note,
                    JobTypeName = p.JobType.Name,
                    JobSubTypeName = p.JobSubType.Name,
                    DateUpdated = p.DateUpdated,
                    DateCreated = p.DateCreated,
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    OrderGroupToken = p.OrderGroupToken,
                    POStatus = p.OrderStatusID.ToString(),
                    Account = p.Account.AccountCode,
                    Property = p.PProperties.AddressLine1,
                    OrderTypeId = p.InventoryTransactionTypeId,
                    OrderType = p.InventoryTransactionTypeId,
                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                    {
                        OrderNoteId = s.OrderNoteId,
                        Notes = s.Notes,
                        NotesByName = p.Tenant.AuthUsers.FirstOrDefault(x => x.UserId == s.CreatedBy).UserName,
                        NotesDate = s.DateCreated
                    }).ToList(),
                    OrderNotes = p.OrderNotes.Where(m => m.IsDeleted != true).Where(a => a.IsDeleted != true && a.TenantId == tenantId)
                        .Select(a => a.Notes),
                    ScheduledStartTime = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).StartTime,
                    ScheduledEndTime = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).EndTime,
                    ResourceId = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).ResourceId,
                    ResourceName = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).AppointmentResources.FirstName + " " + p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).AppointmentResources.SurName
                }).ToList();
        }
        public IQueryable<WorksOrderViewModel> GetAllPendingWorksOrdersIq(int tenantId, Guid? groupToken = null, int? propertyId = null)
        {

            var result = _currentDbContext.Order
                .Where(o => o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder && (o.OrderStatusID == OrderStatusEnum.Active || o.OrderStatusID == OrderStatusEnum.NotScheduled
                || o.OrderStatusID == OrderStatusEnum.Scheduled || o.OrderStatusID == OrderStatusEnum.ReAllocationRequired || o.OrderStatusID == OrderStatusEnum.Hold)
                && (!groupToken.HasValue
                || o.OrderGroupToken == groupToken) && o.IsDeleted != true && (!propertyId.HasValue || o.PPropertyId == propertyId))
                .OrderByDescending(m => m.SLAPriorityId).ThenByDescending(x => x.DateCreated)
                .Include(x => x.Appointmentses)
                .Include(x => x.OrderNotes)
                .Include(x => x.JobType)
                .Include(x => x.JobSubType)
                .Include(x => x.Account)
                .Include(x => x.PProperties)
                .Select(p => new WorksOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    JobNotes = p.Note,
                    JobTypeName = p.JobType.Name,
                    JobSubTypeName = p.JobSubType.Name,
                    DateUpdated = p.DateUpdated,
                    DateCreated = p.DateCreated,
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    OrderGroupToken = p.OrderGroupToken,
                    POStatus = p.OrderStatusID.ToString(),
                    Account = p.Account.AccountCode,
                    Property = p.PProperties.AddressLine1,
                    OrderTypeId = p.InventoryTransactionTypeId,
                    OrderType = p.InventoryTransactionTypeId,
                    EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                    {
                        OrderNoteId = s.OrderNoteId,
                        Notes = s.Notes,
                        NotesByName = s.User.UserName,
                        NotesDate = s.DateCreated
                    }).ToList(),
                    OrderNotes = p.OrderNotes.Where(m => m.IsDeleted != true).Where(a => a.IsDeleted != true && a.TenantId == tenantId)
                        .Select(a => a.Notes),
                    ScheduledStartTime = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).StartTime,
                    ScheduledEndTime = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).EndTime,
                    ResourceId = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).ResourceId,
                    ResourceName = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).AppointmentResources.FirstName + " " + p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).AppointmentResources.SurName
                });

            return result;
        }

        public IQueryable<WorksOrderViewModel> GetAllCompletedWorksOrdersIq(int tenantId, int? propertyId, int? type = null)
        {

            var result = _currentDbContext.Order
                .Where(o => o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder && ((type.HasValue && o.OrderStatusID == OrderStatusEnum.Complete) || !type.HasValue) && o.IsDeleted != true && (!propertyId.HasValue || o.PPropertyId == propertyId))
                .OrderByDescending(m => m.SLAPriorityId).ThenByDescending(x => x.DateCreated)
                .Include(x => x.Appointmentses)
                .Include(x => x.OrderNotes)
                .Include(x => x.JobType)
                .Include(x => x.JobSubType)
                .Include(x => x.Account)
                .Include(x => x.PProperties)
                .Select(p => new WorksOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    JobNotes = p.Note,
                    JobTypeName = p.JobType.Name,
                    JobSubTypeName = p.JobSubType.Name,
                    OrderGroupToken = p.OrderGroupToken,
                    DateUpdated = p.DateUpdated,
                    DateCreated = p.DateCreated,
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    POStatus = p.OrderStatusID.ToString(),
                    Account = p.Account.AccountCode,
                    Property = p.PProperties.AddressLine1,
                    OrderTypeId = p.InventoryTransactionTypeId,
                    EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }).ToList(),
                    OrderType = p.InventoryTransactionTypeId,
                    ScheduledStartTime = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).StartTime,
                    ScheduledEndTime = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).EndTime,
                    ResourceId = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).ResourceId,
                    ResourceName = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).AppointmentResources.FirstName + " " + p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true).AppointmentResources.SurName
                });
            return result;
        }
        public IQueryable<Order> GetAllWorksOrders(int tenantId)
        {
            return _currentDbContext.Order
                .Where(o => o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder && o.IsDeleted != true && o.IsCancel != true);
        }
    }
}