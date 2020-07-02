using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    [Serializable]
    public class caUser
    {
        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public string UserFirstName { get; private set; }
        public string UserLastName { get; private set; }
        public string UserEmail { get; private set; }
        public DateTime? DateCreated { get; private set; }
        public DateTime? DateUpdated { get; private set; }
        public int? CreatedBy { get; private set; }
        public int? UpdatedBy { get; private set; }
        public bool IsActive { get; private set; }
        public bool? IsDeleted { get; private set; }
        public int TenantId { get; private set; }
        public virtual ICollection<AuthPermission> AuthPermissions { get; private set; }
        public bool AuthUserStatus { get; private set; }
        public bool? SuperUser { get; set; }
        public string UserCulture { get; set; }
        public string UserTimeZoneId { get; set; }
        public int? AccountId { get; set; }
        public caUser() { }

        public bool AuthoriseUser(string uname, string upass)
        {
            IApplicationContext context = DependencyResolver.Current.GetService<IApplicationContext>();

            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;

            upass = GaneStaticAppExtensions.GetMd5(upass);

            if (HttpContext.Current.Session["caTenant"] == null)
            {
                // set error details
                caError error = new caError();
                error.ErrorTtile = "Client not validated";
                error.ErrorMessage = "Sorry, system is unable to validate client";
                error.ErrorDetail = "Either client is not registered, inactive or ambiguous, please contact support";

                HttpContext.Current.Session["caError"] = error;
                HttpContext.Current.Session["ErrorUrl"] = "~/error";
            }

            else
            {
                caTenant tenant = (caTenant)HttpContext.Current.Session["caTenant"];
                TenantId = tenant.TenantId;
            }

            var Users = context.AuthUsers.AsNoTracking().Where(e => e.UserName.Equals(uname, StringComparison.CurrentCultureIgnoreCase) && e.UserPassword == upass.Trim() && e.TenantId == TenantId && e.IsActive && e.IsDeleted != true)
                .Include(x => x.AuthPermissions.Select(y => y.AuthActivity))
                .ToList();

            if (Users.Any() && Users.Count() < 2)
            {
                var user = Users.FirstOrDefault();

                UserId = user.UserId;
                UserName = user.UserName;
                UserFirstName = user.UserFirstName;
                UserLastName = user.UserLastName;
                UserEmail = user.UserEmail;
                DateCreated = user.DateCreated;
                DateUpdated = user.DateUpdated;
                CreatedBy = user.CreatedBy;
                UpdatedBy = user.UpdatedBy;
                IsActive = user.IsActive;
                IsDeleted = user.IsDeleted;
                TenantId = user.TenantId;
                AuthPermissions = user.AuthPermissions;
                SuperUser = user.SuperUser;
                UserCulture = user.UserCulture;
                UserTimeZoneId = user.UserTimeZoneId;
                AccountId = user.AccountId;
                AuthUserStatus = true;
            }

            return AuthUserStatus;
        }

        public bool AuthoriseWebsiteUser(string uname, string upass)
        {
            IApplicationContext context = DependencyResolver.Current.GetService<IApplicationContext>();

            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;

            caTenantWebsites website = new caTenantWebsites();

            upass = GaneStaticAppExtensions.GetMd5(upass);

            if (HttpContext.Current.Session["CurrentTenantWebsites"] == null)
            {
                // set error details
                caError error = new caError();
                error.ErrorTtile = "Client not validated";
                error.ErrorMessage = "Sorry, system is unable to validate client";
                error.ErrorDetail = "Either client is not registered, inactive or ambiguous, please contact support";

                HttpContext.Current.Session["caError"] = error;
                HttpContext.Current.Session["ErrorUrl"] = "~/error";
            }

            else
            {
                website = (caTenantWebsites)HttpContext.Current.Session["CurrentTenantWebsites"];
                TenantId = website.TenantId;
            }

            var Users = context.AuthUsers.AsNoTracking().Where(e => e.UserName.Equals(uname, StringComparison.CurrentCultureIgnoreCase) && e.UserPassword == upass.Trim() && e.TenantId == TenantId && e.SiteId == website.SiteID && e.IsActive && e.IsDeleted != true)
                .Include(x => x.AuthPermissions.Select(y => y.AuthActivity))
                .ToList();

            if (Users.Any() && Users.Count() < 2)
            {
                var user = Users.FirstOrDefault();
                if (!user.AccountId.HasValue || user.AccountId <= 0)
                {
                    return false;
                }
                UserId = user.UserId;
                UserName = user.UserName;
                UserFirstName = user.UserFirstName;
                UserLastName = user.UserLastName;
                UserEmail = user.UserEmail;
                DateCreated = user.DateCreated;
                DateUpdated = user.DateUpdated;
                CreatedBy = user.CreatedBy;
                UpdatedBy = user.UpdatedBy;
                IsActive = user.IsActive;
                IsDeleted = user.IsDeleted;
                TenantId = user.TenantId;
                AuthPermissions = user.AuthPermissions;
                SuperUser = user.SuperUser;
                UserCulture = user.UserCulture;
                UserTimeZoneId = user.UserTimeZoneId;
                AccountId = user.AccountId;
                AuthUserStatus = true;
            }

            return AuthUserStatus;
        }


    }
}