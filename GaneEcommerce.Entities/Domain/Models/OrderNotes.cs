using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderNotes : PersistableEntity<int>
    {
        [Key]
        public int OrderNoteId { get; set; }
        public int OrderID { get; set; }
        [Required]
        public string Notes { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual AuthUser User { get; set; }
    }
}