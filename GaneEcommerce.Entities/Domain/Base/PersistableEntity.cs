using Ganedata.Core.Entities.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public abstract class PersistableEntity<TPrimaryKey>
    {
        public int TenantId { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }

        public bool? IsDeleted { get; set; }

        public void UpdateCreatedInfo(int userId)
        {
            this.CreatedBy = userId;
            this.DateCreated = DateTime.UtcNow;
        }
        public void UpdateUpdatedInfo(int userId)
        {
            this.UpdatedBy = userId;
            this.DateUpdated = DateTime.UtcNow;

        }
    }
}