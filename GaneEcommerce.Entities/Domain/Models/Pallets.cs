using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public abstract class BaseAuditInfo
    {
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? DateUpdated { get; set; }
        public int? UpdatedBy { get; set; }

    }

    [Serializable]
    public class PalletsDispatch : BaseAuditInfo
    {
        public PalletsDispatch()
        {
            Pallets = new HashSet<Pallet>();
        }

        [Key]
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
        [ForeignKey("MarketVehicleID")]
        public virtual MarketVehicle Vehicle { get; set; }
        public DeliveryMethods? DeliveryMethod { get; set; }
        public int? VehicleDriverResourceID { get; set; }
        [ForeignKey("VehicleDriverResourceID")]
        public virtual Resources VehicleDriverResource { get; set; }
        public virtual ICollection<Pallet> Pallets { get; set; }
        public PalletDispatchStatusEnum DispatchStatus { get; set; }
        public int? OrderProcessID { get; set; }
        [ForeignKey("OrderProcessID")]
        public virtual OrderProcess OrderProcess { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverSign { get; set; }
        public string NetworkCode { get; set; }
        public string ShipmentId { get; set; }
        public string ConsignmentNumber { get; set; }
        public string ParcelNumbers { get; set; }
        public bool LabelPrintStatus { get; set; }

    }

    [Serializable]
    public class Pallet : BaseAuditInfo
    {
        public Pallet()
        {
            PalletProducts = new HashSet<PalletProduct>();
        }
        [Key]
        public int PalletID { get; set; }
        public bool? IsDeleted { get; set; }
        public string PalletNumber { get; set; }
        public string ProofOfLoadingImage { get; set; }
        public int? RecipientAccountID { get; set; }
        [ForeignKey("RecipientAccountID")]
        public virtual Account RecipientAccount { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int? CompletedBy { get; set; }
        public int? PalletsDispatchID { get; set; }
        [ForeignKey("PalletsDispatchID")]
        public virtual PalletsDispatch PalletsDispatch { get; set; }
        public Guid? MobileToken { get; set; }
        public virtual ICollection<PalletProduct> PalletProducts { get; set; }
        public int? OrderProcessID { get; set; }
        [ForeignKey("OrderProcessID")]
        public virtual OrderProcess OrderProcess { get; set; }
        public bool ScannedOnLoading { get; set; }
        public DateTime? LoadingScanTime { get; set; }
        public bool ScannedOnDelivered { get; set; }
        public DateTime? DeliveredScanTime { get; set; }

    }
    [Serializable]
    public class PalletProduct : BaseAuditInfo
    {
        [Key]
        public int PalletProductID { get; set; }
        public bool? IsDeleted { get; set; }
        public int? OrderProcessDetailID { get; set; }
        [ForeignKey("OrderProcessDetailID")]
        public virtual OrderProcessDetail OrderProcessDetail { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual ProductMaster Product { get; set; }

        public int PalletID { get; set; }
        [ForeignKey("PalletID")]
        public virtual Pallet Pallet { get; set; }

        public decimal Quantity { get; set; }

        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

    }
}