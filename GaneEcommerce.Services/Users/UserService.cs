﻿using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Data.Entity;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Entities.Enums;
using AutoMapper;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Configuration;

namespace Ganedata.Core.Services
{
    public class UserService : IUserService
    {

        private readonly IApplicationContext _currentDbContext;
        private readonly ITenantsServices _tenantServices;
        //private readonly IGaneConfigurationsHelper _configurationsHelper;
        private readonly IMapper _mapper;
        private readonly IShoppingVoucherService _shoppingVoucherService;

        public UserService(IApplicationContext currentDbContext, ITenantsServices tenantServices, IMapper mapper, IShoppingVoucherService shoppingVoucherService)
        {
            _currentDbContext = currentDbContext;
            _tenantServices = tenantServices;
            _mapper = mapper;
            _shoppingVoucherService = shoppingVoucherService;
        }

        public IEnumerable<AuthUser> GetAllAuthUsers(int tenantId)
        {
            return _currentDbContext.AuthUsers.Where(m => m.TenantId == tenantId && m.IsDeleted != true).OrderBy(m => m.UserName);
        }

        public IEnumerable<AuthUser> GetUsersAgainstPermission(int tenantId, int warehouseId,string controller, string action)
        {
            AuthActivity Activity = _currentDbContext.AuthActivities
                .Where(e => e.ActivityController==controller && e.ActivityAction == action && e.IsActive == true && e.IsDeleted != true)?.FirstOrDefault();

            return _currentDbContext.AuthUsers.Where(u => u.TenantId == tenantId &&
                                                          u.AuthPermissions.Any(c => c.ActivityId == Activity.ActivityId && c.IsActive && c.IsDeleted !=true)
                                                          && u.IsActive
                                                          && u.IsDeleted != true);
        }


        public List<AuthUserListForGridViewModel> GetAllAuthUsersForGrid(int tenantId)
        {
            return (from p in _currentDbContext.AuthUsers
                    where ((p.TenantId == tenantId) && (p.IsDeleted != true) && (p.SuperUser != true))
                    select new AuthUserListForGridViewModel
                    {
                        UserId = p.UserId,
                        UserName = p.UserName,
                        FirstName = p.UserFirstName,
                        LastName = p.UserLastName,
                        Email = p.UserEmail,
                        DateUpdated = p.DateUpdated,
                        IsActive = p.IsActive,
                        Account = _currentDbContext.Account.FirstOrDefault(u => u.AccountID == p.AccountId).CompanyName ?? "",
                        UserGroup = p.AuthUserGroups.Name



                    }).ToList();
        }

        public AuthUser GetAuthUserById(int userId)
        {
            return _currentDbContext.AuthUsers.Find(userId);
        }

        public AuthUser GetAuthUserByResetPasswordCode(string resetPasswordCode)
        {
            return _currentDbContext.AuthUsers.FirstOrDefault(m => m.ResetPasswordCode.Equals(resetPasswordCode));
        }

        public AuthUser GetAuthUserByAccountId(int? accountId)
        {
            return _currentDbContext.AuthUsers.Where(x => x.AccountId == accountId).FirstOrDefault();
        }
         
        public AuthUser GetAuthUserByUserName(string userName, int tenantId)
        {
            return _currentDbContext.AuthUsers.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower() && x.TenantId == tenantId);
        }
        public AuthUser GetAuthUserByUserInfo(string email, string phoneNumber, int tenantId)
        {
            return _currentDbContext.AuthUsers.FirstOrDefault(x =>
                (x.UserMobileNumber.ToLower() == phoneNumber.ToLower() || (email!=null &&  x.UserEmail.ToLower() == email.ToLower()))
                && x.TenantId == tenantId);
        }

