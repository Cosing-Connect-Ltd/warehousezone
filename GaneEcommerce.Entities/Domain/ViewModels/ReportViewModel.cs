using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public string Employee { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? ShiftStartTime { get; set; }
        public DateTime? ShiftEndTime { get; set; }
        public DateTime? StampIn { get; set; }
        public DateTime? StampOut { get; set; }
        public string LateTime { get; set; }
        public string OverTime { get; set; }
    }

    public class ExpensivePropertiseTotalsViewModel
    {
        public int PropertyId { get; set; }
        public decimal OrderTotal { get; set; }
    }


    public class WorksorderKpiReportViewModel
    {
        public string Sector { get; set; }
        public int Logged { get; set; }
        public int Completed { get; set; }
        public int Reallocated { get; set; }
        public int Unallocated { get; set; }
        public string OldestJob { get; set; }
    }
    public class InvoiceProfitReportViewModel
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime? Date { get; set; }
        public decimal? NetAmtB { get; set; }
        public decimal? NetAmtS { get; set; }
        public decimal? Profit { get; set; }
        public decimal? ProfitPercent { get; set; }
        public List<InvoiceProfitReportProductsViewModel> ProductsDetail { get; set; }
        public decimal? TotalNetAmtB { get; set; }
        public decimal? TotalNetAmtS { get; set; }
        public decimal? TotalProfit { get; set; }

        public decimal? TotalProfitPercent { get; set; }

    }
    public class StockShortageReportModel
    {
        public string OrderNumber { get; set; }
        public DateTime RDate { get; set; }
        public List<StockShortageReportDetail> StockReportDetails { get; set; }
    }
    public class StockShortageReportDetail
    {
        public string OrderNumber { get; set; }
        public DateTime RDate { get; set; }
        public string SkuCode { get; set; }
        public string ProductName { get; set; }
        public decimal Qty { get; set; }
        public decimal RemainingCases { get; set; }
        public decimal Shortage { get; set; }





    }

    public class ShortagePallets
    {
        public decimal RemainingCases { get; set; }




    }

    public class InvoiceProfitReportProductsViewModel
    {
        public int InvoiceId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal TotalBuyPrice { get; set; }
        public decimal TotalSellPrice { get; set; }
        public decimal? Profit { get; set; }
        public decimal? ProfitPercent { get; set; }
    }

    public class HolidayReportViewModel
    {
        public string UserName { get; set; }
        public DateTime? Date { get; set; }
        public string FirstYear { get; set; }
        public string SecondYear { get; set; }
        public string ThirdYear { get; set; }
        public string FourthYear { get; set; }
        public string FifthYear { get; set; }
    }

    public class ExpirableReportViewModel
    {
        public string ProductName { get; set; }
        public string SkuCode { get; set; }

        public List<ExpirableReportPallets> ExpirableReportPallets { get; set; }
    }
    public class ExpirableReportPallets
    {
        public string PalletSerial { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public decimal TotalCases { get; set; }

        public decimal RemainingCases { get; set; }
        public string Status { get; set; }

    }
}