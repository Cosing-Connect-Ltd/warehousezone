using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ganedata.Core.Models;

namespace Ganedata.Core.Entities.Domain
{

    public class InvoiceExportModel
    {
        [AllowHtml]
        public string ExportHtml { get; set; }
        public int OrderProcessID { get; set; }
        public int InvoiceID { get; set; }
        public string OrderProcessIds { get; set; }
    }

    public class InvoiceDetailViewModel : BaseAuditInfo
    {
        public int InvoiceDetailId { get; set; }

        public int InvoiceId { get; set; }
         
        public int ProductId { get; set; }

        public int OrderProcessDetailId { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal Tax { get; set; }

        public decimal NetAmount { get; set; }

        public decimal WarrantyAmount { get; set; }

        public string SkuCode { get; set; }

        public int? TaxId { get; set; }

        public int? WarrantyId { get; set; }

        public int? Percentage { get; set; }
           

        
        

    }
    public class InvoiceViewModel : BaseAuditInfo
    {
        public InvoiceViewModel()
        {
            AllAccounts = new List<SelectListItem>();
            AllProducts = new List<SelectListItem>();
            AllInvoiceProducts = new List<OrderProcessDetailsViewModel>();
        }
        public int InvoiceMasterId { get; set; }

        public int AccountId { get; set; }
        [Display(Name = "Account")]
        public string AccountName { get; set; }

        public int OrderProcessId { get; set; }
        [Display(Name = "Net Amt")]
        public decimal NetAmount { get; set; }
        [Display(Name = "Tax Amt")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "War Amt")]
        public decimal WarrantyAmount { get; set; }

        public decimal CardCharges { get; set; }
        [Display(Name = "Postage")]
        public decimal PostageCharges { get; set; }
        [Display(Name = "Total")]
        public decimal InvoiceTotal { get; set; }
        [Display(Name = "Invoice No")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Order No")]
        public string OrderNumber { get; set; }

        public string InvoiceAddress { get; set; }

        public string InvoiceCurrency { get; set; }
        [Display(Name = "Date")]
        public DateTime InvoiceDate { get; set; }

        public int ProductId { get; set; }
        public int TaxId { get; set; }
        public int WarrantyId { get; set; }

        public string TaxDataHelper { get; set; }
        public string WarrantyDataHelper { get; set; }

        public List<SelectListItem> AllAccounts { get; set; }
        public List<SelectListItem> AllProducts { get; set; }
        public List<SelectListItem> AllTaxes { get; set; }
        public List<SelectListItem> AllWarranties { get; set; }
        public List<OrderProcessDetailsViewModel> AllInvoiceProducts { get; set; }
        public string TenantName { get; set; }
        public int TenantId { get; set; }
        public int? EmailCount { get; set; }
        public string Emails { get; set; }
      
    }
}