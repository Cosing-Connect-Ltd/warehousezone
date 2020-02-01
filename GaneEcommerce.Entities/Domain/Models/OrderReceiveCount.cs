using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderReceiveCount : PersistableEntity<int>
    {
        public OrderReceiveCount()
        {
            OrderReceiveCountDetail = new HashSet<OrderReceiveCountDetail>();
        }

        [Key]
        public Guid ReceiveCountId { get; set; }
        public int OrderID { get; set; }
        [Required]
        public string ReferenceNo { get; set; }
        public string Notes { get; set; }
        public int WarehouseId { get; set; }
        public virtual Order Order { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual AuthUser User { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual TenantLocations TenantLocations { get; set; }
        public virtual ICollection<OrderReceiveCountDetail> OrderReceiveCountDetail { get; set; }
    }

    public class OrderReceiveCountDetail
    {
        [Key]
        public Guid ReceiveCountDetailId { get; set; }
        [Required]
        public Guid ReceiveCountId { get; set; }
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [DataType(DataType.Text)]
        [Required]
        public decimal Counted { get; set; }
        public decimal Demaged { get; set; }
        public int OrderDetailID { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        public bool? IsDeleted { get; set; }
        [ForeignKey("ReceiveCountId")]
        public virtual OrderReceiveCount OrderReceiveCount { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }


    }


}