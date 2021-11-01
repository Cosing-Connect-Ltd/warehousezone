using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Services;
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
            container.RegisterType<IProductServices, ProductServices>();
            container.RegisterType<IProductLookupService, ProductLookupService>();
            container.RegisterType<ITenantLocationServices, TenantLocationServices>();
            container.RegisterType<ITenantsServices, TenantsServices>();
            container.RegisterType<IStockTakeApiService, StockTakeApiService>();
            container.RegisterType<IOrderService, OrderService>();
            container.RegisterType<IPurchaseOrderService, PurchaseOrderService>();
            container.RegisterType<ITransferOrderService, TransferOrderService>();
            container.RegisterType<ICoreOrderService, CoreOrderService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IActivityServices, ActivityServices>();
            container.RegisterType<ITenantEmailConfigServices, TenantEmailConfigServices>();
            container.RegisterType<IAdminServices, AdminServices>();
            container.RegisterType<ICommonDbServices, CommonDbServices>();
            container.RegisterType<ISalesOrderService, SalesOrderService>();
            container.RegisterType<ITerminalServices, TerminalServices>();
            container.RegisterType<IEmailServices, EmailServices>();
            container.RegisterType<IGaneConfigurationsHelper, GaneConfigurationsHelper>();
            container.RegisterType<IProductPriceService, ProductPriceService>();
            container.RegisterType<IPalletingService, PalletingService>();
            container.RegisterType<IInvoiceService, InvoiceService>();
            container.RegisterType<ITenantsCurrencyRateServices, TenantsCurrencyRateServices>();
            container.RegisterType<ITooltipServices, TooltipServices>();
            container.RegisterType<IApiCredentialServices, ApiCredentialServices>();
            container.RegisterType<IAccountSectorService, AccountSectorService>();
            container.RegisterType<IPaypalPaymentServices, PaypalPaymentServices>();

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