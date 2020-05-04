using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteShippingRules : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        public int CountryId { get; set; }
        public string Courier { get; set; }
        public string Region { get; set; }
        public string PostalArea { get; set; }
        public int WeightinGrams { get; set; }
        public decimal Price { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("CountryId")]
        public virtual GlobalCountry GlobalCountry { get; set; }
    }
}