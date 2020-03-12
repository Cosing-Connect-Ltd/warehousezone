using Ganedata.Core.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        [Display(Name = "Order No.")]
        public string OrderNumber { get; set; }
        [Display(Name = "Issue Date")]
        public DateTime? IssueDate { get; set; }
        [Display(Name = "Expected Date")]
        public DateTime? ExpectedDate { get; set; }
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
        [Display(Name = "Transaction Type")]
        public int InventoryTransactionTypeId { get; set; }
        [Display(Name = "Account")]
        public int? AccountID { get; set; }
        [Display(Name = "Job Type")]
        public int? JobTypeId { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Cancel Date")]
        public DateTime? CancelDate { get; set; }
        [Display(Name = "Confirm Date")]
        public DateTime? ConfirmDate { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Confirm By")]
        public int? ConfirmBy { get; set; }
        [Display(Name = "Cancel By")]
        public int? CancelBy { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        public bool IsCancel { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Order Status")]
        public int OrderStatusID { get; set; }
        [Display(Name = "Loan Type")]
        public int? LoanID { get; set; }
        // Account contact person for this specific order
        [Display(Name = "Account Contact")]
        public int? AccountContactId { get; set; }
        [Display(Name = "Order Total")]
        public decimal OrderTotal { get; set; }
        // flag for Posted into accounting software eg. Sage
        [Display(Name = "Posted for Invoicing")]
        public bool Posted { get; set; }
        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }
        [Display(Name = "Invoice Details")]
        public string InvoiceDetails { get; set; }
        [Display(Name = "Order Cost")]
        public decimal? OrderCost { get; set; }
        public decimal OrderDiscount { get; set; }
        [Display(Name = "Report Type")]
        public int? ReportTypeId { get; set; }
        [Display(Name = "Charge To")]
        public int? ReportTypeChargeId { get; set; }
        [Display(Name = "Transfer Warehouse")]
        public int? TransferWarehouseId { get; set; }
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }
        [Display(Name = "SLA Priority")]
        public int? SLAPriorityId { get; set; }
        [Display(Name = "Expected Hours")]
        public short? ExpectedHours { get; set; }
        public DateTime? AuthorisedDate { get; set; }
        public int? AuthorisedUserID { get; set; }
        public string AuthorisedNotes { get; set; }
        public int? WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        [Display(Name = "Shipment Address Line1")]
        public string ShipmentAddressLine1 { get; set; }
        [Display(Name = "Shipment Address Line2")]
        public string ShipmentAddressLine2 { get; set; }
        [Display(Name = "Shipment Address Line3")]
        public string ShipmentAddressLine3 { get; set; }
        [Display(Name = "Shipment Address Line4")]
        public string ShipmentAddressLine4 { get; set; }
        [Display(Name = "Shipment Address Postcode")]
        public string ShipmentAddressPostcode { get; set; }
        [Display(Name = "Property")]
        public int? PPropertyId { get; set; }
        public virtual int? ShipmentPropertyId { get; set; }
        public Guid? OrderGroupToken { get; set; }
        public virtual int? ShipmentWarehouseId { get; set; }
        public bool IsShippedToTenantMainLocation { get; set; }
        public string CustomEmailRecipient { get; set; }
        public string CustomCCEmailRecipient { get; set; }
        public string CustomBCCEmailRecipient { get; set; }
        public int? AccountCurrencyID { get; set; }
        public int? OrderNoteId { get; set; }
        public string Notes { get; set; }
        public DateTime? NotesDate { get; set; }
        public int? InvoiceId { get; set; }

        public decimal? Qty { get; set; }
        public decimal? ProcessedQty { get; set; }
        public decimal? ReturnQty { get; set; }


    }

    public class TransferOrderViewModel : OrderViewModel
    {
        public string Status { get; set; }
        public string Account { get; set; }
        public int TransType { get; set; }
        public string OrderType { get; set; }
        public string TransferWarehouse { get; set; }

        public int? EmailCount { get; set; }
    }
    public class SalesOrderViewModel : OrderViewModel
    {
        public string POStatus { get; set; }
        [Display(Name = "Account Code")]
        public string Account { get; set; }
        public int OrderTypeId { get; set; }
        [Display(Name = "Order Type")]
        public string OrderType { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        [Display(Name = "Currency")]
        public string Currecny { get; set; }
        public int? EmailCount { get; set; }
        public string Property { get; set; }
        public string SaleNotes { get; set; }
        public string PickerName { get; set; }
        public List<OrderNotesViewModel> OrderNotesList { get; set; }





        public string OrderNotesListText
        {
            get
            {
                var notes = "";
                if (OrderNotesList != null)
                {
                    foreach (var m in OrderNotesList)
                    {
                        notes = notes + "<br/>" + m.Notes + " <small>(" + m.NotesByName + " : " + m.NotesDate.ToString("dd/MM/yyyy") + ")</small>";
                    }
                }
                return notes;
            }
        }
    }

    public class PurchaseOrderViewModel : SalesOrderViewModel
    {
        public PurchaseOrderViewModel()
        {

        }

    }
    public class AwaitingAuthorisationOrdersViewModel : SalesOrderViewModel
    {
        public AwaitingAuthorisationOrdersViewModel()
        {


        }
    }
    public class WorksOrderViewModel : SalesOrderViewModel
    {
        public string JobNotes { get; set; }
        public string JobTypeName { get; set; }
        public string JobSubTypeName { get; set; }
        public IEnumerable<string> OrderNotes { get; set; }

        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public int? ResourceId { get; set; }
        public string ResourceName { get; set; }
    }

    public class OrderDetailsViewModel
    {
        public int OrderID { get; set; }
        public int ProductId { get; set; }
        public int OrderDetailID { get; set; }
        public decimal Price { get; set; }
        public decimal TotalWarrantyAmount { get; set; }
        public decimal WarrantyAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public ProductMaster ProductMaster { get; set; }
        public string Product { get; set; }
        public GlobalTax TaxName { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransType { get; set; }
        public bool? EnableGlobalProcessByPallet { get; set; }
        public bool? DirectShip { get; set; }
        public bool? AutoAllowProcess { get; set; }
        public decimal Qty { get; set; }
        public decimal QtyReceived { get; set; }
        public decimal QtyProcessed { get; set; }
        public decimal QtyReturned { get; set; }

        public string OrderNumber { get; set; }
        public string Notes { get; set; }
        public string BarCode2 { get; set; }
        public string Barcode { get; set; }
        public string SkuCode { get; set; }
        public bool DirectPostAllowed { get; set; }

        public string ProductGroup { get; set; }
        public string ProductCategory { get; set; }
        public int? orderstatusId { get; set; }
        public int? orderProcessstatusId { get; set; }
        public ProductGroups ProductGroups { get; set; }

        public string IdentifiersText
        {
            get
            {
                var ids = new List<string>() { SkuCode ?? "", Barcode ?? "", BarCode2 ?? "" };
                return string.Join(",", ids);
            }
        }
    }

    public class ListViewModel
    {
        public int ResultsCount { get; set; }
    }


    public class PendingListDto
    {
        public int OrderID { get; set; }
        public int WarehouseID { get; set; }
        public int OrderTypeId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string POStatus { get; set; }
        public string Account { get; set; }
        public string OrderType { get; set; }
        public string AccountStatus { get; set; }
        public Decimal OrderTotal { get; set; }
        public double? CreditLimit { get; set; }
        public bool? AddTypeShipping { get; set; }
        public bool? FullShip { get; set; }
        public string ResourceName { get; set; }
        public string Property { get; set; }
        public IEnumerable<OrderNotesViewModel> OrderNotesList { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        public IEnumerable<string> OrderNotes { get; set; }

        public string RecurrenceInfo { get; set; }

        public DateTime? RecurrenceInfoStartDate
        {
            get
            {
                DateTime? date = null;
                if (RecurrenceInfo != null && RecurrenceInfo.Length > 33)
                {
                    bool res = DateTime.TryParse(RecurrenceInfo?.Substring(23, 10), CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out DateTime d1);
                    if (res == true)
                    {
                        date = d1;
                    }
                }

                return date;

            }
        }
        public DateTime? RecurrenceInfoEndDate
        {
            get
            {
                DateTime? date = null;
                if (RecurrenceInfo != null && RecurrenceInfo.Length > 59)
                {
                    bool res = DateTime.TryParse(RecurrenceInfo?.Substring(49, 10), CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out DateTime d1);
                    if (res == true)
                    {
                        date = d1;
                    }

                }

                return date;

            }
        }
    }

    public class AuthoriseSalesOrderModel
    {
        public int OrderID { get; set; }
        public string AuthorisedNotes { get; set; }

        public bool UnAuthorise { get; set; }

    }

    public class OrderIdsWithStatus
    {
        public int OrderID { get; set; }
        public int OrderStatusID { get; set; }

    }

    public class OrderNotesViewModel
    {
        public string Notes { get; set; }
        public string NotesByName { get; set; }
        public DateTime NotesDate { get; set; }
        public int OrderNoteId { get; set; }

    }

    public class DelieveryViewModel
    {
        public string DeliveryNO { get; set; }
        public int? OrderID { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? OrderProcessID { get; set; }
        public string AccountCode { get; set; }
        public string OrderNumber { get; set; }
        public string CompanyName { get; set; }
        public int? Status { get; set; }

        public int? orderstatus { get; set; }

        public int? EmailCount { get; set; }


    }

}