using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WarehouseEcommerce.Models;

namespace WarehouseEcommerce.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult GetStarted()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetStarted(DemoBooking model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p><strong>Name:</strong> {0} <br/> <strong> Phone: </strong> {1} <br/> <strong> Email: </strong> {2} <br/> <strong> Message: </strong> {3}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(ConfigurationManager.AppSettings["ContactFormEmailAddress"]));
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
                        return RedirectToAction("GetStarted", "Home", new { area = "" });
                    }


                    Session["success"] = 1;
                    return RedirectToAction("GetStarted", "Home", new { area = "" });
                }
            }
            return View(model);
        }

        public ActionResult Blog()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Promotions()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult WarehouseManagement()
        {
            return View();
        }

        public ActionResult OrderManagement()
        {
            return View();
        }

        public ActionResult StockControl()
        {
            return View();
        }

        public ActionResult Epod()
        {
            return View();
        }

        public ActionResult FleetTrackingDriverManagement()
        {
            return View();
        }

        public ActionResult YardManagement()
        {
            return View();
        }

        public ActionResult Ecommerce()
        {
            return View();
        }

        public ActionResult EposSelfService()
        {
            return View();
        }

        public ActionResult MobileVanSales()
        {
            return View();
        }

        public ActionResult RealTimeLocationSystem()
        {
            return View();
        }

        public ActionResult HumanResources()
        {
            return View();
        }

        public ActionResult MobileDeviceManagement()
        {
            return View();
        }

        public ActionResult DigitalSignage()
        {
            return View();
        }

        public ActionResult BusinessIntelligence()
        {
            return View();
        }

        public ActionResult LossPrevention()
        {
            return View();
        }

        public ActionResult RetailSolutions()
        {
            return View();
        }

        public ActionResult News()
        {
            return View();
        }

        public ActionResult CourierIntegration()
        {
            return View();
        }


    }
}