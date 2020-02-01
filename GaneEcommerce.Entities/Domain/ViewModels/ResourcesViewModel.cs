using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Ganedata.Core.Models
{
    public class ResourcesViewModel : PersistableEntity<int>
    {
        public int ResourceId { get; set; }
        [DisplayName("Title")]
        public NameTitlesEnum? PersonTitle { get; set; }
        [DisplayName("First Name"), Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [DisplayName("Last Name"), Required(ErrorMessage = "Last Name is required.")]
        public string SurName { get; set; }
        [DisplayName("Like To Be Knows As")]
        public string LikeToBeKnownAs { get; set; }
        [DisplayName("Colour"), Required(ErrorMessage = "Colour is required.")]
        public string Color { get; set; }
        public bool Married { get; set; }
        public GenderTypeEnum? Gender { get; set; }
        public int? Nationality { get; set; }
        [DisplayName("Hourly Rate")]
        public decimal? HourlyRate { get; set; }


        [DisplayName("Job Start Date")]
        public DateTime? JobStartDate { get; set; }
        public int? ContactNumbersId { get; set; }
        [Display(Name = "Payroll Employee No")]
        public int? PayrollEmployeeNo { get; set; }
        [DefaultValue("True")]
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [DisplayName("Internal Staff")]
        public bool InternalStaff { get; set; }
        public int? AddressId { get; set; }
        [DisplayName("Job Types")]
        public IEnumerable<JobType> JobTypes { get; set; }
        [DisplayName("Locations")]
        public List<int> StoresList { get; set; }
        [DisplayName("Roles")]
        public List<int> RolesList { get; set; }
        [DisplayName("Groups")]
        public List<int> GroupsList { get; set; }
        [DisplayName("Holiday Entitlement")]
        public double? HolidayEntitlement { get; set; }
        [DisplayName("Users")]
        [Remote("IsUserAvailable", "Resources", HttpMethod = "Post", ErrorMessage = "This user is not available, Please select other")]
        public int? AuthUserId { get; set; }


        public string FullName
        {
            get
            {
                return $"{FirstName} {SurName}";
            }
        }

        public string FullAddress()
        {
            return String.Format("{0}{1}{2}", Address.AddressLine1,
                (!String.IsNullOrWhiteSpace(Address.Town) ? $", {Address.Town}" : String.Empty),
                (!String.IsNullOrWhiteSpace(Address.County) ? $", {Address.County}" : String.Empty));
        }

        public ContactNumbersViewModel ContactNumbers { get; set; }
        public AddressViewModel Address { get; set; }
        public ICollection<EmployeeShifts_Stores> EmployeeShifts_Stores { get; set; }
        public ICollection<EmployeeRoles> EmployeeRoles { get; set; }
        public virtual GlobalCountry GlobalCountry { get; set; }
    }
}