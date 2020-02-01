using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class EmployeeShiftsStoresServices : IEmployeeShiftsStoresServices
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public EmployeeShiftsStoresServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public IEnumerable<EmployeeShifts_Stores> GetEmployeeShifts_StoresByEmployeeId(int employeeId)
        {
            return _currentDbContext.EmployeeShifts_Stores.Where(s => s.ResourceId == employeeId );
        }

        public void DeleteEmployeeShiftsStoresByEmployeeId(int employeeId)
        {
            var emps = GetEmployeeShifts_StoresByEmployeeId(employeeId).ToList();
            foreach (var item in emps)
            {
                _currentDbContext.EmployeeShifts_Stores.Remove(item);
                _currentDbContext.SaveChanges();
            }
        }

        public void UpdateEmployeeShifts_StoresByEmployeeId(int employeeId, int storeId)
        {
            //do inserts
            var insert = new EmployeeShifts_Stores()
            {
                ResourceId = employeeId,
                WarehouseId = storeId,
            };

            _currentDbContext.EmployeeShifts_Stores.Add(insert);
            _currentDbContext.SaveChanges();
        }

        public void Insert(EmployeeShifts_Stores employeeShiftsStores)
        {
            _currentDbContext.EmployeeShifts_Stores.Add(employeeShiftsStores);
            _currentDbContext.SaveChanges();
        }
    }
}