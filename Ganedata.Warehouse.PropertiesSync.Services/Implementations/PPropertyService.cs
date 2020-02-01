using System.Collections.Generic;
using System.Linq;
using Ganedata.Warehouse.PropertiesSync.Data;
using Ganedata.Warehouse.PropertiesSync.Services.Interfaces;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.Services.Implementations
{
    public class PPropertyService : IPPropertyService
    {
        public List<PProperty> GetPropertiesInfoForSite(int siteId)
        {
            var context = UnityServicesToBeReplaced.Repository.GetContext(siteId);
            return (from m in context.Host_Prop_inf
                    join t in context.Host_Prop_inf2 on m.PCODE equals t.PCODE
                    select new PProperty()
            {
                PropertyCode = m.PCODE,
                SiteId = siteId,
                DateAdded = m.PADDED,
                DateAvailable = m.PAVAIL,
                SyncRequiredFlag = true,
                IsVacant = m.PVACANT,
                LetDate = m.PLETDATE,
                AddressLine1 = m.PADD1,
                AddressLine2 = m.PADD2,
                AddressLine3 = m.PADD3,
                AddressLine4 = m.PADD4,
                AddressLine5 = m.PADD5,
                AddressPostcode = m.PPSTCD,
                PropertyBranch = m.PBRANCH,
                PropertyStatus = m.PSTATUS,
                TenancyMonths = m.PTERM,
                CurrentLandlordCode = m.PLCODE,
                CurrentTenantCode = context.Host_Ten_inf.FirstOrDefault(x => x.TYPCODE.Equals(m.PCODE))!=null? context.Host_Ten_inf.FirstOrDefault(x=> x.TYPCODE.Equals(m.PCODE)).TYCODE:""
                    }).ToList();
        }

    }
}