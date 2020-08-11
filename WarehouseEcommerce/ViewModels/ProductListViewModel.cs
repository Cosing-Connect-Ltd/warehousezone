using Ganedata.Core.Entities.Domain;
using PagedList;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductListViewModel
    {
        public IPagedList<ProductMaster> Products { get; set; }
        public ProductFilteringViewModel ProductFilters { get; set; }
    }
}