using System;
using System.Collections.Generic;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductDynamicFilteringViewModel
    {
        public List<string> Manufacturer { get; set; }
        public string MinAvailablePrice { get; set; }
        public string MaxAvailablePrice { get; set; }
        public Dictionary<ProductAttributes,List<ProductAttributeValues>> AttributeValues { get; set; }
        public List<string> SubCategories  { get; set; }
        public List<WebsiteNavigation> WebsiteNavigationCategories { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
    }
}