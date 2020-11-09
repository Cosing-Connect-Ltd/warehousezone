using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class TimeLogsViewModel
    {
        public int TimeLogsId { get; set; }
        public string WeekDay { get; set; }
        public int WeekNumber { get; set; }
        public string TotalHours { get; set; }
        public decimal? ExpectedHours { get; set; }
        public string ExpectedHoursString { get; set; }
        public string OvertimeWork { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public TimeSpan? Breaks { get; set; }
        public string Status { get; set; }
        public string TotalSalary { get; set; }
        public TimeSpan? BreaksTaken { get; set; }

        public ResourcesViewModel Employees { get; set; }
    }
}