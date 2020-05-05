using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IEmployeeRolesServices
    {
        IEnumerable<EmployeeRoles> GetEmployeeRolesByEmployeeId(int employeeId);

        IEnumerable<EmployeeRoles> GetAllEmployeeRoles(int TenantId);
        void DeleteEmployeeRolesByEmployeeId(int employeeId);
        void UpdateEmployeeRoles_ByEmployeeId(int employeeId, int rolesId);
        void Insert(EmployeeRoles employeeRoles);
    }
}
