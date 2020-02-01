using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IAdminServices
    {
        TenantDepartments SaveTenantDepartment(TenantDepartments department, int userId);

        List<Locations> GetAllLocations(int tenantId, int warehouseId);

        IEnumerable<PalletTracking>GetPalletTrackingsbyProductId(int? productId,int TenantId,int WarehouseId);
    }
}
