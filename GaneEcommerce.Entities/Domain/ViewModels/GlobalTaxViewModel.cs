using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class GlobalTaxViewModel
    {
        public int TaxID { get; set; }
        public string TaxName { get; set; }
        public string TaxDescription { get; set; }
        public int PercentageOfAmount { get; set; }
        public int CountryID { get; set; }
    }
}