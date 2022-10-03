using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using WMS.Reports;
using WMS.Reports.Designs;

namespace WMS.Controllers
{
    public class ReportsController : BaseReportsController
    {
        private readonly ITenantLocationServices _tenantLocationsServices;
        private readonly IShiftScheduleService _shiftScheduleService;
        private readonly IEmployeeShiftsServices _employeeShiftsServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly IProductServices _productServices;
        private readonly IAccountServices _accountServices;
        private readonly IMarketServices _marketServices;
        private readonly IAccountSectorService _accountSectorServices;
        private readonly IPalletingService _palletingService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupService;
        private readonly IUserService _userService;
        private readonly IProductPriceService _productPriceService;

        public ReportsController(ITenantLocationServices tenantLocationsServices, IShiftScheduleService shiftScheduleService, IEmployeeShiftsServices employeeShiftsServices, IEmployeeServices employeeServices, ICoreOrderService orderService,
            IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IAppointmentsService appointmentsService, IGaneConfigurationsHelper ganeConfigurationsHelper, IEmailServices emailServices, IInvoiceService invoiceService,
            ITenantLocationServices tenantLocationservices, IProductServices productServices, ITenantsServices tenantsServices, IPalletingService palleteServices, IMarketServices marketServices, IUserService userService,
            IAccountSectorService accountSectorService, IProductPriceService productPriceService)
            : base(orderService, propertyService, accountServices, lookupServices, appointmentsService, ganeConfigurationsHelper, emailServices, tenantLocationservices, tenantsServices)
        {
            _tenantLocationsServices = tenantLocationsServices;
            _shiftScheduleService = shiftScheduleService;
            _employeeShiftsServices = employeeShiftsServices;
            _employeeServices = employeeServices;
            _productServices = productServices;
            _accountServices = accountServices;
            _palletingService = palleteServices;
            _marketServices = marketServices;
            _accountSectorServices = accountSectorService;
            _invoiceService = invoiceService;
            _lookupService = lookupServices;
            _userService = userService;
            _productPriceService = productPriceService;
        }

        public float JobProgressOutSum = 0;
        public float JobProgressReturnSum = 0;
        private List<ExpensivePropertiseTotalsViewModel> ExpensivePropertyDetailTotals = new List<ExpensivePropertiseTotalsViewModel>();

        #region Inventory

        public ActionResult Inventory()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            InventoryReport report = CreateInventoryReport();
            report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        private void XrLabel18_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel cell = sender as XRLabel;
            DateTime? utc = (DateTime?)cell.Value;
            cell.Text = DateTimeToLocal.Convert(utc, GetCurrentTimeZone()).ToString();
        }

        public InventoryReport CreateInventoryReport()
        {
            InventoryReport InventoryReport = new InventoryReport();
            InventoryReport.paramTenantId.Value = CurrentTenantId;
            InventoryReport.paramWarehouseId.Value = CurrentWarehouseId;

            IEnumerable<ProductMaster> products = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList();
            StaticListLookUpSettings setting = (StaticListLookUpSettings)InventoryReport.paramProductId.LookUpSettings;

            foreach (var item in products)
            {
                LookUpValue product = new LookUpValue();
                product.Description = item.NameWithCode;
                product.Value = item.ProductId;
                setting.LookUpValues.Add(product);
            }

            return InventoryReport;
        }

        #endregion Inventory

        #region StockValueReport

        public ActionResult StockValueReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            StockValueReport report = CreateStockValueReport();
            //report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        public StockValueReport CreateStockValueReport()
        {
            var report = new StockValueReport();
            report.paramsTenantId.Value = CurrentTenantId;
            report.paramWarehouseId.Value = CurrentWarehouseId;

            IEnumerable<ProductMaster> products = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList().Distinct();
            StaticListLookUpSettings setting = (StaticListLookUpSettings)report.paramProductId.LookUpSettings;

            foreach (var item in products)
            {
                LookUpValue product = new LookUpValue();
                product.Description = item.NameWithCode;
                product.Value = item.ProductId;
                setting.LookUpValues.Add(product);
            }

            report.DataSourceDemanded += StockValueReport_DataSourceDemanded;

            return report;
        }

        private void StockValueReport_DataSourceDemanded(object sender, EventArgs e)
        {
            var report = (StockValueReport)sender;
            var productIds = (int[])report.paramProductId.Value;
            var tenantId = (int?)report.paramsTenantId.Value;
            var warehouseId = (int?)report.paramWarehouseId.Value;

            var products = _productServices.GetAllValidProductMasters(CurrentTenantId).Where(p => productIds.Contains(p.ProductId)).ToList();

            var inventoryStocks = _productServices.GetAllPalletTrackings(tenantId ?? 0, warehouseId ?? 0).Where(u => (u.Status == PalletTrackingStatusEnum.Active || u.Status == PalletTrackingStatusEnum.Hold) && productIds.Contains(u.ProductId)).GroupBy(u => u.ProductId).Select(c => new
            {
                ProductId = c.Key,
                inStock = c.Sum(u => u.RemainingCases),
                ProductMaster = c.Select(u => u.ProductMaster).FirstOrDefault()

            }).ToList();

            report.DataSource = inventoryStocks.Where(u => u.inStock > 0).Select(i =>
                {
                    var buyPrice = _productPriceService.GetAveragePurchasePrice(i.ProductId, CurrentTenantId);
                    return new
                    {
                        ProductSkuCode = i.ProductMaster.SKUCode,
                        ProductName = i.ProductMaster.NameWithCode,
                        InStock = i.inStock,
                        BuyPrice = buyPrice,
                        TotalPrice = (buyPrice ?? 0) * i.inStock
                    };
                });
        }

        #endregion StockValueReport

        #region MarketTotalReport

        public ActionResult MarketTotalReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            MarketTotal report = CreateMarketTotalReport();
            //report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        public MarketTotal CreateMarketTotalReport()
        {
            MarketTotal marketTotal = new MarketTotal();
            marketTotal.TenantId.Value = CurrentTenantId;
            marketTotal.paramStartDate.Value = DateTime.Today.AddMonths(-1);
            marketTotal.paramEndDate.Value = DateTime.Today;
            MarketListViewModel markets = _marketServices.GetAllValidMarkets(CurrentTenantId);

            var transactionTypes = from InventoryTransactionTypeEnum d in Enum.GetValues(typeof(InventoryTransactionTypeEnum))
                                   where d == InventoryTransactionTypeEnum.DirectSales || d == InventoryTransactionTypeEnum.Returns || d == InventoryTransactionTypeEnum.Exchange
                                   select new { InventoryTransactionTypeId = (int)d, InventoryTransactionTypeName = d.ToString() };

            StaticListLookUpSettings setting = (StaticListLookUpSettings)marketTotal.MarketId.LookUpSettings;
            var marketlist = markets.Markets.ToList();
            setting.LookUpValues.AddRange(marketlist.Select(u => new LookUpValue(u.Id, u.Name)));
            StaticListLookUpSettings invtrans = (StaticListLookUpSettings)marketTotal.InventoryTransType.LookUpSettings;
            invtrans.LookUpValues.AddRange(transactionTypes.Select(u => new LookUpValue(u.InventoryTransactionTypeId, u.InventoryTransactionTypeName)));

            return marketTotal;
        }

        #endregion MarketTotalReport

        #region LowStockItems

        public ActionResult LowStockItemsReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            LowStockItemsReport report = new LowStockItemsReport();
            StaticListLookUpSettings setting = (StaticListLookUpSettings)report.WarehouseParam.LookUpSettings;
            caTenant tenant = caCurrent.CurrentTenant();
            report.TenantIdParam.Value = tenant.TenantId;

            IEnumerable<ProductGroups> groups = LookupServices.GetAllValidProductGroups(CurrentTenantId).ToList();
            StaticListLookUpSettings groupSettings = (StaticListLookUpSettings)report.paramProductGroupId.LookUpSettings;

            foreach (var grp in groups)
            {
                LookUpValue group = new LookUpValue();
                group.Description = grp.ProductGroup;
                group.Value = grp.ProductGroupId;
                groupSettings.LookUpValues.Add(group);
            }
            IEnumerable<TenantDepartments> tenantDepartments = LookupServices.GetAllValidTenantDepartments(CurrentTenantId).ToList();
            StaticListLookUpSettings tenantDepartmentsSettings = (StaticListLookUpSettings)report.paramdepartmentId.LookUpSettings;

