using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IEmployeeGroupsServices
    {
        IEnumerable<EmployeeGroups> GetEmployeeGroupsByEmployeeId(int employeeId);
        void DeleteEmployeeGroupsByEmployeeId(int employeeId);
        void UpdateEmployeeGroups_ByEmployeeId(int employeeId, int rolesId);
        void Insert(EmployeeGroups employeeRoles);
    }
}
