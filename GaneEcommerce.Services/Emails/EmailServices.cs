using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Enums;
using AutoMapper;
using Ganedata.Core.Entities.Domain.ViewModels;

namespace Ganedata.Core.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IApplicationContext _applicationContext;
        private readonly IMapper _mapper;

        public EmailServices(IApplicationContext applicationContext, IMapper mapper)
        {
            _applicationContext = applicationContext;
            _mapper = mapper;
        }
        public List<TenantEmailTemplates> GetAllTenantEmailTemplates(int tenantId)
        {
            return _applicationContext.TenantEmailTemplates.Where(u=>u.TenantId==tenantId && u.IsDeleted != true).ToList();
        }

        public List<TenantEmailConfig> GetAllActiveTenantEmailConfigurations(int tenantId)
        {
            return _applicationContext.TenantEmailConfigs.AsNoTracking().Where(e => e.TenantId == tenantId && e.IsActive && e.IsDeleted!= true).ToList();
        }

        public TenantEmailConfig GetEmailConfigurationsById(int tenantId)
        {
            return _applicationContext.TenantEmailConfigs.FirstOrDefault(m => m.TenantId == tenantId && m.IsActive == true && m.IsDeleted != true);
        }

        public List<TenantEmailNotificationQueue> GetAllTenantEmailNotificationQueues()
        {
            return _applicationContext.TenantEmailNotificationQueues.AsNoTracking().ToList();
        }

        public TenantEmailNotificationQueue GetTenantEmailNotificationQueueById(int id)
        {
            return _applicationContext.TenantEmailNotificationQueues.AsNoTracking().FirstOrDefault(x => x.TenantEmailNotificationQueueId == id);
        }

        public List<TenantEmailNotificationQueue> GetAllTenantEmailNotificationQueuesAwaitProcessing(DateTime targetTime)
        {
            return _applicationContext.TenantEmailNotificationQueues.AsNoTracking().Where(p =>
            p.ScheduledProcessing && p.ActualProcessingTime == null && p.IsNotificationCancelled != true && (p.Appointment == null || p.Appointment.EndTime > DateTime.Now)).ToList();
        }

        public List<TenantEmailTemplateVariable> GetAllTenantEmailVariables(int tenantId)
        {
            return _applicationContext.TenantEmailTemplateVariables.Where(m => m.TenantId == tenantId).ToList();
        }

        public TenantEmailNotificationQueue SaveEmailNotificationQueue(TenantEmailNotificationQueueViewModel queueItem)
        {
            if (queueItem.TenantEmailNotificationQueueId < 1)
            {
                var newQueueItem = _mapper.Map(queueItem, new TenantEmailNotificationQueue());
                _applicationContext.Entry(newQueueItem).State = EntityState.Added;
                _applicationContext.SaveChanges();
                return newQueueItem;
            }
            else
            {
                var newQueueItem = _applicationContext.TenantEmailNotificationQueues.Find(queueItem.TenantEmailNotificationQueueId);
                _mapper.Map(queueItem, newQueueItem);
                _applicationContext.Entry(newQueueItem).State = EntityState.Modified;
                _applicationContext.SaveChanges();
                return newQueueItem;
            }
        }

        public TenantEmailTemplates GetSuitableEmailTemplate(WorksOrderNotificationTypeEnum notificationType, int tenantId)
        {
            var selectedTemplate = _applicationContext.TenantEmailTemplates.FirstOrDefault(m => m.NotificationType == notificationType && m.TenantId == tenantId && m.IsDeleted !=true);
            return selectedTemplate;
        }

        public List<OrderPTenantEmailRecipient> GetAllPropertyTenantRecipients(int filterByOrderId = 0)
        {
            return _applicationContext.OrderPTenantEmailRecipients.Where(p => (filterByOrderId == 0 || filterByOrderId == p.OrderId) && p.IsDeleted != true).ToList();
        }

        public OrderPTenantEmailRecipient AddPropertyTenantRecipients(int orderId, int shipPropertyId, int propertyTenantId, TenantEmailNotificationQueue lastNotification)
        {
            var existingRecipient = _applicationContext.OrderPTenantEmailRecipients.FirstOrDefault(m => m.PTenantId == propertyTenantId && m.OrderId == orderId);
            if (existingRecipient == null)
            {
                var recipient = new OrderPTenantEmailRecipient()
                {
                    OrderId = orderId,
                    DateUpdated = DateTime.UtcNow,
                    PPropertyId = shipPropertyId,
                    PTenantId = propertyTenantId,
                    LastEmailNotification = lastNotification
                };
                _applicationContext.Entry(recipient).State = EntityState.Added;
                _applicationContext.SaveChanges();
                return recipient;
            }
            return existingRecipient;
        }

        public OrderPTenantEmailRecipient DeletePropertyTenantEmailRecipient(OrderPTenantEmailRecipient notification, int userId)
        {
            notification.IsDeleted = true;
            notification.UpdatedBy = userId;
            notification.DateUpdated = DateTime.UtcNow;
            _applicationContext.Entry(notification).State = EntityState.Modified;
            _applicationContext.SaveChanges();
            return notification;
        }

        public List<TenantEmailNotificationQueue> GetAllTenantEmailNotificationQueuesbyOrderId(int orderId,int InvoiceMasterId,int TenantId,int? templateId)
        {
            if (orderId > 0)
            {
                return _applicationContext.TenantEmailNotificationQueues.Where(u => u.OrderId == orderId && (templateId == 0 || u.TenantEmailTemplatesId==templateId)).ToList();
            }
            else {
                return _applicationContext.TenantEmailNotificationQueues.Where(u => u.InvoiceMasterId == InvoiceMasterId).ToList();
            }
        }
    }
}