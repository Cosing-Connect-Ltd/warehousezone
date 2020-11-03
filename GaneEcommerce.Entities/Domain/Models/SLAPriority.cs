using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class SLAPriorit : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Priority Id")]
        public int SLAPriorityId { get; set; }
        [Display(Name = "Priority")]
        [Required]
        public string Priority { get; set; }
        [Display(Name = "Description")]
        public string  Description { get; set; }

        public string Colour { get; set; }
        public int? SortOrder { get; set; }
    }
}