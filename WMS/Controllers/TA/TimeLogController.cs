using AutoMapper;
using DevExpress.Web.Mvc;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.UI;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers.TA
{
    public class TimeLogController : BaseController
    {
        readonly IEmployeeShiftsServices _employeeShiftsServices;
        readonly IEmployeeShiftsStoresServices _employeeShiftsStoresServices;
        readonly IEmployeeServices _employeeServices;
        readonly ITenantLocationServices _tenantLocationsServices;
        readonly IShiftScheduleService _shiftScheduleService;
        readonly IActivityServices _activityServices;
        private readonly IMapper _mapper;

        public TimeLogController(IEmployeeShiftsServices employeeShiftsServices, IEmployeeShiftsStoresServices employeeShiftsStoresServices,
            IEmployeeServices employeeServices, ITenantLocationServices tenantLocationsServices, IShiftScheduleService shiftScheduleService, ICoreOrderService orderService, IPropertyService propertyService,
            IAccountServices accountServices, ILookupServices lookupServices, IActivityServices activityServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _employeeShiftsServices = employeeShiftsServices;
            _employeeShiftsStoresServices = employeeShiftsStoresServices;
            _employeeServices = employeeServices;
            _tenantLocationsServices = tenantLocationsServices;
            _shiftScheduleService = shiftScheduleService;
            _activityServices = activityServices;
            _mapper = mapper;
        }

        public ActionResult Tindex(int? id, int? weekNumber, int? YearsList)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();
            caUser user = caCurrent.CurrentUser();

            if (id == null)
            {
                id = CurrentWarehouseId;
            }

            if (weekNumber == null)
            {
                //get current week
                weekNumber = GetWeekNumber();
            }
            if (YearsList == null)
            {
                YearsList = DateTime.UtcNow.Year;
            }

            try
            {
                List<SelectListItem> years = new List<SelectListItem>();

                var currentYear = DateTime.UtcNow.Year;
                for (int j = 10; j >= 0; j--)
                {


                    years.AddRange(new[] { new SelectListItem() { Text = (currentYear - j).ToString(), Value = (currentYear - j).ToString() } });
                }
                ViewData["storesId"] = id;

                ViewBag.StoresList = new SelectList(_activityServices.GetAllPermittedWarehousesForUser(CurrentUserId, CurrentTenantId, user.SuperUser == true, false), "WId", "WName", id);

                ViewData["weekNumber"] = weekNumber;
                ViewData["yearNumber"] = YearsList;


                List<SelectListItem> weeks = GetWeeks(currentYear);


                ViewData["WeekDaysList"] = new SelectList(weeks, "Value", "Text", weekNumber);
                ViewData["YearsList"] = new SelectList(years.OrderByDescending(u => u.Value), "Value", "Text", YearsList);
                ViewBag.yearList = new SelectList(years.OrderByDescending(u => u.Value), "Value", "Text", YearsList);
                return View();
            }
            catch (Exception e)
            {
                //log exception
                var err = e.Message;

                return View("_EmptyResult");
            }
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial(int id, int weekNumber, int YearsList)

        {
            ViewData["weekNumber"] = weekNumber;
            ViewData["storesId"] = id;
            ViewData["yearNumber"] = YearsList;


            var viewModel = GridViewExtension.GetViewModel("gridMaster");

            if (viewModel == null)
                viewModel = TimeLogCustomBinding.TimeLogGridViewModel();

            return TimeLogGridActionCore(viewModel, CurrentTenantId, id, weekNumber, YearsList);

        }
        public ActionResult TimeLogGridActionCore(GridViewModel gridViewModel, int teanantId, int locationId, int weekNumber, int year)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    TimeLogCustomBinding.TimeLogGetDataRowCount(args, teanantId, locationId, weekNumber, year);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        TimeLogCustomBinding.TimeLogGetData(args, teanantId, locationId, weekNumber, year);
                    })
            );
            return PartialView("_GridViewPartial", gridViewModel);
        }

        public ActionResult _TimeLogGridViewsPaging(GridViewPagerState pager)
        {
            int id = 0;
            int weeknumber = 0;
            int year = 0;
            if (Request.Params["stores"] != null)
            {
                id = Convert.ToInt32(Request.Params["stores"]);


            }
            if (Request.Params["weeks"] != null)
            {
                weeknumber = Convert.ToInt32(Request.Params["weeks"]);


            }
            if (Request.Params["years"] != null)
            {
                year = Convert.ToInt32(Request.Params["years"]);


            }

            ViewData["weekNumber"] = weeknumber;
            ViewData["storesId"] = id;
            ViewData["yearNumber"] = year;

            var viewModel = GridViewExtension.GetViewModel("gridMaster");
            viewModel.Pager.Assign(pager);
            return TimeLogGridActionCore(viewModel, CurrentTenantId, id, weeknumber, year);
        }

        public ActionResult _TimeLogGridViewFiltering(GridViewFilteringState filteringState)
        {
            int id = 0;
            int weeknumber = 0;
            int year = 0;
            if (Request.Params["stores"] != null)
            {
                id = Convert.ToInt32(Request.Params["stores"]);


            }
            if (Request.Params["weeks"] != null)
            {
                weeknumber = Convert.ToInt32(Request.Params["weeks"]);


            }
            if (Request.Params["years"] != null)
            {
                year = Convert.ToInt32(Request.Params["years"]);


            }
            ViewData["weekNumber"] = weeknumber;
            ViewData["storesId"] = id;
            ViewData["yearNumber"] = year;
            var viewModel = GridViewExtension.GetViewModel("gridMaster");
            viewModel.ApplyFilteringState(filteringState);
            return TimeLogGridActionCore(viewModel, CurrentTenantId, id, weeknumber, year);
        }
        public ActionResult _TimeLogGridViewDataSorting(GridViewColumnState column, bool reset)
        {
            int id = 0;
            int weeknumber = 0;
            int year = 0;
            if (Request.Params["stores"] != null)
            {
                id = Convert.ToInt32(Request.Params["stores"]);


            }
            if (Request.Params["weeks"] != null)
            {
                weeknumber = Convert.ToInt32(Request.Params["weeks"]);


            }
            if (Request.Params["years"] != null)
            {
                year = Convert.ToInt32(Request.Params["years"]);


            }
            ViewData["weekNumber"] = weeknumber;
            ViewData["storesId"] = id;
            ViewData["yearNumber"] = year;
            var viewModel = GridViewExtension.GetViewModel("gridMaster");
            viewModel.ApplySortingState(column, reset);
            return TimeLogGridActionCore(viewModel, CurrentTenantId, id, weeknumber, year);
        }

        public ActionResult GridDetailsViewPartial(int employeeId, int weekNumber, int storesId, int years)
        {
            try
            {
                var model = new List<TimeLogsViewModel>();
                ViewData["weekNumber"] = weekNumber;
                ViewData["EmployeeId"] = employeeId;
                ViewData["yearNumber"] = years;

                List<DateTime> weekDates = GetWeekDatesList(weekNumber, years);

                model = TimeLogDataSource(employeeId, weekNumber, storesId, years, weekDates);

                return PartialView("_GridDetailsViewPartial", model);
            }
            catch (Exception e)
            {
                //log exception
                var err = e.Message;

                return View("_ErrorResult");
            }
        }

        public ActionResult TimeLogReport(int weekNumber, int storesId, int years)
        {
            try
            {
                TimeLogReport report = new TimeLogReport();
                var employeesTimeLogs = new List<EmployeesTimeLogsViewModel>();
                var placeHolder = " ( " + GetDateFromWeekNumberAndDayOfWeek(weekNumber, years, 1).ToString("dd/MM") + " to " + GetDateFromWeekNumberAndDayOfWeek(weekNumber, years, 0).ToString("dd/MM") + " )";

                ViewData["weekNumber"] = weekNumber;
                ViewData["storesId"] = storesId;
                ViewData["yearNumber"] = years;

                List<DateTime> weekDates = GetWeekDatesList(weekNumber, years);

                //get lists of employees by storesId
                var employeeLists = _employeeServices.GetAllEmployeesByLocation(CurrentTenantId, storesId).ToList();
                var locationName = _tenantLocationsServices.GetTenantLocationById(storesId).WarehouseName;

                //CreateReportHeader(report, locationName);
                report.FindControl("xrLabel4", true).Text = $"{locationName}";
                report.FindControl("xrLabel1", true).Text = $"Week {weekNumber} {placeHolder}";
                report.FindControl("xrLabel3", true).Text = $"Time and Attendance @{DateTime.UtcNow.Year}";
                report.FindControl("xrLabel7", true).Text = $"{DateTime.UtcNow.Date.ToString("dd/MM/yyyy")}";


                if (employeeLists.Count() >= 1)
                {
                    //get by employeeId and weeknumber
                    foreach (var item in employeeLists)
                    {
                        employeesTimeLogs.Add(new EmployeesTimeLogsViewModel()
                        {
                            EmployeeId = item.ResourceId,
                            PayrollEmployeeNo = item.PayrollEmployeeNo,
                            FirstName = item.FirstName,
                            SurName = item.SurName,
                            FullName = item.Name,
                            WeekNumber = weekNumber,
                            EmployeeRole = item.EmployeeRoles.Count() <= 0 ? "" : item.EmployeeRoles.Where(x => x.IsDeleted != true).FirstOrDefault().Roles.RoleName,
                            TimeLogs = TimeLogDataSource(item.ResourceId, weekNumber, storesId, years, weekDates)
                        });
                    }

                    report.Status.EvaluateBinding += Status_EvaluateBinding;

                    report.DataSource = employeesTimeLogs;
                }

                return View(report);

            }
            catch (Exception e)
            {
                //log exception
                var err = e.Message;

                return View("_EmptyResult");
            }
        }

        public List<TimeLogsViewModel> TimeLogDataSource(int employeeId, int weekNumber, int storesId, int years, List<DateTime> weekDates)
        {
            var model = new List<TimeLogsViewModel>();




            foreach (var date in weekDates) //loop through days of week.
            {
                bool hasValue = false;
                var timeIn = new DateTime?();
                var timeOut = new DateTime?();
                var status = "";
                List<ResourceShifts> allStamps = new List<ResourceShifts>();
                TimeSpan totalTime = new TimeSpan();
                TimeSpan totalBreaksTaken = new TimeSpan();
                var empShifts = _employeeShiftsServices.GetByEmployeeAndWeekAndStore(employeeId, date, storesId).OrderBy(s => s.TimeStamp).ToList();
                if (empShifts != null)
                {
                    var firstInStamp = empShifts.Where(x => x.TimeStamp.Date == date.Date && x.StatusType == "In").FirstOrDefault();

                    if (firstInStamp != null)
                    {
                        timeIn = firstInStamp.TimeStamp;
                        var lastOutStamp = empShifts.Where(x => x.TimeStamp > firstInStamp.TimeStamp && x.TimeStamp <= firstInStamp.TimeStamp.AddHours(16) && x.StatusType == "Out").LastOrDefault();

                        if (lastOutStamp != null)
                        {
                            timeOut = lastOutStamp.TimeStamp;
                            allStamps = empShifts.Where(x => x.TimeStamp >= firstInStamp.TimeStamp && x.TimeStamp <= lastOutStamp.TimeStamp).ToList();
                        }

                        string statusColor = String.Empty;

                        if (lastOutStamp != null && allStamps.Count() <= 2)
                        {
                            totalTime = lastOutStamp.TimeStamp - firstInStamp.TimeStamp;
                        }
                        else if (allStamps.Count() > 2)
                        {
                            var allInStamps = allStamps.Where(x => x.StatusType == "In").ToList();
                            var allOutStamps = allStamps.Where(x => x.StatusType == "Out").ToList();

                            foreach (var stamp in allInStamps)
                            {
                                try
                                {
                                    int index = allInStamps.IndexOf(stamp);
                                    if (index < allInStamps.Count() && index < allOutStamps.Count())
                                    {
                                        totalTime += allOutStamps[index].TimeStamp - allInStamps[index].TimeStamp;
                                    }

                                }
                                catch
                                {
                                    break;
                                }
                            }

                            // calculate total breaks
                            totalBreaksTaken = (lastOutStamp.TimeStamp - firstInStamp.TimeStamp) - totalTime;
                        }


                        //get shifts info
                        var shiftsInfo = _shiftScheduleService.GetShiftSchedule(employeeId, date, CurrentTenantId);
                        var employeeInfo = _employeeServices.GetByEmployeeId(employeeId);
                        double totalSalary = 0f;
                        TimeSpan? expectedHours = TimeSpan.ParseExact("00:00", @"hh\:mm", CultureInfo.InvariantCulture);
                        double? hourlyRate = 0f;
                        TimeSpan? timeBreaks = TimeSpan.ParseExact("00:00", @"hh\:mm", CultureInfo.InvariantCulture);

                        //TODO: refactor this IF's?
                        if (shiftsInfo == null)
                        {
                            expectedHours = TimeSpan.ParseExact("00:00", @"hh\:mm", CultureInfo.InvariantCulture);
                            hourlyRate = 0f;
                        }
                        else
                        {
                            expectedHours = shiftsInfo.ExpectedHours;
                            timeBreaks = shiftsInfo.TimeBreaks;
                        }

                        if (employeeInfo != null)
                            hourlyRate = (double?)employeeInfo.HourlyRate;

                        if (expectedHours == TimeSpan.ParseExact("00:00", @"hh\:mm", CultureInfo.InvariantCulture))
                        {
                            status = "";
                        }
                        else
                        {
                            if (totalTime.Equals(expectedHours.Value.Hours))
                            {
                                status = "GOOD";
                            }
                            if (totalTime > expectedHours.Value)
                            {
                                status = "OVERTIME";
                            }
                            if (totalTime < expectedHours.Value)
                            {
                                status = "SHORT";
                            }
                        }


                        //calculate TotalSalary (# hours x hourly rate)
                        if (hourlyRate > 0)
                        {
                            var hours = totalTime.TotalHours;
                            totalSalary = ((double)(hours * hourlyRate));
                        }

                        hasValue = true;

                        model.Add(new TimeLogsViewModel()
                        {
                            TotalHours = totalTime.TotalHours.ToString("N2"),
                            TimeIn = timeIn,
                            TimeOut = timeOut,
                            WeekDay = date.DayOfWeek.ToString(),
                            ExpectedHours = (decimal?)(expectedHours.HasValue ? expectedHours.Value.Hours : 0f),
                            ExpectedHoursString = (expectedHours.HasValue ? $"{expectedHours.Value.Hours}:{expectedHours.Value.Minutes}" : String.Empty),
                            TotalSalary = totalSalary.ToString("N2"),
                            Breaks = timeBreaks,
                            BreaksTaken = totalBreaksTaken,
                            Status = status,
                            WeekNumber = weekNumber,

                            Employees = _mapper.Map(empShifts.FirstOrDefault().Resources, new ResourcesViewModel())
                        });

                    }
                }
                if (!hasValue)
                {

                    model.Add(new TimeLogsViewModel()
                    {
                        TotalHours = "0",
                        TotalSalary = "0.00",
                        WeekDay = date.DayOfWeek.ToString(),
                        ExpectedHours = null,
                        Breaks = null,
                        WeekNumber = weekNumber,
                        Status = "",
                        Employees = null
                    });
                }
            }

            return model;
        }

        #region Devexpress Report

        private void DateTimeFormatting_EvaluateBinding(object sender, BindingEventArgs e)
        {
            XRTableCell cell = (sender as XRTableCell);

            if (e.Value != null && !String.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                DateTime? value = Convert.ToDateTime(e.Value);
                //value = DateTimeToLocal.Convert(value, GetCurrentTimeZone());

                string formattedDate = value.Value.ToString("dd/MM/yyyy HH:mm");

                e.Value = formattedDate;
            }
        }

        private void Status_EvaluateBinding(object sender, BindingEventArgs e)
        {
            var cell = sender as XRLabel;

            cell.ForeColor = Color.Black; //default

            if (e.Value != null && !String.IsNullOrWhiteSpace(e.Value.ToString()))
            {
                switch (e.Value.ToString().ToLower())
                {
                    case "good":
                        cell.ForeColor = Color.Green;

                        break;
                    case "overtime":
                        cell.ForeColor = Color.Purple;

                        break;

                    case "short":
                        cell.ForeColor = Color.Red;

                        break;

                    default:
                        cell.ForeColor = Color.Black;

                        break;
                }
            }
        }

        private void AdjustTableWidth(XRTable table)
        {
            XtraReport report = table.RootReport;
            table.WidthF = report.PageWidth - report.Margins.Left - report.Margins.Right;
        }

        void tableHeader_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            AdjustTableWidth(sender as XRTable);
        }

        void tableDetail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            AdjustTableWidth(sender as XRTable);
        }

        private XRLabel CreateBoundLabel(string dataMember, Color backColor, int offset)
        {
            XRLabel label = new XRLabel();

            label.DataBindings.Add(new XRBinding("Text", null, dataMember));
            label.BackColor = backColor;
            label.Location = new Point(offset, 0);

            return label;
        }

        private void CreateReportHeader(XtraReport report, string caption)
        {
            // Create a report title.
            XRLabel label = new XRLabel();
            label.Font = new Font("Tahoma", 12, System.Drawing.FontStyle.Bold);
            label.Text = caption;
            label.WidthF = report.PageWidth - report.Margins.Left - report.Margins.Right;

            // Create a report header and add the title to it.
            ReportHeaderBand reportHeader = new ReportHeaderBand();

            report.Bands.Add(reportHeader);
            reportHeader.Controls.Add(label);
            reportHeader.HeightF = label.HeightF;
        }

        public void SetFunction(XRLabel label)
        {
            // Create an XRSummary object.
            XRSummary summary = new XRSummary();

            // Set a function which should be calculated.
            summary.Func = SummaryFunc.Avg;

            // Set a range for which the function should be calculated.
            summary.Running = SummaryRunning.Group;

            // Set the "ingore null values" option.
            summary.IgnoreNullValues = true;

            // Set the output string format.
            summary.FormatString = "{0:c2}";

            // Make the label calculate the specified function for the
            // value specified by its DataBindings.Text property.
            label.Summary = summary;
        }


        public JsonResult GetWeeksforYear(int year)
        {
            List<SelectListItem> Weeks = GetWeeks(year);
            return Json(Weeks,JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}