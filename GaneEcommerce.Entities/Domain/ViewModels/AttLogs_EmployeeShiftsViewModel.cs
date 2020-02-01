using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class AttLogs_EmployeeShiftsViewModel
    {
        public int Id { get; set; }
        public int AttLogsId { get; set; }
        public int EmployeeShiftsId { get; set; }

        public AttLogsViewModel AttLogs { get; set; }
        public EmployeeShiftsViewModel EmployeeShifts { get; set; }
    }
}