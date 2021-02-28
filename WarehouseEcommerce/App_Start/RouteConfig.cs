using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WarehouseEcommerce
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "page/{pageUrl}/{blogDetail}",
                defaults: new { controller = "Home", action = "page", pageUrl = UrlParameter.Optional, blogDetail = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "GetStarted",
                url: "pages/get-started",
                defaults: new { controller = "Pages", action = "GetStarted" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
