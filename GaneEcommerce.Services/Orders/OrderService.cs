using AutoMapper;
using Elmah;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IProductServices _productService;
        private readonly IAccountServices _accountServices;
        private readonly IInvoiceService _invoiceService;
        private readonly IUserService _userService;
        private readonly ILookupServices _lookupServices;
        private readonly ITenantsServices _tenantServices;
        private readonly IProductPriceService _productPriceService;
        private readonly IMapper _mapper;


        public OrderService(IApplicationContext currentDbContext, IProductServices productService, IAccountServices accountServices, IInvoiceService invoiceService, IUserService userService, ILookupServices lookupServices,
            ITenantsServices tenantServices, IProductPriceService productPriceService, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _productService = productService;
            _accountServices = accountServices;
            _invoiceService = invoiceService;
            _userService = userService;
            _lookupServices = lookupServices;
            _tenantServices = tenantServices;
            _productPriceService = productPriceService;
            _mapper = mapper;
        }

        public string GenerateNextOrderNumber(InventoryTransactionTypeEnum type, int tenantId)
        {
            var lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == type)
                .OrderByDescending(m => m.OrderNumber)
                .FirstOrDefault();

            var prefix = "ON-";
            switch (type)
            {
                case InventoryTransactionTypeEnum.PurchaseOrder:
                    prefix = "PO-";
                    break;
                case InventoryTransactionTypeEnum.SalesOrder:
                case InventoryTransactionTypeEnum.Proforma:
                case InventoryTransactionTypeEnum.Quotation:
                case InventoryTransactionTypeEnum.Samples:
                    prefix = "SO-";
                    lastOrder = _currentDbContext.Order.Where(p =>
                    p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder ||
                    p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Proforma ||
                    p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Quotation ||
                    p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Samples
                    ).OrderByDescending(m => m.OrderNumber)
                     .FirstOrDefault();
                    break;
                case InventoryTransactionTypeEnum.WorksOrder:
                    prefix = "MO-";
                    break;
                case InventoryTransactionTypeEnum.DirectSales:
                    prefix = "DO-";
                    break;
                case InventoryTransactionTypeEnum.TransferIn:
                case InventoryTransactionTypeEnum.TransferOut:
                    lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn ||
                    p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut && p.OrderNumber.Length == 11)
                    .OrderByDescending(m => m.OrderNumber)
                    .FirstOrDefault();
                    prefix = "TO-";
                    break;
                case InventoryTransactionTypeEnum.Returns:
                    lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns && p.OrderNumber.Length == 11)
                   .OrderByDescending(m => m.OrderNumber).FirstOrDefault();
                    prefix = "RO-";
                    break;
                case InventoryTransactionTypeEnum.Wastage:
                    lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Wastage && p.OrderNumber.Length == 11)
                  .OrderByDescending(m => m.OrderNumber).FirstOrDefault();
                    prefix = "WO-";
                    break;
                case InventoryTransactionTypeEnum.AdjustmentIn:
                case InventoryTransactionTypeEnum.AdjustmentOut:
                    lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn ||
                     p.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentOut && p.OrderNumber.Length == 11)
                     .OrderByDescending(m => m.OrderNumber)
                     .FirstOrDefault();
                    prefix = "AO-";

                    break;
                case InventoryTransactionTypeEnum.Exchange:
                    prefix = "EO-";
                    break;
            }

            if (lastOrder != null)
            {

                var lastNumber = lastOrder.OrderNumber.Replace("PO-", string.Empty);
                lastNumber = lastNumber.Replace("SO-", string.Empty);
                lastNumber = lastNumber.Replace("MO-", string.Empty);
                lastNumber = lastNumber.Replace("TO-", string.Empty);
                lastNumber = lastNumber.Replace("DO-", string.Empty);
                lastNumber = lastNumber.Replace("RO-", string.Empty);
                lastNumber = lastNumber.Replace("WO-", string.Empty);
                lastNumber = lastNumber.Replace("EO-", string.Empty);
                lastNumber = lastNumber.Replace("AO-", string.Empty);
                lastNumber = lastNumber.Replace("ON-", string.Empty);

                int n;
                bool isNumeric = int.TryParse(lastNumber, out n);

                if (isNumeric == true)
                {
                    var lastOrderNumber = (int.Parse(lastNumber) + 1).ToString("00000000");
                    return prefix + lastOrderNumber;
                }
                else
                {
                    return prefix + "00000001";
                }
            }
            else
            {
                return prefix + "00000001";
            }
        }

        public string GetAuthorisedUserNameById(int userId)
        {
            return _currentDbContext.AuthUsers.First(m => m.UserId == userId && m.IsDeleted != true).UserName;
        }

        public IEnumerable<AuthUser> GetAllAuthorisedUsers(int tenantId, bool includeSuperUser = false)
        {
            return _currentDbContext.AuthUsers.Where(a => a.TenantId == tenantId && a.IsDeleted != true &&
                                                         (includeSuperUser || a.SuperUser != true));
        }

        public IEnumerable<JobType> GetAllValidJobTypes(int tenantId)
        {
            return _currentDbContext.JobTypes.Where(a => a.TenantId == tenantId && a.IsDeleted != true).OrderBy(m => m.Name);
        }

        public IEnumerable<JobSubType> GetAllValidJobSubTypes(int tenantId)
        {

            return _currentDbContext.JobSubTypes.Where(a => a.TenantId == tenantId).OrderBy(m => m.Name);
        }

        public Order GetOrderById(int orderId)
        {
            if (orderId <= 0) return null;
            return _currentDbContext.Order.AsNoTracking().FirstOrDefault(x => x.OrderID == orderId && x.IsDeleted != true);
        }

        public IQueryable<Order> GetOrdersHistory(int UserId, int SiteId)
        {
            return _currentDbContext.Order.Where(u => u.CreatedBy == UserId && u.SiteID == SiteId && u.IsDeleted != true);
        }

        public bool IsOrderNumberAvailable(string orderNumber)
        {
            var order = _currentDbContext.Order.FirstOrDefault(m => m.OrderNumber == orderNumber && m.IsDeleted != true);
            return order == null;
        }

        public Order CompleteOrder(int orderId, int userId)
        {
            var order = _currentDbContext.Order.Find(orderId);
            if (order == null) return null;
            order.OrderStatusID = OrderStatusEnum.Complete;

            foreach (var process in order.OrderProcess)
            {
                process.OrderProcessStatusId = OrderProcessStatusEnum.Complete;
                process.DateUpdated = DateTime.UtcNow;
                process.UpdatedBy = userId;
                _currentDbContext.Entry(process).State = EntityState.Modified;
            }

            order.UpdatedBy = userId;
            order.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(order).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

            return order;
        }
        public Order FinishinghOrder(int orderId, int userId, int warehouseId)
        {
            var order = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == orderId);

            if (order == null) return null;
            order.OrderStatusID = OrderStatusEnum.Complete;
            if (order.OrderProcess.Count > 0)
            {
                foreach (var process in order.OrderProcess)
                {
                    process.OrderProcessStatusId = OrderProcessStatusEnum.Complete;
                    process.DateUpdated = DateTime.UtcNow;
                    process.UpdatedBy = userId;
                    _currentDbContext.Entry(process).State = EntityState.Modified;
                }
            }
            else
            {
                var orderProcess = new OrderProcess()
                {
                    OrderProcessStatusId = OrderProcessStatusEnum.Complete,
                    OrderID = orderId,
                    TenentId = userId,
                    WarehouseId = warehouseId,
                    InventoryTransactionTypeId = order.InventoryTransactionTypeId,
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    ShipmentAddressLine1 = order.ShipmentAddressLine1,
                    ShipmentAddressLine2 = order.ShipmentAddressLine2,
                    ShipmentAddressLine3 = order.ShipmentAddressLine3,
                    ShipmentAddressLine4 = order.ShipmentAddressLine4,
                    ShipmentAddressPostcode = order.ShipmentAddressPostcode
                };

                _currentDbContext.Entry(orderProcess).State = EntityState.Added;



            }



            order.UpdatedBy = userId;
            order.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(order).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

            return order;
        }

        public Order CreateOrder(Order order, int tenantId, int warehouseId, int userId, IEnumerable<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null)
        {
            order.OrderNumber = order.OrderNumber.Trim();
            order.IssueDate = DateTime.UtcNow;
            order.OrderStatusID = order.OrderStatusID > 0 ? order.OrderStatusID : OrderStatusEnum.Active;
            order.DateCreated = DateTime.UtcNow;
            order.DateUpdated = DateTime.UtcNow;
            order.TenentId = tenantId;
            order.CreatedBy = userId;
            order.UpdatedBy = userId;
            order.WarehouseId = warehouseId;

            if (order.AccountID == 0)
            {
                order.AccountID = null;
            }

            var orderProcesses = order.OrderProcess.Where(u => u.IsDeleted != true).ToList();
            order.OrderProcess = null;
            order = _currentDbContext.Order.Add(order);

            try
            {
                _currentDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (orderDetails != null)
            {
                decimal? ordTotal = 0;
                foreach (var item in orderDetails)
                {
                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.OrderID = order.OrderID;
                    item.TenentId = tenantId;
                    item.WarehouseId = warehouseId;
                    item.ProductMaster = null;
                    item.TaxName = null;
                    _currentDbContext.OrderDetail.Add(item);
                    ordTotal = ordTotal + ((item.Price * item.Qty) + item.TaxAmount);
                }

                if (order.OrderDiscount > 0)
                {
                    order.OrderCost = (decimal)ordTotal;
                    order.OrderTotal = (decimal)ordTotal - order.OrderDiscount;
                }
                else
                {
                    order.OrderCost = (decimal)ordTotal;
                    order.OrderTotal = (decimal)ordTotal;
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
            _currentDbContext.SaveChanges();
            return _currentDbContext.Order.Find(order.OrderID);
        }

        public Order SaveOrder(Order order, int tenantId, int warehouseId, int userId,
            IEnumerable<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null)
        {
            order.OrderNumber = order.OrderNumber.Trim();
            order.DateUpdated = DateTime.UtcNow;
            order.UpdatedBy = userId;
            decimal total = 0;
            order.WarehouseId = warehouseId;
            if (orderDetails != null)
            {
                var items = orderDetails.ToList();
                var toAdd = items.Where(a => a.OrderDetailID < 0).ToList();
                var cItems = items.Where(a => a.OrderDetailID > 0).ToList();
                foreach (var item in toAdd)
                {
                    item.ProductMaster = null;
                    item.TaxName = null;
                    item.Warranty = null;
                    item.DateCreated = DateTime.UtcNow;
                    item.CreatedBy = userId;
                    item.TenentId = tenantId;
                    item.OrderID = order.OrderID;
                    total = total + item.TotalAmount;
                    _currentDbContext.OrderDetail.Add(item);
                }

                var toDelete = _currentDbContext.OrderDetail.Where(a => a.OrderID == order.OrderID && a.IsDeleted != true).Select(a => a.OrderDetailID).Except(cItems.Select(a => a.OrderDetailID).ToList());
                foreach (var item in toDelete)
                {
                    var dItem = _currentDbContext.OrderDetail.FirstOrDefault(a => a.OrderDetailID == item);
                    dItem.IsDeleted = true;
                }
                foreach (var item in cItems)
                {
                    var itm = _currentDbContext.OrderDetail.Find(item.OrderDetailID);
                    if (itm != null)
                        _currentDbContext.Entry(itm).State = System.Data.Entity.EntityState.Detached;

                    _currentDbContext.OrderDetail.Attach(item);
                    var pentry = _currentDbContext.Entry(item);

                    item.UpdatedBy = userId;
                    item.DateUpdated = DateTime.UtcNow;

                    pentry.Property(e => e.ExpectedDate).IsModified = true;
                    pentry.Property(e => e.Notes).IsModified = true;
                    pentry.Property(e => e.Price).IsModified = true;
                    pentry.Property(e => e.ExpectedDate).IsModified = true;
                    pentry.Property(e => e.ProductId).IsModified = true;
                    pentry.Property(e => e.Qty).IsModified = true;
                    pentry.Property(e => e.ProdAccCodeID).IsModified = true;
                    pentry.Property(e => e.TaxAmount).IsModified = true;
                    pentry.Property(e => e.TaxID).IsModified = true;
                    pentry.Property(e => e.TotalAmount).IsModified = true;
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
            if (order.OrderID > 0)
            {
                var obj = _currentDbContext.Order.Find(order.OrderID);
                _currentDbContext.Entry(obj).State = System.Data.Entity.EntityState.Detached;
                _currentDbContext.Order.Attach(order);
                var entry = _currentDbContext.Entry(order);

                entry.Property(e => e.OrderID).IsModified = true;
                entry.Property(e => e.OrderNumber).IsModified = true;
                entry.Property(e => e.ExpectedDate).IsModified = true;
                entry.Property(e => e.Note).IsModified = true;
                entry.Property(e => e.AccountID).IsModified = true;
                entry.Property(e => e.DateUpdated).IsModified = true;
                entry.Property(e => e.UpdatedBy).IsModified = true;
                entry.Property(e => e.InventoryTransactionTypeId).IsModified = true;
                entry.Property(e => e.LoanID).IsModified = true;
                entry.Property(e => e.OrderStatusID).IsModified = true;
                entry.Property(e => e.AccountContactId).IsModified = true;
                entry.Property(e => e.Posted).IsModified = true;
            }
            else
            {
                _currentDbContext.Entry(order).State = System.Data.Entity.EntityState.Added;
            }

            _currentDbContext.SaveChanges();
            return order;
        }

        public IEnumerable<OrderDetail> GetAllValidOrderDetailsByOrderId(int orderId)
        {
            return _currentDbContext.OrderDetail.Where(a => a.OrderID == orderId && a.IsDeleted != true).ToList();
        }

        public string GenerateNextOrderNumber(string type, int tenantId)
        {
            switch (type)
            {
                case "PO":
                    return GenerateNextOrderNumber(InventoryTransactionTypeEnum.PurchaseOrder, tenantId);
                case "SO":
                    return GenerateNextOrderNumber(InventoryTransactionTypeEnum.SalesOrder, tenantId);
                case "WO":
                    return GenerateNextOrderNumber(InventoryTransactionTypeEnum.WorksOrder, tenantId);
                case "TO":
                    return GenerateNextOrderNumber(InventoryTransactionTypeEnum.TransferIn, tenantId);
            }

            return GenerateNextOrderNumber(InventoryTransactionTypeEnum.WorksOrder, tenantId);
        }

        public IEnumerable<OrderConsignmentTypes> GetAllValidConsignmentTypes(int tenantId)
        {
            return _currentDbContext.ConsignmentTypes.Where(a => a.IsDeleted != true && a.TenantId == tenantId);
        }

        public Order CreateOrderByOrderNumber(string orderNumber, int productId, int tenantId, int warehouseId, InventoryTransactionTypeEnum transType, int userId, decimal quantity)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            Order order = new Order();
            var sessionTenantId = tenantId;
            var sessionWarehouseId = warehouseId;
            if (string.IsNullOrEmpty(orderNumber))
            {
                orderNumber = GenerateNextOrderNumber(transType, tenantId);

            }
            order.OrderNumber = orderNumber.Trim();

            var duplicateOrder = context.Order.FirstOrDefault(m => m.OrderNumber.Equals(order.OrderNumber, StringComparison.CurrentCultureIgnoreCase));
            if (duplicateOrder != null)
            {
                throw new Exception($"Order Number {order.OrderNumber} already associated with another Order. Please regenerate order number.", new Exception("Duplicate Order Number"));
            }
            order.InventoryTransactionTypeId = transType;
            order.IssueDate = DateTime.UtcNow;
            order.ExpectedDate = DateTime.UtcNow;
            order.OrderStatusID = OrderStatusEnum.Active;
            order.DateCreated = DateTime.UtcNow;
            order.DateUpdated = DateTime.UtcNow;
            order.TenentId = tenantId;
            order.CreatedBy = userId;
            order.UpdatedBy = userId;
            order.WarehouseId = warehouseId;
            order.OrderStatusID = OrderStatusEnum.Active;
            context.Order.Add(order);
            context.SaveChanges();
            OrderDetail orderDetail = new OrderDetail();
            orderDetail.DateCreated = DateTime.UtcNow;
            orderDetail.CreatedBy = userId;
            orderDetail.OrderID = order.OrderID;
            orderDetail.TenentId = tenantId;
            orderDetail.ProductId = productId;
            orderDetail.Price = _productPriceService.GetProductPriceThresholdByAccountId(productId, null).SellPrice;
            orderDetail.TotalAmount = (orderDetail.Price * quantity);
            orderDetail.Qty = quantity;
            //orderDetail.SortOrder = item.ProductMaster?.ProductGroup?.SortOrder ?? 0;

            orderDetail.TaxName = null;
            orderDetail.Warranty = null;
            orderDetail.WarehouseId = warehouseId;
            context.OrderDetail.Add(orderDetail);
            OrderNotes orderNotes = new OrderNotes();
            orderNotes.DateCreated = DateTime.UtcNow;
            orderNotes.CreatedBy = userId;
            orderNotes.Notes = "Sales return";
            orderNotes.OrderID = order.OrderID;
            orderNotes.TenantId = tenantId;
            context.OrderNotes.Add(orderNotes);
            order.OrderCost = orderDetail.TotalAmount;
            order.OrderTotal = orderDetail.TotalAmount;

            context.SaveChanges();



            return order;



        }

        public List<OrderDetailsViewModel> GetSalesOrderDetails(int id, int tenantId)
        {

            var model = _currentDbContext.OrderDetail.Where(a => a.OrderID == id && a.IsDeleted != true && a.TenentId == tenantId).ToList();

            var results = model.Select(ord => new OrderDetailsViewModel()
            {
                OrderID = ord.OrderID,
                ProductId = ord.ProductId,
                ProductMaster = ord.ProductMaster,
                OrderDetailID = ord.OrderDetailID,
                Product = ord.ProductMaster.Name,
                SkuCode = ord.ProductMaster.SKUCode,
                Barcode = ord.ProductMaster.BarCode,
                BarCode2 = ord.ProductMaster.BarCode2,
                DirectShip = ord.Order.DirectShip,
                Notes = ord.Notes,
                Qty = ord.Qty,
                Price = ord.Price,
                TotalWarrantyAmount = ord.WarrantyAmount * ord.Qty,
                WarrantyAmount = ord.WarrantyAmount,
                TaxName = ord.TaxName,
                TaxAmount = ord.TaxAmount,
                ExpectedDate = ord.ExpectedDate,
                TotalAmount = ord.TotalAmount,
                DateCreated = ord.DateCreated,
                QtyReturned = ord.ReturnedQty,
                EnableGlobalProcessByPallet = ord.TenantWarehouse.EnableGlobalProcessByPallet,
                TransType = _currentDbContext.Order.Find(ord.OrderID).InventoryTransactionTypeId,
                QtyProcessed = ord.ProcessedQty,
                DirectPostAllowed = !ord.ProductMaster.Serialisable && !ord.ProductMaster.ProcessByPallet && ord.ProductMaster.RequiresBatchNumberOnReceipt != true && ord.ProductMaster.RequiresExpiryDateOnReceipt != true && (ord.ProductMaster.ProductLocationsMap != null && ord.ProductMaster.ProductLocationsMap.Count < 1)
            }).ToList();

            return results;
        }

        public List<OrderDetailsViewModel> GetDirectSalesOrderDetails(int id, int tenantId)
        {

            var model = _currentDbContext.OrderDetail.Where(a => a.OrderID == id && a.IsDeleted != true && a.TenentId == tenantId).ToList();

            var results = model.Select(ord => new OrderDetailsViewModel()
            {
                OrderID = ord.OrderID,
                ProductId = ord.ProductId,
                ProductMaster = ord.ProductMaster,
                OrderDetailID = ord.OrderDetailID,
                Product = ord.ProductMaster.Name,
                Qty = ord.Qty,
                Price = ord.Price,
                TotalWarrantyAmount = ord.WarrantyAmount * ord.Qty,
                WarrantyAmount = ord.WarrantyAmount,
                TaxName = ord.TaxName,
                TaxAmount = ord.TaxAmount,
                ExpectedDate = ord.ExpectedDate,
                TotalAmount = ord.TotalAmount,
                DateCreated = ord.DateCreated,
                TransType = _currentDbContext.Order.Find(ord.OrderID).InventoryTransactionTypeId,
                DirectPostAllowed = !ord.ProductMaster.Serialisable && !ord.ProductMaster.ProcessByPallet && ord.ProductMaster.RequiresBatchNumberOnReceipt != true && ord.ProductMaster.RequiresExpiryDateOnReceipt != true && (ord.ProductMaster.ProductLocationsMap != null && ord.ProductMaster.ProductLocationsMap.Count < 1)
            }).ToList();

            return results;
        }
        public List<OrderDetailsViewModel> GetPalletOrdersDetails(int id, int tenantId, bool excludeProcessed = false)
        {

            var model = _currentDbContext.OrderProcessDetail.Where(a => a.OrderProcessId == id && a.IsDeleted != true && a.TenentId == tenantId).ToList();
            if (model.Count > 0)
            {

                var results = model.Where(x => x.QtyProcessed > 0).Select(ord => new OrderDetailsViewModel()
                {
                    OrderID = ord.OrderProcess.OrderID ?? 0,
                    ProductId = ord.ProductId,
                    ProductMaster = ord.ProductMaster,
                    ProductGroups = ord.OrderDetail.ProductGroups,
                    OrderDetailID = ord.OrderProcessDetailID,
                    Product = ord?.ProductMaster?.Name,
                    Qty = ord.QtyProcessed,
                    OrderNumber = ord.OrderProcess.Order.OrderNumber,
                    DateCreated = ord.DateCreated ?? DateTime.UtcNow,
                    QtyProcessed = ord.PalletedQuantity,
                    ProductGroup = ord.ID,
                    orderProcessstatusId = ord.OrderProcess.OrderProcessStatusId,
                    orderstatusId = ord.OrderProcess?.Order?.OrderStatusID,


                }).ToList();

                return results;
            }
            return null;
        }

        public List<OrderDetailsViewModel> GetTransferOrderDetails(int orderId, int warehouseId)
        {
            var model = _currentDbContext.OrderDetail.Where(a => a.OrderID == orderId && a.IsDeleted != true).ToList();
            var results = model.Select(ord => new OrderDetailsViewModel()
            {
                OrderID = ord.OrderID,
                ProductId = ord.ProductId,
                ProductMaster = ord.ProductMaster,
                OrderDetailID = ord.OrderDetailID,
                Product = ord.ProductMaster.Name,
                SkuCode = ord.ProductMaster.SKUCode,
                Barcode = ord.ProductMaster.BarCode,
                BarCode2 = ord.ProductMaster.BarCode2,
                Qty = ord.Qty,
                EnableGlobalProcessByPallet = ord.TenantWarehouse.EnableGlobalProcessByPallet,
                Price = ord.Price,
                DateCreated = ord.DateCreated,
                TotalWarrantyAmount = ord.WarrantyAmount * ord.Qty,
                WarrantyAmount = ord.WarrantyAmount,
                TaxName = ord.TaxName,
                TaxAmount = ord.TaxAmount,
                ExpectedDate = ord.ExpectedDate,
                TotalAmount = ord.TotalAmount,
                TransType = ord.Order.InventoryTransactionTypeId,
                QtyProcessed = ord.ProcessedQty,
                DirectPostAllowed = !ord.ProductMaster.Serialisable && !ord.ProductMaster.ProcessByPallet && ord.ProductMaster.RequiresBatchNumberOnReceipt != true && ord.ProductMaster.RequiresExpiryDateOnReceipt != true && !_currentDbContext.ProductLocationsMap.Any(m => m.ProductId == ord.ProductId && m.IsDeleted != true)

            }).ToList();

            return results;
        }

        public List<OrderDetailsViewModel> GetWorksOrderDetails(int id, int tenantId)
        {
            var model = _currentDbContext.OrderDetail.Where(a => a.OrderID == id && a.IsDeleted != true && a.TenentId == tenantId).ToList();

            var results = model.Select(ord => new OrderDetailsViewModel()
            {
                OrderID = ord.OrderID,
                ProductId = ord.ProductId,
                ProductMaster = ord.ProductMaster,
                OrderDetailID = ord.OrderDetailID,
                Product = ord.ProductMaster.Name,
                Qty = ord.Qty,
                Price = ord.Price,
                DateCreated = ord.DateCreated,
                TotalWarrantyAmount = ord.WarrantyAmount * ord.Qty,
                WarrantyAmount = ord.WarrantyAmount,
                TaxName = ord.TaxName,
                TaxAmount = ord.TaxAmount,
                ExpectedDate = ord.ExpectedDate,
                TotalAmount = ord.TotalAmount,
                TransType = ord.Order.InventoryTransactionTypeId,
                QtyProcessed = ord.ProcessedQty,
                DirectPostAllowed = !ord.ProductMaster.Serialisable && !ord.ProductMaster.ProcessByPallet && ord.ProductMaster.RequiresBatchNumberOnReceipt != true && ord.ProductMaster.RequiresExpiryDateOnReceipt != true && !_currentDbContext.ProductLocationsMap.Any(m => m.ProductId == ord.ProductId && m.IsDeleted != true)

            }).ToList();

            return results;
        }

        public List<OrderProofOfDelivery> GetOrderProofsByOrderProcessId(int OrderId, int TenantId)
        {
            var orderProcessids = _currentDbContext.OrderProcess.Where(u => u.OrderID == OrderId && u.IsDeleted != true && u.TenentId == TenantId).Select(u => u.OrderProcessID).ToList();
            if (orderProcessids.Count > 0)
            {
                var deliveryProof = _currentDbContext.OrderProofOfDelivery.Where(u => orderProcessids.Contains(u.OrderProcessID ?? 0) && u.IsDeleted != true && u.TenantId == TenantId).ToList();
                return deliveryProof;
            }
            return null;

        }


        public IQueryable<TransferOrderViewModel> GetTransferInOrderViewModelDetails(int toWarehouseId, int tenantId, OrderStatusEnum? type = null)
        {
            var result = _currentDbContext.Order.AsNoTracking()
                  .Where(o => o.IsDeleted != true && o.WarehouseId == toWarehouseId && ((type.HasValue && o.OrderStatusID == type) || !type.HasValue) && o.TenentId == tenantId &&
                              o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn).OrderByDescending(x => x.DateCreated)
                  .Select(p => new TransferOrderViewModel()
                  {
                      OrderID = p.OrderID,
                      OrderNumber = p.OrderNumber,
                      TransferWarehouse = p.TransferWarehouse.WarehouseName,
                      IssueDate = p.IssueDate,
                      DateUpdated = p.DateUpdated,
                      EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                      DateCreated = p.DateCreated,
                      Status = p.OrderStatusID.ToString(),
                      Account = _currentDbContext.Account.Where(s => s.AccountID == p.AccountID)
                          .Select(s => s.CompanyName).FirstOrDefault(),
                      OrderType = nameof(p.InventoryTransactionTypeId),
                      TransType = p.InventoryTransactionTypeId
                  });


            return result;
        }

        public IQueryable<TransferOrderViewModel> GetTransferOutOrderViewModelDetailsIq(int fromWarehouseId, int tenantId, OrderStatusEnum? type = null)
        {
            var result = _currentDbContext.Order.AsNoTracking()
                .Where(o => o.IsDeleted != true && o.WarehouseId == fromWarehouseId && ((type.HasValue && o.OrderStatusID == type) || !type.HasValue) && o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut).OrderByDescending(x => x.DateCreated)
                .Select(p => new TransferOrderViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    TransferWarehouse = p.TransferWarehouse.WarehouseName,
                    IssueDate = p.IssueDate,
                    DateCreated = p.DateCreated,
                    DateUpdated = p.DateUpdated,
                    EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.OrderId == p.OrderID),
                    Status = p.OrderStatusID.ToString(),
                    Account = _currentDbContext.Account.Where(s => s.AccountID == p.AccountID).Select(s => s.CompanyName).FirstOrDefault(),
                    OrderType = nameof(p.InventoryTransactionTypeId),
                    TransType = p.InventoryTransactionTypeId
                });

            return result;
        }


        public List<OrderProcessDetail> GetOrderProcessDetailsByProcessId(int processId)
        {
            return _currentDbContext.OrderProcessDetail
                .Where(o => o.OrderProcessId == processId && o.IsDeleted != true)
                .Include(o => o.ProductMaster)
                .ToList();

        }

        public List<OrderProcess> GetOrderProcesssDeliveriesForWarehouse(int warehouseId, int tenantId)
        {
            return _currentDbContext.OrderProcess.Where(a => a.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn &&
                                                             a.Order.WarehouseId == warehouseId &&
                                                             a.TenentId == tenantId && a.IsDeleted != true).ToList();
        }
        public List<OrderProcess> GetOrderProcessConsignmentsForWarehouse(int warehouseId, int tenantId)
        {
            return _currentDbContext.OrderProcess.Where(a => a.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut
                                                             && a.Order.WarehouseId == warehouseId &&
                                                             a.TenentId == tenantId && a.IsDeleted != true).ToList();
        }

        public List<OrderProcessDetail> GetOrderProcessesDetailsForOrderProduct(int orderId, int productId)
        {
            return _currentDbContext.OrderProcess.Where(a => a.OrderID == orderId && a.IsDeleted != true).SelectMany(a => a.OrderProcessDetail).Where(a => a.ProductId == productId && a.IsDeleted != true).ToList();
        }

        public List<InventoryTransaction> GetAllReturnsForOrderProduct(int orderId, int productId)
        {
            return _currentDbContext.InventoryTransactions.Where(x => x.OrderID == orderId && x.ProductId == productId && x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns).ToList();
        }

        public IQueryable<Order> GetAllOrders(int tenantId, int warehouseId = 0, bool excludeProforma = false, DateTime? reqDate = null, bool includeDeleted = false)
        {
            var warehouse = _currentDbContext.TenantWarehouses.FirstOrDefault(x => x.WarehouseId == warehouseId);
            var parentWarehouseId = 0;
            if (warehouse != null && warehouse.IsMobile == true)
            {
                parentWarehouseId = warehouse.ParentWarehouseId ?? warehouseId;
            }

            return _currentDbContext.Order.Where(m => m.TenentId == tenantId
            && m.WarehouseId == warehouseId && (warehouseId == 0 || (warehouse.IsMobile != true && m.InventoryTransactionTypeId != InventoryTransactionTypeEnum.DirectSales)
                || (warehouse.IsMobile == true && parentWarehouseId > 0 && (m.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales || m.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder)))
            && (!excludeProforma || m.InventoryTransactionTypeId != InventoryTransactionTypeEnum.Proforma) && (m.IsDeleted != true || includeDeleted == true) && (!reqDate.HasValue || (m.DateUpdated ?? m.DateCreated) >= reqDate));
        }

        public IEnumerable<OrderIdsWithStatus> GetAllOrderIdsWithStatus(int tenantId, int warehouseId = 0)
        {
            return _currentDbContext.Order.AsNoTracking().Where(m => m.TenentId == tenantId && (warehouseId == 0 || m.WarehouseId == warehouseId) && m.IsDeleted != true).Select(x => new OrderIdsWithStatus { OrderID = x.OrderID, OrderStatusID = x.OrderStatusID });
        }

        public IQueryable<Order> GetAllOrdersIncludingNavProperties(int tenantId, int warehouseId = 0)
        {
            var result = _currentDbContext.Order.AsNoTracking().Where(m => m.TenentId == tenantId && (warehouseId == 0 || m.WarehouseId == warehouseId) && m.IsDeleted != true)
               .Include(x => x.JobType)
                    .Include(x => x.JobSubType)
                    .Include(x => x.PProperties)
                    .Include(x => x.Account)
                    .Include(x => x.Appointmentses)
                    .Include(x => x.OrderNotes)
                    .Include(x => x.Appointmentses.Select(y => y.AppointmentResources));

            return result;
        }

        public Order UpdateOrderStatus(int orderId, OrderStatusEnum statusId, int userId)
        {
            var schOrder = _currentDbContext.Order.Find(orderId);
            schOrder.OrderStatusID = statusId;
            schOrder.UpdatedBy = userId;
            schOrder.DateUpdated = DateTime.UtcNow;

            _currentDbContext.Order.Attach(schOrder);
            var entry = _currentDbContext.Entry<Order>(schOrder);
            entry.Property(e => e.OrderStatusID).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            _currentDbContext.SaveChanges();
            return schOrder;
        }

        public bool UpdateOrderProcessStatus(int orderProcessId, int UserId)
        {
            bool status = false;
            try
            {

                var orderprocess = _currentDbContext.OrderProcess.Find(orderProcessId);
                orderprocess.OrderProcessStatusId = OrderProcessStatusEnum.Complete;
                orderprocess.UpdatedBy = UserId;
                orderprocess.DateUpdated = DateTime.UtcNow;
                _currentDbContext.OrderProcess.Attach(orderprocess);
                _currentDbContext.Entry(orderprocess).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                status = true;
                return status;

            }
            catch (Exception)
            {

                return status;
            }
        }

        public Order GetValidSalesOrderByOrderNumber(string orderNumber, int tenantId)
        {
            return _currentDbContext.Order.FirstOrDefault(a => a.OrderNumber == orderNumber && a.IsDeleted != true && (a.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder ||
            a.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder) && a.TenentId == tenantId);
        }

        public IQueryable<Order> GetValidSalesOrderByOrderNumber(string orderNumber, int tenantId, int? warehouseId = null)
        {
            return _currentDbContext.Order.Where(a => a.OrderNumber == orderNumber && a.IsDeleted != true && (a.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder ||
            a.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder) && a.TenentId == tenantId);
        }
        public IQueryable<Order> GetValidSalesOrder(int tenantId, int warehouseId)
        {
            return _currentDbContext.Order.Where(a => a.IsDeleted != true && (a.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder ||
            a.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder) && a.TenentId == tenantId && a.WarehouseId == warehouseId && a.OrderStatusID != OrderStatusEnum.Complete);
        }
        public IQueryable<ProductMaster> GetAllValidProduct(int tenantId)
        {
            return _currentDbContext.ProductMaster.Where(a => a.IsDeleted != true && a.TenantId == tenantId);
        }

        public List<OrderDetail> GetOrderDetailsForProduct(int orderId, int productId, int TenantId)
        {
            return _currentDbContext.OrderDetail.Where(a => a.OrderID == orderId && a.ProductId == productId && a.IsDeleted != true && a.TenentId == TenantId).ToList();
        }

        public OrderDetail SaveOrderDetail(OrderDetail podetail, int tenantId, int userId)
        {


            podetail.DateUpdated = DateTime.UtcNow;
            podetail.TenentId = tenantId;
            podetail.UpdatedBy = userId;
            decimal total = 0;

            if (podetail.OrderDetailID > 0)
            {
                podetail.IsDeleted = false;

                var detail = _currentDbContext.OrderDetail.Find(podetail.OrderDetailID);
                if (detail != null)
                {
                    detail.ProductId = podetail.ProductId;
                    detail.WarrantyID = podetail.WarrantyID;
                    detail.Price = podetail.Price;
                    detail.Qty = podetail.Qty;
                    detail.TaxID = podetail.TaxID;
                    detail.Notes = podetail.Notes;
                    detail.UpdatedBy = userId;
                    detail.DateUpdated = DateTime.UtcNow;
                    detail.TaxAmount = podetail.TaxAmount;
                    detail.TotalAmount = podetail.TotalAmount;

                }

                _currentDbContext.Entry(detail).State = EntityState.Modified;
                podetail = detail;
            }
            else
            {
                podetail.ProductMaster = null;
                podetail.TaxName = null;
                podetail.Warranty = null;
                podetail.CreatedBy = userId;
                podetail.DateCreated = DateTime.UtcNow;
                total = podetail.TaxAmount;
                _currentDbContext.OrderDetail.Add(podetail);
            }
            _currentDbContext.SaveChanges();
            var po = _currentDbContext.Order.Find(podetail.OrderID);
            po.DateUpdated = DateTime.UtcNow;
            po.UpdatedBy = userId;
            po.OrderTotal = _currentDbContext.OrderDetail.Where(u => u.OrderID == podetail.OrderID).Select(u => u.TotalAmount).DefaultIfEmpty(0).Sum();
            po.OrderCost = po.OrderTotal;
            _currentDbContext.Entry(po).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return podetail;
        }

        public void RemoveOrderDetail(int orderDetailId, int tenantId, int userId)
        {
            var podetail = _currentDbContext.OrderDetail.Find(orderDetailId);

            podetail.IsDeleted = true;

            SaveOrderDetail(podetail, tenantId, userId);
        }

        public OrderDetail GetOrderDetailsById(int orderDetailId)
        {
            return _currentDbContext.OrderDetail.AsNoTracking().FirstOrDefault(a => a.OrderDetailID == orderDetailId && a.IsDeleted != true);
        }

        public List<OrderDetail> GetAllOrderDetailsForOrderAccount(int supplierAccountId, int poOrderId, int tenantId)
        {
            return _currentDbContext.OrderDetail.Where(
                p => p.Order.AccountID == supplierAccountId && p.OrderID != poOrderId && p.IsDeleted != true &&
                     (p.ProductMaster.IsActive == true) && (p.ProductMaster.IsDeleted != true) &&
                     (p.ProductMaster.TenantId == tenantId)).ToList();
        }

        public List<OrderProcessDetail> GetAllOrderProcessesByOrderDetailId(int orderDetailId, int warehouseId)
        {
            return _currentDbContext.OrderProcessDetail
                .Where(x => x.OrderDetailID == orderDetailId && x.IsDeleted != true && x.OrderProcess.WarehouseId == warehouseId).ToList();
        }

        public IQueryable<OrderProcess> GetAllOrderProcesses(DateTime? updatedAfter, int? orderId = 0, OrderProcessStatusEnum? orderProcessStatusId = null, InventoryTransactionTypeEnum? transTypeId = null, bool includeDeleted = false)
        {
            return _currentDbContext.OrderProcess.Where(x => (updatedAfter == null || x.DateCreated > updatedAfter || x.DateUpdated > updatedAfter)
                 && (!transTypeId.HasValue || x.Order.InventoryTransactionTypeId == transTypeId)
                 && (x.IsDeleted != true || includeDeleted == true) && (orderId == null || orderId == 0 || x.OrderID == orderId) && (orderProcessStatusId == null || orderProcessStatusId == 0 || x.OrderProcessStatusId == orderProcessStatusId));
        }

        public List<OrderProcessDetail> GetAllOrderProcessesDetails(DateTime? updatedAfter, int? orderProcessId = 0)
        {
            return _currentDbContext.OrderProcessDetail
                .Where(x => (!updatedAfter.HasValue || x.DateCreated > updatedAfter || x.DateUpdated > updatedAfter) && x.IsDeleted != true && (!orderProcessId.HasValue || x.OrderProcessId == orderProcessId)).ToList();
        }

        public OrderProcess GetOrderProcessByDeliveryNumber(int orderId, InventoryTransactionTypeEnum InventoryTransactionTypeId, string deliveryNumber, int userId, DateTime? createdDate = null, int warehouseId = 0, AccountShipmentInfo shipmentInfo = null)
        {
            var consolidateOrderProcess = false;
            InventoryTransactionTypeEnum? transtypeId = InventoryTransactionTypeId;
            transtypeId = transtypeId == 0 ? null : transtypeId;
            if (warehouseId > 0)
            {
                consolidateOrderProcess = _currentDbContext.TenantWarehouses.FirstOrDefault(x => x.WarehouseId == warehouseId && x.IsDeleted != true).ConsolidateOrderProcesses;
            }

            var receivepo = _currentDbContext.OrderProcess.Where(m => m.OrderID == orderId && m.IsDeleted != true && (!transtypeId.HasValue || m.InventoryTransactionTypeId == transtypeId)).ToList().FirstOrDefault(m => consolidateOrderProcess == true ||
            (!string.IsNullOrEmpty(m.DeliveryNO) && !string.IsNullOrEmpty(deliveryNumber) && m.DeliveryNO.Trim().Equals(deliveryNumber.Trim(), StringComparison.OrdinalIgnoreCase)));

            if (receivepo == null)
            {
                var order = GetOrderById(orderId);
                receivepo = new OrderProcess()
                {
                    OrderID = orderId,
                    CreatedBy = userId,
                    DeliveryNO = deliveryNumber,
                    InventoryTransactionTypeId = transtypeId,
                    TenentId = order.TenentId,
                    OrderProcessStatusId = OrderProcessStatusEnum.Complete,
                    DateCreated = createdDate ?? DateTime.UtcNow,
                    WarehouseId = warehouseId,
                    IsDeleted = false,
                    ShipmentAddressLine1 = order.ShipmentAddressLine1,
                    ShipmentAddressLine2 = order.ShipmentAddressLine2,
                    ShipmentAddressLine3 = order.ShipmentAddressLine3,
                    ShipmentAddressLine4 = order.ShipmentAddressLine4,
                    ShipmentAddressPostcode = order.ShipmentAddressPostcode,
                };
                if (shipmentInfo != null)
                {
                    receivepo.ShipmentAddressLine1 = shipmentInfo.ShipmentAddressLine1;
                    receivepo.ShipmentAddressLine2 = shipmentInfo.ShipmentAddressLine2;
                    receivepo.ShipmentAddressLine3 = shipmentInfo.ShipmentAddressLine3;
                    receivepo.ShipmentAddressLine4 = shipmentInfo.ShipmentAddressLine4;
                    receivepo.ShipmentAddressPostcode = shipmentInfo.ShipmentAddressPostcode;
                    receivepo.FSC = shipmentInfo.FSC;
                    receivepo.PEFC = shipmentInfo.PEFC;
                }
                _currentDbContext.Entry(receivepo).State = EntityState.Added;
                _currentDbContext.SaveChanges();
            }

            else
            {
                receivepo.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(receivepo).State = EntityState.Modified;
                _currentDbContext.SaveChanges();

            }


            return receivepo;
        }

        public OrderProcess CreateOrderProcess(int orderId, string deliveryNo, int[] product, decimal[] qty,
            decimal[] qtyReceived, int[] lines, string serialStamp, int currentUserId, int currentTenantId, int warehouseId)
        {
            if (qtyReceived.Length > 0 && orderId > 0)
            {
                Order po = _currentDbContext.Order.Find(orderId);
                var receivepo = GetOrderProcessByDeliveryNumber(orderId, 0, deliveryNo, currentUserId, null, warehouseId);

                if (receivepo == null)
                {
                    receivepo = new OrderProcess
                    {
                        OrderID = orderId,
                        DeliveryNO = deliveryNo.Trim(),
                        WarehouseId = warehouseId,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        TenentId = currentTenantId,
                        CreatedBy = currentUserId,
                        UpdatedBy = currentUserId,
                        IsDeleted = false,
                        OrderProcessStatusId = OrderProcessStatusEnum.Active,
                        ShipmentAddressLine1 = po.ShipmentAddressLine1,
                        ShipmentAddressLine2 = po.ShipmentAddressLine2,
                        ShipmentAddressLine3 = po.ShipmentAddressLine3,
                        ShipmentAddressLine4 = po.ShipmentAddressLine4,
                        ShipmentAddressPostcode = po.ShipmentAddressPostcode

                    };
                    _currentDbContext.OrderProcess.Add(receivepo);
                }
                else
                {
                    receivepo.DateUpdated = DateTime.UtcNow;
                    receivepo.UpdatedBy = currentUserId;
                    _currentDbContext.Entry(receivepo).State = EntityState.Modified;
                }

                po.DateUpdated = DateTime.UtcNow;
                po.UpdatedBy = currentUserId;

                _currentDbContext.Order.Attach(po);
                var entry = _currentDbContext.Entry(po);
                entry.Property(e => e.DateUpdated).IsModified = true;
                entry.Property(e => e.UpdatedBy).IsModified = true;
                _currentDbContext.SaveChanges();

                for (int i = 0; i < product.Length; i++)
                {
                    if (qtyReceived[i] > 0)
                    {
                        var receivepodetail = new OrderProcessDetail();
                        receivepodetail.OrderProcessDetailID = lines[i];
                        receivepodetail.ProductId = product[i];
                        receivepodetail.OrderProcessId = receivepo.OrderProcessID;
                        receivepodetail.QtyProcessed = qtyReceived[i];
                        int OrderID = _currentDbContext.Order.Where(x => x.OrderID == orderId).Select(x => x.OrderID).FirstOrDefault();
                        InventoryTransactionTypeEnum typeId = _currentDbContext.Order.Where(x => x.OrderID == orderId).Select(x => x.InventoryTransactionTypeId).FirstOrDefault();
                        var p1 = product[i];
                        var serializable = _currentDbContext.ProductMaster.Where(p => p.ProductId == p1)
                            .Select(p => p.Serialisable).FirstOrDefault();
                        if (!serializable)
                        {
                            ProductStockTransactionRecord(product[i], typeId, qtyReceived[i], OrderID, currentTenantId, currentUserId, warehouseId);
                        }

                        receivepodetail.DateCreated = DateTime.UtcNow;
                        receivepodetail.DateUpdated = DateTime.UtcNow;
                        receivepodetail.TenentId = currentTenantId;
                        receivepodetail.CreatedBy = currentUserId;
                        receivepodetail.UpdatedBy = currentUserId;
                        receivepodetail.IsDeleted = false;
                        _currentDbContext.OrderProcessDetail.Add(receivepodetail);
                    }
                }
                _currentDbContext.SaveChanges();
                return receivepo;
            }
            return null;

        }

        public OrderProcess SaveOrderProcess(int orderProcessId, int[] product, decimal[] qty, decimal[] qtyReceived,
            int[] lines, int currentUserId, int currentTenantId, int warehouseId)
        {

            OrderProcess receivepo = GetOrderProcessByOrderProcessId(orderProcessId);

            if (qtyReceived.Length > 0)
            {

                for (int i = 0; i < product.Length; i++)
                {
                    var pdetail = lines[i];

                    var receviceline = GetOrderProcessDetailById(pdetail);

                    if (receviceline != null)
                    {
                        if (qtyReceived[i] < 1)
                        {

                            receviceline.QtyProcessed = 0;
                            receviceline.IsDeleted = true;
                            receviceline.DateUpdated = DateTime.UtcNow;
                            receviceline.UpdatedBy = currentUserId;

                            _currentDbContext.OrderProcessDetail.Attach(receviceline);
                            var entry = _currentDbContext.Entry(receviceline);
                            entry.Property(e => e.QtyProcessed).IsModified = true;
                            entry.Property(e => e.IsDeleted).IsModified = true;
                            entry.Property(e => e.DateUpdated).IsModified = true;
                            entry.Property(e => e.UpdatedBy).IsModified = true;
                        }
                        else
                        {
                            receviceline.QtyProcessed = qtyReceived[i];
                            receviceline.DateUpdated = DateTime.UtcNow;
                            receviceline.UpdatedBy = currentUserId;

                            _currentDbContext.OrderProcessDetail.Attach(receviceline);
                            var entry = _currentDbContext.Entry(receviceline);
                            entry.Property(e => e.QtyProcessed).IsModified = true;
                            entry.Property(e => e.DateUpdated).IsModified = true;
                            entry.Property(e => e.UpdatedBy).IsModified = true;

                        }

                    }

                    else
                    {
                        if (qtyReceived[i] > 0)
                        {
                            OrderProcessDetail receivepodetail = new OrderProcessDetail();
                            receivepodetail.OrderProcessDetailID = lines[i];
                            receivepodetail.ProductId = product[i];
                            receivepodetail.OrderProcessId = orderProcessId;
                            receivepodetail.QtyProcessed = qtyReceived[i];

                            receivepodetail.DateCreated = DateTime.UtcNow;
                            receivepodetail.DateUpdated = DateTime.UtcNow;
                            receivepodetail.TenentId = currentTenantId;
                            receivepodetail.CreatedBy = currentUserId;
                            receivepodetail.UpdatedBy = currentUserId;
                            receivepodetail.IsDeleted = false;
                            _currentDbContext.OrderProcessDetail.Add(receivepodetail);

                            int OrderID = _currentDbContext.Order.Where(x => x.OrderID == receivepo.OrderID).Select(x => x.OrderID).FirstOrDefault();
                            InventoryTransactionTypeEnum typeId = _currentDbContext.Order.Where(x => x.OrderID == receivepo.OrderID).Select(x => x.InventoryTransactionTypeId).FirstOrDefault();

                            ProductStockTransactionRecord(product[i], typeId, qtyReceived[i], OrderID, currentTenantId,
                                currentUserId, warehouseId);

                        }
                    }
                }

                receivepo.DateUpdated = DateTime.UtcNow;
                receivepo.UpdatedBy = currentUserId;

                _currentDbContext.OrderProcess.Attach(receivepo);
                var entry1 = _currentDbContext.Entry(receivepo);
                entry1.Property(e => e.DateUpdated).IsModified = true;
                entry1.Property(e => e.UpdatedBy).IsModified = true;
                _currentDbContext.SaveChanges();
            }
            return receivepo;
        }

        public OrdersSync SaveOrderProcessSync(OrderProcessesSync item, Terminals terminal)
        {
            var groupToken = Guid.NewGuid();
            var serialProcessStatus = new SerialProcessStatus();

            if (item.SaleMade && (item.OrderProcessDetails == null || item.OrderProcessDetails.Count < 1))
            {
                return new OrdersSync() { RequestSuccess = false, RequestStatus = "Sale has been made, but no products specified with this order." };
            }

            if ((!item.OrderID.HasValue && (!item.OrderToken.HasValue || item.OrderToken == new Guid())) || !item.SaleMade)
            {
                if (item.ProgressInfo != null)
                {
                    try
                    {
                        if (item.ProgressInfo.RouteProgressId == Guid.Empty)
                        {
                            item.ProgressInfo.RouteProgressId = Guid.NewGuid();
                        }
                        var progressInfo = _mapper.Map(item.ProgressInfo, new MarketRouteProgress());
                        progressInfo.Latitude = item.ProgressInfo.Latitude;
                        progressInfo.Longitude = item.ProgressInfo.Longitude;
                        progressInfo.DateCreated = item.DateCreated == DateTime.MinValue ? DateTime.UtcNow : item.DateCreated;
                        progressInfo.DateUpdated = DateTime.UtcNow;
                        progressInfo.TenantId = terminal.TenantId;
                        _currentDbContext.MarketRouteProgresses.Add(progressInfo);
                        _currentDbContext.SaveChanges();
                        return new OrdersSync() { RequestSuccess = true, RequestStatus = "No order information provided. But tracking information recorded successfully." };

                    }
                    catch (Exception e)
                    {
                        return new OrdersSync() { RequestSuccess = false, RequestStatus = "No order information provided. Progress info also not well formed. Request failed. Error : " + e.Message };
                    }

                }

            }

            if (item.OrderToken.HasValue && (!item.OrderID.HasValue || item.OrderID == 0))
            {
                var order = new Order
                {
                    OrderNumber = GenerateNextOrderNumber((InventoryTransactionTypeEnum)item.InventoryTransactionTypeId, terminal.TenantId),
                    AccountID = item.AccountID,
                    Note = item.OrderNotes,
                    InventoryTransactionTypeId = item.InventoryTransactionTypeId,
                    DateCreated = item.DateCreated,
                    CreatedBy = item.CreatedBy,
                    OrderStatusID = (item.OrderStatusID.HasValue && item.OrderStatusID > 0) ? item.OrderStatusID.Value : OrderStatusEnum.Active,
                    IsCancel = false,
                    IsActive = false,
                    OrderTotal = (item.OrderProcessDetails.Sum(m => m.Price * m.QtyProcessed) + item.OrderProcessDetails.Sum(m => m.TaxAmount) + item.OrderProcessDetails.Sum(m => m.WarrantyAmount)) - item.OrderProcessDiscount,
                    Posted = false,
                    IsShippedToTenantMainLocation = false,
                    TenentId = terminal.TenantId,
                    WarehouseId = terminal.WarehouseId,
                    OrderDiscount = item.OrderProcessDiscount,
                    InvoiceNo = item.TerminalInvoiceNumber,
                    OrderToken = item.OrderToken
                };

                var orderDetails = item.OrderProcessDetails.Select(m => new OrderDetail()
                {
                    DateCreated = m.DateCreated ?? DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    IsDeleted = m.IsDeleted,
                    OrderDetailStatusId = m.OrderDetailStatusID > 0 ? m.OrderDetailStatusID : OrderStatusEnum.Active,
                    Price = m.Price,
                    ProductId = m.ProductId,
                    Qty = m.QtyProcessed,
                    TenentId = terminal.TenantId,
                    TotalAmount = (m.Price * m.QtyProcessed) + m.TaxAmount + m.WarrantyAmount,
                    CreatedBy = item.CreatedBy,
                    WarrantyID = m.WarrantyID,
                    WarrantyAmount = m.WarrantyAmount,
                    TaxID = m.TaxID,
                    TaxAmount = m.TaxAmount
                }).ToList();

                order = CreateOrder(order, terminal.TenantId, terminal.WarehouseId, order.CreatedBy, orderDetails);

                //Order has to be created and processed immediately
                // If OrderStatus is AwaitingAuthorisation then dont process the items
                if (item?.OrderStatusID != OrderStatusEnum.AwaitingAuthorisation)
                {
                    var process = new OrderProcess()
                    {
                        DateCreated = item.DateCreated,
                        DateUpdated = item.DateUpdated,
                        DeliveryNO = string.IsNullOrEmpty(item.DeliveryNo) ? "T-" + terminal.TermainlSerial + "-" + Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 15) : item.DeliveryNo,
                        TenentId = terminal.TenantId,
                        WarehouseId = terminal.WarehouseId,
                        IsActive = true,
                        OrderProcessStatusId = OrderProcessStatusEnum.Complete,
                        InventoryTransactionTypeId = item.InventoryTransactionTypeId,
                        OrderID = order.OrderID,
                        ShipmentAddressLine1 = order.ShipmentAddressLine1,
                        ShipmentAddressLine2 = order.ShipmentAddressLine2,
                        ShipmentAddressLine3 = order.ShipmentAddressLine3,
                        ShipmentAddressLine4 = order.ShipmentAddressLine4,
                        ShipmentAddressPostcode = order.ShipmentAddressPostcode,
                        CreatedBy = item.CreatedBy
                    };

                    if (item.AccountTransactionInfo != null)
                    {
                        order.AmountPaidByAccount = item.AccountTransactionInfo.PaidAmount;
                        order.AccountBalanceOnPayment = item.AccountTransactionInfo.FinalAccountBalance;
                        order.AccountPaymentModeId = item.AccountTransactionInfo?.AccountPaymentModeId;
                        order.AccountBalanceBeforePayment = item.AccountTransactionInfo.OpeningAccountBalance;
                    }

                    _currentDbContext.OrderProcess.Add(process);
                    _currentDbContext.SaveChanges();


                    foreach (var op in item.OrderProcessDetails)
                    {
                        if (op.Serials != null && op.Serials.Count > 0)
                        {
                            var serialResult = UpdateSerialStockStatus(op.Serials, op.ProductId, item.DeliveryNo, item.OrderID ?? 0, op.OrderDetailID ?? 0, 0, item.InventoryTransactionTypeId, item.CreatedBy,
                                terminal.TenantId, terminal.WarehouseId, terminal.TerminalId);
                            serialProcessStatus.ProcessedSerials.AddRange(serialResult.ProcessedSerials);
                            serialProcessStatus.RejectedSerials.AddRange(serialResult.RejectedSerials);
                        }
                        else
                        {
                            var o = new OrderProcessDetail()
                            {
                                CreatedBy = op.CreatedBy,
                                DateCreated = DateTime.UtcNow,
                                UpdatedBy = op.UpdatedBy,
                                DateUpdated = op.DateUpdated,
                                OrderDetailID = op.OrderDetailID,
                                ProductId = op.ProductId,
                                QtyProcessed = op.QtyProcessed,
                                TenentId = op.TenentId,
                                BatchNumber = op.BatchNumber,
                                ExpiryDate = op.ExpiryDate,
                                OrderProcessId = process.OrderProcessID,
                                TerminalId = terminal.TerminalId
                            };

                            _currentDbContext.OrderProcessDetail.Add(o);

                            _currentDbContext.SaveChanges();
                            Inventory.StockTransactionApi(o.ProductId, item.InventoryTransactionTypeId,
                                o.QtyProcessed, o.OrderProcess.OrderID, terminal.TenantId, terminal.WarehouseId,
                                op.CreatedBy, null, null, terminal.TerminalId,
                                orderProcessId: (o?.OrderProcess?.OrderProcessID),
                                OrderProcessDetialId: o?.OrderDetailID);
                        }
                    }
                }
                else
                {
                    order.OrderStatusID = OrderStatusEnum.AwaitingAuthorisation;
                }

                if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn || item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut)
                {
                    order.OrderGroupToken = groupToken;
                    order.WarehouseId = terminal.WarehouseId;
                    order.OrderStatusID = OrderStatusEnum.Active;
                    order.TransferWarehouseId = terminal.TenantWarehous.ParentWarehouseId;
                    if (order.DateCreated == null || order.DateCreated == DateTime.MinValue)
                    {
                        order.DateCreated = DateTime.UtcNow;
                    }

                    item.OrderID = order.OrderID;


                    item.InventoryTransactionTypeId = (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn) ? InventoryTransactionTypeEnum.TransferOut : InventoryTransactionTypeEnum.TransferIn;

                    var outOrder = new Order
                    {
                        OrderNumber = GenerateNextOrderNumber((InventoryTransactionTypeEnum)item.InventoryTransactionTypeId, terminal.TenantId),
                        AccountID = item.AccountID,
                        Note = item.OrderNotes,
                        InventoryTransactionTypeId = item.InventoryTransactionTypeId,
                        DateCreated = DateTime.UtcNow,
                        CreatedBy = item.CreatedBy,
                        OrderStatusID = OrderStatusEnum.Active,
                        IsCancel = false,
                        IsActive = false,
                        OrderTotal = item.OrderProcessDetails.Sum(m => m.Price),
                        Posted = false,
                        IsShippedToTenantMainLocation = false,
                        TenentId = terminal.TenantId,
                        WarehouseId = terminal.TenantWarehous.ParentWarehouseId,
                        OrderGroupToken = groupToken,
                        TransferWarehouseId = terminal.WarehouseId
                    };

                    if (order.DateCreated == null || order.DateCreated == DateTime.MinValue)
                    {
                        order.DateCreated = DateTime.UtcNow;
                    }

                    var outOrderDetails = item.OrderProcessDetails.Select(m => new OrderDetail()
                    {
                        DateCreated = m.DateCreated ?? DateTime.UtcNow,
                        IsDeleted = m.IsDeleted,
                        OrderDetailStatusId = OrderStatusEnum.Active,
                        Price = m.Price,
                        ProductId = m.ProductId,
                        Qty = m.QtyProcessed,
                        TenentId = terminal.TenantId,
                        TotalAmount = m.Price * m.QtyProcessed,
                        WarrantyID = m.WarrantyID,
                        WarrantyAmount = m.WarrantyAmount,
                        TaxID = m.TaxID,
                        TaxAmount = m.TaxAmount,
                        CreatedBy = m.CreatedBy
                    }).ToList();


                    outOrder = CreateOrder(outOrder, terminal.TenantId, terminal.TenantWarehous.ParentWarehouseId ?? terminal.WarehouseId, 0, outOrderDetails);
                }

                if (item.ProgressInfo != null)
                {
                    var progressInfo = _mapper.Map(item.ProgressInfo, new MarketRouteProgress());
                    progressInfo.Latitude = item.ProgressInfo.Latitude;
                    progressInfo.Longitude = item.ProgressInfo.Longitude;
                    progressInfo.OrderId = order.OrderID;
                    progressInfo.DateCreated = item.DateCreated == DateTime.MinValue ? DateTime.UtcNow : item.DateCreated;
                    progressInfo.DateUpdated = DateTime.UtcNow;
                    progressInfo.TenantId = terminal.TenantId;
                    if (progressInfo.RouteProgressId == Guid.Empty) { progressInfo.RouteProgressId = Guid.NewGuid(); }
                    _currentDbContext.MarketRouteProgresses.Add(progressInfo);
                    _currentDbContext.SaveChanges();
                }

                if (item.OrderProofOfDeliverySync != null)
                {
                    foreach (var orderprocessIds in order.OrderProcess)
                    {


                        OrderProofOfDelivery orderProofOfDelivery = new OrderProofOfDelivery();
                        orderProofOfDelivery.DateCreated = DateTime.UtcNow;
                        orderProofOfDelivery.DateUpdated = DateTime.UtcNow;
                        orderProofOfDelivery.SignatoryName = item.OrderProofOfDeliverySync?.SignatoryName ?? "";
                        orderProofOfDelivery.OrderProcessID = orderprocessIds.OrderProcessID;
                        orderProofOfDelivery.FileName = ByteToFile(item.OrderProofOfDeliverySync?.FileContent ?? null);
                        orderProofOfDelivery.TenantId = item.TenentId;
                        orderProofOfDelivery.NoOfCases = item.OrderProofOfDeliverySync?.NoOfCases ?? 0;
                        _currentDbContext.OrderProofOfDelivery.Add(orderProofOfDelivery);

                    }

                    _currentDbContext.SaveChanges();


                }


                _currentDbContext.SaveChanges();
                var result = _mapper.Map(order, new OrdersSync());
                result.RequestSuccess = true;
                result.RequestStatus = "Order created successfully";
                result.OrderID = order.OrderID;
                result.OrderStatusID = order.OrderStatusID;

                if (item.AccountTransactionInfo != null && item.OrderStatusID != OrderStatusEnum.AwaitingAuthorisation)
                {
                    if (item.AccountTransactionInfo.AccountId > 0)
                    {
                        if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales)
                        {
                            //create an account transaction for order
                            UpdateAccountTransactionInfo(item, terminal.TenantId, AccountTransactionTypeEnum.InvoicedToAccount, order.OrderID);
                        }
                        else if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns || item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WastedReturn)
                        {
                            //create an account transaction for returns order
                            UpdateAccountTransactionInfo(item, terminal.TenantId, AccountTransactionTypeEnum.Refund, order.OrderID);
                        }
                    }

                    if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales)
                    {

                        // create a paid transaction for the amount customer paid if paid amount is greater then zero
                        var transaction = UpdateAccountTransactionInfo(item, terminal.TenantId, AccountTransactionTypeEnum.PaidByAccount, order.OrderID);

                        if (item.AccountTransactionFiles != null)
                        {
                            foreach (var transFile in item.AccountTransactionFiles)
                            {
                                transFile.AccountTransactionID = transaction.AccountTransactionId;
                                _accountServices.AddAccountTransactionFile(transFile, terminal.TenantId);
                            }
                        }
                    }

                }
                return result;
            }

            else if (!item.OrderToken.HasValue && item.OrderID > 0)
            {
                var order = GetOrderById(item.OrderID.Value);

                var orderProcess = GetOrderProcessByDeliveryNumber(item.OrderID.Value, item.InventoryTransactionTypeId, string.IsNullOrEmpty(item.DeliveryNo) ? "T-" + terminal.TermainlSerial + "-"
                    + Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 15) : item.DeliveryNo, item.CreatedBy, null, terminal.WarehouseId);
                foreach (var op in item.OrderProcessDetails)
                {
                    if (op.Serials != null && op.Serials.Count > 0)
                    {
                        var serialResult = UpdateSerialStockStatus(op.Serials, op.ProductId, item.DeliveryNo, item.OrderID ?? 0, op.OrderDetailID ?? 0, 0, item.InventoryTransactionTypeId, item.CreatedBy,
                            terminal.TenantId, terminal.WarehouseId, terminal.TerminalId);
                        serialProcessStatus.ProcessedSerials.AddRange(serialResult.ProcessedSerials);
                        serialProcessStatus.RejectedSerials.AddRange(serialResult.RejectedSerials);
                    }
                    else
                    {
                        var o = new OrderProcessDetail()
                        {
                            CreatedBy = op.CreatedBy,
                            DateCreated = DateTime.UtcNow,
                            UpdatedBy = op.UpdatedBy,
                            DateUpdated = op.DateUpdated,
                            OrderDetailID = op.OrderDetailID,
                            ProductId = op.ProductId,
                            QtyProcessed = op.QtyProcessed,
                            TenentId = op.TenentId,
                            BatchNumber = op.BatchNumber,
                            ExpiryDate = op.ExpiryDate,
                            OrderProcessId = orderProcess.OrderProcessID,
                            TerminalId = terminal.TerminalId
                        };

                        orderProcess.DateUpdated = DateTime.UtcNow;
                        orderProcess.UpdatedBy = op.UpdatedBy;
                        orderProcess.PickContainerCode = item.PickContainerCode;
                        _currentDbContext.OrderProcessDetail.Add(o);
                        _currentDbContext.SaveChanges();
                        // handle Pallet tracking info

                        if (op.PalleTrackingProcess != null)
                        {
                            foreach (var pallet in op.PalleTrackingProcess)
                            {
                                ProductMaster product = _currentDbContext.ProductMaster.AsNoTracking().FirstOrDefault(x => x.ProductId == op.ProductId);
                                int productsPerCase = product.ProductsPerCase == null ? 1 : product.ProductsPerCase.Value;

                                if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder)
                                {
                                    var newpallet = _currentDbContext.PalletTracking.Find(pallet.PalletTrackingId);
                                    if (newpallet != null)
                                    {
                                        newpallet.Status = PalletTrackingStatusEnum.Active;
                                        newpallet.RemainingCases = (pallet.ProcessedQuantity);
                                        newpallet.DateUpdated = DateTime.UtcNow;

                                        decimal quantity = pallet.ProcessedQuantity * productsPerCase;

                                        Inventory.StockTransactionApi(o.ProductId, item.InventoryTransactionTypeId, quantity, o.OrderProcess.OrderID, terminal.TenantId, terminal.WarehouseId, op.CreatedBy,
                                            null, pallet.PalletTrackingId, terminal.TerminalId, orderProcessId: (o.OrderProcess?.OrderProcessID), OrderProcessDetialId: (o?.OrderProcessDetailID));
                                    }
                                    else
                                    {
                                        caError error = new caError();

                                        error.ErrorTtile = "Error Posting Purchase Order from Terminal with wrong pallet Id";
                                        error.ErrorMessage = $"Wrong Pallet Id. Pallet Tracking Id = {pallet.PalletTrackingId}, processed quantity = {pallet.ProcessedQuantity}";
                                        error.ErrorDetail = "Wrong Pallet Id ";
                                        error.ErrorController = "OrderService";
                                        error.ErrorAction = "PostOrder";
                                        error.SimpleErrorLogWriter();
                                    }


                                }
                                else if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder)
                                {
                                    var newpallet = _currentDbContext.PalletTracking.Find(pallet.PalletTrackingId);
                                    if (newpallet != null)
                                    {
                                        newpallet.RemainingCases = newpallet.RemainingCases - (pallet.ProcessedQuantity);
                                        newpallet.DateUpdated = DateTime.UtcNow;
                                        decimal quantity = pallet.ProcessedQuantity * productsPerCase;

                                        Inventory.StockTransactionApi(o.ProductId, item.InventoryTransactionTypeId, quantity, o.OrderProcess.OrderID, terminal.TenantId, terminal.WarehouseId, op.CreatedBy,
                                            null, pallet.PalletTrackingId, terminal.TerminalId, orderProcessId: (o.OrderProcess?.OrderProcessID), OrderProcessDetialId: (o?.OrderProcessDetailID));
                                    }

                                    else
                                    {
                                        caError error = new caError();

                                        error.ErrorTtile = "Error Posting Sales Order from Terminal with wrong pallet Id";
                                        error.ErrorMessage = $"Wrong Pallet Id. Pallet Tracking Id = {pallet.PalletTrackingId}, processed quantity = {pallet.ProcessedQuantity}";
                                        error.ErrorDetail = "Wrong Pallet Id ";
                                        error.ErrorController = "OrderService";
                                        error.ErrorAction = "PostOrder";
                                        error.SimpleErrorLogWriter();
                                    }
                                }
                            }
                        }

                        else
                        {
                            _currentDbContext.SaveChanges();
                            Inventory.StockTransactionApi(o.ProductId, item.InventoryTransactionTypeId, o.QtyProcessed, o.OrderProcess.OrderID, terminal.TenantId, terminal.WarehouseId, op.CreatedBy, null, null, terminal.TerminalId, orderProcessId: (o.OrderProcess?.OrderProcessID), OrderProcessDetialId: (o?.OrderProcessDetailID));
                        }

                    }
                }



                // if Warehouse AutotransferOrder flag is true then process other related Transfer order automatically

                if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn || item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut)
                {
                    var reverseOrder = _currentDbContext.Order.Where(x => x.OrderGroupToken == order.OrderGroupToken && x.OrderID != order.OrderID).FirstOrDefault();

                    if (reverseOrder != null && reverseOrder?.Warehouse?.AutoTransferOrders == true)
                    {
                        var reverseOrderProcess = GetOrderProcessByDeliveryNumber(reverseOrder.OrderID, reverseOrder.InventoryTransactionTypeId, string.IsNullOrEmpty(item.DeliveryNo) ? "T-" + terminal.TermainlSerial + "-" +
                            Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 15) : item.DeliveryNo, item.CreatedBy, null, reverseOrder.WarehouseId ?? 0);

                        foreach (var op in item.OrderProcessDetails)
                        {
                            var reverseOrderdetail = _currentDbContext.OrderDetail.Where(x => x.OrderID == reverseOrder.OrderID && x.ProductId == op.ProductId).FirstOrDefault();

                            if (op.Serials != null && op.Serials.Count > 0)
                            {
                                var serialResult = UpdateSerialStockStatus(op.Serials, op.ProductId, item.DeliveryNo, reverseOrder.OrderID, reverseOrderdetail.OrderDetailID, 0,
                                    reverseOrder.InventoryTransactionTypeId, item.CreatedBy, terminal.TenantId, reverseOrder.WarehouseId ?? terminal.WarehouseId, terminal.TerminalId);

                                serialProcessStatus.ProcessedSerials.AddRange(serialResult.ProcessedSerials);
                                serialProcessStatus.RejectedSerials.AddRange(serialResult.RejectedSerials);
                            }
                            else
                            {
                                var o = new OrderProcessDetail()
                                {
                                    CreatedBy = op.CreatedBy,
                                    DateCreated = DateTime.UtcNow,
                                    UpdatedBy = op.UpdatedBy,
                                    DateUpdated = op.DateUpdated,
                                    OrderDetailID = reverseOrderdetail.OrderDetailID,
                                    ProductId = op.ProductId,
                                    QtyProcessed = op.QtyProcessed,
                                    TenentId = op.TenentId,
                                    BatchNumber = op.BatchNumber,
                                    ExpiryDate = op.ExpiryDate,
                                    OrderProcessId = reverseOrderProcess.OrderProcessID,
                                    TerminalId = terminal.TerminalId
                                };

                                _currentDbContext.OrderProcessDetail.Add(o);
                                _currentDbContext.SaveChanges();
                                Inventory.StockTransactionApi(o.ProductId, reverseOrder.InventoryTransactionTypeId, o.QtyProcessed, reverseOrder.OrderID, terminal.TenantId, (reverseOrder.WarehouseId ?? terminal.WarehouseId),
                                    op.CreatedBy, null, null, terminal.TerminalId, orderProcessId: (reverseOrderProcess?.OrderProcessID), OrderProcessDetialId: (o?.OrderProcessDetailID));

                            }
                        }
                    }

                }

                _currentDbContext.SaveChanges();
                item.OrderProcessID = orderProcess.OrderProcessID;
                if (item.AccountTransactionInfo != null && item.OrderStatusID != OrderStatusEnum.AwaitingAuthorisation)
                {
                    if (item.AccountTransactionInfo.AccountId > 0)
                    {
                        if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales)
                        {
                            //create an account transaction for order
                            UpdateAccountTransactionInfo(item, terminal.TenantId, AccountTransactionTypeEnum.InvoicedToAccount, order.OrderID);
                        }
                        else if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns || item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WastedReturn)
                        {
                            //create an account transaction for returns order
                            UpdateAccountTransactionInfo(item, terminal.TenantId, AccountTransactionTypeEnum.Refund, order.OrderID);
                        }
                    }

                    if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales)
                    {
                        // create a paid transaction for the amount customer paid
                        var transaction = UpdateAccountTransactionInfo(item, terminal.TenantId, AccountTransactionTypeEnum.PaidByAccount, order.OrderID);

                        if (item.AccountTransactionFiles != null)
                        {
                            foreach (var transFile in item.AccountTransactionFiles)
                            {
                                transFile.AccountTransactionID = transaction.AccountTransactionId;
                                _accountServices.AddAccountTransactionFile(transFile, terminal.TenantId);
                            }
                        }
                    }
                }

                if (item.ProgressInfo != null)
                {
                    var progressInfo = _mapper.Map(item.ProgressInfo, new MarketRouteProgress());
                    progressInfo.Latitude = item.ProgressInfo.Latitude;
                    progressInfo.Longitude = item.ProgressInfo.Longitude;
                    progressInfo.OrderId = order.OrderID;
                    progressInfo.DateCreated = item.DateCreated == DateTime.MinValue ? DateTime.UtcNow : item.DateCreated;
                    progressInfo.DateUpdated = DateTime.UtcNow;
                    progressInfo.TenantId = terminal.TenantId;
                    if (progressInfo.RouteProgressId == Guid.Empty) { progressInfo.RouteProgressId = Guid.NewGuid(); }
                    _currentDbContext.MarketRouteProgresses.Add(progressInfo);
                    _currentDbContext.SaveChanges();
                }

                if (item.OrderProofOfDeliverySync != null)
                {
                    foreach (var orderprocessIds in order.OrderProcess)
                    {
                        OrderProofOfDelivery orderProofOfDelivery = new OrderProofOfDelivery();
                        orderProofOfDelivery.DateCreated = DateTime.UtcNow;
                        orderProofOfDelivery.DateUpdated = DateTime.UtcNow;
                        orderProofOfDelivery.SignatoryName = item.OrderProofOfDeliverySync?.SignatoryName ?? "";
                        orderProofOfDelivery.OrderProcessID = orderprocessIds.OrderProcessID;
                        orderProofOfDelivery.FileName = ByteToFile(item.OrderProofOfDeliverySync?.FileContent ?? null);
                        orderProofOfDelivery.TenantId = item.TenentId;
                        orderProofOfDelivery.NoOfCases = item.OrderProofOfDeliverySync?.NoOfCases ?? 0;
                        _currentDbContext.OrderProofOfDelivery.Add(orderProofOfDelivery);
                    }
                }

                _currentDbContext.SaveChanges();

                //update order status to complete if order status is awaiting authorisation.
                if (item.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales && item.OrderStatusID == OrderStatusEnum.Complete)
                {
                    order.OrderProcess = null;
                    _currentDbContext.Order.Attach(order);

                    // prices recalculations for direct sales orders if any changes after approval
                    order.OrderCost = (item.OrderProcessDetails.Sum(m => m.Price * m.QtyProcessed) + item.OrderProcessDetails.Sum(m => m.TaxAmount) + item.OrderProcessDetails.Sum(m => m.WarrantyAmount));
                    order.OrderDiscount = item.OrderProcessDiscount;
                    order.OrderTotal = (decimal)order.OrderCost - item.OrderProcessDiscount;

                    if (item.AccountTransactionInfo != null)
                    {
                        order.AmountPaidByAccount = item.AccountTransactionInfo.PaidAmount;
                        order.AccountBalanceOnPayment = item.AccountTransactionInfo.FinalAccountBalance;
                        order.AccountPaymentModeId = item.AccountTransactionInfo?.AccountPaymentModeId;
                        order.AccountBalanceBeforePayment = item.AccountTransactionInfo.OpeningAccountBalance;
                    }

                    order.OrderStatusID = OrderStatusEnum.Complete;
                    order.InvoiceNo = item.TerminalInvoiceNumber;
                    order.DateUpdated = DateTime.UtcNow;
                    _currentDbContext.SaveChanges();
                }

                var result = _mapper.Map(order, new OrdersSync());
                result.SerialProcessStatus = serialProcessStatus;
                result.RequestSuccess = true;
                return result;
            }

            return new OrdersSync() { RequestSuccess = true, RequestStatus = "Order processed successfully" };
        }

        private AccountTransaction UpdateAccountTransactionInfo(OrderProcessesSync item, int tenantId, AccountTransactionTypeEnum type, int orderId = 0)
        {
            TenantConfig tenantConfig = _tenantServices.GetTenantConfigById(tenantId);

            var transaction = new AccountTransaction();
            var account = new Account();

            if (item.AccountTransactionInfo.AccountId > 0)
            {
                transaction.AccountId = item.AccountTransactionInfo.AccountId;
            }
            else
            {
                transaction.AccountId = tenantConfig.DefaultCashAccountID;
            }

            if (orderId > 0)
            {
                transaction.OrderId = orderId;
            }

            if (type != AccountTransactionTypeEnum.InvoicedToAccount)
            {
                transaction.AccountPaymentModeId = item.AccountTransactionInfo.AccountPaymentModeId;
            }

            transaction.AccountTransactionTypeId = type;
            transaction.OpeningBalance = item.AccountTransactionInfo.OpeningAccountBalance;
            transaction.CreatedBy = item.CreatedBy;

            transaction.TenantId = (item.TenentId > 0) ? item.TenentId : tenantId;
            if (item.AccountID != null && item.AccountID > 0)
            {
                account = _accountServices.GetAccountsById(item.AccountID.Value);

                if (type == AccountTransactionTypeEnum.InvoicedToAccount)
                {
                    transaction.Amount = item.AccountTransactionInfo.OrderCost;
                    transaction.FinalBalance = (account.FinalBalance ?? 0) + item.AccountTransactionInfo.OrderCost;
                    account.FinalBalance = transaction.FinalBalance;
                    account.DateUpdated = DateTime.UtcNow;
                    transaction.Notes = "Invoice: " + item.TerminalInvoiceNumber;

                    _currentDbContext.Account.Attach(account);
                    var entry = _currentDbContext.Entry(account);
                    entry.Property(e => e.FinalBalance).IsModified = true;
                    entry.Property(e => e.DateUpdated).IsModified = true;
                    _currentDbContext.Entry(account).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                    _currentDbContext.Entry(account).State = EntityState.Detached;

                }
                else if (type == AccountTransactionTypeEnum.PaidByAccount || type == AccountTransactionTypeEnum.Refund ||
                    type == AccountTransactionTypeEnum.Discount || type == AccountTransactionTypeEnum.CreditNote)
                {
                    transaction.Amount = item.AccountTransactionInfo.PaidAmount;
                    transaction.FinalBalance = (account.FinalBalance ?? 0) - item.AccountTransactionInfo.PaidAmount;
                    transaction.Notes = "Payment: " + item.TerminalInvoiceNumber;
                    if (type == AccountTransactionTypeEnum.Refund)
                    {
                        transaction.Notes = "Return / Refund: " + item.TerminalInvoiceNumber;
                    }
                    account.FinalBalance = transaction.FinalBalance;
                    account.DateUpdated = DateTime.UtcNow;


                    _currentDbContext.Account.Attach(account);
                    var entry = _currentDbContext.Entry(account);
                    entry.Property(e => e.FinalBalance).IsModified = true;
                    entry.Property(e => e.DateUpdated).IsModified = true;
                    _currentDbContext.Entry(account).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                    _currentDbContext.Entry(account).State = EntityState.Detached;

                }

            }
            else
            {
                transaction.Amount = item.AccountTransactionInfo.PaidAmount;
                transaction.FinalBalance = item.AccountTransactionInfo.FinalAccountBalance;
            }

            transaction.DateCreated = item.DateCreated;
            //transaction.Notes = item.AccountTransactionInfo.Notes;
            transaction.OrderId = item.OrderID;
            transaction.OrderProcessId = item.OrderProcessID;
            _currentDbContext.AccountTransactions.Add(transaction);
            _currentDbContext.SaveChanges();

            return transaction;
        }

        public SerialProcessStatus UpdateSerialStockStatus(List<string> serialList, int product, string delivery, int order, int orderDetailId, int? location, InventoryTransactionTypeEnum type, int userId, int tenantId, int warehouseId, int? terminalId)
        {
            var serialProcessStatus = new SerialProcessStatus();

            if (serialList == null || serialList.Count < 1) return serialProcessStatus;

            var serials = _currentDbContext.ProductSerialization.Where(m => m.TenentId == tenantId).ToList();

            var orderTypeName = ((InventoryTransactionTypeEnum)type).ToString();
            InventoryTransactionTypeEnum[] outOfStockStatuses = { InventoryTransactionTypeEnum.SalesOrder, InventoryTransactionTypeEnum.TransferOut, InventoryTransactionTypeEnum.AdjustmentOut, InventoryTransactionTypeEnum.Loan,
                InventoryTransactionTypeEnum.WorksOrder, InventoryTransactionTypeEnum.Wastage };

            var newSerials = serialList.Where(m => !serials.Select(x => x.SerialNo.Trim()).Contains(m)).ToList();
            var serialsNotInstock = serials.Where(m => outOfStockStatuses.Contains(m.CurrentStatus)).Select(x => x.SerialNo).ToList();
            var serialsInstock = serials.Where(m => !outOfStockStatuses.Contains(m.CurrentStatus)).Select(x => x.SerialNo).ToList();

            if (type == InventoryTransactionTypeEnum.PurchaseOrder)
            {
                var existingSerials = serials.Where(m => serialList.Contains(m.SerialNo)).Select(x => x.SerialNo).ToList();
                if (existingSerials.Any())
                {
                    serialProcessStatus.RejectedSerials.AddRange(existingSerials);

                    foreach (var s in existingSerials)
                    {
                        serialList.RemoveAll(item => item == s);
                    }
                }
            }

            if (type == InventoryTransactionTypeEnum.TransferIn || type == InventoryTransactionTypeEnum.Wastage || type == InventoryTransactionTypeEnum.Returns)
            {
                if (newSerials.Count > 0)
                {
                    serialProcessStatus.RejectedSerials.AddRange(newSerials);

                    foreach (var s in newSerials)
                    {
                        serialList.RemoveAll(item => item == s);
                    }
                }
                var existingSerialsInStock = serialList.Where(m => serialsInstock.Contains(m)).ToList();
                if (existingSerialsInStock.Count > 0)
                {
                    serialProcessStatus.RejectedSerials.AddRange(existingSerialsInStock);

                    foreach (var s in existingSerialsInStock)
                    {
                        serialList.RemoveAll(item => item == s);
                    }
                }
            }

            if (type == InventoryTransactionTypeEnum.SalesOrder || type == InventoryTransactionTypeEnum.Loan || type == InventoryTransactionTypeEnum.TransferOut)
            {
                if (newSerials.Count > 0)
                {
                    serialProcessStatus.RejectedSerials.AddRange(newSerials);

                    foreach (var s in newSerials)
                    {
                        serialList.RemoveAll(item => item == s);
                    }

                }
                var existingSerialsNotInStock = serialList.Where(m => serialsNotInstock.Contains(m)).ToList();
                if (existingSerialsNotInStock.Count > 0)
                {
                    serialProcessStatus.RejectedSerials.AddRange(existingSerialsNotInStock);

                    foreach (var s in existingSerialsNotInStock)
                    {
                        serialList.RemoveAll(item => item == s);
                    }

                }
            }

            var oprocess = GetOrderProcessByDeliveryNumber(order, type, delivery, userId, null, warehouseId);

            if (oprocess == null)
            {
                var opr = new OrderProcess
                {
                    DeliveryNO = delivery,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = userId,
                    OrderID = order,
                    TenentId = tenantId,
                    InventoryTransactionTypeId = type,
                    WarehouseId = warehouseId
                };

                var odet = new OrderProcessDetail
                {
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    OrderProcessId = opr.OrderProcessID,
                    ProductId = product,
                    TenentId = tenantId,
                    QtyProcessed = serialList.Count,
                    OrderDetailID = orderDetailId,
                    TerminalId = terminalId
                };
                opr.OrderProcessDetail.Add(odet);
                _currentDbContext.OrderProcess.Add(opr);
            }
            else
            {
                var odet = new OrderProcessDetail
                {
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    OrderProcessId = oprocess.OrderProcessID,
                    ProductId = product,
                    TenentId = tenantId,
                    QtyProcessed = serialList.Count,
                    OrderDetailID = orderDetailId,
                    TerminalId = terminalId

                };
                _currentDbContext.OrderProcessDetail.Add(odet);
            }

            var orderDetail = _currentDbContext.OrderDetail.FirstOrDefault(m => (orderDetailId > 0 && m.OrderDetailID == orderDetailId) || (m.OrderID == order && m.ProductId == product));

            TenantWarranty warrantyInfo = null;


            if (orderDetail != null)
            {
                warrantyInfo = orderDetail.Warranty;
            }

            foreach (var item in serialList)
            {
                var serial = _currentDbContext.ProductSerialization.FirstOrDefault(a => a.SerialNo == item && a.CurrentStatus != type);

                if (serial != null)
                {
                    serial.CurrentStatus = type;
                    serial.DateUpdated = DateTime.UtcNow;
                    serial.UpdatedBy = userId;
                    serial.TenentId = tenantId;
                    serial.LocationId = location;
                    _currentDbContext.Entry(serial).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    serial = new ProductSerialis
                    {
                        CreatedBy = userId,
                        DateCreated = DateTime.UtcNow,
                        SerialNo = item,
                        TenentId = tenantId,
                        ProductId = product,
                        LocationId = location,
                        CurrentStatus = (InventoryTransactionTypeEnum)type
                    };
                }

                if (warrantyInfo != null)
                {
                    serial.SoldWarrantyStartDate = DateTime.UtcNow;
                    serial.SoldWarrentyEndDate = DateTime.UtcNow.AddDays(warrantyInfo.WarrantyDays);
                    serial.PostageTypeId = warrantyInfo.OrderConsignmentTypes.ConsignmentTypeId;
                    serial.SoldWarrantyIsPercent = warrantyInfo.IsPercent;
                    serial.SoldWarrantyName = warrantyInfo.WarrantyName;
                    serial.SoldWarrantyPercentage = warrantyInfo.PercentageOfPrice;
                    serial.SoldWarrantyFixedPrice = warrantyInfo.FixedPrice;
                    serial.PostageTypeId = warrantyInfo.PostageTypeId;
                }

                if (_currentDbContext.Entry(serial).State == EntityState.Detached)
                {
                    _currentDbContext.Entry(serial).State = EntityState.Added;
                }
                else
                {
                    _currentDbContext.Entry(serial).State = EntityState.Modified;
                }

                if (serial.LocationId == 0)
                {
                    serial.LocationId = (int?)null;

                }

                try
                {
                    _currentDbContext.SaveChanges();
                    serialProcessStatus.ProcessedSerials.Add(serial.SerialNo);

                }
                catch (Exception)
                {
                    serialProcessStatus.RejectedSerials.Add(serial.SerialNo);
                }
            }

            InventoryTransactionTypeEnum transactionTypeId = type;
            int? locationId = location == 0 ? (int?)null : location;
            _currentDbContext.SaveChanges();
            Inventory.StockTransactionApi(serialList.ToList(), order, product, transactionTypeId, locationId,
            tenantId, warehouseId, userId, oprocess.OrderProcessID, OrderProcessDetialId: (oprocess?.OrderProcessDetail?.FirstOrDefault(u => u.ProductId == product)?.OrderProcessDetailID));
            return serialProcessStatus;
        }


        public OrderProcess GetOrderProcessByOrderProcessId(int orderProcessid)
        {
            return _currentDbContext.OrderProcess.FirstOrDefault(u => u.OrderProcessID == orderProcessid && u.IsDeleted != true);
        }

        public OrderProcessDetail GetOrderProcessDetailById(int orderProcessDetailId)
        {
            return _currentDbContext.OrderProcessDetail.First(x => x.OrderProcessDetailID == orderProcessDetailId && x.IsDeleted != true);
        }

        public List<OrderProcessDetail> GetOrderProcessDetailForOrders(int orderprocessId, List<int> orderIds = null)
        {
            if (orderprocessId > 0)
            {
                return _currentDbContext.OrderProcessDetail.AsNoTracking().Where(x => x.OrderProcessId == orderprocessId && x.IsDeleted != true).ToList();
            }
            return _currentDbContext.OrderProcessDetail.AsNoTracking().Where(x => orderIds.Contains(x.OrderDetail.OrderID) && x.IsDeleted != true).ToList();
        }

        public PalletOrderProductsCollection GetAllProductsByOrdersInPallet(int palletId)
        {
            var result = new PalletOrderProductsCollection();
            var pallet = _currentDbContext.Pallets.Find(palletId);
            if (pallet != null)
            {
                result.AccountName = pallet.RecipientAccount.CompanyName;
                result.AccountID = pallet.RecipientAccountID;
                result.PalletNumber = pallet.PalletNumber;
                result.PalletID = pallet.PalletID;
                result.CreatedBy = _userService.GetAuthUserById(pallet.CreatedBy).DisplayName;
                result.DateCreated = pallet.DateCreated.ToString("dd/MM/yyyy HH:mm");
                result.DispatchedBy = _userService.GetAuthUserById(pallet.CompletedBy ?? 0)?.DisplayName;
                result.ProductItems = pallet.PalletProducts.Select(m => new PalletOrderProductItem()
                {
                    ProductNameWithCode = m.Product.NameWithCode,
                    PalletQuantity = m.Quantity
                }).ToList();
            }

            return result;
        }

        public List<OrderProcessViewModel> GetOrderProcessByWarehouseId(int warehouseId)
        {
            return _currentDbContext.OrderProcess.Where(x => x.WarehouseId == warehouseId && x.IsDeleted != true)
                .Select(x => new OrderProcessViewModel()
                {
                    OrderProcessID = x.OrderProcessID,
                    DeliveryNO = x.DeliveryNO,
                    DateCreated = x.DateCreated,
                    PONumber = _currentDbContext.Order.Where(p => p.OrderID == x.Order.OrderID).Select(p => p.OrderNumber).FirstOrDefault(),
                    Supplier = _currentDbContext.Account.Where(s => s.AccountID == x.Order.AccountID).Select(s => s.CompanyName).FirstOrDefault()
                }).ToList();
        }

        private bool ProductStockTransactionRecord(int productId, InventoryTransactionTypeEnum transType, decimal quantity, int orderId, int tenantId, int userId, int warehouseId, int? OrderprocessId = null, int? OrderProcessDetailId = null)
        {

            var status = false;

            if (quantity < 0) { return false; }

            status = Inventory.StockTransaction(productId, transType, quantity, (orderId > 0 ? orderId : (int?)null), null, string.Empty, null, orderprocessId: (OrderprocessId ?? null), orderProcessDetailId: (OrderProcessDetailId ?? null));

            return status;
        }

        public List<ProductSerialis> GetProductSerialsByNumber(string serialNo, int tenantId)
        {
            return _currentDbContext.ProductSerialization.Where(a => a.SerialNo == serialNo && a.TenentId == tenantId)
                .ToList();
        }

        public InventoryTransaction GetLastInventoryTransactionsForSerial(string serial, int tenantId)
        {
            return _currentDbContext
                .InventoryTransactions
                .Where(a => a.ProductSerial.SerialNo ==
                            serial && a.TenentId == tenantId)?.OrderByDescending(x => x.DateCreated)?.FirstOrDefault();
        }

        public IQueryable<Order> GetAllPendingOrdersForProcessingForDate()
        {
            var result = _currentDbContext.Order.AsNoTracking();
            return result;
        }

        public OrderNotes DeleteOrderNoteById(int orderNoteId, int userId)
        {
            var cItem = _currentDbContext.OrderNotes.Find(orderNoteId);
            if (cItem != null)
            {
                cItem.IsDeleted = true;
                cItem.UpdatedBy = userId;
                cItem.DateUpdated = DateTime.UtcNow;
                _currentDbContext.SaveChanges();
            }
            return cItem;
        }
        public Order DeleteOrderById(int orderId, int userId)
        {
            var cItem = _currentDbContext.Order.Find(orderId);
            if (cItem != null)
            {
                cItem.IsDeleted = true;
                cItem.UpdatedBy = userId;
                cItem.OrderStatusID = OrderStatusEnum.Cancelled;
                cItem.DateUpdated = DateTime.UtcNow;
                int res = _currentDbContext.SaveChanges();
                if (res == 1)
                {
                    var appointments = _currentDbContext.Appointments.Where(x => x.OrderId == orderId && x.EndTime > DateTime.UtcNow && x.IsCanceled != true).ToList();

                    foreach (var item in appointments)
                    {
                        item.IsCanceled = true;
                        _currentDbContext.SaveChanges();
                    }
                }

                Inventory.StockRecalculateByOrderId(orderId, caCurrent.CurrentWarehouse().WarehouseId, caCurrent.CurrentTenant().TenantId, caCurrent.CurrentUser().UserId, true);

            }
            return cItem;
        }

        public OrderNotes UpdateOrderNote(int noteId, string notes, int userId, int? orderId = null)
        {
            var cItem = _currentDbContext.OrderNotes.Find(noteId);

            if (cItem == null && orderId == null) return null;

            if (cItem == null && orderId.HasValue)
            {
                cItem = new OrderNotes()
                {
                    OrderID = orderId.Value,
                    Notes = notes,
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow
                };
                _currentDbContext.Entry(cItem).State = EntityState.Added;
            }
            else
            {
                _currentDbContext.Entry(cItem).State = EntityState.Modified;
            }
            cItem.UpdatedBy = userId;
            cItem.DateUpdated = DateTime.UtcNow;
            cItem.Notes = notes;
            _currentDbContext.SaveChanges();

            var order = _currentDbContext.Order.Find(cItem.OrderID);
            if (order != null)
            {
                var activeAppointment = order.Appointmentses?.FirstOrDefault(m => !m.IsCanceled);
                if (activeAppointment != null && order.OrderNotes.Any())
                {
                    var noteString =
                        string.Join("\n", order.OrderNotes.Select(x => x.Notes + "(" + (x.DateUpdated ?? x.DateCreated).ToString("dd/MM/yyyy HH:mm") + "-" + _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserId == x.CreatedBy)?.DisplayName + ")"));
                    activeAppointment.Description = noteString;
                    _currentDbContext.Entry(activeAppointment).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }
            }

            return cItem;
        }
        public Order CreateDirectSalesOrder(DirectSalesViewModel model, int tenantId, int userId, int warehouseId)
        {
            Account account;
            if (model.AccountId > 0)
            {
                account = _currentDbContext.Account.First(m => m.AccountID == model.AccountId);
            }
            else
            {
                var tenantConfig = _currentDbContext.TenantConfigs.First(m => m.TenantId == tenantId);
                account = tenantConfig.DefaultCashAccount;
            }

            if (account == null) throw new Exception("Please setup Default cash account for the tenant for direct sales.");

            model.AccountId = account.AccountID;

            var order = new Order()
            {
                OrderNumber = GenerateNextOrderNumber(InventoryTransactionTypeEnum.DirectSales, tenantId),
                TenentId = tenantId,
                DateCreated = DateTime.UtcNow,
                InventoryTransactionTypeId = InventoryTransactionTypeEnum.DirectSales,
                CreatedBy = userId,
                OrderStatusID = OrderStatusEnum.Complete,
                Note = model.InvoiceAddress,
                WarehouseId = warehouseId,
                AccountID = model.AccountId,
                AccountCurrencyID = account != null ? account.CurrencyID : 1,
                DateUpdated = DateTime.UtcNow,
                OrderCost = model.NetAmount,

            };


            order.OrderDiscount = GetDiscountOnTotalCost(order.AccountID ?? 0, model.NetAmount);
            order.OrderTotal = model.NetAmount - order.OrderDiscount;




            order.InvoiceNo = GaneStaticAppExtensions.GenerateDateRandomNo();
            _currentDbContext.Order.Add(order);
            _currentDbContext.SaveChanges();


            var process = new OrderProcess()
            {
                OrderProcessStatusId = OrderProcessStatusEnum.Complete,
                DateUpdated = DateTime.UtcNow,
                UpdatedBy = userId,
                TenentId = tenantId,
                OrderID = order.OrderID,
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                WarehouseId = warehouseId,
                InventoryTransactionTypeId = InventoryTransactionTypeEnum.DirectSales
            };
            order.OrderProcess.Add(process);

            foreach (var m in model.AllInvoiceProducts)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = m.ProductId,
                    Qty = m.QtyProcessed,
                    Price = m.Price,
                    TenentId = tenantId,
                    TotalAmount = m.TotalAmount + (m.WarrantyAmount) + (m.TaxAmounts),
                    TaxAmount = m.TaxAmounts,

                    TaxID = m.TaxId > 0 ? m.TaxId : null,
                    WarrantyAmount = m.WarrantyAmount,
                    WarrantyID = m.WarrantyId,
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    UpdatedBy = userId,
                    WarehouseId = warehouseId
                };
                order.OrderDetails.Add(orderDetail);

                var orderProcessDetail = new OrderProcessDetail
                {
                    ProductId = m.ProductId,
                    QtyProcessed = m.QtyProcessed,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    TenentId = tenantId,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    IsDeleted = false
                };
                process.OrderProcessDetail.Add(orderProcessDetail);
                _currentDbContext.Entry(order).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                ProductStockTransactionRecord(m.ProductId, order.InventoryTransactionTypeId, m.QtyProcessed, order.OrderID, tenantId, userId, order.WarehouseId ?? 0, process.OrderProcessID, orderProcessDetail.OrderProcessDetailID);

            }

            _invoiceService.AddAccountTransaction(AccountTransactionTypeEnum.InvoicedToAccount, model.InvoiceTotal, ("Invoiced for Direct Sales Order #" + order.OrderNumber).Trim(), account.AccountID, tenantId, userId, AccountPaymentModeEnum.Cash);

            if (model.PaymentToday > 0)
            {
                _invoiceService.AddAccountTransaction(AccountTransactionTypeEnum.PaidByAccount, model.PaymentToday, ("Paid For Direct Sales order #" + order.OrderNumber).Trim(), account.AccountID, tenantId, userId, model.PaymentModeId);
            }

            return order;
        }
        public Order SaveDirectSalesOrder(Order order, int tenantId, int warehouseId,
          int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            decimal total = 0;
            var obj = _currentDbContext.Order.Find(order.OrderID);
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
                    item.Qty = item.Qty;
                    item.CreatedBy = userId;
                    item.TenentId = tenantId;
                    item.OrderID = order.OrderID;
                    total = total + item.TotalAmount;
                    item.TenentId = tenantId;
                    item.WarehouseId = obj.WarehouseId ?? warehouseId;
                    item.IsDeleted = null;

                    if (item.WarrantyID > 0)
                    {
                        bool warrantyTypePercent = false;
                        var warranty = _currentDbContext.TenantWarranty.AsNoTracking().FirstOrDefault(x => x.WarrantyID == item.WarrantyID);
                        if (warranty != null)
                        {
                            warrantyTypePercent = warranty.IsPercent;
                        }
                        if (warrantyTypePercent)
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
                    orderDetail.WarrantyAmount = orderDetail.WarrantyAmount > 0 ? orderDetail.WarrantyAmount : item.WarrantyAmount;

                    _currentDbContext.Entry(orderDetail).State = EntityState.Modified;

                    total = total + item.TotalAmount + (orderDetail.WarrantyAmount > 0 ? orderDetail.WarrantyAmount : item.WarrantyAmount);
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


            obj.OrderCost = total;
            obj.OrderDiscount = GetDiscountOnTotalCost(order.AccountID ?? 0, total);
            obj.OrderTotal = (decimal)total - obj.OrderDiscount;
            obj.UpdatedBy = userId;
            obj.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(obj).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            Inventory.StockRecalculateByOrderId(order.OrderID, obj.WarehouseId ?? warehouseId, tenantId, caCurrent.CurrentUser().UserId);
            return obj;

        }

        public DirectSalesViewModel GetDirectSalesModelByOrderId(int orderId)
        {
            var model = new DirectSalesViewModel();
            var order = GetOrderById(orderId);
            model.OrderNumber = order.OrderNumber;
            model.DirectSalesOrderId = order.OrderID;
            model.InvoiceAddress = order.Note;
            model.InvoiceCurrency = order.AccountCurrency.Symbol;
            model.NetAmount = order.OrderTotal;

            var paymentModes = from AccountPaymentModeEnum d in Enum.GetValues(typeof(AccountPaymentModeEnum))
                               select new { ID = (int)d, Name = d.ToString() };
            model.AllPaymentModes = new SelectList(paymentModes, "Value", "Text", paymentModes.FirstOrDefault()).ToList();

            model.AllInvoiceProducts = order.OrderDetails.Select(m => new DirectSaleProductsViewModel()
            {
                QtyProcessed = m.Qty,
                ProductId = m.ProductId,
                WarrantyAmount = m.WarrantyAmount,
                WarrantyId = m.TaxID,
                TaxId = m.TaxID,
                TaxPercent = m.ProductMaster.GlobalTax != null ? m.ProductMaster.GlobalTax.PercentageOfAmount : 0
            }).ToList();
            return model;
        }
        public Order OrderProcessAutoComplete(int orderId, string deliveryNumber, int userId, bool includeProcessing, bool forceComplete)
        {
            var order = GetOrderById(orderId);
            var directshipment = order.DirectShip;
            if (includeProcessing)
            {
                var results = new List<bool>();

                var invalidForDirectProcessing = false;

                var orderDetailsToProcess = order.OrderDetails.Where(x => x.IsDeleted != true).Where(m => m.ProcessedQty < m.Qty).ToList();

                foreach (var orderDetail in orderDetailsToProcess)
                {
                    invalidForDirectProcessing = orderDetail.ProductMaster.Serialisable || orderDetail.ProductMaster.ProductLocationsMap.Count() > 1;
                    if (!invalidForDirectProcessing || directshipment == true)
                    {
                        var qtyDifference = orderDetail.Qty - orderDetail.ProcessedQty;
                        var model = new InventoryTransaction() { OrderID = orderId, ProductId = orderDetail.ProductId, Quantity = qtyDifference };
                        Inventory.StockTransaction(model, order.InventoryTransactionTypeId, null, deliveryNumber ?? "", orderDetail.OrderDetailID, null);
                        results.Add(true);
                    }
                    else
                    {
                        results.Add(false);
                    }
                }

                if (results.Any(m => m == false))
                {
                    throw new Exception("This order is not fully complete. Make sure there is no serialised products or products with multiple locations in the order.");
                }
            }

            if (forceComplete)
            {
                order = GetOrderById(orderId);
                var OrderProcess = _currentDbContext.OrderProcess.Where(u => u.OrderID == orderId && u.IsDeleted != true).ToList();
                OrderProcess.ForEach(u => u.OrderProcessStatusId = OrderProcessStatusEnum.Complete);
                order.OrderStatusID = OrderStatusEnum.Complete;

                order.UpdatedBy = userId;
                order.DateUpdated = DateTime.UtcNow;

                if (order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder && (!order.OrderCost.HasValue || order.OrderCost == 0))
                {
                    order.OrderCost = 0;
                    var allOrderAppointments = order.Appointmentses.Where(m => m.IsCanceled != true).ToList();
                    foreach (var newAppt in allOrderAppointments)
                    {
                        var totalMinutesToWork = (int)newAppt.EndTime.Subtract(newAppt.StartTime).TotalMinutes;
                        var resource = _currentDbContext.Resources.FirstOrDefault(m => m.ResourceId == newAppt.ResourceId);
                        if (resource != null)
                        {
                            var ratePerMinute = Math.Round(((resource.HourlyRate ?? 0) / 60), 2);
                            order.OrderCost += (decimal)totalMinutesToWork * ratePerMinute;
                        }
                    }
                }

                _currentDbContext.Entry(order).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                if (caCurrent.CurrentWarehouse() != null && caCurrent.CurrentUser() != null)
                {
                    Inventory.StockRecalculateByOrderId(order.OrderID, caCurrent.CurrentWarehouse().WarehouseId, order.TenentId, caCurrent.CurrentUser().UserId);
                }
            }

            return order;

        }
        public List<AwaitingAuthorisationOrdersViewModel> GetAllOrdersAwaitingAuthorisation(int tenantId, int warehouseId, OrderStatusEnum? OrderStatusId = null)
        {

            IQueryable<TenantLocations> childWarehouseIds = _currentDbContext.TenantWarehouses.Where(x => x.ParentWarehouseId == warehouseId);

            return _currentDbContext.Order.Where(o => o.TenentId == tenantId &&
            (o.WarehouseId == warehouseId || childWarehouseIds.Any(x => x.ParentWarehouseId == warehouseId))
            && (OrderStatusId == 0 ? o.OrderStatusID == OrderStatusEnum.AwaitingAuthorisation || o.OrderStatusID == OrderStatusEnum.Approved : o.OrderStatusID == OrderStatusId) && o.IsDeleted != true)
                .OrderByDescending(x => x.DateCreated)
                .Select(p => new AwaitingAuthorisationOrdersViewModel()
                {
                    OrderID = p.OrderID,
                    OrderNumber = p.OrderNumber,
                    IssueDate = p.IssueDate,
                    DateUpdated = p.DateUpdated,
                    DateCreated = p.DateCreated,
                    POStatus = p.OrderStatusID.ToString(),
                    Account = p.Account.AccountCode,
                    InvoiceNo = p.InvoiceNo,
                    InvoiceDetails = p.InvoiceDetails,
                    OrderCost = p.OrderCost,
                    OrderTypeId = p.InventoryTransactionTypeId,
                    OrderType = p.InventoryTransactionTypeId,
                    AccountName = p.Account.CompanyName,
                    Currecny = p.AccountCurrency.CurrencyName,
                    OrderTotal = p.OrderTotal,
                    WarehouseName = p.Warehouse.WarehouseName,
                    OrderStatusID = p.OrderStatusID,
                    OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                    {
                        OrderNoteId = s.OrderNoteId,
                        Notes = s.Notes,
                        NotesByName = p.Tenant.AuthUsers.FirstOrDefault(x => x.UserId == s.CreatedBy).UserName,
                        NotesDate = s.DateCreated
                    }).ToList()
                }).ToList();
        }


        public IQueryable<OrderReceiveCount> GetAllOrderReceiveCounts(int tenantId, int warehouseId, DateTime? dateUpdated)
        {
            var res = _currentDbContext.OrderReceiveCount.Where(x => x.TenantId == tenantId && x.WarehouseId == warehouseId && (!dateUpdated.HasValue || (x.DateUpdated ?? x.DateCreated) >= dateUpdated));
            return res;
        }

        public OrderReceiveCountSync SaveOrderReceiveCount(OrderReceiveCountSync countRecord, Terminals terminal)
        {
            var newCountRecord = new OrderReceiveCount();
            _mapper.Map(countRecord, newCountRecord);

            if (newCountRecord.TenantId < 1)
            {
                newCountRecord.TenantId = terminal.TenantId;
            }

            if (newCountRecord.DateCreated == null || newCountRecord.DateCreated == DateTime.MinValue)
            {
                newCountRecord.DateCreated = DateTime.UtcNow;
            }

            _currentDbContext.Entry(newCountRecord).State = EntityState.Added;
            _currentDbContext.SaveChanges();

            _mapper.Map(newCountRecord, countRecord);
            return countRecord;
        }
        public InventoryTransaction AddGoodsReturnPallet(List<string> serials, string orderNumber, int prodId, InventoryTransactionTypeEnum transactionTypeId, decimal quantity, int? OrderId, int tenantId, int currentWarehouseId, int UserId, int palletTrackigId = 0)
        {

            var products = _currentDbContext.ProductMaster.Find(prodId);
            decimal totalqunatity = 0;
            if ((!OrderId.HasValue && transactionTypeId == InventoryTransactionTypeEnum.Wastage) || transactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn || transactionTypeId == InventoryTransactionTypeEnum.AdjustmentOut)
            {
                if (serials == null && palletTrackigId > 0)
                {
                    var newpallet = _currentDbContext.PalletTracking.Find(palletTrackigId);
                    var product = _currentDbContext.ProductMaster.Find(newpallet.ProductId);

                    if (transactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn)
                    {
                        if (newpallet.Status == PalletTrackingStatusEnum.Created)
                        {
                            newpallet.Status = PalletTrackingStatusEnum.Active;
                        }
                        newpallet.DateUpdated = DateTime.UtcNow;
                        _currentDbContext.PalletTracking.Add(newpallet);
                        _currentDbContext.Entry(newpallet).State = EntityState.Modified;
                        _currentDbContext.SaveChanges();
                        Inventory.StockTransactionApi(newpallet.ProductId, transactionTypeId, (newpallet.RemainingCases * (product.ProductsPerCase ?? 1)), OrderId, caCurrent.CurrentTenant().TenantId, currentWarehouseId, caCurrent.CurrentUser().UserId, null, palletTrackigId);


                    }
                    return _currentDbContext.InventoryTransactions.FirstOrDefault(u => u.OrderID == 0);
                }
                foreach (var pallet in serials)
                {
                    decimal qty = 0;
                    var palletdData = pallet.Split(new string[] { "#+#", "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (palletdData.Length >= 2)
                    {
                        int PalletTrackingId = 0;

                        if (!string.IsNullOrEmpty(palletdData[0]))
                        {
                            PalletTrackingId = int.Parse(palletdData[0]);
                        }
                        if (!string.IsNullOrEmpty(palletdData[1]))
                        {
                            qty = decimal.Parse(palletdData[1]);
                        }
                        var newpallet = _currentDbContext.PalletTracking.Find(PalletTrackingId);

                        if (transactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn)
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
                        Inventory.StockTransactionApi(prodId, transactionTypeId, (qty * (products.ProductsPerCase ?? 1)), OrderId, caCurrent.CurrentTenant().TenantId, currentWarehouseId, caCurrent.CurrentUser().UserId, null, PalletTrackingId);

                    }
                }

                return _currentDbContext.InventoryTransactions.FirstOrDefault(u => u.OrderID == 0);
            }

            else
            {
                int? lineId = 0;
                if (OrderId.HasValue)
                {
                    var cOrder = _currentDbContext.Order.Find(OrderId);
                    foreach (var pallet in serials)
                    {
                        decimal qty = 0;
                        var palletdData = pallet.Split(new string[] { "#+#", "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                        if (palletdData.Length >= 2)
                        {
                            int PalletTrackingId = 0;

                            if (!string.IsNullOrEmpty(palletdData[0]))
                            {
                                PalletTrackingId = int.Parse(palletdData[0]);
                            }
                            if (!string.IsNullOrEmpty(palletdData[1]))
                            {
                                qty = decimal.Parse(palletdData[1]);
                            }
                            var newpallet = _currentDbContext.PalletTracking.Find(PalletTrackingId);
                            newpallet.Status = PalletTrackingStatusEnum.Active;
                            if (transactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn || transactionTypeId == InventoryTransactionTypeEnum.AdjustmentOut)
                            {
                                if (newpallet.TotalCases < qty)
                                {
                                    newpallet.TotalCases = newpallet.TotalCases + qty;
                                }
                            }

                            if (transactionTypeId == InventoryTransactionTypeEnum.Returns)
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases + qty;
                            }
                            else
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases - qty;
                            }

                            totalqunatity += qty;

                            newpallet.DateUpdated = DateTime.UtcNow;
                            _currentDbContext.PalletTracking.Add(newpallet);
                            _currentDbContext.Entry(newpallet).State = EntityState.Modified;
                            _currentDbContext.SaveChanges();
                            Inventory.StockTransactionApi(prodId, transactionTypeId, (qty * (products.ProductsPerCase ?? 1)), OrderId, caCurrent.CurrentTenant().TenantId, currentWarehouseId, caCurrent.CurrentUser().UserId, null, PalletTrackingId);

                        }
                    }

                    if (products != null)
                    {
                        quantity = totalqunatity * products.ProductsPerCase ?? 1;
                    }
                    else { quantity = totalqunatity; }


                    if (lineId == null || lineId < 1)
                    {
                        lineId = cOrder.OrderDetails.FirstOrDefault()?.OrderDetailID;
                    }

                    var xopr = new OrderProcess
                    {
                        DeliveryNO = "",
                        DateCreated = DateTime.UtcNow,
                        InventoryTransactionTypeId = transactionTypeId,
                        CreatedBy = UserId,
                        OrderID = OrderId,
                        TenentId = tenantId,
                        WarehouseId = currentWarehouseId
                    };

                    var orderProcessDetail = new OrderProcessDetail
                    {
                        CreatedBy = UserId,
                        DateCreated = DateTime.UtcNow,
                        OrderProcessId = xopr.OrderProcessID,
                        ProductId = prodId,
                        TenentId = tenantId,
                        QtyProcessed = quantity,
                        OrderDetailID = lineId
                    };
                    xopr.OrderProcessDetail.Add(orderProcessDetail);
                    _currentDbContext.OrderProcess.Add(xopr);
                    _currentDbContext.SaveChanges();

                    // if DontMonitorStock flag is true then make that flag true in inventory as well

                    return _currentDbContext.InventoryTransactions.Where(u => u.OrderID == OrderId).OrderByDescending(u => u.DateCreated).FirstOrDefault();
                }
                else
                {
                    Order order = new Order();

                    order.OrderNumber = orderNumber.Trim();
                    var duplicateOrder = _currentDbContext.Order.FirstOrDefault(m => m.OrderNumber.Equals(order.OrderNumber, StringComparison.CurrentCultureIgnoreCase));
                    if (duplicateOrder != null)
                    {
                        throw new Exception($"Order Number {order.OrderNumber} already associated with another Order. Please regenerate order number.", new Exception("Duplicate Order Number"));
                    }
                    order.InventoryTransactionTypeId = transactionTypeId;
                    order.IssueDate = DateTime.UtcNow;
                    order.ExpectedDate = DateTime.UtcNow;
                    order.OrderStatusID = OrderStatusEnum.Active;
                    order.DateCreated = DateTime.UtcNow;
                    order.DateUpdated = DateTime.UtcNow;
                    order.TenentId = tenantId;
                    order.CreatedBy = UserId;
                    order.UpdatedBy = UserId;
                    order.WarehouseId = currentWarehouseId;

                    _currentDbContext.Order.Add(order);
                    _currentDbContext.SaveChanges();
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.DateCreated = DateTime.UtcNow;
                    orderDetail.CreatedBy = UserId;
                    orderDetail.OrderID = order.OrderID;
                    orderDetail.TenentId = tenantId;
                    orderDetail.ProductId = prodId;
                    //orderDetail.SortOrder = item.ProductMaster?.ProductGroup?.SortOrder ?? 0;

                    orderDetail.TaxName = null;
                    orderDetail.Warranty = null;
                    orderDetail.WarehouseId = currentWarehouseId;
                    _currentDbContext.OrderDetail.Add(orderDetail);
                    OrderNotes orderNotes = new OrderNotes();
                    orderNotes.DateCreated = DateTime.UtcNow;
                    orderNotes.CreatedBy = UserId;

                    orderNotes.Notes = "return";
                    orderNotes.OrderID = order.OrderID;
                    orderNotes.TenantId = tenantId;
                    _currentDbContext.OrderNotes.Add(orderNotes);
                    _currentDbContext.SaveChanges();



                    var cOrder = _currentDbContext.Order.Find(order.OrderID);
                    foreach (var pallet in serials)
                    {
                        decimal qty = 0;
                        var palletdData = pallet.Split(new string[] { "#+#", "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                        if (palletdData.Length >= 2)
                        {
                            int PalletTrackingId = 0;

                            if (!string.IsNullOrEmpty(palletdData[0]))
                            {
                                PalletTrackingId = int.Parse(palletdData[0]);
                            }
                            if (!string.IsNullOrEmpty(palletdData[1]))
                            {
                                qty = decimal.Parse(palletdData[1]);
                            }
                            var newpallet = _currentDbContext.PalletTracking.Find(PalletTrackingId);
                            newpallet.Status = PalletTrackingStatusEnum.Active;

                            if (transactionTypeId == InventoryTransactionTypeEnum.Returns)
                            {
                                newpallet.RemainingCases = newpallet.RemainingCases + qty;
                            }



                            totalqunatity += qty;

                            newpallet.DateUpdated = DateTime.UtcNow;
                            _currentDbContext.PalletTracking.Add(newpallet);
                            _currentDbContext.Entry(newpallet).State = EntityState.Modified;
                            _currentDbContext.SaveChanges();
                            Inventory.StockTransactionApi(prodId, transactionTypeId, (qty * (products.ProductsPerCase ?? 1)), order.OrderID, caCurrent.CurrentTenant().TenantId, currentWarehouseId, caCurrent.CurrentUser().UserId, null, PalletTrackingId);

                        }
                    }
                    if (products != null)
                    {
                        quantity = totalqunatity * (products.ProductsPerCase ?? 1);
                    }
                    else { quantity = totalqunatity; }

                    if (lineId == null || lineId < 1)
                    {
                        lineId = cOrder.OrderDetails.FirstOrDefault()?.OrderDetailID;
                    }

                    var xopr = new OrderProcess
                    {
                        DeliveryNO = "",
                        DateCreated = DateTime.UtcNow,
                        InventoryTransactionTypeId = transactionTypeId,
                        CreatedBy = UserId,
                        OrderID = order.OrderID,
                        TenentId = tenantId,
                        WarehouseId = currentWarehouseId
                    };

                    xopr.OrderProcessDetail.Add(new OrderProcessDetail
                    {
                        CreatedBy = UserId,
                        DateCreated = DateTime.UtcNow,
                        OrderProcessId = xopr.OrderProcessID,
                        ProductId = prodId,
                        TenentId = tenantId,
                        QtyProcessed = quantity,
                        OrderDetailID = lineId
                    });

                    _currentDbContext.OrderProcess.Add(xopr);
                    _currentDbContext.SaveChanges();
                    OrderId = order.OrderID;

                    return _currentDbContext.InventoryTransactions.Where(u => u.OrderID == OrderId).OrderByDescending(u => u.DateCreated).FirstOrDefault();

                }
            }
        }

        public List<OrderProcess> GetALLOrderProcessByOrderProcessId(int orderProcessid)
        {
            return _currentDbContext.OrderProcess.Where(u => u.OrderProcessID == orderProcessid).ToList();
        }

        public bool UpdateDeliveryAddress(AccountShipmentInfo accountShipmentInfo)
        {
            var orderprocess = _currentDbContext.OrderProcess.FirstOrDefault(u => u.OrderProcessID == accountShipmentInfo.OrderProcessId);
            if (orderprocess != null)
            {
                orderprocess.ShipmentAddressLine1 = string.IsNullOrEmpty(accountShipmentInfo.ShipmentAddressLine1) ? orderprocess.ShipmentAddressLine1 : accountShipmentInfo.ShipmentAddressLine1;
                orderprocess.ShipmentAddressLine2 = string.IsNullOrEmpty(accountShipmentInfo.ShipmentAddressLine2) ? orderprocess.ShipmentAddressLine2 : accountShipmentInfo.ShipmentAddressLine2;
                orderprocess.ShipmentAddressLine3 = string.IsNullOrEmpty(accountShipmentInfo.ShipmentAddressLine3) ? orderprocess.ShipmentAddressLine3 : accountShipmentInfo.ShipmentAddressLine3;
                orderprocess.ShipmentAddressLine4 = string.IsNullOrEmpty(accountShipmentInfo.ShipmentAddressLine4) ? orderprocess.ShipmentAddressLine4 : accountShipmentInfo.ShipmentAddressLine4;
                orderprocess.ShipmentAddressPostcode = string.IsNullOrEmpty(accountShipmentInfo.ShipmentAddressPostcode) ? orderprocess.ShipmentAddressPostcode : accountShipmentInfo.ShipmentAddressPostcode;
                orderprocess.DateCreated = accountShipmentInfo.CreatedDate.HasValue ? accountShipmentInfo.CreatedDate.Value : orderprocess.DateCreated;
                orderprocess.DeliveryNO = string.IsNullOrEmpty(accountShipmentInfo.DeliveryNo) ? orderprocess.DeliveryNO : accountShipmentInfo.DeliveryNo;
                orderprocess.DateUpdated = DateTime.UtcNow;
                orderprocess.UpdatedBy = accountShipmentInfo.UserId;
                _currentDbContext.Entry(orderprocess).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return true;
            }
            return false;

        }
        public decimal QunatityForOrderDetail(int orderDetailId)
        {
            decimal quantity = 0;
            var qty = _currentDbContext.OrderProcessDetail.Where(u => u.OrderDetailID == orderDetailId && u.IsDeleted != true && u.OrderProcess.IsDeleted != true).ToList();
            if (qty.Count > 0)
            {
                return qty.Sum(u => u.QtyProcessed);
            }
            return quantity;
        }

        public bool UpdateDateInOrder(int OrderId)
        {
            var order = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == OrderId);
            if (order != null)
            {
                order.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(order).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public string UpdateOrderProcessDetail(int OrderProcessDetailId, decimal Quantity, int CurrentUserId, int TenantId, int? SerialId, int? PalletTrackingId, bool? wastedReturn = false)
        {
            string status = "";
            decimal newQty = 0;
            var inventorytranscation = _currentDbContext.
                   InventoryTransactions.FirstOrDefault(u => u.OrderProcessDetailId == OrderProcessDetailId && u.TenentId == TenantId && (!SerialId.HasValue || u.SerialID == SerialId) && ((!PalletTrackingId.HasValue
                   || PalletTrackingId <= 0) || u.PalletTrackingId == PalletTrackingId) && (u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder
                   || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder));
            var orderProcessDetails = _currentDbContext.OrderProcessDetail.FirstOrDefault(u => u.OrderProcessDetailID == OrderProcessDetailId);
            if (wastedReturn == true)
            {
                if (inventorytranscation?.OrderProcessDetail?.QtyProcessed < Quantity)
                {
                    newQty = (Quantity - inventorytranscation?.OrderProcessDetail?.QtyProcessed ?? 0);
                    Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.WastedReturn, newQty, inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, null, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);

                }
                else
                {
                    newQty = (inventorytranscation?.OrderProcessDetail?.QtyProcessed ?? 0) - Quantity;
                    Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.WastedReturn, newQty, inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, null, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);


                }
            }
            #region Serialized Product
            else if (inventorytranscation.SerialID != null)
            {
                var productSerialis = _currentDbContext.ProductSerialization
                                                 .FirstOrDefault(u => u.SerialID == inventorytranscation.SerialID);
                if (productSerialis != null)
                {
                    productSerialis.CreatedBy = CurrentUserId;
                    productSerialis.UpdatedBy = TenantId;
                    productSerialis.DateUpdated = DateTime.UtcNow;
                    if (inventorytranscation.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder)
                    {
                        productSerialis.CurrentStatus = InventoryTransactionTypeEnum.SalesOrder;
                        Inventory.StockTransaction(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentOut, 1, inventorytranscation.OrderID, null, null, inventorytranscation.SerialID, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);

                    }
                    else
                    {
                        productSerialis.CurrentStatus = InventoryTransactionTypeEnum.PurchaseOrder;
                        Inventory.StockTransaction(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentIn, 1, inventorytranscation.OrderID, null, null, inventorytranscation.SerialID, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);
                    }
                    Quantity = orderProcessDetails.QtyProcessed - 1;
                    _currentDbContext.ProductSerialization.Attach(productSerialis);
                    _currentDbContext.Entry(productSerialis).State = EntityState.Modified;


                }

            }
            #endregion
            #region PalletProduct
            else if (inventorytranscation.PalletTrackingId != null)
            {
                var product = _currentDbContext.ProductMaster.FirstOrDefault(u => u.ProductId == inventorytranscation.ProductId);
                var pallet = _currentDbContext.PalletTracking.FirstOrDefault(u => u.PalletTrackingId == PalletTrackingId);
                var quantityPallet = _productService.GetInventoryTransactionsByPalletTrackingId(PalletTrackingId ?? 0, OrderProcessDetailId);
                if (quantityPallet < Quantity)
                {
                    string pallets = string.Format("{0:#,0.00}", (Quantity / Convert.ToDecimal((product.ProductsPerCase ?? 1))));
                    string formate = string.Format("{0:#,0.00}", (Quantity - quantityPallet) / (product.ProductsPerCase ?? 1));
                    newQty = Convert.ToDecimal(string.IsNullOrEmpty(formate) ? "0" : formate);
                    if (inventorytranscation.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder)
                    {
                        if (pallet.Status == PalletTrackingStatusEnum.Created)
                        {
                            return "Pallet is not available to modify orderprocess";
                        }
                        else if ((pallet.RemainingCases + newQty) > pallet.TotalCases)
                        {
                            return "Quantity is exceeding from total cases";
                        }
                        else
                        {
                            pallet.RemainingCases = (pallet.RemainingCases + newQty);
                            Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentIn, Math.Round((newQty * (product?.ProductsPerCase ?? 1)), 2), inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, inventorytranscation.PalletTrackingId, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);
                            Quantity = orderProcessDetails.QtyProcessed + newQty;

                        }
                    }
                    else
                    {
                        if (pallet.Status == PalletTrackingStatusEnum.Created)
                        {
                            return "Pallet is not available to modify orderprocess";
                        }
                        else if (pallet.RemainingCases < newQty)
                        {
                            return "Quantity is exceeding from Remaining cases";
                        }
                        else
                        {
                            pallet.RemainingCases = (pallet.RemainingCases - newQty);
                            Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentOut, Math.Round((newQty * (product?.ProductsPerCase ?? 1)), 2), inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, inventorytranscation.PalletTrackingId, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);
                            Quantity = orderProcessDetails.QtyProcessed + newQty;
                        }
                    }

                }
                else
                {
                    string pallets = string.Format("{0:#,0.00}", (Quantity / Convert.ToDecimal((product.ProductsPerCase ?? 1))));
                    string formate = string.Format("{0:#,0.00}", ((quantityPallet - Quantity)) / Convert.ToDecimal(product.ProductsPerCase ?? 1));
                    newQty = Convert.ToDecimal(string.IsNullOrEmpty(formate) ? "0" : formate);
                    if (inventorytranscation.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder)
                    {
                        if (pallet.Status == PalletTrackingStatusEnum.Created)
                        {
                            return "Pallet is not available to modify orderprocess";
                        }
                        else if (pallet.RemainingCases < newQty)
                        {
                            return "Remaining cases is not enough to modify this order process";

                        }
                        else
                        {
                            pallet.RemainingCases = pallet.RemainingCases - newQty;
                            Quantity = orderProcessDetails.QtyProcessed - newQty;
                            Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentOut, Math.Round((newQty * (product?.ProductsPerCase ?? 1)), 2), inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, inventorytranscation.PalletTrackingId, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);

                        }




                    }
                    else
                    {
                        if (pallet.Status == PalletTrackingStatusEnum.Created)
                        {
                            return "Pallet is not available to modify orderprocess";
                        }
                        if ((pallet.RemainingCases + newQty) > pallet.TotalCases)
                        {
                            return "Quantity is exceeding from total cases";
                        }

                        pallet.RemainingCases = (pallet.RemainingCases + newQty);
                        Quantity = orderProcessDetails.QtyProcessed - newQty;
                        Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentIn, Math.Round((newQty * (product?.ProductsPerCase ?? 1)), 2), inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, inventorytranscation.PalletTrackingId, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);

                    }

                }

                pallet.DateUpdated = DateTime.UtcNow;
                _currentDbContext.PalletTracking.Attach(pallet);
                _currentDbContext.Entry(pallet).State = EntityState.Modified;

            }
            #endregion
            #region NormalProduct
            else
            {
                if (inventorytranscation?.OrderProcessDetail?.QtyProcessed < Quantity)
                {
                    newQty = (Quantity - (inventorytranscation?.OrderProcessDetail?.QtyProcessed ?? 0));

                    if (inventorytranscation.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder)
                    {
                        Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentIn, newQty, inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, inventorytranscation.PalletTrackingId, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);

                    }
                    else
                    {
                        Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentOut, newQty, inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, inventorytranscation.PalletTrackingId, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);
                    }


                }
                else
                {
                    newQty = ((inventorytranscation?.OrderProcessDetail?.QtyProcessed ?? 0) - Quantity);
                    if (inventorytranscation.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder)
                    {
                        Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentOut, newQty, inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, inventorytranscation.PalletTrackingId, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);
                    }
                    else
                    {
                        Inventory.StockTransactionApi(inventorytranscation.ProductId, InventoryTransactionTypeEnum.AdjustmentIn, newQty, inventorytranscation.OrderID, TenantId, caCurrent.CurrentWarehouse().WarehouseId, CurrentUserId, null, inventorytranscation.PalletTrackingId, null, null, inventorytranscation.OrderProcessId, inventorytranscation.OrderProcessDetailId);

                    }
                }
            }
            #endregion
            #region orderProcessEdit

            if (orderProcessDetails != null)
            {
                orderProcessDetails.QtyProcessed = Quantity;
                orderProcessDetails.UpdatedBy = CurrentUserId;
                orderProcessDetails.DateUpdated = DateTime.UtcNow;
                _currentDbContext.OrderProcessDetail.Attach(orderProcessDetails);
                _currentDbContext.Entry(orderProcessDetails).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return "Order Process Updated";

            }
            #endregion
            return status;
        }

        public bool UpdateOrderInvoiceNumber(int orderProcessId, string InvoiceNumber, DateTime? InvoiceDate)
        {
            var orderprocess = _currentDbContext.OrderProcess.FirstOrDefault(u => u.OrderProcessID == orderProcessId);
            bool status = false;
            if (orderprocess != null)
            {
                if (orderprocess.InventoryTransactionTypeId != InventoryTransactionTypeEnum.PurchaseOrder)
                {
                    status = true;
                }
                orderprocess.InvoiceDate = InvoiceDate.HasValue ? InvoiceDate : orderprocess.InvoiceDate;
                orderprocess.InvoiceNo = string.IsNullOrEmpty(InvoiceNumber) ? orderprocess.InvoiceNo : InvoiceNumber;
                _currentDbContext.OrderProcess.Attach(orderprocess);
                _currentDbContext.Entry(orderprocess).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                var orderPorcessCount = _currentDbContext.OrderProcess.Where(u => u.OrderID == orderprocess.OrderID && (string.IsNullOrEmpty(u.InvoiceNo))).Count();
                if (orderPorcessCount <= 0)
                {
                    var order = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == orderprocess.OrderID);
                    order.InvoiceNo = string.Join(",", _currentDbContext.OrderProcess.Where(u => u.OrderID == order.OrderID)
                                                       .Select(u => u.InvoiceNo).ToList());
                    _currentDbContext.OrderProcess.Attach(orderprocess);
                    _currentDbContext.Entry(orderprocess).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }

                return status;

            }
            return false;
        }

        public int[] GetOrderProcessbyOrderId(int OrderId)
        {
            return _currentDbContext.OrderProcess.Where(u => u.OrderID == OrderId && u.IsDeleted != true).Select(u => u.OrderProcessID).ToArray();
        }
        public string ByteToFile(byte[] filebytes)
        {
            string filename = "";
            if (filebytes != null)
            {
                Guid guid = Guid.NewGuid();
                filename = guid.ToString() + ".png";
                string directory = "~/UploadedFiles/SignatureProofs/";
                try
                {
                    string filePath = directory + filename;
                    File.WriteAllBytes(HttpContext.Current.Server.MapPath(filePath), filebytes);
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            return filename;

        }

        public bool CheckOrdersAuthroization(decimal OrderTotal, InventoryTransactionTypeEnum InventoryTransactionType, int UserId, int TenantId, double? CreditLimit = null)
        {
            bool status = false;
            var orderauthorization = _currentDbContext.TenantConfigs.Where(u => u.TenantId == TenantId).FirstOrDefault();
            var orderlimit = _currentDbContext.AuthUsers.FirstOrDefault(u => u.UserId == UserId)?.OrderValueLimit;
            if (orderauthorization != null && orderauthorization.OrdersAuthroisingRequired == true)
            {
                switch ((InventoryTransactionTypeEnum)InventoryTransactionType)
                {
                    case InventoryTransactionTypeEnum.PurchaseOrder:
                        if (OrderTotal > orderlimit)
                            status = true;
                        return status;

                    case InventoryTransactionTypeEnum.SalesOrder:
                        if (CreditLimit.HasValue && OrderTotal > (decimal)CreditLimit)
                            status = true;
                        return status;

                    default:
                        return status;
                }
            }

            return false;
        }
        public decimal GetDiscountOnTotalCost(int accountId, decimal OrdersTotal)
        {
            decimal discount = 0;
            if (accountId > 0)
            {
                var account = _currentDbContext.Account.FirstOrDefault(u => u.AccountID == accountId && u.IsDeleted != true);
                if (account != null && account.TenantPriceGroups != null && account.TenantPriceGroups.ApplyDiscountOnTotal && account.TenantPriceGroups.Percent > 0)
                {
                    discount = Math.Round(((OrdersTotal * account.TenantPriceGroups.Percent) / 100), 2);
                }

            }
            return discount;
        }

        public CheckoutViewModel CreateShopOrder(CheckoutViewModel orderDetails, int tenantId, int UserId, int warehouseId, int SiteId)
        {
            Order order = new Order();
            decimal total = 0;
            if (string.IsNullOrEmpty(order.OrderNumber)){
                order.OrderNumber = GenerateNextOrderNumber(InventoryTransactionTypeEnum.SalesOrder, tenantId);
            }

            var duplicateOrder = _currentDbContext.Order.FirstOrDefault(m => m.OrderNumber.Equals(order.OrderNumber, StringComparison.CurrentCultureIgnoreCase));
            if (duplicateOrder != null)
            {
                throw new Exception($"Order Number {order.OrderNumber} already associated with another Order. Please regenerate order number.", new Exception("Duplicate Order Number"));
            }
            order.AccountID = orderDetails.AccountId;
            order.InventoryTransactionTypeId = InventoryTransactionTypeEnum.SalesOrder;
            order.IssueDate = DateTime.UtcNow;
            order.ExpectedDate = DateTime.UtcNow;
            order.OrderStatusID = OrderStatusEnum.Hold;
            order.DateCreated = DateTime.UtcNow;
            order.DateUpdated = DateTime.UtcNow;
            order.TenentId = tenantId;
            order.CreatedBy = UserId;
            order.UpdatedBy = UserId;
            order.WarehouseId = warehouseId;
            order.SiteID = SiteId;
            order.OrderStatusID = OrderStatusEnum.Hold;
            
            _currentDbContext.Order.Add(order);
            _currentDbContext.SaveChanges();
            orderDetails.OrderNumber = order.OrderNumber;
            if (orderDetails.CartItems != null)
            {
                var items = orderDetails.CartItems.ToList();
                foreach (var item in items)
                {
                    OrderDetail detail = new OrderDetail();
                    detail.ProductId = item.ProductId;
                    detail.Qty = item.Qty;
                    detail.Warranty = null;
                    detail.Price = item.Price;
                    detail.ProductMaster = null;
                    detail.WarehouseId = warehouseId;
                    detail.DateCreated = DateTime.UtcNow;
                    detail.CreatedBy = UserId;
                    detail.TenentId = tenantId;
                    detail.OrderID = order.OrderID;
                    total = total + (item.Price * item.Qty);
                    _currentDbContext.OrderDetail.Add(detail);
                }
                order.OrderTotal = total;

                var cartIds = orderDetails.CartItems.Select(u => u.CartId).ToList();
                var cartItemsList =
                    _currentDbContext.WebsiteCartItems.Where(a => cartIds.Contains(a.Id) && a.IsDeleted != true).ToList();
                if (cartItemsList.Count > 0)
                {
                    cartItemsList.ForEach(u=>u.IsDeleted=true);
                }



                _currentDbContext.SaveChanges();

            }

            return orderDetails;
        }

        public bool UpdatePickerId(int OrderId, int? pickerId, int userId)
        {
            var order = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == OrderId);
            if (order != null)
            {
                order.PickerId = pickerId;
                order.UpdatedBy = userId;
                order.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(order).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return true;
            }
            return false;

        }
    }
}