﻿using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;
using System;
using Ganedata.Core.Entities.Enums;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IUserService
    {
        IEnumerable<AuthUser> GetAllAuthUsers(int tenantId);
        List<AuthUserListForGridViewModel> GetAllAuthUsersForGrid(int tenantId);
        AuthUser GetAuthUserById(int userId);
        AuthUser GetAuthUserByAccountId(int? accountId);
        AuthUser GetAuthUserByUserName(string userName, int tenantId);
        List<AuthUser> GetAuthUsersByTenantAndDateUpdated(int tenantId, DateTime dateUpdated);
        int SaveAuthUser(AuthUser user, int userId, int tenantId);
        void UpdateAuthUser(AuthUser user, int userId, int tenantId);
        void DeleteAuthUser(AuthUser user, int userId);
        int SaveAuthUserLogin(AuthUserLogin userLogin, int userId, int tenantId);
        int IsUserNameExists(string userName, int tenantId);
        int IsUserNameExistsForSite(string userName, int siteId);
        void UpdateAuthUserForPermissions(AuthUser user, int userId, int tenantId);
        int GetResourceIdByUserId(int userId);
        string GetResourceNameByUserId(int? userId);
        IEnumerable<ApiCredentials> GetApiCredentials(int TenantId, int WarehouseId, ApiTypes SiteType);
        Tenant GetTenantBySiteId(int SiteId);
        TenantLocations GetWarehouseBySiteId(int SiteId);
        UserLoginStatusResponseViewModel GetUserLoginStatus(UserLoginStatusViewModel loginStatus, bool? webUser = null);
        bool CreateOrUpdateAuthUserGroup(AuthUserGroups authUserGroups, int UserId, int TenantId);
        AuthUserGroups GetUserGroupsById(int groupId);
        AuthUserGroups RemoveUserGroupsById(int groupId, int UserId);
        IEnumerable<AuthUserGroups> GetAllAuthUserGroups(int TenantId);
        Task<bool> CreateUserVerificationCode(int userId, int tenantId, UserVerifyTypes type);
        bool VerifyUserVerificationCode(int userId, int tenantId, string code, UserVerifyTypes type);
        Task<bool> SendSmsBroadcast(string user, string password, string to, string from, string reference, string message);
        string GenerateVerifyRandomNo();
        IEnumerable<AuthUser> GetUsersAgainstPermission(int tenantId, int warehouseId, string premissionName);
    }
}