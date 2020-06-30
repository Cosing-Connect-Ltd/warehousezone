using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class KitProductCartItem : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SimpleProductId { get; set; }

        public int KitProductId { get; set; }

        public decimal Quantity { get; set; }

        public int CartId { get; set; }

        [ForeignKey("SimpleProductId")]
        public virtual ProductMaster SimpleProductMaster { get; set; }

        [ForeignKey("KitProductId")]
        public virtual ProductMaster KitProduct { get; set; }

        [ForeignKey("CartId")]
        public virtual WebsiteCartItem WebsiteCartItem { get; set; }
    }
}