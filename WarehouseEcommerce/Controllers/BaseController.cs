using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.Models;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WarehouseEcommerce.Helpers;

namespace WarehouseEcommerce.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ICoreOrderService OrderService;
        protected readonly IPropertyService PropertyService;
        protected readonly IAccountServices AccountServices;
        protected readonly ILookupServices LookupServices;
        private readonly ITenantsCurrencyRateServices tenantsCurrencyRateServices;
        public static string NoImage = "/UploadedFiles/Products/Products/no-image.png";
        public static string uploadedProductCategoryfilePath = "/UploadedFiles/ProductCategory/";

        public BaseController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsService)
        {
            tenantsCurrencyRateServices = tenantsService;
            OrderService = orderService;
            PropertyService = propertyService;
            AccountServices = accountServices;
            LookupServices = lookupServices;

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
            get { return _CurrentTenantWebsite.WarehouseId; }
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
            return OrderService.GenerateNextOrderNumber(type, CurrentTenantId);
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
            ViewBag.TimeZone = GetCurrentTimeZone();
            ViewBag.BaseFilePath = ConfigurationManager.AppSettings["BaseFilePath"];
            ViewBag.BasePath = ConfigurationManager.AppSettings["BasePath"];
            ViewBag.LoginDetail = CurrentUserId > 0 ? "Logout" : "Login";
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.Currencies = LookupServices.GetAllGlobalCurrencies();
            ViewBag.TenantWebsite = CurrentTenantWebsite;
            if (Session["CurrencyDetail"] == null)
            {
                CurrencyDetail(null);
            }



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
            var order = OrderService.GetOrderById(orderId);
            if (order.OrderStatusID == (int)OrderStatusEnum.Active)
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
                var detail = LookupServices.GetAllGlobalCurrencies().Where(c => (!CurrencyId.HasValue || c.CurrencyID == CurrencyId)).Select(u => new caCurrencyDetail
                {

                    Symbol = u.Symbol,
                    Id = u.CurrencyID,
                    CurrencyName = u.CurrencyName

                }).ToList();
                var getTenantCurrencies = tenantsCurrencyRateServices.GetTenantCurrencies(CurrentTenantId).FirstOrDefault(u => u.CurrencyID == detail.FirstOrDefault()?.Id);
                detail.ForEach(c =>
                    c.Rate = tenantsCurrencyRateServices.GetCurrencyRateByTenantid(getTenantCurrencies.TenantCurrencyID)
                );
                Session["CurrencyDetail"] = detail.FirstOrDefault();
            }

            else
            {
                if (Session["CurrencyDetail"] == null)
                {
                    CurrencyId = 1;

                    var detail = LookupServices.GetAllGlobalCurrencies().Where(c => (!CurrencyId.HasValue || c.CurrencyID == CurrencyId)).Select(u => new caCurrencyDetail
                    {

                        Symbol = u.Symbol,
                        Id = u.CurrencyID,
                        CurrencyName = u.CurrencyName

                    }).ToList();
                    var getTenantCurrencies = tenantsCurrencyRateServices.GetTenantCurrencies(CurrentTenantId).FirstOrDefault(u => u.CurrencyID == detail.FirstOrDefault()?.Id);
                    detail.ForEach(c =>
                        c.Rate = tenantsCurrencyRateServices.GetCurrencyRateByTenantid(getTenantCurrencies?.TenantCurrencyID ?? 0)
                    );
                    Session["CurrencyDetail"] = detail.FirstOrDefault();
                }

            }


        }

        public void GetItemsDetail()
        {
            if (CurrentUserId > 0)
            {
                var _tenantWebsiteService = DependencyResolver.Current.GetService<ITenantWebsiteService>();
                ViewBag.CartItemCount = _tenantWebsiteService.GetAllValidCartItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).Count();
                ViewBag.CartItems = _tenantWebsiteService.GetAllValidCartItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).ToList();
                ViewBag.WishListItemCount = _tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).Count();
            }
            else
            {
                ViewBag.CartItemCount = GaneCartItemsSessionHelper.GetCartItemsSession().Count;
                ViewBag.CartItems = GaneCartItemsSessionHelper.GetCartItemsSession().ToList();
                ViewBag.WishListItemCount = GaneWishListItemsSessionHelper.GetWishListItemsSession().Count();
            }
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