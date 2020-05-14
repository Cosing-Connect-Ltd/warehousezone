using AutoMapper;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class TooltipsController : BaseController
    {
        private readonly ITooltipServices _tooltipServices;
        private readonly IMapper _mapper;

        public TooltipsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITooltipServices tooltipServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tooltipServices = tooltipServices;
            _mapper = mapper;
        }

        [OutputCache(Duration = 86400, VaryByParam = "*")]
        public async Task<JsonResult> GetTooltipsDetailByKey(string[] keys)
        {
            var tooltips = await _tooltipServices.GetTooltipsDetailByKey(keys, CurrentTenantId);

            return Json(_mapper.Map<List<TooltipViewModel>>(tooltips), JsonRequestBehavior.AllowGet);

        }

    }
}