            foreach (var dep in tenantDepartments)
            {
                LookUpValue group = new LookUpValue();
                group.Description = dep.DepartmentName;
                group.Value = dep.DepartmentId;
                tenantDepartmentsSettings.LookUpValues.Add(group);
            }

            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            LookUpValue item = new LookUpValue();
            item.Description = "Select Location";
            item.Value = 0;
            setting.LookUpValues.Add(item);

            IEnumerable<TenantLocations> Warehouses = LookupServices.GetAllWarehousesForTenant(CurrentTenantId);

            foreach (var whs in Warehouses)
            {
                LookUpValue item2 = new LookUpValue();
                item2.Description = whs.WarehouseName;
                item2.Value = whs.WarehouseId;
                setting.LookUpValues.Add(item2);
            }

            return View(report);
        }

        #endregion LowStockItems

        #region PurchaseOrder

        public ActionResult PurchaseOrderPrint(int id = 0)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PurchaseOrderPrint report = CreatePurchaseOrderPrint(id);
            return View(report);
        }

        public ActionResult PurchaseOrderNotePrint(int id = 0)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            caTenant tenant = caCurrent.CurrentTenant();
            TenantConfig config = _tenantServices.GetTenantConfigById(tenant.TenantId);

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            POCollectionNotePrint report = CreatePurchaseOrderPrints(id);
            return View(report);
        }

        #endregion PurchaseOrder

        #region SalesOrder

        public ActionResult SalesOrderPrint(int id = 0, bool directsales = false, bool returns = false)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var report = CreateSalesOrderPrint(id, directsales, returns);
            ViewBag.DirectSalesOrder = directsales;
            ViewBag.OrderId = id;

            return View(report);
        }

        #endregion SalesOrder

        #region TransferOrder

        public ActionResult TransferOrderPrint(int id = 0)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var report = CreateTransferOrderPrint(id);
            return View(report);
        }

        #endregion TransferOrder

        #region WorksOrderPrint

        public ActionResult WorksOrderPrint(int id = 0)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var report = CreateWorksOrderPrint(id);
            report.paramTenantId.Value = CurrentTenantId;
            return View(report);
        }

        #endregion WorksOrderPrint

        #region WorksOrderDayPrint

        public ActionResult WorksOrderDayPrint()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            caTenant tenant = caCurrent.CurrentTenant();
            var resources = _employeeServices.GetAllActiveAppointmentResourceses(CurrentTenantId).ToList().Select(x => new SelectListItem() { Text = x.Name, Value = x.ResourceId.ToString() }).ToList();
            resources.Insert(0, new SelectListItem() { Text = "All", Value = "0" });

            var report = CreateWorksOrderDayPrint();

            report.paramTenantId.Value = tenant.TenantId;

            return View(report);
        }

        #endregion WorksOrderDayPrint

        #region SlaWorksOrder

        public ActionResult SlaWorkOrderReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var report = GetSlaWorksOrderReport();
            return View(report);
        }

        private SlaWorksOrderReport GetSlaWorksOrderReport()
        {
            var report = new SlaWorksOrderReport();

            var setting = (StaticListLookUpSettings)report.WarehouseParam.LookUpSettings;

            report.TenantIdParam.Value = CurrentTenantId;
            report.FromDate.Value = DateTime.Now.AddDays(-60);
            report.ToDate.Value = DateTime.Now;
            setting.LookUpValues.Add(new LookUpValue(0, "Select Location"));
            var warehouses = _tenantLocationsServices.GetAllTenantLocations(CurrentTenantId).ToList();
            foreach (var whs in warehouses)
            {
                setting.LookUpValues.Add(new LookUpValue(whs.WarehouseId, whs.WarehouseName));
            }
            return report;
        }

        #endregion SlaWorksOrder

        #region WorksOrderExpensiveProperties

        public ActionResult WorksOrderExpensiveProperties()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var report = new WorkOrderExpensivePropertiesReport();
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            PopulateExpensivePropertyReportParams(report);
            return View(report);
        }

        private void PopulateExpensivePropertyReportParams(XtraReport report)
        {
            var allProperties = PropertyService.GetAllValidProperties().ToList();
            WorkOrderExpensivePropertiesReport expensivePropertiesReport = (WorkOrderExpensivePropertiesReport)report;
            StaticListLookUpSettings paramPPropertyIdSettings = (StaticListLookUpSettings)expensivePropertiesReport.paramPPropertyId.LookUpSettings;

            paramPPropertyIdSettings.LookUpValues.AddRange(allProperties.Select(m => new LookUpValue(m.PPropertyId, m.PartAddress)));
            expensivePropertiesReport.paramTenantId.Value = CurrentTenantId;
            expensivePropertiesReport.paramStartDate.Value = DateTime.Today.AddMonths(-1);
            expensivePropertiesReport.paramEndDate.Value = DateTime.Today;
            expensivePropertiesReport.ParametersRequestBeforeShow += expensivePropertiesReport_ParametersRequestBeforeShow;
        }

        private void expensivePropertiesReport_ParametersRequestBeforeShow(object sender, ParametersRequestEventArgs e)
        {
            List<int> propertyIds = new List<int>();

            var property = PropertyService.GetAllValidProperties().Where(x => x.SiteId == 1).ToList();

            foreach (var prop in property)
            {
                propertyIds.Add(prop.PPropertyId);
            }

            e.ParametersInformation[1].Parameter.Value = propertyIds;
        }

        #endregion WorksOrderExpensiveProperties

        #region ChargeReports

        public ActionResult WorksOrdersChargesReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var report = GetWorksOrdersChargesReport();

            return View(report);
        }

        private WorksOrdersChargesReport GetWorksOrdersChargesReport()
        {
            WorksOrdersChargesReport chargesReport = new Reports.WorksOrdersChargesReport();
            StaticListLookUpSettings paramChargeTypeSettings = (StaticListLookUpSettings)chargesReport.paramChargeType.LookUpSettings;

            var chargeTypes = LookupServices.GetAllReportTypes(CurrentTenantId).Where(x => x.AllowChargeTo == true);

            // add static lookup values in paramter
            LookUpValue item = new LookUpValue();
            item.Description = "Select Charge Type";
            item.Value = 0;

            paramChargeTypeSettings.LookUpValues.Add(item);

            paramChargeTypeSettings.LookUpValues.AddRange(chargeTypes.Select(m => new LookUpValue(m.ReportTypeId, m.TypeName)));
            chargesReport.paramTenantId.Value = CurrentTenantId;
            chargesReport.paramStartDate.Value = DateTime.Today.AddMonths(-1);
            chargesReport.paramEndDate.Value = DateTime.Today;

            chargesReport.DataSourceDemanded += ChargesReport_DataSourceDemanded;
            return chargesReport;
        }

        private void ChargesReport_DataSourceDemanded(object sender, EventArgs e)
        {
            WorksOrdersChargesReport report = (WorksOrdersChargesReport)sender;
            int DefaultId = Convert.ToInt32(report.paramChargeType.Value);

            DateTime endDate = (DateTime)report.Parameters["paramEndDate"].Value;
            endDate = endDate.AddHours(24);
            report.Parameters["paramEndDate"].Value = endDate;

            if (DefaultId != 0)
            {
                report.FilterString = "[ReportTypeChargeId] = ?paramChargeType";
            }
        }

        #endregion ChargeReports

        #region TAReports

        public ActionResult ReportListing()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            var tenant = caCurrent.CurrentTenant();

            var report = new TimeAttendanceStatusReport();

            //PageHeader(report);
            //CreateListingReport(report, "Employee");

            report.DataSourceDemanded += Report_DataSourceDemanded;

            var paramTenantId = new Parameter()
            {
                Name = "tenantId",
                Type = typeof(int),
                Value = tenant.TenantId,
                Visible = false
            };

            var paramReportType = new Parameter()
            {
                Name = "ReportTypeId",
                Type = typeof(string),
                Value = "Lateness & Overtime",
                Description = "Report Type:",
                LookUpSettings = new StaticListLookUpSettings(),
            };

            ((StaticListLookUpSettings)paramReportType.LookUpSettings).LookUpValues.AddRange(new LookUpValue[] {
                new LookUpValue("1", "Lateness & Overtime"),
                new LookUpValue("2", "Absence")
            });

            var paramLocations = new Parameter()
            {
                Name = "WarehouseId",
                Type = typeof(string),
                Description = "Location: ",
                Value = "Select a Location",
                LookUpSettings = new DynamicListLookUpSettings()
                {
                    DataSource = _tenantLocationsServices.GetAllTenantLocations(tenant.TenantId),
                    DataMember = String.Empty,
                    DisplayMember = "WarehouseName",
                    ValueMember = "WarehouseId"
                }
            };

            var paramStartDate = new Parameter()
            {
                Name = "StartDate",
                Type = typeof(DateTime),
                Value = DateTime.Today.AddDays(-7),
                Description = "Start Date:",
            };

            var paramEndDate = new Parameter()
            {
                Name = "EndDate",
                Type = typeof(DateTime),
                Value = DateTime.Today,
                Description = "End Date:",
            };

            // Add the parameter to the report.
            report.Parameters.Add(paramReportType);
            report.Parameters.Add(paramLocations);
            report.Parameters.Add(paramStartDate);
            report.Parameters.Add(paramEndDate);
            report.Parameters.Add(paramTenantId);

            return View(report);
        }

        private void Report_DataSourceDemanded(object sender, EventArgs e)
        {
            var report = (TimeAttendanceStatusReport)sender;

            int reportTypeId = (!String.IsNullOrEmpty(report.Parameters["ReportTypeId"].Value.ToString()) ? (int)(report.Parameters["ReportTypeId"].Value.ToString() == "Lateness & Overtime" ? 1 : Convert.ToInt32(report.Parameters["ReportTypeId"].Value)) : 1); //1=Lateness & Overtime
            int locationId = (!String.IsNullOrEmpty(report.Parameters["WarehouseId"].Value.ToString()) ? (int)(report.Parameters["WarehouseId"].Value.ToString() == "Select a Location" ? 0 : Convert.ToInt32(report.Parameters["WarehouseId"].Value)) : 0);
            var startDate = (DateTime)report.Parameters["StartDate"].Value;
            var endDate = (DateTime)report.Parameters["EndDate"].Value;
            int tenantId = (int)report.Parameters["tenantId"].Value;

            report.FindControl("xrLabel2", true).Text = _tenantLocationsServices.GetTenantLocationById(locationId)?.WarehouseName;
            report.FindControl("xrLabel3", true).Text = $"{startDate.ToShortDateString()} - {endDate.ToShortDateString()}";

            var shiftSchedules = _shiftScheduleService.GetShiftSchedules(locationId, startDate, endDate);

            var isLatenessReport = reportTypeId == 1;

            report.StampInLabel.Visible = isLatenessReport;
            report.StampOutLabel.Visible = isLatenessReport;
            report.LateTimeLabel.Visible = isLatenessReport;
            report.OverTimeLabel.Visible = isLatenessReport;

            if (isLatenessReport)
            {//1=Lateness
                report.FindControl("xrLabel1", true).Text = "Lateness & Overtime";
                report.DataSource = LatenessAndOvertime(shiftSchedules);
            }

            if (reportTypeId == 2)
            { //4=Absence
                report.FindControl("xrLabel1", true).Text = "Absence";
                report.DataSource = Absence(shiftSchedules);
            }
        }

        public List<ReportViewModel> LatenessAndOvertime(IEnumerable<ShiftScheduleViewModel> shiftSchedules)
        {
            List<ReportViewModel> model = new List<ReportViewModel>();

            if (shiftSchedules != null && shiftSchedules.Count() >= 1)
            {
                int id = 1;

                //foreach employeeId
                foreach (var itemShift in shiftSchedules)
                {
                    //get EmployeeShifts by EmployeeId and Date
                    var allStamps = _employeeShiftsServices.SearchByEmployeeIdAndDate(itemShift.EmployeeId, itemShift.Date, CurrentTenantId);

                    if (allStamps != null && allStamps.Count() >= 1)
                    {
                        //get first stamp
                        var firstInStamp = allStamps.FirstOrDefault(a => a.StatusType == "In")?.TimeStamp;
                        var lastOutStamp = allStamps.LastOrDefault(a => a.StatusType == "Out" && a.TimeStamp > firstInStamp)?.TimeStamp;

                        if (lastOutStamp == null)
                        {
                            var nextDayStampLastOut = _employeeShiftsServices.SearchByEmployeeIdAndDate(itemShift.EmployeeId, itemShift.Date.AddDays(1), CurrentTenantId).FirstOrDefault();

                            lastOutStamp = nextDayStampLastOut.StatusType == "Out" ? nextDayStampLastOut.TimeStamp : (DateTime?)null;
                        }


                        if (firstInStamp.HasValue)
                        {
                            var totalTime = new TimeSpan();
                            var totalBreaksTaken = new TimeSpan();

                            if (lastOutStamp != null && allStamps.Count() <= 2)
                            {
                                var inTimestamp = !itemShift.IsFlexibleWorkingAllowed && itemShift != null && firstInStamp < itemShift?.StartTime ?
                                                    itemShift.StartTime :
                                                    itemShift.AttendanceGracePeriodInMinutes > 0 && itemShift != null && firstInStamp < itemShift.StartTime && (itemShift.StartTime - firstInStamp.Value).TotalMinutes > itemShift.AttendanceGracePeriodInMinutes ? itemShift.StartTime : firstInStamp.Value;

                                var outTimestamp = !itemShift.IsFlexibleWorkingAllowed && !itemShift.IsOvertimeWorkingAllowed && itemShift != null && lastOutStamp > itemShift?.EndTime ?
                                                     itemShift.EndTime :
                                                     lastOutStamp.Value;

                                totalTime = outTimestamp - inTimestamp;
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
                                            var inTimestamp = !itemShift.IsFlexibleWorkingAllowed && itemShift != null && allInStamps[index].TimeStamp < itemShift?.StartTime ?
                                                                itemShift.StartTime :
                                                                itemShift.AttendanceGracePeriodInMinutes > 0 && itemShift != null && allInStamps[index].TimeStamp < itemShift.StartTime && (itemShift.StartTime - allInStamps[index].TimeStamp).TotalMinutes > itemShift.AttendanceGracePeriodInMinutes ? itemShift.StartTime : allInStamps[index].TimeStamp;

                                            var outTimestamp = !itemShift.IsFlexibleWorkingAllowed && !itemShift.IsOvertimeWorkingAllowed && itemShift != null && allOutStamps[index].TimeStamp > itemShift?.EndTime ?
                                                                 itemShift.EndTime :
                                                                 allOutStamps[index].TimeStamp;

                                            totalTime += outTimestamp - inTimestamp;
                                        }

                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }

                                // calculate total breaks
                                totalBreaksTaken = (lastOutStamp.Value - firstInStamp.Value) - totalTime;
                            }

                            if (totalTime.TotalMinutes > 0)
                            {
                                model.Add(ReportTypeModel(id, firstInStamp, lastOutStamp, itemShift, totalTime));
                                id += 1;
                            };
                        }
                    }
                }
            }

            return model;
        }

        public List<ReportViewModel> Absence(IEnumerable<ShiftScheduleViewModel> shiftSchedules)
        {
            var model = new List<ReportViewModel>();

            if (shiftSchedules != null && shiftSchedules.Count() >= 1)
            {
                int id = 1;

                //foreach employeeId
                foreach (var shiftSchedule in shiftSchedules)
                {
                    //get EmployeeShifts by EmployeeId and Date
                    var employeeShifts = _employeeShiftsServices.SearchByEmployeeIdAndDate(shiftSchedule.EmployeeId, (DateTime)shiftSchedule.Date, CurrentTenantId);

                    //if no employeeShifts TimeStamp
                    if (employeeShifts.Count() == 0)
                    {
                        model.Add(ReportTypeModel(id, null, null, shiftSchedule, null));
                    }
                }
            }

            return model;
        }

        private ReportViewModel ReportTypeModel(int id, DateTime? stampFirstIn, DateTime? stampLastOut, ShiftScheduleViewModel shiftSchedule, TimeSpan? totalTime)
        {
            var overtime = totalTime != null ? ((TimeSpan)(totalTime - shiftSchedule.ExpectedHours)) : (TimeSpan?)null;

            return new ReportViewModel()
            {
                Id = id,
                Employee = shiftSchedule.EmployeeName,
                Date = shiftSchedule.Date,
                ShiftStartTime = shiftSchedule.StartTime,
                ShiftEndTime = shiftSchedule.EndTime,
                StampIn = stampFirstIn,
                StampOut = stampLastOut,
                LateTime = (stampFirstIn != null ? (stampFirstIn.Value.TimeOfDay - shiftSchedule.StartTime.TimeOfDay).ToString() : string.Empty),
                OverTime = overtime != null && overtime > new TimeSpan(0, 0, 0) ? overtime.ToString() : string.Empty
            };
        }

        #endregion TAReports

        #region InvoiceReport

        public ActionResult InvoiceDetails(int? id, string InvoiceMasterIds)
        {
            if (!caSession.AuthoriseSession())
            { return Redirect((string)Session["ErrorUrl"]); }
            if (!string.IsNullOrEmpty(InvoiceMasterIds))
            {
                int[] masterIds = Array.ConvertAll(InvoiceMasterIds.Split(','), Int32.Parse);
                var reports = CreateInvoicePrint(0, masterIds);

                return View(reports);
            }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var report = CreateInvoicePrint(id ?? 0);
            return View(report);
        }

        #endregion InvoiceReport

        #region PriceGroup

        public ActionResult PriceGroupReport(int PriceGroupId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            PriceGroupPrint report = CreatePriceGroup(PriceGroupId);
            return View(report);
        }

        public PriceGroupPrint CreatePriceGroup(int PriceGroupId)
        {
            PriceGroupPrint PriceGroupReport = new PriceGroupPrint();
            PriceGroupReport.TenantId.Value = CurrentTenantId;
            PriceGroupReport.PriceGroupId.Value = PriceGroupId;
            return PriceGroupReport;
        }

        #endregion PriceGroup

        #region OrderReport

        public ActionResult OrdersReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            OrdersReport report = CreateOrderReport();
            return View(report);
        }

        public OrdersReport CreateOrderReport()
        {
            OrdersReport OrderReport = new OrdersReport();
            OrderReport.paramStartDate.Value = DateTime.Today.AddMonths(-1);
            OrderReport.paramEndDate.Value = DateTime.Today;
            OrderReport.paramTenantId.Value = CurrentTenantId;

            StaticListLookUpSettings transactionTypesSettings = (StaticListLookUpSettings)OrderReport.paramTransactionTypes.LookUpSettings;
            StaticListLookUpSettings propertiesSettings = (StaticListLookUpSettings)OrderReport.paramProperty.LookUpSettings;
            StaticListLookUpSettings accountsSettings = (StaticListLookUpSettings)OrderReport.paramAccountIds.LookUpSettings;

            var transactionTypes = from InventoryTransactionTypeEnum d in Enum.GetValues(typeof(InventoryTransactionTypeEnum))
                                   select new { InventoryTransactionTypeId = (int)d, InventoryTransactionTypeName = d.ToString() };

            transactionTypesSettings.LookUpValues.AddRange(transactionTypes.Select(m => new LookUpValue(m.InventoryTransactionTypeId, m.InventoryTransactionTypeName)));

            var accounts = _accountServices.GetAllValidAccounts(CurrentTenantId).ToList();
            accountsSettings.LookUpValues.AddRange(accounts.Select(m => new LookUpValue(m.AccountID, m.CompanyName)));

            var properties = PropertyService.GetAllValidProperties().ToList();
            propertiesSettings.LookUpValues.AddRange(properties.Select(m => new LookUpValue(m.PPropertyId, m.AddressLine1)));
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                OrderReport.lblQuantity.TextFormatString = "{0:0.##}";
            }

            //OrderReport.DataSourceDemanded += OrderReport_DataSourceDemanded;

            return OrderReport;
        }

        private void OrderReport_DataSourceDemanded(object sender, EventArgs e)
        {
            OrdersReport report = (OrdersReport)sender;
            int[] defaultAccountId = (int[])report.paramAccountIds.Value;
            int[] defaultPropertyId = (int[])report.paramProperty.Value;

            if (defaultAccountId.Count() > 0)
            {
                report.FilterString = "[Ac Account ID] In (?paramAccountIds)";
            }

            if (defaultPropertyId.Count() > 0)
            {
                report.FilterString = "[PProperty Id] In (?paramProperty)";
            }
        }

        #endregion OrderReport

        #region PickersOrdersReport

        public ActionResult PickersOrdersReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var report = CreatePickersOrdersReport();
            return View(report);
        }

        public PickersOrdersReport CreatePickersOrdersReport()
        {
            var pickersOrdersReport = new PickersOrdersReport();
            pickersOrdersReport.paramDate.Value = DateTime.Today;
            pickersOrdersReport.paramTenantId.Value = CurrentTenantId;

            var pickersSettings = (StaticListLookUpSettings)pickersOrdersReport.paramPickerIds.LookUpSettings;
            var propertiesSettings = (StaticListLookUpSettings)pickersOrdersReport.paramProperty.LookUpSettings;
            var accountsSettings = (StaticListLookUpSettings)pickersOrdersReport.paramAccountIds.LookUpSettings;

            var users = _userService.GetUsersAgainstPermission(CurrentTenantId, CurrentWarehouseId, "Handheld", "SalesOrderPerm").Where(u => !string.IsNullOrEmpty(u.DisplayName?.Trim()));

            pickersSettings.LookUpValues.AddRange(users.Select(m => new LookUpValue(m.UserId, m.UserFirstName)));

            var accounts = _accountServices.GetAllValidAccounts(CurrentTenantId).ToList();
            accountsSettings.LookUpValues.AddRange(accounts.Select(m => new LookUpValue(m.AccountID, m.CompanyName)));

            var properties = PropertyService.GetAllValidProperties().ToList();
            propertiesSettings.LookUpValues.AddRange(properties.Select(m => new LookUpValue(m.PPropertyId, m.AddressLine1)));
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                pickersOrdersReport.lblQuantity.TextFormatString = "{0:0.##}";
            }

            return pickersOrdersReport;
        }

        #endregion PickersOrdersReport

        #region OrdersTotal

        public ActionResult OrdersTotal()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            OrdersTotal report = CreateOrderTotalReport();
            return View(report);
        }

        public OrdersTotal CreateOrderTotalReport()
        {
            OrdersTotal OrderTotal = new OrdersTotal();
            OrderTotal.paramStartDate.Value = DateTime.Now.AddMonths(-1);
            OrderTotal.paramEndDate.Value = DateTime.Now;
            OrderTotal.paramTenantId.Value = CurrentTenantId;
            StaticListLookUpSettings transactionTypesSettings = (StaticListLookUpSettings)OrderTotal.paramTransactionTypes.LookUpSettings;

            var transactionTypes = from InventoryTransactionTypeEnum d in Enum.GetValues(typeof(InventoryTransactionTypeEnum))
                                   select new { InventoryTransactionTypeId = (int)d, InventoryTransactionTypeName = d.ToString() };

            transactionTypesSettings.LookUpValues.AddRange(transactionTypes.Select(m => new LookUpValue(m.InventoryTransactionTypeId, m.InventoryTransactionTypeName)));

            return OrderTotal;
        }

        #endregion OrdersTotal

        #region WorksOrderKpiReport

        public ActionResult WorksOrderKpiReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            WorksOrderKpiReport report = new WorksOrderKpiReport();

            report.paramStartDate.Value = DateTime.Today.AddDays(-15);
            report.paramEndDate.Value = DateTime.Today;
            report.paramTenantId.Value = tenant.TenantId;

            report.DataSourceDemanded += WorksOrderKpi_DataSourceDemanded;

            // binding

            report.Sector.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".Sector")});

            report.Logged.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".Logged")});

            report.Completed.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".Completed")});

            report.Reallocated.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".Reallocated")});

            report.Unallocated.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".Unallocated")});

            report.OldestJob.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".OldestJob")});

            return View(report);
        }

        private void WorksOrderKpi_DataSourceDemanded(object sender, EventArgs e)
        {
            WorksOrderKpiReport report = (WorksOrderKpiReport)sender;
            DateTime startDate = (DateTime)report.Parameters["paramStartDate"].Value;
            DateTime endDate = (DateTime)report.Parameters["paramEndDate"].Value;
            endDate = endDate.AddHours(24);
            int tenantId = (int)report.Parameters["paramTenantId"].Value;
            var Logged = OrderService.GetAllWorksOrders(tenantId).Where(x => x.DateCreated >= startDate && x.DateCreated < endDate && x.IsDeleted != true);
            var Completed = OrderService.GetAllWorksOrders(tenantId).Where(x => x.DateUpdated >= startDate && x.DateUpdated < endDate && x.OrderStatusID == OrderStatusEnum.Complete && x.IsDeleted != true);
            var jobTypes = LookupServices.GetAllJobTypes(tenantId).ToList();
            var dataSource = new List<WorksorderKpiReportViewModel>();
            foreach (var type in jobTypes)
            {
                var loggedForType = Logged.Where(x => x.JobTypeId == type.JobTypeId);
                var completedForType = Completed.Where(x => x.JobTypeId == type.JobTypeId).Count();
                var reallocated = loggedForType.Where(x => x.OrderStatusID == OrderStatusEnum.ReAllocationRequired).Count();
                var unallocated = loggedForType.Where(x => x.OrderStatusID == OrderStatusEnum.NotScheduled).Count();
                var oldestJob = OrderService.GetAllWorksOrders(tenantId).FirstOrDefault(x => x.OrderStatusID != OrderStatusEnum.Complete && x.JobTypeId == type.JobTypeId)?.DateCreated.ToString("dd/MM/yyyy");

                if (loggedForType.Count() > 0 || completedForType > 0 || reallocated > 0 || unallocated > 0)
                {
                    var sourceItem = new WorksorderKpiReportViewModel();
                    sourceItem.Sector = type.Name;
                    sourceItem.Logged = loggedForType.Count();
                    sourceItem.Completed = completedForType;
                    sourceItem.Reallocated = reallocated;
                    sourceItem.Unallocated = unallocated;
                    sourceItem.OldestJob = oldestJob;
                    dataSource.Add(sourceItem);
                }
            }

            report.JobsLogged.Text = Logged.Count().ToString();
            report.JobsCompleted.Text = Completed.Count().ToString();
            report.DataSource = dataSource;
        }

        #endregion WorksOrderKpiReport

        #region DeliveryNote

        public ActionResult DeliveryNotePrint(int id = 0)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            caTenant tenant = caCurrent.CurrentTenant();
            TenantConfig config = _tenantServices.GetTenantConfigById(tenant.TenantId);

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var groupstatus = _tenantServices.GetAllTenantConfig(CurrentTenantId)?.FirstOrDefault(u => u.EnableTimberProperties)?.EnableTimberProperties;
            if (groupstatus == true)
            {
                DileveryReportPrintGroup report = CreateDeliveryNotePrintByGroup(id);
                report.FooterMsg1.Text = config.DnReportFooterMsg1;
                report.FooterMsg2.Text = config.DnReportFooterMsg2;
                return View(report);
            }
            else
            {
                DeliveryNotePrint report = CreateDeliveryNotePrint(id);
                report.FooterMsg1.Text = config.DnReportFooterMsg1;
                report.FooterMsg2.Text = config.DnReportFooterMsg2;
                return View(report);
            }
        }

        #endregion DeliveryNote

        #region PalleteReport

        public ActionResult PalleteReportPrint(int dispatchId = 0)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            caTenant tenant = caCurrent.CurrentTenant();
            //var dataSource = _palletingService.GetAllPalletsDispatch(dispatchId);

            if (dispatchId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PalleteDispatchesReport report = CreatePalleteReport(dispatchId);
            report.PoPictureBox.BeforePrint += PRPictureBox_BeforePrint;

            return View(report);
        }

        private void PRPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("dn-logo.png");
        }

        #endregion PalleteReport

        #region GoodsBookInNote

        public ActionResult GoodsBookInNotePrint(int id = 0)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DeliveryNotePrint report = CreateDeliveryNotePrint(id);
            report.RequestParameters = false;
            report.xrLabel16.Text = "Goods Book In Note";

            //report.xrLabel26.Text = "";
            return View(report);
        }

        #endregion GoodsBookInNote

        #region GoodsReceiveNotePrint

        public ActionResult GoodsReceiveNotePrint(Guid id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GoodsReceiveNotePrint report = CreateGoodsReceiveNotePrint(id);
            return View(report);
        }

        #endregion GoodsReceiveNotePrint

        #region MarketRoutePrint

        public ActionResult MarketRoutePrint(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MarketRoutePrint report = CreateMarketRoutePrint(id);
            return View(report);
        }

        #endregion MarketRoutePrint

        #region MarketCustomerPrint

        public ActionResult MarketCustomerPrint(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            MarketCustomerReport report = CreateMarketCustomerPrint(id);
            return View(report);
        }

        #endregion MarketCustomerPrint

        #region FinancialTransactionReport

        public ActionResult FinancialTransactionReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            FinancialTransactionReport report = CreateFinancialTransactionReport();
            //report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        #endregion FinancialTransactionReport

        #region ProductMovementReport

        public ActionResult ProductMovementReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ProductMovementPrint report = CreateProductMovementPrint();
            //report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        public ProductMovementPrint CreateProductMovementPrint()
        {
            ProductMovementPrint productMovement = new ProductMovementPrint();
            productMovement.StartDate.Value = DateTime.Today.AddMonths(-1);
            productMovement.EndDate.Value = DateTime.Today;
            productMovement.lbldate.Text = DateTime.UtcNow.ToShortDateString();
            productMovement.TenantId.Value = CurrentTenantId;
            productMovement.WarehouseId.Value = CurrentWarehouseId;
            IEnumerable<ProductMaster> products = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList();
            StaticListLookUpSettings setting = (StaticListLookUpSettings)productMovement.paramProductId.LookUpSettings;

            foreach (var item in products)
            {
                LookUpValue product = new LookUpValue();
                product.Description = item.NameWithCode;
                product.Value = item.ProductId;
                setting.LookUpValues.Add(product);
            }

            IEnumerable<ProductGroups> groups = LookupServices.GetAllValidProductGroups(CurrentTenantId).ToList();
            StaticListLookUpSettings groupSettings = (StaticListLookUpSettings)productMovement.paramProductGroups.LookUpSettings;

            foreach (var grp in groups)
            {
                LookUpValue group = new LookUpValue();
                group.Description = grp.ProductGroup;
                group.Value = grp.ProductGroupId;
                groupSettings.LookUpValues.Add(group);
            }
            return productMovement;
        }

        #endregion ProductMovementReport

        #region ProductSalesReportbyAccount

        public ActionResult ProductSaleReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ProductSoldReport report = CreateProductsalePrint();
            //report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        public ProductSoldReport CreateProductsalePrint()
        {
            ProductSoldReport productMovement = new ProductSoldReport();
            productMovement.StartDate.Value = DateTime.Now.AddMonths(-1);
            productMovement.EndDate.Value = DateTime.Now;
            productMovement.lbldate.Text = DateTime.UtcNow.ToShortDateString();
            StaticListLookUpSettings marketSettings = (StaticListLookUpSettings)productMovement.MarketId.LookUpSettings;
            var accounts = _marketServices.GetAllValidMarkets(CurrentTenantId).Markets.ToList();
            marketSettings.LookUpValues.AddRange(accounts.Select(m => new LookUpValue(m.Id, m.Name)));

            return productMovement;
        }

        #endregion ProductSalesReportbyAccount

        #region ProductPurchase/SaleHistoryReport

        public ActionResult ProductPurchaseReportBySku()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ProductOrdersHistoryReport report = CreateProductOrdersHistoryReport(InventoryTransactionTypeEnum.PurchaseOrder);
            ViewBag.Title = "Purchase History Report";
            return View("ProductOrdersHistoryReport", report);
        }

        public ActionResult ProductSaleReportBySku()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ProductOrdersHistoryReport report = CreateProductOrdersHistoryReport(InventoryTransactionTypeEnum.SalesOrder);
            ViewBag.Title = "Sales History Report";
            return View("ProductOrdersHistoryReport", report);
        }

        private ProductOrdersHistoryReport CreateProductOrdersHistoryReport(InventoryTransactionTypeEnum ordersType)
        {
            var tenantConfig = _tenantServices.GetTenantConfigById(CurrentTenantId);

            var productOrdersHistoryReport = new ProductOrdersHistoryReport();
            productOrdersHistoryReport.detailedOrdersToggle.Visible = tenantConfig.EnableOrdersHistoryReportDetails;
            productOrdersHistoryReport.DetailReport.Visible = tenantConfig.EnableOrdersHistoryReportDetails;
            productOrdersHistoryReport.StartDate.Value = DateTime.Today.AddMonths(-1);
            productOrdersHistoryReport.EndDate.Value = DateTime.Today;
            productOrdersHistoryReport.lbldate.Text = DateTime.UtcNow.ToShortDateString();
            productOrdersHistoryReport.TenantId.Value = CurrentTenantId;
            productOrdersHistoryReport.WarehouseId.Value = CurrentWarehouseId;

            if (ordersType == InventoryTransactionTypeEnum.SalesOrder)
            {
                int[] inventoryTypes = { (int)InventoryTransactionTypeEnum.SalesOrder, (int)InventoryTransactionTypeEnum.DirectSales };
                productOrdersHistoryReport.InventoryTransactionTypeIds.Value = inventoryTypes;
                productOrdersHistoryReport.xrLabel22.Text = "Sales History Report";
            }
            else
            {
                int[] inventoryTypes = { (int)ordersType };
                productOrdersHistoryReport.InventoryTransactionTypeIds.Value = inventoryTypes;
                productOrdersHistoryReport.xrLabel22.Text = "Purchase History Report";
            }

            var markets = _marketServices.GetAllValidMarkets(CurrentTenantId);
            StaticListLookUpSettings marketSettings = (StaticListLookUpSettings)productOrdersHistoryReport.MarketId.LookUpSettings;
            marketSettings.LookUpValues.AddRange(markets.Markets.Select(m => new LookUpValue(m.Id, m.Name)));

            var accounts = _accountServices.GetAllValidAccounts(CurrentTenantId).ToList();
            StaticListLookUpSettings accountSettings = (StaticListLookUpSettings)productOrdersHistoryReport.AccountIds.LookUpSettings;
            accountSettings.LookUpValues.AddRange(accounts.Select(m => new LookUpValue(m.AccountID, m.CompanyName)));

            var accountSectors = _accountSectorServices.GetAll();
            StaticListLookUpSettings accountSectorsSettings = (StaticListLookUpSettings)productOrdersHistoryReport.AccountSectorIds.LookUpSettings;
            accountSectorsSettings.LookUpValues.AddRange(accountSectors.Select(m => new LookUpValue(m.Id, m.Name)));


            var users = OrderService.GetAllAuthorisedUsers(CurrentTenantId, true);
            StaticListLookUpSettings ownerSettings = (StaticListLookUpSettings)productOrdersHistoryReport.OwnerIds.LookUpSettings;
            ownerSettings.LookUpValues.AddRange(users.Select(m => new LookUpValue(m.UserId, m.DisplayName)));

            IEnumerable<ProductGroups> groups = LookupServices.GetAllValidProductGroups(CurrentTenantId).ToList();
            StaticListLookUpSettings groupSettings = (StaticListLookUpSettings)productOrdersHistoryReport.paramProductGroups.LookUpSettings;

            foreach (var grp in groups)
            {
                LookUpValue group = new LookUpValue();
                group.Description = grp.ProductGroup;
                group.Value = grp.ProductGroupId;
                groupSettings.LookUpValues.Add(group);
            }
            IEnumerable<TenantDepartments> tenantDepartments = LookupServices.GetAllValidTenantDepartments(CurrentTenantId).ToList();
            StaticListLookUpSettings tenantDepartmentsSettings = (StaticListLookUpSettings)productOrdersHistoryReport.paramProductDepartment.LookUpSettings;

            foreach (var dep in tenantDepartments)
            {
                LookUpValue group = new LookUpValue();
                group.Description = dep.DepartmentName;
                group.Value = dep.DepartmentId;
                tenantDepartmentsSettings.LookUpValues.Add(group);
            }

            var products = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList();
            StaticListLookUpSettings setting = (StaticListLookUpSettings)productOrdersHistoryReport.ProductsIds.LookUpSettings;
            foreach (var item in products)
            {
                LookUpValue product = new LookUpValue();
                product.Description = item.NameWithCode;
                product.Value = item.ProductId;
                setting.LookUpValues.Add(product);
            }

            if (tenantConfig.EnableOrdersHistoryReportDetails)
            {
                var productOrdersHistoryDetailReport = new ProductOrdersHistoryDetailReport();
                productOrdersHistoryDetailReport.DataSourceDemanded += CreateProductOrdersHistoryReport_DataSourceDemanded;
                productOrdersHistoryDetailReport.OrderNumber.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", productOrdersHistoryDetailReport.DataSource, ".OrderNumber")});

                productOrdersHistoryDetailReport.Qty.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", productOrdersHistoryDetailReport.DataSource, ".Qty")});

                productOrdersHistoryDetailReport.BuyPrice.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", productOrdersHistoryDetailReport.DataSource, ".BuyPrice")});

                if (ordersType == InventoryTransactionTypeEnum.SalesOrder)
                {
                    productOrdersHistoryDetailReport.SellPrice.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", productOrdersHistoryDetailReport.DataSource, ".SellPrice")});
                }
                else
                {
                    productOrdersHistoryDetailReport.SellPriceTitle.Visible = false;
                }

                productOrdersHistoryDetailReport.TaxAmount.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", productOrdersHistoryDetailReport.DataSource, ".TaxAmount")});

                productOrdersHistoryDetailReport.AccountName.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", productOrdersHistoryDetailReport.DataSource, ".AccountNameCode")});

                productOrdersHistoryDetailReport.TotalAmount.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", productOrdersHistoryDetailReport.DataSource, ".TotalAmount")});

                productOrdersHistoryDetailReport.ExpectedDate.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", productOrdersHistoryDetailReport.DataSource, ".Date")});


                productOrdersHistoryReport.xrSubreport1.ReportSource = productOrdersHistoryDetailReport;
            }
            return productOrdersHistoryReport;
        }

        public void CreateProductOrdersHistoryReport_DataSourceDemanded(object sender, EventArgs e)
        {
            var report = (ProductOrdersHistoryDetailReport)sender;

            DateTime startDate = (DateTime)report.Parameters["StartDate"].Value;
            DateTime endDate = (DateTime)report.Parameters["EndDate"].Value;
            endDate = endDate.AddHours(24);
            var accountIds = (int[])report.Parameters["AccountIds"].Value;
            var accountSectorIds = (int[])report.Parameters["AccountSectorIds"].Value;
            var productId = (int?)report.Parameters["ProductId"].Value;
            var ownerIds = (int[])report.Parameters["OwnerIds"].Value;
            var marketId = (int?)report.Parameters["MarketId"].Value;
            var tenantId = (int)report.Parameters["TenantId"].Value;
            var warehouseId = (int)report.Parameters["WarehouseId"].Value;
            var inventoryTransactionTypeIds = (InventoryTransactionTypeEnum[])report.Parameters["InventoryTransactionTypeIds"].Value;


            report.DataSource = OrderService.GetAllOrdersByProductId(inventoryTransactionTypeIds, productId ?? 0, startDate, endDate, tenantId, warehouseId, accountIds, ownerIds, accountSectorIds, marketId);

        }

        #endregion ProductSalesReportbySKU

        #region InvoiceProfitReport

        public ActionResult InvoiceProfitReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            var tenant = caCurrent.CurrentTenant();
            var report = new InvoiceProfitPrint();
            report.paramStartDate.Value = DateTime.Today.AddMonths(-1);
            report.paramEndDate.Value = DateTime.Today;
            var accountsSettings = (StaticListLookUpSettings)report.paramAccountIds.LookUpSettings;
            var accounts = _accountServices.GetAllValidAccounts(CurrentTenantId).ToList();
            accountsSettings.LookUpValues.AddRange(accounts.Select(m => new LookUpValue(m.AccountID, m.CompanyName)));

            var productsSettings = (StaticListLookUpSettings)report.paramProductIds.LookUpSettings;
            var products = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList();
            productsSettings.LookUpValues.AddRange(products.Select(m => new LookUpValue(m.ProductId, m.NameWithCode)));

            var marketsSettings = (StaticListLookUpSettings)report.paramMarketId.LookUpSettings;
            var markets = _marketServices.GetAllValidMarkets(CurrentTenantId).Markets;
            marketsSettings.LookUpValues.AddRange(markets.Select(m => new LookUpValue(m.Id, m.Name)));

            var users = OrderService.GetAllAuthorisedUsers(CurrentTenantId, true);
            StaticListLookUpSettings ownerSettings = (StaticListLookUpSettings)report.paramOwnerIds.LookUpSettings;
            ownerSettings.LookUpValues.AddRange(users.Select(m => new LookUpValue(m.UserId, m.DisplayName)));

            report.DataSourceDemanded += ProfitInvoiceReport_DataSourceDemanded;

            return View(report);
        }

        private void ProfitInvoiceReport_DataSourceDemanded(object sender, EventArgs e)
        {
            var report = (InvoiceProfitPrint)sender;
            var startDate = (DateTime)report.paramStartDate.Value;
            var endDate = (DateTime)report.paramEndDate.Value;
            endDate = endDate.AddHours(24);
            var accountIds = (int[])report.paramAccountIds.Value;
            var productIds = (int[])report.paramProductIds.Value;
            var marketId = (int?)report.paramMarketId.Value;
            var ownerIds = (int[])report.paramOwnerIds.Value;
            var invoices = _invoiceService.GetAllInvoiceMastersWithAllStatus(CurrentTenantId, accountIds)
                                          .Where(x => x.InvoiceDate >= startDate && x.InvoiceDate < endDate);

            if (ownerIds != null && ownerIds.Count() > 0)
            {
                invoices = invoices.Where(x => ownerIds.Contains(x.Account.OwnerUserId));
            }

            if (marketId != null && marketId != 0)
            {
                invoices = invoices.Where(i => i.Account.MarketCustomers.Any(m => m.MarketId == marketId));
            }

            if (productIds != null && productIds.Count() > 0)
            {
                invoices = invoices.Where(i => i.OrderProcess.OrderProcessDetail.Any(m => productIds.Contains(m.ProductId)));
            }

            var dataSource = new List<InvoiceProfitReportViewModel>();
            foreach (var invoice in invoices.ToList())
            {
                var invoiceProductsPrices = _invoiceService.GetInvoiceProductsPrices(invoice.InvoiceMasterId, productIds, CurrentTenantId);
                var buyingNetAmount = invoiceProductsPrices.Sum(p => p.TotalBuyPrice);
                var sellingNetAmount = invoiceProductsPrices.Sum(p => p.TotalSellPrice);
                dataSource.Add(new InvoiceProfitReportViewModel
                {
                    InvoiceId = invoice.InvoiceMasterId,
                    InvoiceNumber = invoice.InvoiceNumber,
                    CompanyName = invoice.Account?.CompanyName ?? "",
                    Date = invoice.DateCreated,
                    NetAmtB = invoiceProductsPrices.Sum(p => p.TotalBuyPrice),
                    NetAmtS = invoiceProductsPrices.Sum(p => p.TotalSellPrice),
                    Profit = sellingNetAmount - buyingNetAmount,
                    ProfitPercent = sellingNetAmount > 0 ? ((sellingNetAmount - buyingNetAmount) / sellingNetAmount) * 100.0m : 0,

                    ProductsDetail = invoiceProductsPrices.Select(p => new InvoiceProfitReportProductsViewModel
                    {
                        InvoiceId = p.InvoiceId,
                        ProductName = p.ProductName,
                        Quantity = p.Quantity,
                        BuyPrice = p.BuyPrice,
                        SellPrice = p.SellPrice,
                        TotalBuyPrice = p.TotalBuyPrice,
                        TotalSellPrice = p.TotalSellPrice,
                        Profit = p.TotalSellPrice - p.TotalBuyPrice,
                        ProfitPercent = p.TotalSellPrice > 0 ? ((p.TotalSellPrice - p.TotalBuyPrice) / (p.TotalSellPrice)) * 100.0m : 0
                    }).ToList()
                });
            }

            report.TotalNetAmtB.Text = string.Format("{0:0.00}", dataSource.Sum(u => u.NetAmtB));
            report.TotalNetAmtS.Text = string.Format("{0:0.00}", dataSource.Sum(u => u.NetAmtS));
            report.TotalProfit.Text = string.Format("{0:0.00}", dataSource.Sum(u => u.Profit));
            //report.Profit.Text = string.Format("{0:0.00}", dataSource.Sum(u => u.Profit));

            var totalProfit = dataSource.Sum(u => u.Profit);
            var totalNetSale = dataSource.Sum(u => u.NetAmtS);

            var profitPercent = totalNetSale > 0 ? ((totalProfit ?? 0) / (totalNetSale ?? 1)) * 100.0m : 0;
            //report.xrLabelProfitPercent.Text = string.Format("{0:0.00}", profitPercent);
            report.TotalProfitPercent.Text = string.Format("{0:0.00}", profitPercent);


            report.DataSource = dataSource;
        }


        #region StockShortageReport
        public ActionResult StockShortageReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            var report = new StockShortage();
            report.paramEndDate.Value = DateTime.Today;


            report.DataSourceDemanded += StockShortageReport_DataSourceDemanded;

            return View(report);
        }
        private void StockShortageReport_DataSourceDemanded(object sender, EventArgs e)
        {
            var report = (StockShortage)sender;
            var endDate = (DateTime)report.paramEndDate.Value;
            var orders = OrderService.GetAllOrders(CurrentTenantId, 1)
                                          .Where(x => DbFunctions.TruncateTime(x.ExpectedDate) == DbFunctions.TruncateTime(endDate) && x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder).ToList();
            var orderDetails = orders.SelectMany(u => u.OrderDetails).ToList();
            var dataSource = new List<StockShortageReportDetail>();
            foreach (var orderDetail in orderDetails.ToList())
            {
                var remainingCases = _palletingService.CheckPalletRemaingCases(orderDetail.ProductId, orderDetail.Qty);
                if (orderDetail.Qty > remainingCases.RemainingCases)
                {
                    dataSource.Add(new StockShortageReportDetail
                    {
                        OrderNumber = orderDetail.Order.OrderNumber,
                        ProductName = orderDetail.ProductMaster.Name,
                        Qty = orderDetail.Qty,
                        SkuCode = orderDetail.ProductMaster.SKUCode,
                        RemainingCases = remainingCases.RemainingCases,
                        Shortage = orderDetail.Qty - remainingCases.RemainingCases,

                    });
                }

            }
            var groupedData = dataSource.GroupBy(u => u.OrderNumber).ToList();
            var data = groupedData.Select(u => new StockShortageReportModel
            {

                OrderNumber = u.Key,
                StockReportDetails = u.SelectMany(c => new List<StockShortageReportDetail>
                {
                    new StockShortageReportDetail
                {
                    ProductName = c.ProductName,
                    SkuCode = c.SkuCode,
                    Qty = c.Qty,
                    RemainingCases=c.RemainingCases,
                    Shortage=c.Shortage
                }
                }).ToList()

            }).ToList();
            report.DataSource = data;
        }



        #endregion

        #region StockShortageReport
        public ActionResult InvoiceTrackingReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            var report = new InvoiceTrackingReport();

            return View(report);
        }

        #endregion StockShortageReport


        #endregion InvoiceProfitReport

        #region InvoiceByProduct

        public ActionResult InvoiceByProductReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var report = InvoiceByProductReportPrint();
            return View(report);
        }

        public InvoiceByProductReport InvoiceByProductReportPrint()
        {
            var InvoiceByProductReport = new InvoiceByProductReport();
            InvoiceByProductReport.paramStartDate.Value = DateTime.Today.AddMonths(-1);
            InvoiceByProductReport.paramEndDate.Value = DateTime.Today;
            InvoiceByProductReport.TenantID.Value = CurrentTenantId;
            var products = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList();
            var setting = (StaticListLookUpSettings)InvoiceByProductReport.paramProductsIds.LookUpSettings;

            foreach (var item in products)
            {
                var product = new LookUpValue();
                product.Description = item.NameWithCode;
                product.Value = item.ProductId;
                setting.LookUpValues.Add(product);
            }

            var groups = LookupServices.GetAllValidProductGroups(CurrentTenantId).ToList();
            StaticListLookUpSettings groupSettings = (StaticListLookUpSettings)InvoiceByProductReport.paramProductGroups.LookUpSettings;

            foreach (var grp in groups)
            {
                var group = new LookUpValue();
                group.Description = grp.ProductGroup;
                group.Value = grp.ProductGroupId;
                groupSettings.LookUpValues.Add(group);
            }

            return InvoiceByProductReport;
        }

        #endregion InvoiceByProduct

        #region InvoiceByProduct

        public ActionResult InvoiceDetailReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var report = new InvoiceDetailReport();

            report.TenantID.Value = CurrentTenantId;

            report.DataSourceDemanded += InvoiceDetailReport_DataSourceDemanded;

            // binding

            report.SkuCode.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".SkuCode") });
            report.ProductName.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".ProductName") });
            report.Quantity.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".Quantity") });
            report.PalletNumber.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".PalletNumber") });
            report.SaleOrderNumber.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".SaleOrderNumber") });
            report.DeliveryNote.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".DeliveryNote") });
            report.PurchaseOrderNumber.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".PurchaseOrderNumber") });
            report.SupplierName.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".SupplierName") });
            report.SupplierInvoiceNumber.DataBindings.AddRange(new XRBinding[] { new XRBinding("Text", report.DataSource, ".SupplierInvoiceNumber") });

            return View(report);
        }

        private void InvoiceDetailReport_DataSourceDemanded(object sender, EventArgs e)
        {
            var report = (InvoiceDetailReport)sender;
            var invoiceNo = (string)report.Parameters["InvoiceNo"].Value;

            var invoice = _invoiceService.GetAllInvoiceViews(CurrentTenantId).FirstOrDefault(i => i.InvoiceNumber == invoiceNo);

            if (invoice == null)
            {
                return;
            }

            report.InvoiceNumber.Value = invoice?.InvoiceNumber;
            report.InvoiceDate.Value = invoice?.InvoiceDate;

            report.DataSource = _invoiceService.GetAllInvoiceDetailReportData(invoice.InvoiceMasterId);
        }

        #endregion InvoiceByProduct

        #region PalletTrackingLabelReport

        public ActionResult PrintLabelReport(string PalletTrackingIds)
        {
            if (!caSession.AuthoriseSession())
            { return Redirect((string)Session["ErrorUrl"]); }
            if (!string.IsNullOrEmpty(PalletTrackingIds))
            {
                int[] palletIds = Array.ConvertAll(PalletTrackingIds.Split(','), Int32.Parse);
                var reports = CreateLabelPrint(palletIds);
                return View(reports);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public PalletTrackingLabelPrint CreateLabelPrint(int[] PalletTrackingIds)
        {
            PalletTrackingLabelPrint PrintLabel = new PalletTrackingLabelPrint();
            PrintLabel.ParamPalletTrackingId.Value = PalletTrackingIds;
            PrintLabel.RequestParameters = false;
            return PrintLabel;
        }

        #endregion PalletTrackingLabelReport

        #region HolidayReportsPrint

        public ActionResult HolidayReportsPrint()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();
            HolidayReportPrint report = new HolidayReportPrint();
            report.DataSourceDemanded += HolidayReport_DataSourceDemanded;

            report.UserName.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".UserName")});

            report.Date.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".Date")});

            report.FirstYear.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".FirstYear")});

            report.SecondYear.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".SecondYear")});
            report.ThirdYear.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".ThirdYear")});
            report.FourthYear.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".FourthYear")});

            report.FifthYear.DataBindings.AddRange(new XRBinding[] {
                new XRBinding("Text", report.DataSource, ".FifthYear")});

            return View(report);
        }

        private void HolidayReport_DataSourceDemanded(object sender, EventArgs e)
        {
            HolidayReportPrint report = (HolidayReportPrint)sender;
            var years = _employeeServices.GetEmployeeYears(CurrentTenantId);
            if (years.Count > 0)
            {
                report.lblYear1.Text = years[years.Count - years.Count].ToString();
                report.lblYear2.Text = years[years.Count - (years.Count - 1)].ToString();
                report.lblYear3.Text = years[years.Count - (years.Count - 2)].ToString();
                report.lblYear4.Text = years[years.Count - (years.Count - 3)].ToString();
                report.lblYear5.Text = years[years.Count - (years.Count - 4)].ToString();
                var dataSource = new List<HolidayReportViewModel>();
                var EmployeesList = _employeeServices.GetAllEmployees(CurrentTenantId);
                foreach (var item in EmployeesList)
                {
                    var EmployeeName = item.Name;
                    var JobStartDate = item.JobStartDate;
                    var firstYear = string.Format("{0} {1} {2}", _employeeServices.GetHolidaysCountByEmployeeId(item.ResourceId, years[years.Count - years.Count]).ToString(), "out of", item.HolidayEntitlement.ToString());
                    var secondYear = string.Format("{0} {1} {2}", _employeeServices.GetHolidaysCountByEmployeeId(item.ResourceId, years[years.Count - (years.Count - 1)]).ToString(), "out of", item.HolidayEntitlement.ToString());
                    var thirdYear = string.Format("{0} {1} {2}", _employeeServices.GetHolidaysCountByEmployeeId(item.ResourceId, years[years.Count - (years.Count - 2)]).ToString(), "out of", item.HolidayEntitlement.ToString());
                    var fourthYear = string.Format("{0} {1} {2}", _employeeServices.GetHolidaysCountByEmployeeId(item.ResourceId, years[years.Count - (years.Count - 3)]).ToString(), "out of", item.HolidayEntitlement.ToString());
                    var fifthYear = string.Format("{0} {1} {2}", _employeeServices.GetHolidaysCountByEmployeeId(item.ResourceId, years[years.Count - (years.Count - 4)]).ToString(), "out of", item.HolidayEntitlement.ToString());
                    var sourceItem = new HolidayReportViewModel();
                    sourceItem.UserName = EmployeeName;
                    sourceItem.Date = JobStartDate;
                    sourceItem.FirstYear = firstYear;
                    sourceItem.SecondYear = secondYear;
                    sourceItem.ThirdYear = thirdYear;
                    sourceItem.FourthYear = fourthYear;
                    sourceItem.FifthYear = fifthYear;
                    dataSource.Add(sourceItem);
                }
                report.DataSource = dataSource;
            }
        }

        #endregion HolidayReportsPrint

        #region DailySaleZReport

        public ActionResult DailySaleZReport(int VanSaleCashId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            DailySalesZReport report = CreateDailySaleZReport(VanSaleCashId);
            //report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        public DailySalesZReport CreateDailySaleZReport(int VanSaleCashId)
        {
            DailySalesZReport dailySalesZReport = new DailySalesZReport();
            dailySalesZReport.ParamVanSalesCashId.Value = VanSaleCashId;
            dailySalesZReport.RequestParameters = true;
            return dailySalesZReport;
        }

        #endregion DailySaleZReport

        #region PurchaseOrderAuditReport

        public ActionResult PurchaseOrderAuditReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            PurchaseOrderAuditReport report = CreatePurchaseOrderAuditReport();
            //report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        public PurchaseOrderAuditReport CreatePurchaseOrderAuditReport()
        {
            PurchaseOrderAuditReport PurchaseOrderAuditReport = new PurchaseOrderAuditReport();
            //PurchaseOrderAuditReport.OrderNumber.Value = "PO-00018163";
            //PurchaseOrderAuditReport.RequestParameters = true;
            return PurchaseOrderAuditReport;
        }

        #endregion PurchaseOrderAuditReport

        #region ProductSaleReportByPalletType

        public ActionResult ProductSaleReportByPalletType()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ProductSoldByPalletType report = CreateProductSoldByPalletType();
            //report.xrLabel18.BeforePrint += XrLabel18_BeforePrint;
            return View(report);
        }

        public ProductSoldByPalletType CreateProductSoldByPalletType()
        {
            ProductSoldByPalletType productSoldByPalletType = new ProductSoldByPalletType();
            productSoldByPalletType.StartDate.Value = DateTime.Today.AddMonths(-1);
            productSoldByPalletType.EndDate.Value = DateTime.Today;
            productSoldByPalletType.lbldate.Text = DateTime.UtcNow.ToShortDateString();
            productSoldByPalletType.TenantId.Value = CurrentTenantId;
            productSoldByPalletType.warehouseId.Value = CurrentWarehouseId;

            IEnumerable<PalletType> PalletTypes = _lookupService.GetAllValidPalletTypes(CurrentTenantId);
            StaticListLookUpSettings setting = (StaticListLookUpSettings)productSoldByPalletType.PalletTypes.LookUpSettings;

            foreach (var item in PalletTypes)
            {
                LookUpValue PalletType = new LookUpValue();
                PalletType.Description = item.Description;
                PalletType.Value = item.PalletTypeId;
                setting.LookUpValues.Add(PalletType);
            }

            return productSoldByPalletType;
        }

        #endregion ProductSaleReportByPalletType
    }
}