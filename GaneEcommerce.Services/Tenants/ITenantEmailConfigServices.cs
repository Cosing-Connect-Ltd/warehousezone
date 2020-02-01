using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface ITenantEmailConfigServices   
    {
        IEnumerable<TenantEmailConfig> GetAllEmialConfigByTenant(int tenantId);
        TenantEmailConfig GetEmailConfigById(int emailConfigId);
        int SaveEmailConfig(TenantEmailConfig emailConfig, int userId, int tenantId);
        void UpdateEmailConfig(TenantEmailConfig emailConfig, int userId, int tenantId);
        void DeleteEmailConfig(TenantEmailConfig emailConfig, int userId);
    }
}
