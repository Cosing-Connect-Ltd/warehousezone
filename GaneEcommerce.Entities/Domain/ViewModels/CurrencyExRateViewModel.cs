using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class CurrencyExRateViewModel
    {
        public int ExchangeRateId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal? ExchangeRate { get; set; }
        public DateTime? DateUpdated { get; set; }

    }
}