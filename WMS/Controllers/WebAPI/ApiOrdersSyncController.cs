using AutoMapper;
using DevExpress.DataProcessing;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WMS.Controllers.WebAPI
{
    [EnableCors("*", "*", "*")]
    public class ApiOrdersSyncController : BaseApiController
    {

        private readonly ILookupServices _lookupService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IProductServices _productService;
        private readonly IAccountServices _accountServices;
        private readonly IGaneConfigurationsHelper _configurationHelper;
        private readonly IMapper _mapper;

        public ApiOrdersSyncController(ITerminalServices terminalServices, IPurchaseOrderService purchaseOrderService,
            ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, ILookupServices lookupService, IProductServices productService, IMapper mapper, IAccountServices accountServices, IGaneConfigurationsHelper configurationHelper) :
            base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _lookupService = lookupService;
            _purchaseOrderService = purchaseOrderService;
            _productService = productService;
            _accountServices = accountServices;
            _configurationHelper = configurationHelper;
            _mapper = mapper;
        }

        // GET http://ganetest.qsrtime.net/api/sync/orders/{reqDate}/{serialNo}
        // GET http://ganetest.qsrtime.net/api/sync/orders/2014-11-23/920013c000814
        public IHttpActionResult GetOrders(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            reqDate = TerminalServices.GetTerminalSyncDate(reqDate, terminal.TenantId);

            var result = new OrdersSyncCollection();

            var allOrders = OrderService.GetAllOrders(terminal.TenantId, terminal.WarehouseId, true, reqDate, true).ToList();
            var warehouses = _lookupService.GetAllWarehousesForTenant(terminal.TenantId);
            var orders = new List<OrdersSync>();

            foreach (var p in allOrders)
            {
                var order = new OrdersSync();
                var mapped = _mapper.Map(p, order);
                mapped.TransferWarehouseName = warehouses.FirstOrDefault(x => x.WarehouseId == mapped.TransferWarehouseId)?.WarehouseName;

                for (var i = 0; i < p.OrderDetails.Count; i++)
                {
                    mapped.OrderDetails[i].ProductAttributeValueName = p.OrderDetails.ToList()[i].ProductAttributeValue?.Value;
                }
                //if user is assocaited to the account
                if (p.AccountID != null)
                {
                    var user = UserService.GetAuthUserByAccountId(p.AccountID);
                    if (user != null)
                    {
                        mapped.FullName = user.DisplayName;
                        mapped.MobileNumber = user.UserMobileNumber;

                    }
                }

                orders.Add(mapped);
            }

            result.Count = orders.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, orders.Count, terminal.TerminalId, TerminalLogTypeEnum.OrdersSync).TerminalLogId;
            result.Orders = orders;
            return Ok(_mapper.Map(result, new OrdersSyncCollection()));
        }

        // GET http://ganetest.qsrtime.net/api/sync/account-orders/{reqDate}/{serialNo}
        // GET http://ganetest.qsrtime.net/api/sync/account-orders/2014-11-23/920013c000814
        public IHttpActionResult GetOrdersByAccount(DateTime reqDate, string serialNo, int accountId)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);
            var account = _accountServices.GetAccountsById(accountId);

            if (terminal == null) { return Unauthorized(); }
            if (account == null) { return NotFound(); }

            reqDate = TerminalServices.GetTerminalSyncDate(reqDate, terminal.TenantId);

            var result = new OrdersSyncCollection();

            var allOrders = OrderService.GetAllDirectSalesOrdersByAccount(terminal.TenantId, accountId, reqDate, true);
            var warehouses = _lookupService.GetAllWarehousesForTenant(terminal.TenantId);
            var orders = new List<OrdersSync>();

            foreach (var p in allOrders)
            {
                var order = new OrdersSync();
                var mapped = _mapper.Map(p, order);

                for (var i = 0; i < p.OrderDetails.Count; i++)
                {
                    mapped.OrderDetails[i].ProductAttributeValueName = p.OrderDetails.ToList()[i].ProductAttributeValue?.Value;
                }

                mapped.TransferWarehouseName = warehouses.FirstOrDefault(x => x.WarehouseId == mapped.TransferWarehouseId)?.WarehouseName;

                //if user is assocaited to the account
                if (p.AccountID != null)
                {
                    var user = UserService.GetAuthUserByAccountId(p.AccountID);
                    if (user != null)
                    {
                        mapped.FullName = user.DisplayName;
                        mapped.MobileNumber = user.UserMobileNumber;

                    }
                }

                orders.Add(mapped);
            }

            result.Count = orders.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, orders.Count, terminal.TerminalId, TerminalLogTypeEnum.OrdersSync).TerminalLogId;
            result.Orders = orders;
            return Ok(_mapper.Map(result, new OrdersSyncCollection()));
        }

        // GET http://ganetest.qsrtime.net/api/sync/order-status/{serialNo}
        // GET http://ganetest.qsrtime.net/api/sync/order-status/920013c000814
        //TODO: UserId should be passed to this method rather than using 0 as user id
        [HttpGet]
        [Obsolete("Use Post UpdateOrderStatus instead")]
        public IHttpActionResult UpdateOrderStatus(string serialNo, int orderId, OrderStatusEnum statusId)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }
            var result = OrderService.UpdateOrderStatus(orderId, statusId, 0);
            return Ok(_mapper.Map(result, new OrdersSync()));
        }

        // POST http://ganetest.qsrtime.net/api/sync/order-status
        [HttpPost]
        public async Task<IHttpActionResult> UpdateOrderStatus(UpdateOrderStatusViewModel model)
        {
            model.SerialNo = model.SerialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(model.SerialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var user = UserService.GetAuthUserById(model.UserId);

            if (user == null)
            {
                return Unauthorized();
            }

            var order = OrderService.GetOrderById(model.OrderId);

            if (order == null)
            {
                return NotFound();
            }

            var result = OrderService.UpdateOrderStatus(model.OrderId, model.StatusId, model.UserId);

            await NotifyOrderStatusChange(result.OrderStatusID, order);

            return Ok(_mapper.Map(result, new OrdersSync()));
        }

        private async Task NotifyOrderStatusChange(OrderStatusEnum statusId, Order order)
        {
            if (order.InventoryTransactionTypeId != InventoryTransactionTypeEnum.SalesOrder ||
                order.InventoryTransactionTypeId != InventoryTransactionTypeEnum.DirectSales ||
                order.Warehouse?.SelectedNotifiableOrderStatuses == null ||
                !order.Warehouse.SelectedNotifiableOrderStatuses.Contains(order.OrderStatusID))
            {
                return;
            }

            if (order.SendOrderStatusByEmail)
            {
                await _configurationHelper.CreateTenantEmailNotificationQueue($"#{order.OrderNumber} - Order status updated to {statusId}", _mapper.Map(order, new OrderViewModel()), worksOrderNotificationType: WorksOrderNotificationTypeEnum.OrderStatusNotification);
            }

            if (order.SendOrderStatusBySms)
            {
                await UserService.SendSmsBroadcast(order.Account.AccountOwner.UserMobileNumber, order.Tenant.TenantName, order.Account.OwnerUserId.ToString(), $"#{order.OrderNumber} - Order status updated to {statusId}");
            }

        }

        [HttpPost]
        public IHttpActionResult GoodsReturn(GoodsReturnRequestSync model)
        {
            var terminal = TerminalServices.GetTerminalBySerial(model.SerialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(model.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            bool proccess = true;
            var result = _mapper.Map(model, new GoodsReturnResponse());
            var product = _productService.GetProductMasterById(model.ProductId);
            if (product != null)
            {
                if (model.userId > 0)
                {
                    if (!product.Serialisable && !product.ProcessByPallet)
                    {
                        model.OrderId = Inventory.StockTransaction(model, null, null);

                    }
                    if (product.Serialisable)
                    {
                        if (model.ProductSerials != null || model.ProductSerials.Count > 0)
                        {
                            model.OrderId = Inventory.StockTransaction(model, null, null, null);
                        }
                        else
                        {
                            model.OrderId = -1;
                        }
                    }
                    if (product.ProcessByPallet)
                    {
                        if (model.InventoryTransactionType.HasValue && (model.InventoryTransactionType == InventoryTransactionTypeEnum.PurchaseOrder || model.InventoryTransactionType == InventoryTransactionTypeEnum.SalesOrder))
                        {
                            proccess = false;
                        }
                        model.OrderId = _purchaseOrderService.ProcessPalletTrackingSerial(model, null, proccess);
                    }

                    if (model.OrderId >= 0)
                    {
                        result.IsSuccess = true;
                        result.CanProceed = true;
                        result.orderId = model.OrderId;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.CanProceed = false;
                        result.FailureMessage = "Serialisable products must have serials along with stock.";
                    }


                }
                else
                {
                    result.FailureMessage = "User Id must be greater than 0.";
                }
            }
            else { result.FailureMessage = "Product not found."; }


            return Ok(result);
        }

        public async Task<IHttpActionResult> VerifyProductInfoBySerial(ProductDetailRequest request)
        {
            var result = await _productService.GetProductInfoBySerial(request.SerialCode, request.TenantId);

            return Ok(result);
        }

        public IHttpActionResult GetOrderss(int? orderId, int shopId, string orderNumber)
        {
            var result = new OrdersSyncCollection();

            List<Order> list = OrderService.GetAllOrdersByTenantId(shopId).Where(u => (orderId.HasValue || u.IsDeleted != true) && (string.IsNullOrEmpty(orderNumber) || u.OrderNumber == orderNumber) && (!orderId.HasValue || (int?)u.OrderID == orderId) && (int)u.InventoryTransactionTypeId == 2).ToList<Order>();
            var warehouses = _lookupService.GetAllWarehousesForTenant(shopId);
            var orders = new List<OrdersSync>();

            foreach (var p in list)
            {
                var order = new OrdersSync();
                var mapped = _mapper.Map(p, order);
                for (var i = 0; i < p.OrderDetails.Count; i++)
                {
                    mapped.OrderDetails[i].ProductAttributeValueName = p.OrderDetails.ToList()[i].ProductAttributeValue?.Value;
                    mapped.OrderDetails[i].ProductAttributeValueName = p.OrderDetails.ToList<OrderDetail>()[i].ProductAttributeValue?.Value;
                    mapped.OrderDetails[i].ProductName = p.OrderDetails.ToList<OrderDetail>()[i].ProductMaster?.Name;
                    mapped.OrderDetails[i].SkuCode = p.OrderDetails.ToList<OrderDetail>()[i].ProductMaster?.SKUCode;
                    mapped.OrderDetails[i].Barcode = p.OrderDetails.ToList<OrderDetail>()[i].ProductMaster?.BarCode;
                    OrderDetailSync orderDetail = mapped.OrderDetails[i];
                    ProductMaster productMaster = p.OrderDetails.ToList<OrderDetail>()[i].ProductMaster;
                    int num = productMaster != null ? (productMaster.ProcessByPallet ? 1 : 0) : 0;
                    orderDetail.ProcessByPallet = num != 0;
                    mapped.OrderDetails[i].QuantityProcessed = new Decimal?(p.OrderDetails.ToList<OrderDetail>()[i].ProcessedQty);
                    mapped.OrderDetails[i].InStock = _productService.GetAllPalletTrackings(1, shopId).Any(u => u.ProductId == productMaster.ProductId && u.Status == PalletTrackingStatusEnum.Active);
                }
                //if user is assocaited to the account
                if (p.AccountID != null)
                {
                    var user = UserService.GetAuthUserByAccountId(p.AccountID);
                    if (user != null)
                    {
                        mapped.FullName = user.DisplayName;
                        mapped.MobileNumber = user.UserMobileNumber;

                    }
                }

                orders.Add(mapped);
            }

            result.Count = orders.Count;
            result.Orders = orders;
            return Ok(_mapper.Map(result, new OrdersSyncCollection()));
        }



    }
}