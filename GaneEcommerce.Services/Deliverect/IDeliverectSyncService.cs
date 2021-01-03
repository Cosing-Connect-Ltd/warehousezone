using Ganedata.Core.Entities.Domain.ImportModels;
using Ganedata.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IDeliverectSyncService
    {
        Task SyncChannelLinks();
        Task SyncProducts(int? tenantId, int currentUserId);
        Task<bool> SendOrderToDeliverect(OrdersSync order);
        void SaveProduct(int tenantId, int currentUserId, DeliverectProduct deliverectProduct, int? sortOrder);
        void SaveCategory(int tenantId, int currentUserId, DeliverectCategory deliverectCategory, int? sortOrder);
        void DeleteProductsExceptDeliverect(int currentUserId, List<string> menuProducts = null);
        void DeleteDepartmentsExceptDeliverect(int currentUserId, List<string> menuCategories = null);
        void UpdateProductCategory(string deliverectProductId, int categoryId, int currentUserId);

    }
}