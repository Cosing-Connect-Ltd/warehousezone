using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiOrderProcessesSyncController : BaseApiController
    {
        private readonly IAccountServices _accountServices;
        private readonly IGaneConfigurationsHelper _configHelper;
        private readonly IMapper _mapper;

        public ApiOrderProcessesSyncController(ITerminalServices terminalServices,
            ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, IAccountServices accountServices, IGaneConfigurationsHelper configHelper, IMapper mapper) :
            base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _accountServices = accountServices;
            _configHelper = configHelper;
            _mapper = mapper;
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

                        results.Add(order);

                        if (order.OrderStatusID == OrderStatusEnum.AwaitingAuthorisation)
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

    }
}