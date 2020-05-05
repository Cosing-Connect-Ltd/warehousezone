using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class StockMovement : PersistableEntity<int>
    {
        [Key]
        public Guid? StockMovementId { get; set; }
        public int WarehouseId { get; set; }
    }
}