using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DevExpress.Web.Mvc;
using WMS.CustomBindings;
using Ganedata.Core.Entities.Domain;

namespace WMS.Controllers.TA
{
    public class EmployeeShiftsController : BaseController
    {
        private readonly IEmployeeShiftsServices _employeeShiftsServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly ITerminalServices _terminalServices;
        private readonly IMapper _mapper;

        public EmployeeShiftsController(IEmployeeShiftsServices employeeShiftsServices, ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, 
            IEmployeeServices employeeServices, ITerminalServices terminalServices, IMapper mapper) 
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _employeeShiftsServices = employeeShiftsServices;
            _employeeServices = employeeServices;
            _terminalServices = terminalServices;
            _mapper = mapper;

        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);
            return View("_EmployeeShiftListing");
        }

        public ActionResult Create()
        {
            int shiftId = Convert.ToInt16(!string.IsNullOrEmpty(Request.Params["ShiftId"]) ? Request.Params["ShiftId"].ToString() : "0");
            var emplyeeShift = _employeeShiftsServices.GetResourceShifts(shiftId);

            ViewBag.ResourceIds = new SelectList(_employeeServices.GetAllEmployeesByLocation(CurrentTenantId, CurrentWarehouseId), "ResourceId", "FirstName");

            ViewBag.TerminalId = new SelectList(_terminalServices.GetAllTerminalsForGrid(CurrentTenantId, CurrentWarehouseId), "TerminalId", "TerminalName");
            return PartialView("_Create", emplyeeShift);

        }


        [HttpPost]
        public JsonResult CreateResourceShift(ResourceShifts resourceShifts)
        {
            if (resourceShifts.Id > 0)
            {
                resourceShifts.TenantId = CurrentTenantId;
                resourceShifts.DateUpdated = DateTime.UtcNow;
                resourceShifts.Date = resourceShifts.TimeStamp.Date;
                resourceShifts.UpdatedBy = CurrentUserId;
                var weeknumbers = GetWeekNumber(resourceShifts.TimeStamp);
                resourceShifts.WeekNumber = weeknumbers;
                _employeeShiftsServices.Update(resourceShifts);

                return Json(true, JsonRequestBehavior.AllowGet);

            }

            resourceShifts.TenantId = CurrentTenantId;
            resourceShifts.DateCreated = DateTime.UtcNow;
            var weeknumber = GetWeekNumber(resourceShifts.TimeStamp);
            resourceShifts.WeekNumber = weeknumber;
            resourceShifts.Date = resourceShifts.TimeStamp.Date;
            resourceShifts.CreatedBy = CurrentUserId;
            _employeeShiftsServices.Insert(resourceShifts);

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        public JsonResult deleteResourceShift(int? id)
        {
            ResourceShifts resourceShifts = _employeeShiftsServices.GetResourceShifts(id);
            resourceShifts.TenantId = CurrentTenantId;
            resourceShifts.DateUpdated = DateTime.UtcNow;
            resourceShifts.UpdatedBy = CurrentUserId;
            resourceShifts.IsDeleted = true;
            _employeeShiftsServices.Update(resourceShifts);

            return Json(true, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Search EmployeeShift by Name
        /// </summary>
        /// <param name="search"></param>
        /// <returns>Lists of Employees contains in search input</returns>
        [HttpPost]
        public ActionResult SearchByEmployee(string search)
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                var result = _employeeShiftsServices.SearchByEmployee(search, tenant.TenantId);

                if (search.Count() <= 0 || result == null || result.Count() <= 0)
                    return View("_EmptyResult");

                return View("_EmployeeShiftListing", _mapper.Map<List<EmployeeShiftsViewModel>>(result));
            }
            catch (Exception e)
            {
                //log exception
                var err = e.Message;

                return View("_ErrorResult");
            }
        }

        /// <summary>
        /// Search EmployeeShifts by Date
        /// </summary>
        /// <param name="searchDate"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchByDate(string searchDate)
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                var result = _employeeShiftsServices.SearchByDate(Convert.ToDateTime(searchDate), tenant.TenantId, CurrentWarehouseId).ToList();

                return PartialView("_EmployeeShiftListing", _mapper.Map<List<EmployeeShiftsViewModel>>(result));
            }
            catch (Exception e)
            {
                //log exception
                var err = e.Message;

                return View("_ErrorResult");
            }
        }

        public ActionResult EmployeesShiftsDetails(int employeeShiftId)
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            return View();
        }

        [ValidateInput(false)]
        public ActionResult EmployeesShiftsGridview(string searchDate)

        {
            DateTime? searchDates = null;
            DateTime paramDate;
            if (DateTime.TryParse(searchDate, out paramDate))
            {
                searchDates = paramDate;
            }

            var viewModel = GridViewExtension.GetViewModel("EmployeeShiftsGridview");

            if (viewModel == null)
                viewModel = EmployeeShiftsCustomBinding.CreateEmployeeShiftsGridViewModel();

            return EmployeesShiftsGridActionCore(viewModel, searchDates, CurrentTenantId, CurrentWarehouseId);

        }
        public ActionResult EmployeesShiftsGridActionCore(GridViewModel gridViewModel, DateTime? searchdate, int teanantId, int warehouseId)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    EmployeeShiftsCustomBinding.EmployeeShiftsGetDataRowCount(args, searchdate, teanantId, warehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        EmployeeShiftsCustomBinding.EmployeeShiftsGetData(args, searchdate, teanantId, warehouseId);
                    })
            );
            return PartialView("_EmployeesShiftsGridview", gridViewModel);
        }

        public ActionResult _EmployeesShiftsGridViewsPaging(GridViewPagerState pager)
        {
            DateTime? searchDate = null;
            DateTime paramDate;
            if (DateTime.TryParse(Request.Params["searchDate"], out paramDate))
            {
                searchDate = paramDate;
            }

            var viewModel = GridViewExtension.GetViewModel("EmployeeShiftsGridview");
            viewModel.Pager.Assign(pager);
            return EmployeesShiftsGridActionCore(viewModel, searchDate, CurrentTenantId, CurrentWarehouseId);
        }

        public ActionResult _EmployeesShiftsGridViewFiltering(GridViewFilteringState filteringState)
        {
            DateTime? searchDate = null;
            DateTime paramDate;
            if (DateTime.TryParse(Request.Params["searchDate"], out paramDate))
            {
                searchDate = paramDate;
            }

            var viewModel = GridViewExtension.GetViewModel("EmployeeShiftsGridview");
            viewModel.ApplyFilteringState(filteringState);
            return EmployeesShiftsGridActionCore(viewModel, searchDate, CurrentTenantId, CurrentWarehouseId);
        }
        public ActionResult _EmployeesShiftsGridViewDataSorting(GridViewColumnState column, bool reset)
        {
            DateTime? searchDate = null;
            DateTime paramDate;
            if (DateTime.TryParse(Request.Params["searchDate"], out paramDate))
            {
                searchDate = paramDate;
            }

            var viewModel = GridViewExtension.GetViewModel("EmployeeShiftsGridview");
            viewModel.ApplySortingState(column, reset);
            return EmployeesShiftsGridActionCore(viewModel, searchDate, CurrentTenantId, CurrentWarehouseId);
        }
    }
}