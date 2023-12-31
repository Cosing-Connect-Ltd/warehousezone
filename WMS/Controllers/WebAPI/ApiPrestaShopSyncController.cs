﻿using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace WMS.Controllers.WebAPI
{
    public class ApiPrestaShopSyncController : BaseApiController
    {

        private readonly ILookupServices _lookupService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IProductServices _productService;
        private readonly IMapper _mapper;
        public readonly IApplicationContext context;
        public readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private DataImportFactory dataImportFactory = new DataImportFactory();

        public ApiPrestaShopSyncController(ITerminalServices terminalServices, IPurchaseOrderService purchaseOrderService,
            ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, ILookupServices lookupService, IProductServices productService, IMapper mapper, IApplicationContext contexts) :
            base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _lookupService = lookupService;
            _purchaseOrderService = purchaseOrderService;
            _productService = productService;
            _orderService = orderService;
            _mapper = mapper;
            _userService = userService;
            context = contexts;
        }

        // Get http://localhost:8005/api/sync/Import-PrestaShop-Orders/?TenatId=1&WarehouseId=1

        [HttpGet]
        public IHttpActionResult ImportPrestaShopOrders(int TenatId, int WarehouseId)
        {
            var sites = _userService.GetApiCredentials(TenatId, WarehouseId, ApiTypes.PrestaShop).ToList();
            string result = "";
            foreach (var item in sites)
            {
                result = dataImportFactory.GetPrestaShopOrdersSync(item.TenantId, item.DefaultWarehouseId, item.ApiUrl, item.ApiKey, item.Id).Result;
            }

            return Ok(!string.IsNullOrEmpty(result) ? result : "Orders synced successfully");
        }

        [HttpGet]
        //Post http://localhost:8005/api/sync/Post-PrestaShop-ProductStock/?TenatId=1&WarehouseId=1
        public async Task<IHttpActionResult> PrestaShopStockSync(int TenatId, int WarehouseId)
        {
            var sites = _userService.GetApiCredentials(TenatId, WarehouseId, ApiTypes.PrestaShop);
            string result = "";
            foreach (var item in sites)
            {
                result = await dataImportFactory.PrestaShopStockSync(item.TenantId, item.DefaultWarehouseId, item.ApiUrl, item.ApiKey, item.Id);
            }

            return Ok(result);
        }

        [HttpGet]
        //Post http://localhost:8005/api/sync/Get-PrestaShop-country/?TenatId=1&WarehouseId=1
        public async Task<IHttpActionResult> GetPrestaShopCountry(int TenatId, int WarehouseId)
        {
            var sites = _userService.GetApiCredentials(TenatId, WarehouseId, ApiTypes.PrestaShop);
            string result = "";

            foreach (var item in sites)
            {
                var data = await dataImportFactory.GetPrestaShopCountry(null, item.ApiUrl, item.ApiKey);
                if (data.Count > 0)
                    result = "Countries Imported";
                break;
            }

            return Ok(result);
        }
    }
}
