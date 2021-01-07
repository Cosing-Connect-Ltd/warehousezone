using Ganedata.Core.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace Ganedata.Core.Entities.Domain
{
    public class TenantConfig : PersistableEntity<int>
    {
        [Key]
        public int TenantConfigId { get; set; }
        [Display(Name = "MinimumProductPrice")]
        public bool AlertMinimumProductPrice { get; set; }
        public bool EnforceMinimumProductPrice { get; set; }
        public string AlertMinimumPriceMessage { get; set; }
        public string EnforceMinimumPriceMessage { get; set; }
        public string PoReportFooterMsg1 { get; set; }
        public string PoReportFooterMsg2 { get; set; }
        public string SoReportFooterMsg1 { get; set; }
        public string SoReportFooterMsg2 { get; set; }
        public string DnReportFooterMsg1 { get; set; }
        public string DnReportFooterMsg2 { get; set; }
        [Display(Name = "WorksScheduleByMarginHours")]
        public int? WorksOrderScheduleByMarginHours { get; set; }
        public bool? WorksOrderScheduleByAmPm { get; set; }
        public bool? EnableLiveEmails { get; set; }
        public bool? MiniProfilerEnabled { get; set; }
        public bool? EnablePalletingOnPick { get; set; }
        public string WarehouseLogEmailsToDefault { get; set; }
        public string WarehouseScheduleStartTime { get; set; }
        public string WarehouseScheduleEndTime { get; set; }
        public string ErrorLogsForwardEmails { get; set; }
        public string DefaultReplyToAddress { get; set; }
        public string DefaultMailFromText { get; set; }
        public bool? AutoTransferStockEnabled { get; set; }
        public bool EnableStockVarianceAlerts { get; set; }
        public string AuthorisationAdminEmail { get; set; }
        public int? DefaultCashAccountID { get; set; }
        [ForeignKey("DefaultCashAccountID")]
        public virtual Account DefaultCashAccount { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public string TenantReceiptPrintHeaderLine1 { get; set; }
        public string TenantReceiptPrintHeaderLine2 { get; set; }
        public string TenantReceiptPrintHeaderLine3 { get; set; }
        public string TenantReceiptPrintHeaderLine4 { get; set; }
        public string TenantReceiptPrintHeaderLine5 { get; set; }
        public byte[] TenantLogo { get; set; }
        public bool PrintLogoForReceipts { get; set; }
        public short SessionTimeoutHours { get; set; }
        public bool AllowDuplicateBarcode { get; set; }
        public string ProductCatagories { get; set; }
        public string TenantAgreement { get; set; }
        public bool EnableTimberProperties { get; set; }
        public string DefaultCustomMessage { get; set; }
        public bool ShowDecimalPoint { get; set; }
        public int TerminalSyncDays { get; set; }
        public int ArchivableItemsAgeDays { get; set; }
        public string IvReportFooterMsg1 { get; set; }
        public bool EmailAutoCheckedOnEdit { get; set; }
        public bool AllowDuplicateProductName { get; set; }
        public bool OrdersAuthroisingRequired { get; set; }
        public bool AllowOrderStatusEmailConfigChange { get; set; }
        public bool AllowOrderStatusSmsConfigChange { get; set; }
        public int? SiteId { get; set; }
        [ForeignKey("SiteId")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        public bool MandatoryLocationScan { get; set; }

        [Display(Name = "Default Sales Nominal Code")]
        public int? DefaultSaleNominalCode { get; set; }
        [Display(Name = "Default Purchases Nominal Code")]
        public int? DefaultPurchaseNominalCode { get; set; }
        public bool ShowDeliveryServiceInOrdersList { get; set; }
        public bool ShowExternalShopSiteNameInOrdersList { get; set; }
        public bool EnableDynamicPriceCalculation { get; set; }
        public bool EnableRebateCalculation { get; set; }
        public bool EnableOrdersHistoryReportDetails { get; set; }
        public string LoyaltyAppSplashScreenImage { get; set; }
        public string LoyaltyAppPrimaryColour { get; set; }
        public string LoyaltyAppSecondaryColour { get; set; }
        public string LoyaltyAppPrimaryTextColour { get; set; }
        public string LoyaltyAppSecondaryTextColour { get; set; }
        public string LoyaltyAppAboutUsPageText { get; set; }
        public LoyaltyAppOrderProcessTypeEnum LoyaltyAppOrderProcessType { get; set; }

        public decimal? StandardDeliveryCost { get; set; }
        public decimal? NextDayDeliveryCost { get; set; }
    }
}