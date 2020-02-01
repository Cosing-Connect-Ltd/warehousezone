using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class ShiftsServices : IShiftsServices
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public ShiftsServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public IEnumerable<Shifts> GetAllEmployeeShifts(int TenantId)
        {
            return _currentDbContext.Shifts.Where(u=>u.IsDeleted != true && u.TenantId== TenantId);
        }

        public IEnumerable<Shifts> GetShiftsByEmployeeId(int employeeId)
        { 
            return _currentDbContext.Shifts.Where(s => s.EmployeeId == employeeId && s.IsDeleted !=true);
        }

        public Shifts GetByShiftsId(int shiftsId)
        {
            return _currentDbContext.Shifts.Find(shiftsId);
        }

        public IEnumerable<Shifts> GetShiftsByEmployeeIdAndWeekDayAndWeekNumber(int employeeId, DateTime date, int weekNumber)
        {
            return _currentDbContext.Shifts.Where(s => s.EmployeeId == employeeId && DbFunctions.TruncateTime(s.Date.Value) == DbFunctions.TruncateTime(date) && s.WeekNumber == weekNumber && s.IsDeleted != true);
        }

        public IEnumerable<Shifts> SearchShiftsBetweenDates(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return _currentDbContext.Shifts.Where(s => s.EmployeeId == employeeId && (s.Date >= fromDate && s.Date <= toDate));
        }

        public IEnumerable<Shifts> GetShiftsByEmployeeIdAndStoreIdAndStartDateAndEndDate(int employeeId, int storeId, DateTime date)
        {
            return _currentDbContext.Shifts.Where(s => s.EmployeeId == employeeId && s.LocationsId == storeId && s.Date == date);
        }

        public IEnumerable<Shifts> GetShiftsByLocationIdAndBetweenDates(int locationId, DateTime fromDate, DateTime toDate)
        {
            return _currentDbContext.Shifts.Where(s => s.LocationsId == locationId && (s.Date >= fromDate && s.Date <= toDate) && s.IsDeleted != true);
        }

        public int Insert(Shifts shifts)
        {
            _currentDbContext.Entry(shifts).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return shifts.Id;
        }

        public void Update(Shifts shifts)
        {
            _currentDbContext.Shifts.Attach(shifts);
            _currentDbContext.Entry(shifts).State = System.Data.Entity.EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public void DeleteShiftssByEmployeeId(int employeeId)
        {
            foreach (var item in GetShiftsByEmployeeId(employeeId))
            {
                _currentDbContext.Shifts.Remove(item);
                _currentDbContext.SaveChanges();
            }
        }

        public void Delete(Shifts shifts)
        {
            _currentDbContext.Shifts.Attach(shifts);
            _currentDbContext.Shifts.Remove(shifts);
            _currentDbContext.SaveChanges();
        }
    }
}