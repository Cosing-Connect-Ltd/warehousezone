using DevExpress.Web.Mvc;
using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WMS.Controllers.TA
{
    public class ShiftsController : BaseController
    {
        readonly IShiftsServices _shiftsServices;
        readonly ITenantLocationServices _storesServices;
        readonly IEmployeeServices _employeeServices;
        private readonly IMapper _mapper;

        public ShiftsController(IShiftsServices shiftsServices, ITenantLocationServices storesServices, ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices,
            ILookupServices lookupServices, IEmployeeServices employeeServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _shiftsServices = shiftsServices;
            _storesServices = storesServices;
            _employeeServices = employeeServices;
            _mapper = mapper;
        }

        public ActionResult Index(string message = "")
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            try
            {
                if (!String.IsNullOrWhiteSpace(message))
                    ViewBag.Message = message;

                return View("_Listing");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("_EmptyResult");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Employee Id</param>
        /// <returns></returns>
        public ActionResult Add()
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);


            try
            {
                ViewBag.StoresList = new SelectList(_storesServices.GetAllTenantLocations(CurrentTenantId), "WarehouseId", "WarehouseName");
                ViewBag.EmployeeId = new SelectList(_employeeServices.GetAllEmployeesByLocation(CurrentTenantId, CurrentWarehouseId), "ResourceId", "Name");

                List<SelectListItem> weeks = new List<SelectListItem>();
                for (int i = 1; i < 53; i++) //loop through weeks. 
                {
                    weeks.AddRange(new[] {
                    new SelectListItem() { Text = "Week " + i,  Value = i.ToString()}
                });
                }

                ViewData["WeekDaysList"] = new SelectList(weeks, "Value", "Text", GetWeekNumber());

                return View("_CreateEdit", new ShiftsViewModel()
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    ExpectedHours = TimeSpan.ParseExact("00:00", @"hh\:mm", System.Globalization.CultureInfo.InvariantCulture)
                }
                );
            }
            catch (Exception e)
            {
                //log exception
                var err = e.Message;

                return View("_EmptyResult");
            }
        }
        /// <summary>
        /// Get Shifts Details
        /// </summary>
        /// <param name="shiftsId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ActionResult Details(int id, string message = "")
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);


            try
            {
                if (!String.IsNullOrWhiteSpace(message))
                    ViewBag.Message = message;

                var result = _shiftsServices.GetByShiftsId(id);

                ViewBag.StoresList = new SelectList(_storesServices.GetAllTenantLocations(CurrentTenantId), "WarehouseId", "WarehouseName", result.LocationsId);
                ViewBag.EmployeeId = new SelectList(_employeeServices.GetAllEmployeesByLocation(CurrentTenantId, CurrentWarehouseId), "ResourceId", "Name");

                List<SelectListItem> weeks = new List<SelectListItem>();
                for (int i = 1; i < 53; i++) //loop through weeks. 
                {
                    weeks.AddRange(new[] {
                        new SelectListItem() { Text = "Week " + i,  Value = i.ToString()}
                    });
                }

                ViewData["WeekDaysList"] = new SelectList(weeks, "Value", "Text", (result.WeekNumber != null ? result.WeekNumber : GetWeekNumber()));

                return View("_CreateEdit", _mapper.Map(result, new ShiftsViewModel()));
            }
            catch (Exception e)
            {
                //log exception
                var err = e.Message;

                return View("_EmptyResult");
            }
        }
        /// <summary>
        /// Submit Shifts Request
        /// </summary>
        /// <param name="shifts"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Submit(ShiftsViewModel shifts)
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            try
            {
                if (ModelState.IsValid)
                {
                    var employeeId = shifts.EmployeeId;

                    foreach (var itemWeekDay in shifts.RepeatShifts)
                    {
                        if (itemWeekDay.HasValue)
                        {
                            var weekNumber = (int)shifts.WeekDaysList.FirstOrDefault().Value;
                            var weekDay = itemWeekDay.Value;
                            int year = DateTime.UtcNow.Year;
                            var date = GetDateFromWeekNumberAndDayOfWeek(weekNumber, year, weekDay);
                            var dateValue = GetDateFromWeekNumberAndDayOfWeek(weekNumber, year, weekDay).ToShortDateString();
                            var startTime = DateTime.Parse(dateValue + ' ' + shifts.StartTime.Value.ToString("HH:mm"));
                            var endTime = DateTime.Parse(dateValue + ' ' + shifts.EndTime.Value.ToString("HH:mm"));
                            var shiftsInfo = _shiftsServices.GetShiftsByEmployeeIdAndWeekDayAndWeekNumber(employeeId, date, weekNumber).ToList();

                            //delete first the shift
                            foreach (var itemShifts in shiftsInfo)
                                _shiftsServices.Delete(itemShifts);

                            //insert next
                            var newShift = new Shifts
                            {
                                EmployeeId = employeeId,
                                WeekNumber = weekNumber,
                                WeekDay = weekDay,
                                ExpectedHours = shifts.ExpectedHours,
                                TimeBreaks = TimeSpan.Parse(shifts.TimeBreaks),
                                LocationsId = CurrentWarehouseId,
                                Date = DateTime.Parse(dateValue),
                                StartTime = startTime,
                                EndTime = endTime,
                                TenantId = CurrentTenantId,
                                DateCreated = DateTime.UtcNow,
                                DateUpdated = DateTime.UtcNow,
                                CreatedBy = CurrentUserId,
                                UpdatedBy = CurrentUserId
                            };

                            _shiftsServices.Insert(newShift);
                        }
                    }

                    if (shifts.Id <= 0)
                    {
                        //insert                   
                        ViewBag.Message = $"Successfully Added on {DateTime.Now}.";
                    }
                    else
                    {
                        //update
                        ViewBag.Message = $"Successfully Updated on {DateTime.Now}.";
                    }

                    //redirect to Index to show all shifts for this employeeId
                    return RedirectToAction("Index", new { id = shifts.EmployeeId, message = ViewBag.Message });
                }
                else //ModelState.IsValid is not valid
                {
                    ViewBag.StoresList = new SelectList(_storesServices.GetAllTenantLocations(CurrentTenantId), "LocationId", "WarehouseName", 0);
                    ViewBag.EmployeeId = new SelectList(_employeeServices.GetAllEmployeesByLocation(CurrentTenantId, CurrentWarehouseId), "ResourceId", "Name");

                    List<SelectListItem> weeks = new List<SelectListItem>();
                    for (int i = 1; i < 53; i++) //loop through weeks. 
                    {
                        weeks.AddRange(new[] {
                        new SelectListItem() { Text = "Week " + i,  Value = i.ToString()}
                    });
                    }

                    ViewData["WeekDaysList"] = new SelectList(weeks, "Value", "Text", GetWeekNumber());

                    return View("_CreateEdit", _mapper.Map(_shiftsServices.GetByShiftsId(shifts.Id), new ShiftsViewModel()));
                }
            }
            catch (Exception e)
            {
                //log exception
                var err = e.Message;

                return View("_ErrorResult");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Delete(int Id)
        {
            if (!caSession.AuthoriseSession()) return Json(new { success = false });

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                _shiftsServices.Delete(_mapper.Map<Shifts>(new ShiftsViewModel { Id = Id }));
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                //catch error here
                var err = e.Message;

            }
            return Json(new { success = false });
        }


        [ValidateInput(false)]
        public ActionResult ShiftsGridViewPartial()
        {
            var model = _mapper.Map(_shiftsServices.GetAllEmployeeShifts(CurrentTenantId), new List<ShiftsViewModel>());
            return PartialView("~/Views/Shifts/_ShiftsGridViewPartial.cshtml", model);
        }
    }
}