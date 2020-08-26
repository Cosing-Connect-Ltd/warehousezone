using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class OurBrandsViewModel
    {
        public List<ProductManufacturer> Manufacturers { get; set; }

        public string OurBrandsText { get; set; }
    }
}