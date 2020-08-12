using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using PagedList;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductListViewModel
    {
        public IPagedList<ProductMaster> Products { get; set; }
        public ProductDynamicFilteringViewModel DynamicFilters { get; set; }
        public string CurrentCategoryName { get; set; }
        public int? CategoryId { get; set; }
        public SortProductTypeEnum CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public WebsiteNavigation Category { get; set; }
        public WebsiteNavigation SubCategory { get; set; }
    }
}