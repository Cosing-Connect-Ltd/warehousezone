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
        public int? groupId { get; set; }
        public int? departmentId { get; set; }

        public int? CurrentSort { get; set; }

        public SelectList pageList { get; set; }

        public string searchString { get; set; }

        public string CurrencySymbol { get; set; }

        public string CurrentFilter { get; set; }


        public Dictionary<string, int> ProductCounted { get; set; }

    }
}