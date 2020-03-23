using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductFilteringViewModel
    {
        public List<string> Manufacturer { get; set; }

        public List<Tuple<string,string>> PriceInterval { get; set; }

        public string  CurrencySymbol { get; set; }

        public Dictionary<string,List<ProductAttributeValues>> AttributeValues { get; set; }

        public List<string> subCategories  { get; set; }

    }
}