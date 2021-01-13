using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ganedata.Core.Models.AdyenPayments;

namespace Ganedata.Core.Entities.Domain
{
    public class AdyenOrderPaylink
    {
        public AdyenOrderPaylink()
        {
            HookCreatedDate = DateTime.Now;
        }

        [Key]
        public int AdyenOrderPaylinkID { get; set; }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Order Order { get; set; }
        
        public string LinkID { get; set; }
        public string LinkUrl { get; set; }
        public DateTime? LinkExpiryDate { get; set; }
        public decimal LinkAmount { get; set; }
        public string LinkAmountCurrency { get; set; }
        public string LinkOrderDescription { get; set; }
        public string LinkShopperReference { get; set; }
        public string LinkPaymentReference { get; set; }
        public string LinkMerchantAccount { get; set; }
        public bool LinkStorePaymentMethod { get; set; }
        public string LinkRecurringProcessingModel { get; set; }

        public string HookEventCode { get; set; }
        public string HookPspReference { get; set; }
        public bool HookSuccess { get; set; }
        public string HookAmountCurrency { get; set; }
        public decimal HookAmountPaid { get; set; }
        public string HookMerchantOrderReference { get; set; }
        public DateTime HookCreatedDate { get; set; }
        public string HookRawJson { get; set; }

        public int? RefundRequestedUserID { get; set; }
        public string RefundMerchantReference { get; set; }
        public string RefundOriginalMerchantReference { get; set; }
        public decimal? RefundRequestedAmount { get; set; } 
        public string RefundRequestedAmountCurrency{ get; set; }
        public DateTime? RefundRequestedDateTime { get; set; }
        public DateTime? RefundProcessedDateTime { get; set; }

        public string RefundHookEventCode { get; set; }
        public string RefundHookPspReference { get; set; }
        public bool? RefundHookSuccess { get; set; }
        public string RefundHookAmountCurrency { get; set; }
        public decimal? RefundHookAmountPaid { get; set; }
        public string RefundHookMerchantOrderReference { get; set; }
        public DateTime? RefundHookCreatedDate { get; set; }
        public string RefundHookReason { get; set; }

    }
}