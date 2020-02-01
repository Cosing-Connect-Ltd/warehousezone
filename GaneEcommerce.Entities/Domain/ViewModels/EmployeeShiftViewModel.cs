using Ganedata.Core.Entities.Domain;
using System;
using Ganedata.Core.Entities.Helpers;
using System.Linq;

namespace Ganedata.Core.Models
{
    public class EmployeeShiftsViewModel
    {
        public int Id { get; set; } 
        public string EmployeeShiftID { get; set; }
        public DateTime Date { get; set; }
        public int WeekNumber { get; set; }
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
        public int? ShiftStatusId { get; set; }
        public Enum StatusTypeValue { get; set; }
        public string StatusType { get; set; }
        public int? ResourceId { get; set; }
        public int TerminalId { get; set; }
        public DateTime? TimeStamp { get; set; }

        public string ToShortDate
        {
            get
            {
                return Date.ToShortDateString();
            }
        }

        public decimal? TotalHours { get; set; }
        public ResourcesViewModel Resources { get; set; }
        public ShiftStatusViewModel ShiftStatus { get; set; }
        public virtual Terminals Terminals { get; set; }
        //Devex custom properties
        public string FullName => Resources == null ? "Unknown Client" : (Resources.SurName + ", " + Resources.FirstName);
        public string EmployeeRole => Resources.EmployeeRoles.Count() <= 0 ? "" : Resources.EmployeeRoles.Where(x => x.IsDeleted != true).FirstOrDefault().Roles.RoleName;
        public string ShiftDateString => Date.AsDateString();
        public string TimestampString => TimeStamp.AsTimeString();
    }
}