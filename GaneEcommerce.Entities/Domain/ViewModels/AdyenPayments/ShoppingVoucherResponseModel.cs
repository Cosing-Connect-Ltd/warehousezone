using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using Ganedata.Core.Entities.Enums;
using Braintree;

namespace Ganedata.Core.Models.AdyenPayments
{
    public class ShoppingVoucherValidationRequestModel
    {
        public string Username { get; set; }
        public string VoucherCode { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
    }
    public class ShoppingVoucherValidationResponseModel
    {
        public string VoucherCode { get; set; }
        public decimal DiscountFigure { get; set; }
        public int DiscountType { get; set; }

        //Can be used to determine if expiry becomes an issue
        public DateTime VerifiedTimestamp { get; set; }
        public int Status { get; set; }

        //Promotions
        public int UserId { get; set; }
        public string VoucherTitle { get; set; }
        public decimal MinimumOrderPrice { get; set; }
        public DateTime? UsedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? RewardProductId { get; set; }
        public RewardProductCategoryEnum? RewardProductCategory { get; set; }
    }

    public class PromotionsSyncRequest
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

}