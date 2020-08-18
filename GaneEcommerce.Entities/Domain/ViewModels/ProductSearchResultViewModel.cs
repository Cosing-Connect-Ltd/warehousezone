using System.Collections.Generic;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductSearchResultViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DefaultImage { get; set; }
        public string Category { get; set; }
        public string SkuCode { get; set; }
        public string SearchKey { get; set; }
    }
}