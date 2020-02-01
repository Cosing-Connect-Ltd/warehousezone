using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public class TenantsServices : ITenantsServices
    {

        private readonly IApplicationContext _currentDbContext;

        //constructor
        public TenantsServices(IApplicationContext applicationContext)
        {
            _currentDbContext = applicationContext;
        }

        public IEnumerable<Tenant> GetAllClients(int tenantId)
        {
            return _currentDbContext.Tenants.Where(x => x.TenantId.Equals(tenantId) && x.IsDeleted != true);
        }

        public Tenant GetByClientId(int clientId)
        {
            return _currentDbContext.Tenants.Find(clientId);
        }

        public void Update(Tenant clients)
        {
            _currentDbContext.Tenants.Attach(clients);
            _currentDbContext.Entry(clients).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }
        public void Add(Tenant clients)
        {
            _currentDbContext.Tenants.Add(clients);
            _currentDbContext.Entry(clients).State = EntityState.Added;
            _currentDbContext.SaveChanges();
        }
        public void UpdateTenantConfig(TenantConfig tenantConfig)
        {

            _currentDbContext.TenantConfigs.Add(tenantConfig);
            _currentDbContext.Entry(tenantConfig).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }
        public void AddTenantConfig(TenantConfig tenantConfig)
        {
            _currentDbContext.TenantConfigs.Add(tenantConfig);
            _currentDbContext.Entry(tenantConfig).State = EntityState.Added;
            _currentDbContext.SaveChanges();
        }
        public IEnumerable<Tenant> GetAllTenants()
        {
            return _currentDbContext.Tenants.Where(a => a.IsDeleted != true);
        }
        public IEnumerable<TenantConfig> GetAllTenantConfig(int CurrentTenantId)
        {
            return _currentDbContext.TenantConfigs.Where(u => u.TenantId == CurrentTenantId);
        }

        public List<TenantModules> GetAllTenantModules(int tenantId)
        {
            return _currentDbContext.TenantModules.Where(m => tenantId == 0 || m.TenantId == tenantId).ToList();
        }

        public TenantConfig GetTenantConfigById(int tenantId)
        {
            return _currentDbContext.TenantConfigs.AsNoTracking().FirstOrDefault(m => m.TenantId == tenantId);
        }

        public bool IsModuleEnabled(int tenantId, TenantModuleEnum tenantModule)
        {
            return _currentDbContext.TenantModules.Any(m => m.ModuleId == (int)tenantModule && m.TenantId == tenantId);
        }

    }
}