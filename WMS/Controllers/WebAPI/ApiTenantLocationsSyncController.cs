using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using Elmah;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiTenantLocationsSyncController : BaseApiController
    {
        private readonly ILookupServices _LookupService;
        private readonly IMapper _mapper;

        public ApiTenantLocationsSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, ILookupServices LookupService, IMapper mapper) :
            base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _LookupService = LookupService;
            _mapper = mapper;
        }
        // GET http://localhost:8005/api/lookup/warehouse/{warehouseId}
        [HttpGet]
        public IHttpActionResult GetTenantLocation(int id)
        {
            var warehouse = TenantLocationServices.GetTenantLocationById(id);
            var result = new TenantLocationsSync();
            
            _mapper.Map(warehouse, result);

            result.OpeningHours = TenantLocationServices.GetOpeningTimesByWarehouseId(id);

            return Ok(result);
        }

        // GET http://localhost:8005/api/sync/tenant-locations/2014-11-23/920013c000814
        public IHttpActionResult GetTenantLoctions(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new TenantLocationsSyncCollection();

            var allTenantLocations = TenantLocationServices.GetAllTenantLocations(terminal.TenantId, true).ToList();

            _mapper.Map(allTenantLocations, result.TenantLocationSync);

            for(var i=0; i< allTenantLocations.Count;i++)
            {
                result.TenantLocationSync[i].TelephoneNumber1 = allTenantLocations[i].ContactNumbers?.HomeNumber;
                result.TenantLocationSync[i].TelephoneNumber2 = allTenantLocations[i].ContactNumbers?.WorkNumber?? allTenantLocations[i].ContactNumbers?.MobileNumber;
                result.TenantLocationSync[i].OpeningHours = TenantLocationServices.GetOpeningTimesByWarehouseId(result.TenantLocationSync[i].WarehouseId);
            }

            result.Count = result.TenantLocationSync?.Count() ?? 0;
            result.TerminalLogId = TerminalServices
                .CreateTerminalLog(reqDate, terminal.TenantId, result.Count, terminal.TerminalId,
                    TerminalLogTypeEnum.TenantLocationsSyncLog).TerminalLogId;
            return Ok(result);
        }

    }
}