using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.Models;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Models;

namespace ShopOrderApp
{
    public class AutoMapperBootStrapper
    {
        public MapperConfiguration RegisterMappings()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TenantLocations, LocationsViewModel>().ReverseMap();
                cfg.CreateMap<Tenant, TenantsViewModel>().ReverseMap();
                
                cfg.CreateMap<OrderDetail, OrderDetailsViewModel>().ReverseMap();
                cfg.CreateMap<OrderDetail, OrderDetail>().ReverseMap();
                cfg.CreateMap<Order, Order>().ReverseMap();
                cfg.CreateMap<Order, OrderViewModel>().ReverseMap();
              
                cfg.CreateMap<Order, ReceivePOVM>().ReverseMap();
                cfg.CreateMap<OrderDetail, OrderDetailSessionViewModel>().ReverseMap();
                cfg.CreateMap<ProductMaster, ProductMasterViewModel>().ReverseMap();
                cfg.CreateMap<ProductAccountCodes, ProductAccountCodesViewModel>().ReverseMap();
                cfg.CreateMap<ApiCredentials, ApiCredentialsViewModel>();
                cfg.CreateMap<Tooltip, TooltipViewModel>()
                .ForMember(s => s.TenantName, c => c.MapFrom(m => m.Tenant.TenantName)).ReverseMap();

                //APIs
                cfg.CreateMap<ProductMaster, ProductMasterSync>().ReverseMap();
                cfg.CreateMap<Account, AccountSync>().ReverseMap();
                cfg.CreateMap<InventoryStock, InventoryStockSync>().ReverseMap();
                cfg.CreateMap<StockTake, StockTakeSync>().ReverseMap();
                cfg.CreateMap<ProductSerialis, ProductSerialSync>().ReverseMap();
                cfg.CreateMap<OrderDetail, OrderDetailSync>().ReverseMap();
                cfg.CreateMap<Order, OrdersSync>().ForMember(s => s.OrderDetails, c => c.MapFrom(m => m.OrderDetails)).ReverseMap();
                cfg.CreateMap<PalletViewModel, PalletSync>().ReverseMap();
                cfg.CreateMap<PalletDispatchSync, PalletsDispatch>().ReverseMap();
                cfg.CreateMap<OrderProcessDetail, OrderProcessDetailSync>().ReverseMap();
                cfg.CreateMap<OrderProcess, OrderProcessesSync>()
                .ForMember(s => s.OrderProcessDetails, c => c.MapFrom(m => m.OrderProcessDetail))
                .ForMember(s => s.AccountID, c => c.MapFrom(m => m.Order.AccountID)).ReverseMap();
                cfg.CreateMap<InvoiceMaster, InvoiceViewModel>().ReverseMap();
                cfg.CreateMap<InvoiceDetail, InvoiceDetailViewModel>().ReverseMap();
                cfg.CreateMap<AccountTransaction, AccountTransactionViewModel>().ReverseMap();
                cfg.CreateMap<GoodsReturnRequestSync, GoodsReturnResponse>().ReverseMap();
                cfg.CreateMap<WastedGoodsReturnRequestSync, WastedGoodsReturnResponse>().ReverseMap();
                cfg.CreateMap<HolidayRequestSync, HolidayResponseSync>().ReverseMap();
                cfg.CreateMap<AccountTransactionFile, AccountTransactionFileSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroupDetail, ProductSpecialPriceViewModel>().ReverseMap();
                cfg.CreateMap<VanSalesDailyCash, VanSalesDailyCashSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroups, PriceGroupViewModel>().ReverseMap();

                cfg.CreateMap<TenantPriceGroups, TenantPriceGroupViewModel>().ReverseMap();
                cfg.CreateMap<TenantEmailNotificationQueue, TenantEmailNotificationQueueViewModel>().ReverseMap();
                cfg.CreateMap<TerminalGeoLocation, TerminalGeoLocationViewModel>().ReverseMap();
                cfg.CreateMap<PalletTracking, PalletTrackingSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroups, TenantPriceGroupsSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroupDetail, TenantPriceGroupDetailSync>().ReverseMap();
                cfg.CreateMap<OrderReceiveCount, OrderReceiveCountSync>().ReverseMap();
                cfg.CreateMap<OrderReceiveCountDetail, OrderReceiveCountDetailSync>().ReverseMap();
                cfg.CreateMap<OrderProofOfDelivery, OrderProofOfDeliverySync>().ReverseMap();
            
                cfg.CreateMap<GlobalTax, GlobalTaxViewModel>().ReverseMap();
                cfg.CreateMap<ProductKitMap, ProductKitMapViewModel>().ReverseMap();
                cfg.CreateMap<PalletProductsSync, PalletProductAddViewModel>().ReverseMap();
                cfg.CreateMap<TenantLocations, TenantLocationsSync>().ReverseMap();
                cfg.CreateMap<AuthUser, UserLoginStatusResponseViewModel>().ReverseMap();
                cfg.CreateMap<AccountAddresses, AccountAddressSync>().ReverseMap();
                cfg.CreateMap<AccountAddresses, AddressViewModel>().ReverseMap();
                cfg.CreateMap<GlobalCountry, CountryViewModel>().ReverseMap();
                cfg.CreateMap<TenantLocations, CollectionPointViewModel>().ReverseMap();
                cfg.CreateMap<ProductLocationStocks, ProductLocationStocksSync>().ReverseMap();
            });

            return config;
        }
    }
}