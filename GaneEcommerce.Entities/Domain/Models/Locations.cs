using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public partial class Locations : PersistableEntity<int>
    {
        public Locations()
        {
            ProductLocations = new HashSet<ProductLocations>();
        }

        [Key]
        [Display(Name = "Location Id")]
        public int LocationId { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        [Display(Name = "Location Group")]
        public int? LocationGroupId { get; set; }
        [Display(Name = "Location Type")]
        public int? LocationTypeId { get; set; }
        [Required(ErrorMessage = "Location Name is required")]
        [Display(Name = "Name")]
        public string LocationName { get; set; }

        [Required(ErrorMessage = "Location Code is required")]
        [StringLength(50)]
        [Display(Name = "Location Code")]
        public string LocationCode { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Weight")]
        public double? LocationWeight { get; set; }
        [Display(Name = "Height")]
        public double? LocationHeight { get; set; }
        [Display(Name = "Width")]
        public double? LocationWidth { get; set; }
        [Display(Name = "Depth")]
        public double? LocationDepth { get; set; }
        [Display(Name = "UOM")]
        public int UOMId { get; set; }
        [Display(Name = "Level Of Detail")]
        public int? LevelOfDetail { get; set; }
        [Display(Name = "Dimension Group")]
        public int DimensionUOMId { get; set; }
        [Display(Name = "Staging Location")]
        public bool StagingLocation { get; set; }
        [Display(Name = "Mix Container")]
        public bool MixContainer { get; set; }
        [Display(Name = "Allow Put Away")]
        public bool AllowPutAway { get; set; }
        [Display(Name = "Allow Pick")]
        public bool AllowPick { get; set; }
        [Display(Name = "Allow Replenish")]
        public bool AllowReplenish { get; set; }
        [Display(Name = "Put Away Sequence")]
        public int? PutAwaySeq { get; set; }
        [Display(Name = "Pick Sequence")]
        public int? PickSeq { get; set; }
        [Display(Name = "Replenish Sequence")]
        public int? ReplenishSeq { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public virtual GlobalUOM GlobalUOM { get; set; }
        [ForeignKey("LocationGroupId")]
        public virtual LocationGroup LocationGroup { get; set; }
        public virtual TenantLocations TenantWarehouses { get; set; }
        public virtual ICollection<ProductLocations> ProductLocations { get; set; }
        public virtual LocationTypes LocationType { get; set; }
        public string LocationWithCode
        {
            get { return LocationName + " - " + LocationCode; }
        }
    }
}
