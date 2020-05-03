using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class NavigationProductsViewModel
    {

        public int Id { get; set; }
        public int NavigationId { get; set; }
        public int ProductId { get; set; }
        public int SiteID { get; set; }
        public string SKUCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DepartmentName { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductCategoryName { get; set; }
        public bool? IsActive { get; set; }
        public int? SortOrder { get; set; }

    }
}