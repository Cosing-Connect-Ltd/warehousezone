using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WarehouseEcommerce.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ICoreOrderService _orderService;
        protected readonly IPropertyService _propertyService;
        protected readonly IAccountServices _accountServices;
        protected readonly ILookupServices _lookupServices;
        protected readonly ITenantWebsiteService _tenantWebsiteServices;
        private readonly ITenantsCurrencyRateServices _tenantsCurrencyRateServices;
        public static string NoImage = "/UploadedFiles/Products/Products/no-image.png";
        public static string uploadedProductCategoryfilePath = "/UploadedFiles/ProductCategory/";

        public BaseController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices,
            ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices, ITenantWebsiteService tenantWebsiteService)
        {
            _tenantsCurrencyRateServices = tenantsCurrencyRateServices;
            _orderService = orderService;
            _propertyService = propertyService;
            _accountServices = accountServices;
            _lookupServices = lookupServices;
            _tenantWebsiteServices = tenantWebsiteService;

            var res = CurrentTenantWebsite;
        }

        private caTenantWebsites _CurrentTenantWebsite { get; set; }

        protected caTenantWebsites CurrentTenantWebsite
        {
            get
            {
                if (_CurrentTenantWebsite != null) return _CurrentTenantWebsite;

                _CurrentTenantWebsite = caCurrent.CurrentTenantWebSite();

                return _CurrentTenantWebsite;
            }
            set { _CurrentTenantWebsite = value; }
        }

        private caUser _CurrentUser { get; set; }

        protected caUser CurrentUser
        {
            get
            {
                if (_CurrentUser != null) return _CurrentUser;

                _CurrentUser = caCurrent.CurrentWebsiteUser();
                return _CurrentUser;
            }
            set { _CurrentUser = value; }
        }

        public int CurrentUserId
        {
            get { return CurrentUser.UserId; }
        }

        public int CurrentTenantId
        {
            get { return _CurrentTenantWebsite.TenantId; }
        }
        public int CurrentWarehouseId
        {
            get { return _CurrentTenantWebsite.DefaultWarehouseId; }
        }

        protected void PrepareDirectory(string virtualDirPath)
        {
            if (!Directory.Exists(Server.MapPath(virtualDirPath)))
            {
                Directory.CreateDirectory(Server.MapPath(virtualDirPath));
            }
        }

        public string GenerateNextOrderNumber(InventoryTransactionTypeEnum type)
        {
            return _orderService.GenerateNextOrderNumber(type, CurrentTenantId);
        }
        public string GenerateNextOrderNumber(string type)
        {
            switch (type)
            {
                case "PO":
                    return GenerateNextOrderNumber(InventoryTransactionTypeEnum.PurchaseOrder);
                case "SO":
                    return GenerateNextOrderNumber(InventoryTransactionTypeEnum.SalesOrder);
                case "WO":
                    return GenerateNextOrderNumber(InventoryTransactionTypeEnum.WorksOrder);
                case "TO":
                    return GenerateNextOrderNumber(InventoryTransactionTypeEnum.TransferIn);
            }
            return GenerateNextOrderNumber(InventoryTransactionTypeEnum.WorksOrder);
        }

        public ActionResult ErrorPage()
        {
            return RedirectToAction("Index", "Error");
        }

        public DayOfWeek GetWeekDay(DateTime date)
        {
            CultureInfo cInfo = CultureInfo.CurrentCulture;

            return cInfo.Calendar.GetDayOfWeek(date);
        }

        public int GetWeekNumber(DateTime date)
        {
            CultureInfo cInfo = CultureInfo.CurrentCulture;

            return cInfo.Calendar.GetWeekOfYear(date, cInfo.DateTimeFormat.CalendarWeekRule, cInfo.DateTimeFormat.FirstDayOfWeek);
        }

        public int GetWeekNumber()
        {
            CultureInfo cInfo = CultureInfo.CurrentCulture;

            return cInfo.Calendar.GetWeekOfYear(DateTime.UtcNow, cInfo.DateTimeFormat.CalendarWeekRule, cInfo.DateTimeFormat.FirstDayOfWeek);
        }

        public DateTime GetDateFromWeekNumberAndDayOfWeek(int weekNumber, int year, int dayOfWeek)
        {
            // current culture info
            CultureInfo cInfo = CultureInfo.CurrentCulture;

            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)cInfo.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);

            int firstWeek = cInfo.Calendar.GetWeekOfYear(jan1, cInfo.DateTimeFormat.CalendarWeekRule, cInfo.DateTimeFormat.FirstDayOfWeek);

            var weekNum = weekNumber;

            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
            {
                weekNum -= 1;
            }

            int noToAdd = dayOfWeek;
            if (dayOfWeek == 0) noToAdd = 7;

            var result = firstMonday.AddDays(weekNum * 7 + noToAdd - 1);
            return result;
        }

        public List<DateTime> GetWeekDatesList(int week, int year)
        {
            List<DateTime> weekDates = new List<DateTime>();
            int[] weekdaysEnumIds = { 1, 2, 3, 4, 5, 6, 0 };

            foreach (var i in weekdaysEnumIds)
            {
                weekDates.Add(GetDateFromWeekNumberAndDayOfWeek(week, year, i));
            }

            return weekDates;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var website = _tenantWebsiteServices.GetTenantWebSiteBySiteId(CurrentTenantWebsite.SiteID);
            ViewBag.SiteId = CurrentTenantWebsite.SiteID;
            ViewBag.TimeZone = GetCurrentTimeZone();
            ViewBag.LoginDetail = CurrentUserId > 0 ? "Logout" : "Login";
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.Currencies = _lookupServices.GetAllGlobalCurrencies();
            ViewBag.TenantWebsite = website;
            ViewBag.BaseFilePath = website.BaseFilePath;
            ViewBag.BasePath = Request.Url.Scheme + "://" + website.HostName;
            if (Session["CurrencyDetail"] == null)
            {
                CurrencyDetail(null);
            }

            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;



            var queryString = Request.QueryString["fragment"];
            if (queryString != null && queryString != string.Empty)
            {
                ViewBag.Fragment = queryString;
            }
            GetItemsDetail();
            base.OnActionExecuting(filterContext);
        }

        public string GetCurrentTimeZone()
        {
            var user = CurrentUser;
            var Tenantwebiste = CurrentTenantWebsite;
            string timeZone = "GMT Standard Time";



            if (user != null && !string.IsNullOrEmpty(user.UserTimeZoneId))
            {
                timeZone = user.UserTimeZoneId;
            }

            return timeZone;

        }

        public void VerifyOrderStatus(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order.OrderStatusID == OrderStatusEnum.Active)
            {
                ViewBag.PreventProcessing = false;

            }
            else
            {
                ViewBag.PreventProcessing = true;
                ViewBag.Error = "This order is on hold and cannot be processed";
            }


        }


        public string GetPathAgainstProductId(int productId, bool status = false)
        {
            string paths = "";
            var _productServices = DependencyResolver.Current.GetService<IProductServices>();
            //string[] imageFormats = ConfigurationManager.AppSettings["ImageFormats"].Split(new char[] { ',' });
            //bool defaultimage = false; bool hover = false;
            //if (status) { defaultimage = true; } else { hover = true; };

            //var path = _productServices.GetProductFiles(productId, tenantId, defaultimage: defaultimage, hover: hover).ToList();
            //if (path.Count > 0)
            //{

            //    var data = from files in path.Where(a => imageFormats.Contains(new DirectoryInfo(a.FilePath).Extension, StringComparer.CurrentCultureIgnoreCase))
            //               select new
            //               {
            //                   FileUrl = files.FilePath

            //               };
            //    if (data != null)
            //    {
            //        paths = data.FirstOrDefault()?.FileUrl;
            //    }
            //    else
            //    {
            //        paths = NoImage;
            //    }


            //}

            return paths;
        }


        public void CurrencyDetail(int? CurrencyId)
        {

            if (CurrencyId.HasValue)
            {
                var detail = _lookupServices.GetAllGlobalCurrencies().Where(c => (!CurrencyId.HasValue || c.CurrencyID == CurrencyId)).Select(u => new caCurrencyDetail
                {

                    Symbol = u.Symbol,
                    Id = u.CurrencyID,
                    CurrencyName = u.CurrencyName

                }).ToList();
                var getTenantCurrencies = _tenantsCurrencyRateServices.GetTenantCurrencies(CurrentTenantId).FirstOrDefault(u => u.CurrencyID == detail.FirstOrDefault()?.Id);
                detail.ForEach(c =>
                    c.Rate = _tenantsCurrencyRateServices.GetCurrencyRateByTenantid(getTenantCurrencies.TenantCurrencyID)
                );
                Session["CurrencyDetail"] = detail.FirstOrDefault();
            }

            else
            {
                if (Session["CurrencyDetail"] == null)
                {
                    CurrencyId = 1;

                    var detail = _lookupServices.GetAllGlobalCurrencies().Where(c => (!CurrencyId.HasValue || c.CurrencyID == CurrencyId)).Select(u => new caCurrencyDetail
                    {

                        Symbol = u.Symbol,
                        Id = u.CurrencyID,
                        CurrencyName = u.CurrencyName

                    }).ToList();
                    var getTenantCurrencies = _tenantsCurrencyRateServices.GetTenantCurrencies(CurrentTenantId).FirstOrDefault(u => u.CurrencyID == detail.FirstOrDefault()?.Id);
                    detail.ForEach(c =>
                        c.Rate = _tenantsCurrencyRateServices.GetCurrencyRateByTenantid(getTenantCurrencies?.TenantCurrencyID ?? 0)
                    );
                    Session["CurrencyDetail"] = detail.FirstOrDefault();
                }

            }


        }

        public void GetItemsDetail()
        {

            var _tenantWebsiteService = DependencyResolver.Current.GetService<ITenantWebsiteService>();
            var cartItems = _tenantWebsiteService.GetAllValidCartItemsList(CurrentTenantWebsite.SiteID, CurrentUserId,
                HttpContext.Session.SessionID);
            ViewBag.CartItemCount = cartItems.Count();
            ViewBag.CartItems = cartItems.ToList();
            ViewBag.WishListItemCount = _tenantWebsiteService.GetAllValidWishListItemsCount(CurrentTenantWebsite.SiteID, CurrentUserId);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
    }
}