using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using DevExpress.XtraScheduler;
using WMS.CustomBindings;
using DevExpress.Web.Mvc;
using System.Web.Hosting;

namespace WMS.Controllers.WebAPI
{
    public class ApiVanSalesController : BaseApiController
    {
        private readonly IMarketServices _marketServices;
        private readonly IVanSalesService _vanSalesService;
        private readonly IVehicleInspectionService _inspectionService;
        private readonly IEmployeeServices _employeeServices;
        private readonly IAccountServices _accountServices;
        private readonly ITransferOrderService _transferOrderService;
        private readonly ITenantsServices _tenantServices;
        private readonly IMapper _mapper;

        public ApiVanSalesController(ITerminalServices terminalServices,
            ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, IMarketServices marketServices, IVanSalesService vanSalesService, IVehicleInspectionService inspectionService, IEmployeeServices employeeServices,
            IAccountServices accountServices, ITransferOrderService transferOrderService, ITenantsServices tenantServices, IMapper mapper)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _marketServices = marketServices;
            _vanSalesService = vanSalesService;
            _inspectionService = inspectionService;
            _employeeServices = employeeServices;
            _accountServices = accountServices;
            _transferOrderService = transferOrderService;
            _tenantServices = tenantServices;
            _mapper = mapper;
        }

