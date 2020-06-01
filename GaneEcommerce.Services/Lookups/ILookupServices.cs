using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface ILookupServices
    {
        IEnumerable<GlobalUOM> GetAllValidGlobalUoms(EnumUomType uomType= EnumUomType.All);
        IEnumerable<GlobalTax> GetAllValidGlobalTaxes(int? countryId = null);
        IEnumerable<TenantLoanTypes> GetAllValidTenantLoanTypes(int tenantId);
        IEnumerable<TenantDepartments> GetAllValidTenantDepartments(int tenantId);
        IEnumerable<GlobalWeightGroups> GetAllValidGlobalWeightGroups();
        IEnumerable<SLAPriorit> GetAllValidSlaPriorities(int tenantId);

        IEnumerable<LocationTypes> GetAllValidLocationTypes(int tenantId);
        IEnumerable<LocationGroup> GetAllValidLocationGroups(int tenantId);
        IEnumerable<ProductGroups> GetAllValidProductGroups(int tenantId, int numberofproduct=0);
        IEnumerable<PalletType> GetAllValidPalletTypes(int tenantId);
        IEnumerable<ProductManufacturer> GetAllValidProductManufacturer(int tenantId,int? Id=null);
        bool RemoveProductManufacturer(int Id);
        IEnumerable<ReportType> GetAllReportTypes(int tenantId);
        TenantDepartments GetTenantDepartmentById(int departmentId);
        TenantDepartments SaveTenantDepartment(string departmentName,int? accountID, int userId, int tenantId);
        TenantDepartments UpdateTenantDepartment(TenantDepartments tenantDepartments);
        TenantDepartments RemoveTenantDepartment(int departmentId, int currentUserId);
        List<TenantLocations> GetAllWarehousesForTenant(int tenantId, int? excludeWarehouseId = null, bool? excludeMobileLocations = false);
        IEnumerable<GlobalCountry> GetAllGlobalCountries();
        IEnumerable<GlobalCurrency> GetAllGlobalCurrencies();
        IEnumerable<TenantWarranty> GetAllTenantWarrenties(int tenantId);
        IEnumerable<GlobalAccountStatus> GetAllAccountStatuses();
        IEnumerable<TenantPriceGroups> GetAllPriceGroups(int tenantId, int filterById = 0);
        IEnumerable<JobType> GetAllJobTypes(int tenantId);
        IEnumerable<SlaWorksOrderListViewModel> GetAllJobTypesIncludingNavProperties(int tenantId);
        IEnumerable<Locations> GetAllLocations(int tenantId, DateTime? reqDate = null, bool includeIsDeleted = false);
        Locations GetLocationById(int locationId, int tenantId);
        LocationTypes GetLocationTypeById(int locationTypeId);
        void DeleteLocationById(int locationId, int tenantId, int userId);
        Locations GetLocationByName(string locationName, int tenantId);
        Locations GetLocationByCode(string locationCode, int tenantId);
        IEnumerable<ProductMaster> GetProductsByLocationId(int locationId, int tenantId);

        Locations CreateLocation(Locations location, List<int> productIds, int userId, int tenantId, int warehouseId);
        Locations BulkCreateProductsLocation(Locations location, List<int> productIds, int? startValue, int? endValue, int tenantId, int warehouseId, int userId);
        Locations BulkEditProductsLocation(Locations location, List<int> productIds , int tenantId, int warehouseId, int userId);

        LocationTypes GetLocationTypeByName(string locationTypeName, int tenantId, int? excludeLocationTypeId=null);
        JobType CreateJobType(JobType jobtype, int tenantId, int userId, List<int> resourceIds);
        JobType GetJobTypeById(int jobTypeId, int tenantId);
        JobType SaveJobType(JobType jobtype, int tenantId, int userId, List<int> resourceIds);
        void DeleteJobType(int jobtypeId, int tenantId);

        IEnumerable<AuthActivity> GetAllActiveActivities();
        IEnumerable<AuthActivityGroup> GetAllActiveActivityGroups();

        bool IsLocationCodeAvailable(string locationcode, int locationId, int tenantId);

        LocationTypes CreateLocationType(string locationTypeName, string locationDescription, int tenantId, int userId);

        LocationGroup CreateLocationGroup(string locationGroupName, int tenantId, int userId);
        LocationGroup GetLocationGroupById(int locationGroupId);

        LocationGroup GetLocationGroupByName(string locationGroupName, int tenantId, int? excludeLocGroupTypeId = null);

        List<AccountPaymentMode> GetAllAccountPaymentModes();

        IEnumerable<object> GetProductDepartments(int accountId);

        IEnumerable<object> GetProductGroups(int departmentId);

        IEnumerable<object> GetProductCategory(int groupId);

        IEnumerable<object> GetAllValidWebsites(int currentTenantId);

        IQueryable<TenantEmailNotificationQueue> GetEmailNotifcationQueue(int tenantId);

        bool CheckStockIssue(int ProductId, decimal InStock,bool serialize, bool palletsPrdocut);

       ProductManufacturer SaveAndUpdateProductManufacturer(ProductManufacturer productManufacturer,int UserId);

        IEnumerable<ProductCategory> GetAllValidProductCategories(int tenantId, int numberofproduct = 0, int? ProductGroupId = null);

        IEnumerable<ProductKitType> GetAllValidProductKitType(int tenantId);

        bool UpdateStockMovement(StockMovementCollectionViewModel data);
        Guid CreateStockMovement(int userId, int TenantId,int warehouseId);
    }
}
