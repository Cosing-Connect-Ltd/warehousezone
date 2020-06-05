namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class WebsiteWarehousesViewModel
    {

        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public int SiteID { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehouseCity { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int? SortOrder { get; set; }

    }
}