using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiProductLocationStocksSyncController : BaseApiController
    {
        private readonly ICommonDbServices _commonDbServices;
        private readonly IMapper _mapper;

        public ApiProductLocationStocksSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices,
            IUserService userService, ICommonDbServices commonDbServices, IMapper mapper)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _commonDbServices = commonDbServices;
            _mapper = mapper;
        }

        // GET http://localhost:8005/api/sync/product-location--stocks/{reqDate}/{serialNo}
        // GET http://localhost:8005/api/sync/product-location--stocks/2014-11-23/920013c000814
        public IHttpActionResult GetProductLocationStocks(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new ProductLocationStocksSyncCollection();

            var allStocks = ProductServices.GetAllProductLocationStocks(terminal.TenantId, terminal.WarehouseId, reqDate);

            var stocks = new List<ProductLocationStocksSync>();

            foreach (var p in allStocks)
            {
                var stock = new ProductLocationStocksSync();
                var mapped = _mapper.Map(p, stock);
                stocks.Add(mapped);
            }

            result.Count = stocks.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, stocks.Count, terminal.TerminalId, TerminalLogTypeEnum.ProductLocationStocksSync).TerminalLogId;
            result.ProductLocationStocksSync = stocks;
            return Ok(result);
        }
    }
}