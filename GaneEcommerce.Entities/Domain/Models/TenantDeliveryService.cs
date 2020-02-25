using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class TenantDeliveryService : PersistableEntity<int>
    {
        public int Id { get; set; }
        public string NetworkCode { get; set; }
        public string NetworkDescription { get; set; }
        public DeliveryMethods DeliveryMethod { get; set; }
       

        

    }
}