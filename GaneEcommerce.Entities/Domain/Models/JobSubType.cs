using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class JobSubType 
    {
        [Key]
        [Display(Name = "Job SubType Id")]
        public int JobSubTypeId { get; set; }

        [Required]
        [Display(Name = "Job SubType")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public int TenantId { get; set; }
    }
}