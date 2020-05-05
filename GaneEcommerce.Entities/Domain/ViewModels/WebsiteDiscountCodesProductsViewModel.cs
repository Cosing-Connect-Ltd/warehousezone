using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class WebsiteDiscountCodesProductsViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int DiscountId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}