using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class SlaWorksOrderListViewModel
    {
        public int JobTypeId { get; set; }

        public string Name { get; set; }
        public List<SlaWorksOrderViewModel> Orders { get; set; }
    }


    public class SlaWorksOrderViewModel

    {
        public int OrderID { get; set; }

        public int TenentId { get; set; }


        public string OrderNumber { get; set; }

        public int? JobTypeId { get; set; }

        public string JobTypeName { get; set; }

        public int? SLAPriorityId { get; set; }

        public string SlaPriorityName { get; set; }

        public string Colour { get; set; }

        public int? PPropertyId { get; set; }

        public string AddressLine1 { get; set; }

        public int OrderStatusID { get; set; }

        public short? ExpectedHours { get; set; }

        public string JobSubTypeName { get; set; }

        public ICollection<OrderNotes> OrderNotes { get; set; }
    }

}