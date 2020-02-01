using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Table("VehicleInspection")]
    public class VehicleInspection : PersistableEntity<int>
    {
        public VehicleInspection()
        {
            ConfirmedChecklists = new HashSet<VehicleInspectionConfirmedList>();
        }
        [Key]
        public int Id { get; set; }

        public DateTime InspectionDateTime { get; set; }

        public int? VehicleDriverId { get; set; }
        [ForeignKey("VehicleDriverId")]
        public virtual Resources VehicleDriver { get; set; }
        public byte[] DriverSignature { get; set; }

        public int? MarketVehicleId { get; set; }
        [ForeignKey("MarketVehicleId")]
        public virtual MarketVehicle MarketVehicle { get; set; }

        public string FleetNumber { get; set; }

        public int MileageReading { get; set; }

        public string Notes { get; set; }
        public string LoadingComments { get; set; }

        public int? ReportedToUserId { get; set; }
        [ForeignKey("ReportedToUserId")]
        public virtual Resources ReportedToUser { get; set; }
        public string ReportedToName { get; set; }

        public int? RectifiedUserId { get; set; }
        [ForeignKey("RectifiedUserId")]
        public virtual Resources RectifiedUser { get; set; }
        public string RectifiedUserName { get; set; }
        public byte[] RectifiedSignature { get; set; }
        public DateTime? RectifiedDateTime { get; set; }

        public bool? NilDefect { get; set; }

        public virtual ICollection<VehicleInspectionConfirmedList> ConfirmedChecklists { get; set;}
    }

    [Table("VehicleInspectionType")]
    public class VehicleInspectionType : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string TypeName { get; set; }
    }
    [Table("VehicleInspectionCheckList")]
    public class VehicleInspectionCheckList : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int VehicleInspectionTypeId { get; set; }

        [ForeignKey("VehicleInspectionTypeId")]
        public virtual VehicleInspectionType VehicleInspectionType { get; set; }
    }

    [Table("VehicleInspectionConfirmedList")]
    public class VehicleInspectionConfirmedList 
    {
        [Key]
        public int Id { get; set; }

        public int VehicleInspectionId { get; set; }
        [ForeignKey("VehicleInspectionId")]
        public virtual VehicleInspection VehicleInspection { get; set; }

        public int VehicleInspectionCheckListId { get; set; }
        [ForeignKey("VehicleInspectionCheckListId")]
        public virtual VehicleInspectionCheckList VehicleInspectionCheckList { get; set; }
    }
}