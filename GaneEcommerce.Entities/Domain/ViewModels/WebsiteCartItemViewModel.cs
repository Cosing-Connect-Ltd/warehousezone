using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class WebsiteCartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public virtual ProductMasterViewModel ProductMaster { get; set; }
        public List<KitProductCartSession> KitProductCartItems { get; set; }
        public int? CartId { get; set; }
        public bool IsAvailableForCollection { get; set; }
        public bool IsAvailableForDelivery { get; set; }
        public decimal ProductTotalAmount => Math.Round((Quantity * Price), 2);
    }

    [Serializable]
    public class WebsiteCartItemsViewModel
    {
        public List<WebsiteCartItemViewModel> WebsiteCartItems { get; set; }
        public decimal TotalAmount => Math.Round(WebsiteCartItems.Sum(c => c.ProductTotalAmount), 2);
        public bool ShowLoginPopUp { get; set; }
        public bool ShowCartPopUp { get; set; }
        public string CurrenySymbol { get; set; }
        public IEnumerable<AddressViewModel> ShippmentAddresses { get; set; }

    }
}