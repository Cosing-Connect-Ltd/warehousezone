using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class TenantDeliveryService : PersistableEntity<int>
    {

        public TenantDeliveryService()
        {
            TenantDeliveryServiceCountryMap = new HashSet<TenantDeliveryServiceCountryMap>();
        }

        public int Id { get; set; }
        public string NetworkCode { get; set; }
        public string NetworkDescription { get; set; }
        public DeliveryMethods DeliveryMethod { get; set; }
        public int? SLAPriorityId { get; set; }
        public virtual ICollection<TenantDeliveryServiceCountryMap> TenantDeliveryServiceCountryMap { get; set; }
        [ForeignKey("SLAPriorityId")]
        public virtual SLAPriorit SLAPriority { get; set; }
    }

    public class TenantDeliveryServiceCountryMap
    {
        public int Id { get; set; }
        public int TenantDeliveryServiceId { get; set; }
        public int CountryId { get; set; }
        public virtual TenantDeliveryService TenantDeliveryService { get; set; }
        [ForeignKey("CountryId")]
        public virtual GlobalCountry GlobalCountry { get; set; }

    }
}