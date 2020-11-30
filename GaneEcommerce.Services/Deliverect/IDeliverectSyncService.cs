using Ganedata.Core.Models;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IDeliverectSyncService
    {
        Task SyncChannelLinks();
        Task SyncProducts(int? tenantId, int currentUserId);
        Task<bool> SendOrderToDeliverect(OrdersSync order);
    }
}