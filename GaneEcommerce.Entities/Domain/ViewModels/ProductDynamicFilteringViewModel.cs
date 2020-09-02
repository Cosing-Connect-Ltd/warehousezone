using System;
using System.Collections.Generic;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductDynamicFilteringViewModel
    {
        public Dictionary<int, string> Brands { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public Dictionary<ProductAttributes,List<ProductAttributeValues>> Attributes { get; set; }
        public Dictionary<int, string> Types  { get; set; }
        public List<WebsiteNavigation> Categories { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
    }
}