        public AuthUser GetAuthUserByName(string userName)
        {
            return _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserName == userName && m.IsDeleted != true);
        }

        public List<AuthUser> GetAuthUsersByTenantAndDateUpdated(int tenantId, DateTime dateUpdated)
        {
            return _currentDbContext.AuthUsers.Where(e => e.TenantId == tenantId && e.SuperUser != true && (!e.DateUpdated.HasValue || e.DateUpdated > dateUpdated)).ToList();
        }

        public int SaveAuthUser(AuthUser user, int userId, int tenantId)
        {
            user.DateCreated = DateTime.UtcNow;
            user.DateUpdated = DateTime.UtcNow;
            user.CreatedBy = userId;
            user.UpdatedBy = userId;
            user.TenantId = tenantId;
            if (userId < 1)
            {
                user.PersonalReferralCode = GetNextUniquePersonalReferralCode();
            }
            _currentDbContext.Entry(user).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return user.UserId;
        }

        public string GetNextUniquePersonalReferralCode()
        {
            var code = string.Empty;
            var isNewCode = false;
            while (!isNewCode)
            {
                code = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                isNewCode = !_currentDbContext.AuthUsers.Any(m => m.PersonalReferralCode != null && m.PersonalReferralCode.ToUpper().Equals(code));
            }
            return code;
        }

        public void UpdateAllUsersWithPersonalReferralCode()
        {
            var authUsers = _currentDbContext.AuthUsers.Where(m => string.IsNullOrEmpty(m.PersonalReferralCode) && m.IsActive && m.IsDeleted != true);
            foreach (var item in authUsers)
            {
                item.PersonalReferralCode = GetNextUniquePersonalReferralCode();
                _currentDbContext.Entry(item).Property(m => m.PersonalReferralCode).IsModified = true;
            }
            _currentDbContext.SaveChanges();
        }

        public int CreateNewEcommerceUser(string email, string firstName, string lastName, string password, int accountId, int siteId, int tenantId, int currentUserId)
        {
            var authUser = new AuthUser
            {
                UserPassword = GaneStaticAppExtensions.GetMd5(password),
                UserEmail = email,
                UserFirstName = firstName,
                UserLastName = lastName,
                UserName = email,
                SiteId = siteId,
                IsActive = false,
                AccountId = accountId,
            };

            SaveAuthUser(authUser, currentUserId, tenantId);

            return authUser.UserId;
        }

        public void UpdateAuthUser(AuthUser user, int userId, int tenantId, bool hashedPwd = false)
        {
            user.UpdateUpdatedInfo(userId);
            _currentDbContext.AuthUsers.Attach(user);
            var entry = _currentDbContext.Entry(user);
            if (!string.IsNullOrWhiteSpace(user.UserFirstName))
            {
                entry.Property(e => e.UserName).IsModified = true;
            }

            if (!string.IsNullOrWhiteSpace(user.UserMobileNumber))
            {
                entry.Property(e => e.UserMobileNumber).IsModified = true;
            }

            if (!string.IsNullOrWhiteSpace(user.UserLastName))
            {
                entry.Property(e => e.UserLastName).IsModified = true;
            }

            if (!string.IsNullOrWhiteSpace(user.UserFirstName))
            {
                entry.Property(e => e.UserFirstName).IsModified = true;
            }

            if (!string.IsNullOrWhiteSpace(user.UserEmail))
            {
                entry.Property(e => e.UserEmail).IsModified = true;
            }

            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            entry.Property(e => e.IsActive).IsModified = true;
            entry.Property(e => e.AccountId).IsModified = true;
            entry.Property(e => e.UserGroupId).IsModified = true;
            entry.Property(e => e.MobileNumberVerified).IsModified = true;
            entry.Property(e => e.VerificationRequired).IsModified = true;
            //dont change password if password field is blank/null
            if (!hashedPwd && user.UserPassword != null)
            {
                // change user password into MD5 hash value
                user.UserPassword = GaneStaticAppExtensions.GetMd5(user.UserPassword);
                entry.Property(e => e.UserPassword).IsModified = true;
            }

            if (hashedPwd && user.UserPassword != null)
            {
                // Keep the user password with MD5 as it is and set it to be modified
                entry.Property(e => e.UserPassword).IsModified = true;
            }

            _currentDbContext.SaveChanges();
        }

        public void UpdateAuthUserForPermissions(AuthUser user, int userId, int tenantId)
        {
            user.UpdateUpdatedInfo(userId);
            _currentDbContext.AuthUsers.Attach(user);
            var entry = _currentDbContext.Entry(user);
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            _currentDbContext.SaveChanges();
        }

        public void DeleteAuthUser(AuthUser user, int userId)
        {
            user.IsDeleted = true;
            user.UpdatedBy = userId;
            user.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(user).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public int SaveAuthUserLogin(AuthUserLogin userLogin, int userId, int tenantId)
        {
            userLogin.UserId = userId;
            userLogin.TenantId = tenantId;
            userLogin.DateLoggedIn = DateTime.UtcNow;

            _currentDbContext.Entry(userLogin).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return userLogin.UserLoginId;
        }

        public int IsUserNameExists(string userName, int tenantId)
        {
            return (from p in _currentDbContext.AuthUsers
                    where (p.UserName == userName && p.TenantId == tenantId && p.IsDeleted != true)
                    select p).Count();
        }

        public int IsUserNameExistsForSite(string userName, int siteId)
        {
            return (from p in _currentDbContext.AuthUsers
                    where (p.UserName == userName && p.SiteId == siteId && p.IsDeleted != true)
                    select p).Count();
        }

        public int GetResourceIdByUserId(int userId)
        {
            var resource = _currentDbContext.Resources.FirstOrDefault(m => m.AuthUserId == userId && m.IsDeleted != true);
            return resource?.ResourceId ?? 0;
        }

        public string GetResourceNameByUserId(int? userId = 0)
        {
            string name = "";

            if (userId > 0)
            {
                var resource = _currentDbContext.Resources.FirstOrDefault(m => m.AuthUserId == userId && m.IsDeleted != true);
                name = resource?.Name ?? "";
            }

            return name;
        }

        public IEnumerable<ApiCredentials> GetApiCredentials(int TenantId, int WarehouseId, ApiTypes SiteType)
        {

            return _currentDbContext.ApiCredentials.Where(u => u.DefaultWarehouseId == WarehouseId && u.TenantId == TenantId && u.IsDeleted != true && u.ApiTypes == SiteType);

        }

        public Tenant GetTenantBySiteId(int SiteId)
        {
            return _currentDbContext.TenantWebsites.FirstOrDefault(u => u.SiteID == SiteId && u.IsDeleted != true).Tenant;
        }
        public TenantLocations GetWarehouseBySiteId(int SiteId)
        {
            return _currentDbContext.TenantWebsites.FirstOrDefault(u => u.SiteID == SiteId && u.IsDeleted != true).Warehouse;
        }

        public UserLoginStatusResponseViewModel GetUserLoginStatus(UserLoginStatusViewModel loginStatus, bool? webUser = null)
        {
            UserLoginStatusResponseViewModel resp = new UserLoginStatusResponseViewModel();

            var user = _currentDbContext.AuthUsers.AsNoTracking().FirstOrDefault(e => (e.UserName.Equals(loginStatus.UserName, StringComparison.CurrentCultureIgnoreCase) || e.UserMobileNumber.Equals(loginStatus.UserName, StringComparison.CurrentCultureIgnoreCase))
                                                                                      && e.UserPassword.Equals(loginStatus.Md5Pass.Trim(), StringComparison.CurrentCultureIgnoreCase)
                                                                                      && (e.WebUser == webUser) && e.TenantId == loginStatus.TenantId && e.IsActive && e.IsDeleted != true);
            if (user != null)
            {
                _shoppingVoucherService.LoadDefaultSystemVouchersForNewUser(user.UserId);
                _mapper.Map(user, resp);
                resp.Success = true;
                resp.shopId = user.TenantId;
                resp.ShopName = "Becseco";
            }

            return resp;
        }

        public IEnumerable<AuthUserGroups> GetAllAuthUserGroups(int TenantId)
        {
            return _currentDbContext.AuthUserGroups.Where(u => u.TenantId == TenantId && u.IsDeleted != true);
        }
        public bool CreateOrUpdateAuthUserGroup(AuthUserGroups authUserGroup, int UserId, int TenantId)
        {
            var authUserGroups = _currentDbContext.AuthUserGroups.AsNoTracking().Where(x => x.TenantId == TenantId
             && x.GroupId == authUserGroup.GroupId && x.IsDeleted != true).FirstOrDefault();
            if (authUserGroups == null)
            {
                authUserGroup.TenantId = TenantId;
                authUserGroup.UpdateCreatedInfo(UserId);
                _currentDbContext.AuthUserGroups.Add(authUserGroup);
                _currentDbContext.SaveChanges();
            }
            else
            {
                authUserGroup.TenantId = TenantId;
                authUserGroup.CreatedBy = authUserGroups.CreatedBy;
                authUserGroup.DateCreated = authUserGroups.DateCreated;
                authUserGroup.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(authUserGroup).State = EntityState.Modified;

            }

            _currentDbContext.SaveChanges();
            return true;

        }
        public AuthUserGroups GetUserGroupsById(int groupId)
        {
            return _currentDbContext.AuthUserGroups.Find(groupId);
        }
        public AuthUserGroups RemoveUserGroupsById(int groupId, int UserId)
        {
            var authUserGroups = _currentDbContext.AuthUserGroups.Where(x => x.GroupId == groupId && x.IsDeleted != true).FirstOrDefault();
            if (authUserGroups == null)
            {
                authUserGroups.IsDeleted = true;
                authUserGroups.UpdateUpdatedInfo(UserId);
                _currentDbContext.Entry(authUserGroups).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return authUserGroups;
        }


        public async Task<UserVerifyResponseModel> CreateUserVerificationCode(int userId, int tenantId, UserVerifyTypes type)
        {
            var user = GetAuthUserById(userId);
            return user == null ? null : await CreateUserVerificationCodeByUser(user, tenantId, type);
        }

        public async Task<UserVerifyResponseModel> CreateUserVerificationCodeByUser(AuthUser user, int tenantId, UserVerifyTypes vtype = UserVerifyTypes.Mobile)
        {
            var res = false;
            var code = GenerateVerifyRandomNo();
            var tenant = _tenantServices.GetByClientId(tenantId);

            if (vtype == UserVerifyTypes.Mobile)
            {
                res = await SendSmsBroadcast(user.UserMobileNumber, tenant.TenantName, user.UserId.ToString(),
                    $"{code} is your verification code");
            }

            if (res)
            {
                AuthUserVerifyCodes record = new AuthUserVerifyCodes
                {
                    UserId = user.UserId,
                    VerifyCode = code,
                    VerifyType = vtype,
                    TenantId = tenantId,
                    DateCreated = DateTime.Now,
                    Expiry = DateTime.Now
                };

                _currentDbContext.AuthUserVerifyCodes.Add(record);
                _currentDbContext.SaveChanges();
                return new UserVerifyResponseModel()
                {
                    UserId = user.UserId,
                    Code = code,
                    EmailAddress = user.UserEmail
                };
            }

            return  null;
        }

        public async Task<UserVerifyResponseModel> CreateUserVerificationCode(string emailAddress, int tenantId, UserVerifyTypes type)
        {
            var user = _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserEmail.ToLower() == emailAddress.ToLower() && m.IsActive && m.IsDeleted!=true);

            return user == null ? null : await CreateUserVerificationCodeByUser(user, tenantId, type);
        }

        public bool VerifyUserVerificationCode(int userId, int tenantId, string code, UserVerifyTypes type)
        {
            var record = _currentDbContext.AuthUserVerifyCodes.AsNoTracking().Where(x => x.UserId == userId && x.TenantId == tenantId && x.VerifyType == type).OrderByDescending(x => x.Id).FirstOrDefault();

            if (record?.VerifyCode?.Trim() == code.Trim())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendSmsBroadcast(string to, string from, string reference, string message)
        {
            var smsBraodcastUser = ConfigurationManager.AppSettings["SMSBroadcastUser"];
            var smsBraodcastPassword = ConfigurationManager.AppSettings["SMSBroadcastPassword"];
            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.QueryString.Add("username", smsBraodcastUser);
            client.QueryString.Add("password", smsBraodcastPassword);
            client.QueryString.Add("to", to);
            client.QueryString.Add("from", from);
            client.QueryString.Add("message", message);
            client.QueryString.Add("ref", reference);
            client.QueryString.Add("maxsplit", "1");
            var baseurl = new Uri("https://www.smsbroadcast.co.uk/api-adv.php");
            var data = client.OpenRead(baseurl);
            var reader = new StreamReader(data);
            await reader.ReadToEndAsync();
            data.Close();
            reader.Close();
            return true;
        }

        public string GenerateVerifyRandomNo()
        {
            Random _random = new Random();
            return _random.Next(0, 999999).ToString("D6");
        }

        public IEnumerable<AuthUser> GetAuthUserByIds(int[] userIds)
        {
            return _currentDbContext.AuthUsers.Where(m => userIds.Contains(m.UserId) && m.IsDeleted != true).ToList();
        }

        public AuthUser UpdateUserPassword(int authUserId, string resetPasswordCode, string newPassword)
        {
            var user = _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserId == authUserId && m.ResetPasswordCode == resetPasswordCode && m.IsActive && m.IsDeleted != true);
            if (user != null)
            {
                user.UserPassword = GaneStaticAppExtensions.GetMd5(newPassword);
                user.ResetPasswordCodeExpiry = null;
                user.ResetPasswordCode = null;
                _currentDbContext.Entry(user).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return user;
        }
        public AuthUser UpdateUserPaypalCustomerId(int authUserId, string paypalCustomerId)
        {
            var user = _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserId == authUserId);
            if (user != null && !string.IsNullOrWhiteSpace(paypalCustomerId))
            {
                user.PaypalCustomerId = paypalCustomerId;
                _currentDbContext.Entry(user).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return user;
        }
    }
}