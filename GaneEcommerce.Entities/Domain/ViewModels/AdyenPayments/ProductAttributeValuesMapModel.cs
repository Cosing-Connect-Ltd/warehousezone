namespace Ganedata.Core.Models.AdyenPayments
{
    public class ProductAttributeValuesMapModel
    {
        public int ProductSpecialAttributePriceId { get; set; }
        public int ProductId { get; set; }
        public int PriceGroupID { get; set; }
        public int AttributeId { get; set; }
        public int AttributeValueId { get; set; }
        public int AttributeMapId { get; set; }
        public decimal? AttributeSpecificPrice { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValueName { get; set; }
        public int TenantId { get; set; }
    }
}