using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface ITenantsServices
    {
        IEnumerable<Tenant> GetAllClients(int tenantId);
        IEnumerable<Tenant> GetAllTenants();
        Tenant GetByClientId(int clientId);
        void Update(Tenant clients);
        void Add(Tenant clients);
        IEnumerable<TenantConfig> GetAllTenantConfig(int CurrentTenantId);
        void UpdateTenantConfig(TenantConfig tenantConfig);
        void AddTenantConfig(TenantConfig tenantConfig);
        List<TenantModules> GetAllTenantModules(int tenantId);

        TenantConfig GetTenantConfigById(int tenantId,int?siteId=null);

        bool IsModuleEnabled(int tenantId, TenantModuleEnum tenantModule);
    };
}
