using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class VanSalesDailyCashSync
    {
        public int VanSalesDailyCashId { get; set; }
        public DateTime SaleDate { get; set; }
        public int TerminalId { get; set; }
        public int MobileLocationId { get; set; }
        public int SalesManUserId { get; set; }
        public int TenantId { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public int FiveHundred { get; set; }
        public int TwoHundred { get; set; }
        public int OneHundred { get; set; }
        public int Fifty { get; set; }
        public int Twenty { get; set; }
        public int Ten { get; set; }
        public int Five { get; set; }
        public int Two { get; set; }
        public int One { get; set; }
        public int PointFifty { get; set; }
        public int PointTwentyFive { get; set; }
        public int PointTwenty { get; set; }
        public int PointTen { get; set; }
        public int PointFive { get; set; }
        public int PointTwo { get; set; }
        public int PointOne { get; set; }
        public decimal TotalCashSale { get; set; }
        public decimal TotalCardSale { get; set; }
        public decimal TotalChequeSale { get; set; }
        public decimal TotalSale { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalNetSale { get; set; }
        public decimal TotalNetTax { get; set; }
        public decimal TotalPaidCash { get; set; }
        public decimal TotalPaidCheques { get; set; }
        public decimal TotalPaidCards { get; set; }
        public decimal TotalCashSubmitted { get; set; }
        public decimal TotalChequeSubmitted { get; set; }
        public decimal TotalCardSubmitted { get; set; }
        public DateTime SubmittedDate { get; set; }
        public int ChequesCount { get; set; }
        public int CashCount { get; set; }
        public int CardCount { get; set; }
        public string Notes { get; set; }
        public string SerialNumber { get; set; }
        public int UserId { get; set; }
        public string SalesManName { get; set; }
        public string VehicleName { get; set; }
        public string TerminalSerial { get; set; }
        public Guid TransactionLogId { get; set; }
        public List<Guid> OrderIds { get; set; }
        public decimal ReturnOrderValue { get; set; }
        public int ReturnOrderCount { get; set; }

        public string TotalPaidText
        {
            get
            {
                var results = new List<string>();
                var totals = 0.0m;
                if (TotalPaidCash > 0)
                {
                    results.Add("Cash   = " + TotalPaidCash.ToString("0.00"));
                    totals += TotalPaidCash;
                }
                if (TotalPaidCards > 0)
                {
                    results.Add("Card   = " + TotalPaidCards.ToString("0.00"));
                    totals += TotalPaidCards;
                }
                if (TotalPaidCheques > 0)
                {
                    results.Add("Cheques (" + ChequesCount + ") = " + TotalPaidCheques.ToString("0.00"));
                    totals += TotalPaidCheques;
                }
                if (totals > 0)
                {
                    results.Add("--------------");
                    results.Add("Total Paid = " + totals.ToString("0.00"));
                }
                return string.Join("<br/>", results);
            }
        }

        public string CashNoteDenominations
        {
            get
            {
                var results = new List<string>();
                var totals = 0.0m;
                if (FiveHundred > 0)
                {
                    results.Add("500 x " + FiveHundred.ToString());
                    totals += (500 * FiveHundred);
                }
                if (TwoHundred > 0)
                {
                    results.Add("200 x " + TwoHundred.ToString());
                    totals += (200 * TwoHundred);
                }
                if (OneHundred > 0)
                {
                    results.Add("100 x " + OneHundred.ToString());
                    totals += (100 * OneHundred);
                }
                if (Fifty > 0)
                {
                    results.Add(" 50 x " + Fifty.ToString());
                    totals += (50 * Fifty);
                }
                if (Twenty > 0)
                {
                    results.Add(" 20 x " + Twenty.ToString());
                    totals += (20 * Twenty);
                }
                if (Ten > 0)
                {
                    results.Add(" 10 x " + Ten.ToString());
                    totals += (10 * Ten);
                }
                if (Five > 0)
                {
                    results.Add("  5 x " + Five.ToString());
                    totals += (5 * Five);
                }
                if (Two > 0)
                {
                    results.Add("  2 x " + Two.ToString());
                    totals += (2 * Two);
                }
                if (One > 0)
                {
                    results.Add("  1 x " + One.ToString());
                    totals += One;
                }
                if (totals > 0)
                {
                    results.Add("--------------");
                    results.Add("Total = " + totals.ToString("0.00"));
                }
                return string.Join("<br/>", results);
            }
        }
        public string CashCoinDenominations
        {
            get
            {
                var results = new List<string>();
                var totals = 0.0m;
                if (PointFifty > 0)
                {
                    results.Add("0.50 x " + PointFifty.ToString());
                    totals += (0.5m * PointFifty);
                }
                if (PointTwentyFive > 0)
                {
                    results.Add("0.25 x " + PointTwentyFive.ToString());
                    totals += (0.25m * PointTwentyFive);
                }
                if (PointTwenty > 0)
                {
                    results.Add("0.20 x " + PointTwenty.ToString());
                    totals += (0.20m * PointTwenty);
                }
                if (PointTen > 0)
                {
                    results.Add("0.10 x " + PointTen.ToString());
                    totals += (0.10m * PointTen);
                }
                if (PointFive > 0)
                {
                    results.Add("0.05 x " + PointFive.ToString());
                    totals += (0.05m * PointFive);
                }
                if (PointTwo > 0)
                {
                    results.Add("0.02 x " + PointTwo.ToString());
                    totals += (0.02m * PointTwo);
                }
                if (PointOne > 0)
                {
                    results.Add("0.01 x " + PointOne.ToString());
                    totals += (0.01m * PointOne);
                }
                if (totals > 0)
                {
                    results.Add("--------------");
                    results.Add("Total = " + totals.ToString("0.00"));
                }
                return string.Join("<br/>", results);
            }
        }
    }
}