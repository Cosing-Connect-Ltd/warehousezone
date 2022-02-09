using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Services;
using Ganedata.Core.Services.Feedbacks;
using LazyCache;
using System;

using Unity;
using Unity.Injection;

namespace ShopOrderApp
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IApplicationContext, ApplicationContext>();

            container.RegisterType<IAccountServices, AccountServices>();
            container.RegisterType<ILookupServices, LookupServices>();
            container.RegisterType<IPropertyService, PropertyService>();
            container.RegisterType<IProductServices, ProductServices>();
            container.RegisterType<IProductLookupService, ProductLookupService>();
            container.RegisterType<ITenantsServices, TenantsServices>();
            container.RegisterType<IEmployeeShiftsServices, EmployeeShiftsServices>();
            container.RegisterType<IEmployeeServices, EmployeeServices>();
            container.RegisterType<ITenantLocationServices, TenantLocationServices>();
            container.RegisterType<IAddressServices, AddressServices>();
            container.RegisterType<IContactNumbersServices, ContactNumbersServices>();
            container.RegisterType<IEmployeeShiftsStoresServices, EmployeeShiftsStoresServices>();
            container.RegisterType<IAttLogsServices, AttLogsServices>();
            container.RegisterType<ITenantsServices, TenantsServices>();
            container.RegisterType<IShiftScheduleService, ShiftScheduleService>();
            container.RegisterType<IRolesServices, RolesServices>();
            container.RegisterType<IGroupsServices, GroupsServices>();
            container.RegisterType<IEmployeeRolesServices, EmployeeRolesServices>();
            container.RegisterType<IEmployeeGroupsServices, EmployeeGroupsServices>();
            container.RegisterType<IAttLogsStampsServices, AttLogsStampsServices>();
            container.RegisterType<IOperLogsServices, OperLogsServices>();
            container.RegisterType<IStockTakeApiService, StockTakeApiService>();
            container.RegisterType<IWarehouseSyncService, WarehouseSyncService>();
            container.RegisterType<IOrderService, OrderService>();
            container.RegisterType<IAppointmentsService, AppointmentsService>();
            container.RegisterType<IPurchaseOrderService, PurchaseOrderService>();
            container.RegisterType<IWorksOrderService, WorksOrderService>();
            container.RegisterType<ITransferOrderService, TransferOrderService>();
            container.RegisterType<ICoreOrderService, CoreOrderService>();
            container.RegisterType<IMarketServices, MarketServices>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IEmailNotificationService, EmailNotificationService>();
            container.RegisterType<IActivityServices, ActivityServices>();
            container.RegisterType<ITenantEmailConfigServices, TenantEmailConfigServices>();
            container.RegisterType<IAdminServices, AdminServices>();
            container.RegisterType<ICommonDbServices, CommonDbServices>();
            container.RegisterType<ISalesOrderService, SalesOrderService>();
            container.RegisterType<ITerminalServices, TerminalServices>();
            container.RegisterType<IEmailServices, EmailServices>();
            container.RegisterType<IEmailNotificationService, EmailNotificationService>();
            container.RegisterType<IGaneConfigurationsHelper, GaneConfigurationsHelper>();
            container.RegisterType<IProductPriceService, ProductPriceService>();
            container.RegisterType<IPalletingService, PalletingService>();
            container.RegisterType<IMarketRouteScheduleService, MarketRouteScheduleService>();
            container.RegisterType<IVehicleInspectionService, VehicleInspectionService>();
            container.RegisterType<IInvoiceService, InvoiceService>();
            container.RegisterType<IResourceHolidayServices, ResourceHolidayServices>();
            container.RegisterType<IVanSalesService, VanSalesService>();
            container.RegisterType<IAssetServices, AssetServices>();
            container.RegisterType<ITenantsCurrencyRateServices, TenantsCurrencyRateServices>();
            container.RegisterType<ITenantWebsiteService, TenantWebsiteService>();
            container.RegisterType<IUISettingServices, UISettingServices>();
            container.RegisterType<IAdyenPaymentService, AdyenPaymentService>();
            container.RegisterType<IFeedbackService, FeedbackService>();
            container.RegisterType<IShoppingVoucherService, ShoppingVoucherService>();

            container.RegisterType<IAppCache, CachingService>(new InjectionConstructor());

            //Register Auto Mapper
            var newMapper = new AutoMapperBootStrapper();
            var mapperConfig = newMapper.RegisterMappings();
            var mapper = mapperConfig.CreateMapper();
            container.RegisterType<IMapper, Mapper>(new InjectionConstructor(mapperConfig));
            container.RegisterInstance(mapper);
        }
    }
}