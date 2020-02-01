using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class EmployeeShifts_StoresViewModel
    {
        public int Id { get; set; }
        public int LocationsId { get; set; }
        public int EmployeesId { get; set; }

        public TenantLocations TenantLocations { get; set; }
        public Resources Resources { get; set; }
    }
}