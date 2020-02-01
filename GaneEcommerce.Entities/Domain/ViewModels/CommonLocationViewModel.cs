using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class CommonLocationViewModel
    {
        public string LocationCode { get; set; }
        public string Decription { get; set; }
        public int LocationId { get; set; }

        public string BatchNumber { get; set; }
        public decimal Quantity { get; set; }
    }

    public class CommonProductProcessingViewModel
    {
        public int OrderDetailID { get; set; } 

        public int OrderID { get; set; } 

        public int InventoryTransactionTypeId { get; set; } 

        public int InventoryTransactionTypeID {
            set { InventoryTransactionTypeId = value; }
        } 
        public int ProductID { get; set; } 
        public int Quantity { get; set; } 
        public string DeliveryNo { get; set; } 

        public bool IsCaseQuantity { get; set; }
    }

    public class ProductLocationsResponse
    {
        public ProductLocationsResponse()
        {
            ProductDetails = new List<ProductLocationsDetailResponse>();
        }
        public List<ProductLocationsDetailResponse> ProductDetails { get; set; }
        public bool ContainsBatches { get; set; }
        public bool ContainsExpiryDate { get; set; }
        public bool Serialised { get; set; }
    }

    public class ProductLocationsDetailResponse
    {
        public int Id { get; set; }

        public decimal Quantity { get; set; }
        public string LocationCode { get; set; }
        public string Location { get; set; }
        public string Serial { get; set; }
        public string ProductName { get; set; }
        public List<ProductLocationBatchResponse> Batches { get; set; }
        public int ProductId { get; set; }
        public string IsSerializable { get; set; }
    }

    public class ProductLocationBatchResponse
    {
        public int LocationId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryDateString { get; set; }
        public decimal Quantity { get; set; }
    }

}