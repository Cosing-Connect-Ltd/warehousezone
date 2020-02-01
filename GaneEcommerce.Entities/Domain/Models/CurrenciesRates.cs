using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.Models
{
    public class CurrenciesRates
    {
        public bool success { get; set; }
        public string terms { get; set; }
        public string privacy { get; set; }
        public int timestamp { get; set; }
        public string source { get; set; }
        public Dictionary<string, double> quotes { get; set; }
        public Error error { get; set; }
    }
    public class Error
    {
        public int code { get; set; }
        public string info { get; set; }
    }
}