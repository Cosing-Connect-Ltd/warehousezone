using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Services
{
    public interface IRolesServices
    {
        IEnumerable<Roles> GetAllRoles(int tenantId);
        IEnumerable<Roles> SearchRolesByName(string roleName, int tenantId);
        Roles GetByRolesId(int rolesId);
        int Insert(Roles roles, int userId);
        void Update(Roles roles, int userId);
        void Delete(Roles roles, int userId);
        int GetAllRolesByResource(int resourceId);
    }
}
