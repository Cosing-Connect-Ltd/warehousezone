using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public Double LateTime { get; set; }
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
        public string InvoiceNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime? Date { get; set; }
        public decimal? NetAmtB { get; set; }
        public decimal? NetAmtS { get; set; }
        public decimal? Profit { get; set; }
        public decimal? TotalNetAmtB { get; set; }
        public decimal? TotalNetAmtS { get; set; }
        public decimal? TotalProfit { get; set; }
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