using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class AuthUserGroupsForPermViewModel
    {
        public int ActivityGroupId { get; set; }
        public string ActivityGroupName { get; set; }
        public string ActivityGroupDetail { get; set; }
        public int? ActivityGroupParentId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int TenantId { get; set; }
        public int SortOrder { get; set; }
        public string GroupIcon { get; set; }
    }

    public class AuthActivitiesForPermViewModel
    {
        public int ActivityId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ActivityName { get; set; }
        public int SortOrder { get; set; }
        public int ActivityGroupId { get; set; }
    }

    public class AuthUserListForGridViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserLoginStatusViewModel
    {
        public string UserName { get; set; }
        public string Md5Pass { get; set; }
        public int TenantId { get; set; }
    }
}