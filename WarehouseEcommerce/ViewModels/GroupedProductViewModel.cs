using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class GroupedProductViewModel: BaseProductViewModel
    {
        public List<ProductPriceViewModel> ChildProductsPrices { get; set; }
    }
}