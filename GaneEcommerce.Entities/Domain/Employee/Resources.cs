using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class Resources :PersistableEntity<int>
    {
        public Resources()
        {
            Appointmentses = new HashSet<Appointments>();
            AppointmentResourceShiftses = new HashSet<ResourceShifts>();
            JobTypes = new HashSet<JobType>();
        }

        [Key]
        public int ResourceId { get; set; }
        [Display(Name = "Title")]
        public NameTitlesEnum? PersonTitle { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name(s)")]
        public string MiddleName { get; set; }
        [Display(Name = "Sur Name")]
        public string SurName { get; set; }
        [Display(Name = "Full Name")]
        public string Name
        {
            get
            {
                return $"{FirstName} {SurName}";
            }
        }
        [Display(Name = "Known As")]
        public string LikeToBeKnownAs { get; set; }
        public GenderTypeEnum? Gender { get; set; }
        public bool Married { get; set; }
        public int? Nationality { get; set; }
        public decimal? HourlyRate { get; set; }
        [Display(Name = "Group Colour")]
        public string Color { get; set; }
        [Display(Name = "Job Description")]
        public string JobDescription { get; set; }
        [Display(Name = "Internal Staff")]
        public bool InternalStaff { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public int? AddressId { get; set; }
        public int? ContactNumbersId { get; set; }
        [Display(Name = "Payroll Employee No")]
        public int? PayrollEmployeeNo { get; set; }
        public double? HolidayEntitlement { get; set; }
        public DateTime? JobStartDate { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<Appointments> Appointmentses { get; set; }
        public virtual ICollection<ResourceShifts> AppointmentResourceShiftses { get; set; }
        public virtual ICollection<JobType> JobTypes { get; set; }
        public virtual ICollection<EmployeeShifts_Stores> EmployeeShifts_Stores { get; set; }
        public virtual ICollection<EmployeeRoles> EmployeeRoles { get; set; }
        public virtual ICollection<EmployeeGroups> EmployeeGroups { get; set; }
        public virtual ContactNumbers ContactNumbers { get; set; }
        [ForeignKey("Nationality")]
        public virtual GlobalCountry GlobalCountry { get; set; }

        public int? AuthUserId { get; set; }
        [ForeignKey("AuthUserId")]
        public virtual AuthUser AuthUser { get; set; }

        public bool IsFlexibleWorkingAllowed { get; set; }
        public bool IsOvertimeWorkingAllowed { get; set; }
        public int AttendanceGracePeriodInMinutes { get; set; }
    }
}