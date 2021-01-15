using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class OrderProcessesSyncCollection
    {
        public OrderProcessesSyncCollection()
        {
            OrderProcesses = new List<OrderProcessesSync>();
        }
        public Guid TerminalLogId { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<OrderProcessesSync> OrderProcesses { get; set; }
    }

    public class OrderProcessesSync
    {
        public OrderProcessesSync()
        {
            OrderProcessDetails = new List<OrderProcessDetailSync>();
            AccountTransactionFiles = new List<AccountTransactionFileSync>();
        }

        public Guid? OrderToken { get; set; }
        public int OrderProcessID { get; set; }
        public string DeliveryNo { get; set; }
        public DeliveryMethods? DeliveryMethod { get; set; }
        public int? OrderID { get; set; }
        public int WarehouseId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int TenentId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int? AccountID { get; set; }
        public string OrderNotes { get; set; }
        public decimal OrderProcessDiscount { get; set; }
        public decimal OrderProcessTotal { get; set; }
        public InventoryTransactionTypeEnum InventoryTransactionTypeId { get; set; }
        public OrderStatusEnum? OrderStatusID { get; set; }
        public int? TransferToWarehouseId { get; set; }
        public string TransferToWarehouseName { get; set; }
        public List<OrderProcessDetailSync> OrderProcessDetails { get; set; }
        public List<AccountTransactionFileSync> AccountTransactionFiles { get; set; }
        public bool SaleMade { get; set; }
        public MarketRouteProgressSync ProgressInfo { get; set; }
        public AccountTransactionInfoSync AccountTransactionInfo { get; set; }
        public OrderProofOfDeliverySync OrderProofOfDeliverySync { get; set; }
        public int? OrderProcessStatusId { get; set; }
        public string TerminalInvoiceNumber { get; set; }
        public string ShipmentAddressLine1 { get; set; }
        public string ShipmentAddressLine2 { get; set; }
        public string ShipmentAddressPostcode { get; set; }
        public string ShipmentAddressTown { get; set; }
        public string PickContainerCode { get; set; }
        public FoodOrderTypeEnum? FoodOrderType { get; set; }
        public bool OfflineSale { get; set; }

        public int? DeliveryAccountAddressID { get; set; }
        public int? BillingAccountAddressID { get; set; }
        public string VoucherCode { get; set; }
        public decimal? VoucherCodeDiscount { get; set; }
    }

    public class MarketRouteProgressSync
    {
        public Guid RouteProgressId { get; set; }
        public int MarketId { get; set; }
        public int MarketRouteId { get; set; }
        public int AccountId { get; set; }
        public int? OrderId { get; set; }
        public string Comment { get; set; }
        public bool? SaleMade { get; set; }
        public DateTime? Timestamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class OrderProcessDetailSync
    {
        public int OrderProcessDetailID { get; set; }
        public int OrderProcessId { get; set; }
        public int ProductId { get; set; }
        public decimal QtyProcessed { get; set; }
        public OrderStatusEnum OrderDetailStatusID { get; set; }
        public int? WarrantyID { get; set; }
        public decimal WarrantyAmount { get; set; }
        public int? TaxID { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Price { get; set; }
        public int? OrderDetailID { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int TenentId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool IsSerialised { get; set; }
        public List<string> Serials { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<PalleTrackingProcess> PalleTrackingProcess { get; set; }
        public List<ProductKitMapViewModel> ProductKitMapViewModel { get; set; }
        public string LocationCode { get; set; }
        public string Notes { get; set; }
    }

    public class AccountTransactionInfoSync
    {
        public int AccountTransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal OrderCost { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OpeningAccountBalance { get; set; }
        public decimal FinalAccountBalance { get; set; }
        public string Notes { get; set; }
        public DateTime DateCreated { get; set; }
        public AccountPaymentModeEnum? AccountPaymentModeId { get; set; }
        public string AccountPaymentMode { get; set; }
    }

    public class OrderProofOfDeliverySync
    {
        public int Id { get; set; }
        public string SignatoryName { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileContent { get; set; }
        public int? OrderProcessID { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public decimal NoOfCases { get; set; }
    }

}