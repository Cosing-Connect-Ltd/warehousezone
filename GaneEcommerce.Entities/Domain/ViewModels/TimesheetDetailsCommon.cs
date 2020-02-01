using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;

namespace Ganedata.Core.Models
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