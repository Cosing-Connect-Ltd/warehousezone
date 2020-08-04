using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class AuthUserVerifyCodes
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string VerifyCode { get; set; }
        public UserVerifyTypes VerifyType { get; set; }
        public DateTime Expiry { get; set; }
        public DateTime DateCreated { get; set; }
        public int TenantId { get; set; }
        [ForeignKey("UserId")]
        public virtual AuthUser AuthUser { get; set; }
    }
}