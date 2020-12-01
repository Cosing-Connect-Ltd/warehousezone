using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Web.Http;

namespace WMS.Controllers.WebAPI
{
    public class ApiAssetController : BaseApiController
    {
        private readonly IActivityServices _activityServices;
        private readonly ITenantLocationServices _tenantLocationServices;

        public ApiAssetController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IActivityServices activityServices)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _activityServices = activityServices;
            _tenantLocationServices = tenantLocationServices;
        }


        [HttpPost]
        public IHttpActionResult PostAssetLog(AssetLogViewModel assetLog)
        {
            string serialNo = assetLog.piAddress.Trim().ToLower();

            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            int res = TerminalServices.SaveAssetLog(assetLog, terminal.TerminalId, terminal.TenantId);

            if (res > 0)
            {
                return Ok("Success");
            }
            else
            {
                return BadRequest("Unable to save records");
            }
        }
    }
}