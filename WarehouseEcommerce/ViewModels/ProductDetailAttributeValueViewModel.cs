namespace WarehouseEcommerce.ViewModels
{
    public class ProductDetailAttributeValueViewModel
    {
        public bool IsSelected { get; set; }
        public bool IsAvailableWithCurrentSelection { get; set; }
        public int RelatedProductId { get; set; }
        public bool IsColorTyped { get; set; }
        public string Color { get; set; }
        public string Value { get; set; }
    }
}