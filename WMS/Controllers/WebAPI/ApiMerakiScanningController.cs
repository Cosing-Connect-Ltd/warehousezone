using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.Models;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace WMS.Controllers
{
    public class ApiMerakiScanningController : BaseApiController
    {
        private readonly IActivityServices _activityServices;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly IAssetServices _assetServices;
        private readonly IMapper _mapper;

        public ApiMerakiScanningController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IActivityServices activityServices, IAssetServices assetServices, IMapper mapper)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _activityServices = activityServices;
            _tenantLocationServices = tenantLocationServices;
            _assetServices = assetServices;
            _mapper = mapper;
        }

        // GET api/<controller>
        public HttpResponseMessage Get()
        {
            var SecretKey = ConfigurationManager.AppSettings["MerakiSecretKey"];

            return new HttpResponseMessage()
            {
                Content = new StringContent(
            SecretKey,
            Encoding.UTF8,
            "text/html"
        )
            };
        }

        // POST api/<controller>
        public IHttpActionResult Post(MerakiScanningApiViewModel apiData)
        {
            var serialNo = apiData.Data.ApMac;
            var results = new List<int>();


            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            foreach (var item in apiData.Data.Observations)
            {
                AssetLogViewModel assetLog = new AssetLogViewModel();
                _mapper.Map(item, assetLog);
                int res = TerminalServices.SaveAssetLog(assetLog, terminal.TerminalId, terminal.TenantId);
                results.Add(res);
            }

            if (results.Any(x => x <= 0))
            {
                return BadRequest("Unable to save records");
            }
            else
            {
                return Ok();
            }
        }
    }
}