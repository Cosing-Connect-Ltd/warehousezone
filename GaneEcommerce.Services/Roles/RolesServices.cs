using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Helpers;

namespace Ganedata.Core.Services
{
    public class RolesServices : IRolesServices
    {
        private readonly IApplicationContext _applicationContext;

        public RolesServices(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public IEnumerable<Roles> GetAllRoles(int tenantId)
        {
            return _applicationContext.Roles.Where(x => x.TenantId.Equals(tenantId) && x.IsDeleted!=true);
        }

        public int GetAllRolesByResource(int resourceId)
        {
            int roleId = _applicationContext.EmployeeRoles.Where(x => x.ResourceId == resourceId && x.IsDeleted != true).Select(a => a.RolesId).FirstOrDefault();
            return roleId;
        }

        public IEnumerable<Roles> SearchRolesByName(string rolesName, int tenantId)
        {
            return _applicationContext.Roles.Where(s => s.TenantId.Equals(tenantId) && s.IsDeleted != true && s.RoleName.ToLower().Contains(rolesName));
        }
        public Roles GetByRolesId(int rolesId)
        {
            return _applicationContext.Roles.Find(rolesId);
        }

        public int Insert(Roles roles, int userId)
        {
            roles.DateCreated = DateTime.UtcNow;
            roles.CreatedBy = userId;
            _applicationContext.Roles.Add(roles);
            _applicationContext.SaveChanges();
            return roles.Id;
        }
        public void Update(Roles roles, int userId)
        {
            var role = _applicationContext.Roles.First(m=> m.Id ==roles.Id);
            role.RoleName = roles.RoleName;
            role.DateUpdated = DateTime.UtcNow;
            role.UpdatedBy = userId;
            _applicationContext.Entry(role).State = EntityState.Modified;
            _applicationContext.SaveChanges();
        }
        public void Delete(Roles roles, int userId)
        {
            var role = _applicationContext.Roles.First(m => m.Id == roles.Id);
            role.DateUpdated = DateTime.UtcNow;
            role.UpdatedBy = userId;
            role.IsDeleted = true;
            _applicationContext.Entry(role).State = EntityState.Modified;
            _applicationContext.SaveChanges();
        }
    }
}