using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class EmployeesTimeLogsViewModel
    {
        public int EmployeeId { get; set; }
        public int? PayrollEmployeeNo { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string FullName { get; set; }
        public int WeekNumber { get; set; }
        public int years { get; set; }
        public string EmployeeRole { get; set; }
        public IEnumerable<TimeLogsViewModel> TimeLogs { get; set; }
    }
}