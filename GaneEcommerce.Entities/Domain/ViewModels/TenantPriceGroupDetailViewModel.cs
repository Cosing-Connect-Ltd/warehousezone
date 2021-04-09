using System;
using Ganedata.Core.Models;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductSpecialPriceViewModel
    {
        public int? PriceGroupDetailID { get; set; }
        public int ProductID { get; set; }
        public int? PriceGroupID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? SpecialPrice { get; set; }
        public string PriceGroupName { get; set; }
        public string ProductName { get; set; }
        public DateTime? DateCreated { get; set; }
        public string SkuCode { get; set; }
    }
   

}