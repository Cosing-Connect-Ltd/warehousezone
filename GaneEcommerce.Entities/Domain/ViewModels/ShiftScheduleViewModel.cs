using System;

namespace Ganedata.Core.Models
{
    public class ShiftScheduleViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public TimeSpan? ExpectedHours { get; set; }
        public TimeSpan? TimeBreaks { get; set; }
        public int? LocationId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsFlexibleWorkingAllowed { get; set; }
        public bool IsOvertimeWorkingAllowed { get; set; }
        public int AttendanceGracePeriodInMinutes { get; set; }
    }
}