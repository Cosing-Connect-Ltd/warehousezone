﻿using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using PagedList;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductListViewModel
    {
        public IPagedList<ProductMaster> Products { get; set; }
        public ProductDynamicFilteringViewModel DynamicFilters { get; set; }
        public string CurrentCategoryName { get; set; }
        public int? CurrentCategoryId { get; set; }
        public SortProductTypeEnum CurrentSort { get; set; }
        public string CurrentSearch { get; set; }
        public string CurrentFilters { get; set; }
        public WebsiteNavigation Category { get; set; }
        public WebsiteNavigation SubCategory { get; set; }
    }
}