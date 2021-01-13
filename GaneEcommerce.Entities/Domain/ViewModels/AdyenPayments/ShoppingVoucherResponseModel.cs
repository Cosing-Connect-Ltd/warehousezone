using System;
using Ganedata.Core.Entities.Enums;

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
        public DateTime VerifiedTimestamp{ get; set; }
        public int Status { get; set; }
    }

}