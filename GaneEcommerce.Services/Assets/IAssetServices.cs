using Ganedata.Core.Entities.Domain;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IAssetServices
    {
        IQueryable<Assets> GetAllValidAssets(int TenantId);
        IQueryable<AssetLog> GetAllAssetLog(int? AssetId);
        void SaveAsset(Assets assets);
        void UpdateAsset(Assets assets);
        void RemoveAsset(int AssetId);
        Assets GetAssetById(int id);
        Assets GetAssetByTag(string tag);
    }
}
