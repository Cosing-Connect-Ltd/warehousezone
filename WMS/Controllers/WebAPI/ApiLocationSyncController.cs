using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiLocationSyncController : BaseApiController
    {
        private readonly ILookupServices _LookupService;
        private readonly IMapper _mapper;

        public ApiLocationSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, ILookupServices LookupService, IMapper mapper) :
            base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _LookupService = LookupService;
            _mapper = mapper;
        }

        // GET http://localhost:8005/api/sync/Locations/{reqDate}/{serialNo}
        // GET http://localhost:8005/api/sync/Locations/2014-11-23/920013c000814
        public IHttpActionResult GetLoctions(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new LocationSyncCollection();

            var alllocations = _LookupService.GetAllLocations(terminal.TenantId, reqDate, true);

            var LocationList = new List<LocationSync>();

            foreach (var l in alllocations)
            {
                var LocationSync = new LocationSync();
                LocationSync.Description = l.Description;
                LocationSync.LocationCode = l.LocationCode;
                LocationSync.LocationGroup = l.LocationGroup?.Locdescription;
                LocationSync.LocationType = l.LocationType?.LocTypeName;
                LocationSync.LocationId = l.LocationId;
                LocationSync.LocationGroupId = l.LocationGroupId;
                LocationSync.LocationTypeId = l.LocationTypeId;
                LocationSync.WarehouseId = l.WarehouseId;
                LocationSync.WarehouseName = l.TenantWarehouses.WarehouseName;
                LocationSync.IsActive = l.IsActive;
                LocationSync.IsDeleted = l.IsDeleted;
                LocationSync.SortOrder = l.SortOrder;
                LocationList.Add(LocationSync);
            }

            result.Count = LocationList.Count;
            result.TerminalLogId = TerminalServices
                .CreateTerminalLog(reqDate, terminal.TenantId, LocationList.Count, terminal.TerminalId,
                    TerminalLogTypeEnum.LocationsSync).TerminalLogId;
            result.LocationSync = LocationList;
            return Ok(result);
        }

        // GET http://localhost:8005/api/sync/locations-stocks/{reqDate}/{serialNo}
        // GET http://localhost:8005/api/sync/locations-stocks/2014-11-23/920013c000814
        public IHttpActionResult GetLoctionsStocks(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new ProductLocationStocksSyncCollection();

            var alllStocks = _LookupService.GetAllLocationStocks(terminal.TenantId, reqDate, true);

            var StocksList = new List<ProductLocationStocksSync>();

            foreach (var stock in alllStocks)
            {
                var LocationStockSync = new ProductLocationStocksSync();
                _mapper.Map(stock, LocationStockSync);
                StocksList.Add(LocationStockSync);
            }

            result.Count = StocksList.Count;
            result.TerminalLogId = TerminalServices
                .CreateTerminalLog(reqDate, terminal.TenantId, StocksList.Count, terminal.TerminalId,
                    TerminalLogTypeEnum.ProductLocationStocksSync).TerminalLogId;
            result.ProductLocationStocksSync = StocksList;
            return Ok(result);
        }

        // POST http://localhost:8005/api/sync/post-stockmovement-detail
        public IHttpActionResult PostStockMovementDetail(StockMovementCollectionViewModel data)
        {
            data.SerialNo = data.SerialNo.Trim().ToLower();
            var terminal = TerminalServices.GetTerminalBySerial(data.SerialNo);
            if (terminal == null)
            {
                return Unauthorized();
            }
            var updateStockMovement = _LookupService.UpdateStockMovement(data);
            TerminalServices.CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, data.StockMovements?.Count ?? 0, terminal.TerminalId, TerminalLogTypeEnum.PostStockMovement);
            return Ok(updateStockMovement);
        }


    }
}