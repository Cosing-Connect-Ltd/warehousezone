using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;

namespace Ganedata.Core.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IApplicationContext _currentDbContext;

        public EmailNotificationService(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public TenantEmailTemplates GetTenantEmailTemplateById(int id, int tenantId)
        {
            return _currentDbContext.TenantEmailTemplates.FirstOrDefault(m => m.TemplateId == id && m.TenantId == tenantId);
        }

        public IEnumerable<TenantEmailTemplates> GetAllTenantEmailTemplates(int tenantId)
        {
            return _currentDbContext.TenantEmailTemplates.Where(m => m.TenantId == tenantId);
        }

        public TenantEmailTemplates CreateEmailEmailTemplate(TenantEmailTemplates template)
        {
            var emailTemplate = new TenantEmailTemplates()
            {
                HtmlHeader = template.HtmlHeader,
                EventName = template.EventName,
                Body = template.Body,
                HtmlFooter = template.HtmlFooter,
                DateCreated = DateTime.UtcNow,
                CreatedBy = template.CreatedBy,
                NotificationType = template.NotificationType,
                TenantId = template.TenantId,
            };
            _currentDbContext.Entry(emailTemplate).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return emailTemplate;
        }

        public TenantEmailTemplates SaveEmailEmailTemplate(TenantEmailTemplates newTemplate)
        {
            var template = _currentDbContext.TenantEmailTemplates.Find(newTemplate.TemplateId);
            if (template != null)
            {
                template.HtmlHeader = newTemplate.HtmlHeader;
                template.Body = newTemplate.Body;
                template.HtmlFooter = newTemplate.HtmlFooter;
                template.NotificationType = newTemplate.NotificationType;
                template.EventName = newTemplate.EventName;
                template.DateUpdated = DateTime.UtcNow;
                template.UpdatedBy = newTemplate.UpdatedBy;
                template.TenantId = newTemplate.TenantId;

                _currentDbContext.Entry(template).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }

            return template;
        }

        public void RemoveTenantEmailTemplateById(int id, int tenantId)
        {
            _currentDbContext.TenantEmailTemplates.Remove(GetTenantEmailTemplateById(id, tenantId));
            _currentDbContext.SaveChanges();
        }
    }
}