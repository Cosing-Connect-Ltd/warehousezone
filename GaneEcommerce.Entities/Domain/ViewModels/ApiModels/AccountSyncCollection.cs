﻿using Ganedata.Core.Entities.Domain;
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

    public class UserAccountSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public AccountSync Account { get; set; }
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
        public int? PointsToNextReward { get; set; }
        public List<RecentRewardPointSync> RecentRewardPoints { get; set; }

    }

    public class RecentRewardPointSync
    {
        public  int OrderID { get; set; }
        public int PointsEarned { get; set; }
        public DateTime OrderDateTime { get; set; }
    }


    public class AccountAddressesSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<AccountAddressSync> Addresses { get; set; }
    }

    public class AccountAddressSync
    {
        public int AddressID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public int CountryID { get; set; }
        public int AccountID { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string SerialNo { get; set; }
        public bool? IsDefaultDeliveryAddress { get; set; } 
        public bool? IsDefaultBillingAddress { get; set; } 

    }

    public class AccountPasswordResetSync
    {
        public string EmailAddress { get; set; }
        public int TenantId { get; set; }
        public int WarehouseId { get; set; }
        public string TerminalSerialNo { get; set; }
    }

    public class AccountPasswordResetSyncResponse
    {
        public bool SentResetLinkSuccessfully { get; set; }
        public string FailureMessage { get; set; }
        public bool EmailRegistered { get; set; }
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
        public List<ProductSpecialAttributePriceSync> ProductAttributeVariations { get; set; }

    }

    public class ProductSpecialAttributePriceSync
    {

        //Size/Scoop that helps the UI to show its elements
        public LoyaltyProductAttributeTypeEnumSync ProductAttributeType { get; set; }

        //For Auditing purpose
        public int ProductAttributeId { get; set; }
        public string ProductAttributeName { get; set; }
      
        //This will uniquely identify the selected attribute per product
        public int ProductAttributeValueId { get; set; }
      
        public string ProductAttributeValueName { get; set; }
        
        public decimal? AttributeSpecificPrice { get; set; }
        public int SortOrder { get; set; }

    }

}