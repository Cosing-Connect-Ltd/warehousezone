using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class AccountSector
    {
        [Key]
        [Display(Name = "Sector Id")]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Sector Name")]
        public string Name { get; set; }
        public bool IsDeleted{ get; set; }
    }
}
