using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IVanSalesService
    {
        List<MarketRouteSchedule> GetAllMarketRouteSchedules(int tenantId, DateTime? requestDate = null);
        List<MarketRouteSync> GetAllMarketRoutes(int tenantId, DateTime? requestDate = null);
        List<MarketRouteSync> GetMyMarketRoutes(int tenantId, int warehouseId, DateTime? requestDate);
        List<InspectionCheckListViewModel> GetAllValidVehicleInspectionCheckList(int tenantId, DateTime? requestDate = null, bool includeIsDeleted = false);

        VanSalesDailyCash SaveVanSalesDailyReport(VanSalesDailyCashSync vanSales);
        List<VanSalesDailyCashSync> GetAllVanSalesDailyReports(int tenantId, int warehouseId, DateTime? fromDate = null, DateTime? toDate = null);
        MarketRouteSchedule GetMarketRouteScheduleById(int id);
    }

    public class VanSalesService : IVanSalesService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;

        public VanSalesService(IApplicationContext currentDbContext, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _mapper = mapper;
        }

        public List<MarketRouteSchedule> GetAllMarketRouteSchedules(int tenantId, DateTime? requestDate = null)
        {
            return _currentDbContext.MarketRouteSchedules.Where(m => m.TenentId == tenantId && m.IsCanceled != true && (!requestDate.HasValue || m.StartTime >= requestDate)).ToList();
        }

        private List<MarketRouteSync> MapRouteSchedulesToRouteSync(IOrderedQueryable<MarketRouteSchedule> marketSchedules, DateTime? requestDate)
        {
            var results = new List<MarketRouteSync>();
            marketSchedules.ForEach(m =>
            {
                var route = _mapper.Map<MarketRoute, MarketRouteSync>(m.MarketRoute);
                route.RouteScheduleId = m.MarketRouteScheduleId;
                route.Market = m.MarketRoute.MarketRouteMap.OrderBy(s => s.SortOrder).Select(x => new MarketSync()
                {
                    MarketName = x.Market.Name,
                    MarketId = x.MarketId,
                    SortOrder = x.SortOrder,
                    Description = x.Market.Description,
                    MarketCustomers =
                     (from s in x.Market.MarketCustomers
                      join b in _currentDbContext.Account on s.AccountId equals b.AccountID
                      join mc in x.Market.MarketCustomers on b.AccountID equals mc.AccountId
                      orderby mc.SortOrder
                      where (!mc.SkipFromDate.HasValue && !mc.SkipToDate.HasValue) || (requestDate <= mc.SkipFromDate && requestDate >= mc.SkipToDate)
                      select new MarketCustomersSync()
                      {
                          ContactNumber = b.AccountContacts.Any() ? b.AccountContacts.FirstOrDefault()?.TenantContactPhone : "",
                          MarketCustomerName = b.CompanyName,
                          FullAddress = b.FullAddress,
                          SortOrder = mc.SortOrder,
                          IsSkippable = mc.IsSkippable,
                          MarketCustomerId = mc.Id,
                          MarketCustomerAccountId = b.AccountID
                      }).ToList()
                }).Where(x => x.MarketCustomers.Any()).ToList();
                results.Add(route);
            });
            return results;

        }

        public List<MarketRouteSync> GetAllMarketRoutes(int tenantId, DateTime? requestDate = null)
        {
            var marketSchedules = _currentDbContext.MarketRouteSchedules.Where(m => !requestDate.HasValue || (m.StartTime >= requestDate && requestDate <= m.EndTime)).OrderBy(s => s.StartTime);
            var results = MapRouteSchedulesToRouteSync(marketSchedules, requestDate);
            return results;
        }

        public List<MarketRouteSync> GetMyMarketRoutes(int tenantId, int warehouseId, DateTime? requestDate)
        {
            var startDate = requestDate.Value.Date;
            var endDate = requestDate.Value.Date.AddHours(24);


            var marketSchedules = _currentDbContext.MarketRouteSchedules.Where(m => ((m.StartTime >= startDate && m.StartTime <= endDate) || (m.EndTime >= startDate && m.EndTime <= endDate) ||
                                             (m.StartTime >= startDate && m.EndTime <= endDate) || (m.StartTime < startDate && m.EndTime > endDate) || (m.EventType > 0)) && m.IsCanceled != true && m.WarehouseId == warehouseId)
                                             .OrderBy(s => s.StartTime);

            var results = MapRouteSchedulesToRouteSync(marketSchedules, requestDate);
            return results;
        }
        public List<MarketRouteSync> GetMyMarketRoutesEntities(int tenantId, int warehouseId, DateTime? requestDate)
        {
            var marketSchedules = _currentDbContext.MarketRouteSchedules.Where(m => m.StartTime >= requestDate && requestDate <= m.EndTime && m.WarehouseId == warehouseId).OrderBy(s => s.StartTime);
            var results = MapRouteSchedulesToRouteSync(marketSchedules, requestDate);
            return results;
        }
        public List<InspectionCheckListViewModel> GetAllValidVehicleInspectionCheckList(int tenantId, DateTime? requestDate = null, bool includeIsDeleted = false)
        {
            var results = new List<InspectionCheckListViewModel>();

            var checkLists = _currentDbContext.VehicleInspectionCheckLists.Where(m => m.TenantId == tenantId && (includeIsDeleted || m.IsDeleted != true) && (!requestDate.HasValue || (m.DateUpdated ?? m.DateCreated) >= requestDate));
            checkLists.ForEach(m =>
            {
                results.Add(_mapper.Map(m, new InspectionCheckListViewModel()));
            });

            return results;
        }

        public VanSalesDailyCash SaveVanSalesDailyReport(VanSalesDailyCashSync vanSalesCashReport)
        {
           var model = _mapper.Map<VanSalesDailyCashSync, VanSalesDailyCash>(vanSalesCashReport);
            model.UpdateCreatedInfo(vanSalesCashReport.UserId);
            _currentDbContext.VanSalesDailyCashes.Add(model);
            _currentDbContext.SaveChanges();
            model.SalesManUserId = vanSalesCashReport.UserId;
            foreach (var id in vanSalesCashReport.OrderIds)
            {
                var order = _currentDbContext.Order.Where(x => x.OrderToken == id).FirstOrDefault();
                if (order != null)
                {
                    order.DateUpdated = DateTime.UtcNow;
                    order.VanSalesDailyCashId = model.VanSalesDailyCashId;
                    order.EndOfDayGenerated = true;
                }
            }

            _currentDbContext.SaveChanges();
            return model;
        }

        public List<VanSalesDailyCashSync> GetAllVanSalesDailyReports(int tenantId, int warehouseId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var results =
                from c in _currentDbContext.VanSalesDailyCashes.Where(m => m.TenantId == tenantId && m.MobileLocationId == warehouseId && (!fromDate.HasValue || m.SaleDate >= fromDate) && (!toDate.HasValue || m.SaleDate <= toDate))
                join d in _currentDbContext.TenantWarehouses on c.MobileLocationId equals d.WarehouseId
                join u in _currentDbContext.AuthUsers on c.SalesManUserId equals u.UserId
                select new VanSalesDailyCashSync()
                {
                    ChequesCount = c.ChequesCount,
                    DateCreated = c.DateCreated,
                    SalesManUserId = c.SalesManUserId,
                    UserId = c.CreatedBy ?? 0,
                    Fifty = c.Fifty,
                    Five = c.Five,
                    FiveHundred = c.FiveHundred,
                    IsDeleted = c.IsDeleted,
                    MobileLocationId = c.MobileLocationId,
                    Notes = c.Notes,
                    One = c.One,
                    OneHundred = c.OneHundred,
                    PointFifty = c.PointFifty,
                    PointFive = c.PointFive,
                    PointOne = c.PointOne,
                    PointTen = c.PointTen,
                    PointTwenty = c.PointTwenty,
                    PointTwentyFive = c.PointTwentyFive,
                    PointTwo = c.PointTwo,
                    SaleDate = c.SaleDate,
                    SalesManName = c.SalesManUser.UserFirstName + " " + c.SalesManUser.UserLastName,
                    SubmittedDate = c.SubmittedDate,
                    Ten = c.Ten,
                    TenantId = c.TenantId,
                    TerminalId = c.TerminalId,
                    TotalCashSubmitted = c.TotalCashSubmitted,
                    TotalPaidCards = c.TotalPaidCards,
                    TotalPaidCash = c.TotalPaidCash,
                    TotalPaidCheques = c.TotalPaidCheques,
                    TotalSale = c.TotalSale,
                    TotalNetSale = c.TotalNetSale,
                    TotalNetTax = c.TotalNetTax,
                    Twenty = c.Twenty,
                    Two = c.Two,
                    TwoHundred = c.TwoHundred,
                    VanSalesDailyCashId = c.VanSalesDailyCashId,
                    VehicleName = c.TenantLocations.WarehouseName
                };

            return results.ToList();
        }

        public MarketRouteSchedule GetMarketRouteScheduleById(int id)
        {
            var marketSchedules = _currentDbContext.MarketRouteSchedules.Find(id);
            return marketSchedules;
        }
    }
}