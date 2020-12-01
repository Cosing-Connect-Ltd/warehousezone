using eSpares.Levity;
using Ganedata.Core.Barcoding;
using Ganedata.Core.Data;
using Ganedata.Core.Services;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        public HomeController(ITenantWebsiteService tenantWebsiteService)
        {
            _tenantWebsiteService = tenantWebsiteService;
        }
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of current tenant
            caTenant tenant = caCurrent.CurrentTenant();

            //get properties of user
            caUser user = caCurrent.CurrentUser();

            ViewData["Ten"] = user.AuthPermissions;
            ViewData["user"] = user;
            ViewData["custom"] = "Welcome  <b>" + user.UserName + "</b>";

            var tick = DateTime.UtcNow.Ticks;
            var fileTime = DateTime.UtcNow.ToFileTime();
            var mid = Environment.MachineName.ToString();

            var rem = Request.UserHostName;



            return View();
        }

        public ActionResult About()
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            ViewBag.Message = "";

            Assembly assembly = ApplicationAssemblyUtility.ApplicationAssembly;
            Version version = assembly.GetName().Version;
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string fileVersion = fvi.FileVersion;

            ViewBag.AppVersion = version.ToString();
            ViewBag.FileVersion = fileVersion;

            return View();
        }

        public ActionResult Contact()
        {
            var barcode = new GS128Decoder();
            var barcodeResult = barcode.GS128Decode("(01)05021306320031(10)00180266(99)0210218(98)0188");

            var res = barcodeResult.GTIN + " : " + barcodeResult.Status;

            var res2 = barcodeResult.LotNumber;

            var res3 = barcode.GS128DecodeByType("(01)05021306320031(10)00180266(99)0210218(98)0188", GS128DecodeType.GTIN);

            var res4 = barcode.GS128DecodeGTINOrDefault("(01)05021306320031(10)00180266(99)0210218(98)0188");

            var res5 = barcode.GS128DecodeGTINOrDefault("05021306320031");

            //ViewBag.Error = res6 + "---" + res7 + "---" + res8;

            //ViewBag.Error = TimeZoneInfo.GetSystemTimeZones().ToString();

            return View();
        }

        public ActionResult Preload()
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            context.Account.Where(x => x.IsDeleted != true).Take(10).ToList();
            context.ProductMaster.Where(x => x.IsDeleted != true).Take(10).ToList();
            context.Order.Where(x => x.IsDeleted != true).Take(10).ToList();
            context.InventoryTransactions.Where(x => x.IsDeleted != true).Take(10).ToList();
            context.PalletTracking.Where(x => x.RemainingCases != 0).Take(10).ToList();
            context.Appointments.Where(x => x.IsCanceled != true).Take(10).ToList();
            context.AuthPermissions.Where(x => x.IsDeleted != true).Take(10).ToList();
            context.AuthActivities.Where(x => x.IsDeleted != true).Take(10).ToList();
            context.AuthActivityGroupMaps.Where(x => x.IsDeleted != true).Take(10).ToList();

            var warmUpUrl = ConfigurationManager.AppSettings["WarmUpUrl"];
            var cli = new WebClient();
            string data = cli.DownloadString(warmUpUrl);

            return View();
        }

        public ActionResult page(string pageUrl)
        {
            var content = _tenantWebsiteService.GetWebsiteContentByUrl(null, pageUrl);
            if (string.IsNullOrEmpty(pageUrl) || content == null) { RedirectToAction("Index"); }

            return View(content);
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