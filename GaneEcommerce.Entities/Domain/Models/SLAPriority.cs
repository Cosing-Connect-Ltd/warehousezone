using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class SLAPriorit:PersistableEntity<int>
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
    }
}