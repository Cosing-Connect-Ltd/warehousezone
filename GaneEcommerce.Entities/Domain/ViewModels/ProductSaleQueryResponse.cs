using System;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductSaleQueryResponse
    {
        public ProductSaleQueryResponse()
        {
            Success = true;
            ShowWarning = false;
            CanProceed = true;
            FailureMessage = string.Empty;
        }
        public bool Success { get; set; }
        public bool ShowWarning { get; set; }
        public bool CanProceed { get; set; }

        public string FailureMessage { get; set; }
        public string StopMessage { get; set; }


        public decimal SellingPrice { get; set; }

        public decimal SellPrice { get; set; }

        public decimal LandingCostWithMargin { get; set; }

        public decimal LandingCost { get; set; }

        public decimal ProfitMargin { get; set; }

        public decimal MinimumThresholdPrice { get; set; }
         
        public int PriceGroupID { get; set; }

        public decimal PriceGroupPercent { get; set; }

        public decimal? MinimumSellPrice { get; set; }

    }

    [Serializable]
    public class RecipeProductItemRequest
    {
        public int ParentProductId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public int KitType { get; set; }

        public int? KitIds { get; set; }

    }
    public class RemoveRecipeItemRequest
    {
        public int Id { get; set; }
        public int RecipeProductId { get; set; }

        public int? KitType { get; set; }
    }

    public class ProductPriceHistoryModel
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Product { get; set; }
        public int TypeIdentifier { get; set; }
        public string Timestamp { get; set; }
        public string CurrencySymbol { get; set; }
        public string PriceWithDate { get; set; }
    }

    public class ProductPriceRequestModel
    {
        public int ProductId { get; set; }
        public int AccountId { get; set; }
    }
     
}