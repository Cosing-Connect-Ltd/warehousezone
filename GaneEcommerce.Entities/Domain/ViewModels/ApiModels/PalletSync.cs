using System;
using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Models
{
    public class PalletSync
    {
        public PalletSync()
        {
            PalletDispatchInfo = new PalletDispatchSync();
        }

        public int PalletID { get; set; }
        public string PalletNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ProofOfLoadingImage { get; set; }
        public int RecipientAccountID { get; set; }
        public virtual Account RecipientAccount { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int? CompletedBy { get; set; }
        public int CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? PalletsDispatchID { get; set; }
        public bool IsDispatched { get; set; }
        public byte[][] ProofOfLoadingImageBytes { get; set; }
        public string TerminalSerialNumber { get; set; }
        public int TenantId { get; set; }
        public PalletDispatchSync PalletDispatchInfo { get; set; }
        public Guid TransactionLogId { get; set; }
        public List<int> SelectedPallets { get; set; }
        public int? OrderProcessID { get; set; }
        public bool ScannedOnLoading { get; set; }
        public DateTime? LoadingScanTime { get; set; }
        public bool ScannedOnDelivered { get; set; }
        public DateTime? DeliveredScanTime { get; set; }
    }


    public class PalletDispatchSync
    {
        public int PalletsDispatchID { get; set; }
        public string DispatchReference { get; set; }
        public string VehicleIdentifier { get; set; }
        public string VehicleDescription { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int? CompletedBy { get; set; }
        public string TrackingReference { get; set; }
        public string CustomVehicleNumber { get; set; }
        public string CustomVehicleModel { get; set; }
        public string CustomDriverDetails { get; set; }
        public string ProofOfDeliveryImageFilenames { get; set; }
        public string DispatchNotes { get; set; }
        public int? MarketVehicleID { get; set; }
        public DeliveryMethods? DeliveryMethod { get; set; }
        public int? MarketVehicleDriverID { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedBy { get; set; }
        public PalletDispatchStatusEnum DispatchStatus { get; set; }
        public int? OrderProcessID { get; set; }
    }

    public class PalletsDispatchSyncCollection
    {
        public PalletsDispatchSyncCollection()
        {
            PalletDispatchSync = new List<PalletDispatchSync>();
        }

        public Guid TerminalLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<PalletDispatchSync> PalletDispatchSync { get; set; }
    }


    public class PalletsSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<PalletSync> Pallets { get; set; }
    }

    public class PalletProductsSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<PalletProductsSync> PalletProducts { get; set; }
    }

    public class PalletProductsSync
    {
        public decimal PalletQuantity { get; set; }
        public int CurrentPalletID { get; set; }
        public int OrderProcessDetailID { get; set; }
        public int? AccountID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int PalletProductID { get; set; }
        public bool PostedSuccess { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class PalletDispatchMethodSync
    {
        public int SentMethodID { get; set; }
        public string SentMethod { get; set; }
    }
    public class PalletDispatchMethodSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<PalletDispatchMethodSync> PalletDispatchMethods { get; set; }
    }

    public class PalletDispatchProgress
    {
        public Guid TerminalLogId { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
        public int DispatchId { get; set; }
        public PalletDispatchStatusEnum DispatchStatus { get; set; }
        public List<int> ScannedPalletSerials { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public string ReceiverName { get; set; }
        public byte[] ReceiverSign { get; set; }
        public string Comments { get; set; }
    }
}