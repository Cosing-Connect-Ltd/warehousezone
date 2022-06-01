using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Ganedata.Core.Models.AdyenPayments;

namespace WMS.Controllers.WebAPI
{
    public class ApiOrderProcessesSyncController : BaseApiController
    {
        private readonly IAccountServices _accountServices;
        private readonly ITenantsServices _tenantServices;
        private readonly IDeliverectSyncService _deliverectSyncService;
        private readonly IGaneConfigurationsHelper _configHelper;
        private readonly IMapper _mapper;
        private readonly IShoppingVoucherService _shoppingVoucherService;
        private readonly IPalletingService _palletingService;
        public ApiOrderProcessesSyncController(ITerminalServices terminalServices, IDeliverectSyncService deliverectSyncService,
            ITenantLocationServices tenantLocationServices, IOrderService orderService, ITenantsServices tenantsServices,
            IProductServices productServices, IUserService userService, IAccountServices accountServices,
            IGaneConfigurationsHelper configHelper, IMapper mapper, IShoppingVoucherService shoppingVoucherService,IPalletingService palletingService) :
            base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _accountServices = accountServices;
            _tenantServices = tenantsServices;
            _deliverectSyncService = deliverectSyncService;
            _configHelper = configHelper;
            _mapper = mapper;
            _shoppingVoucherService = shoppingVoucherService;
            _palletingService = palletingService;
        }

        // GET http://ganetest.qsrtime.net/api/sync/order-processes/{reqDate}/{serialNo}
        // GET http://ganetest.qsrtime.net/api/sync/order-processes/2014-11-23/920013c000814
        public IHttpActionResult GetOrderProcesses(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            reqDate = TerminalServices.GetTerminalSyncDate(reqDate, terminal.TenantId);

            var accounts = _accountServices.GetAllAccountsSelectList(terminal.TenantId);

            var result = new OrderProcessesSyncCollection();

            var allorderProcess = OrderService.GetAllOrderProcesses(reqDate, 0, null, null, true);

            var orderProcesses = new List<OrderProcessesSync>();
            foreach (var item in allorderProcess)
            {
                var pSync = new OrderProcessesSync();
                _mapper.Map(item, pSync);

                var orderProcessDetails = new List<OrderProcessDetailSync>();

                foreach (var p in item.OrderProcessDetail)
                {
                    var order = new OrderProcessDetailSync();
                    var pd = _mapper.Map(p, order);
                    pd.ProductAttributeValueName = p.ProductAttributeValue?.Value;
                    orderProcessDetails.Add(pd);
                }

                pSync.OrderProcessDetails = orderProcessDetails;
                orderProcesses.Add(pSync);
            }

            result.Count = orderProcesses.Count;
            result.TerminalLogId = TerminalServices
                .CreateTerminalLog(reqDate, terminal.TenantId, orderProcesses.Count, terminal.TerminalId,
                    TerminalLogTypeEnum.OrderProcessSync).TerminalLogId;
            result.OrderProcesses = orderProcesses;
            return Ok(_mapper.Map(result, new OrderProcessesSyncCollection()));
        }


