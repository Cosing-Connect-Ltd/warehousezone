//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ganedata.Warehouse.PropertiesSync.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Host_Prop_inf2
    {
        public int ID { get; set; }
        public string PCODE { get; set; }
        public bool OzGSTExemption { get; set; }
        public string VatNo { get; set; }
        public Nullable<System.DateTime> VATPeriodEndDate { get; set; }
        public bool CashAccounting { get; set; }
        public bool IncludeService { get; set; }
        public string VATPeriod { get; set; }
        public Nullable<int> TaxCategory1 { get; set; }
        public Nullable<int> TaxCategory2 { get; set; }
        public Nullable<int> TaxCategory3 { get; set; }
        public Nullable<int> TaxCategory4 { get; set; }
        public Nullable<int> TaxCategory5 { get; set; }
        public Nullable<int> TaxCategory6 { get; set; }
        public Nullable<int> TaxCategoryLetting { get; set; }
        public Nullable<int> TaxCategoryManagement { get; set; }
        public string FAPArea { get; set; }
        public bool PTenTopUp { get; set; }
        public string BS7666_SAON { get; set; }
        public string BS7666_PAON { get; set; }
        public string BS7666_Street { get; set; }
        public string BS7666_Locality { get; set; }
        public string BS7666_Town { get; set; }
        public string BS7666_County { get; set; }
        public string BS7666_PstCd { get; set; }
        public byte[] P2DateTimeStamp { get; set; }
        public string EnergyRatingCurrent { get; set; }
        public string EnergyRatingPotential { get; set; }
        public string CO2RatingCurrent { get; set; }
        public string CO2RatingPotential { get; set; }
        public string EPCOwner { get; set; }
        public bool EPCUseScotCharts { get; set; }
        public bool EPCUseCommercialChart { get; set; }
        public Nullable<System.DateTime> lastmodified { get; set; }
        public int PLetType { get; set; }
        public decimal PDepositAmount { get; set; }
        public Nullable<int> LettingFeeAccountID { get; set; }
        public Nullable<int> AdditionalFee1AccountID { get; set; }
        public Nullable<int> AdditionalFee2AccountID { get; set; }
        public Nullable<int> AdditionalFee3AccountID { get; set; }
        public Nullable<int> AdditionalFee4AccountID { get; set; }
        public Nullable<int> AdditionalFee5AccountID { get; set; }
        public Nullable<int> AdditionalFee6AccountID { get; set; }
        public bool LeaseDateNotUpdated { get; set; }
        public bool Section153 { get; set; }
        public bool Section158 { get; set; }
        public bool Section166 { get; set; }
        public Nullable<decimal> LeaseAmount { get; set; }
        public Nullable<System.DateTime> LeaseDate { get; set; }
        public string LeasePeriod { get; set; }
        public bool EnforcePeriodEndDate { get; set; }
        public Nullable<System.DateTime> PeriodEndDate { get; set; }
        public Nullable<int> PeriodFrequency { get; set; }
        public int MOBranch { get; set; }
        public bool MOShared { get; set; }
        public Nullable<int> InitialInvoiceType { get; set; }
        public Nullable<int> RentChargePeriodType { get; set; }
        public Nullable<int> RentDescriptionType { get; set; }
        public string ElectricityMeterSerial { get; set; }
        public string GasMeterSerial { get; set; }
        public string ElectricitySupplyNumber { get; set; }
        public string GasMeterPointReference { get; set; }
        public bool ElectricityPrepayment { get; set; }
        public bool GasPrepayment { get; set; }
        public string Last_Used_Image_Location { get; set; }
        public string Last_Used_Floorplan_Location { get; set; }
        public string Last_Used_VT_Location { get; set; }
        public Nullable<int> PropertyManagedTemplate { get; set; }
        public bool GasUtilityAvailable { get; set; }
        public bool UploadProperty { get; set; }
        public Nullable<double> StreetViewLatitude { get; set; }
        public Nullable<double> StreetViewLongditude { get; set; }
        public Nullable<double> StreetViewPitch { get; set; }
        public Nullable<double> StreetViewYaw { get; set; }
        public Nullable<int> StreetViewZoom { get; set; }
        public string PURL { get; set; }
        public Nullable<bool> AsbestosPresent { get; set; }
    }
}
