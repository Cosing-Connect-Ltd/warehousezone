using System;
using System.Globalization;

namespace WMS.Helpers
{
    public class TimesheetDetailsCommon
    {
        public DateTime FirstDayOfWeek(DateTime date)
        {
            DayOfWeek theFirstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            int offset = theFirstDayOfWeek - date.DayOfWeek;
            DateTime firstDayOfWeekDate = date.AddDays(offset);
            return firstDayOfWeekDate;
        }

        public DateTime LastDayOfWeek(DateTime date)
        {
            DateTime lastDayOfWeekDate = FirstDayOfWeek(date).AddDays(6);
            return lastDayOfWeekDate;
        }

    }
}