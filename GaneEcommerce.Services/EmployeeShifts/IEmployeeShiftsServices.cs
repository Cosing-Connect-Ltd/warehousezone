using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IEmployeeShiftsServices
    {
        IEnumerable<ResourceShifts> GetAllEmployeeShifts(int tenantId, int locationId = 0);
        IEnumerable<ResourceShifts> SearchByEmployee(string name, int tenantId);
        IQueryable<ResourceShifts> SearchByDate(DateTime? searchDate, int tenantId, int locationId = 0);
        IEnumerable<ResourceShifts> GetByEmployeeAndWeek(int employeeId, int weekNumber);
        IEnumerable<ResourceShifts> GetByEmployeeAndWeekAndStore(int employeeId, DateTime date, int storesId);
        decimal GetAttendanceTotalHours(DateTime In, DateTime Out);
        IEnumerable<IGrouping<DateTime, ResourceShifts>> GetByGroupDate();
        int Insert(ResourceShifts employeeShift);
        void Update(ResourceShifts employeeShift);
        IEnumerable<ResourceShifts> SearchByEmployeeIdAndDateAndIFaceStatus(int employeeId, DateTime dateStamp, int iFaceStatus);
        IEnumerable<ResourceShifts> SearchByEmployeeIdAndDate(int employeeId, DateTime dateStamp,int TenantId);
        ResourceShifts SearchLastStampByDateAndEmployee(int employeeId, DateTime dateStamp);
        ResourceShifts SearchDuplicateStampByDateAndEmployee(int employeeId, DateTime dateStamp);
        IEnumerable<ResourceShifts> GetByEmployeeId(int employeeId);
        void DeleteEmployeeShiftByEmployeeId(int employeeId);
        ResourceShifts GetResourceShifts(int? shiftId);
    }
}
