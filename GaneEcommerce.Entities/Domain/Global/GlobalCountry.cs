namespace Ganedata.Core.Entities.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("GlobalCountry")]
    [Serializable]
    public class GlobalCountry
    {
        public GlobalCountry()
        {
            GlobalCurrency = new HashSet<GlobalCurrency>();
        }

        [Key]
        [Display(Name = "Country Id")]
        public int CountryID { get; set; }
        [Required]
        [StringLength(4)]
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }  
        [Required]
        [StringLength(200)]
        [Display(Name = "Country Name")]
        public string CountryName { get; set; }
        public virtual ICollection<GlobalCurrency> GlobalCurrency { get; set; }
    }
}
