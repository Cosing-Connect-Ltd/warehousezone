using System.Collections.Generic;

namespace Ganedata.Core.Entities.Domain.ImportModels
{
    public class SortedProductCategoriesAssociationsImportModel
    {

        public string CategoryName { get; set; }
        public List<string> AssociatedSkuCodes { get; set; }
        public List<SortedProductCategoriesAssociationsImportModel> Childs { get; set; }
    }
}