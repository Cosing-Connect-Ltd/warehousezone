using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
     public interface IShiftsServices
    {
        IEnumerable<Shifts> GetAllEmployeeShifts(int TenantId);
        IEnumerable<Shifts> GetShiftsByEmployeeId(int employeeId);
        IEnumerable<Shifts> GetShiftsByEmployeeIdAndWeekDayAndWeekNumber(int employeeId, DateTime date, int weekNumber);
        IEnumerable<Shifts> SearchShiftsBetweenDates(int employeeId, DateTime fromDate, DateTime toDate);
        IEnumerable<Shifts> GetShiftsByEmployeeIdAndStoreIdAndStartDateAndEndDate(int employeeId, int storeId, DateTime date);
        IEnumerable<Shifts> GetShiftsByLocationIdAndBetweenDates(int locationId, DateTime toDate, DateTime fromDate);
        int Insert(Shifts shifts);
        void Update(Shifts shifts);
        Shifts GetByShiftsId(int shiftsId);
        void DeleteShiftssByEmployeeId(int employeeId);
        void Delete(Shifts shifts);
    }
}