using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IEmailServices
    {
        List<TenantEmailTemplates> GetAllTenantEmailTemplates(int tenantId);
        List<TenantEmailConfig> GetAllActiveTenantEmailConfigurations(int tenantId);
        TenantEmailConfig GetEmailConfigurationsById(int tenantId,int? siteId=null);
        List<TenantEmailNotificationQueue> GetAllTenantEmailNotificationQueues();
        List<TenantEmailTemplateVariable> GetAllTenantEmailVariables(int tenantId);
        TenantEmailNotificationQueue SaveEmailNotificationQueue(TenantEmailNotificationQueueViewModel queueItem);
        TenantEmailNotificationQueue GetTenantEmailNotificationQueueById(int id);
        TenantEmailTemplates GetSuitableEmailTemplate(WorksOrderNotificationTypeEnum notificationType, int tenantId,int? siteId=null);
        List<OrderPTenantEmailRecipient> GetAllPropertyTenantRecipients(int filterByOrderId = 0);
        OrderPTenantEmailRecipient AddPropertyTenantRecipients(int orderId, int shipPropertyId, int propertyTenantId, TenantEmailNotificationQueue lastNotification);
        OrderPTenantEmailRecipient DeletePropertyTenantEmailRecipient(OrderPTenantEmailRecipient notification, int userId);
        List<TenantEmailNotificationQueue> GetAllTenantEmailNotificationQueuesAwaitProcessing(DateTime targetTime);
        List<TenantEmailNotificationQueue> GetAllTenantEmailNotificationQueuesbyOrderId(int orderId, int InvoiceMasterId,int TenantId,int? TemplateId);
        TenantEmailConfig GetFirstActiveTenantEmailConfiguration();
    }
}
