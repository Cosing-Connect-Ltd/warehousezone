using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class EmployeeRolesServices : IEmployeeRolesServices
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public EmployeeRolesServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }
        public IEnumerable<EmployeeRoles> GetEmployeeRolesByEmployeeId(int employeeId)
        {
            return _currentDbContext.EmployeeRoles.AsNoTracking().Where(s => s.ResourceId == employeeId && s.IsDeleted !=true);
        }

        public void DeleteEmployeeRolesByEmployeeId(int employeeId)
        {
            var matchingEmps = GetEmployeeRolesByEmployeeId(employeeId).ToList();
            foreach (var item in matchingEmps)
            {
                item.IsDeleted = true;
                _currentDbContext.Entry(item).State= EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
        }

        public void UpdateEmployeeRoles_ByEmployeeId(int employeeId, int rolesId)
        {
            //do inserts
            EmployeeRoles insert = new EmployeeRoles()
            {
                ResourceId = employeeId,
                RolesId = rolesId,
            };

            _currentDbContext.EmployeeRoles.Add(insert);
            _currentDbContext.SaveChanges();
        }

        public void Insert(EmployeeRoles employeeRoles)
        {
            _currentDbContext.EmployeeRoles.Add(employeeRoles);
            _currentDbContext.SaveChanges();
        }
    }
}