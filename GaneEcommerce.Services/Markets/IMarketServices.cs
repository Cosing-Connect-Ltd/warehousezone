using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IMarketServices
    {
        MarketListViewModel GetAllValidMarkets(int tenantId);
        MarketViewModel GetMarketById(int marketId);
        IEnumerable<object> GetAllValidMarkets(int tenantId, int warehouseId);
        Market SaveMarket(Market market, int userId);
        void DeleteMarket(int marketId, int userId);
        MarketCustomersViewModel GetMarketCustomersById(int marketId, int tenantId, string searchText);
        MarketCustomersViewModel SaveMarketCustomer(MarketCustomersViewModel model, int userId, int TenantId);

        MarketVehicleViewModel GetMarketVehicleById(int marketVehicleId);
        MarketVehicleListViewModel GetAllValidMarketVehicles(int tenantId, DateTime? reqDate = null, bool includeIsDeleted = false);
        MarketVehicleViewModel GetMarketVehicleByVehicleNumber(string marketVehicleNumber);
        MarketVehicleViewModel SaveMarketVehicle(MarketVehicle marketVehicle, int userId);
        void DeleteMarketVehicle(int marketId, int userId);

        MarketRouteListViewModel GetAllValidMarketRoutes(int tenantId);
        MarketRouteViewModel GetMarketRouteById(int routeId);
        void DeleteMarketRoute(int marketRouteId, int userId);
        MarketRoute SaveMarketRoute(MarketRoute marketRoute, int userId);
        RouteMarketsViewModel GetRouteMarketsById(int marketId, int tenantId, string searchText = null);
        RouteMarketsViewModel SaveRouteMarkets(RouteMarketsViewModel model, int userId);

        IQueryable<MarketJob> GetAllValidMarketJobs(int tenantId, MarketJobStatusEnum? statusEnum = null);
        MarketJobViewModel GetMarketJobById(int marketJobId);
        MarketJobViewModel SaveMarketJob(MarketJob marketJob, int? resourceId, MarketJobStatusEnum? latestjobStatusId, int userId, int tenantId);
        Task<MarketJobViewModel> UpdateMarketJobAllocation(int marketJobId, int resourceId, int userId, int tenantId,
            MarketJobStatusEnum? latestJobStatusId = null, string reason = null, DateTime? actionDate = null, double? latitude = null, double? longitude = null, string terminalSerial = null);

        Task<MarketJobViewModel> AcceptMarketJob(int marketJobId, int userId, int tenantId, string terminalSerial = null, double? latitude = null, double? longitude = null);
        Task<MarketJobViewModel> DeclineMarketJob(int marketJobId, int userId, int tenantId, string reason, string terminalSerial = null, double? latitude = null, double? longitude = null);
        Task<MarketJobViewModel> CompleteMarketJob(int marketJobId, int userId, int tenantId, string reason, string terminalSerial = null, double? latitude = null, double? longitude = null);
        Task<MarketJobViewModel> CancelMarketJob(int marketJobId, int userId, int tenantId, string reason, string terminalSerial = null, double? latitude = null, double? longitude = null);
        List<MarketJobAllocationModel> GetAllResourceJobs(int resourceId, DateTime? reqDate = null, bool includeIsDeleted = false);
        List<MarketProductLevelViewModel> GetAllStockLevelsForMarket(int marketId, int tenantId);
        ProductMarketStockLevel UpdateProductLevelsForMarkets(int marketId, int productId, decimal stockQty, int userId);


        IQueryable<MarketRouteProgress> GetMarketRouteProgresses(int? marketRouteId, DateTime dateTime, int tenantId);

        string GetMarketName(int accountId, int MarketId);
    }
}
