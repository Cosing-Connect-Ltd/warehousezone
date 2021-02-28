using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Ganedata.Core.Services;
using WarehouseEcommerce.Models;

namespace WarehouseEcommerce.Controllers
{
    public class PagesController : BaseController
    {
        public PagesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices, ITenantWebsiteService tenantWebsiteService) : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices, tenantWebsiteService)
        {

        }

        [HttpPost]
        public ActionResult GetStarted(DemoBooking model)
        {
            if (ModelState.IsValid)
            {
                var contactEmail = ConfigurationManager.AppSettings["ContactFormEmailAddress"] ??
                                   "info@ganedata.co.uk";
                if (CurrentTenantWebsite != null &&
                    !string.IsNullOrWhiteSpace(CurrentTenantWebsite.WebsiteContactEmail))
                {
                    contactEmail = CurrentTenantWebsite.WebsiteContactEmail;
                }

                var body =
                    "<p><strong>Name:</strong> {0} <br/> <strong> Phone: </strong> {1} <br/> <strong> Email: </strong> {2} <br/> <strong> Message: </strong> {3}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(contactEmail));
                message.From = new MailAddress(model.Email);
                message.Subject = "Demo Booking Request";
                message.Body = string.Format(body, model.Name, model.Phone, model.Email, model.Message);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["SmtpClientUserName"],
                        Password = ConfigurationManager.AppSettings["SmtpClientPassword"]
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    try
                    {
                        smtp.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Session["EXCP"] = ex;
                        //Url.Action("page","Home",new {pageUrl=item.pageUrl,BlogDetail="true"})
                        return RedirectToAction("Page", "Home", new { pageUrl = "get-started" });
                    }

                    Session["success"] = 1;
                    return RedirectToAction("Page", "Home", new { pageUrl = "get-started" });
                }
            }

            return View(model);
        }

        public ActionResult GetStarted()
        {
            return View();
        }


    }
}