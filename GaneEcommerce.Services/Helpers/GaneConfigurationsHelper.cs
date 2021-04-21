using AutoMapper;
using Elmah;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Ganedata.Core.Services
{
    public class GaneConfigurationsHelper : IGaneConfigurationsHelper
    {
        private readonly IEmailServices _emailServices;
        private readonly IAccountServices _accountServices;
        private readonly IPropertyService _propertyService;
        private readonly IEmployeeServices _employeeServices;
        private readonly IOrderService _orderService;
        private readonly IStockTakeApiService _stockTakeService;
        private readonly ITenantsServices _tenantServices;
        private readonly ITerminalServices _terminalServices;
        private readonly IAppointmentsService _appointmentServices;
        private readonly IMapper _mapper;

        public GaneConfigurationsHelper(IEmailServices emailServices, IAccountServices accountServices, IPropertyService propertyService, ITerminalServices terminalServices,
            IEmployeeServices employeeServices, IOrderService orderService, IStockTakeApiService stockTakeService, ITenantsServices tenantServices, IAppointmentsService appointmentServices, IMapper mapper)
        {
            _emailServices = emailServices;
            _accountServices = accountServices;
            _propertyService = propertyService;
            _employeeServices = employeeServices;
            _orderService = orderService;
            _stockTakeService = stockTakeService;
            _tenantServices = tenantServices;
            _terminalServices = terminalServices;
            _appointmentServices = appointmentServices;
            _mapper = mapper;

        }

        public Boolean ApiErrorNotification(string body, int tenantId)
        {
            TenantEmailConfig emailconfig = _emailServices.GetAllActiveTenantEmailConfigurations(tenantId).FirstOrDefault();
            var tenantConfig = _tenantServices.GetTenantConfigById(tenantId);
            EmailSender m = new EmailSender(tenantConfig.ErrorLogsForwardEmails, emailconfig.UserEmail, body, "Api Error Notification", "", emailconfig.SmtpHost, emailconfig.SmtpPort, emailconfig.UserEmail, emailconfig.Password);

            if (m.SendMail())
                return false;
            else
                return true;
        }

        public string GetRecipientEmailForAccount(int? accountId, int accountContactId = 0)
        {
            var account = _accountServices.GetAccountsById(accountId ?? 0);
            if (account != null)
            {
                if (accountContactId > 0)
                {
                    var accountContact = account.AccountContacts.FirstOrDefault(m => m.AccountContactId == accountContactId && m.IsDeleted != true);
                    if (accountContact != null)
                    {
                        return accountContact.ContactEmail;
                    }
                }

                if (string.IsNullOrEmpty(account.AccountEmail))
                {
                    return account.AccountEmail;
                }

                if (account.AccountContacts == null) return string.Empty;

                var billingContact = account.AccountContacts.FirstOrDefault(m => m.ConTypeInvoices && m.IsDeleted != true);

                if (billingContact != null)
                {
                    return billingContact.ContactEmail;
                }
                else
                {
                    billingContact = account.AccountContacts.FirstOrDefault(m => m.ConTypeStatment && m.IsDeleted != true);
                    if(billingContact!=null)
                        return billingContact.ContactEmail;
                }

                var availableContact = account.AccountContacts.FirstOrDefault(m=> m.IsDeleted != true);
                if (availableContact != null)
                {
                    return availableContact.ContactEmail;
                }
            }
            return string.Empty;
        }

        public List<MailAddress> GetRecipientEmailsForOrder(Order order)
        {
            var tenantConfig = _tenantServices.GetTenantConfigById(order.TenentId);

            if (tenantConfig == null || tenantConfig.EnableLiveEmails != true) return new List<MailAddress>();

            var recipientEmails = new List<MailAddress>();

            switch ((InventoryTransactionTypeEnum)order.InventoryTransactionTypeId)
            {
                case InventoryTransactionTypeEnum.PurchaseOrder:
                case InventoryTransactionTypeEnum.SalesOrder:
                    if (order != null && order.Account != null && order.AccountID.HasValue)
                    {
                        var emails = _emailServices.GetAllPropertyTenantRecipients(order.OrderID).Select(u => u.EmailAddress).ToList();
                        foreach (var item in emails)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                recipientEmails.Add(new MailAddress(item));
                            }
                        }
                    }

                    break;
                case InventoryTransactionTypeEnum.WorksOrder:
                    var tenants = _propertyService.GetAppointmentRecipientTenants(order.OrderID).ToList();
                    if (tenants.Any())
                    {
                        var tenantEmails = tenants.Select(c => new MailAddress(c.Email, c.TenantFullName));
                        recipientEmails.AddRange(tenantEmails);
                    }
                    break;
                case InventoryTransactionTypeEnum.TransferIn:
                case InventoryTransactionTypeEnum.TransferOut:
                    break;
            }

            return recipientEmails;
        }
        public List<MailAddress> GetRecipientEmailsForAccount(int accountId, int TenentId)
        {
            var tenantConfig = _tenantServices.GetTenantConfigById(TenentId);

            if (tenantConfig == null || tenantConfig.EnableLiveEmails != true) return new List<MailAddress>();

            var recipientEmails = new List<MailAddress>();
            if (accountId > 0)
            {
                var emails = _accountServices.GetAllValidAccountContactsByAccountId(accountId, TenentId).Where(u => u.ConTypeInvoices == true).Select(u => u.ContactEmail).ToList();
                foreach (var item in emails)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        recipientEmails.Add(new MailAddress(item));
                    }
                }
            }



            return recipientEmails;
        }

        public async Task<string> DispatchMailNotification(TenantEmailNotificationQueue notification, int tenantId, WorksOrderNotificationTypeEnum worksOrderNotificationType = WorksOrderNotificationTypeEnum.WorksOrderLogTemplate, bool sendImmediately = true, int? accountId = null, int? UserId = null, string userEmail = null, string confrmationLink = null, int? siteId = null)
        {

            if (!sendImmediately) return "Email notification has been scheduled.";

            var mailmsg = new MailMessage();
            var smtp = new SmtpClient();
            Attachment attachment = null;
            List<MailAddress> recipientEmails = new List<MailAddress>();


            try
            {
                var tenantConfig = _tenantServices.GetTenantConfigById(tenantId, siteId);

                mailmsg = new MailMessage();
                mailmsg.ReplyToList.Add(new MailAddress(tenantConfig.DefaultReplyToAddress));
                var notificationItem = _emailServices.GetTenantEmailNotificationQueueById(notification.TenantEmailNotificationQueueId);
                mailmsg.AlternateViews.Add(CreateAlternateView(TranslateEmailTemplateForOrder(notificationItem, tenantId, worksOrderNotificationType, accountId, UserId, confrmationLink)));

                if (notificationItem.OrderId > 0 && notificationItem.Order == null)
                {
                    notificationItem.Order = _orderService.GetOrderById(notificationItem.OrderId ?? 0);
                }

                if (notificationItem.TenantEmailTemplatesId == (int)WorksOrderNotificationTypeEnum.AwaitingOrderTemplate && !string.IsNullOrEmpty(tenantConfig.AuthorisationAdminEmail))
                {
                    mailmsg.To.Add(tenantConfig.AuthorisationAdminEmail);
                }

                if (notificationItem.Order != null)
                {
                    if (worksOrderNotificationType == WorksOrderNotificationTypeEnum.InvoiceTemplate)
                    {
                        recipientEmails = GetRecipientEmailsForAccount(accountId ?? 0, tenantId);
                    }
                    else
                    {
                        recipientEmails = GetRecipientEmailsForOrder(notificationItem.Order);
                    }
                }
                if (!string.IsNullOrEmpty(userEmail))
                {
                    recipientEmails.Add(new MailAddress(userEmail));

                }

                if (recipientEmails.Any() && tenantConfig.EnableLiveEmails == true)
                {
                    var unique = new List<MailAddress>();
                    if (!string.IsNullOrEmpty(notificationItem.CustomRecipients))
                    {
                        unique = recipientEmails.Where(x => !notificationItem.CustomRecipients.Contains(x.ToString())).Distinct().ToList();
                    }
                    else
                    {
                        unique = recipientEmails.Distinct().ToList();
                    }

                    if (unique.Any())
                    {
                        unique.CopyItemsTo(mailmsg.To);
                    }

                }

                if (!string.IsNullOrEmpty(tenantConfig.WarehouseLogEmailsToDefault))
                {
                    var emails =
                        tenantConfig.WarehouseLogEmailsToDefault.Split(new[] { ";" },
                            StringSplitOptions.RemoveEmptyEntries);
                    foreach (var email in emails)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            mailmsg.Bcc.Add(email);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(notificationItem.CustomRecipients) && tenantConfig.EnableLiveEmails == true)
                {
                    notificationItem.CustomRecipients
                        .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Distinct()
                        .Select(m => new MailAddress(m))
                        .ForEach(x => mailmsg.To.Add(x));
                }

                if (!string.IsNullOrEmpty(notificationItem.CustomCcRecipients) && tenantConfig.EnableLiveEmails == true)
                {
                    notificationItem.CustomCcRecipients
                        .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Distinct()
                        .Select(m => new MailAddress(m))
                        .ForEach(x => mailmsg.CC.Add(x));
                }

                if (!string.IsNullOrEmpty(notificationItem.CustomBccRecipients) && tenantConfig.EnableLiveEmails == true)
                {
                    notificationItem.CustomBccRecipients
                        .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Distinct()
                        .Select(m => new MailAddress(m))
                        .ForEach(x => mailmsg.Bcc.Add(x));
                }

                // update queue with final email addresses included in mail message
                notificationItem.CustomRecipients = string.Join(";", mailmsg.To.Select(m => m.Address));
                notificationItem.CustomCcRecipients = string.Join(";", mailmsg.CC.Select(m => m.Address));
                notificationItem.CustomBccRecipients = string.Join(";", mailmsg.Bcc.Select(m => m.Address));
                notificationItem.ActualProcessingTime = DateTime.Now;
                _emailServices.SaveEmailNotificationQueue(_mapper.Map(notificationItem, new TenantEmailNotificationQueueViewModel()));

                var emailconfig = _emailServices.GetEmailConfigurationsById(tenantId);
                if (emailconfig == null)
                {
                    return "No email configuration found!";
                }

                if (mailmsg.To.Count < 1 && mailmsg.Bcc.Count < 1)
                {
                    return "No recipient found!";
                }

                mailmsg.From = new MailAddress(emailconfig.UserEmail, tenantConfig.DefaultMailFromText);
                mailmsg.Subject = notificationItem.EmailSubject;


                if (!string.IsNullOrEmpty(notificationItem.AttachmentVirtualPath))
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath(notificationItem.AttachmentVirtualPath)))
                    {
                        attachment = new Attachment(HttpContext.Current.Server.MapPath(notificationItem.AttachmentVirtualPath));

                        mailmsg.Attachments.Add(attachment);
                    }
                }

                smtp.Host = emailconfig.SmtpHost.Trim();
                smtp.Port = emailconfig.SmtpPort;
                smtp.EnableSsl = emailconfig.EnableSsl;
                mailmsg.IsBodyHtml = true;


                if (emailconfig.EnableRelayEmailServer != true)
                {
                    smtp.Credentials = new NetworkCredential(emailconfig.UserEmail.Trim(), emailconfig.Password.Trim());
                }
                if (mailmsg.To.Any() || mailmsg.Bcc.Any())
                {
                    await Task.Run(() =>
                    {
                        smtp.Send(mailmsg);
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return ex.Message;
            }
            finally
            {
                if (attachment != null) { attachment.Dispose(); }
                mailmsg.Dispose();
                smtp.Dispose();
            }

            return "Success";
        }

        public async Task<string> SendStandardMailNotification(int tenantId, string subject, string bodyHtml, string footerHtml, string recipients, bool salesRequiresAuthorisation = true)
        {
            var mailmsg = new MailMessage();
            var smtp = new SmtpClient();
            Attachment attachment = null;

            try
            {
                var tenantConfig = _tenantServices.GetTenantConfigById(tenantId);

                mailmsg = new MailMessage();
                mailmsg.ReplyToList.Add(new MailAddress(tenantConfig.DefaultReplyToAddress));

                var recipientEmails = new List<MailAddress>();

                if (!string.IsNullOrEmpty(recipients))
                {
                    recipientEmails.AddRange(recipients.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(m => new MailAddress(m)));
                }

                if (recipientEmails.Any() && tenantConfig.EnableLiveEmails == true)
                {
                    recipientEmails.CopyItemsTo(mailmsg.To);
                }

                if (!string.IsNullOrEmpty(tenantConfig.WarehouseLogEmailsToDefault))
                {
                    var emails = tenantConfig.WarehouseLogEmailsToDefault.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var email in emails)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            mailmsg.Bcc.Add(new MailAddress(email));
                        }
                    }
                }

                if (mailmsg.To.Count < 1 && mailmsg.Bcc.Count < 1 && mailmsg.CC.Count < 1)
                {
                    return "No recipient found!";
                }

                var emailconfig = _emailServices.GetAllActiveTenantEmailConfigurations(tenantId).FirstOrDefault();
                if (emailconfig == null)
                {
                    return "Could not send email, no email configuration found!";
                }

                if (salesRequiresAuthorisation && !string.IsNullOrEmpty(tenantConfig.AuthorisationAdminEmail))
                {
                    mailmsg.To.Add(tenantConfig.AuthorisationAdminEmail);
                }

                var template = _emailServices.GetSuitableEmailTemplate(WorksOrderNotificationTypeEnum.Standard, tenantId);
                if (template == null)
                {
                    return "Could not send email, no suitable template found";
                }

                mailmsg.Body = template.Body.Replace("{EMAILBODY}", bodyHtml);

                if (!string.IsNullOrEmpty(template.HtmlFooter))
                {
                    mailmsg.Body += template.HtmlFooter.Replace("{EMAILFOOTER}", footerHtml ?? string.Empty);
                }
                mailmsg.IsBodyHtml = true;
                mailmsg.From = new MailAddress(emailconfig.UserEmail, tenantConfig.DefaultMailFromText);
                mailmsg.Subject = subject;

                smtp.Host = emailconfig.SmtpHost.Trim();
                smtp.Port = emailconfig.SmtpPort;
                smtp.EnableSsl = emailconfig.EnableSsl;

                if (emailconfig.EnableRelayEmailServer != true)
                {
                    smtp.Credentials = new NetworkCredential(emailconfig.UserEmail.Trim(), emailconfig.Password.Trim());
                }

                await Task.Run(() =>
                {
                    smtp.Send(mailmsg);
                });

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return ex.Message;
            }
            finally
            {
                if (attachment != null) { attachment.Dispose(); }
                mailmsg.Dispose();
                smtp.Dispose();
            }

            return "Success";

        }

        public async Task<string> SendStandardMailProductGroup(int tenantId, string subject, int accountId)
        {
            var mailmsg = new MailMessage();
            var smtp = new SmtpClient();
            Attachment attachment = null;

            try
            {
                var tenantConfig = _tenantServices.GetTenantConfigById(tenantId);
                var Accountemail = _accountServices.GetAccountsById(accountId)?.AccountEmail;
                if (Accountemail != null)
                {
                    mailmsg = new MailMessage();
                    mailmsg.ReplyToList.Add(new MailAddress(tenantConfig.DefaultReplyToAddress));
                    var recipientEmails = new List<MailAddress>();
                    recipientEmails.Add(new MailAddress(Accountemail));
                    if (recipientEmails.Any() && tenantConfig.EnableLiveEmails == true)
                    {
                        recipientEmails.CopyItemsTo(mailmsg.To);
                    }

                    if (!string.IsNullOrEmpty(tenantConfig.WarehouseLogEmailsToDefault))
                    {
                        var emails = tenantConfig.WarehouseLogEmailsToDefault.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var email in emails)
                        {
                            if (!string.IsNullOrEmpty(email))
                            {
                                mailmsg.Bcc.Add(new MailAddress(email));
                            }
                        }
                    }

                    if (mailmsg.To.Count < 1 && mailmsg.Bcc.Count < 1 && mailmsg.CC.Count < 1)
                    {
                        return "No recipient found!";
                    }

                    var emailconfig = _emailServices.GetAllActiveTenantEmailConfigurations(tenantId).FirstOrDefault();
                    if (emailconfig == null)
                    {
                        return "Could not send email, no email configuration found";
                    }

                    var template = _emailServices.GetSuitableEmailTemplate(WorksOrderNotificationTypeEnum.ProductGroupTemplate, tenantId);
                    if (template == null)
                    {
                        return "Could not send email, no suitable template found";
                    }

                    var file = HttpContext.Current.Server.MapPath("~/UploadedFiles/Invoices/SO-00000001_23122018223354159.xlsx");

                    if (File.Exists(HttpContext.Current.Server.MapPath(file)))
                    {
                        attachment = new Attachment(HttpContext.Current.Server.MapPath(file));
                        mailmsg.Attachments.Add(attachment);
                    }

                    mailmsg.Attachments.Add(attachment);
                    mailmsg.Body = template.Body;
                    mailmsg.IsBodyHtml = true;
                    mailmsg.From = new MailAddress(emailconfig.UserEmail, tenantConfig.DefaultMailFromText);
                    mailmsg.Subject = subject;
                    smtp.Host = emailconfig.SmtpHost.Trim();
                    smtp.Port = emailconfig.SmtpPort;
                    smtp.EnableSsl = emailconfig.EnableSsl;

                    if (emailconfig.EnableRelayEmailServer != true)
                    {
                        smtp.Credentials = new NetworkCredential(emailconfig.UserEmail.Trim(), emailconfig.Password.Trim());
                    }

                    await Task.Run(() =>
                    {
                        smtp.Send(mailmsg);
                    });

                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return ex.Message;
            }
            finally
            {
                if (attachment != null) { attachment.Dispose(); }
                mailmsg.Dispose();
                smtp.Dispose();
            }

            return "Success";
        }

        private static AlternateView CreateAlternateView(string htmlBody)
        {
            //  Create an AlternateView object for supporting the HTML
            var avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);
            return avHtml;
        }

        public async Task UpdateIsCurrentTenantFlags()
        {
            await _propertyService.UpdateCurrentTenancyFlags();
        }

        public string TranslateEmailTemplateForOrder(TenantEmailNotificationQueue notificationItem, int tenantId, WorksOrderNotificationTypeEnum worksOrderNotificationType = WorksOrderNotificationTypeEnum.WorksOrderLogTemplate, int? accountId = null, int? userId = null, string ConfirmatonLink = null)
        {

            var order = _orderService.GetOrderById(notificationItem.OrderId ?? 0);

            var allEmailTemplates = _emailServices.GetAllTenantEmailTemplates(tenantId);
            var selectedTemplate = allEmailTemplates.FirstOrDefault(m => m.NotificationType == worksOrderNotificationType);

            if (selectedTemplate == null)
            {
                return string.Empty;
            }

            var templateVariables = _emailServices.GetAllTenantEmailVariables(tenantId);

            if (selectedTemplate != null)
            {
                var emailHeader = GetTranslatedString(selectedTemplate.HtmlHeader, templateVariables, notificationItem, accountId, userId, ConfirmatonLink);
                var emailBody = GetTranslatedString(selectedTemplate.Body, templateVariables, notificationItem, accountId, userId, ConfirmatonLink);
                var emailFooter = GetTranslatedString(selectedTemplate.HtmlFooter, templateVariables, notificationItem, accountId, userId, ConfirmatonLink);

                return emailHeader + "<br/>" + emailBody + "<br/>" + emailFooter;

            }

            return string.Empty;
        }

        public async Task<string> CreateTenantEmailNotificationQueue(string subject,
            OrderViewModel order, string attachmentVirtualPath = null, bool sendImmediately = true,
            DateTime? scheduleStartTime = null, DateTime? scheduleEndTime = null, string scheduleResourceName = null,
            OrderRecipientInfo shipmentAndRecipientInfo = null,
            WorksOrderNotificationTypeEnum worksOrderNotificationType =
                WorksOrderNotificationTypeEnum.WorksOrderLogTemplate, Appointments appointment = null, int TenantId = 0, int? accountId = null, string UserEmail = null, string confirmationLink = null, int? userId = null, int? siteId = null)
        {


            var emailconfig = _emailServices.GetEmailConfigurationsById(order?.TenentId ?? TenantId, siteId);
            if (emailconfig == null)
            {
                return "Could not send email, no email configuration found";
            }

            var suitableTemplate = _emailServices.GetSuitableEmailTemplate(worksOrderNotificationType, (order?.TenentId ?? TenantId), siteId);
            if (suitableTemplate == null)
            {
                return "Could not send email, no suitable template found";
            }


            var dispatchTime = DateTime.UtcNow.Date;
            int hour = 18;
            int minute = 0;
            if (emailconfig.DailyEmailDispatchTime != null)
            {
                hour = int.Parse(emailconfig.DailyEmailDispatchTime.Split(':')[0]);
                minute = int.Parse(emailconfig.DailyEmailDispatchTime.Split(':')[1]);
            }

            dispatchTime = dispatchTime.AddHours(hour);
            dispatchTime = dispatchTime.AddMinutes(minute);

            TenantEmailNotificationQueue notificationQueue = UpdateNotificationQueue(subject, order, attachmentVirtualPath, sendImmediately, scheduleStartTime, scheduleEndTime, scheduleResourceName, shipmentAndRecipientInfo,
                worksOrderNotificationType, appointment, emailconfig, dispatchTime, suitableTemplate.TemplateId);
            if (accountId.HasValue)
            {
                var result = await DispatchMailNotification(notificationQueue, (order?.TenentId ?? TenantId), worksOrderNotificationType, sendImmediately, ((accountId.HasValue) ? accountId : null), userId, UserEmail, confirmationLink, siteId);
                return result;
            }
            else
            {
                var result = await DispatchMailNotification(notificationQueue, (order?.TenentId ?? TenantId), worksOrderNotificationType, sendImmediately, ((worksOrderNotificationType == WorksOrderNotificationTypeEnum.InvoiceTemplate) ? order.AccountID : null));
                return result;
            }


        }

        private TenantEmailNotificationQueue UpdateNotificationQueue(string subject, OrderViewModel order, string attachmentVirtualPath, bool sendImmediately, DateTime? scheduleStartTime, DateTime? scheduleEndTime, string scheduleResourceName,
            OrderRecipientInfo shipmentAndRecipientInfo, WorksOrderNotificationTypeEnum worksOrderNotificationType, Appointments appointment, TenantEmailConfig emailconfig, DateTime dispatchTime, int templateId)
        {
            var notificationQueue = new TenantEmailNotificationQueue()
            {
                TenantEmailTemplatesId = templateId,
                OrderId = order?.OrderID,
                EmailSubject = subject,
                CustomEmailMessage = shipmentAndRecipientInfo?.CustomMessage ?? "",
                IsNotificationCancelled = (worksOrderNotificationType == WorksOrderNotificationTypeEnum.WorksOrderBlankList),
                AttachmentVirtualPath = attachmentVirtualPath,
                WorkOrderStartTime = scheduleStartTime,
                WorkOrderEndTime = scheduleEndTime,
                WorksOrderResourceName = scheduleResourceName,
                ScheduledProcessingTime = dispatchTime,
                ScheduledProcessing = !sendImmediately,
                ProcessedImmediately = sendImmediately,
                InvoiceMasterId = order?.InvoiceId,
            };

            if (shipmentAndRecipientInfo != null)
            {
                notificationQueue.CustomRecipients = shipmentAndRecipientInfo.CustomRecipients;
                notificationQueue.CustomCcRecipients = shipmentAndRecipientInfo.CustomCCRecipients;
                notificationQueue.CustomBccRecipients = shipmentAndRecipientInfo.CustomBCCRecipients;

                if (shipmentAndRecipientInfo.PropertyTenantIds != null)
                {
                    foreach (var item in shipmentAndRecipientInfo.PropertyTenantIds)
                    {
                        if (item != "")
                        {
                            _emailServices.AddPropertyTenantRecipients(order.OrderID, shipmentAndRecipientInfo.PPropertyID, item.AsInt(), notificationQueue);
                        }
                    }
                }
            }
            else
            {
                if (appointment != null)
                {
                    notificationQueue.AppointmentId = appointment.AppointmentId;
                    notificationQueue.WorkOrderStartTime = appointment.StartTime;
                    notificationQueue.WorkOrderEndTime = appointment.EndTime;
                    if (appointment.ResourceId.HasValue)
                    {
                        var resource = _employeeServices.GetAppointmentResourceById(appointment.ResourceId ?? 0);
                        if (resource != null)
                        {
                            notificationQueue.WorksOrderResourceName = resource.Name;
                        }
                    }
                }

            }

            notificationQueue = _emailServices.SaveEmailNotificationQueue(_mapper.Map(notificationQueue, new TenantEmailNotificationQueueViewModel()));
            return notificationQueue;
        }

        public async Task<string> DispatchTenantEmailNotificationQueues(int tenantId)
        {
            var targetTime = DateTime.Now.AddMinutes(5);
            var awaitingMailNotifications = _emailServices.GetAllTenantEmailNotificationQueuesAwaitProcessing(targetTime);
            try
            {
                foreach (var notification in awaitingMailNotifications)
                {
                    await DispatchMailNotification(notification, tenantId, sendImmediately: true, worksOrderNotificationType: WorksOrderNotificationTypeEnum.WorksOrderScheduledTemplate);
                }
            }
            catch (Exception ex)
            {
                return "Failure sending e-mail : " + ex.Message;
            }

            return "Success";
        }

        public async Task<string> SendRegistrationEmail(int tenantId, WorksOrderNotificationTypeEnum worksOrderNotificationType, string subject, string ConfirmatonLink, string recipients, int? accountId, bool salesRequiresAuthorisation = true, int userID = 0)
        {
            var mailmsg = new MailMessage();
            var smtp = new SmtpClient();
            Attachment attachment = null;


            try
            {
                var tenantConfig = _tenantServices.GetTenantConfigById(tenantId);

                mailmsg = new MailMessage();
                mailmsg.ReplyToList.Add(new MailAddress(tenantConfig.DefaultReplyToAddress));

                var recipientEmails = new List<MailAddress>();

                if (!string.IsNullOrEmpty(recipients))
                {
                    recipientEmails.AddRange(recipients.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(m => new MailAddress(m)));
                }

                if (recipientEmails.Any() && tenantConfig.EnableLiveEmails == true)
                {
                    recipientEmails.CopyItemsTo(mailmsg.To);
                }

                if (!string.IsNullOrEmpty(tenantConfig.WarehouseLogEmailsToDefault))
                {
                    var emails = tenantConfig.WarehouseLogEmailsToDefault.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var email in emails)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            mailmsg.Bcc.Add(new MailAddress(email));
                        }
                    }
                }

                if (mailmsg.To.Count < 1 && mailmsg.Bcc.Count < 1 && mailmsg.CC.Count < 1)
                {
                    return "No recipient found!";
                }

                var emailconfig = _emailServices.GetAllActiveTenantEmailConfigurations(tenantId).FirstOrDefault();
                if (emailconfig == null)
                {
                    return "No email configuration found!";
                }

                if (salesRequiresAuthorisation && !string.IsNullOrEmpty(tenantConfig.AuthorisationAdminEmail))
                {
                    mailmsg.To.Add(tenantConfig.AuthorisationAdminEmail);
                }


                ConfirmatonLink = "<a href='" + ConfirmatonLink + "'>Activate Account</a>";
                mailmsg.Body = TranslateEmailTemplateForConfirmationEmail(tenantId, worksOrderNotificationType, accountId, userID);
                mailmsg.Body = mailmsg.Body.Replace("{ConfirmationLink}", ConfirmatonLink);
                mailmsg.IsBodyHtml = true;
                mailmsg.From = new MailAddress(emailconfig.UserEmail, tenantConfig.DefaultMailFromText);
                mailmsg.Subject = subject;

                smtp.Host = emailconfig.SmtpHost.Trim();
                smtp.Port = emailconfig.SmtpPort;
                smtp.EnableSsl = emailconfig.EnableSsl;

                if (emailconfig.EnableRelayEmailServer != true)
                {
                    smtp.Credentials = new NetworkCredential(emailconfig.UserEmail.Trim(), emailconfig.Password.Trim());
                }

                await Task.Run(() =>
                {
                    smtp.Send(mailmsg);
                });

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return ex.Message;
            }
            finally
            {
                if (attachment != null) { attachment.Dispose(); }
                mailmsg.Dispose();
                smtp.Dispose();
            }

            return "Success";

        }

        private string GetTranslatedString(string htmlContent, List<TenantEmailTemplateVariable> templateVariables, TenantEmailNotificationQueue notification, int? accountID, int? userId = null, string confirmationLink = null)
        {
            var account = _accountServices.GetAccountsById(accountID ?? 0);
            if (string.IsNullOrEmpty(htmlContent)) return string.Empty;

            var order = _orderService.GetOrderById(notification?.OrderId ?? 0);
            if (order != null)
            {
                account = order.Account;
            }
            var totalVariable = new List<string>();
            var variablesToReplace = templateVariables.Select(m => "{ " + m.VariableName + " }")
                .Where(htmlContent.Contains)
                .ToList();
            variablesToReplace.ForEach(u => totalVariable.Add(u.ToString()));
            variablesToReplace = templateVariables.Select(m => "{" + m.VariableName + "}")
              .Where(htmlContent.Contains)
              .ToList();
            variablesToReplace.ForEach(u => totalVariable.Add(u.ToString()));

            var translatedResult = htmlContent;

            foreach (var variable in totalVariable)
            {
                var replaceWith = "";
                try
                {

                    switch (variable)
                    {
                        case "{ CompanyName }":
                        case "{CompanyName}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.CompanyName);
                            break;
                        case "{ AccountCode }":
                        case "{AccountCode}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.AccountCode);
                            break;
                        case "{ AccountRemittancesContactName }":
                        case "{AccountRemittancesContactName}":
                            replaceWith =
                                GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.AccountRemittancesContactName);
                            break;
                        case "{ AccountStatementsContactName }":
                        case "{AccountStatementsContactName}":
                            replaceWith =
                                GetMailMergeVariableValueFromOrder(order, account,
                                    MailMergeVariableEnum.AccountStatementsContactName);
                            break;
                        case "{ AccountInvoicesContactName }":
                        case "{AccountInvoicesContactName}":
                            replaceWith =
                                GetMailMergeVariableValueFromOrder(order, account,
                                    MailMergeVariableEnum.AccountInvoicesContactName);
                            break;
                        case "{ AccountMarketingContactName }":
                        case "{AccountMarketingContactName}":
                            replaceWith =
                                GetMailMergeVariableValueFromOrder(order, account,
                                    MailMergeVariableEnum.AccountMarketingContactName);
                            break;
                        case "{ OrderId }":
                        case "{OrderId}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.OrderId);
                            break;
                        case "{ OrderNumber }":
                        case "{OrderNumber}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.OrderNumber);
                            break;
                        case "{ OrderStatus }":
                        case "{OrderStatus}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.OrderStatus);
                            break;
                        case "{ BillingAccountToEmail }":
                        case "{BillingAccountToEmail}":
                            replaceWith =
                                GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.BillingAccountToEmail);
                            break;
                        case "{ WorksOrderResourceName }":
                        case "{WorksOrderResourceName}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                MailMergeVariableEnum.WorksOrderResourceName, notification);
                            break;
                        case "{ WorksOrderTimeslot }":
                        case "{WorksOrderTimeslot}":
                            replaceWith =
                                GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.WorksOrderTimeslot,
                                    notification);
                            break;
                        case "{ ScheduledDate }":
                        case "{ScheduledDate}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.ScheduledDate,
                                notification);
                            break;
                        case "{ WorksTenantName }":
                        case "{WorksTenantName}":
                            replaceWith =
                                GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.WorksTenantName,
                                    notification);
                            break;
                        case "{ WorkPropertyAddress }":
                        case "{WorkPropertyAddress}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                MailMergeVariableEnum.WorkPropertyAddress, notification);
                            break;
                        case "{ WorksJobTypeDescription }":
                        case "{WorksJobTypeDescription}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                MailMergeVariableEnum.WorksJobTypeDescription, notification);
                            break;
                        case "{ WorksJobSubTypeDescription }":
                        case "{WorksJobSubTypeDescription}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                MailMergeVariableEnum.WorksJobSubTypeDescription, notification);
                            break;
                        case "{ WorksSlaJobPriorityName }":
                        case "{WorksSlaJobPriorityName}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                MailMergeVariableEnum.WorksSlaJobPriorityName, notification);
                            break;
                        case "{ WorksPropertyContactNumber }":
                        case "{WorksPropertyContactNumber}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                MailMergeVariableEnum.WorksPropertyContactNumber, notification);
                            break;
                        case "{ CustomMessage }":
                        case "{CustomMessage}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                MailMergeVariableEnum.CustomMessage, notification);
                            break;
                        case "{ AccountPurchasingContactName }":
                        case "{AccountPurchasingContactName}":
                            replaceWith =
                                GetMailMergeVariableValueFromOrder(order, account,
                                    MailMergeVariableEnum.AccountPurchasingContactName);
                            break;
                        case "{ UserName }":
                        case "{UserName}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                    MailMergeVariableEnum.UserName, null, (userId ?? 0));
                            break;
                        case "{ ConfirmationLink }":
                        case "{ConfirmationLink}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                    MailMergeVariableEnum.ConfirmationLink, null, (userId ?? 0), confirmationLink);
                            break;
                        case "{ TransactionReferenceNumber }":
                        case "{TransactionReferenceNumber}":
                            replaceWith = GetMailMergeVariableValueFromOrder(order, account,
                                MailMergeVariableEnum.TransactionReferenceNumber, null, (userId ?? 0), confirmationLink);
                            break;

                        default:
                            replaceWith = string.Empty;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    throw ex;
                }

                translatedResult = translatedResult.Replace(variable, replaceWith);
            }

            return translatedResult;
        }

        public string GetMailMergeVariableValueFromOrder(Order order, Account account, MailMergeVariableEnum variableType, TenantEmailNotificationQueue notificationItem = null, int? UserId = null, string confirmationLink = null)
        {
            TenantConfig config = null;
            if (order != null)
            {
                order = _orderService.GetOrderById(order.OrderID);
                config = _tenantServices.GetTenantConfigById(order.TenentId);
                if (notificationItem == null)
                {
                    notificationItem = new TenantEmailNotificationQueue() { Order = order };
                }
            }
            var variableValue = " ";

            try
            {
                switch (variableType)
                {
                    case MailMergeVariableEnum.CompanyName:
                        variableValue = account?.CompanyName;
                        break;
                    case MailMergeVariableEnum.CustomMessage:
                        variableValue = notificationItem?.CustomEmailMessage;
                        break;
                    case MailMergeVariableEnum.AccountCode:
                        variableValue = account?.AccountCode;
                        break;
                    case MailMergeVariableEnum.AccountRemittancesContactName:
                        var executiveContact = account?.AccountContacts?.FirstOrDefault(m => m.ConTypeRemittance);
                        variableValue = executiveContact != null ? executiveContact.ContactName : GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.CompanyName, notificationItem);
                        break;

                    case MailMergeVariableEnum.AccountStatementsContactName:
                        var adminContact = account?.AccountContacts.FirstOrDefault(m => m.ConTypeStatment);
                        variableValue = adminContact != null ? adminContact.ContactName : GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.CompanyName, notificationItem);
                        break;

                    case MailMergeVariableEnum.AccountInvoicesContactName:
                        var billingContact = account?.AccountContacts?.FirstOrDefault(m => m.ConTypeInvoices);
                        variableValue = billingContact != null ? billingContact.ContactName : GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.CompanyName, notificationItem);
                        break;

                    case MailMergeVariableEnum.AccountMarketingContactName:
                        var marketingContact = account?.AccountContacts?.FirstOrDefault(m => m.ConTypeMarketing);
                        variableValue = marketingContact != null ? marketingContact.ContactName : string.Empty;
                        break;

                    case MailMergeVariableEnum.OrderId:
                        variableValue = order?.OrderID.ToString();
                        break;

                    case MailMergeVariableEnum.OrderNumber:
                        variableValue = order?.OrderNumber;
                        break;

                    case MailMergeVariableEnum.OrderStatus:
                        variableValue = order?.OrderStatusID.ToString();
                        break;

                    case MailMergeVariableEnum.BillingAccountToEmail:
                        var billingEmailContact = account?.AccountContacts?.FirstOrDefault(m => m.ConTypeInvoices);
                        variableValue = billingEmailContact != null
                            ? billingEmailContact.ContactEmail
                            : account.AccountEmail;
                        break;

                    case MailMergeVariableEnum.WorksOrderResourceName:
                        if (notificationItem != null)
                        {
                            if (notificationItem.AppointmentId != null && notificationItem.AppointmentId > 0)
                            {
                                var appointment = _appointmentServices.GetAppointmentById(notificationItem.AppointmentId ?? 0);
                                if (appointment != null)
                                {
                                    variableValue = appointment.AppointmentResources.Name;
                                }
                            }
                        }
                        break;

                    case MailMergeVariableEnum.WorksOrderTimeslot:
                        if (notificationItem != null)
                        {
                            if (notificationItem.AppointmentId != null && notificationItem.AppointmentId > 0)
                            {
                                var appointment = _appointmentServices.GetAppointmentById(notificationItem.AppointmentId ?? 0);
                                if (appointment != null)
                                {
                                    if (config.WorksOrderScheduleByAmPm == true || config.WorksOrderScheduleByMarginHours == null || config.WarehouseScheduleEndTime == null)
                                    {
                                        variableValue = $" for {appointment.StartTime:dd/MM/yyyy} " + ((appointment.StartTime.Hour < 13) ? "AM" : "PM");
                                    }
                                    else
                                    {
                                        var dayStartArray = config.WarehouseScheduleStartTime.Split(':');
                                        var dayStart = appointment.StartTime.Date.AddHours(int.Parse(dayStartArray[0]));
                                        dayStart = dayStart.AddMinutes(int.Parse(dayStartArray[1]));

                                        var dayEndArray = config.WarehouseScheduleEndTime.Split(':');
                                        var dayEnd = appointment.StartTime.Date.AddHours(int.Parse(dayEndArray[0]));
                                        dayEnd = dayEnd.AddMinutes(int.Parse(dayEndArray[1]));

                                        var resourceArrivalStart = appointment.StartTime.AddHours(-config.WorksOrderScheduleByMarginHours.Value);
                                        if (resourceArrivalStart < dayStart)
                                        {
                                            resourceArrivalStart = dayStart;
                                        }

                                        var resourceArrivalEnd = appointment.StartTime.AddHours(config.WorksOrderScheduleByMarginHours.Value);
                                        if (resourceArrivalEnd > dayEnd)
                                        {
                                            resourceArrivalEnd = dayEnd;
                                        }

                                        variableValue = $" between {resourceArrivalStart:dd/MM/yyyy HH:mm} and {resourceArrivalEnd:dd/MM/yyyy HH:mm}";
                                    }
                                }
                            }
                        }
                        break;

                    case MailMergeVariableEnum.ScheduledDate:
                        if (notificationItem != null)
                        {
                            if (notificationItem.AppointmentId != null && notificationItem.AppointmentId > 0)
                            {
                                var appointment = _appointmentServices.GetAppointmentById(notificationItem.AppointmentId ?? 0);
                                if (appointment != null)
                                {
                                    variableValue = appointment.StartTime.ToString("dd/MMMM/yyy");
                                }
                            }
                        }
                        break;

                    case MailMergeVariableEnum.WorksTenantName:
                        var recipients = _emailServices.GetAllPropertyTenantRecipients(order?.OrderID ?? 0);
                        var headRecipient = recipients.FirstOrDefault(m => m.PTenant != null && m.PTenant.IsHeadTenant);
                        var tenantName = headRecipient != null ? headRecipient.PTenant.TenantFullName : "Tenant";
                        if (headRecipient == null && recipients.Count() > 0 && recipients.First().PTenantId.HasValue)
                        {
                            var tenant = _propertyService.GetPropertyTenantById(recipients.First().PTenantId.Value);
                            tenantName = tenant.TenantFullName;
                        }
                        variableValue = tenantName;
                        break;

                    case MailMergeVariableEnum.WorkPropertyAddress:
                        variableValue = _propertyService.GetPropertyById(order?.PPropertyId ?? 0)?.FullAddress;
                        break;

                    case MailMergeVariableEnum.WorksJobTypeDescription:
                        variableValue = order?.JobType?.Name;
                        break;

                    case MailMergeVariableEnum.WorksJobSubTypeDescription:
                        variableValue = order?.JobSubType?.Name;
                        break;

                    case MailMergeVariableEnum.WorksSlaJobPriorityName:
                        variableValue = order?.SLAPriority?.Priority;
                        break;

                    case MailMergeVariableEnum.WorksPropertyContactNumber:
                        var recipientContacts = _emailServices.GetAllPropertyTenantRecipients(order?.OrderID ?? 0);
                        variableValue = string.Join(";", recipientContacts.Select(m => m?.PTenant?.MobileNumber));
                        break;

                    case MailMergeVariableEnum.AccountPurchasingContactName:
                        var PurchasingContact = account?.AccountContacts?.FirstOrDefault(m => m.ConTypePurchasing);
                        variableValue = PurchasingContact != null ? PurchasingContact.ContactName : GetMailMergeVariableValueFromOrder(order, account, MailMergeVariableEnum.CompanyName, notificationItem);
                        break;

                    case MailMergeVariableEnum.UserName:
                        var username = GetUserName(UserId);
                        variableValue = username;
                        break;

                    case MailMergeVariableEnum.ConfirmationLink:
                        variableValue = confirmationLink;
                        break;

                    case MailMergeVariableEnum.TransactionReferenceNumber:
                        variableValue = _accountServices.GetTransactionNumberByOrderId(order?.OrderID ?? 0);
                        break;

                    default:
                        variableValue = string.Empty;
                        break;
                }

            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
            return variableValue;
        }

        public string TranslateEmailTemplateForConfirmationEmail(int tenantId, WorksOrderNotificationTypeEnum worksOrderNotificationType = WorksOrderNotificationTypeEnum.WorksOrderLogTemplate, int? accountId = null, int? UserId = 0)
        {


            var allEmailTemplates = _emailServices.GetAllTenantEmailTemplates(tenantId);
            var selectedTemplate = allEmailTemplates.FirstOrDefault(m => m.NotificationType == worksOrderNotificationType);

            if (selectedTemplate == null)
            {
                return string.Empty;
            }

            var templateVariables = _emailServices.GetAllTenantEmailVariables(tenantId);

            if (selectedTemplate != null)
            {
                var emailHeader = GetTranslatedString(selectedTemplate.HtmlHeader, templateVariables, null, accountId, (UserId ?? 0));
                var emailBody = GetTranslatedString(selectedTemplate.Body, templateVariables, null, accountId, (UserId ?? 0));
                var emailFooter = GetTranslatedString(selectedTemplate.HtmlFooter, templateVariables, null, accountId, (UserId ?? 0));

                return emailHeader + "<br/>" + emailBody + "<br/>" + emailFooter;

            }

            return string.Empty;
        }

        // po close overload method for Web Api calls
        public bool IsPOComplete(int? POID, int UserId, int warehouseId)
        {
            var pdetial = _orderService.GetAllValidOrderDetailsByOrderId(POID ?? 0);

            foreach (var item in pdetial)
            {
                decimal qtyrec = _orderService.GetAllOrderProcessesByOrderDetailId(item.OrderDetailID, warehouseId).Sum(x => (decimal?)x.QtyProcessed) ?? 0;
                if (item.Qty > qtyrec)
                    return false;
            }

            _orderService.UpdateOrderStatus(POID ?? 0, OrderStatusEnum.Complete, UserId);
            return true;
        }
        public bool ActiveStocktake(int warehouseId)
        {
            Boolean status = false;

            var model = _stockTakeService.GetStockTakesInProgress(warehouseId).ToList();

            if (!model.Any())
            {
                status = true;
            }

            return status;

        }

        public bool SendMail(string subject, string body, int tenantId)
        {
            bool res = true;

            try
            {
                Tenant tenant = new Tenant();
                TenantEmailConfig emailConfig = new TenantEmailConfig();
                tenant = _tenantServices.GetByClientId(tenantId);
                if (tenant != null && tenant.TenantEmail != string.Empty && tenant.TenantEmailConfig.Count > 0)
                {
                    emailConfig = tenant.TenantEmailConfig.FirstOrDefault();

                    var fromAddress = new MailAddress(emailConfig.UserEmail);
                    var toAddress = new MailAddress(tenant.TenantEmail);
                    string fromPassword = emailConfig.Password;

                    var smtp = new SmtpClient
                    {
                        Host = emailConfig.SmtpHost,
                        Port = emailConfig.SmtpPort,
                        EnableSsl = emailConfig.EnableSsl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(message);
                    }
                }

                else
                {
                    res = false;
                }
            }

            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                res = false;
            }

            return res;
        }

        public string GetPropertyFirstline(int pPropertyId)
        {
            return _propertyService.GetPropertyById(pPropertyId).AddressLine1;
        }

        // return time zone id to convert time with daylight saving changes
        public static TimeZoneInfo TimeZoneId()
        {
            string zone = "GMT Standard Time";
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(zone);
            return tz;

        }

        // check stocktake status
        public Boolean GetStStatus(int id)
        {
            Boolean Status = false;

            // change status in database
            StockTake model = _stockTakeService.GetStockTakeById(id);

            if (model != null && model.Status == 1)
            {
                Status = true;
            }

            return Status;
        }

        public string GetStStatusString(int StatusCode)
        {
            string Status = "";

            switch (StatusCode)
            {
                case 1:
                    Status = "Stopped, changes not yet applied";
                    break;
                case 2:
                    Status = "Changes Applied";
                    break;
                case 3:
                    Status = "cancelled";
                    break;
                default:
                    Status = "Running";
                    break;
            }


            // return user name
            return Status;
        }

        public string GetDeviceLastIp(string serial)
        {
            TerminalsLog device = _terminalServices.GetTerminalLogBySerial(serial).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            return device?.clientIp ?? "";
        }

        public string GetDeviceLastPingDate(string serial)
        {
            TerminalsLog device = _terminalServices.GetTerminalLogBySerial(serial).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            return device?.DateCreated.ToString() ?? "";
        }

        public bool GetDeviceCurrentStatus(string serial)
        {
            TerminalsLog deviceLog = _terminalServices.GetTerminalLogBySerial(serial).OrderByDescending(x => x.DateCreated).FirstOrDefault();

            DateTime currerntTime = DateTime.UtcNow;
            DateTime lastPingTime = deviceLog?.DateCreated ?? DateTime.MinValue;
            TimeSpan span = currerntTime.Subtract(lastPingTime);
            double diff = span.TotalMinutes;

            return !(diff > 3);
        }

        public string GetUserName(int? userid = 0)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            string UserName = "";

            if (userid > 0)
            {
                //get user name against id
                AuthUser user = new AuthUser();
                user = context.AuthUsers.FirstOrDefault(m => m.UserId == userid.Value);
                if (user != null)
                {
                    UserName = user.UserFirstName;
                }
            }
            // return user name
            return UserName;
        }

        public string GetActionResultHtml(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData,
                    controller.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                return sw.ToString();
            }
        }
    }

    public class HtmlEditorModel
    {
        public string EditorName { get; set; }
        public string CallbackController { get; set; }
        public string CallbackAction { get; set; }

        public int Height { get; set; } = 360;
        public int Width { get; set; } = 690;
        public string HtmlContent { get; set; }
    }

    public static class LinqHelpers
    {
        public static IEnumerable<IEnumerable<T>> Batches<T>(this IEnumerable<T> items, int maxItems)
        {
            return items.Select((item, inx) => new { item, inx })
                .GroupBy(x => x.inx / maxItems)
                .Select(g => g.Select(x => x.item));
        }
    }
}