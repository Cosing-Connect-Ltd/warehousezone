using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Services
{
    public interface IMarketRouteScheduleService
    {
        IEnumerable<MarketRouteSchedule> GetAllMarketRouteSchedules(int tenantId, DateTime? filterByDate = null, bool includeCancelled = false);

        MarketRouteSchedule GetMarketRouteScheduleById(int marketRouteScheduleId);

        MarketRouteSchedule CreateMarketRouteSchedule(string start, string end, int routeId, int mobileLocationId, int tenantId);
    }

    public class MarketRouteScheduleService : IMarketRouteScheduleService
    {
        private readonly IApplicationContext _currentDbContext;
        public MarketRouteScheduleService(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public IEnumerable<MarketRouteSchedule> GetAllMarketRouteSchedules(int tenantId, DateTime? filterByDate = null, bool includeCancelled = false)
        {
            return _currentDbContext.MarketRouteSchedules.Where(m => (includeCancelled || !m.IsCanceled) && (filterByDate == null || DbFunctions.TruncateTime((DateTime?)m.StartTime) == DbFunctions.TruncateTime(filterByDate)))
                .OrderBy(m => m.StartTime).ThenBy(m => m.MarketRouteScheduleId);
        }

        public MarketRouteSchedule GetMarketRouteScheduleById(int marketRouteScheduleId)
        {
            return _currentDbContext.MarketRouteSchedules.Find(marketRouteScheduleId);
        }

        public MarketRouteSchedule CreateMarketRouteSchedule(string start, string end, int routeId, int mobileLocationId, int tenantId)
        {
            var route = _currentDbContext.MarketRoutes.Find(routeId);
            var resIds = $"<ResourceIds>\r\n<ResourceId Type = \"System.Int32\" Value = \"{mobileLocationId}\" />\r\n</ResourceIds>";
            var newAppt = new MarketRouteSchedule()
            {
                StartTime = ParseDate(start),
                EndTime = ParseDate(end),
                Subject = route.Name + " : " + route.Description,
                RouteId = route.Id,
                VehicleId = Convert.ToInt32(mobileLocationId),
                WarehouseIDs = resIds,
                TenentId = tenantId,
                WarehouseId = mobileLocationId
            };
            _currentDbContext.MarketRouteSchedules.Add(newAppt);
            _currentDbContext.SaveChanges();
            return newAppt;
        }

        private DateTime ParseDate(string utcDateString)
        {
            DateTime utcDate = new DateTime(1970, 1, 1);
            utcDate = utcDate.AddMilliseconds(System.Convert.ToDouble(utcDateString));
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDate, DateTimeKind.Utc), TimeZoneInfo.Local);
        }
    }
}