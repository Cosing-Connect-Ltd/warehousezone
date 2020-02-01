using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;

namespace Ganedata.Core.Services
{
    public interface IAssetServices
    {
        IQueryable<Assets> GetAllValidAssets(int TenantId);
        IQueryable<AssetLog> GetAllAssetLog (int AssetId);

        void SaveAsset(Assets assets);

        void UpdateAsset(Assets assets);

        void RemoveAsset(int AssetId);
        Assets GetAssetById(int id);



    }
}
