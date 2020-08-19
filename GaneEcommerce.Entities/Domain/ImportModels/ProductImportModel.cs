using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Entities.Domain.ImportModels
{
    public class ProductImportModel
    {

        public string SkuCode { get; set; }
        public string ManufacturerPartNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? InventoryLevel { get; set; }
        public decimal? BuyPrice { get; set; }
        public decimal? SellPrice { get; set; }
        public string PreferredSupplier { get; set; }
        public string BarCode { get; set; }
        public string OuterBarCode { get; set; }
        public bool? Serialisable { get; set; }
        public ProductKitTypeEnum? ProductType { get; set; }
    }
}