        // POST http://ganetest.qsrtime.net/api/sync/post-order-processes
        public async Task<IHttpActionResult> PostOrderProcesses(OrderProcessesSyncCollection data)
        {
            data.SerialNo = data.SerialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(data.SerialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(data.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            var mobileLocation = TerminalServices.GetMobileLocationByTerminalId(terminal.TerminalId);
            if (mobileLocation != null)
            {
                terminal.WarehouseId = mobileLocation.WarehouseId;
            }

            var results = new List<OrdersSync>();

            if (data.OrderProcesses != null && data.OrderProcesses.Any())
            {
                var groupedOrderProcesses = data.OrderProcesses.GroupBy(p => p.OrderToken, (key, g) => new { OrderToken = key, OrderProcesses = g.ToList() });

                foreach (var processGroup in groupedOrderProcesses)
                {
                    var orderProcesses = processGroup.OrderProcesses;
                    foreach (var item in orderProcesses)
                    {
                        var order = OrderService.SaveOrderProcessSync(item, terminal);
                       
                        if (!string.IsNullOrEmpty(item.VoucherCode))
                        {
                            _shoppingVoucherService.ApplyVoucher(new ShoppingVoucherValidationRequestModel()
                            {
                                UserId = item.CreatedBy,
                                VoucherCode = item.VoucherCode,
                                OrderId = order.OrderID
                            });
                        }

                        var deliverectSyncResult = true;
                        var tenantConfig = _tenantServices.GetTenantConfigById(terminal.TenantId);
                        if (tenantConfig.LoyaltyAppOrderProcessType == LoyaltyAppOrderProcessTypeEnum.Deliverect &&
                            !string.IsNullOrEmpty(order.DeliverectChannelLinkId?.Trim()) &&
                            !string.IsNullOrEmpty(order.DeliverectChannel?.Trim()))
                        {
                            deliverectSyncResult = await _deliverectSyncService.SendOrderToDeliverect(order);

                            if (!deliverectSyncResult)
                            {
                                OrderService.UpdateOrderStatus(order.OrderID, OrderStatusEnum.Hold, 1);
                            }
                        }

                        if (deliverectSyncResult)
                        {
                            results.Add(order);
                        }

                        if (order.OrderStatusID == OrderStatusEnum.AwaitingAuthorisation && deliverectSyncResult)
                        {
                            OrderViewModel orderViewModel = new OrderViewModel();
                            orderViewModel.OrderID = order.OrderID;
                            orderViewModel.TenentId = order.TenentId;
                            orderViewModel.AccountID = order.AccountID;
                            await _configHelper.CreateTenantEmailNotificationQueue($"#{order.OrderNumber} - Order Requires Authorisation", orderViewModel, null, shipmentAndRecipientInfo: null,
                           worksOrderNotificationType: WorksOrderNotificationTypeEnum.AwaitingOrderTemplate);
                        }
                    }
                }
            }

            TerminalServices.CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, data.OrderProcesses.Count, terminal.TerminalId, TerminalLogTypeEnum.OrderProcessesPost);

            return Ok(results);
        }
        public IHttpActionResult GetOrderProcessesByOrderNumber(int shopId, string orderNumber)
        {

            var allorderProcess = OrderService.GetAllOrderProcesses(null, 0, null, null, true).Where(u => u.Order.OrderNumber.Equals(orderNumber) && u.WarehouseId == shopId).SelectMany(u => u.OrderProcessDetail);

            var orderProcesses = new OrderProcessPallets();
            orderProcesses.OrderId = allorderProcess.FirstOrDefault()?.OrderProcess?.OrderID ?? 0;
            orderProcesses.OrderProcessId = allorderProcess.FirstOrDefault()?.OrderProcess?.OrderProcessID ?? 0;
            orderProcesses.AccountId = allorderProcess.FirstOrDefault()?.OrderProcess?.Order?.AccountID ?? 0;

            foreach (var item in allorderProcess)
            {
                var orderProcessDetailList = new OrderProcessDetailList();
                orderProcessDetailList.OrderProcessDetailId = item.OrderProcessDetailID;
                orderProcessDetailList.Quantity = item.QtyProcessed;
                orderProcessDetailList.PalletedQuantity = item.PalletedQuantity;
                orderProcessDetailList.ProductId = item.ProductId;
                orderProcessDetailList.ProductName = item.ProductMaster.Name;
                orderProcessDetailList.SkuCode = item.ProductMaster.SKUCode;
                orderProcesses.OrderProcessDetailList.Add(orderProcessDetailList);

            }
            var palletDetails = _palletingService.GetAllPallets(orderProcesses.OrderProcessId).Select(u => new PalletList
            {
                PalletID = u.PalletID,
                PalletNumber = u.PalletNumber,
                PalletProducts = u.PalletProducts.Where(c=>c.IsDeleted != true).Select(g => new PalletProductList
                {
                    ProductId = g.ProductID,
                    ProductName = g.Product.Name,
                    SkuCode = g.Product.SKUCode,
                    Quantity = g.Quantity

                }).ToList()

            });
            orderProcesses.PalletList.AddRange(palletDetails);
            return Ok(orderProcesses);
        }

    }

}