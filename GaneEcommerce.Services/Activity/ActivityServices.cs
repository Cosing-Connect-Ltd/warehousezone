using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class ActivityServices : IActivityServices
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly ITenantsServices _tenantsServices;

        public ActivityServices(IApplicationContext currentDbContext, ITenantsServices tenantsServices)
        {
            _currentDbContext = currentDbContext;
            _tenantsServices = tenantsServices;
        }

        public IEnumerable<AuthActivity> GetAllActivities(int TenantId)
        {
            return _currentDbContext.AuthActivities.Where(e => e.IsDeleted != true && e.TenantId==TenantId).ToList();
        }

        public AuthActivity GetActivityById(int activityId)
        {
            return _currentDbContext.AuthActivities.Find(activityId);
        }

        public int SaveActivity(AuthActivity activity, int userId, int tenantId)
        {

            activity.DateCreated = DateTime.UtcNow;
            activity.DateUpdated = DateTime.UtcNow;
            activity.CreatedBy = userId;
            activity.UpdatedBy = userId;
            activity.TenantId = tenantId;
            _currentDbContext.Entry(activity).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            int res = activity.ActivityId;
            return res;
        }

        public bool UpdateActivity(AuthActivity activity, int userId, int tenantId)
        {
            var act = _currentDbContext.AuthActivities.FirstOrDefault(m => m.ActivityId == activity.ActivityId);
            if (act != null)
            {
                act.ActivityName = activity.ActivityName;
                act.ActivityAction = activity.ActivityAction;
                act.ActivityController = activity.ActivityController;
                act.IsActive = activity.IsActive;
                act.RightNav = activity.RightNav;
                act.DateUpdated = DateTime.UtcNow;
                act.UpdatedBy = userId;
                _currentDbContext.Entry(act).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return true;
            }

            else
            {
                return false;
            }
        }

        public void DeleteActivity(AuthActivity activity, int userId)
        {
            activity.IsDeleted = true;
            activity.UpdatedBy = userId;
            activity.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(activity).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public IQueryable<AuthActivitiesForPermViewModel> GetAuthActivitiesForPermByGroup(AuthUserGroupsForPermViewModel activityGroup, IEnumerable<int> userModules, int TenantId)
        {
            return (from e in _currentDbContext.AuthActivities
                    join t in _currentDbContext.AuthActivityGroupMaps on e.ActivityId equals t.ActivityId
                    join x in _currentDbContext.AuthActivityGroups on t.ActivityGroupId equals x.ActivityGroupId
                    where t.IsDeleted != true && e.SuperAdmin != true && x.ActivityGroupId == activityGroup.ActivityGroupId && e.IsActive == true
                    && e.IsDeleted != true && e.ExcludePermission != true && (!e.ModuleId.HasValue || userModules.Contains(e.ModuleId.Value)) 
                    && e.TenantId==TenantId

                    select new AuthActivitiesForPermViewModel
                    {
                        ActivityId = e.ActivityId,
                        Controller = e.ActivityController,
                        Action = e.ActivityAction,
                        ActivityName = e.ActivityName,
                        SortOrder = e.SortOrder
                    }).OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityName);
        }

        public IQueryable<AuthActivitiesForPermViewModel> GetAuthActivitiesForPermNoGroup(IEnumerable<int> userModules)
        {
            return (from e in _currentDbContext.AuthActivities
                    where _currentDbContext.AuthActivityGroupMaps.All(x => x.ActivityId != e.ActivityId) && e.IsActive == true
                    && e.IsDeleted != true && e.ExcludePermission != true && e.SuperAdmin != true && (!e.ModuleId.HasValue || userModules.Contains(e.ModuleId.Value))
                    select new AuthActivitiesForPermViewModel
                    {
                        ActivityId = e.ActivityId,
                        Controller = e.ActivityController,
                        Action = e.ActivityAction,
                        ActivityName = e.ActivityName,
                        SortOrder = e.SortOrder
                    }).OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityName);
        }

        public List<AuthActivity> GetExcludedActivities()
        {
            return _currentDbContext.AuthActivities.Where(e => e.ExcludePermission == true).ToList();
        }

        // ******************** Activity Groups *********************
        public IEnumerable<AuthActivityGroup> GetAllActivityGroups(int CurrentTenantId)
        {
            return _currentDbContext.AuthActivityGroups.Where(e => e.IsDeleted != true && e.TenantId==CurrentTenantId).ToList();
        }

        public AuthActivityGroup GetActivityGroupById(int activityGroupId)
        {
            return _currentDbContext.AuthActivityGroups.Find(activityGroupId);
        }

        public int SaveActivityGroup(AuthActivityGroup activityGroup, int userId, int tenantId)
        {

            activityGroup.DateCreated = DateTime.UtcNow;
            activityGroup.DateUpdated = DateTime.UtcNow;
            activityGroup.CreatedBy = userId;
            activityGroup.UpdatedBy = userId;
            activityGroup.TenantId = tenantId;

            _currentDbContext.Entry(activityGroup).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            int res = activityGroup.ActivityGroupId;
            return res;
        }

        public void UpdateActivityGroup(AuthActivityGroup activityGroup, int userId, int tenantId)
        {

            activityGroup.DateUpdated = DateTime.UtcNow;
            activityGroup.UpdatedBy = userId;
            _currentDbContext.Entry(activityGroup).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

        }

        public void DeleteActivityGroup(AuthActivityGroup activityGroup, int userId)
        {
            activityGroup.IsDeleted = true;
            activityGroup.UpdatedBy = userId;
            activityGroup.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(activityGroup).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public List<AuthPermissionsViewModel> GetAllNavigationPermissionsInGroup(int tenantId)
        {
            var tenantModules = _tenantsServices.GetAllTenantModules(0).Select(m => m.ModuleId);

            return (from e in _currentDbContext.AuthActivities
                    join t in _currentDbContext.AuthActivityGroupMaps on e.ActivityId equals t.ActivityId
                    join x in _currentDbContext.AuthActivityGroups on t.ActivityGroupId equals x.ActivityGroupId
                    where t.IsDeleted != true && e.IsActive == true && e.IsDeleted != true && e.RightNav == true && (!e.ModuleId.HasValue || tenantModules.Contains(e.ModuleId.Value))
                    select new AuthPermissionsViewModel
                    {
                        Controller = e.ActivityController,
                        Action = e.ActivityAction,
                        ActivityName = e.ActivityName,
                        SortOrder = e.SortOrder,
                        ActivityGroupId = x.ActivityGroupId
                    }).OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityName).ToList();
        }

        public List<WarehousePermissionViewModel> GetAllPermittedWarehousesForUser(int userId, int tenantId, bool isSuperAdmin = false, bool includeMobileLocations = false)
        {
            var allWarehouses = _currentDbContext.TenantWarehouses.Where(x => x.TenantId == tenantId && (includeMobileLocations || x.IsMobile != true)).ToList();

            if (!isSuperAdmin)
            {
                var userWarehouses = _currentDbContext.AuthPermissions.Where(u => u.UserId == userId)
                    .Select(m => m.TenantLocation.WarehouseId);
                allWarehouses.RemoveAll(m => !userWarehouses.Contains(m.WarehouseId));
            }

            return (from e in allWarehouses
                    select new WarehousePermissionViewModel
                    {
                        WId = e.WarehouseId,
                        WName = e.WarehouseName
                    }).ToList();
        }


        public IQueryable<AuthUserGroupsForPermViewModel> GetDistinctActivityGroupsForPerm(IEnumerable<int> userModules)
        {
            return (from e in _currentDbContext.AuthActivityGroups
                    join t in _currentDbContext.AuthActivityGroupMaps on e.ActivityGroupId equals t.ActivityGroupId
                    join q in _currentDbContext.AuthActivities on t.ActivityId equals q.ActivityId
                    where e.IsActive == true && e.IsDeleted != true && q.SuperAdmin != true && q.IsActive == true && q.IsDeleted != true && q.ExcludePermission != true
                    && (!q.ModuleId.HasValue || userModules.Contains(q.ModuleId.Value))
                    && t.IsDeleted != true && q.IsActive == true && q.IsDeleted != true
                    select new AuthUserGroupsForPermViewModel
                    {
                        ActivityGroupId = e.ActivityGroupId,
                        ActivityGroupName = e.ActivityGroupName,
                        ActivityGroupDetail = e.ActivityGroupDetail,
                        ActivityGroupParentId = e.ActivityGroupParentId,
                        DateCreated = e.DateCreated,
                        DateUpdated = e.DateUpdated,
                        CreatedBy = e.CreatedBy,
                        UpdatedBy = e.UpdatedBy,
                        IsActive = e.IsActive,
                        IsDeleted = e.IsDeleted,
                        TenantId = e.TenantId,
                        SortOrder = e.SortOrder
                    }).Distinct().OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityGroupName);
        }

        public IQueryable<AuthUserGroupsForPermViewModel> GetDistinctActivityGroupsForNavigation(int userId, int warehouseId, IEnumerable<int> userModules)
        {
            return (from ep in _currentDbContext.AuthPermissions
                    join e in _currentDbContext.AuthActivities on ep.ActivityId equals e.ActivityId
                    join t in _currentDbContext.AuthActivityGroupMaps on e.ActivityId equals t.ActivityId
                    join x in _currentDbContext.AuthActivityGroups on t.ActivityGroupId equals x.ActivityGroupId
                    where ep.UserId == userId && ep.WarehouseId == warehouseId && (!e.ModuleId.HasValue || userModules.Contains(e.ModuleId.Value))
                    && ep.IsActive == true && ep.IsDeleted != true && e.RightNav == true && e.IsActive == true
                    && e.IsDeleted != true && t.IsDeleted != true && x.IsActive == true && x.IsDeleted != true
                    select new AuthUserGroupsForPermViewModel
                    {
                        ActivityGroupId = x.ActivityGroupId,
                        ActivityGroupName = x.ActivityGroupName,
                        ActivityGroupDetail = x.ActivityGroupDetail,
                        ActivityGroupParentId = x.ActivityGroupParentId,
                        DateCreated = x.DateCreated,
                        DateUpdated = x.DateUpdated,
                        CreatedBy = x.CreatedBy,
                        UpdatedBy = x.UpdatedBy,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        TenantId = x.TenantId,
                        SortOrder = x.SortOrder,
                        GroupIcon = x.GroupIcon
                    }).Distinct().OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityGroupName);
        }
        public IQueryable<AuthUserGroupsForPermViewModel> GetDistinctActivityGroupsForSuperUserNavigation(IEnumerable<int> userModules)
        {
            return (from e in _currentDbContext.AuthActivityGroups
                    join t in _currentDbContext.AuthActivityGroupMaps on e.ActivityGroupId equals t.ActivityGroupId
                    join q in _currentDbContext.AuthActivities on t.ActivityId equals q.ActivityId
                    where e.IsActive == true && e.IsDeleted != true && q.IsActive == true && q.IsDeleted != true
                          && q.ExcludePermission != true && q.RightNav == true && t.IsDeleted != true && (!q.ModuleId.HasValue || userModules.Contains(q.ModuleId.Value))
                    select new AuthUserGroupsForPermViewModel
                    {
                        ActivityGroupId = e.ActivityGroupId,
                        ActivityGroupName = e.ActivityGroupName,
                        ActivityGroupDetail = e.ActivityGroupDetail,
                        ActivityGroupParentId = e.ActivityGroupParentId,
                        DateCreated = e.DateCreated,
                        DateUpdated = e.DateUpdated,
                        CreatedBy = e.CreatedBy,
                        UpdatedBy = e.UpdatedBy,
                        IsActive = e.IsActive,
                        IsDeleted = e.IsDeleted,
                        TenantId = e.TenantId,
                        SortOrder = e.SortOrder,
                        GroupIcon = e.GroupIcon
                    }).Distinct().OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityGroupName);
        }
        public IQueryable<AuthActivitiesForPermViewModel> GetDistinctActivityNonGroupsForSuperUserNavigation(IEnumerable<int> userModules)
        {
            return (from e in _currentDbContext.AuthActivities
                    where _currentDbContext.AuthActivityGroupMaps.All(x => x.ActivityId != e.ActivityId) && (!e.ModuleId.HasValue || userModules.Contains(e.ModuleId.Value))
                          && e.IsActive == true && e.IsDeleted != true && e.RightNav == true
                    select new AuthActivitiesForPermViewModel
                    {
                        Controller = e.ActivityController,
                        Action = e.ActivityAction,
                        ActivityName = e.ActivityName,
                        SortOrder = e.SortOrder
                    }).OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityName);
        }
        public IQueryable<AuthActivitiesForPermViewModel> GetDistinctActivitiesForSuperUserNavigation(IEnumerable<int> userModules)
        {
            return (from e in _currentDbContext.AuthActivities
                    join t in _currentDbContext.AuthActivityGroupMaps on e.ActivityId equals t.ActivityId
                    join x in _currentDbContext.AuthActivityGroups on t.ActivityGroupId equals x.ActivityGroupId
                    where t.IsDeleted != true && e.IsActive == true && e.IsDeleted != true && e.RightNav == true &&
                          (!e.ModuleId.HasValue || userModules.Contains(e.ModuleId.Value))
                    select new AuthActivitiesForPermViewModel
                    {
                        Controller = e.ActivityController,
                        Action = e.ActivityAction,
                        ActivityName = e.ActivityName,
                        SortOrder = e.SortOrder,
                        ActivityGroupId = x.ActivityGroupId
                    }).OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityName);
        }

        public IQueryable<AuthActivitiesForPermViewModel> GetDistinctActivityNonGroupsForNavigation(int userId, int warehouseId, IEnumerable<int> userModules)
        {
            return (from ep in _currentDbContext.AuthPermissions
                    join e in _currentDbContext.AuthActivities on ep.ActivityId equals e.ActivityId
                    where ep.UserId == userId && ep.WarehouseId == warehouseId &&
                          _currentDbContext.AuthActivityGroupMaps.All(x => x.ActivityId != e.ActivityId)
                          && ep.IsActive == true && ep.IsDeleted != true && e.RightNav == true
                    select new AuthActivitiesForPermViewModel
                    {
                        Controller = e.ActivityController,
                        Action = e.ActivityAction,
                        ActivityName = e.ActivityName,
                        SortOrder = e.SortOrder
                    }).OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityName);
        }

        public IQueryable<AuthActivitiesForPermViewModel> GetDistinctActivityPermissionsForUserNavigation(int userId, int warehouseId, IEnumerable<int> userModules)
        {
            return (from ep in _currentDbContext.AuthPermissions
                    join e in _currentDbContext.AuthActivities on ep.ActivityId equals e.ActivityId
                    join t in _currentDbContext.AuthActivityGroupMaps on e.ActivityId equals t.ActivityId
                    join x in _currentDbContext.AuthActivityGroups on t.ActivityGroupId equals x.ActivityGroupId
                    where x.IsDeleted != true && x.IsActive == true && ep.UserId == userId && ep.WarehouseId == warehouseId
                          && ep.IsActive == true && ep.IsDeleted != true && e.RightNav == true && e.IsActive == true
                          && e.IsDeleted != true && t.IsDeleted != true && ep.WarehouseId == warehouseId && (!e.ModuleId.HasValue || userModules.Contains(e.ModuleId.Value))
                    select new AuthActivitiesForPermViewModel
                    {
                        Controller = e.ActivityController,
                        Action = e.ActivityAction,
                        ActivityName = e.ActivityName,
                        SortOrder = e.SortOrder,
                        ActivityGroupId = x.ActivityGroupId
                    }).OrderBy(x => x.SortOrder).ThenBy(a => a.ActivityName);
        }

        // ******************** Activity Groups Mapping *********************

        public AuthActivityGroupMap GetActivityGroupMapById(int activityGroupMapId)
        {
            return _currentDbContext.AuthActivityGroupMaps.Find(activityGroupMapId);
        }

        public int SaveActivityGroupMap(AuthActivityGroupMap activityGroupMap, int userId, int tenantId)
        {
            activityGroupMap.DateCreated = DateTime.UtcNow;
            activityGroupMap.DateUpdated = DateTime.UtcNow;
            activityGroupMap.CreatedBy = userId;
            activityGroupMap.UpdatedBy = userId;
            activityGroupMap.TenantId = tenantId;
            _currentDbContext.Entry(activityGroupMap).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            int res = activityGroupMap.ActivityGroupMapId;
            return res;
        }

        public void UpdateActivityGroupMap(AuthActivityGroupMap activityGroupMap, int userId, int tenantId)
        {
            var UpdatedActivityGroupMap = _currentDbContext.AuthActivityGroupMaps.Find(activityGroupMap.ActivityGroupMapId);

            UpdatedActivityGroupMap.ActivityGroupId = activityGroupMap.ActivityGroupId;
            UpdatedActivityGroupMap.ActivityId = activityGroupMap.ActivityId;
            UpdatedActivityGroupMap.DateUpdated = DateTime.UtcNow;
            UpdatedActivityGroupMap.UpdatedBy = userId;
            _currentDbContext.Entry(UpdatedActivityGroupMap).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

        }

        public void DeleteActivityGroupMap(AuthActivityGroupMap activityGroupMap, int userId)
        {
            activityGroupMap.IsDeleted = true;
            activityGroupMap.UpdatedBy = userId;
            activityGroupMap.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(activityGroupMap).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public IEnumerable<ActivityGroupMapWithNames> GetActivityGroupMapsForGroupList()
        {

            return _currentDbContext.AuthActivityGroupMaps.Where(e => e.IsDeleted != true && e.AuthActivity.IsDeleted != true && e.AuthActivityGroup.IsDeleted != true)
                 .Select(x => new ActivityGroupMapWithNames
                 {
                     ActivityGroupMapId = x.ActivityGroupMapId,
                     ActivityName = _currentDbContext.AuthActivities.Where(a => a.ActivityId == x.ActivityId).Select(a => a.ActivityName).FirstOrDefault(),
                     GroupName = _currentDbContext.AuthActivityGroups.Where(a => a.ActivityGroupId == x.ActivityGroupId).Select(a => a.ActivityGroupName).FirstOrDefault()
                 });

        }

        public Boolean PermCheck(string controller, string action, int UserId, int WareHId)
        {

            Boolean Status = false;

            //get the handheld termianl activities
            AuthActivity Activity = _currentDbContext.AuthActivities
                .Where(e => e.ActivityController == controller && e.ActivityAction == action
                    && e.IsActive == true && e.IsDeleted != true)?.FirstOrDefault();

            if (Activity != null)
            {
                AuthPermission Permission = _currentDbContext.AuthPermissions.Where(e => e.ActivityId == Activity.ActivityId && e.UserId == UserId && e.WarehouseId == WareHId
               && e.IsActive == true && e.IsDeleted != true)?.FirstOrDefault();

                if (Permission != null)
                {
                    Status = true;
                }
            }

            return Status;
        }

        public Boolean PermCheckByActivityId(int activityId, int userId, int tenantLocationId)
        {
            return _currentDbContext.AuthPermissions.Any(x => x.ActivityId == activityId && x.UserId == userId && x.WarehouseId == tenantLocationId && x.IsDeleted != true);
        }

        public void RemoveAuthPermissions(List<AuthPermission> permissions)
        {
            foreach (var item in permissions)
            {
                // remove each old permission from db context
                _currentDbContext.AuthPermissions.Remove(item);
            }
            _currentDbContext.SaveChanges();

        }
        public void AddAuthPermissions(List<AuthPermission> permissions)
        {
            foreach (var item in permissions)
            {
                // add each new permission in database context
                _currentDbContext.AuthPermissions.Add(item);
            }
            _currentDbContext.SaveChanges();
        }

        public List<AuthPermission> GetPermissionsByUserId(int userId)
        {
            return _currentDbContext.AuthPermissions.Where(e => e.UserId == userId && e.IsDeleted != true).ToList();
        }

    }
}