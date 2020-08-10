using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class AccountSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<AccountSync> Accounts { get; set; }
    }
    public class AccountSync
    {
        public int AccountID { get; set; }
        public string AccountCode { get; set; }
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public int AccountStatusID { get; set; }
        public string PriceGroupName { get; set; }
        public int PriceGroupID { get; set; }
        public string VATNo { get; set; }
        public string RegNo { get; set; }
        public string Comments { get; set; }
        public string AccountEmail { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string website { get; set; }
        public double? CreditLimit { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public int TenantId { get; set; }
        public bool AccountTypeCustomer { get; set; }
        public bool AccountTypeSupplier { get; set; }
        public bool AccountTypeEndUser { get; set; }
        public int OwnerUserId { get; set; }
        public decimal FinalBalance { get; set; }
        public bool CashOnlyAccount { get; set; }
        public string FullAddress { get; set; }
        public int TaxID { get; set; }
        public decimal TaxPercent { get; set; }
        public int? AcceptedShelfLife { get; set; }
        public string AccountLoyaltyCode { get; set; }
        public int AccountLoyaltyPoints { get; set; }
    }

    public class TenantPriceGroupsSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<TenantPriceGroupsSync> TenantPriceGroupsSync { get; set; }
    }

    public class TenantPriceGroupsSync
    {
        public int PriceGroupID { get; set; }
        public string Name { get; set; }
        public decimal Percent { get; set; }
        public int TenantId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool? IsDeleted { get; set; }
        public bool ApplyDiscountOnTotal { get; set; }
        public bool ApplyDiscountOnSpecialPrice { get; set; }
    }


    public class TenantPriceGroupDetailSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<TenantPriceGroupDetailSync> TenantPriceGroupDetailSync { get; set; }
    }

    public class TenantPriceGroupDetailSync
    {
        public int PriceGroupDetailID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal SpecialPrice { get; set; }
        public int ProductID { get; set; }
        public int PriceGroupID { get; set; }
        public int TenantId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool? IsDeleted { get; set; }

    }


}