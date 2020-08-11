using System;
using System.Collections.Generic;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductFilteringViewModel
    {
        public List<string> Manufacturer { get; set; }
        public Tuple<string,string> PriceInterval { get; set; }
        public Dictionary<ProductAttributes,List<ProductAttributeValues>> AttributeValues { get; set; }
        public List<string> subCategories  { get; set; }
        public List<WebsiteNavigation> WebsiteNavigationCategories { get; set; }
        public int Count { get; set; }
    }
}