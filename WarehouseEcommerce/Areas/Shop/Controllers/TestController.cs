using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WarehouseEcommerce.Areas.Shop.Controllers
{
    public class TestController : Controller
    {
        string PAYPAL_RET_URL = System.Configuration.ConfigurationManager.AppSettings["PAYPAL_RET_URL"] != null
                                            ? System.Configuration.ConfigurationManager.AppSettings["PAYPAL_RET_URL"] : "";
        string PAYPAL_URL = System.Configuration.ConfigurationManager.AppSettings["PAYPAL_URL"] != null
                                        ? System.Configuration.ConfigurationManager.AppSettings["PAYPAL_URL"] : "";
        
        // GET: Test
        public ActionResult Index()
        {
            ViewBag.cart = true;
            ViewBag.RetUrl = PAYPAL_RET_URL;
            ViewBag.PAYPALURL = PAYPAL_URL;
            var OrderDetailSession = Session["CartItemsSession"] as List<OrderDetailSessionViewModel> ?? new List<OrderDetailSessionViewModel>();
            return View(OrderDetailSession);
        }
        public async Task<ActionResult> IPN(FormCollection form)

        {
            //var datas = Request.QueryString.Get("tx");
            //var propertyId = 0;
            // var packageId = _adPackageService.GetAdPackages().FirstOrDefault(f => f.Name.Equals("CUSTOM", StringComparison.InvariantCulture)).Id;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var ipn = Request.Form.AllKeys.ToDictionary(k => k, k => Request[k]);

            WriteLog(string.Join(",", ipn.Values.ToList()));
            ipn.Add("cmd", "_notify-validate");
            var isIpnValid = await ValidateIpnAsync(ipn);

            if (isIpnValid && ipn["custom"] == "Test")
            {
                WriteLog("successfull return");
                return RedirectToAction("Success", new { id = -10 });


            }
            if (!isIpnValid && ipn["custom"] == "Test")
            {
                return RedirectToAction("Failed", new { id = -10 });
            }

            //var custom = ipn["custom"].Split('|');
            //propertyId = Convert.ToInt32(custom[0]);








            return View();
        }
        public ActionResult Success(int Id)

        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult Failed(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        private async Task<bool> ValidateIpnAsync(IEnumerable<KeyValuePair<string, string>> ipn)
        {
            using (var client = new HttpClient())
            {
                // This is necessary in order for PayPal to not resend the IPN.
                await client.PostAsync(PAYPAL_URL, new StringContent(string.Empty));
                var response = await client.PostAsync(PAYPAL_URL, new FormUrlEncodedContent(ipn));
                var responseString = await response.Content.ReadAsStringAsync();
                return (responseString == "VERIFIED");
            }

        }

        public static void WriteLog(string message)
        {
            try
            {
                var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFiles\\" +"paypal"+DateTime.UtcNow.ToString("ddMMMyyyy") + ".txt", true);
                var msg = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") + ": " + message;
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
                Console.WriteLine(msg);
            }
            catch (Exception ex)
            {
                var err = "Ganedata Sync Service - Writing logs :" + ex.Source;
                EventLog.WriteEntry(err, ex.Message);
                Console.WriteLine(err);
            }
        }
    }
}