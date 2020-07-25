using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductDetailViewModel
    {
        public ProductMaster SelectedProduct { get; set; }
        public List<ProductDetailAttributeViewModel> AvailableAttributes { get; set; }
        public List<ProductMaster> RelatedProducts { get; set; }
        public int BaseProductId { get; set; }
        public string BaseProductSKUCode { get; set; }
        public string BaseProductName { get; set; }
        public ProductKitTypeEnum BaseProductType { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public decimal? Quantity { get; set; }
    }
}