using System.Web.Mvc;

namespace WarehouseEcommerce.Areas.Shop
{
    public class ShopAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Shop";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Shop_default",
                "Shop/{controller}/{action}/{id}",
                new { Controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WarehouseEcommerce.Areas.Shop.Controllers" }
            );
        }
    }
}