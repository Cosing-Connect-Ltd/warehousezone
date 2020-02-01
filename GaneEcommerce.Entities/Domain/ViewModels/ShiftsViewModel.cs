using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class ShiftsViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int? WeekNumber { get; set; }
        public int? WeekDay { get; set; }
        [DisplayName("Expected Hours")]
        public TimeSpan? ExpectedHours { get; set; }
        [DisplayName("Time Breaks")]
        public string TimeBreaks { get; set; }
        [DisplayName("Hourly Rate")]
        public int? StoresId { get; set; }
        public int StoresList { get; set; }
        public List<int?> RepeatShifts { get; set; }
        public List<int?> WeekDaysList { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? Date { get; set; }
    }
}