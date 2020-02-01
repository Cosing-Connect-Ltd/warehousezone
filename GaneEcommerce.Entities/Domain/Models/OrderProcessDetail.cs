using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderProcessDetail
    {
        public OrderProcessDetail()
        {
            this.PalletProducts = new HashSet<PalletProduct>();
        }

        [Key]
        [Display(Name = "Detail Id")]
        public int OrderProcessDetailID { get; set; }
        [Required]
        [Display(Name = "Process Id")]
        public int OrderProcessId { get; set; }
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [DataType(DataType.Text)]
        [Required]
        [Display(Name = "Quantity")]
        public decimal QtyProcessed { get; set; }
        [Display(Name = "Date Created")]
        public int? OrderDetailID { get; set; }
        public DateTime? DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int UpdatedBy { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        [Display(Name = "Terminal Id")]
        public int? TerminalId { get; set; }
        public virtual OrderProcess OrderProcess { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
        [ForeignKey("TerminalId")]
        public virtual Terminals Terminals { get; set; }
        public virtual ICollection<PalletProduct> PalletProducts { get; set; }

        public decimal PalletedQuantity
        {
            get
            {
                if (PalletProducts == null || !PalletProducts.Any()) return 0;
                return this.PalletProducts.Where(x => x.OrderProcessDetailID == OrderProcessDetailID && x.IsDeleted != true).ToList().Sum(x => x.Quantity);
            }
        }

        // to store product combined IDs // timber properties
        public string ID { get; set; }
        public string FscPercent { get; set; }
    }
}