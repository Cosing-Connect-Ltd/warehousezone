using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            OrderProcess = new HashSet<OrderProcess>();
            Appointmentses = new HashSet<Appointments>();
            OrderNotes = new HashSet<OrderNotes>();
        }

        [Key]
        [Display(Name = "Order Id")]
        public int OrderID { get; set; }

        [Remote("IsOrderNumberAvailable", "Order", AdditionalFields = "OrderID", ErrorMessage = "Order No already taken, please regenerate")]
        //[Required]
        //[RegularExpression("^.{3}[0-9]{8}$", ErrorMessage = "Order No must be 11 characters long and last eight digits should be numeric")]
        // order number sent from customer
        [Display(Name = "Order No.")]
        public string OrderNumber { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Issue Date")]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Required Date")]
        public DateTime? ExpectedDate { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        [Display(Name = "Transaction Type")]
        public InventoryTransactionTypeEnum InventoryTransactionTypeId { get; set; }

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

        public int? AccountAddressId { get; set; }

        public int? PickerId { get; set; }

        public bool IsCancel { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "Direct Ship")]
        public bool? DirectShip { get; set; }

        [Display(Name = "Order Status")]
        public OrderStatusEnum OrderStatusID { get; set; }

        [Display(Name = "Loan Type")]
        public int? LoanID { get; set; }

        // Account contact person for this specific order
        [Display(Name = "Account Contact")]
        public int? AccountContactId { get; set; }

        [Display(Name = "Discount")]
        public decimal OrderDiscount { get; set; }

        [Display(Name = "Order Total")]
        public decimal OrderTotal { get; set; }

        // flag for Posted into accounting software eg. Sage
        [Display(Name = "Posted for Invoicing")]
        public bool Posted { get; set; }

        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }

        [Display(Name = "External Order Number")]
        public string ExternalOrderNumber { get; set; }

        [Display(Name = "Invoice Details")]
        public string InvoiceDetails { get; set; }

        [Display(Name = "Invoice Cost")]
        public decimal? OrderCost { get; set; }

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

        public virtual Account Account { get; set; }

        [ForeignKey("PickerId")]
        public virtual AuthUser Picker { get; set; }

        [ForeignKey("TenentId")]
        public virtual Tenant Tenant { get; set; }

        public int? WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual TenantLocations Warehouse { get; set; }

        public virtual AccountContacts AccountContacts { get; set; }
        public string ShipmentAddressLine1 { get; set; }
        public string ShipmentAddressLine2 { get; set; }
        public string ShipmentAddressLine3 { get; set; }
        public string ShipmentAddressTown { get; set; }
        public string ShipmentAddressPostcode { get; set; }
        public int? ShipmentCountryId { get; set; }

        [ForeignKey("ShipmentCountryId")]
        public virtual GlobalCountry ShipmentCountry { get; set; }

        [Display(Name = "Property")]
        public int? PPropertyId { get; set; }

        public int? ShipmentPropertyId { get; set; }

        [ForeignKey("ShipmentPropertyId")]
        public virtual PProperty ShipmentProperty { get; set; }

        public Guid? OrderGroupToken { get; set; }

        public int? ShipmentWarehouseId { get; set; }

        [ForeignKey("ShipmentWarehouseId")]
        public virtual TenantLocations ShipmentWarehouse { get; set; }

        public bool IsShippedToTenantMainLocation { get; set; }

        public bool IsCollectionFromCustomerSide { get; set; }

        public string CustomEmailRecipient { get; set; }
        public string CustomCCEmailRecipient { get; set; }
        public string CustomBCCEmailRecipient { get; set; }

        public int? AccountCurrencyID { get; set; }

        [ForeignKey("AccountCurrencyID")]
        public virtual GlobalCurrency AccountCurrency { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<OrderProcess> OrderProcess { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransaction { get; set; }

        [ForeignKey("TransferWarehouseId")]
        public virtual TenantLocations TransferWarehouse { get; set; }

        [ForeignKey("PPropertyId")]
        public virtual PProperty PProperties { get; set; }

        public virtual JobType JobType { get; set; }
        public virtual TenantDepartments Department { get; set; }
        public virtual SLAPriorit SLAPriority { get; set; }
        public virtual ICollection<Appointments> Appointmentses { get; set; }

        public int? JobSubTypeId { get; set; }

        [ForeignKey("JobSubTypeId")]
        public virtual JobSubType JobSubType { get; set; }

        public virtual ICollection<OrderNotes> OrderNotes { get; set; }

        public virtual ReportType Report_Type { get; set; }

        public int? ShipmentAccountAddressId { get; set; }

        [ForeignKey("ShipmentAccountAddressId")]
        public virtual AccountAddresses ShipmentAccountAddress { get; set; }

        public DeliveryMethods? DeliveryMethod { get; set; }

        [Display(Name = "Delivery Service")]
        public int? TenantDeliveryServiceId { get; set; }

        [ForeignKey("TenantDeliveryServiceId")]
        public virtual TenantDeliveryService TenantDeliveryService { get; set; }

        public decimal? AmountPaidByAccount { get; set; }
        public decimal? AccountBalanceBeforePayment { get; set; }
        public decimal? AccountBalanceOnPayment { get; set; }

        [Display(Name = "Prestashop Id")]
        public int? PrestaShopOrderId { get; set; }

        [Display(Name = "Site Id")]
        public int? SiteID { get; set; }

        public AccountPaymentModeEnum? AccountPaymentModeId { get; set; }
        public bool EndOfDayGenerated { get; set; }
        public int? VanSalesDailyCashId { get; set; }

        [ForeignKey("VanSalesDailyCashId")]
        public virtual VanSalesDailyCash VanSalesDailyCash { get; set; }

        public Guid? OrderToken { get; set; }

        public int? BaseOrderID { get; set; }

        [ForeignKey("BaseOrderID")]
        public virtual Order BaseOrder { get; set; }

        public virtual IEnumerable<Order> RelatedOrders { get; set; }
    }
}