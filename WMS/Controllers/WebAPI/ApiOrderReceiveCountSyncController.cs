using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiOrderReceiveCountSyncController : BaseApiController
    {
        private readonly IMapper _mapper;

        public ApiOrderReceiveCountSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IMapper mapper)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _mapper = mapper;
        }

        // call example through URI http://localhost:8005/api/sync/get-order-receive-count?ReqDate=2014-11-23&SerialNo=920013c000814
        public IHttpActionResult GetOrderReceiveCount(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            reqDate = TerminalServices.GetTerminalSyncDate(reqDate, terminal.TenantId);

            var result = new OrderReceiveCountSyncCollection();
            var orderReceiveCountsSync = new List<OrderReceiveCountSync>();

            var allOrderReceiveCounts = OrderService.GetAllOrderReceiveCounts(terminal.TenantId, terminal.WarehouseId, reqDate).ToList();
            _mapper.Map(allOrderReceiveCounts, orderReceiveCountsSync);


            result.Count = orderReceiveCountsSync.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, orderReceiveCountsSync.Count(), terminal.TerminalId, TerminalLogTypeEnum.OrderReceiveCountSync).TerminalLogId;
            result.OrderReceiveCountSync = orderReceiveCountsSync;
            return Ok(result);
        }

        // POST http://localhost:8005/api/sync/post-order-receive-count
        public IHttpActionResult PostOrderReceiveCount(OrderReceiveCountSyncCollection data)
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

            var results = new OrderReceiveCountSyncCollection();

            if (data.OrderReceiveCountSync != null && data.OrderReceiveCountSync.Any())
            {

                foreach (var item in data.OrderReceiveCountSync)
                {
                    var countRecord = OrderService.SaveOrderReceiveCount(item, terminal);

                    results.OrderReceiveCountSync.Add(countRecord);
                    results.Count += 1;
                }
            }

            TerminalServices.CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, data.OrderReceiveCountSync.Count, terminal.TerminalId, TerminalLogTypeEnum.PostOrderReceiveCount);

            return Ok(results);
        }

    }
}