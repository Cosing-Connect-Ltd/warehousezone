using Ganedata.Core.Entities.Enums;
using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductDetailViewModel: BaseProductViewModel
    {
        public List<ProductDetailAttributeViewModel> AvailableAttributes { get; set; }
        public int ParentProductId { get; set; }
        public string ParentProductSKUCode { get; set; }
        public string ParentProductName { get; set; }
        public ProductKitTypeEnum ParentProductType { get; set; }
    }
}