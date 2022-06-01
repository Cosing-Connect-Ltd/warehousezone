using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class OrdersSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<OrdersSync> Orders { get; set; }
    }

    public class OrderStatusSync
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public OrderStatusEnum OrderStatusID { get; set; }
    }

    public class OrdersSync
    {
        public OrdersSync()
        {
            OrderDetails = new List<OrderDetailSync>();
            ProductsWithoutEnoughStock = new List<OrderProcessLowStockItems>();
        }
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Note { get; set; }
        public int InventoryTransactionTypeId { get; set; }
        public int? AccountID { get; set; }
        public int? JobTypeId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? ConfirmBy { get; set; }
        public int? CancelBy { get; set; }
        public int TenentId { get; set; }
        public bool IsCancel { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public OrderStatusEnum OrderStatusID { get; set; }
        public int? LoanID { get; set; }
        // Account contact person for this specific order
        public int? AccountContactId { get; set; }
        public decimal OrderTotal { get; set; }
        // flag for Posted into accounting software eg. Sage
        public bool Posted { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDetails { get; set; }
        public decimal? OrderCost { get; set; }
        public int? ReportTypeId { get; set; }
        public int? ReportTypeChargeId { get; set; }
        public int? TransferWarehouseId { get; set; }
        public string TransferWarehouseName { get; set; }
        public int? DepartmentId { get; set; }
        public int? SLAPriorityId { get; set; }
        public short? ExpectedHours { get; set; }
        public DateTime? AuthorisedDate { get; set; }
        public int? AuthorisedUserID { get; set; }
        public string AuthorisedNotes { get; set; }
        public int? WarehouseId { get; set; }
        public string DeliverectChannelLinkId { get; set; }
        public string DeliverectChannel { get; set; }
        public string ShipmentAddressLine1 { get; set; }
        public string ShipmentAddressLine2 { get; set; }
        public string ShipmentAddressLine3 { get; set; }
        public string ShipmentAddressTown { get; set; }
        public string ShipmentAddressPostcode { get; set; }
        public int? PPropertyId { get; set; }
        public int? ShipmentPropertyId { get; set; }
        public Guid? OrderGroupToken { get; set; }
        public int? ShipmentWarehouseId { get; set; }
        public bool IsShippedToTenantMainLocation { get; set; }
        public string CustomEmailRecipient { get; set; }
        public string CustomCCEmailRecipient { get; set; }
        public string CustomBCCEmailRecipient { get; set; }
        public int? AccountCurrencyID { get; set; }
        public int? JobSubTypeId { get; set; }
        public string RequestStatus { get; set; }
        public bool RequestSuccess { get; set; }
        public List<OrderDetailSync> OrderDetails { get; set; }
        public SerialProcessStatus SerialProcessStatus { get; set; }
        public bool? DirectShip { get; set; }
        public decimal? AmountPaidByAccount { get; set; }
        public decimal? AccountBalanceBeforePayment { get; set; }
        public decimal? AccountBalanceOnPayment { get; set; }
        public int? AccountPaymentModeId { get; set; }
        public bool OrderPaid { get; set; }
        public FoodOrderTypeEnum FoodOrderType { get; set; }
        public bool EndOfDayGenerated { get; set; }
        public int? VanSalesDailyCashId { get; set; }
        public decimal OrderDiscount { get; set; }
        public Guid? OrderToken { get; set; }
        public int? PickerId { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public bool OfflineSale { get; set; }
        public string VoucherCode { get; set; }
        public string PaypalBraintreeNonce { get; set; }

        public List<OrderProcessLowStockItems> ProductsWithoutEnoughStock { get; set; }
    }

    public class OrderProcessLowStockItems  {
    
        public int ProductId { get; set; }
        public decimal StockLevel { get; set; }
        public string ProductName { get; set; }
        public string SkuCode { get; set; }

    }

    
    public class OrderDetailSync
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int WarehouseId { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Notes { get; set; }
        public int ProductId { get; set; }
        public int? ProdAccCodeID { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public int? WarrantyID { get; set; }
        public decimal WarrantyAmount { get; set; }
        public int? TaxID { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int TenentId { get; set; }
        public bool? IsDeleted { get; set; }
        public OrderStatusEnum? OrderDetailStatusId { get; set; }
        public int SortOrder { get; set; }
        public int? ProductAttributeValueId { get; set; }
        public string ProductAttributeValueName { get; set; }
        public string ProductName { get; set; }
        public string SkuCode { get; set; }
        public string Barcode { get; set; }
        public bool ProcessByPallet { get; set; }
        public decimal? QuantityProcessed { get; set; }
        public bool InStock { get; set; }
    }

    public class SerialProcessStatus
    {
        public SerialProcessStatus()
        {
            ProcessedSerials = new List<string>();
            RejectedSerials = new List<string>();
        }

        public List<string> ProcessedSerials { get; set; }
        public List<string> RejectedSerials { get; set; }
    }

    public class UpdateOrderStatusViewModel
    {
        public string SerialNo { get; set; }
        public int OrderId { get; set; }
        public OrderStatusEnum StatusId { get; set; }
        public int UserId { get; set; }
    }

}