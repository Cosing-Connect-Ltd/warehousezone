using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IEmployeeShiftsStoresServices
    {
        IEnumerable<EmployeeShifts_Stores> GetEmployeeShifts_StoresByEmployeeId(int employeeId);
        void UpdateEmployeeShifts_StoresByEmployeeId(int employeeId, int storeId);
        void Insert(EmployeeShifts_Stores employeeShiftsStores);
        void DeleteEmployeeShiftsStoresByEmployeeId(int employeeId);
    }
}
