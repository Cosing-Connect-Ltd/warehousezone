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
        public List<InvoiceProfitReportProductsViewModel> ProductsDetail { get; set; }
        public decimal? TotalNetAmtB { get; set; }
        public decimal? TotalNetAmtS { get; set; }
        public decimal? TotalProfit { get; set; }

        public decimal? TotalProfitPercent { get; set; }
        
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
        public string  FirstYear { get; set; }
        public string SecondYear { get; set; }
        public string ThirdYear { get; set; }
        public string FourthYear { get; set; }
        public string FifthYear { get; set; }
    }
}