using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class DevicesViewModel : PersistableEntity<int>
    {
        public int Id { get; set; }
        [DisplayName("Device Serial *"), Required(ErrorMessage = "Device Serial is required.")]
        public string SerialNo { get; set; }
        [DisplayName("Device Name"), Required(ErrorMessage = "Device Name is required.")]
        public string Name { get; set; }
        public int WarehouseId { get; set; }
        public virtual TenantLocations TenantLocations { get; set; }
    }
}