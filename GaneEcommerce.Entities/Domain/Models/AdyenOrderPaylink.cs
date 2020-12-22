using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string RawJson { get; set; }
    }
}