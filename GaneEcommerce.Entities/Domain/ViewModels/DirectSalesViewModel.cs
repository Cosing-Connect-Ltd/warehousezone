using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ganedata.Core.Models;

namespace Ganedata.Core.Entities.Domain
{
    public class DirectSalesViewModel : BaseAuditInfo
    {
        public DirectSalesViewModel()
        {
            AllAccounts = new List<SelectListItem>();
            AllProducts = new List<SelectListItem>();
            AllPaymentModes = new List<SelectListItem>();
            AllTaxes = new List<SelectListItem>();
            AllWarranties = new List<SelectListItem>();
            AllInvoiceProducts = new List<DirectSaleProductsViewModel>();
        }
        public int DirectSalesOrderId { get; set; }

        public int AccountId { get; set; }
        public int PaymentModeId { get; set; }

        public string AccountName { get; set; }
         
        public decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal WarrantyAmount { get; set; }
        

        public decimal CardCharges { get; set; }

        public decimal InvoiceTotal { get; set; }

        public string OrderNumber { get; set; }

        public string InvoiceAddress { get; set; }

        public string InvoiceCurrency { get; set; }

        public int ProductId { get; set; }
        public int TaxId { get; set; }
        public int WarrantyId { get; set; }

        public string TaxDataHelper { get; set; }
        public string WarrantyDataHelper { get; set; }

        public decimal? DiscountAmount { get; set; }
        public List<SelectListItem> AllAccounts { get; set; }
        public List<SelectListItem> AllProducts { get; set; }
        public List<SelectListItem> AllTaxes { get; set; }
        public List<SelectListItem> AllWarranties { get; set; }
        public List<SelectListItem> AllPaymentModes { get; set; }
        public List<DirectSaleProductsViewModel> AllInvoiceProducts { get; set; }

        public string TenantName { get; set; }
        public int TenantId { get; set; }

        public decimal PaymentToday { get; set; }

    }

    public class DirectSaleProductsViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public decimal QtyProcessed { get; set; }

        public decimal Price { get; set; }

        public decimal TaxAmount => (TotalAmount / 100) * TaxPercent;
        public decimal TaxPercent { get; set; }
        public int? TaxId { get; set; }

        public decimal TaxAmounts { get;set; }

        public decimal TotalAmount => (Price * QtyProcessed);
        public decimal NetAmount => TotalAmount + TaxAmount + WarrantyAmount;
        public decimal WarrantyAmount { get; set; }

        public decimal DiscountAmount { get; set; }
        public int? WarrantyId { get; set; }

        public DateTime ProcessedDate { get; set; }
    }
}