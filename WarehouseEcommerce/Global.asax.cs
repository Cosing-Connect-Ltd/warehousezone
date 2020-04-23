using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WMS.App_Start;

namespace WarehouseEcommerce
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // remove unnecessary view engines
            ViewEngines.Engines.Clear();
            IViewEngine razorEngine = new ThemedRazorViewEngine() { FileExtensions = new string[] { "cshtml" } };
            ViewEngines.Engines.Add(razorEngine);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // force TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        }
    }
}
