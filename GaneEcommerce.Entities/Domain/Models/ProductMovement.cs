using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductMovement : PersistableEntity<int>
    {
        public ProductMovement()
        {
        }

        [Key]
        public Guid? ProductMovementId { get; set; }
    }
}