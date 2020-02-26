using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    public class caCurrent
    {
        public static caTenant CurrentTenant()
        {
            caTenant tenant = new caTenant();

            if (HttpContext.Current.Session["caTenant"] != null)
            {
                tenant = (caTenant)HttpContext.Current.Session["caTenant"];
            }
            return tenant;
        }

        public static caUser CurrentUser()
        {
            caUser User = new caUser();

            if (HttpContext.Current.Session["caUser"] != null)
            {
                // get properties of tenant
                User = (caUser)HttpContext.Current.Session["caUser"];
            }

            return User;
        }

        public static TenantLocations CurrentWarehouse()
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            int CurrentWarehouseId;
            TenantLocations Warehouse = new TenantLocations();

            if (HttpContext.Current.Session["CurrentWarehouseId"] != null)
            {
                CurrentWarehouseId = (int)HttpContext.Current.Session["CurrentWarehouseId"];
                Warehouse = context.TenantWarehouses.FirstOrDefault(e => e.WarehouseId == CurrentWarehouseId && e.IsDeleted != true);
            }
            return Warehouse;
        }

        public static caTenantWebsites CurrentTenantWebSite()
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            int currentSiteId = int.Parse(string.IsNullOrEmpty(ConfigurationManager.AppSettings["SiteId"].ToString()) ? "0" : ConfigurationManager.AppSettings["SiteId"]);
            caTenantWebsites tenantWebsites = new caTenantWebsites();

            if (HttpContext.Current.Session["CurrentTenantWebsites"] == null)
            {
                var tenantWeb = context.TenantWebsites.FirstOrDefault(e => e.SiteID == currentSiteId && e.IsDeleted != true);
                if (tenantWeb != null)
                {
                    tenantWebsites.ApiToken = tenantWeb.ApiToken;
                    tenantWebsites.SiteApiUrl = tenantWeb.SiteApiUrl;
                    tenantWebsites.TenantId = tenantWeb.TenantId;
                    tenantWebsites.WarehouseId = tenantWeb.WarehouseId;
                    tenantWebsites.SiteName = tenantWeb.SiteName;
                    tenantWebsites.SiteDescription = tenantWeb.SiteDescription;
                    HttpContext.Current.Session["CurrentTenantWebsites"] = tenantWebsites;
                }
            }
            else
            {
                tenantWebsites = (caTenantWebsites)HttpContext.Current.Session["CurrentTenantWebsites"];
            }
            return tenantWebsites;
        }
        public static caUser CurrentWebsiteUser()
        {
            caUser User = new caUser();

            if (HttpContext.Current.Session["cawebsiteUser"] != null)
            {  // get properties of tenant
                User = (caUser)HttpContext.Current.Session["cawebsiteUser"];
            }


            return User;
        }
    }
}