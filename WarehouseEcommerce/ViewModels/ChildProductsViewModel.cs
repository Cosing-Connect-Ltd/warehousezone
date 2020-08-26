using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class ChildProductsViewModel
    {
        public ProductKitType ProductKitType { get; set; }
        public List<ProductMaster> Products { get; set; }
        public Dictionary<int, decimal> ProductsAvailableCounts { get; set; }
        public Dictionary<int, ProductPriceViewModel> Prices { get; set; }
    }
}