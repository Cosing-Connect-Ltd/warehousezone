using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class LocationsViewModel : PersistableEntity<int>
    {
        [Display(Name = "Location Id")]
        public int LocationId { get; set; }
        [StringLength(200, ErrorMessage = "Warehouse maximum {1} characters exceeded")]
        [Display(Name = "Location Name"), Required(ErrorMessage = "Name is required.")]
        public string LocationName { get; set; }
        [StringLength(200, ErrorMessage = "Address Line 1 maximum {1} characters exceeded")]
        [Display(Name = "Address Line 1"), Required(ErrorMessage = "Address Line 1 is required.")]
        public string AddressLine1 { get; set; }
        [StringLength(200, ErrorMessage = "Address Line 2 maximum {1} characters exceeded")]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [StringLength(200, ErrorMessage = "Address Line 3 maximum {1} characters exceeded")]
        [Display(Name = "Address Line 3")]
        public string AddressLine3 { get; set; }
        [StringLength(200, ErrorMessage = "County / State maximum {1} characters exceeded")]
        [Display(Name = "County / State")]
        public string CountyState { get; set; }
        [StringLength(50, ErrorMessage = "Postal Code maximum {1} characters exceeded")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string TimeZone { get; set; }
        //public int? AddressId { get; set; }
        public int? ContactNumbersId { get; set; }
        public int StoreType { get; set; }
        public int StoresList { get; set; }
        public List<int> DevicesList { get; set; }
        [DisplayName("Minimum Drivers"), Range(0, int.MaxValue, ErrorMessage = "Minimum Drivers must be an integer.")]
        public int MinimumDrivers { get; set; }
        [DisplayName("Minimum Kitchen Staff"), Range(0, int.MaxValue, ErrorMessage = "Minimum Kitchen Staff must be an integer.")]
        public int MinimumKitchenStaff { get; set; }
        [DisplayName("Minimum General Staff"), Range(0, int.MaxValue, ErrorMessage = "Minimum General Staff must be an integer.")]
        public int MinimumGeneralStaff { get; set; }
        public TenantsViewModel Tenants { get; set; }
        public ContactNumbersViewModel ContactNumbers { get; set; }
        public ICollection<EmployeeShifts_Stores> EmployeeShifts_Stores { get; set; }
        public ICollection<DevicesViewModel> Devices { get; set; }
    }
}