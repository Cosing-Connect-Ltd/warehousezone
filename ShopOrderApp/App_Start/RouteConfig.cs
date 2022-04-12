using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShopOrderApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "vechile",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "user", action = "VechileIdentifer", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "load-truck",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "unload-truck",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "UnloadTruck", id = UrlParameter.Optional }
            );
        }
    }
}
