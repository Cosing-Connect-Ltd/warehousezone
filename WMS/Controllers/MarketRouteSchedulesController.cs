using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class MarketRouteSchedulesController : BaseController
    {
        private readonly IMarketServices _marketServices;
        private readonly IMarketRouteScheduleService _marketRouteScheduleService;

        public MarketRouteSchedulesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices,
            IMarketServices marketServices, IMarketRouteScheduleService marketRouteScheduleService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _marketServices = marketServices;
            _marketRouteScheduleService = marketRouteScheduleService;
        }
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public ActionResult SchedulerPartial()
        {
            return PartialView("_SchedulerPartial", MarketRouteSchedulerSettings.DataObject);
        }

        public ActionResult SchedulerPartialEditAppointment()
        {
            try
            {
                MarketRouteSchedulerSettings.UpdateEditableDataObject();
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }

            return PartialView("_SchedulerPartial", MarketRouteSchedulerSettings.DataObject);
        }

        public ActionResult _ListMarketRoutes()
        {
            var result = _marketServices.GetAllValidMarketRoutes(CurrentTenantId).MarketRouteViewModel;
            return PartialView(result);
        }


        public ActionResult CreateAppointment(string start, string end, int mobileLocationId, int routeId, int tenantId)
        {
            var result = _marketRouteScheduleService.CreateMarketRouteSchedule(start, end, routeId, mobileLocationId, tenantId);
            return PartialView("_SchedulerPartial", MarketRouteSchedulerSettings.DataObject);
        }
    }
}