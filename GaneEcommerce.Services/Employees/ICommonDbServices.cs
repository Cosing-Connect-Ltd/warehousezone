using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;

namespace Ganedata.Core.Services
{
    public interface ICommonDbServices
    {
        List<ProductLocationsDetailResponse> ProductsByLocationDetails(int locationId);
        ProductLocationsResponse LocationsByProductDetails(int productId, int warehouseId);
        List<CommonLocationViewModel> ProductsByLocations(int tenant, int warehouse);
        OrderDetail SetDetails(OrderDetail model,int? accountId, string urlReferer, string processBy);
        int GetQuantityInLocation(InventoryTransaction transaction);
    }
}