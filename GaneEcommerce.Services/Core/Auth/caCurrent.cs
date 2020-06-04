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
            caTenantWebsites tenantWebsite = new caTenantWebsites();
            string hostName = HttpContext.Current.Request.Url.Authority;

            if (HttpContext.Current.Session["CurrentTenantWebsites"] == null)
            {
                var tenantWeb = context.TenantWebsites.FirstOrDefault(e => e.HostName == hostName && e.IsActive == true && e.IsDeleted != true);
                if (tenantWeb != null)
                {
                    tenantWebsite.SiteID = tenantWeb.SiteID;
                    tenantWebsite.TenantId = tenantWeb.TenantId;
                    tenantWebsite.DefaultWarehouseId = tenantWeb.DefaultWarehouseId;
                    tenantWebsite.SiteName = tenantWeb.SiteName;
                    tenantWebsite.SiteDescription = tenantWeb.SiteDescription;
                    tenantWebsite.Theme = tenantWeb.Theme;
                    tenantWebsite.Logo = tenantWeb.Logo;
                    tenantWebsite.WebsiteContactAddress = tenantWeb.WebsiteContactAddress;
                    tenantWebsite.WebsiteContactEmail = tenantWeb.WebsiteContactEmail;
                    tenantWebsite.WebsiteContactPhone = tenantWeb.WebsiteContactPhone;
                    tenantWebsite.FacebookUrl = tenantWeb.FacebookUrl;
                    tenantWebsite.InstaGramUrl = tenantWeb.InstaGramUrl;
                    tenantWebsite.LinkedInUrl = tenantWeb.LinkedInUrl;
                    tenantWebsite.YoutubeUrl = tenantWeb.YoutubeUrl;
                    tenantWebsite.TwitterUrl = tenantWeb.TwitterUrl;
                    tenantWebsite.FooterText = tenantWeb.FooterText;
                    tenantWebsite.HostName = tenantWeb.HostName;
                    tenantWebsite.BaseFilePath = tenantWeb.BaseFilePath;
                    HttpContext.Current.Session["CurrentTenantWebsites"] = tenantWebsite;
                    HttpContext.Current.Session["caErrors"] = null;
                }
                else
                {
                    caError error = new caError();
                    error.ErrorTtile = "Host name Issue";
                    error.ErrorMessage = "Sorry, system is unable to validate client";
                    error.ErrorDetail = "Either client is not registered, inactive or ambiguous, please contact support";
                    HttpContext.Current.Session["caErrors"] = error;

                }
            }
            else
            {
                tenantWebsite = (caTenantWebsites)HttpContext.Current.Session["CurrentTenantWebsites"];
            }
            return tenantWebsite;
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