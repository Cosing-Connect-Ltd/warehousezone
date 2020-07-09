using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class WebsiteShippingRulesViewModel
    {
        public int Id { get; set; }
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
    }
}