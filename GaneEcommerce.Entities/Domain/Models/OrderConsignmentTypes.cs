using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderConsignmentTypes:PersistableEntity<int>
    {
        [Key]
        public int ConsignmentTypeId { get; set; }
        [Display(Name = "Consignment Type")]
        public string ConsignmentType { get; set; }
    }
}