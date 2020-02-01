namespace Ganedata.Core.Entities.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GlobalCurrency")]
    [Serializable]
    public partial class GlobalCurrency
    {
        [Key]
        [Display(Name = "Currency Id")]
        public int CurrencyID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Currency Name")]
        public string CurrencyName { get; set; }
        [Display(Name = "Symbol")]
        public string Symbol { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        public virtual GlobalCountry GlobalCountry { get; set; }
    }
}
