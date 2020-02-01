using System.Collections.Generic;
using System.Threading.Tasks;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Services
{
    public interface IWarehouseSyncService
    {
        Task<List<PTenant>> UpdateTenantInformationForSite(int siteId, List<PTenant> tenantsToImport);

        Task<List<PLandlord>> UpdateLandlordInformationForSite(int siteId, List<PLandlord> landlordsToImport);

        Task<List<PProperty>> UpdatePropertiesInformationForSite(int siteId, List<PProperty> propertiesToImport);
        Task UpdateTenantCurrentProperties();


    }
}