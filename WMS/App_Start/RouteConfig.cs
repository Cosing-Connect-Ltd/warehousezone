using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            routes.MapRoute(
               name: "Admin_elmah",
               url: "Admin/ErrorLog/{type}",
               defaults: new { action = "Index", controller = "ErrorLog", type = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "ProductCreate",
                url: "Products/Create/{id}",
                defaults: new { action = "Create", controller = "Products", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "MarketCreate",
                url: "Markets/Create",
                defaults: new { action = "Edit", controller = "Markets", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "VehicleCreate",
                url: "Markets/Vehicles/Create",
                defaults: new { action = "Edit", controller = "MarketsVehicle", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "VehicleInspectionCreate",
                url: "VehicleInspection/Edit/{id}/{vehicleid}",
                defaults: new { action = "Edit", controller = "VehicleInspection", id = UrlParameter.Optional, vehicleid = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PalletsById",
                url: "pallets/account/pallet/{id}",
                defaults: new { action = "PalletEditor", controller = "Pallets", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "PalletsByAccount",
                url: "pallets/account/{accountid}",
                defaults: new { action = "PalletEditor", controller = "Pallets", accountid = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "PalletsByOrderProcess",
               url: "pallets/orderProcess/{OrderProcessId}",
               defaults: new { action = "PalletEditor", controller = "Pallets", OrderProcessId = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "WarehouseStockRange",
                url: "warehouse/stock-levels/{warehouseId}",
                defaults: new { action = "EditStockLevels", controller = "TenantLocations", warehouseId = UrlParameter.Optional }
            );
            routes.MapRoute(
                "UpdateOrderStatus",                                              // Route name
                "Order/UpdateOrderStatus/{orderId}/{statusId}",                           // URL with parameters
                new { controller = "Order", action = "UpdateOrderStatus", orderId = UrlParameter.Optional, statusId = UrlParameter.Optional }  // Parameter defaults
            );
            routes.MapRoute(
                "OrdersDefault",
                "{controller}/AnchoredOrderIndex/{id}",
                new { controller = "BaseController", action = "AnchoredOrderIndex", id = UrlParameter.Optional }, new { controller = new NotEqual("ErrorLog") }
            );
            routes.MapRoute(
                "OrderComplete",                                              // Route name
                "Order/Complete/{action}/{id}/{frag}",                           // URL with parameters
                new { controller = "Order", action = "Complete", id = UrlParameter.Optional, frag = UrlParameter.Optional }  // Parameter defaults
            );   
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new { controller = new NotEqual("ErrorLog") }
            );

        }
    }

    public class NotEqual : IRouteConstraint
    {
        private string _match = String.Empty;

        public NotEqual(string match)
        {
            _match = match;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return String.Compare(values[parameterName].ToString(), _match, true) != 0;
        }
    }
}
