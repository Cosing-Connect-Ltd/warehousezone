using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class EmployeeShiftsServices : IEmployeeShiftsServices
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public EmployeeShiftsServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        /// <summary>
        /// Get All Employees Shifts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ResourceShifts> GetAllEmployeeShifts(int tenantId, int locationId = 0)
        {
            return _currentDbContext.ResourceShifts.Where(x => x.TenantId == tenantId && x.Terminals.WarehouseId == locationId && x.IsDeleted != true);
        }

        public ResourceShifts GetResourceShifts(int? shiftId)
        {
            return _currentDbContext.ResourceShifts.FirstOrDefault(u => u.Id == shiftId && u.IsDeleted !=true);
        }

        /// <summary>
        /// Search by Date
        /// </summary>
        /// <param name="searchDate"></param>
        /// <returns></returns>
        public IQueryable<ResourceShifts> SearchByDate(DateTime? searchDate, int tenantId, int locationId = 0)
        {
            return _currentDbContext.ResourceShifts.Where(s => (searchDate == null || DbFunctions.TruncateTime(s.Date) == DbFunctions.TruncateTime(searchDate))
            && s.TenantId == tenantId && (s.Terminals.WarehouseId == locationId || locationId == 0) && s.IsDeleted != true)
            .Include(x => x.Resources.EmployeeShifts_Stores)
            .Include(x => x.Resources)
            .Include(x => x.Resources.EmployeeRoles)
            .Include(x => x.Resources.EmployeeRoles.Select(y => y.Roles))
            .Include(x => x.Resources.ContactNumbers)
            .Include(x => x.Resources.Address)
            .Include(x => x.Resources.JobTypes);
        }

        /// <summary>
        /// Search by Employee
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<ResourceShifts> SearchByEmployee(string name, int tenantId)
        {
            return _currentDbContext.ResourceShifts.Where(s => s.EmployeeShiftID.Contains(name) && s.TenantId == tenantId && s.IsDeleted != true);
        }

        public IEnumerable<ResourceShifts> GetByEmployeeId(int employeeId)
        {
            return _currentDbContext.ResourceShifts.Where(s => s.ResourceId == employeeId && s.IsDeleted != true);
        }

        /// <summary>
        /// Get by group    
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGrouping<DateTime, ResourceShifts>> GetByGroupDate()
        {

            var result = _currentDbContext.ResourceShifts.GroupBy(c => new { Date = c.Date.Date, Employee = c.Resources })
                .Select(g => new
                {
                    Date = g.Key.Date,
                    Employee = g.Key.Employee
                });

            return (IEnumerable<IGrouping<DateTime, ResourceShifts>>)result;
        }

        /// <summary>
        /// Get Timesheet By Employee and WeekNumber
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="weekNumber"></param>
        /// <returns>Return lists of Shifts by week</returns>
        public IEnumerable<ResourceShifts> GetByEmployeeAndWeek(int employeeId, int weekNumber)
        {
            return _currentDbContext.ResourceShifts.Where(s => s.ResourceId == employeeId && s.WeekNumber == weekNumber);
        }

        public IEnumerable<ResourceShifts> GetByEmployeeAndWeekAndStore(int employeeId, DateTime date, int storesId)
        {
            // 24 hours of that day and 16 hours of next day = 40
            // add 40 hours to check if out stamp can be after mid night
            DateTime endTime = date.AddHours(40);
            return _currentDbContext.ResourceShifts.Where(s => s.ResourceId == employeeId && s.TimeStamp >= date && s.TimeStamp < endTime && s.Terminals.WarehouseId == storesId
            && s.ResourceId == employeeId && s.IsDeleted != true);
        }

        /// <summary>
        /// Get the Total Hours between In and Out
        /// </summary>
        /// <param name="inDate"></param>
        /// <param name="outDate"></param>
        /// <returns>Sum of both In and Out DateTime</returns>
        public decimal GetAttendanceTotalHours(DateTime inDate, DateTime outDate)
        {
            decimal totalHours = 0;
            TimeSpan span = outDate - inDate;

            totalHours += (decimal)span.TotalHours;

            return totalHours;
        }

        public int Insert(ResourceShifts employeeShift)
        {
            _currentDbContext.Entry(employeeShift).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return employeeShift.Id;
        }

        public void Update(ResourceShifts employeeShift)
        {
            _currentDbContext.ResourceShifts.Attach(employeeShift);
            _currentDbContext.Entry(employeeShift).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public IEnumerable<ResourceShifts> SearchByEmployeeIdAndDateAndIFaceStatus(int employeeId, DateTime dateStamp, int iFaceStatus)
        {
            int status = 0;

            //replace the iFace status code to our own ShiftStatus code
            switch (iFaceStatus) //iFace API: 1 = Clock Out; 2 = Out; 5 = Clock out for Ever time; 6 = Meal End
            {
                case 1:
                    status = 1; //1= Shift
                    break;
                case 2:
                    status = 8; //8= In/OUT
                    break;
                case 5:
                    status = 2; //2= Break
                    break;
                case 7:
                    status = 3; //3= Lunch
                    break;
            }

            return _currentDbContext.ResourceShifts.Where(s => s.ResourceId == employeeId && s.Date == dateStamp && s.ShiftStatusId == status);
        }

        public IEnumerable<ResourceShifts> SearchByEmployeeIdAndDate(int employeeId, DateTime dateStamp, int TenantId)
        {
            return _currentDbContext.ResourceShifts.Where(s => s.ResourceId == employeeId && s.Date.Day == dateStamp.Day && s.IsDeleted != true && s.TenantId==TenantId);
        }

        public ResourceShifts SearchLastStampByDateAndEmployee(int employeeId, DateTime dateStamp)
        {
            var minDateSpan = dateStamp.AddHours(-16);
            var lastStamp = _currentDbContext.ResourceShifts.Where(x => x.TimeStamp >= minDateSpan && x.TimeStamp < dateStamp && x.ResourceId == employeeId && x.IsDeleted != true).OrderByDescending(o => o.TimeStamp).FirstOrDefault();
            return lastStamp;
        }

        public ResourceShifts SearchDuplicateStampByDateAndEmployee(int employeeId, DateTime dateStamp)
        {
            return _currentDbContext.ResourceShifts.FirstOrDefault(x => x.TimeStamp == dateStamp && x.ResourceId == employeeId && x.IsDeleted != true);

        }

        public void DeleteEmployeeShiftByEmployeeId(int employeeId)
        {
            foreach (var item in GetByEmployeeId(employeeId))
            {
                _currentDbContext.ResourceShifts.Remove(item);
                _currentDbContext.SaveChanges();
            }
        }
    }
}