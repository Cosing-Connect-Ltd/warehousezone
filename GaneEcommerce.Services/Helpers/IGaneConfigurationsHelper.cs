using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    public interface IGaneConfigurationsHelper
    {
        Boolean ApiErrorNotification(string body, int tenantId);

        string GetRecipientEmailForAccount(int? accountId, int accountContactId = 0);
        List<MailAddress> GetRecipientEmailsForOrder(Order order);
        Task<string> DispatchMailNotification(TenantEmailNotificationQueue notification, int tenantId, WorksOrderNotificationTypeEnum worksOrderNotificationType = WorksOrderNotificationTypeEnum.WorksOrderLogTemplate, bool sendImmediately = true, int? accountId = null, int? userId = null,string userEmail=null,string confrmationLink=null);

        Task<string> SendStandardMailNotification(int tenantId, string subject, string bodyHtml, string footerHtml, string recipients, bool salesRequiresAuthorisation = true);
        Task UpdateIsCurrentTenantFlags();
        string TranslateEmailTemplateForOrder(TenantEmailNotificationQueue notificationItem, int tenantId, WorksOrderNotificationTypeEnum worksOrderNotificationType = WorksOrderNotificationTypeEnum.WorksOrderLogTemplate, int? accountId = null, int? userId = null,string ConfirmationLink=null);

        Task<string> CreateTenantEmailNotificationQueue(string subject,
            OrderViewModel order, string attachmentVirtualPath = null, bool sendImmediately = true,
            DateTime? scheduleStartTime = null, DateTime? scheduleEndTime = null, string scheduleResourceName = null,
            OrderRecipientInfo shipmentAndRecipientInfo = null,
            WorksOrderNotificationTypeEnum worksOrderNotificationType =
                WorksOrderNotificationTypeEnum.WorksOrderLogTemplate, Appointments appointment = null, int TenantId = 0, int? accountId = null,string UserEmail=null, string confirmationLink = null, int? userId = null);

        Task<string> DispatchTenantEmailNotificationQueues(int tenantId);
        string GetMailMergeVariableValueFromOrder(Order order, Account account, MailMergeVariableEnum variableType, TenantEmailNotificationQueue notificationItem = null, int? userID = null,string confirmationLink=null);
        bool IsPOComplete(int? POID, int UserId, int warehouseId);
        bool ActiveStocktake(int warehouseId);
        Boolean GetStStatus(int id);
        string GetStStatusString(int StatusCode);
        string GetDeviceLastIp(string serial);
        string GetDeviceLastPingDate(string serial);
        bool GetDeviceCurrentStatus(string serial);
        bool SendMail(string subject, string body, int tenantId);
        string GetPropertyFirstline(int pPropertyId);
        string GetActionResultHtml(Controller controller, string partialViewName, object model);
        Task<string> SendStandardMailProductGroup(int tenantId, string subject, int accountId);
        Task<string> SendRegistrationEmail(int tenantId, WorksOrderNotificationTypeEnum worksOrderNotificationType, string subject, string ConfirmatonLink, string recipients, int? accountId, bool salesRequiresAuthorisation = true, int userID = 0);
    }
}