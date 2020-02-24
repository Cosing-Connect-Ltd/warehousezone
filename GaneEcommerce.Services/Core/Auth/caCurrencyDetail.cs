using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    [Serializable]
    public class caCurrencyDetail
    {
        public int Id { get; set; }

        public string Symbol { get; set; }

        public string CurrencyName { get; set; }

        public decimal? Rate { get; set; }

       
    }
}