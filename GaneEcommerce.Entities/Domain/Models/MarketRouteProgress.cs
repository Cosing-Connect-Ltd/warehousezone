using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class MarketRouteProgress : PersistableEntity<Guid>
    {
        [Key]
        public Guid RouteProgressId { get; set; }

        public int MarketId { get; set; }
        [ForeignKey("MarketId")]
        public virtual Market Market { get; set; }

        public int MarketRouteId { get; set; }
        [ForeignKey("MarketRouteId")]
        public virtual MarketRoute MarketRoute { get; set; }

        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public string Comment { get; set; }

        public bool? SaleMade { get; set; }

        public DateTime? Timestamp { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }


}