        // GET http://localhost:8005/api/sync/vehicles/{reqDate}/{serialNo}
        public IHttpActionResult GetAllVehicles(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new MarketVehiclesSyncCollection();

            var allVehicles = _marketServices.GetAllValidMarketVehicles(terminal.TenantId, reqDate, true).MarketVehicles;

            var results = new List<MarketVehiclesSync>();

            foreach (var p in allVehicles)
            {
                var sync = new MarketVehiclesSync();
                var mapped = _mapper.Map(p, sync);
                results.Add(mapped);
            }

            result.Count = results.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, results.Count, terminal.TerminalId,
                    TerminalLogTypeEnum.MarketVehiclesSync).TerminalLogId;
            result.Vehicles = results;
            return Ok(result);
        }

        // GET http://localhost:8005/api/sync/terminal-data/{serialNo}
        public IHttpActionResult GetTerminalMetadata(string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            TenantConfig config = _tenantServices.GetTenantConfigById(terminal.TenantId);

            string baseUrl = HostingEnvironment.ApplicationPhysicalPath;
            string ImageUrl = baseUrl + "Content/images/logo.png";
            byte[] data = System.IO.File.ReadAllBytes(ImageUrl);

            if (config.TenantLogo == null)
            {
                config.TenantLogo = data;

            }

            var result = new TerminalMetadataSync()
            {
                Serial = terminal.TermainlSerial,
                TerminalId = terminal.TerminalId,
                TerminalName = terminal.TerminalName,
                ParentWarehouseId = terminal.WarehouseId,
                ParentWarehouseName = terminal.TenantWarehous?.WarehouseName,
                TenantId = terminal.TenantId,
                PalletTrackingScheme = terminal.TenantWarehous.PalletTrackingScheme,
                PrintLogoForReceipts = config.PrintLogoForReceipts,
                TenantLogo = data,
                TenantReceiptPrintHeaderLine1 = config.TenantReceiptPrintHeaderLine1,
                TenantReceiptPrintHeaderLine2 = config.TenantReceiptPrintHeaderLine2,
                TenantReceiptPrintHeaderLine3 = config.TenantReceiptPrintHeaderLine3,
                TenantReceiptPrintHeaderLine4 = config.TenantReceiptPrintHeaderLine4,
                TenantReceiptPrintHeaderLine5 = config.TenantReceiptPrintHeaderLine5,
                GlobalProcessByPallet = terminal.TenantWarehous.EnableGlobalProcessByPallet,
                SessionTimeoutHours = config.SessionTimeoutHours,
                AllowStocktakeAddNew = terminal.TenantWarehous.AllowStocktakeAddNew,
                AllowStocktakeEdit = terminal.TenantWarehous.AllowStocktakeEdit,
                PostGeoLocation = terminal.PostGeoLocation,
                VehicleChecksAtStart = terminal.VehicleChecksAtStart,
                AllowSaleWithoutAccount = terminal.TenantWarehous.AllowSaleWithoutAccount,
                ShowFullBalanceOnPayment = terminal.TenantWarehous.ShowFullBalanceOnPayment,
                AllowExportDatabase = terminal.AllowExportDatabase,
                ShowCasePrices = terminal.ShowCasePrices,
                ScanVehicleLicensePlate = terminal.ScanVehicleLicensePlate,
                PickByContainer = terminal.TenantWarehous.PickByContainer,
                MandatoryPickByContainer = terminal.TenantWarehous.MandatoryPickByContainer,
                MandatoryLocationScan = config.MandatoryLocationScan
            };

            var mobileLocation = TerminalServices.GetMobileLocationByTerminalId(terminal.TerminalId);
            if (mobileLocation != null)
            {
                result.ParentWarehouseId = (int)mobileLocation.ParentWarehouseId;
                result.ParentWarehouseName = mobileLocation.ParentWarehouse?.WarehouseName;
                result.MobileWarehouseId = mobileLocation.WarehouseId;
                result.MobileWarehouseName = mobileLocation.WarehouseName;

                result.MarketVehicleId = mobileLocation.MarketVehicleID;
                result.VehicleRegistration = mobileLocation.MarketVehicle?.VehicleIdentifier;

                result.SalesManUserId = mobileLocation.SalesManUserId;
                result.SalesManResourceName = UserService.GetResourceNameByUserId(mobileLocation.SalesManUserId);
            }

            return Ok(result);
        }

        // GET http://localhost:8005/api/sync/vehicles/{serialNo}
        public IHttpActionResult GetAllMarketSchedules(string serialNo, DateTime reqDate)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }


            var result = new MarketRouteScheduleSyncCollection();

            var allRouteSchedules = _vanSalesService.GetAllMarketRouteSchedules(terminal.TenantId, reqDate);

            var results = new List<MarketRouteScheduleSync>();

            foreach (var p in allRouteSchedules)
            {
                var sync = new MarketRouteScheduleSync();
                var mapped = _mapper.Map(p, sync);
                results.Add(mapped);
            }

            result.Count = results.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, results.Count, terminal.TerminalId, TerminalLogTypeEnum.MarketRouteScheduleSync).TerminalLogId;
            result.MarketRouteSchedules = results;
            return Ok(result);
        }
        // GET http://localhost:8005/api/sync/vehicles/{serialNo}
        public IHttpActionResult GetAllMarketRoutes(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new MarketRouteSyncCollection();

            var allRoutes = _vanSalesService.GetAllMarketRoutes(terminal.TenantId, reqDate);

            var results = new List<MarketRouteSync>();

            foreach (var p in allRoutes)
            {
                var sync = new MarketRouteSync();
                var mapped = _mapper.Map(p, sync);
                results.Add(mapped);
            }
            if (results.Count > 1)
            {
                results = results.Take(1).ToList();
            }

            result.Count = results.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, results.Count, terminal.TerminalId, TerminalLogTypeEnum.MarketRoutesSync).TerminalLogId;
            result.MarketRoutes = results;
            return Ok(result);
        }

        public IHttpActionResult GetMyMarketRoutes(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new MarketRouteSyncCollection();

            var warehouseId = terminal.WarehouseId;

            var mobileLocation = TerminalServices.GetMobileLocationByTerminalId(terminal.TerminalId);

            if (mobileLocation != null)
            {
                warehouseId = mobileLocation.WarehouseId;
            }

            var allRoutes = _vanSalesService.GetMyMarketRoutes(terminal.TenantId, warehouseId, reqDate);

            var results = new List<MarketRouteSync>();

            foreach (var p in allRoutes)
            {
                DateTime nextAppointment = DateTime.MinValue;
                DateTime lastAppointment = DateTime.MinValue;
                RecurrenceInfo info = new RecurrenceInfo();
                var appt = _vanSalesService.GetMarketRouteScheduleById(p.RouteScheduleId);

                Appointment newAppt = DevExpress.XtraScheduler.Compatibility.StaticAppointmentFactory.CreateAppointment(AppointmentType.Pattern);
                newAppt.RecurrenceInfo.FromXml(appt.RecurrenceInfo);
                newAppt.Start = appt.StartTime;
                newAppt.End = appt.EndTime;

                if (appt.RecurrenceInfo != null)
                {
                    info.FromXml(appt.RecurrenceInfo);

                    OccurrenceCalculator calc = OccurrenceCalculator.CreateInstance(info);
                    nextAppointment = calc.FindNextOccurrenceTimeAfter(new DateTime(reqDate.Year, reqDate.Month, reqDate.Day, 0, 0, 0), newAppt);

                    if (!newAppt.SameDay && nextAppointment.Date != reqDate.Date)
                    {
                        var reqDate2 = reqDate.AddDays(-1);
                        nextAppointment = calc.FindNextOccurrenceTimeAfter(new DateTime(reqDate2.Year, reqDate2.Month, reqDate2.Day, 0, 0, 0), newAppt);
                    }
                }

                else
                {
                    nextAppointment = newAppt.Start;

                    if (!newAppt.SameDay && nextAppointment.Date != reqDate.Date)
                    {
                        nextAppointment = newAppt.Start.AddDays(1);
                    }
                }

                if (reqDate.Date == nextAppointment.Date && appt.WarehouseId == warehouseId || (!newAppt.SameDay && reqDate.Date == nextAppointment.Date.AddDays(1)))
                {
                    var sync = new MarketRouteSync();
                    var mapped = _mapper.Map(p, sync);
                    results.Add(mapped);
                }
            }

            result.Count = results.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, results.Count, terminal.TerminalId, TerminalLogTypeEnum.DayMarketRouteSync).TerminalLogId;
            result.MarketRoutes = results;
            return Ok(result);
        }

        public IHttpActionResult GetAllVehicleCheckLists(string serialNo, DateTime reqDate)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new VehiclesInspectionChecklistSyncCollection
            {
                Vehicles = _vanSalesService.GetAllValidVehicleInspectionCheckList(terminal.TenantId, reqDate, true)
                    .Select(_mapper.Map<InspectionCheckListViewModel, VehicleInspectionChecklistSync>)
                    .ToList()
            };
            result.Count = result.Vehicles.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, result.Vehicles.Count, terminal.TerminalId, TerminalLogTypeEnum.VehicleCheckLists).TerminalLogId;
            return Ok(result);
        }

        public IHttpActionResult SaveInspectionReport(string serialNo, VehicleInspectionReportSync model)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(model.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            var inspection = _mapper.Map(model, new VehicleInspection());
            inspection.TenantId = terminal.TenantId;
            inspection = _inspectionService.SaveInspection(inspection, model.CurrentUserId, model.CheckedInspectionIds, true);
            var result = _mapper.Map(inspection, model);
            return Ok(result);
        }

        public IHttpActionResult CreateHolidayRequest(HolidayRequestSync model)
        {
            var serialNumber = model.SerialNumber.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNumber);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(model.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            var result = _employeeServices.AddResourceHolidaySync(model, terminal.TenantId, model.UserId);
            return Ok(result);
        }

        //http://ganetest.qsrtime.net/api/sync/my-holidays
        public IHttpActionResult GetUserHolidayRequests(string serialNo, DateTime reqDate, int userId)
        {
            var serialNumber = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNumber);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new HolidaySyncCollection();

            result.HolidayResponseSync = _employeeServices.GetUserHolidays(userId, reqDate, true);

            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, result.HolidayResponseSync.Count(), terminal.TerminalId, TerminalLogTypeEnum.HolidaySync).TerminalLogId;
            result.Count = result.HolidayResponseSync.Count();

            return Ok(result);
        }

        public IHttpActionResult VanSalesDailyReport(VanSalesDailyCashSync model)
        {
            var serialNumber = model.SerialNumber.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNumber);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(model.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            model.TenantId = terminal.TenantId;


            var result = _vanSalesService.SaveVanSalesDailyReport(model);

            var terminalLog = TerminalServices.CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, 0, terminal.TerminalId, TerminalLogTypeEnum.VanSalesDailyReport).TerminalLogId;

            var report = _mapper.Map<VanSalesDailyCash, VanSalesDailyCashSync>(result);
            report.UserId = model.UserId;
            report.SerialNumber = model.SerialNumber;
            report.TerminalId = model.TerminalId;
            return Ok(report);
        }


        //http://ganetest.qsrtime.net/api/sync/add-transaction-file
        public IHttpActionResult AddAccountTransactionFileSync(AccountTransactionFileSync model)
        {
            var serialNumber = model.SerialNumber.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNumber);

            if (terminal == null)
            {
                return Unauthorized();
            }
            var result = _accountServices.AddAccountTransactionFile(model, terminal.TenantId);
            return Ok(result);
        }

        //http://ganetest.qsrtime.net/api/sync/van-sales-stocking/{tenantid}/{reqDate}
        [HttpGet]
        public IHttpActionResult TransferReplenishmentsForVans(int tenantId, DateTime reqDate)
        {
            var results = _transferOrderService.AutoTransferOrdersForMobileLocations(tenantId);

            return Ok(results);
        }




    }
}