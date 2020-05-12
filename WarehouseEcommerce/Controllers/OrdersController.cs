﻿using AutoMapper;
using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WarehouseEcommerce.Helpers;

namespace WarehouseEcommerce.Controllers
{
    public class OrdersController : BaseController
    {

        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsServices _tenantServices;
        private readonly IProductLookupService _productlookupServices;
        private readonly ILookupServices _lookupServices;
        private readonly ICoreOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IGaneConfigurationsHelper _configurationsHelper;
        string PAYPAL_RET_URL = System.Configuration.ConfigurationManager.AppSettings["PAYPAL_RET_URL"] != null
                                           ? System.Configuration.ConfigurationManager.AppSettings["PAYPAL_RET_URL"] : "";
        string PAYPAL_URL = System.Configuration.ConfigurationManager.AppSettings["PAYPAL_URL"] != null
                                        ? System.Configuration.ConfigurationManager.AppSettings["PAYPAL_URL"] : "";

        public OrdersController(IProductServices productServices, IProductLookupService productlookupServices, IProductPriceService productPriceService, ICommonDbServices commonDbServices, ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices, IUserService userService, IActivityServices activityServices, ITenantsServices tenantServices, IMapper mapper, IGaneConfigurationsHelper configurationsHelper)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices)
        {
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
            _productlookupServices = productlookupServices;
            _lookupServices = lookupServices;
            _orderService = orderService;
            _mapper = mapper;
            _configurationsHelper = configurationsHelper;


        }
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAddress(int? AccountId, int? AccountAddressId, int? AccountBillingId, int? AccountShippingId, int? ShippmentTypeId, bool ShipingAddress = false)
        {
            AccountShippingId = AccountShippingId <= 0? null: AccountShippingId;
            AccountBillingId = AccountBillingId <= 0 ? null : AccountBillingId;
            if (GaneCartItemsSessionHelper.GetCartItemsSession().Count > 0)
            {
                if (CurrentUser?.UserId <= 0)
                {
                    return RedirectToAction("Login", "User", new { PlaceOrder = true });
                }
                ViewBag.cart = true;
                ViewBag.AccountIds = AccountId = CurrentUser?.AccountId;
                ViewBag.Country = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName");
                ViewBag.ShipingAddressId = AccountShippingId;
                ViewBag.BillingAddressId = AccountBillingId;
                ViewBag.Shiping = ShipingAddress;
                ViewBag.ShippmentMethodType = ShippmentTypeId;
                //Billing Address Section
                if (!ShipingAddress)
                {

                    if (AccountAddressId.HasValue)
                    {
                        var model = AccountServices.GetAccountAddressById(AccountAddressId ?? 0);

                        return View(model);
                    }
                    var billingAddress = AccountServices.GetAllValidAccountAddressesByAccountId(AccountId ?? 0).Where(u => u.AddTypeBilling == true).ToList();
                    if (billingAddress.Count > 0)
                    {
                        ViewBag.BillingAddress = true;

                        ViewBag.Addresses = billingAddress;

                    }
                    ViewBag.AddressMessage = "Billing Address";

                }
                // Shipping Address Section
                else if (AccountBillingId.HasValue && ShipingAddress)
                {

                    if (AccountAddressId.HasValue)
                    {
                        var model = AccountServices.GetAccountAddressById(AccountAddressId ?? 0);
                        return View(model);
                    }
                    ViewBag.Addresses = AccountServices.GetAllValidAccountAddressesByAccountId(AccountId ?? 0).Where(u => u.AddTypeShipping == true).ToList();
                    ViewBag.AddressMessage = "Shipping Address";


                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View();

        }

        public PartialViewResult _PaymentAndShipmentMethods()
        {

            return PartialView();
        }




        public ActionResult SaveAddress(AccountAddresses accountAddresses)
        {
            accountAddresses.Name = "Ecommerce";
            if (accountAddresses.AccountID <= 0)
            {
                accountAddresses.AccountID = caCurrent.CurrentWebsiteUser().AccountId ?? 0;
            }
            AccountServices.SaveAccountAddress(accountAddresses, CurrentUserId == 0 ? 1 : CurrentUserId);
            if (accountAddresses.AddTypeShipping == true)
            {
                return RedirectToAction("GetAddress", new { AccountId = accountAddresses.AccountID, AccountBillingId=AccountServices.GetAllValidAccountAddressesByAccountId(accountAddresses.AccountID).FirstOrDefault()?.AddressID, ShipingAddress=true });
            }

            return RedirectToAction("GetAddress", new { AccountId = accountAddresses.AccountID});

        }

        public ActionResult RemoveAddress(int AccountAddressId)
        {
            var accountAddresses = AccountServices.DeleteAccountAddress(AccountAddressId, (CurrentUserId == 0 ? 1 : CurrentUserId));
            return RedirectToAction("GetAddress", new { AccountId = accountAddresses.AccountID, AccountBillingId = AccountServices.GetAllValidAccountAddressesByAccountId(accountAddresses.AccountID).FirstOrDefault()?.AddressID, ShipingAddress = true });
        }

        public ActionResult ConfirmOrder(int accountId, int ShipmentAddressId, int ShippmentTypeId,int PaymentTypeId)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.cart = true;
            ViewBag.CartModal = true;
            ViewBag.paymentMethod = PaymentTypeId;
            List<AccountAddresses> accountAddresses = new List<AccountAddresses>();
            accountAddresses.Add(AccountServices.GetAccountAddressById(ShipmentAddressId));
            accountAddresses.Add(AccountServices.GetAllValidAccountAddressesByAccountId(accountId).FirstOrDefault(u => u.AddTypeBilling == true));
            ViewBag.Addresses = accountAddresses;
            var models = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();
            var orders = OrderService.CreateShopOrder(accountAddresses.FirstOrDefault().AccountID, _mapper.Map(models, new List<OrderDetail>()), 1, 1, 1);
            ViewBag.TotalQty = Math.Round(((models.Sum(u => u.TotalAmount)) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
            ViewBag.Symbol = currencyyDetail.Symbol;
            ViewBag.ShipmentMethod = ShippmentTypeId;
            ViewBag.RetUrl = PAYPAL_RET_URL;
            ViewBag.PAYPALURL = PAYPAL_URL;
            ViewBag.OrdersId = orders.OrderID;
            return View(models);
        }

        public async Task<JsonResult> GetApiAddressAsync(string postCode)
        {
            DataImportFactory dataImportFactory = new DataImportFactory();
            var addresses = await dataImportFactory.GetAddressByPostCodeAsync(postCode);
            
            return Json(addresses, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddShippingMethod(int AccountAddressId)
        {
            ViewBag.cart = true;
            ViewBag.AccountAddressId = AccountAddressId;

            return View();
        }

        public async Task<ActionResult> IPN(FormCollection form)

        {
            var datas = Request.QueryString.Get("tx");
            var propertyId = 0;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var ipn = Request.Form.AllKeys.ToDictionary(k => k, k => Request[k]);
            ipn.Add("cmd", "_notify-validate");
            var isIpnValid = await ValidateIpnAsync(ipn);

            if (isIpnValid && ipn["custom"] == "Test")
            {
                return RedirectToAction("Success", new { id = -10 });
            }
            if (!isIpnValid && ipn["custom"] == "Test")
            {
                return RedirectToAction("Failed", new { id = -10 });
            }

            var custom = ipn["custom"].Split('|');
            propertyId = Convert.ToInt32(custom[0]);

            //OrderService.UpdateOrderStatus( )
            //var report = CreateSalesOrderPrint(Order.OrderID);
            //PrepareDirectory("~/UploadedFiles/reports/so/");
            //var reportPath = "~/UploadedFiles/reports/so/" + Order.OrderNumber + ".pdf";
            //report.ExportToPdf(Server.MapPath(reportPath));
            //var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Sales order", _mapper.Map(Order, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
            //    worksOrderNotificationType: (WorksOrderNotificationTypeEnum)EmailTemplate);
            //if (result != "Success")
            //{
            //    TempData["Error"] = result;
            //}









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



    }
}