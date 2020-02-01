using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    // currency exchange rates for each tenants and currencies
    public class TenantCurrenciesExRates
    {
        [Key]
        [Display(Name = "Id")]
        public int ExchnageRateID { get; set; }
        [Display(Name = "Tenant Currency Id")]
        public int TenantCurrencyID { get; set; }   
        // use following to cover a difference between actual market rate and rate from online API's in percentage. 
        [Display(Name = " Difference Factor")]
        public decimal? DiffFactor { get; set; }
        [Display(Name = " Actual Rate")]
        public decimal ActualRate { get; set; }
        // calculated rate to be used for conversions Actual Rate multiplied by Difference factor.
        [Display(Name = " Rate")]
        public decimal Rate { get; set; }
        [Display(Name = " Date Updated")]
        public System.DateTime? DateUpdated { get; set; }
        [ForeignKey("TenantCurrencyID")]
        public virtual TenantCurrencies TenantCurrencies { get; set; }
    }
}
