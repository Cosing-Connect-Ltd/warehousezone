using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.Models;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Models;

namespace WMS
{
    public class AutoMapperBootStrapper
    {
        public MapperConfiguration RegisterMappings()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ResourceShifts, EmployeeShiftsViewModel>().ReverseMap();
                cfg.CreateMap<Resources, ResourcesViewModel>().ReverseMap();
                cfg.CreateMap<ContactNumbers, ContactNumbersViewModel>().ReverseMap();
                cfg.CreateMap<Address, AddressViewModel>().ReverseMap();
                cfg.CreateMap<TenantLocations, LocationsViewModel>().ReverseMap();
                cfg.CreateMap<AttLogs, AttLogsViewModel>().ReverseMap();
                cfg.CreateMap<Tenant, TenantsViewModel>().ReverseMap();
                cfg.CreateMap<Shifts, ShiftsViewModel>().ReverseMap();
                cfg.CreateMap<Roles, RolesViewModel>().ReverseMap();
                cfg.CreateMap<Groups, GroupsViewModel>().ReverseMap();
                cfg.CreateMap<EmployeeRoles, EmployeeRolesViewModel>().ReverseMap();
                cfg.CreateMap<EmployeeGroups, EmployeeGroupsViewModel>().ReverseMap();
                cfg.CreateMap<AttLogsStamps, AttLogsStampsViewModel>().ReverseMap();
                cfg.CreateMap<OperLogs, OperLogsViewModel>().ReverseMap();
                cfg.CreateMap<Market, MarketViewModel>().ReverseMap();
                cfg.CreateMap<MarketVehicle, MarketVehicleViewModel>().ReverseMap();
                cfg.CreateMap<MarketVehicleViewModel, MarketVehiclesSync>().ReverseMap();
                cfg.CreateMap<MarketRoute, MarketCustomersViewModel>().ReverseMap();
                cfg.CreateMap<MarketJob, MarketJobViewModel>().ReverseMap();
                cfg.CreateMap<OrderDetail, OrderDetailsViewModel>().ReverseMap();
                cfg.CreateMap<OrderDetail, OrderDetail>().ReverseMap();
                cfg.CreateMap<Order, Order>().ReverseMap();
                cfg.CreateMap<Order, OrderViewModel>().ReverseMap();
                cfg.CreateMap<ResourceRequests, ResourceRequestsViewModel>().ReverseMap();
                cfg.CreateMap<Order, ReceivePOVM>().ReverseMap();
                cfg.CreateMap<PTenant, PTenantViewModelForTenantFlag>().ReverseMap();
                cfg.CreateMap<VehicleInspection, VehicleInspectionViewModel>().ReverseMap();
                cfg.CreateMap<MarketRoute, MarketRouteViewModel>().ReverseMap();
                cfg.CreateMap<OrderDetail, OrderDetailSessionViewModel>().ReverseMap();
                cfg.CreateMap<ProductMaster, ProductMasterViewModel>().ReverseMap();
                cfg.CreateMap<ProductAccountCodes, ProductAccountCodesViewModel>().ReverseMap();
                cfg.CreateMap<Tooltip, TooltipViewModel>().ReverseMap();

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
                cfg.CreateMap<MyJobSync, MarketJobAllocationModel>().ReverseMap();
                cfg.CreateMap<MarketRouteScheduleSync, MarketRouteSchedule>().ReverseMap();
                cfg.CreateMap<MarketRouteSync, MarketRoute>().ReverseMap();
                cfg.CreateMap<VehicleInspectionCheckList, InspectionCheckListViewModel>().ReverseMap();
                cfg.CreateMap<VehicleInspectionChecklistSync, InspectionCheckListViewModel>().ReverseMap();
                cfg.CreateMap<GoodsReturnRequestSync, GoodsReturnResponse>().ReverseMap();
                cfg.CreateMap<WastedGoodsReturnRequestSync, WastedGoodsReturnResponse>().ReverseMap();
                cfg.CreateMap<HolidayRequestSync, HolidayResponseSync>().ReverseMap();
                cfg.CreateMap<AccountTransactionFile, AccountTransactionFileSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroupDetail, ProductSpecialPriceViewModel>().ReverseMap();
                cfg.CreateMap<MarketCustomer, CustomerAccountViewModel>().ReverseMap();
                cfg.CreateMap<Account, CustomerAccountViewModel>().ReverseMap();
                cfg.CreateMap<MarketRouteProgress, MarketRouteProgressSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroups, TenantPriceGroupViewModel>().ReverseMap();
                cfg.CreateMap<VanSalesDailyCash, VanSalesDailyCashSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroups, PriceGroupViewModel>().ReverseMap();
                cfg.CreateMap<TenantEmailNotificationQueue, TenantEmailNotificationQueueViewModel>().ReverseMap();
                cfg.CreateMap<TerminalGeoLocation, TerminalGeoLocationViewModel>().ReverseMap();
                cfg.CreateMap<PalletTracking, PalletTrackingSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroups, TenantPriceGroupsSync>().ReverseMap();
                cfg.CreateMap<TenantPriceGroupDetail, TenantPriceGroupDetailSync>().ReverseMap();
                cfg.CreateMap<OrderReceiveCount, OrderReceiveCountSync>().ReverseMap();
                cfg.CreateMap<OrderReceiveCountDetail, OrderReceiveCountDetailSync>().ReverseMap();
                cfg.CreateMap<OrderProofOfDelivery, OrderProofOfDeliverySync>().ReverseMap();
                cfg.CreateMap<AssetLog, AssetLogViewModel>().ReverseMap();
                cfg.CreateMap<GlobalTax, GlobalTaxViewModel>().ReverseMap();
                cfg.CreateMap<PalletProductsSync, PalletProductAddViewModel>().ReverseMap();
                cfg.CreateMap<AssetLogViewModel, Observation>().ReverseMap();
                cfg.CreateMap<ProductKitMap, ProductKitMapViewModel>().ReverseMap();
            });

            return config;
        }
    }
}