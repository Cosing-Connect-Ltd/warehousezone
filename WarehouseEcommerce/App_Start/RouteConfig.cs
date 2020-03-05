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
            name: "CourierIntegration",
            url: "courier-integration",
            defaults: new { controller = "Home", action = "CourierIntegration" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "Services",
            url: "services",
            defaults: new { controller = "Home", action = "Services" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "GetStarted",
            url: "get-started",
            defaults: new { controller = "Home", action = "GetStarted" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "Blog",
            url: "Blog",
            defaults: new { controller = "Home", action = "Blog" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "AboutUs",
            url: "about-us",
            defaults: new { controller = "Home", action = "AboutUs" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "Promotions",
            url: "Promotions",
            defaults: new { controller = "Home", action = "Promotions" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "PrivacyPolicy",
            url: "privacy-policy",
            defaults: new { controller = "Home", action = "PrivacyPolicy" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "WarehouseManagement",
            url: "warehouse-management-system",
            defaults: new { controller = "Home", action = "WarehouseManagement" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "OrderManagement",
            url: "order-management-system",
            defaults: new { controller = "Home", action = "OrderManagement" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "StockControl",
            url: "stock-control-system",
            defaults: new { controller = "Home", action = "StockControl" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "Epod",
            url: "epod-system",
            defaults: new { controller = "Home", action = "Epod" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "FleetTrackingDriverManagement",
            url: "fleet-management-system",
            defaults: new { controller = "Home", action = "FleetTrackingDriverManagement" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "YardManagement",
            url: "yard-management",
            defaults: new { controller = "Home", action = "YardManagement" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "Ecommerce",
            url: "ecommerce",
            defaults: new { controller = "Home", action = "Ecommerce" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "EposSelfService",
            url: "epos-self-service",
            defaults: new { controller = "Home", action = "EposSelfService" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "MobileVanSales",
            url: "mobile-van-sales",
            defaults: new { controller = "Home", action = "MobileVanSales" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "RealTimeLocationSystem",
            url: "real-time-location-system",
            defaults: new { controller = "Home", action = "RealTimeLocationSystem" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "HumanResources",
            url: "HR-time-and-attendance",
            defaults: new { controller = "Home", action = "HumanResources" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "MobileDeviceManagement",
            url: "mobile-device-management",
            defaults: new { controller = "Home", action = "MobileDeviceManagement" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
           name: "DigitalSignage",
           url: "digital-signage",
           defaults: new { controller = "Home", action = "DigitalSignage" },
           namespaces: new[] { "WarehouseEcommerce.Controllers" }
           );

            routes.MapRoute(
            name: "BusinessIntelligence",
            url: "shoppertrak-business-intelligence",
            defaults: new { controller = "Home", action = "BusinessIntelligence" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "LossPrevention",
            url: "loss-prevention",
            defaults: new { controller = "Home", action = "LossPrevention" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "RetailSolutions",
            url: "truevue-retail-rfid-solution",
            defaults: new { controller = "Home", action = "RetailSolutions" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "News",
            url: "news",
            defaults: new { controller = "Home", action = "News" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
            name: "DataProtection",
            url: "data-protection",
            defaults: new { controller = "Home", action = "DataProtection" },
            namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WarehouseEcommerce.Controllers" }
            );

            

        }
    }
}
