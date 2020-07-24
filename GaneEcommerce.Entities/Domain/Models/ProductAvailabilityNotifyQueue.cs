using System;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public partial class ProductAvailabilityNotifyQueue
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual ProductMaster Product { get; set; }
        public int TenantId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
