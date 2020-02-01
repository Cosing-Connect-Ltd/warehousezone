using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderStatus
    {
         public OrderStatus()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        [Display(Name = "Order Status Id")]
        public int OrderStatusID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Order Status")]
        public string Status { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}