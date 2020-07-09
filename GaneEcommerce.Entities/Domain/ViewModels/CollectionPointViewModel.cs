using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ganedata.Core.Models;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class CollectionPointViewModel
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public int CountryID { get; set; }
        public virtual CountryViewModel GlobalCountry { get; set; }
    }
}