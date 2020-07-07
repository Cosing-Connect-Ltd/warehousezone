using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class WebsiteShippingRules : PersistableEntity<int>
    {
        public int Id { get; set; }
        [Display(Name ="Site")]
        public int SiteID { get; set; }
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        [Display(Name = "Courier")]
        public string Courier { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        [Display(Name = "Postal Area")]
        public string PostalArea { get; set; }
        [Display(Name = "Weight in Grams")]
        public int WeightinGrams { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("CountryId")]
        public virtual GlobalCountry GlobalCountry { get; set; }
    }
}