using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class BaseProductViewModel
    {
        public ProductMaster Product { get; set; }
        public ProductDynamicFilteringViewModel DynamicFilters { get; set; }
        public ProductPriceViewModel Prices { get; set; }
        public List<ProductMaster> RelatedProducts { get; set; }
        public List<ChildProductsViewModel> GroupedTabs { get; set; }
        public decimal? AvailableProductCount { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string SubCategory { get; set; }
        public int SubCategoryId { get; set; }
        public decimal? Quantity { get; set; }
    }
}