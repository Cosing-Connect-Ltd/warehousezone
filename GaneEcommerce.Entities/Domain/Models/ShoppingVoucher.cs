using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Entities.Domain
{
    public class ShoppingVoucher : BaseAuditInfo
    {
        [Key]
        [Display(Name = "Voucher Id")]
        public int ShoppingVoucherId { get; set; }
        [Display(Name = "Voucher Title")]
        public string VoucherTitle { get; set; }
        [Display(Name = "Voucher Code")]
        public string VoucherCode { get; set; }

        [Display(Name = "User (If voucher is user specific)")]
        public int? VoucherUserId { get; set; }
        [ForeignKey("VoucherUserId")]
        public virtual AuthUser VoucherUser { get; set; }

        [Display(Name = "Discount Type")]
        public ShoppingVoucherDiscountTypeEnum DiscountType { get; set; }

        [Display(Name = "Discount")]
        public decimal DiscountFigure { get; set; }
        public DateTime? VoucherUsedDate { get; set; }
        public DateTime? VoucherExpiryDate { get; set; }
        public virtual ICollection<ShoppingVoucherUsage> OrderDetails { get; set; }
        public int? MaximumAllowedUse { get; set; }
        public int? VoucherUsedCount { get; set; }
    }

    public class ShoppingVoucherUsage : BaseAuditInfo
    {
        [Key]
        [Display(Name = "Voucher Usage Id")]
        public int ShoppingVoucherUsageId { get; set; }
        
        public int ShoppingVoucherId { get; set; }
        [ForeignKey("ShoppingVoucherId")]
        public virtual ShoppingVoucher ShoppingVoucher { get; set; }

        public int? VoucherUserId { get; set; }
        [ForeignKey("VoucherUserId")]
        public virtual AuthUser VoucherUser { get; set; }
        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order{ get; set; }

    }
}