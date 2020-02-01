using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IActivityServices
    {
        IEnumerable<AuthActivity> GetAllActivities(int tenantId);
        AuthActivity GetActivityById(int activityId);
        int SaveActivity(AuthActivity activity, int userId, int tenantId);
        bool UpdateActivity(AuthActivity activity, int userId, int tenantId);  
        void DeleteActivity(AuthActivity activity, int userId);
        IQueryable<AuthActivitiesForPermViewModel> GetAuthActivitiesForPermByGroup(AuthUserGroupsForPermViewModel activityGroup, IEnumerable<int> userModules, int TenantId);
        IQueryable<AuthActivitiesForPermViewModel> GetAuthActivitiesForPermNoGroup(IEnumerable<int> userModules);
        List<AuthActivity> GetExcludedActivities();


        // ******************** Activity Groups *********************
        IEnumerable<AuthActivityGroup> GetAllActivityGroups(int CurrentTenantId);
        AuthActivityGroup GetActivityGroupById(int activityGroupId);
        int SaveActivityGroup(AuthActivityGroup activityGroup, int userId, int tenantId);
        void UpdateActivityGroup(AuthActivityGroup activityGroup, int userId, int tenantId);
        void DeleteActivityGroup(AuthActivityGroup activityGroup, int userId);
        IQueryable<AuthUserGroupsForPermViewModel> GetDistinctActivityGroupsForPerm(IEnumerable<int> userModules);
        IQueryable<AuthUserGroupsForPermViewModel> GetDistinctActivityGroupsForNavigation(int userId, int warehouseId, IEnumerable<int> userModules);
        IQueryable<AuthActivitiesForPermViewModel> GetDistinctActivityNonGroupsForNavigation(int userId, int warehouseId, IEnumerable<int> userModules);

        IQueryable<AuthActivitiesForPermViewModel> GetDistinctActivityPermissionsForUserNavigation(int userId,
            int warehouseId, IEnumerable<int> userModules);
        IQueryable<AuthUserGroupsForPermViewModel> GetDistinctActivityGroupsForSuperUserNavigation(
            IEnumerable<int> userModules);

        IQueryable<AuthActivitiesForPermViewModel> GetDistinctActivitiesForSuperUserNavigation(
            IEnumerable<int> userModules);

        IQueryable<AuthActivitiesForPermViewModel> GetDistinctActivityNonGroupsForSuperUserNavigation(
            IEnumerable<int> userModules);

        // ******************** Activity Groups Mapping *********************
        AuthActivityGroupMap GetActivityGroupMapById(int activityGroupMapId);
        int SaveActivityGroupMap(AuthActivityGroupMap activityGroupMap, int userId, int tenantId);
        void UpdateActivityGroupMap(AuthActivityGroupMap activityGroupMap, int userId, int tenantId);
        void DeleteActivityGroupMap(AuthActivityGroupMap activityGroupMap, int userId);
        IEnumerable<ActivityGroupMapWithNames> GetActivityGroupMapsForGroupList();

        // ******************** Activity Permissions *********************

        Boolean PermCheck(string controller, string action, int UserId, int WareHId);
        Boolean PermCheckByActivityId(int activityId, int userId, int tenantLocationId);
        void RemoveAuthPermissions(List<AuthPermission> permissions);
        void AddAuthPermissions(List<AuthPermission> permissions);
        List<AuthPermission> GetPermissionsByUserId(int userId);
        List<AuthPermissionsViewModel> GetAllNavigationPermissionsInGroup(int tenantId);

        List<WarehousePermissionViewModel> GetAllPermittedWarehousesForUser(int userId, int tenantId, bool isSuperAdmin = false, bool includeMobileLocations=false);

    }
}
