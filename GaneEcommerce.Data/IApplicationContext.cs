using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading;
using System.Threading.Tasks;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Data
{
    public interface IApplicationContext : IDbContext
    {
        DbSet<AuthUser> AuthUsers { get; set; }
        DbSet<AuthUserLogin> AuthUsersLogin { get; set; }
        DbSet<AuthUserLoginActivity> AuthUsersLoginActivities { get; set; }
        DbSet<AuthUserprofile> AuthUserprofiles { get; set; }
        DbSet<AuthPermission> AuthPermissions { get; set; }
        DbSet<AuthActivity> AuthActivities { get; set; }
        DbSet<AuthActivityGroup> AuthActivityGroups { get; set; }
        DbSet<AuthActivityGroupMap> AuthActivityGroupMaps { get; set; }
        DbSet<Module> Modules { get; set; }
        DbSet<GlobalCountry> GlobalCountries { get; set; }
        DbSet<GlobalCurrency> GlobalCurrencies { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        DbSet<TenantLocations> TenantWarehouses { get; set; }
        DbSet<Resources> Resources { get; set; }
        DbSet<ContactNumbers> ContactNumbers { get; set; }
        DbSet<Address> Address { get; set; }
        DbSet<ResourceRequests> ResourceHolidays { get; set; }
        DbSet<EmployeeShifts_Stores> EmployeeShifts_Stores { get; set; }
        DbSet<AttLogs> AttLogs { get; set; }
        DbSet<Shifts> Shifts { get; set; }
        DbSet<EmployeeRoles> EmployeeRoles { get; set; }
        DbSet<Roles> Roles { get; set; }
        DbSet<EmployeeGroups> EmployeeGroups { get; set; }
        DbSet<Groups> Groups { get; set; }
        DbSet<AttLogsStamps> AttLogsStamps { get; set; }
        DbSet<OperLogs> OperLogs { get; set; }
        DbSet<Account> Account { get; set; }
        DbSet<AccountAddresses> AccountAddresses { get; set; }
        DbSet<AccountStatusAudit> AccountStatusAudits { get; set; }
        DbSet<AccountContacts> AccountContacts { get; set; }
        DbSet<AccountTransaction> AccountTransactions { get; set; }
        DbSet<AccountTransactionType> AccountTransactionTypes { get; set; }
        DbSet<AccountPaymentMode> AccountPaymentModes { get; set; }
        DbSet<ResourceShifts> ResourceShifts { get; set; }
        DbSet<Appointments> Appointments { get; set; }
        DbSet<MarketRouteSchedule> MarketRouteSchedules { get; set; }
        DbSet<OrderConsignmentTypes> ConsignmentTypes { get; set; }

        DbSet<GlobalUOM> GlobalUOM { get; set; }
        DbSet<GlobalUOMTypes> GlobalUOMTypes { get; set; }
        DbSet<GlobalWeightGroups> GlobalWeightGroups { get; set; }
        DbSet<TenantLoanTypes> TenantLoanTypes { get; set; }
        DbSet<GlobalTax> GlobalTax { get; set; }
        DbSet<TenantWarranty> TenantWarranty { get; set; }
        DbSet<TenantPriceGroups> TenantPriceGroups { get; set; }
        DbSet<GlobalAccountStatus> GlobalAccountStatus { get; set; }
        DbSet<TenantDepartments> TenantDepartments { get; set; }
        DbSet<InventoryStock> InventoryStocks { get; set; }
        DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        DbSet<InventoryTransactionType> InventoryTransactionTypes { get; set; }
        DbSet<JobType> JobTypes { get; set; }
        DbSet<JobSubType> JobSubTypes { get; set; }
        DbSet<Locations> Locations { get; set; }
        DbSet<LocationGroup> LocationGroups { get; set; }
        DbSet<LocationTypes> LocationTypes { get; set; }
        DbSet<ProductKitType> ProductKitTypes { get; set; }
        DbSet<Order> Order { get; set; }
        DbSet<OrderNotes> OrderNotes { get; set; }
        DbSet<OrderDetail> OrderDetail { get; set; }
        DbSet<OrderStatus> OrderStatus { get; set; }
        DbSet<OrderProcess> OrderProcess { get; set; }
        DbSet<OrderProcessStatus> OrderProcessStatuses { get; set; }
        DbSet<OrderProcessDetail> OrderProcessDetail { get; set; }
        DbSet<InvoiceMaster> InvoiceMasters { get; set; }
        DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        DbSet<OrderPTenantEmailRecipient> OrderPTenantEmailRecipients { get; set; }
        DbSet<PalletsDispatch> PalletsDispatches { get; set; }
        DbSet<Pallet> Pallets { get; set; }
        DbSet<PalletProduct> PalletProducts { get; set; }
        DbSet<ProductLocations> ProductLocationsMap { get; set; }
        DbSet<ProductAttributes> ProductAttributes { get; set; }
        DbSet<ProductGroups> ProductGroups { get; set; }
        DbSet<ProductCategory> ProductCategories { get; set; }
        DbSet<ProductAttributeValues> ProductAttributeValues { get; set; }
        DbSet<ProductAttributeValuesMap> ProductAttributeValuesMap { get; set; }
        DbSet<ProductLotOptionsCodes> ProductLotOptionsCodes { get; set; }
        DbSet<ProductLotProcessTypeCodes> ProductLotProcessTypeCodes { get; set; }
        DbSet<ProductMaster> ProductMaster { get; set; }
        DbSet<ProductSCCCodes> ProductSCCCodes { get; set; }
        DbSet<ProductSerialis> ProductSerialization { get; set; }
        DbSet<ProductAccountCodes> ProductAccountCodes { get; set; }
        DbSet<ProductKitMap> ProductKitMaps { get; set; }
        DbSet<TenantPriceGroupDetail> ProductSpecialPrices { get; set; }
        DbSet<PTenant> PTenants { get; set; }
        DbSet<PLandlord> PLandlords { get; set; }
        DbSet<PProperty> PProperties { get; set; }
        DbSet<ProductLocationStockLevel> ProductLocationStockLevels { get; set; }
        DbSet<PSyncHistory> PSyncHistories { get; set; }
        DbSet<ReportType> ReportTypes { get; set; }
        DbSet<StockMovement> StockMovements { get; set; }
        DbSet<MarketJobAllocation> MarketJobAllocations { get; set; }
        DbSet<MarketRouteProgress> MarketRouteProgresses { get; set; }

        DbSet<StockTake> StockTakes { get; set; }
        DbSet<StockTakeDetail> StockTakeDetails { get; set; }
        DbSet<StockTakeDetailsSerial> StockTakeDetailsSerials { get; set; }
        DbSet<StockTakeSnapshot> StockTakeSnapshot { get; set; }
        DbSet<StockTakeSerialSnapshot> StockTakeSerialSnapshots { get; set; }
        DbSet<StockTakeScanLog> StockTakeScanLogs { get; set; }
        DbSet<SLAPriorit> SLAPriorities { get; set; }
        DbSet<TenantContact> TenantContact { get; set; }
        DbSet<TenantEmailNotificationQueue> TenantEmailNotificationQueues { get; set; }
        DbSet<TenantEmailConfig> TenantEmailConfigs { get; set; }
        DbSet<TenantEmailTemplates> TenantEmailTemplates { get; set; }
        DbSet<TenantEmailTemplateVariable> TenantEmailTemplateVariables { get; set; }
        DbSet<TenantProfile> TenantProfile { get; set; }
        DbSet<Terminals> Terminals { get; set; }
        DbSet<TerminalsLog> TerminalsLog { get; set; }
        DbSet<TenantCurrencies> TenantCurrencies { get; set; }
        DbSet<TenantConfig> TenantConfigs { get; set; }
        DbSet<TenantCurrenciesExRates> TenantCurrenciesExRates { get; set; }
        DbSet<TenantModules> TenantModules { get; set; }
        DbSet<WastageReason> WastageReasons { get; set; }
        DbSet<VanSalesDailyCash> VanSalesDailyCashes { get; set; }

        //MARKETS
        DbSet<Market> Markets { get; set; }
        DbSet<MarketRoute> MarketRoutes { get; set; }
        DbSet<MarketVehicle> MarketVehicles { get; set; }
        DbSet<MarketJob> MarketJobs { get; set; }
        DbSet<MarketJobStatus> MarketJobStatus { get; set; }
        DbSet<MarketCustomer> MarketCustomers { get; set; }
        DbSet<MarketRouteMap> MarketRouteMap { get; set; }

        //VehicleInspection
        DbSet<VehicleInspectionType> VehicleInspectionTypes { get; set; }
        DbSet<VehicleInspectionCheckList> VehicleInspectionCheckLists { get; set; }
        DbSet<VehicleInspection> VehicleInspections { get; set; }
        DbSet<VehicleInspectionConfirmedList> VehicleInspectionConfirmedLists { get; set; }
        DbSet<AccountTransactionFile> AccountTransactionFiles { get; set; }
        DbSet<ProductMarketStockLevel> ProductMarketStockLevel { get; set; }
        DbSet<TerminalGeoLocation> TerminalGeoLocation { get; set; }

        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IEnumerable<DbEntityValidationResult> GetValidationErrors();
        DbSet<PalletTracking> PalletTracking { get; set; }
        DbSet<TextTranslations> TextTranslations { get; set; }
        DbSet<StockTakeDetailsPallets> StockTakeDetailsPallets { get; set; }
        DbSet<StockTakePalletsSnapshot> StockTakePalletsSnapshot { get; set; }
        DbSet<OrderReceiveCount> OrderReceiveCount { get; set; }
        DbSet<OrderReceiveCountDetail> OrderReceiveCountDetail { get; set; }
        DbSet<OrderProofOfDelivery> OrderProofOfDelivery { get; set; }
        DbSet<TerminalCommands> TerminalCommands { get; set; }
        DbSet<TerminalCommandsQueue> TerminalCommandsQueue { get; set; }
        DbSet<Assets> Assets { get; set; }
        DbSet<AssetLog> AssetLog { get; set; }
        DbSet<TenantDeliveryService> TenantDeliveryServices { get; set; }
        DbSet<GlobalApi> GlobalApis { get; set; }
        DbSet<ProductManufacturer> ProductManufacturers { get; set; }
        DbSet<TerminalsTransactionsLog> TerminalsTransactionsLog { get; set; }
        DbSet<ProductFiles> ProductFiles { get; set; }
        DbSet<PalletType> PalletTypes { get; set; }
        DbSet<TenantWebsites> TenantWebsites { get; set; }
        DbSet<OrderSchedule> OrderSchedule { get; set; }
        DbSet<ProductsWebsitesMap> ProductsWebsitesMap { get; set; }
        DbSet<WebsiteContentPages> WebsiteContentPages { get; set; }
        DbSet<WebsiteNavigation> WebsiteNavigations { get; set; }
        DbSet<WebsiteSlider> WebsiteSliders { get; set; }

    }
}