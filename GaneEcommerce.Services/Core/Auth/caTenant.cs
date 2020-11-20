using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    [Serializable]
    public class caTenant : caTenantBase
    {
        public bool AuthorizeTenant(Uri url)
        {
            IApplicationContext context = DependencyResolver.Current.GetService<IApplicationContext>();

            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;

            string TenantSubDomain = FilterSubDomain(url);
            if (string.IsNullOrWhiteSpace(TenantSubDomain) || TenantSubDomain == "ganedev") { TenantSubDomain = "ganedev"; }

            var Tenants = context.Tenants.AsNoTracking().Where(e => e.TenantSubDmoain.Contains(TenantSubDomain.Trim().ToLower()) && e.IsActive == true && e.IsDeleted != true)
                .Include(x => x.TenantLocations)
                .Include(x => x.TenantModules)
                .ToList();

            if (Tenants.Any() && Tenants.Count() < 2)
            {
                var tenant = Tenants.FirstOrDefault();
                TenantId = tenant.TenantId;
                TenantName = tenant.TenantName;
                TenantNo = tenant.TenantNo;
                PalletingEnabled = context.TenantConfigs?.FirstOrDefault(m => m.TenantId == tenant.TenantId)?.EnablePalletingOnPick == true;
                TenantVatNo = tenant.TenantVatNo;
                TenantAccountReference = tenant.TenantAccountReference;
                TenantWebsite = tenant.TenantWebsite;
                TenantDayPhone = tenant.TenantDayPhone;
                TenantEveningPhone = tenant.TenantEveningPhone;
                TenantMobilePhone = tenant.TenantMobilePhone;
                TenantFax = tenant.TenantFax;
                TenantEmail = tenant.TenantEmail;
                TenantAddress1 = tenant.TenantAddress1;
                TenantAddress2 = tenant.TenantAddress2;
                TenantAddress3 = tenant.TenantAddress3;
                TenantCity = tenant.TenantCity;
                TenantStateCounty = tenant.TenantStateCounty;
                TenantPostalCode = tenant.TenantPostalCode;

                CountryID = tenant.CountryID;
                TenantSubDmoain = tenant.TenantSubDmoain;
                ProductCodePrefix = tenant.ProductCodePrefix;
                DateCreated = tenant.DateCreated;
                DateUpdated = tenant.DateUpdated;
                CreatedBy = tenant.CreatedBy;
                UpdatedBy = tenant.UpdatedBy;
                TenantLocations = tenant.TenantLocations;
                TenantCulture = tenant.TenantCulture;
                TenantTimeZoneId = tenant.TenantTimeZoneId;
                TenantModules = tenant.TenantModules;
                AuthStatus = true;
                Theme = tenant.Theme;
            }

            return AuthStatus;
        }


        private string FilterSubDomain(Uri url)
        {
            if (url.HostNameType == UriHostNameType.Dns)
            {
                string host = url.Host;
                if (host.Split('.').Length > 2)
                {
                    string[] SubDomain = host.Split('.');
                    return SubDomain[0];
                }
            }

            return null;
        }
    }
}