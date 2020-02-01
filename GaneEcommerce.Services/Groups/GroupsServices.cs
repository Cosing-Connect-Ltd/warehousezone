using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Helpers;

namespace Ganedata.Core.Services
{
    public class GroupsServices : IGroupsServices
    {
        private readonly IApplicationContext _applicationContext;

        public GroupsServices(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public IEnumerable<Groups> GetAllGroups(int tenantId)
        {
            return _applicationContext.Groups.Where(x => x.TenantId.Equals(tenantId) && x.IsDeleted != true);
        }
        public IEnumerable<Groups> SearchGroupsByName(string groupsName, int tenantId)
        {
            return _applicationContext.Groups.Where(s => s.TenantId.Equals(tenantId) && s.GroupName.ToLower().Contains(groupsName) && s.IsDeleted != true);
        }
        public Groups GetByGroupsId(int groupsId)
        {
            return _applicationContext.Groups.Find(groupsId);
        }
        public int Insert(Groups groups, int userId)
        {
            groups.DateCreated = DateTime.UtcNow;
            groups.CreatedBy = userId;
            _applicationContext.Groups.Add(groups);
            _applicationContext.SaveChanges();
            return groups.Id;
        }
        public void Update(Groups groups, int userId)
        {
            var group = _applicationContext.Groups.First(m => m.Id == groups.Id);
            group.GroupName = groups.GroupName;
            group.DateUpdated = DateTime.UtcNow;
            group.UpdatedBy = userId;
            _applicationContext.Entry(groups).State = EntityState.Modified;
            _applicationContext.SaveChanges();
        }
        public void Delete(Groups groups, int userId)
        {
            var group = _applicationContext.Groups.First(m => m.Id == groups.Id);
            group.GroupName = groups.GroupName;
            group.DateUpdated = DateTime.UtcNow;
            group.UpdatedBy = userId;
            group.IsDeleted = true;
            _applicationContext.Entry(group).State = EntityState.Modified;
            _applicationContext.SaveChanges();
        }

        public int GetResourceGroupIds(int resourceId)
        {
            
            Resources resource = _applicationContext.Resources.FirstOrDefault(a => a.IsActive && a.ResourceId == resourceId);

            int groupId = 0;

            if(resource?.EmployeeGroups != null)
            {
                groupId = resource.EmployeeGroups.Select(x => x.GroupsId).FirstOrDefault();
            }
   
            return groupId;
        }
    }
}