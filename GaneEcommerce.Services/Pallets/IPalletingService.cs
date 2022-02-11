using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IPalletingService
    {
        PalletProduct AddFulFillmentPalletProduct(PalletProductAddViewModel model);

        void AddFulFillmentPalletAllOrderProducts(int orderProcessId, int palletId, int currentUserId);

        Pallet CreateNewPallet(int orderProcessId, int userId);

        List<PalletProduct> GetFulFillmentPalletProductsForPallet(int palletId);

        List<PalletProductsSync> GetAllPalletProductsForSync(DateTime? afterDate);

        List<DeliveryMethods> GetAllSentMethods();

        IQueryable<PalletViewModel> GetAllPallets(int? lastXdays = null, PalletStatusEnum? palletStatusEnum = null, int? orderProcessId = null, DateTime? reqDate = null, int? filterByPalletDetail = null, int? dispatchId = null);

        List<PalletsDispatch> GetAllPalletsDispatch(int? dispatchId, DateTime? reqDate, int? orderProcessID = null);

        string GenerateNextPalletNumber(string prefix = null);

        Pallet GetFulfillmentPalletById(int palletId);

        Pallet GetFulfillmentPalletByNumber(string palletNumber);

        decimal GetFulfilledProductQuantity(int orderProcessDetailId);

        PalletDispatchInfoViewModel GetPalletDispatchDetailByPallet(int palletId);

        Task<string> DispatchPallets(PalletDispatchViewModel dispatch, int userId);

        PalletSync DispatchPalletsFromHandheld(PalletSync currentPallet, int userId);

        Pallet UpdatePalletStatus(int palletId, bool isCompleted, Guid? newPalletToken = null, byte[][] palletProofImage = null);

        Pallet UpdatePalletProof(string palletNumber, byte[][] palletProofImage);

        bool DeletePallet(int palletId);

        int DeletePalletProduct(int palletProductId, int userId);

        PalletDispatchProgress UpdateDispatchProgress(PalletDispatchProgress currentDispatch, int userId);

        PalletsDispatch GetPalletsDispatchByDispatchId(int palletDispatchId);

        IEnumerable<TenantDeliveryService> GetAllTenantDeliveryServices(int tenantId, DeliveryMethods? deliveryMethod = null);

        bool MarkedOrderProcessAsDispatch(int OrderProcessId);

        IEnumerable<PalletsDispatch> GetAllPalletsDispatch();

        IQueryable<PalletsDispatch> GetAllPalletsDispatchs();

        IQueryable<Pallet> GetAllPalletByDispatchId(int id);
        bool UpdatePalletsDispatchStatus(int dispatchId, int? resourceId, int userID, bool Status = false);

        PalletDispatchLabelPrintViewModel PalletDispatchForLabels(int tenantId, int userId);

        bool UpdateDispatchForLabelsStatus(string shipmentId);

        bool LoadPalletOnTruck(string palletIds, int truckId, int palletDispatchId);
    }
}