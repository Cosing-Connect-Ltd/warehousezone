using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Entities.Domain
{

    public class MarketListViewModel : ListViewModel
    {
        public MarketListViewModel()
        {
            Markets = new List<MarketViewModel>();
        }
        public List<MarketViewModel> Markets { get; set; }
    }

    public class MarketViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Town { get; set; }
        public int? ExternalId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int TenantId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool AllowDelete { get; set; }
    }


    public class MarketRouteListViewModel : ListViewModel
    {
        public MarketRouteListViewModel()
        {
            MarketRouteViewModel = new List<MarketRouteViewModel>();
        }
        public List<MarketRouteViewModel> MarketRouteViewModel { get; set; }

    }

    public class MarketRouteViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Duration (Minutes)")]
        public int? RouteDurationMins { get; set; }
        public virtual ICollection<MarketRouteMap> MarketRouteMap { get; set; }
        public int TenantId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool AllowDelete { get; set; }
    }

    public class MarketVehicleListViewModel : ListViewModel
    {
        public MarketVehicleListViewModel()
        {
            MarketVehicles = new List<MarketVehicleViewModel>();
        }
        public List<MarketVehicleViewModel> MarketVehicles { get; set; }

    }

    public class MarketVehicleViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Vehicle Registration")]
        [Required(ErrorMessage = "Vehicle Registration is mandatory.")]
        public string VehicleIdentifier { get; set; }
        public int? MarketId { get; set; }
        public int TenantId { get; set; }
        public string MarketName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class MarketCustomersListViewModel : ListViewModel
    {
        public MarketCustomersListViewModel()
        {
            MarketCustomers = new List<MarketCustomersViewModel>();
        }
        public List<MarketCustomersViewModel> MarketCustomers { get; set; }
        public string MarketName { get; set; }
        public int MarketId { get; set; }
    }
    public class MarketCustomersViewModel
    {
        public MarketCustomersViewModel()
        {
            AvailableCustomers = new List<CustomerAccountViewModel>();
            SelectedCustomers = new List<CustomerAccountViewModel>();
            MarketCustomerAccounts = new List<SelectedAccountViewModel>();
        }

        public List<SelectedAccountViewModel> MarketCustomerAccounts { get; set; }
        public List<CustomerAccountViewModel> AvailableCustomers { get; set; }
        public List<CustomerAccountViewModel> SelectedCustomers { get; set; }
        public string MarketCustomerEntries { get; set; }

        public int MarketId { get; set; }
    }

    public class SelectedAccountViewModel
    {
        public int AccountId { get; set; }
        public int SortOrder { get; set; }
        public int MarketId { get; set; }
        public bool IsSkippable { get; set; }
        public DateTime? SkipFromDate { get; set; }
        public DateTime? SkipToDate { get; set; }
        public MarketCustomerVisitFrequency VisitFrequency { get; set; }
    }


    public class CustomerAccountViewModel
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountAddressLine1 { get; set; }
        public string AccountAddressPostCode { get; set; }
        public bool IsSelected { get; set; }
        public int SortOrder { get; set; }
        public bool IsSkippable { get; set; }
        public DateTime? SkipFromDate { get; set; }
        public DateTime? SkipToDate { get; set; }
        public MarketCustomerVisitFrequency VisitFrequency { get; set; }
    }

    public class TenantLocationToRouteAssociateViewModel
    {
        public TenantLocationToRouteAssociateViewModel()
        {
            MarketRoutes = new List<MarketCustomersViewModel>();
        }
        public int TenantLocationId { get; set; }
        public string WarehouseName { get; set; }
        public int MarketId { get; set; }
        public int MarketRouteId { get; set; }
        public List<MarketCustomersViewModel> MarketRoutes { get; set; }
        public List<MarketViewModel> Markets { get; set; }
        public bool IsMobileLocation { get; set; }
    }

    public class MarketAssocationViewModel
    {
        public int AssociationId { get; set; }
        public int MarketId { get; set; }
        public int MarketRouteId { get; set; }
        public int MarketVehicleId { get; set; }
        public string MarketName { get; set; }
        public string RouteName { get; set; }
        public string VehicleName { get; set; }
        public string DisplayText
        {
            get { return MarketName + ">" + RouteName + ">" + VehicleName; }
        }

    }

    public class MarketJobViewModel
    {
        public MarketJobViewModel()
        {
            AllCustomerAccounts = new List<SelectListItem>();
        }

        public int Id { get; set; }
        [Required]
        [DisplayName("Job Title")]
        public string Name { get; set; }
        [DisplayName("Job Description")]
        public string Description { get; set; }
        public int? TenantId { get; set; }
        public string DisplayText { get; set; }
        [DisplayName("Customer")]
        public int? AccountID { get; set; }
        public List<SelectListItem> AllCustomerAccounts { get; set; }
        [DisplayName("Resource")]
        public int? ResourceID { get; set; }
        public List<SelectListItem> AllResources { get; set; }
        [DisplayName("Market Route")]
        public int? MarketRouteId { get; set; }
        public int? MarketJobStatusId { get; set; }
        public MarketJobStatusEnum MarketJobStatusEnum { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateCancelled { get; set; }
        public string CancelledReason { get; set; }
        public DateTime? DateDeclined { get; set; }
        public string Reason { get; set; }
        public string DeviceIdentifier { get; set; }
        public string DeviceUsername { get; set; }
        public string MarketName { get; set; }
        public string ResourceName { get; set; }
    }
    public class MarketJobsListViewModel : ListViewModel
    {
        public MarketJobsListViewModel()
        {
            AllMarketJobs = new List<MarketJobViewModel>();
        }
        public List<MarketJobViewModel> AllMarketJobs { get; set; }

        public List<MarketJobViewModel> UnAllocatedMarketJobs { get { return AllMarketJobs.Where(m => m.MarketJobStatusEnum == MarketJobStatusEnum.UnAllocated).ToList(); } }
        public List<MarketJobViewModel> AllocatedMarketJobs { get { return AllMarketJobs.Where(m => m.MarketJobStatusEnum == MarketJobStatusEnum.Allocated).ToList(); } }
        public List<MarketJobViewModel> AcceptedMarketJobs { get { return AllMarketJobs.Where(m => m.MarketJobStatusEnum == MarketJobStatusEnum.Accepted).ToList(); } }
        public List<MarketJobViewModel> DeclinedMarketJobs { get { return AllMarketJobs.Where(m => m.MarketJobStatusEnum == MarketJobStatusEnum.Declined).ToList(); } }
        public List<MarketJobViewModel> CancelledMarketJobs { get { return AllMarketJobs.Where(m => m.MarketJobStatusEnum == MarketJobStatusEnum.Cancelled).ToList(); } }
        public List<MarketJobViewModel> CompletedMarketJobs { get { return AllMarketJobs.Where(m => m.MarketJobStatusEnum == MarketJobStatusEnum.Completed).ToList(); } }

    }

    public class MarketJobAllocationModel
    {
        public int MarketJobId { get; set; }
        public int ResourceId { get; set; }
        public MarketJobStatusEnum LatestJobStatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public DateTime? ActionDate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class MarketProductLevelViewModel
    {
        public int ProductMarketStockLevelID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int MarketId { get; set; }
        public decimal ReOrderQuantity { get; set; }
        public decimal MinStockQuantity { get; set; }
        public string SKUCode { get; set; }
    }


    public class RouteMarketsViewModel
    {
        public RouteMarketsViewModel()
        {
            AvailableMarkets = new List<MarketRouteAccountViewModel>();
            SelectedMarkets = new List<MarketRouteAccountViewModel>();
            MarketRouteAccounts = new List<SelectedMarketViewModel>();
        }

        public List<SelectedMarketViewModel> MarketRouteAccounts { get; set; }
        public List<MarketRouteAccountViewModel> AvailableMarkets { get; set; }
        public List<MarketRouteAccountViewModel> SelectedMarkets { get; set; }
        public string RouteMarketsEntries { get; set; }
        public int RouteId { get; set; }
    }

    public class SelectedMarketViewModel
    {
        public int MarketId { get; set; }
        public string MarketName { get; set; }
        public bool IsSelected { get; set; }
        public int SortOrder { get; set; }
        public int RouteId { get; set; }
    }




    public class MarketRouteAccountViewModel
    {
        public int MarketId { get; set; }
        public string MarketName { get; set; }
        public bool IsSelected { get; set; }
        public int SortOrder { get; set; }
    }
    public class TenantPriceGroupViewModel
    {
        public int PriceGroupID { get; set; }
        public string Name { get; set; }
        public decimal Percent { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }

    public class SpecialPriceGroupViewModel
    {
        public int ProductId { get; set; }
        public int PriceGroupId { get; set; }
        public decimal SpecialPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}