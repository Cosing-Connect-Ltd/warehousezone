using Ganedata.Warehouse.PropertiesSync.Data.Helpers;
using Ganedata.Warehouse.PropertiesSync.Services.Interfaces;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Warehouse.PropertiesSync.Services.Implementations
{
    public class PTenantsService : ITenantsService
    {
        public List<PTenant> GetAllTenantsFromAllSites()
        {
            var result = new List<PTenant>();
            for (var siteId = 1; siteId <= GanedataGlobalConfigurations.EntityConnections.Count; siteId++)
            {
                var tenants = GetTenantsForSite(siteId);
                result.AddRange(tenants);
            }
            return result.OrderBy(m => m.TenantYCode).ToList();
        }

        public List<PTenant> GetTenantsForSite(int siteId)
        {
            DateTime tenancyStartDateLimit = new DateTime(2016, 01, 01);

            var context = UnityServicesToBeReplaced.Repository.GetContext(siteId);

            return (from t in context.Host_Tenants
                    join tInfo in context.Host_Ten_inf on t.TTYCODE equals tInfo.TYCODE
                    where (tInfo.TYSTART > tenancyStartDateLimit)
                    select new PTenant()
                    {
                        AddressLine1 = tInfo.TYADD1,
                        AddressLine2 = tInfo.TYADD2,
                        AddressLine3 = tInfo.TYADD3,
                        AddressLine4 = tInfo.TYADD4,
                        AddressPostcode = tInfo.TYPSTCD,
                        Email = t.TE_MAIL,
                        IsHeadTenant = t.TTHEAD,
                        HomeTelephone = t.TEMTEL,
                        MobileNumber = t.TTELMOBL,
                        TenancyAdded = t.TADDED,
                        TenancyCategory = t.TCategory,
                        TenancyPeriodMonths = tInfo.TYTERMMTH,
                        TenancyRenewDate = tInfo.TYRENEW,
                        TenancyVacateDate = tInfo.TYVAC,
                        TenancyStarted = tInfo.TYSTART,
                        TenancyStatus = tInfo.TYSTATUS,
                        TenantCode = t.TCODE,
                        TenantFullName = t.TNAME,
                        TenantSalutation = t.TSALU,
                        TenantYCode = t.TTYCODE,
                        CurrentPropertyCode = tInfo.TYPCODE,
                        WorkTelephone1 = t.TTELWK1,
                        WorkTelephone2 = t.TTELWK2,
                        WorkTelephoneFax = t.TTELFAX,
                        SiteId = siteId
                    }).ToList();
        }

    }
}