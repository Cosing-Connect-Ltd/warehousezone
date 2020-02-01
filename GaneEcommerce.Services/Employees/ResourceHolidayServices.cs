using System;
using System.Data.Entity;
using System.Linq;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;

namespace Ganedata.Core.Services
{
    public interface IResourceHolidayServices
    {
        HolidayRequestResult VerifyHoliday(int requestId, int resourceId, DateTime startTime, DateTime? endTime = null, int eventTypeId = 1);
        HolidayRequestResult VerifyHolidayList(int requestId, int resourceId, DateTime startTime, DateTime? endTime = null, int eventTypeId = 1);
    }

    public class ResourceHolidayServices : IResourceHolidayServices
    {
        private readonly IApplicationContext _currentDbContext;

        public ResourceHolidayServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public HolidayRequestResult VerifyHoliday(int requestId, int resourceId, DateTime startTime, DateTime? endTime = null,
            int eventTypeId = 1)
        {
            var upcomingHolidays = _currentDbContext.ResourceHolidays
                .Where(m => m.Id != requestId && m.EventType == eventTypeId
                && (
                ((DbFunctions.TruncateTime(m.StartDate) == DbFunctions.TruncateTime(startTime)) || (endTime.HasValue && m.EndDate.HasValue && DbFunctions.TruncateTime(m.EndDate) == DbFunctions.TruncateTime(endTime)))
                ||
                (DbFunctions.TruncateTime(m.StartDate) <= DbFunctions.TruncateTime(startTime) && DbFunctions.TruncateTime(m.EndDate) >= DbFunctions.TruncateTime(endTime))));

            var staffsOnHoliday = upcomingHolidays.Count(m => m.EventType == 0);
            var staffsOnMeetings = upcomingHolidays.Count(m => m.EventType == 1);

            if (upcomingHolidays.Any() && (staffsOnHoliday > 0 || staffsOnMeetings > 0))
            {
                return new HolidayRequestResult()
                {
                    HasWarning = true,
                    StaffsOnHoliday = staffsOnHoliday,
                    StaffsOnMeetings = staffsOnMeetings,
                    WarningMessage = staffsOnHoliday + " staffs got approved holidays for that day and " +
                                     staffsOnMeetings + " were on meetings at that time."
                };
            }
            return new HolidayRequestResult();
        }
        public HolidayRequestResult VerifyHolidayList(int requestId, int resourceId, DateTime startTime, DateTime? endTime = null,
            int eventTypeId = 1)
        {
            var upcomingHolidays = _currentDbContext.ResourceHolidays
                .Where(m => m.Id != requestId && m.EventType == eventTypeId
                && m.RequestStatus == ResourceRequestStatusEnum.Accepted && (
                ((DbFunctions.TruncateTime(m.StartDate) == DbFunctions.TruncateTime(startTime)) || (endTime.HasValue && m.EndDate.HasValue && DbFunctions.TruncateTime(m.EndDate) == DbFunctions.TruncateTime(endTime)))
                ||
                (DbFunctions.TruncateTime(m.StartDate) <= DbFunctions.TruncateTime(startTime) && DbFunctions.TruncateTime(m.EndDate) >= DbFunctions.TruncateTime(endTime))));

            var staffsOnHoliday = upcomingHolidays.Count(m => m.EventType == 0);
            var staffsOnMeetings = upcomingHolidays.Count(m => m.EventType == 1);

            if (upcomingHolidays.Any() && (staffsOnHoliday > 0 || staffsOnMeetings > 0))
            {
                return new HolidayRequestResult()
                {
                    HasWarning = true,
                    StaffsOnHoliday = staffsOnHoliday,
                    StaffsOnMeetings = staffsOnMeetings,
                    WarningMessage = staffsOnHoliday + " staffs got approved holidays for that day and " +
                                     staffsOnMeetings + " were on meetings at that time."
                };
            }
            return new HolidayRequestResult();
        }
    }
}