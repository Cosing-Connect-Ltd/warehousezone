using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiProductSerialSyncController : BaseApiController
    {
        private readonly IMapper _mapper;

        public ApiProductSerialSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IMapper mapper) 
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _mapper = mapper;
        }

        // GET http://localhost:8005/api/sync/productserials/{reqDate}/{serialNo}
        // GET http://localhost:8005/api/sync/productserials/2014-11-23/920013c000814
        public IHttpActionResult GetSerials(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new ProductSerialSyncCollection();

            var allSerials = ProductServices.GetAllProductSerialsByTenantId(terminal.TenantId, reqDate);
             
            var serials = new List<ProductSerialSync>();

            foreach (var p in allSerials)
            {
                var serial = new ProductSerialSync();
                var mappedSerial = _mapper.Map(p, serial);
                serials.Add(mappedSerial);
            }

            result.Count = serials.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, serials.Count, terminal.TerminalId, TerminalLogTypeEnum.ProductSerialSync).TerminalLogId;
            result.ProductSerials = serials;
            return Ok(result);
        }
    }
}