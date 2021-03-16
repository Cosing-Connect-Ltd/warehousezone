using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

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
        public string Account { get; set; }
        public string UserGroup { get; set; }
    }

    public class UserLoginStatusViewModel
    {
        public string SerialNo { get; set; }
        public string UserName { get; set; }
        public string Md5Pass { get; set; }
        public int TenantId { get; set; }
    }

    public class UserLoginStatusResponseViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int? AccountId { get; set; }
        public bool VerificationRequired { get; set; }
        public bool EmailVerified { get; set; }
        public bool MobileNumberVerified { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string UserMobileNumber { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string PersonalReferralCode { get; set; }
        public bool ReferralConfirmed { get; set; }
    }

    public class UserRegisterRequestViewModel
    {
        public string SerialNo { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        [Required]
        public string UserMobileNumber { get; set; }
        [Required]
        public string Md5Pass { get; set; }
        [Required]
        public int TenantId { get; set; }
    }

    public class UserVerifyRequestViewModel
    {
        public string SerialNo { get; set; }
        public int UserId { get; set; }
        public UserVerifyTypes Type { get; set; }
        public string Code { get; set; }
    }
    public class UserPasswordResetRequestModel
    {
        public string SerialNo { get; set; }
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
    }

    public class UserVerifyResponseModel
    {
        public string Code { get; set; }
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
    }

}