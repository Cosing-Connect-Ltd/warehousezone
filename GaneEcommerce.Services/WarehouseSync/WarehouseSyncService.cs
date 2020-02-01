using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;

namespace Ganedata.Core.Services
{
    public class WarehouseSyncService : IWarehouseSyncService
    {
        private readonly IApplicationContext _context;

        public WarehouseSyncService(IApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<PTenant>> UpdateTenantInformationForSite(int siteId, List<PTenant> tenantsToImport)
        {
            var existingTenants = _context.PTenants.Where(m => m.SiteId == siteId).ToList();

            var processedTenants = new List<PTenant>();
            try
            {
                int counter = 0;
                foreach (var tenant in tenantsToImport)
                {
                    //If the tenant exist already just update the information, else just add the new tenant information
                    var existingTenant = existingTenants.FirstOrDefault(m => m.SiteId == siteId && m.TenantCode == tenant.TenantCode);
                    if (existingTenant != null)
                    {
                        //Check whether all information are up to date, if not update the information
                        if (tenant.TenantFullName != existingTenant.TenantFullName ||
                            tenant.TenantSalutation != existingTenant.TenantSalutation ||
                            tenant.AddressLine1 != existingTenant.AddressLine1 ||
                            tenant.AddressLine2 != existingTenant.AddressLine2 ||
                            tenant.AddressLine3 != existingTenant.AddressLine3 ||
                            tenant.AddressLine4 != existingTenant.AddressLine4 ||
                            tenant.AddressPostcode != existingTenant.AddressPostcode
                            || tenant.Email != existingTenant.Email ||
                            tenant.MobileNumber != existingTenant.MobileNumber ||
                            tenant.HomeTelephone != existingTenant.HomeTelephone ||
                            tenant.WorkTelephone1 != existingTenant.WorkTelephone1 ||
                            tenant.WorkTelephone2 != existingTenant.WorkTelephone2 ||
                            tenant.WorkTelephoneFax != existingTenant.WorkTelephoneFax
                            || tenant.TenancyAdded != existingTenant.TenancyAdded ||
                            tenant.TenancyCategory != existingTenant.TenancyCategory ||
                            tenant.TenancyPeriodMonths != existingTenant.TenancyPeriodMonths ||
                            tenant.TenancyRenewDate != existingTenant.TenancyRenewDate ||
                            tenant.TenancyVacateDate != existingTenant.TenancyVacateDate ||
                            tenant.TenancyStarted != existingTenant.TenancyStarted ||
                            tenant.TenancyStatus != existingTenant.TenancyStatus ||
                            tenant.IsHeadTenant != existingTenant.IsHeadTenant ||
                            tenant.TenantCode != existingTenant.TenantCode || tenant.CurrentPropertyCode != existingTenant.CurrentPropertyCode)
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
                            existingTenant.IsHeadTenant = tenant.IsHeadTenant;

                            existingTenant.TenancyAdded = tenant.TenancyAdded;
                            existingTenant.TenancyCategory = tenant.TenancyCategory;
                            existingTenant.TenancyPeriodMonths = tenant.TenancyPeriodMonths;
                            existingTenant.TenancyRenewDate = tenant.TenancyRenewDate;
                            existingTenant.TenancyVacateDate = tenant.TenancyVacateDate;
                            existingTenant.TenancyStarted = tenant.TenancyStarted;
                            existingTenant.TenancyStatus = tenant.TenancyStatus;
                            existingTenant.TenantYCode = tenant.TenantYCode;
                            existingTenant.CurrentPropertyCode = tenant.CurrentPropertyCode;
                            existingTenant.CurrentProperty = _context.PProperties.FirstOrDefault(m => m.PropertyCode == tenant.CurrentPropertyCode);
                            existingTenant.SyncRequiredFlag = true;
                            existingTenant.IsCurrentTenant = tenant.TenancyStatus != "ARCHIVED" && tenant.TenancyStatus != "ARCHIVE" && tenant.TenancyStatus != "VACATED" && tenant.TenancyStarted > DateTime.UtcNow.AddYears(-2)
                                && tenant.TenancyStarted <= DateTime.UtcNow && tenant.TenancyVacateDate > DateTime.UtcNow;
                            existingTenant.IsFutureTenant = tenant.TenancyStatus != "ARCHIVED" && tenant.TenancyStatus != "ARCHIVE" && tenant.TenancyStatus != "VACATED" && tenant.TenancyStarted > DateTime.UtcNow
                                && tenant.TenancyVacateDate >= DateTime.UtcNow;
                            _context.Entry(existingTenant).State = EntityState.Modified;
                        }

                        processedTenants.Add(existingTenant);
                    }
                    else
                    {
                        //code to add tenant information when the tenant don't exist
                        tenant.SyncRequiredFlag = true;
                        tenant.DateCreated = DateTime.UtcNow;
                        tenant.DateUpdated = DateTime.UtcNow;
                        tenant.CreatedUserId = 1;
                        tenant.CurrentProperty = _context.PProperties.FirstOrDefault(m => m.PropertyCode == tenant.CurrentPropertyCode);
                        tenant.IsCurrentTenant = tenant.TenancyStatus != "ARCHIVED" && tenant.TenancyStatus != "ARCHIVE" && tenant.TenancyStatus != "VACATED" && tenant.TenancyStarted > DateTime.UtcNow.AddYears(-2)
                            && tenant.TenancyStarted <= DateTime.UtcNow && tenant.TenancyVacateDate > DateTime.UtcNow;
                        tenant.IsFutureTenant = tenant.TenancyStatus != "ARCHIVED" && tenant.TenancyStatus != "ARCHIVE" && tenant.TenancyStatus != "VACATED" && tenant.TenancyStarted > DateTime.UtcNow;

                        _context.Entry(tenant).State = EntityState.Added;
                        processedTenants.Add(tenant);
                    }

                    counter++;

                    if (counter == 500)
                    {
                        await _context.SaveChangesAsync();
                        counter = 0;
                    }

                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return processedTenants;
        }

        public async Task<List<PLandlord>> UpdateLandlordInformationForSite(int siteId, List<PLandlord> landlordsToImport)
        {
            var existingLandlords = _context.PLandlords.Where(m => m.SiteId == siteId).ToList(); ;

            var processedLandlords = new List<PLandlord>();

            foreach (var landlord in landlordsToImport)
            {
                //If the item exist already just update the information, else just add the new information
                var existingLandlord = existingLandlords.FirstOrDefault(m => m.SiteId == siteId && m.LandlordCode == landlord.LandlordCode);
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

                        existingLandlord.LandlordAdded = landlord.LandlordAdded;
                        existingLandlord.LandlordStatus = landlord.LandlordStatus;

                        existingLandlord.DateUpdated = DateTime.UtcNow;

                        existingLandlord.SyncRequiredFlag = true;
                        _context.Entry(existingLandlord).State = EntityState.Modified;
                        processedLandlords.Add(existingLandlord);
                    }
                }
                else
                {
                    //code to add information when the item don't exist
                    landlord.SyncRequiredFlag = true;
                    landlord.LandlordFullname = landlord.LandlordFullname ?? landlord.LandlordCode;
                    landlord.DateCreated = DateTime.UtcNow;
                    landlord.CreatedUserId = 1;
                    landlord.DateUpdated = DateTime.UtcNow;
                    _context.Entry(landlord).State = EntityState.Added;
                    processedLandlords.Add(landlord);
                }
            }
            await _context.SaveChangesAsync();
            return processedLandlords;
        }

