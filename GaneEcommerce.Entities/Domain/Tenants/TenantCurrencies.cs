using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Ganedata.Core.Entities.Domain
{

    public class TenantCurrencies : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "ID")]
        public int TenantCurrencyID { get; set; }
        [Display(Name = "Currency")]
        public int CurrencyID { get; set; }
        // use following to cover a difference between actual market rate and rate from online API's in percentage.
        [Display(Name = "Difference Factor")]
        public decimal DiffFactor { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("TenantId")] 
        public virtual Tenant Tenant { get; set; }
        [ForeignKey("CurrencyID")]
        public virtual GlobalCurrency GlobalCurrency { get; set; }
        public virtual ICollection<TenantCurrenciesExRates> TenantCurrenciesExRates { get; set; }
    }
}
