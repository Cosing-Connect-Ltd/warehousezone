using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("InvoiceMaster")]
    public class InvoiceMaster : PersistableEntity<int>
    {
        [Key]
        public int InvoiceMasterId { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal CardCharges { get; set; }
        public decimal PostageCharges { get; set; }
        public decimal WarrantyAmount { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal InvoiceTotal { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceAddress { get; set; }
        public string InvoiceCurrency { get; set; }
        public int CurrencyId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public InvoiceStatusEnum InvoiceStatus { get; set; }
        public int? OrderProcessId { get; set; }
        public virtual OrderProcess OrderProcess { get; set; }
        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new HashSet<InvoiceDetail>();
        [ForeignKey("CurrencyId")]
        public virtual GlobalCurrency GlobalCurrency { get; set; }
    }

    [Table("InvoiceDetail")]
    public class InvoiceDetail : PersistableEntity<int>
    {
        [Key]
        public int InvoiceDetailId { get; set; }

        public int InvoiceMasterId { get; set; }
        [ForeignKey("InvoiceMasterId")]
        public virtual InvoiceMaster Invoice { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster Product { get; set; }

        public int? OrderDetailId { get; set; }
        [ForeignKey("OrderDetailId")]
        public virtual OrderDetail OrderDetail { get; set; }
        public int? TaxId { get; set; }
        [ForeignKey("TaxId")]
        public virtual GlobalTax GlobalTax { get; set; }

        public string Description { get; set; }


        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
        public decimal WarrantyAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int? WarrantyId { get; set; }

    }

}