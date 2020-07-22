using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductDetailAttributeViewModel
    {
        public ProductDetailAttributeViewModel()
        {
            AttributeValues = new List<ProductDetailAttributeValueViewModel>();
        }
        public string Name { get; set; }
        public string SelectedValue { get; set; }
        public List<ProductDetailAttributeValueViewModel> AttributeValues { get; set; }
    }
}