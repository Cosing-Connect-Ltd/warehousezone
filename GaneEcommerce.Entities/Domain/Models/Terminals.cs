using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class Terminals
    {
        public Terminals()
        {
            TerminalsLog = new HashSet<TerminalsLog>();
            TerminalGeoLocation = new HashSet<TerminalGeoLocation>();
            TerminalCommandsQueue = new HashSet<TerminalCommandsQueue>();
            TerminalsTransactionsLog = new HashSet<TerminalsTransactionsLog>();
        }

        [Key]
        [Display(Name = "Terminal Id")]
        public int TerminalId { get; set; }
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string TerminalName { get; set; }
        [MaxLength(50)]
        [Remote("IsSerialAvailable", "Terminals", AdditionalFields = "TerminalId", ErrorMessage = "Serial no already exists.")]
        [Required(ErrorMessage = "Serial No is Required")]
        [Display(Name = "Serial No")]
        public string TermainlSerial { get; set; }
        [Display(Name = "Date Created")]
        public System.DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        [Display(Name = "Vehicle Checks On Start")]
        public bool VehicleChecksAtStart { get; set; }
        [Display(Name = "Post Geo Location")]
        public bool PostGeoLocation { get; set; }
        [Display(Name = "Location")]
        public bool AllowExportDatabase { get; set; }
        public bool ShowCasePrices { get; set; }
        public bool ScanVehicleLicensePlate { get; set; }
        public bool IgnoreWarehouseForOrderPost { get; set; }
        public int WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual TenantLocations TenantWarehous { get; set; }
        public virtual ICollection<TerminalsLog> TerminalsLog { get; set; }
        public virtual ICollection<TerminalGeoLocation> TerminalGeoLocation { get; set; }
        public virtual ICollection<TerminalCommandsQueue> TerminalCommandsQueue { get; set; }
        public virtual ICollection<TerminalsTransactionsLog> TerminalsTransactionsLog { get; set; }


    }
}