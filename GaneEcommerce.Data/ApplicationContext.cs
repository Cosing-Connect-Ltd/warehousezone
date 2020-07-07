using Ganedata.Core.Entities.Domain;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Ganedata.Core.Data
{

    public class ApplicationContext : DbContext, IApplicationContext
    {
        public ApplicationContext()
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Database.CommandTimeout = 60;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Entity<Order>().HasIndex(d => d.OrderNumber).IsUnique();
            Database.SetInitializer<ApplicationContext>(null);
            base.OnModelCreating(modelBuilder);
        }


        #region "Common Entities"

        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserLogin> AuthUsersLogin { get; set; }
        public DbSet<AuthUserLoginActivity> AuthUsersLoginActivities { get; set; }
        public DbSet<AuthUserprofile> AuthUserprofiles { get; set; }
        public DbSet<AuthPermission> AuthPermissions { get; set; }
        public DbSet<AuthActivity> AuthActivities { get; set; }
        public DbSet<AuthActivityGroup> AuthActivityGroups { get; set; }
        public DbSet<AuthActivityGroupMap> AuthActivityGroupMaps { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<GlobalCountry> GlobalCountries { get; set; }
        public DbSet<GlobalCurrency> GlobalCurrencies { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantLocations> TenantWarehouses { get; set; }

        #endregion

        #region "T&A Entities"
        public DbSet<Resources> Resources { get; set; }
        public DbSet<ContactNumbers> ContactNumbers { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<ResourceRequests> ResourceHolidays { get; set; }
        public DbSet<EmployeeShifts_Stores> EmployeeShifts_Stores { get; set; }
        public DbSet<AttLogs> AttLogs { get; set; }
        public DbSet<Shifts> Shifts { get; set; }
        public DbSet<EmployeeRoles> EmployeeRoles { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<EmployeeGroups> EmployeeGroups { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<AttLogsStamps> AttLogsStamps { get; set; }
        public DbSet<OperLogs> OperLogs { get; set; }
        #endregion

        //#region "WMS Entities"
        public DbSet<Account> Account { get; set; }
        public DbSet<AccountAddresses> AccountAddresses { get; set; }
        public DbSet<AccountStatusAudit> AccountStatusAudits { get; set; }
        public DbSet<AccountContacts> AccountContacts { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }
        public DbSet<ResourceShifts> ResourceShifts { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<MarketRouteSchedule> MarketRouteSchedules { get; set; }
        public DbSet<OrderConsignmentTypes> ConsignmentTypes { get; set; }
        public DbSet<GlobalUOM> GlobalUOM { get; set; }
        public DbSet<GlobalUOMTypes> GlobalUOMTypes { get; set; }
        public DbSet<GlobalWeightGroups> GlobalWeightGroups { get; set; }
        public DbSet<TenantLoanTypes> TenantLoanTypes { get; set; }
        public DbSet<GlobalTax> GlobalTax { get; set; }
        public DbSet<TenantWarranty> TenantWarranty { get; set; }
        public DbSet<TenantPriceGroups> TenantPriceGroups { get; set; }
        public DbSet<TenantDepartments> TenantDepartments { get; set; }
        public DbSet<ProductGroups> ProductGroups { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<InventoryStock> InventoryStocks { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<TenantDeliveryService> TenantDeliveryServices { get; set; }
        public DbSet<ApiCredentials> ApiCredentials { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<JobSubType> JobSubTypes { get; set; }
        public DbSet<Locations> Locations { get; set; }
        public DbSet<LocationGroup> LocationGroups { get; set; }
        public DbSet<LocationTypes> LocationTypes { get; set; }

        public DbSet<ProductKitType> ProductKitTypes { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderNotes> OrderNotes { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<OrderProcess> OrderProcess { get; set; }
        public DbSet<OrderProcessDetail> OrderProcessDetail { get; set; }
        public DbSet<InvoiceMaster> InvoiceMasters { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<OrderPTenantEmailRecipient> OrderPTenantEmailRecipients { get; set; }
        public DbSet<ProductLocations> ProductLocationsMap { get; set; }
        public DbSet<ProductAttributes> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValues> ProductAttributeValues { get; set; }
        public DbSet<ProductAttributeValuesMap> ProductAttributeValuesMap { get; set; }
        public DbSet<ProductLotOptionsCodes> ProductLotOptionsCodes { get; set; }
        public DbSet<ProductLotProcessTypeCodes> ProductLotProcessTypeCodes { get; set; }
        public DbSet<ProductMaster> ProductMaster { get; set; }

        public DbSet<ProductSCCCodes> ProductSCCCodes { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<ProductSerialis> ProductSerialization { get; set; }
        public DbSet<ProductAccountCodes> ProductAccountCodes { get; set; }
        public DbSet<ProductKitMap> ProductKitMaps { get; set; }
        public DbSet<TenantPriceGroupDetail> ProductSpecialPrices { get; set; }
        public DbSet<PTenant> PTenants { get; set; }
        public DbSet<PLandlord> PLandlords { get; set; }
        public DbSet<PProperty> PProperties { get; set; }
        public DbSet<ProductLocationStockLevel> ProductLocationStockLevels { get; set; }
        public DbSet<PSyncHistory> PSyncHistories { get; set; }
        public DbSet<WastageReason> WastageReasons { get; set; }
        public DbSet<ReportType> ReportTypes { get; set; }
        public DbSet<MarketJobAllocation> MarketJobAllocations { get; set; }
        public DbSet<StockTake> StockTakes { get; set; }
        public DbSet<StockTakeDetail> StockTakeDetails { get; set; }
        public DbSet<StockTakeDetailsSerial> StockTakeDetailsSerials { get; set; }
        public DbSet<StockTakeSnapshot> StockTakeSnapshot { get; set; }
        public DbSet<StockTakeSerialSnapshot> StockTakeSerialSnapshots { get; set; }
        public DbSet<StockTakeScanLog> StockTakeScanLogs { get; set; }
        public DbSet<SLAPriorit> SLAPriorities { get; set; }
        public DbSet<TenantContact> TenantContact { get; set; }
        public DbSet<TenantEmailNotificationQueue> TenantEmailNotificationQueues { get; set; }
        public DbSet<TenantEmailConfig> TenantEmailConfigs { get; set; }
        public DbSet<TenantEmailTemplates> TenantEmailTemplates { get; set; }
        public DbSet<TenantEmailTemplateVariable> TenantEmailTemplateVariables { get; set; }
        public DbSet<TenantModules> TenantModules { get; set; }
        public DbSet<TenantProfile> TenantProfile { get; set; }
        public DbSet<Terminals> Terminals { get; set; }
        public DbSet<TerminalsLog> TerminalsLog { get; set; }
        public DbSet<TenantCurrencies> TenantCurrencies { get; set; }
        public DbSet<TenantConfig> TenantConfigs { get; set; }
        public DbSet<TenantCurrenciesExRates> TenantCurrenciesExRates { get; set; }

        //MARKETS
        public DbSet<Market> Markets { get; set; }
        public DbSet<MarketRoute> MarketRoutes { get; set; }
        public DbSet<MarketVehicle> MarketVehicles { get; set; }
        public DbSet<MarketJob> MarketJobs { get; set; }
        public DbSet<MarketCustomer> MarketCustomers { get; set; }
        public DbSet<MarketRouteMap> MarketRouteMap { get; set; }

        //Palleting
        public DbSet<Pallet> Pallets { get; set; }
        public DbSet<PalletProduct> PalletProducts { get; set; }
        public DbSet<PalletsDispatch> PalletsDispatches { get; set; }

        //Vehicle Inspections
        public DbSet<VehicleInspectionType> VehicleInspectionTypes { get; set; }
        public DbSet<VehicleInspectionCheckList> VehicleInspectionCheckLists { get; set; }
        public DbSet<VehicleInspection> VehicleInspections { get; set; }
        public DbSet<VehicleInspectionConfirmedList> VehicleInspectionConfirmedLists { get; set; }
        public DbSet<AccountTransactionFile> AccountTransactionFiles { get; set; }
        public DbSet<ProductMarketStockLevel> ProductMarketStockLevel { get; set; }
        public DbSet<MarketRouteProgress> MarketRouteProgresses { get; set; }
        public DbSet<VanSalesDailyCash> VanSalesDailyCashes { get; set; }
        public DbSet<TerminalGeoLocation> TerminalGeoLocation { get; set; }
        public DbSet<PalletTracking> PalletTracking { get; set; }
        public DbSet<TextTranslations> TextTranslations { get; set; }
        public DbSet<StockTakeDetailsPallets> StockTakeDetailsPallets { get; set; }
        public DbSet<StockTakePalletsSnapshot> StockTakePalletsSnapshot { get; set; }
        public DbSet<OrderReceiveCount> OrderReceiveCount { get; set; }
        public DbSet<OrderReceiveCountDetail> OrderReceiveCountDetail { get; set; }
        public DbSet<OrderProofOfDelivery> OrderProofOfDelivery { get; set; }
        public DbSet<TerminalCommands> TerminalCommands { get; set; }
        public DbSet<TerminalCommandsQueue> TerminalCommandsQueue { get; set; }
        public DbSet<Assets> Assets { get; set; }
        public DbSet<AssetLog> AssetLog { get; set; }
        public DbSet<TerminalsTransactionsLog> TerminalsTransactionsLog { get; set; }
        public DbSet<ProductFiles> ProductFiles { get; set; }

        public DbSet<PalletType> PalletTypes { get; set; }
        public DbSet<TenantWebsites> TenantWebsites { get; set; }

        public DbSet<OrderSchedule> OrderSchedule { get; set; }
        public DbSet<ProductsWebsitesMap> ProductsWebsitesMap { get; set; }

        public DbSet<ProductManufacturer> ProductManufacturers { get; set; }

        public DbSet<WebsiteContentPages> WebsiteContentPages { get; set; }

        public DbSet<WebsiteNavigation> WebsiteNavigations { get; set; }

        public DbSet<WebsiteSlider> WebsiteSliders { get; set; }

        public DbSet<ProductsNavigationMap> ProductsNavigationMaps { get; set; }
        public DbSet<WebsiteVouchers> WebsiteVouchers { get; set; }
        public DbSet<WebsiteShippingRules> WebsiteShippingRules { get; set; }
        public DbSet<WebsiteDiscountCodes> WebsiteDiscountCodes { get; set; }
        public DbSet<WebsiteDiscountProductsMap> websiteDiscountProductsMaps { get; set; }
        public DbSet<AuthUserGroups> AuthUserGroups { get; set; }
        public DbSet<ProductAllowance> ProductAllowance { get; set; }
        public DbSet<ProductAllowanceAdjustmentLog> ProductAllowanceAdjustmentLog { get; set; }
        public DbSet<ProductAllowanceGroup> ProductAllowanceGroup { get; set; }
        public DbSet<ProductAllowanceGroupMap> ProductAllowanceGroupMap { get; set; }
        public DbSet<Tooltip> Tooltips { get; set; }
        public DbSet<WebsiteCartItem> WebsiteCartItems { get; set; }
        public DbSet<WebsiteWishListItem> WebsiteWishListItems { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductTagMap> ProductTagMaps { get; set; }
        public DbSet<ProductAttributeMap> ProductAttributeMaps { get; set; }
        public DbSet<UISetting> UISettings { get; set; }
        public DbSet<UISettingItem> UISettingItems { get; set; }
        public DbSet<WebsiteWarehouses> WebsiteWarehouses { get; set; }
        public DbSet<AbandonedCartNotification> AbandonedCartNotifications { get; set; }
        public DbSet<AbandonedCartSetting> AbandonedCartSettings { get; set; }
        public DbSet<WebsiteLayoutSettings> WebsiteLayoutSettings { get; set; }
        public DbSet<KitProductCartItem> KitProductCartItems { get; set; }

        //#end region
    }

}