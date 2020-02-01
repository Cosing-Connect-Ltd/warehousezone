namespace Ganedata.Core.Entities.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("GlobalTax")]
    public partial class GlobalTax
    {
        public GlobalTax()
        {
            ProductMaster = new HashSet<ProductMaster>();
            OrderDetail = new HashSet<OrderDetail>();
        }
          
        [Key]
        [Display(Name = "Tax Id")]
        public int TaxID { get; set; }
        [Display(Name = "Tax Name")]
        public string TaxName { get; set; }
        [Display(Name = "Tax Detail")]
        public string TaxDescription { get; set; }
        [Required]
        [Display(Name = "Percent Of Amount")]
        public int PercentageOfAmount { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        [ForeignKey("CountryID")]
        public virtual GlobalCountry Country { get; set; }
        public virtual ICollection<ProductMaster> ProductMaster { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
