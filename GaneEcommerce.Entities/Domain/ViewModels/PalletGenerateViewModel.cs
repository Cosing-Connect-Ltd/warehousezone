using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    public class PalletGenerateViewModel
    {
        public PalletGenerateViewModel()
        {
            AllCurrentPallets = new List<SelectListItem>();
        }
        public bool PalletsEnabled { get; set; }


        public string NextPalletNumber { get; set; }
        public bool IsNewPallet { get; set; }
        public bool IsCompleted { get; set; }
        public string PalletDateCompleted { get; set; }
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public decimal ProcessedQuantity { get; set; }
        public string ProductName { get; set; }
        public int SelectedOrderProcessId { get; set; }
        public int SelectedPalletID { get; set; }
        public int? dispatchId { get; set; }
        public List<SelectListItem> AllCurrentPallets { get; set; }
        public OrderProcess OrderProcesses { get; set; }
    }

    public class PalletDispatchViewModel
    {

        public PalletDispatchViewModel()
        {
            AllSentMethods = new List<SelectListItem>();
        }
        public int? MarketVehicleID { get; set; }
        public int? MarketVehicleDriverID { get; set; }
        public DeliveryMethods? DeliveryMethod { get; set; }
        public int PalletDispatchId { get; set; }
        public string TrackingReference { get; set; }
        public string SentMethod { get; set; }
        public string CustomVehicleNumber { get; set; }
        public string CustomVehicleModel { get; set; }
        public string CustomDriverDetails { get; set; }
        public string ProofOfDeliveryImageFilenames { get; set; }
        public string DispatchSelectedPalletIds { get; set; }
        public string DispatchNotes { get; set; }
        public string DispatchRefrenceNumber { get; set; }

        public string NetworkCode { get; set; }
        public virtual IEnumerable<SelectListItem> AllSentMethods { get; set; }
        public virtual IEnumerable<int> SelectedPallets { get; set; }

        public virtual IEnumerable<SelectListItem> AllVehicles { get; set; }
        public virtual IEnumerable<SelectListItem> AllDrivers { get; set; }
        public virtual IEnumerable<SelectListItem> AllDpdServices { get; set; }


    }

    public class PalletDispatchInfoViewModel
    {
        public int PalletID { get; set; }
        public string TrackingReference { get; set; }
        public string SentMethod { get; set; }
        public string VehicleNumber { get; set; }
        public string DriverName { get; set; }
        public string DispatchNotes { get; set; }
        public string DispatchEvidenceImages { get; set; }
        public bool IsDispatched { get; set; }
        public List<string> DispatchEvidenceImagePaths
        {
            get
            {
                if (!string.IsNullOrEmpty(DispatchEvidenceImages))
                {
                    return DispatchEvidenceImages.Split(',').Select(m => "~/UploadedFiles/Pallets/" + m).ToList();
                }
                return new List<string>();
            }
        }
        public string DispatchDate { get; set; }

    }

    public class PalletOrderProcessList
    {
        public PalletOrderProcessList()
        {
            OrderDetailsLists = new List<PalletAccountOrderDetailsList>();
        }
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public List<PalletAccountOrderDetailsList> OrderDetailsLists { get; set; }

    }

    public class PalletAccountOrderList
    {
        public PalletAccountOrderList()
        {
            OrderDetailsLists = new List<PalletAccountOrderDetailsList>();
        }

        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public List<PalletAccountOrderDetailsList> OrderDetailsLists { get; set; }
    }

    public class PalletAccountOrderDetailsList
    {
        public int OrderDetailID { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal ProcessedQuantity { get; set; }

    }


    public class PalletViewModel
    {
        public int PalletID { get; set; }
        public string PalletNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public int? RecipientAccountID { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DispatchTime { get; set; }
        public PalletsDispatch Dispatch { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool ProductCount { get; set; }
        public int? OrderProcessID { get; set; }
        public bool ScannedOnLoading { get; set; }
        public DateTime? LoadingScanTime { get; set; }
        public bool ScannedOnDelivered { get; set; }
        public DateTime? DeliveredScanTime { get; set; }

        public int? DispatchStatus { get; set; }

    }

    public class PalletProductsViewModel
    {
        public int PalletProductID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string WarrantyType { get; set; }
        public DateTime PalletTime { get; set; }
    }

    public class PalletProductAddViewModel
    {
        public decimal PalletQuantity { get; set; }
        public int CurrentPalletID { get; set; }
        public int OrderProcessDetailID { get; set; }
        public int ProductID { get; set; }
        public int AccountID { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
    }



    public class PalletOrderProductsCollection
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }

        public int? AccountID { get; set; }
        public string AccountName { get; set; }

        public int PalletID { get; set; }
        public string PalletNumber { get; set; }

        public string DateCreated { get; set; }
        public string CreatedBy { get; set; }

        public string DateDispatched { get; set; }
        public string DispatchedBy { get; set; }

        public string HtmlContent { get; set; }

        public List<PalletOrderProductItem> ProductItems { get; set; } = new List<PalletOrderProductItem>();

    }

    public class PalletOrderProductItem
    {
        public int ProductID { get; set; }

        public string ProductNameWithCode { get; set; }

        public decimal PalletQuantity { get; set; }

        public string DateCreated { get; set; }
    }
}