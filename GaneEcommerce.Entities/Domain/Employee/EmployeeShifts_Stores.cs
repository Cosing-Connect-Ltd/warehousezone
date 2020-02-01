using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class EmployeeShifts_Stores 

    {
        [Key]
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int ResourceId { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual TenantLocations TenantLocations { get; set; }
        public virtual Resources Resources { get; set; }
    }
}