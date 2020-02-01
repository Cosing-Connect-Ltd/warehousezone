using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class JobType : PersistableEntity<int>
    {
        public JobType()
        {
            Orders = new HashSet<Order>();
            AppointmentResources = new  HashSet<Resources>();
        }

        [Key]
        [Display(Name = "Job Type Id")]
        public int JobTypeId { get; set; }
        [Required]
        [Display(Name = "Job Type")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        [Display(Name = "Resources")]
        public virtual ICollection<Resources> AppointmentResources { get; set; }

    }
}