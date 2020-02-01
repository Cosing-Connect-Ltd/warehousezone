using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Services
{
    public interface IStockTakeApiService
    {
        Task<StockTakeProductCodeScanResponse> RecordScannedProducts(StockTakeProductCodeScanRequest request, bool moveToNextProduct = false);
        Task<ResponseObject> UpdateStockTakeDetailQuantity(StockDetailQuantityUpdateRequest request);
        Task<ResponseObject> DeleteStockTakeDetail(StockDetailDeleteRequest request);
        Task<ResponseObject> CreateProductOnStockTake(ProductDetailRequest request);
        StockTake GetStockTakeById(int id);
        IEnumerable<StockTake> GetAllStockTakes(int warehouseId, int TenantId, DateTime? reqDate = null, bool includeIsDeleted = false);
        StockTake StopStockTake(int stockTakeId);
        StockTake CreateStockTake(StockTake stockTake, int userId, int tenantId, int warehouseId);
        StockTake UpdateStockTakeStatus(int stockTakeId, int userId, int statusId);
        StockTake GetStockTakeByStatus(int warehouseId, int statusId,int TenantId);
        List<StockTakeDetailsViewModel> GetStockTakeDetailsByStockTakeId(int stockTakeId);
        List<StockTakeSnapshotsViewModel> GetStockTakeSnapshotsByStockTakeId(int stockTakeId);
        StockTakeDetail GetStockTakeDetailById(int stockTakeDetailId);
        List<StockTakeDetail> GetUnAppliedStockTakeDetailById(int productId, int stockTakeId);
        StockTake ApplyStockTakeChanges(StockTakeApplyChangeRequest request, int userId);
        List<StockTake> GetStockTakesInProgress(int warehouseId);
        List<StockTake> GetStockTakesPendingOrStopped(int warehouseId);
        List<StockTakeDetailsSerial> GetProductStockTakeSerials(int stockTakeId, int productId);
        string GetStockTakeFullReport(int stockTakeId);

        StockTakeReportResponse GetStockTakeReportById(int stockTakeId, int tenantId, int warehouseId, int userId, bool justVariations = false);

        bool DeleteStockTakeDetial(int stockTakeDetailId);

        IQueryable<object> GetDetialStock(int tenantId, int warehouseId, int stockTakeId);
    }
}