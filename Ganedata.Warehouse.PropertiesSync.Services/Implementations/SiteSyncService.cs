using Ganedata.Warehouse.PropertiesSync.Data.Helpers;
using Ganedata.Warehouse.PropertiesSync.Services.HelperModels;
using Ganedata.Warehouse.PropertiesSync.Services.Interfaces;
using Ganedata.Warehouse.PropertiesSync.SyncData;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Ganedata.Warehouse.PropertiesSync.Services.Implementations
{
    public class SiteSyncService : ISiteSyncService
    {
        private readonly ISyncDataDbContext _syncDataDbContext = UnityServicesToBeReplaced.SyncDataDbContext;
        private readonly ITenantsService _pTenantsService = UnityServicesToBeReplaced.PTenantsService;
        private readonly IPLandlordService _pLandlordService = UnityServicesToBeReplaced.PLandlordService;
        private readonly IPPropertyService _pPropertyService = UnityServicesToBeReplaced.PPropertyService;

        public List<PTenant> GetTenantsFromWarehouseSync(bool onlyRequireSync = false)
        {
            return _syncDataDbContext.PTenants.Where(p => (!onlyRequireSync || p.SyncRequiredFlag)).ToList();
        }
        public List<PLandlord> GetLandlordsFromWarehouseSync(bool onlyRequireSync = false)
        {
            return _syncDataDbContext.PLandlords.Where(p => !onlyRequireSync || p.SyncRequiredFlag).ToList();
        }
        public List<PProperty> GetPropertiesFromWarehouseSync(bool onlyRequireSync = false)
        {
            return _syncDataDbContext.PProperties.Where(p => !onlyRequireSync || p.SyncRequiredFlag).ToList();
        }

        public void UpdateTenantAsSynced(int siteId, string tenantCode)
        {
            var tenant = _syncDataDbContext.PTenants.FirstOrDefault(m => m.SiteId == siteId && m.TenantCode == tenantCode);
            if (tenant != null)
            {

                tenant.SyncRequiredFlag = false;
                _syncDataDbContext.Entry(tenant).State = EntityState.Modified;
                _syncDataDbContext.SaveChanges();
            }
        }

        public void UpdateTenantsAsSynced(int siteId, List<PTenant> tenantsToSet)
        {
            foreach (var t in tenantsToSet)
            {
                var tenant = _syncDataDbContext.PTenants.FirstOrDefault(m => m.SiteId == siteId && m.TenantCode == t.TenantCode);
                if (tenant != null)
                {
                    tenant.SyncRequiredFlag = false;
                    _syncDataDbContext.Entry(tenant).State = EntityState.Modified;
                }
            }
            _syncDataDbContext.SaveChanges();

        }


        public void UpdateLandlordAsSynced(int siteId, string landlordCode)
        {
            var landlord = _syncDataDbContext.PLandlords.FirstOrDefault(m => m.SiteId == siteId && m.LandlordCode == landlordCode);
            if (landlord != null)
            {
                landlord.SyncRequiredFlag = false;
                _syncDataDbContext.Entry(landlord).State = EntityState.Modified;
                _syncDataDbContext.SaveChanges();
            }
        }

        public void UpdateLandlordAsSynced(int siteId, List<PLandlord> landlordsToSet)
        {
            foreach (var l in landlordsToSet)
            {
                var landlord = _syncDataDbContext.PLandlords.FirstOrDefault(m => m.SiteId == siteId && m.LandlordCode.Equals(l.LandlordCode, StringComparison.CurrentCultureIgnoreCase));
                if (landlord != null)
                {
                    landlord.SyncRequiredFlag = false;
                    _syncDataDbContext.Entry(landlord).State = EntityState.Modified;
                }
            }
            _syncDataDbContext.SaveChanges();
        }

        public void UpdatePropertyAsSynced(int siteId, string propertyCode)
        {
            var property = _syncDataDbContext.PProperties.FirstOrDefault(m => m.SiteId == siteId && m.PropertyCode.Equals(propertyCode, StringComparison.CurrentCultureIgnoreCase));
            if (property != null)
            {
                property.SyncRequiredFlag = false;
                _syncDataDbContext.Entry(property).State = EntityState.Modified;
                _syncDataDbContext.SaveChanges();
            }
        }

        public void UpdatePropertiesAsSynced(int siteId, List<PProperty> propertiesToSet)
        {
            foreach (var p in propertiesToSet)
            {
                var property = _syncDataDbContext.PProperties.FirstOrDefault(m => m.SiteId == siteId && m.PropertyCode.Equals(p.PropertyCode, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    property.SyncRequiredFlag = false;
                    _syncDataDbContext.Entry(property).State = EntityState.Modified;
                }
            }
            _syncDataDbContext.SaveChanges();
        }

        public void AddSyncHistory(PSyncHistory syncHistory)
        {
            _syncDataDbContext.Entry(syncHistory).State = EntityState.Added;
            _syncDataDbContext.SaveChanges();
        }

        public async Task<List<PTenant>> UpdateTenantInformationForSite(int siteId)
        {
            var tenants = _pTenantsService.GetTenantsForSite(siteId);
            var tenantsOnWarehouseSync = GetTenantsFromWarehouseSync();
            var processedTenants = new List<PTenant>();
            int counter = 1;
            int interval = 500;

            foreach (var tenant in tenants)
            {
                counter++;
                //If the tenant exist already just update the information, else just add the new tenant information
                var existingTenant = tenantsOnWarehouseSync.FirstOrDefault(m => m.SiteId == siteId && m.TenantCode.Equals(tenant.TenantCode));
                if (existingTenant != null)
                {
                    //Check whether all information are up to date, if not update the information
                    if (tenant.TenantFullName != existingTenant.TenantFullName || tenant.TenantSalutation != existingTenant.TenantSalutation || tenant.AddressLine1 != existingTenant.AddressLine1 || tenant.AddressLine2 != existingTenant.AddressLine2 ||
                        tenant.AddressLine3 != existingTenant.AddressLine3 || tenant.AddressLine4 != existingTenant.AddressLine4 || tenant.AddressPostcode != existingTenant.AddressPostcode
                        || tenant.Email != existingTenant.Email || tenant.MobileNumber != existingTenant.MobileNumber || tenant.HomeTelephone != existingTenant.HomeTelephone || tenant.WorkTelephone1 != existingTenant.WorkTelephone1
                        || tenant.WorkTelephone2 != existingTenant.WorkTelephone2 || tenant.WorkTelephoneFax != existingTenant.WorkTelephoneFax || tenant.TenancyAdded != existingTenant.TenancyAdded || tenant.TenancyVacateDate != existingTenant.TenancyVacateDate
                        || tenant.TenancyCategory != existingTenant.TenancyCategory || tenant.IsHeadTenant != existingTenant.IsHeadTenant || tenant.TenancyPeriodMonths != existingTenant.TenancyPeriodMonths || tenant.TenancyRenewDate != existingTenant.TenancyRenewDate
                        || tenant.TenancyStarted != existingTenant.TenancyStarted || tenant.TenancyStatus != existingTenant.TenancyStatus || tenant.TenantCode != existingTenant.TenantCode)
                    {
                        //code to update tenant information
                        existingTenant.TenantFullName = tenant.TenantFullName;
                        existingTenant.TenantSalutation = tenant.TenantSalutation;
                        existingTenant.AddressLine1 = tenant.AddressLine1;
                        existingTenant.AddressLine2 = tenant.AddressLine2;
                        existingTenant.AddressLine3 = tenant.AddressLine3;
                        existingTenant.AddressLine4 = tenant.AddressLine4;
                        existingTenant.AddressPostcode = tenant.AddressPostcode;
                        existingTenant.Email = tenant.Email;
                        existingTenant.MobileNumber = tenant.MobileNumber;
                        existingTenant.HomeTelephone = tenant.HomeTelephone;
                        existingTenant.WorkTelephone1 = tenant.WorkTelephone1;
                        existingTenant.WorkTelephone2 = tenant.WorkTelephone2;
                        existingTenant.WorkTelephoneFax = tenant.WorkTelephoneFax;
                        existingTenant.SiteId = siteId;
                        existingTenant.TenancyAdded = tenant.TenancyAdded;
                        existingTenant.TenancyCategory = tenant.TenancyCategory;
                        existingTenant.TenancyPeriodMonths = tenant.TenancyPeriodMonths;
                        existingTenant.TenancyRenewDate = tenant.TenancyRenewDate;
                        existingTenant.TenancyVacateDate = tenant.TenancyVacateDate;
                        existingTenant.TenancyStarted = tenant.TenancyStarted;
                        existingTenant.TenancyStatus = tenant.TenancyStatus;
                        existingTenant.TenantCode = tenant.TenantCode;
                        existingTenant.CurrentPropertyCode = tenant.CurrentPropertyCode;
                        existingTenant.IsHeadTenant = tenant.IsHeadTenant;
                        existingTenant.SyncRequiredFlag = true;
                        _syncDataDbContext.Entry(existingTenant).State = EntityState.Modified;
                        processedTenants.Add(existingTenant);
                    }
                }
                else
                {
                    //code to add tenant information when the tenant don't exist
                    tenant.SyncRequiredFlag = true;
                    tenant.SiteId = siteId;
                    //tenant.CurrentProperty = _syncDataDbContext.PProperties.FirstOrDefault(m => m.PropertyCode.Equals(tenant.CurrentPropertyCode));
                    //_syncDataDbContext.Entry(tenant).State = EntityState.Added;
                    _syncDataDbContext.PTenants.Add(tenant);
                    processedTenants.Add(tenant);
                }

                if (counter % interval == 0)
                {
                    await _syncDataDbContext.SaveChangesAsync();
                }
            }
            await _syncDataDbContext.SaveChangesAsync();
            return processedTenants;
        }

        public async Task<List<PLandlord>> UpdateLandlordInformationForSite(int siteId)
        {
            var landlords = _pLandlordService.GetLandlordsInfoForSite(siteId);
            var landlordsFromWarehouseSync = GetLandlordsFromWarehouseSync();
            var processedLandlords = new List<PLandlord>();

            foreach (var landlord in landlords)
            {
                //If the item exist already just update the information, else just add the new information
                var existingLandlord = landlordsFromWarehouseSync.FirstOrDefault(m => m.SiteId == siteId && m.LandlordCode.Equals(landlord.LandlordCode, StringComparison.CurrentCultureIgnoreCase));
                if (existingLandlord != null)
                {
                    //Check whether all information are up to date, if not update the information
                    if (landlord.LandlordFullname != existingLandlord.LandlordFullname || landlord.LandlordSalutation != existingLandlord.LandlordSalutation || landlord.AddressLine1 != existingLandlord.AddressLine1 || landlord.AddressLine2 != existingLandlord.AddressLine2 ||
                        landlord.AddressLine3 != existingLandlord.AddressLine3 || landlord.AddressLine4 != existingLandlord.AddressLine4 || landlord.AddressPostcode != existingLandlord.AddressPostcode
                        || landlord.Email != existingLandlord.Email || landlord.MobileNumber != existingLandlord.MobileNumber || landlord.HomeTelephone != existingLandlord.HomeTelephone || landlord.WorkTelephone1 != existingLandlord.WorkTelephone1 || landlord.WorkTelephone2 != existingLandlord.WorkTelephone2 || landlord.WorkTelephoneFax != existingLandlord.WorkTelephoneFax
                        || landlord.LandlordAdded != existingLandlord.LandlordAdded || landlord.LandlordStatus != existingLandlord.LandlordStatus)
                    {
                        //code to update information
                        existingLandlord.LandlordFullname = landlord.LandlordFullname;
                        existingLandlord.LandlordSalutation = landlord.LandlordSalutation;
                        existingLandlord.AddressLine1 = landlord.AddressLine1;
                        existingLandlord.AddressLine2 = landlord.AddressLine2;
                        existingLandlord.AddressLine3 = landlord.AddressLine3;
                        existingLandlord.AddressLine4 = landlord.AddressLine4;
                        existingLandlord.AddressPostcode = landlord.AddressPostcode;
                        existingLandlord.Email = landlord.Email;
                        existingLandlord.MobileNumber = landlord.MobileNumber;
                        existingLandlord.HomeTelephone = landlord.HomeTelephone;
                        existingLandlord.WorkTelephone1 = landlord.WorkTelephone1;
                        existingLandlord.WorkTelephone2 = landlord.WorkTelephone2;
                        existingLandlord.WorkTelephoneFax = landlord.WorkTelephoneFax;
                        existingLandlord.SiteId = siteId;
                        existingLandlord.LandlordAdded = landlord.LandlordAdded;
                        existingLandlord.LandlordStatus = landlord.LandlordStatus;

                        existingLandlord.SyncRequiredFlag = true;
                        _syncDataDbContext.Entry(existingLandlord).State = EntityState.Modified;
                        processedLandlords.Add(existingLandlord);
                    }
                }
                else
                {
                    //code to add information when the item don't exist
                    landlord.SyncRequiredFlag = true;
                    landlord.SiteId = siteId;
                    //_syncDataDbContext.Entry(landlord).State = EntityState.Added;
                    _syncDataDbContext.PLandlords.Add(landlord);
                    processedLandlords.Add(landlord);
                }
            }
            await _syncDataDbContext.SaveChangesAsync();
            return processedLandlords;
        }

        public async Task<List<PProperty>> UpdatePropertiesInformationForSite(int siteId)
        {
            var properties = _pPropertyService.GetPropertiesInfoForSite(siteId);
            var propertiesFromWarehouseSync = GetPropertiesFromWarehouseSync();
            var processedProperties = new List<PProperty>();

            foreach (var property in properties)
            {
                //If the item exist already just update the information, else just add the new information
                var existingProperty = propertiesFromWarehouseSync.FirstOrDefault(m => m.SiteId == siteId && m.PropertyCode == property.PropertyCode);
                if (existingProperty != null)
                {
                    //Check whether all information are up to date, if not update the information
                    if (property.AddressLine1 != existingProperty.AddressLine1 || property.AddressLine2 != existingProperty.AddressLine2 ||
                        property.AddressLine3 != existingProperty.AddressLine3 || property.AddressLine4 != existingProperty.AddressLine4 || property.AddressPostcode != existingProperty.AddressPostcode
                        || property.PropertyStatus != existingProperty.PropertyStatus || property.IsVacant != existingProperty.IsVacant || property.DateAvailable != existingProperty.DateAvailable || property.DateAdded != existingProperty.DateAdded || property.PropertyBranch != existingProperty.PropertyBranch || property.TenancyMonths != existingProperty.TenancyMonths)
                    {
                        //code to update information
                        existingProperty.AddressLine1 = property.AddressLine1;
                        existingProperty.AddressLine2 = property.AddressLine2;
                        existingProperty.AddressLine3 = property.AddressLine3;
                        existingProperty.AddressLine4 = property.AddressLine4;
                        existingProperty.AddressLine5 = property.AddressLine5;
                        existingProperty.AddressPostcode = property.AddressPostcode;
                        existingProperty.PropertyStatus = property.PropertyStatus;
                        existingProperty.IsVacant = property.IsVacant;
                        existingProperty.DateAvailable = property.DateAvailable;
                        existingProperty.DateAdded = property.DateAdded;
                        existingProperty.PropertyBranch = property.PropertyBranch;
                        existingProperty.TenancyMonths = property.TenancyMonths;
                        existingProperty.SiteId = siteId;
                        existingProperty.SyncRequiredFlag = true;
                        _syncDataDbContext.Entry(existingProperty).State = EntityState.Modified;
                        processedProperties.Add(existingProperty);
                    }
                }
                else
                {
                    //code to add information when the item don't exist
                    property.SyncRequiredFlag = true;
                    property.SiteId = siteId;
                    //_syncDataDbContext.Entry(property).State = EntityState.Added;
                    _syncDataDbContext.PProperties.Add(property);
                    processedProperties.Add(property);
                }
            }
            await _syncDataDbContext.SaveChangesAsync();
            return processedProperties;
        }


        public async Task<PropertySyncFinalResponse> ExecuteSyncProcess(int siteId)
        {
            var response = new PropertySyncFinalResponse();

            var landlordsReponse = await UpdateLandlordInformationForSite(siteId);
            SyncLogger.WriteLog("Landlord information has been imported successfully");

            var propertiesResponse = await UpdatePropertiesInformationForSite(siteId);
            SyncLogger.WriteLog("Properties information has been imported successfully");

            var tenantsResponse = await UpdateTenantInformationForSite(siteId);
            SyncLogger.WriteLog("Tenant information has been imported successfully");


            response.SyncedTenants = tenantsResponse;
            response.SyncedLandlords = landlordsReponse;
            response.SyncedProperties = propertiesResponse;

            var itemsToSync = PropertySyncItemsResponse.MapAll(tenantsResponse, SyncEntityTypeEnum.Tenants);
            itemsToSync.AddRange(PropertySyncItemsResponse.MapAll(landlordsReponse, SyncEntityTypeEnum.Landlords));
            itemsToSync.AddRange(PropertySyncItemsResponse.MapAll(propertiesResponse, SyncEntityTypeEnum.Properties));

            response.Responses = itemsToSync;

            SyncLogger.WriteLog("Responses has been mapped correctly");

            return response;
        }
    }
}