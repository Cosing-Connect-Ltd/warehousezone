using System;
using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using System.Data.Entity;
using System.Threading.Tasks;
using Ganedata.Core.Models;
using AutoMapper;
using Ganedata.Core.Data;

namespace Ganedata.Core.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;

        public PropertyService(IApplicationContext currentDbContext, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _mapper = mapper;
        }

        public IQueryable<PProperty> GetAllValidProperties(int? filterLandlordId = null, bool includeArchived = false)
        {
            return _currentDbContext.PProperties.AsNoTracking().Where(m => (includeArchived || m.PropertyStatus != "ARCHIVED")
            && (!filterLandlordId.HasValue || m.CurrentLandlordId == filterLandlordId) && !string.IsNullOrEmpty(m.AddressLine1) && m.IsDeleted != true).OrderBy(m => m.PropertyCode)
            .Include(x => x.PropertyLandlord);
        }

        public IQueryable<PLandlord> GetAllValidPropertyLandlords()
        {
            return _currentDbContext.PLandlords.Where(u => u.IsDeleted != true);
        }

        public IQueryable<PTenant> GetAllPropertyTenants(int? id)
        {
            return _currentDbContext.PTenants.AsNoTracking().Where(u => u.CurrentPropertyId == id || !id.HasValue);
        }

        public IEnumerable<PTenant> GetAllCurrentTenants(int filterByPropertyId = 0)
        {
            return _currentDbContext.PTenants.AsNoTracking().Where(m => (filterByPropertyId == 0 || filterByPropertyId == m.CurrentPropertyId) && m.IsCurrentTenant).ToList();
        }

        public IEnumerable<PTenant> GetAppointmentRecipientTenants(int filterByOrderId = 0)
        {
            return _currentDbContext.OrderPTenantEmailRecipients.Where(m => m.OrderId == filterByOrderId)
                .Select(m => m.PTenant).ToList();
        }

        public IEnumerable<PTenantViewModelForTenantFlag> GetAllTenantsToUpdateIsCurrentTenant(int filterByPropertyId = 0)
        {
            var results =
             (from a in _currentDbContext.PTenants.AsNoTracking().Where(m => (filterByPropertyId == 0 || filterByPropertyId == m.CurrentPropertyId))
              select new PTenantViewModelForTenantFlag
              {
                  PTenantId = a.PTenantId,
                  TenantCode = a.TenantCode,
                  TenantYCode = a.TenantYCode,
                  TenantFullName = a.TenantFullName,
                  TenantSalutation = a.TenantSalutation,
                  TenancyCategory = a.TenancyCategory,
                  TenancyStatus = a.TenancyStatus,
                  TenancyAdded = a.TenancyAdded,
                  TenancyStarted = a.TenancyStarted,
                  TenancyRenewDate = a.TenancyRenewDate,
                  TenancyVacateDate = a.TenancyVacateDate,
                  IsCurrentTenant = a.IsCurrentTenant,
                  IsFutureTenant = a.IsFutureTenant,
                  SiteId = a.SiteId,
                  SyncRequiredFlag = a.SyncRequiredFlag,
                  CurrentPropertyCode = a.CurrentPropertyCode,
                  CurrentPropertyId = a.CurrentPropertyId,
                  DateCreated = a.DateCreated,
                  CreatedUserId = a.CreatedUserId,
                  DateUpdated = a.DateUpdated,
                  UpdatedUserId = a.UpdatedUserId,
                  IsHeadTenant = a.IsHeadTenant

              }).ToList();

            return results;

        }

        public IEnumerable<PTenant> GetAllEmailRecipientTenantsForOrder(int orderId)
        {
            return _currentDbContext.OrderPTenantEmailRecipients
                .Where(m => m.OrderId == orderId)
                .Select(m => m.PTenant);
        }

        public PTenant GetPropertyTenantById(int pTenantId)
        {
            return _currentDbContext.PTenants.Find(pTenantId);
        }

        public PProperty GetPropertyById(int pPropertyId)
        {
            PProperty pproperty = _currentDbContext.PProperties.Find(pPropertyId); ;
            return pproperty;
        }

        public PLandlord GetPropertyLandlordById(int pPropertyLandlordId)
        {
            return _currentDbContext.PLandlords.Find(pPropertyLandlordId);
        }

        public PProperty SavePProperty(PProperty model, int userId)
        {
            if (model.PPropertyId > 0)
            {
                model.UpdatedUserId = userId;
                model.DateUpdated = DateTime.UtcNow;
            }
            else
            {
                model.CreatedUserId = userId;
                model.DateCreated = DateTime.UtcNow;
                _currentDbContext.PProperties.Add(model);
            }

            _currentDbContext.SaveChanges();

            if (model.CurrentPTenentId > 0)
            {
                var tenant = _currentDbContext.PTenants.FirstOrDefault(m => m.PTenantId == model.CurrentPTenentId);
                if (tenant != null)
                {
                    tenant.CurrentPropertyCode = model.PropertyCode;
                    tenant.CurrentProperty = model;
                    tenant.IsCurrentTenant = true;
                    _currentDbContext.Entry(tenant).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }
            }
            return model;
        }


        public PLandlord SavePLandlord(PLandlord pLandlord, int userId)
        {
            pLandlord.LandlordAdded = DateTime.UtcNow;
            pLandlord.DateCreated = DateTime.UtcNow;
            pLandlord.DateUpdated = DateTime.UtcNow;
            pLandlord.UpdatedUserId = userId;
            _currentDbContext.PLandlords.Add(pLandlord);
            _currentDbContext.SaveChanges();
            return pLandlord;
        }

        public PTenant SavePTenant(PTenant model, int userId)
        {
            model.CreatedUserId = userId;
            model.DateCreated = DateTime.UtcNow;
            model.IsCurrentTenant = true;
            _currentDbContext.PTenants.Add(model);
            _currentDbContext.SaveChanges();
            return model;
        }

        public void DeletePropertyLandlord(int pLandlordId)
        {
            var pLandlord = GetPropertyLandlordById(pLandlordId);
            pLandlord.IsDeleted = true;
            _currentDbContext.Entry(pLandlord).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }
        public void DeletePropertyById(int pProrpertyId)
        {
            PProperty pProperty = GetPropertyById(pProrpertyId);
            pProperty.IsDeleted = true;
            _currentDbContext.Entry(pProperty).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public PTenant CreatePropertyTenant(PTenant pTenant, int userId)
        {
            pTenant.TenancyAdded = DateTime.UtcNow;
            pTenant.DateCreated = DateTime.UtcNow;
            pTenant.DateUpdated = DateTime.UtcNow;
            pTenant.CreatedUserId = userId;
            if (pTenant.CurrentPropertyId.HasValue && pTenant.CurrentPropertyId > 0)
            {
                pTenant.IsCurrentTenant = true;
            }
            _currentDbContext.PTenants.Add(pTenant);
            _currentDbContext.SaveChanges();
            return pTenant;
        }

        public PTenant UpdatePropertyTenant(PTenant tenant, int userId)
        {
            var currentProperty = GetPropertyById(tenant.CurrentPropertyId ?? 0);

            if (currentProperty != null)
            {
                tenant.CurrentPropertyCode = currentProperty.PropertyCode;
            }

            tenant.DateUpdated = DateTime.UtcNow;
            tenant.UpdatedUserId = userId;
            _currentDbContext.Entry(tenant).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return tenant;
        }

        public void DeletePropertyTenant(int pTenantId)
        {
            PTenant pTenant = _currentDbContext.PTenants.Find(pTenantId);
            _currentDbContext.PTenants.Remove(pTenant);
            _currentDbContext.SaveChanges();
        }

        public async Task UpdateCurrentTenancyFlags()
        {
            var tenants = new HashSet<PTenantViewModelForTenantFlag>(GetAllTenantsToUpdateIsCurrentTenant());
            int counter = 0;
            foreach (var tenant in tenants)
            {
                var pTenant = _mapper.Map(tenant, new PTenant());
                pTenant.IsCurrentTenant = tenant.TenancyStarted <= DateTime.UtcNow && tenant.TenancyVacateDate >= DateTime.UtcNow ? true : false;
                pTenant.IsFutureTenant = tenant.TenancyStarted > DateTime.UtcNow ? true : false;
                _currentDbContext.PTenants.Attach(pTenant);
                _currentDbContext.Entry(pTenant).Property(x => x.IsCurrentTenant).IsModified = true;
                _currentDbContext.Entry(pTenant).Property(x => x.IsFutureTenant).IsModified = true;
                counter++;

                if (counter == 500)
                {
                    await _currentDbContext.SaveChangesAsync();
                    counter = 0;
                }
            }

            await _currentDbContext.SaveChangesAsync();
        }

        public async Task UpdateAllPropertyTenantsFlags()
        {
            var properties = new List<PProperty>();
            var tenants = new List<PTenant>();

            properties = _currentDbContext.PProperties.AsNoTracking().Where(x => x.PropertyStatus != "ARCHIVE" && x.PropertyStatus != "ARCHIVED" && x.PropertyStatus != "VACATED").ToList();
            tenants = _currentDbContext.PTenants.AsNoTracking().ToList();

            int counter = 0;
            foreach (var property in properties)
            {

                int pTenantId = 0;

                pTenantId = tenants.FirstOrDefault(x => x.CurrentPropertyCode == property.PropertyCode && x.IsHeadTenant == true && x.IsCurrentTenant == true)?.PTenantId ?? 0;

                if (pTenantId == 0)
                {
                    pTenantId = tenants.FirstOrDefault(x => x.CurrentPropertyCode == property.PropertyCode)?.PTenantId ?? 0;
                }

                if (pTenantId > 0)
                {
                    property.CurrentPTenentId = pTenantId;
                    _currentDbContext.PProperties.Attach(property);
                    _currentDbContext.Entry(property).Property(x => x.CurrentPTenentId).IsModified = true;
                }

                counter++;

                if (counter == 500)
                {
                    await _currentDbContext.SaveChangesAsync();
                    counter = 0;
                }
            }

            await _currentDbContext.SaveChangesAsync();
        }
    }
}