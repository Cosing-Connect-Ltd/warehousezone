﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Entities.Domain
{
    public class VoucherRewardPoints
    {
        [Key]
        [Display(Name = "Reward Id")]
        public int VoucherRewardPointsId { get; set; }
        public string RewardTitle { get; set; }
        public string RewardDescription { get; set; }
        public int RewardPoints { get; set; }

        public int? RewardShoppingVoucherId { get; set; }
        [ForeignKey("RewardShoppingVoucherId")]
        public virtual ShoppingVoucher RewardShoppingVoucher { get; set; }

        public int? RewardProductId { get; set; }
        [ForeignKey("RewardProductId")]
        public virtual ProductMaster RewardProduct { get; set; } 

        public bool IsDeleted { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }

    [Serializable]
    public class ShoppingVoucher : BaseAuditInfo
    {
        [Key]
        [Display(Name = "Voucher Id")]
        public int ShoppingVoucherId { get; set; }
        [Display(Name = "Voucher Title")]
        [Required]
        public string VoucherTitle { get; set; }
        [Display(Name = "Voucher Description")]
        public string RewardDescription { get; set; }

        [Display(Name = "Voucher Code")]
        public string VoucherCode { get; set; }

        [Display(Name = "User (If voucher is user specific)")]
        public int? VoucherUserId { get; set; }
        [ForeignKey("VoucherUserId")]
        public virtual AuthUser VoucherUser { get; set; }

        [Required]
        [Display(Name = "Discount Type")]
        public ShoppingVoucherDiscountTypeEnum DiscountType { get; set; }

        [Required]
        [Display(Name = "Discount")]
        public decimal DiscountFigure { get; set; }
        public DateTime? VoucherUsedDate { get; set; }
        public DateTime? VoucherExpiryDate { get; set; }
        public virtual ICollection<ShoppingVoucherUsage> OrderDetails { get; set; }
        public int? MaximumAllowedUse { get; set; }
        public int? VoucherUsedCount { get; set; }
        public int? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }

        public int? WarehouseId { get; set; }
        public virtual TenantLocations Warehouse { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal? MinimumOrderPrice { get; set; }
        public int? RewardProductId { get; set; }
        [ForeignKey("RewardProductId")]
        public virtual ProductMaster RewardProduct { get; set; }

        public RewardProductCategoryEnum? RewardProductCategory { get; set; }
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

    [Table("RewardPointTriggers")]
    public class RewardPointTrigger : BaseAuditInfo
    {
        [Key]
        public int RewardPointTriggerId { get; set; }
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Type")]
        public VoucherDiscountTriggerEnum TriggerType { get; set; }
        [Display(Name = "Reward Voucher")]
        public int ShoppingVoucherId { get; set; }
        [ForeignKey("ShoppingVoucherId")]
        public virtual ShoppingVoucher ShoppingVoucher { get; set; }

        //Limitations
        public int? LoyaltyPointToTrigger { get; set; }
        public decimal? MinimumOrderValueToTrigger { get; set; }
        public FoodOrderTypeEnum? FoodOrderTypeToTrigger { get; set; }
        public int? MaximumAllowed { get; set; }
        public int? TenantId { get; set; }
        public int? WarehouseId { get; set; }
        public bool? IsDeleted { get; set; }

    }
}