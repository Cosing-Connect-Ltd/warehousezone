using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class TenantEmailConfigServices : ITenantEmailConfigServices
    {
        private readonly IApplicationContext _currentDbContext;

        public TenantEmailConfigServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public IEnumerable<TenantEmailConfig> GetAllEmialConfigByTenant(int tenantId)
        {
            return _currentDbContext.TenantEmailConfigs.Where(e => e.TenantId == tenantId && e.IsDeleted != true).ToList();
        }

        public TenantEmailConfig GetEmailConfigById(int emailConfigId)
        {
            return _currentDbContext.TenantEmailConfigs.Find(emailConfigId);
        }

        public int SaveEmailConfig(TenantEmailConfig emailConfig, int userId, int tenantId)
        {

            emailConfig.DateCreated = DateTime.UtcNow;
            emailConfig.DateUpdated = DateTime.UtcNow;
            emailConfig.CreatedBy = userId;
            emailConfig.UpdatedBy = userId;
            emailConfig.TenantId = tenantId;

            _currentDbContext.Entry(emailConfig).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return emailConfig.TenantEmailConfigId;
        }

        public void UpdateEmailConfig(TenantEmailConfig emailConfig, int userId, int tenantId)
        {

            emailConfig.DateUpdated = DateTime.UtcNow;
            emailConfig.UpdatedBy = userId;
            emailConfig.TenantId = tenantId;
            emailConfig.CreatedBy = userId;
            _currentDbContext.Entry(emailConfig).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

        }

        public void DeleteEmailConfig(TenantEmailConfig emailConfig, int userId)
        {
            emailConfig.IsDeleted = true;
            emailConfig.UpdatedBy = userId;
            emailConfig.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(emailConfig).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }


    }
}