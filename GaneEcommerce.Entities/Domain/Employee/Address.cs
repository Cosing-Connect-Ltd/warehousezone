using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string HouseNumber { get; set; }
        public string PostCode { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public int CountryID { get; set; }
        public virtual GlobalCountry GlobalCountry { get; set; }
    }
}