        public async Task<List<PProperty>> UpdatePropertiesInformationForSite(int siteId, List<PProperty> propertiesToImport)
        {
            //New requirement to ignore RentInc
            if (siteId == GaneStaticAppExtensions.WarehouseSyncIgnorePropertiesSiteId) return new List<PProperty>();

            var existingProperties = _context.PProperties.Where(m => m.SiteId == siteId).ToList();

            var processedProperties = new List<PProperty>();

            foreach (var property in propertiesToImport)
            {
                //If the item exist already just update the information, else just add the new information
                var existingProperty = existingProperties.FirstOrDefault(m => m.SiteId == siteId && m.PropertyCode == property.PropertyCode);
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
                        existingProperty.AddressPostcode = property.AddressPostcode ?? "";
                        existingProperty.PropertyStatus = property.PropertyStatus;
                        var propertyStatus = Regex.Replace(property.PropertyStatus, "[^0-9a-zA-Z]+", "").ToLower();

                        var landlordManaged = !string.IsNullOrEmpty(propertyStatus) && (((propertyStatus.Equals("let-only", StringComparison.CurrentCultureIgnoreCase) || propertyStatus.Equals("letonly", StringComparison.CurrentCultureIgnoreCase)) || propertyStatus.Equals("managed", StringComparison.CurrentCultureIgnoreCase)));

                        existingProperty.IsLandlordManaged = landlordManaged;
                        existingProperty.IsVacant = property.IsVacant;
                        existingProperty.DateAvailable = property.DateAvailable;
                        existingProperty.DateAdded = property.DateAdded;
                        existingProperty.PropertyBranch = property.PropertyBranch;
                        existingProperty.TenancyMonths = property.TenancyMonths;
                        existingProperty.CurrentLandlordCode = property.CurrentLandlordCode;
                        existingProperty.PropertyLandlord = _context.PLandlords.FirstOrDefault(m => m.LandlordCode.Equals(property.CurrentLandlordCode, StringComparison.CurrentCultureIgnoreCase));
                        existingProperty.SyncRequiredFlag = true;
                        existingProperty.DateUpdated = DateTime.UtcNow;
                        existingProperty.AddressPostcode = string.IsNullOrEmpty(property.AddressPostcode) ? "-" : property.AddressPostcode;
                        _context.Entry(existingProperty).State = EntityState.Modified;
                        processedProperties.Add(existingProperty);
                    }
                }
                else
                {
                    property.PropertyLandlord = _context.PLandlords.FirstOrDefault(m => m.LandlordCode.Equals(property.CurrentLandlordCode, StringComparison.CurrentCultureIgnoreCase));
                    property.AddressPostcode = string.IsNullOrEmpty(property.AddressPostcode) ? "-" : property.AddressPostcode;
                    //code to add information when the item don't exist
                    property.SyncRequiredFlag = true;
                    property.CreatedUserId = 1;
                    property.DateCreated = DateTime.UtcNow;
                    property.DateUpdated = DateTime.UtcNow;
                    _context.Entry(property).State = EntityState.Added;
                    processedProperties.Add(property);
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return processedProperties;
        }

        public async Task UpdateTenantCurrentProperties()
        {
            var existingTenants = _context.PTenants.ToList();

            var processedTenants = new List<PTenant>();
            try
            {
                foreach (var tenant in existingTenants)
                {
                    tenant.IsCurrentTenant = tenant.TenancyStatus != "ARCHIVED" && tenant.TenancyStatus != "VACATED" && tenant.TenancyStarted > DateTime.UtcNow.AddYears(-2) && tenant.TenancyVacateDate >= DateTime.UtcNow;
                    _context.Entry(tenant).State = EntityState.Modified;
                    processedTenants.Add(tenant);

                    tenant.IsFutureTenant = tenant.TenancyStarted > DateTime.UtcNow.AddDays(1) && tenant.TenancyVacateDate > DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message + processedTenants.Count + " processed";
            }

        }
    }
}