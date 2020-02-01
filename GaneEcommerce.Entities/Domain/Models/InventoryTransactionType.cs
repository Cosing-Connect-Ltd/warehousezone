using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    using System;
    using System.Collections.Generic;
    [Serializable]
    public class InventoryTransactionType
    {
        public InventoryTransactionType()
        {
            this.InventoryTransactions = new HashSet<InventoryTransaction>();
        }

        [Key]
        [Display(Name = "Transaction Type Id")]
        public int InventoryTransactionTypeId { get; set; }

        [Required]
        [Display(Name = "Order Type")]
        public string OrderType { get; set; }
        [Display(Name = "Transaction Type")]
        public string InventoryTransactionTypeName { get; set; }
        [Display(Name = "Date Created")]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public System.DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public Nullable<int> CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }

    }
}
