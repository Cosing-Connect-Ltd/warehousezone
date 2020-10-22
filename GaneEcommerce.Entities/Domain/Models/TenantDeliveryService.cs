using Ganedata.Core.Entities.Enums;
using System;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class TenantDeliveryService : PersistableEntity<int>
    {
        public int Id { get; set; }
        public string NetworkCode { get; set; }
        public string NetworkDescription { get; set; }
        public DeliveryMethods DeliveryMethod { get; set; }




    }
}