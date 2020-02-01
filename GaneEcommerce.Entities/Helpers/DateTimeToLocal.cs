using System;

namespace Ganedata.Core.Entities.Helpers
{
    public static class DateTimeToLocal
    {
        // return time zone id to convert time with daylight saving changes
        private static TimeZoneInfo TimeZoneId(string zone)
        {
            TimeZoneInfo tz;

            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(zone);
            }
            catch (TimeZoneNotFoundException)
            {
                tz = TimeZoneInfo.Local;
            }

            return tz;

        }
        public static DateTime GmtStandardTimeNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneId("GMT Standard Time"));
        }

        public static DateTime? Convert(DateTime? utcDate, string zone = "GMT Standard Time")
        {
            if (utcDate == null)
            {
                return utcDate;
            }

            DateTime date = TimeZoneInfo.ConvertTimeFromUtc(utcDate ?? DateTime.UtcNow, TimeZoneId(zone));
            return date;

        }

        public static DateTime Convert(DateTime utcDate, string zone = "GMT Standard Time")
        {
            DateTime date = TimeZoneInfo.ConvertTimeFromUtc(utcDate, TimeZoneId(zone));
            return date;

        }
    }
}