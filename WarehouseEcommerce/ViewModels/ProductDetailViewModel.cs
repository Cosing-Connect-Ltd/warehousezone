using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductDetailViewModel
    {
        public List<ProductMaster> productMasterList { get; set; }
        public ProductMaster ProductMaster { get; set; }

        public List<ProductFiles> ProductFilesList { get; set; }
        public ProductFiles ProductFiles { get; set; }

        public string FeaturedText { get; set; }
    }
}