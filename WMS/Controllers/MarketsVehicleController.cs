using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class MarketsVehicleController : BaseController
    {
        private readonly IAccountServices _accountServices;
        private readonly IMarketServices _marketServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly IMapper _mapper;

        public MarketsVehicleController(ICoreOrderService orderService,
            IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IMarketServices marketServices, IEmployeeServices employeeServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _accountServices = accountServices;
            _marketServices = marketServices;
            _employeeServices = employeeServices;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            var results = _marketServices.GetAllValidMarketVehicles(CurrentTenantId);

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View(results);
        }

        public ActionResult MarketsVehicleListPartial()
        {
            var result = _marketServices.GetAllValidMarketVehicles(CurrentTenantId);
            return PartialView("_GridPartial", result);
        }

        public ActionResult Edit(int? id)
        {
            var model = new MarketVehicleViewModel();
            if (id.HasValue && id > 0)
            {
                model = _marketServices.GetMarketVehicleById(id.Value);
            }
            return View("_CreateEdit", model);
        }

        [HttpPost]
        public ActionResult SaveVehicle(MarketVehicleViewModel model)
        {
            model.TenantId = CurrentTenantId;
            _marketServices.SaveMarketVehicle(_mapper.Map(model, new MarketVehicle()), CurrentUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult DeleteVehicle(int id)
        {
            _marketServices.DeleteMarketVehicle(id, CurrentUserId);
            return Json(new { success = true });
        }
    }
}