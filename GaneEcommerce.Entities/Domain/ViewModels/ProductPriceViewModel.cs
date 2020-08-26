namespace Ganedata.Core.Entities.Domain
{
    public class ProductPriceViewModel
    {
        public int ProductId { get; set; }
        public decimal? FinalSellPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TaxPercentage { get; set; }
    }
}