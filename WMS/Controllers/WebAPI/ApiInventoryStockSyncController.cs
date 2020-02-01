using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiInventoryStockSyncController : BaseApiController
    {
        private readonly ICommonDbServices _commonDbServices;
        private readonly IMapper _mapper;

        public ApiInventoryStockSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, 
            IUserService userService, ICommonDbServices commonDbServices, IMapper mapper) 
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _commonDbServices = commonDbServices;
            _mapper = mapper;
        }

        // GET http://localhost:8005/api/sync/inventorystocks/{reqDate}/{serialNo}
        // GET http://localhost:8005/api/sync/inventorystocks/2014-11-23/920013c000814
        public IHttpActionResult GetInventorystocks(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new InventoryStockSyncCollection();

            var allInventoryStocks = ProductServices.GetAllInventoryStocks(terminal.TenantId, terminal.WarehouseId, reqDate);

            var inventoryStocks = new List<InventoryStockSync>();

            foreach (var p in allInventoryStocks)
            {
                var inventory = new InventoryStockSync();
                var mapped = _mapper.Map(p, inventory);
                inventoryStocks.Add(mapped);
            }

            result.Count = inventoryStocks.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, inventoryStocks.Count, terminal.TerminalId, TerminalLogTypeEnum.InventoryStockSync).TerminalLogId;
            result.InventoryStocks = inventoryStocks;
            return Ok(result);
        }

        // GET http://localhost:8005/api/sync/product-stock/{serialNo}/{productId}
        public IHttpActionResult GetInventoryStockForProduct(string serialNo, int productId, int warehouseId)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new InventoryStockSync();

            var inventoryStock = ProductServices.GetInventoryStocksByProductAndTenantLocation(productId, warehouseId);

            result = _mapper.Map(inventoryStock, result);
            if (result != null)
            {
                result.LocationDetails = _commonDbServices.LocationsByProductDetails(productId, warehouseId);

                result.Count = result.LocationDetails.ProductDetails.Count;

                result.TerminalLogId = TerminalServices
                    .CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, 1, terminal.TerminalId,
                        TerminalLogTypeEnum.InventoryStockSync).TerminalLogId;

                return Ok(result);
            }
            return BadRequest("No stock information available for the requested product in the warehouse.");
        }
    }
}