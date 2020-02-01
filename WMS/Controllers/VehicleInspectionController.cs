using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class VehicleInspectionController : BaseController
    {
        private readonly IVehicleInspectionService _inspectionService;
        private readonly IMarketServices _marketServices;
        private readonly IMapper _mapper;

        public VehicleInspectionController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IVehicleInspectionService inspectionService, 
            IMarketServices marketServices, IMapper mapper) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _inspectionService = inspectionService;
            _marketServices = marketServices;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new InspectionsViewModel()
            {
              AllInspections = _inspectionService.GetAllVehicleInspections(CurrentTenantId)
            };

            return View(model);
        }

        public ActionResult InspectionsListPartial()
        {
            var model = new InspectionsViewModel()
            {
                AllInspections = _inspectionService.GetAllVehicleInspections(CurrentTenantId)
            };
            return PartialView("_GridPartial", model);
        }
        public ActionResult Edit(int? id, int vehicleId=0)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new VehicleInspectionViewModel { MarketVehicleId = vehicleId };
            if (id.HasValue && id > 0)
            {
                model = _inspectionService.GetVehicleInspectionById(id.Value);
                if (model.Id < 1) return HttpNotFound();
            }
            model.CheckList = _inspectionService.GetAllInspectionCheckLists(CurrentTenantId);
            model.AllEmployees = _inspectionService.GetAllVehicleDrivers(CurrentTenantId);
            model.AllVehicles = _marketServices.GetAllValidMarketVehicles(CurrentTenantId).MarketVehicles;
           
            return View("_CreateEdit", model);
        }

        [HttpPost]
        public ActionResult SaveInspection(VehicleInspectionViewModel model, FormCollection form)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var tickedInspections = form.AllKeys.Where(m=> m.StartsWith("Inspection_") && !string.IsNullOrEmpty(form[m]) && form[m].Contains("true")).Select(x=> int.Parse(x.Replace("Inspection_",string.Empty))).ToList();

            model.TenantId = CurrentTenantId;
            var inspection = _mapper.Map(model, new VehicleInspection());
            _inspectionService.SaveInspection(inspection, CurrentUserId, tickedInspections);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {

            try
            {
                _inspectionService.DeleteInspection(id, CurrentUserId);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.InnerException });
            }
        }

    }
}