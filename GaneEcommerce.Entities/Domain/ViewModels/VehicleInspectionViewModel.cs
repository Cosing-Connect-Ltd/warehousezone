using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class VehicleInspectionViewModel
    {
        public VehicleInspectionViewModel()
        {
            CheckList = new List<InspectionCheckListViewModel>();
            AllEmployees = new List<VehicleDriverViewModel>();
            CheckedInspectionIds = new List<int>();
        }

        public int Id { get; set; }

        public DateTime InspectionDateTime { get; set; }

        [Display(Name = "Inspecting Driver")]
        public int? VehicleDriverId { get; set; }

        public string DriverName { get; set; }

        [Display(Name = "Inspecting Vehicle")]
        public int? MarketVehicleId { get; set; }

        public string VehicleName { get; set; }

        public int TenantId { get; set; }

        public string FleetNumber { get; set; }

        public int MileageReading { get; set; }

        public string Notes { get; set; }

        public string LoadingComments { get; set; }

        [Display(Name = "Reported To Staff")]
        public int? ReportedToUserId { get; set; }

        public string ReportedToName { get; set; }
        [Display(Name = "Rectified Staff")]
        public int? RectifiedUserId { get; set; }
        public string RectifiedUserName { get; set; }

        [Display(Name = "Rectified Date")]
        public DateTime? RectifiedDateTime { get; set; }

        public byte[] DriverSignature { get; set; }
        public byte[] RectifiedSignature { get; set; }

        public List<InspectionCheckListViewModel> CheckList { get; set; }
        public List<int> CheckedInspectionIds { get; set; }
        public List<VehicleDriverViewModel> AllEmployees { get; set; }
        public List<MarketVehicleViewModel> AllVehicles { get; set; }
    }

    public class InspectionCheckListViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int VehicleInspectionTypeId { get; set; }

        public string VehicleInspectionTypeName { get; set; }
        public bool? IsDeleted { get; set; }

    }

    public class VehicleDriverViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class InspectionsViewModel
    {
        public List<VehicleInspectionViewModel> AllInspections { get; set; }

        public int RecordsCount => AllInspections.Count;
    }

}