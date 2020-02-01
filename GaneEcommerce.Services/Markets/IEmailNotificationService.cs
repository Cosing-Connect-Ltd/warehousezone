using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Services
{
    public interface IEmailNotificationService
    {
        TenantEmailTemplates GetTenantEmailTemplateById(int id, int tenantId);
        IEnumerable<TenantEmailTemplates> GetAllTenantEmailTemplates(int tenantId);

        TenantEmailTemplates CreateEmailEmailTemplate(TenantEmailTemplates template);

        TenantEmailTemplates SaveEmailEmailTemplate(TenantEmailTemplates template);
        void RemoveTenantEmailTemplateById(int id, int tenantId);
    }
}