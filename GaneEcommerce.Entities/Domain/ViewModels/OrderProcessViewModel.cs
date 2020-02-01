using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{

    public class OrderPriceViewModel
    {
        public int ProductId { get; set; }
        public string CompanyName { get; set; }
        public string CurrencyName { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public int OrderID { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderProcessInvoicingViewModel
    {
        public List<OrderProcessViewModel> OrderProcessViewModels { get; set; }
        public int ResultCount => OrderProcessViewModels.Count;
    }

    public class OrderProcessViewModel
    {
        public int OrderProcessID { get; set; }
        public string DeliveryNO { get; set; }
        public DateTime DateCreated { get; set; }
        public string PONumber { get; set; }
        public string Supplier { get; set; }

        public string InvoiceNumber { get; set; }
        public List<OrderProcessDetailsViewModel> OrderProcessDetails { get; set; }

        public decimal? InvoiceTotal { get; set; }

        public string Email { get; set; }

        public int? AccountId { get; set; }

        public DateTime? InvoiceDate { get; set; }
    }

    public class OrderProcessDetailsViewModel
    {
        public int OrderProcessId { get; set; }
        public int OrderProcessDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public decimal QtyProcessed { get; set; }
        public decimal Price { get; set; }
        public decimal TaxAmount
        {
            get { return Math.Round((TotalAmount / 100) * TaxPercent, 2); }
        }
        public decimal TaxPercent { get; set; }
        public decimal TotalAmount => (Price * QtyProcessed);
        public decimal NetAmount => TotalAmount + TaxAmount + WarrantyAmount;
        public decimal WarrantyAmount { get; set; }

        public decimal? TaxAmountsInvoice { get; set; }

        public DateTime ProcessedDate { get; set; }

        public int? TaxId { get; set; }
        public int? WarrantyId { get; set; }

        public int? OrderDetailId { get; set; }

        public decimal? MinSellPrice { get; set; }
    }


}