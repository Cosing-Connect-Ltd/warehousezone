using AutoMapper;
using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ganedata.Core.Models;
using WarehouseEcommerce.Helpers;
using PaymentMethod = Ganedata.Core.Entities.Domain.ViewModels.PaymentMethod;

namespace WarehouseEcommerce.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsServices _tenantServices;
        private readonly IProductLookupService _productlookupServices;
        private readonly ILookupServices _lookupServices;
        private readonly ICoreOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IGaneConfigurationsHelper _configurationsHelper;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly string _paypalReturnUrl;
        private readonly string _paypalUrl;
        private readonly string _paypalIpnUrl;

        public OrdersController(IProductServices productServices,
                                IProductLookupService productlookupServices,
                                ICoreOrderService orderService,
                                IPropertyService propertyService,
                                IAccountServices accountServices,
                                ILookupServices lookupServices,
                                ITenantsCurrencyRateServices tenantsCurrencyRateServices,
                                IUserService userService,
                                IActivityServices activityServices,
                                ITenantsServices tenantServices,
                                IMapper mapper,
                                IGaneConfigurationsHelper configurationsHelper,
                                ITenantLocationServices tenantLocationServices,
                                ITenantWebsiteService tenantWebsiteService
                                )
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices)
        {
            _paypalReturnUrl = ConfigurationManager.AppSettings["PAYPAL_RET_URL"] ?? "";
            _paypalUrl = ConfigurationManager.AppSettings["PAYPAL_URL"] ?? "";
            _paypalIpnUrl = ConfigurationManager.AppSettings["PAYPAL_IPN_URL"] ?? "";
            _productServices = productServices;
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
            _productlookupServices = productlookupServices;
            _lookupServices = lookupServices;
            _orderService = orderService;
            _mapper = mapper;
            _configurationsHelper = configurationsHelper;
            _tenantLocationServices = tenantLocationServices;
            _tenantWebsiteService = tenantWebsiteService;
        }
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Checkout()
        {
            if (CurrentUser?.UserId <= 0)
            {
                return RedirectToAction("Login", "User", new { PlaceOrder = true });
            }

            return View((Session["CheckoutViewModel"] == null ? new CheckoutViewModel() : Session["CheckoutViewModel"] as CheckoutViewModel));
        }

        public PartialViewResult _CheckoutProcessPartial(CheckoutViewModel checkoutViewModel, CheckoutStep? checkoutStep)
        {
            checkoutViewModel.CurrentStep = checkoutStep ?? checkoutViewModel.CurrentStep;
            var model = SetCheckoutProcessViewModel(checkoutViewModel);
            Session["CheckoutViewModel"] = model;
            return PartialView(model);
        }

        public async Task<JsonResult> GetNearWarehouses(string postCode)
        {
            var warehouses = _lookupServices.GetAllWarehousesForTenant(CurrentTenantId);

            var dataImportFactory = new DataImportFactory();

            var distances = await dataImportFactory.GetDistancesFromPostcode(postCode, warehouses.Select(w => w.PostalCode).ToList());

            if (distances.Status != "OK")
            {
                return null;
            }

            var warehousesByDistance = distances.Destinations.Select((x, i) =>
                                                                    {
                                                                        var warehouse = warehouses[i];
                                                                        return new
                                                                        {
                                                                            warehouse?.WarehouseName,
                                                                            warehouse?.WarehouseId,
                                                                            warehouse?.PostalCode,
                                                                            Address = warehouse?.AddressLine1 +
                                                                                      (!string.IsNullOrEmpty(warehouse?.AddressLine2?.Trim()) ? $", {warehouse?.AddressLine2}" : string.Empty) +
                                                                                      (!string.IsNullOrEmpty(warehouse?.AddressLine3?.Trim()) ? $", {warehouse?.AddressLine3}" : string.Empty) +
                                                                                      (!string.IsNullOrEmpty(warehouse?.AddressLine4?.Trim()) ? $", {warehouse?.AddressLine4}" : string.Empty),
                                                                            warehouse?.City,
                                                                            warehouse?.GlobalCountry?.CountryName,
                                                                            warehouse?.ContactNumbers,
                                                                            Distance = distances.Rows[0]?.Elements[i],
                                                                        };
                                                                    })
                                                                .OrderBy(w => w.Distance?.Distance?.Value)
                                                                .ToList();

            return Json(warehousesByDistance, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _PaymentAndShipmentMethods()
        {
            return PartialView();
        }

        public ActionResult SaveAddress(CheckoutViewModel checkoutViewModel, int? deliveryMethodId, int? billingAddressId)
        {
            var accountAddresses = _mapper.Map(checkoutViewModel.AccountAddress, new AccountAddresses());
            accountAddresses.Name = "Ecommerce";
            if (accountAddresses.AccountID <= 0)
            {
                accountAddresses.AccountID = caCurrent.CurrentWebsiteUser().AccountId ?? 0;
            }

            AccountServices.SaveAccountAddress(accountAddresses, CurrentUserId == 0 ? 1 : CurrentUserId);

            if (accountAddresses.AddTypeShipping == true)
            {
                return RedirectToAction("Checkout", new { checkoutStep = CheckoutStep.ShippingAddress });
            }

            return RedirectToAction("Checkout", new { checkoutStep = CheckoutStep.BillingAddress });
        }

        public ActionResult RemoveShippingAddress(int accountAddressId, int billingAddressId, int accountId, int deliveryMethodId)
        {
            var accountAddresses = AccountServices.DeleteAccountAddress(accountAddressId, (CurrentUserId == 0 ? 1 : CurrentUserId));
            return RedirectToAction("Checkout", new CheckoutViewModel() { AccountId = accountId, BillingAddressId = billingAddressId, DeliveryMethodId = deliveryMethodId, CurrentStep = CheckoutStep.ShippingAddress });
        }

        public ActionResult ConfirmOrder(int paymentTypeId)
        {

            var model = Session["CheckoutViewModel"] as CheckoutViewModel;
            model = _tenantWebsiteService.SetCheckOutProcessModel(model, CurrentTenantWebsite.SiteID, CurrentTenantId, CurrentUserId, Session.SessionID);
            model.PaymentMethodId = paymentTypeId;
            ViewBag.cart = true;
            ViewBag.CartModal = true;
            ViewBag.paymentMethod = paymentTypeId;
            ViewBag.RetUrl = _paypalReturnUrl;
            ViewBag.PAYPALURL = _paypalUrl;
            ViewBag.PayPalIpnUrl = _paypalIpnUrl;
            Session["AllCheckoutData"] = model;
            return View(model);
        }

        public async Task<JsonResult> GetApiAddressAsync(string postCode)
        {
            DataImportFactory dataImportFactory = new DataImportFactory();
            var addresses = await dataImportFactory.GetAddressByPostCodeAsync(postCode);

            return Json(addresses, JsonRequestBehavior.AllowGet);
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
                await client.PostAsync(_paypalUrl, new StringContent(string.Empty));
                var response = await client.PostAsync(_paypalUrl, new FormUrlEncodedContent(ipn));
                var responseString = await response.Content.ReadAsStringAsync();
                return (responseString == "VERIFIED");
            }
        }


        public async Task<ActionResult> SagePay()
        {
            SagepayTokenResponse resp = await GetSagePayToken();
            ViewBag.AuthCode = resp.merchantSessionKey;
            return View();
        }

        public async Task<ActionResult> ConfirmPayment()
        {
            var cardIdentifier = Request.Form["cardIdentifier"];
            var sessionKey = Request.Form["sessionKey"];

            var model = Session["CheckoutViewModel"] as CheckoutViewModel;

            var root = new SagePayPaymentViewModel();
            var address = new BillingAddress { address1 = "88", city = "Leeds", country = "GB", postalCode = "412" };
            Card card = new Card { cardIdentifier = cardIdentifier, save = "false", merchantSessionKey = sessionKey };
            PaymentMethod paymentMethod = new PaymentMethod { card = card };

            root.billingAddress = address;
            root.amount = (int)model.TotalOrderAmount * 100;
            root.currency = "GBP";
            root.transactionType = "Payment";
            root.paymentMethod = paymentMethod;
            root.customerFirstName = "Test";
            root.customerLastName = "Account";
            root.description = "Test Transaction";
            root.entryMethod = "Ecommerce";
            root.vendorTxCode = Guid.NewGuid().ToString();



            var json = JsonConvert.SerializeObject(root);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic aEpZeHN3N0hMYmo0MGNCOHVkRVM4Q0RSRkxodUo4RzU0TzZyRHBVWHZFNmhZRHJyaWE6bzJpSFNyRnliWU1acG1XT1FNdWhzWFA1MlY0ZkJ0cHVTRHNocktEU1dzQlkxT2lONmh3ZDlLYjEyejRqNVVzNXU=");
                var response = await client.PostAsync("https://pi-test.sagepay.com/api/v1/transactions", stringContent);
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    //TempData["Error"] = responseString;
                    TempData["Error"] = "Authorization failed, please check details and retry";

                    return RedirectToAction("ConfirmOrder", new { paymentTypeId = (int)PaymentMethodEnum.SagePay });
                    // redirect to action and show this message
                }
                else
                {
                    // create order
                    var responseString = await response.Content.ReadAsStringAsync();

                    var checkOutModel = Session["CheckoutViewModel"] == null
                        ? new CheckoutViewModel()
                        : Session["CheckoutViewModel"] as CheckoutViewModel;
                    checkOutModel.SagePayPaymentResponse = JsonConvert.DeserializeObject<SagePayPaymentResponseViewModel>(responseString);
                    Session["CheckoutViewModel"] = OrderService.CreateShopOrder(checkOutModel, CurrentTenantId, CurrentUserId, CurrentWarehouseId, CurrentTenantWebsite.SiteID);
                    return RedirectToAction("ConfirmPaymentMessage");
                }

            }
        }

        public ActionResult ConfirmPaymentMessage()
        {
            Console.WriteLine("3D confirmation");
            if (Session["CheckoutViewModel"] == null)
            {
                RedirectToAction("Index", "Home");
            }

            var checkOutModel = Session["CheckoutViewModel"] == null
                ? new CheckoutViewModel()
                : Session["CheckoutViewModel"] as CheckoutViewModel;

            Session["CheckoutViewModel"] = null;

            return View(checkOutModel);
        }

        private async Task<SagepayTokenResponse> GetSagePayToken()
        {
            var root = new SagepayVendor();
            root.vendorName = "sandbox";

            var json = JsonConvert.SerializeObject(root);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic aEpZeHN3N0hMYmo0MGNCOHVkRVM4Q0RSRkxodUo4RzU0TzZyRHBVWHZFNmhZRHJyaWE6bzJpSFNyRnliWU1acG1XT1FNdWhzWFA1MlY0ZkJ0cHVTRHNocktEU1dzQlkxT2lONmh3ZDlLYjEyejRqNVVzNXU=");
                var response = await client.PostAsync("https://pi-test.sagepay.com/api/v1/merchant-session-keys", stringContent);
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SagepayTokenResponse>(responseString);
            }
        }

        private CheckoutViewModel SetCheckoutProcessViewModel(CheckoutViewModel checkoutViewModel)
        {
            var model = new CheckoutViewModel();
            if (Session["CheckoutViewModel"] != null)
            {
                model = Session["CheckoutViewModel"] as CheckoutViewModel;


            }
            model.CurrentStep = checkoutViewModel.CurrentStep ?? model.CurrentStep;
            model.AccountId = checkoutViewModel.AccountId ?? (model.AccountId ?? CurrentUser.AccountId);
            model.ShippingAddressId = checkoutViewModel.ShippingAddressId <= 0 ? null : (model.ShippingAddressId ?? checkoutViewModel.ShippingAddressId);
            model.BillingAddressId =
                checkoutViewModel.BillingAddressId <= 0 ? null : (model.BillingAddressId ?? checkoutViewModel.BillingAddressId);
            model.ShipmentRuleId = checkoutViewModel.ShipmentRuleId ?? (model.ShipmentRuleId ?? checkoutViewModel.ShipmentRuleId);
            model.DeliveryMethodId = checkoutViewModel.DeliveryMethodId ?? (model.DeliveryMethodId ?? checkoutViewModel.DeliveryMethodId);
            model.CollectionPointId = checkoutViewModel.CollectionPointId ?? (model.CollectionPointId ?? checkoutViewModel.CollectionPointId);
            model.Countries = _lookupServices.GetAllGlobalCountries().Select(u => new CountryViewModel { CountryId = u.CountryID, CountryName = u.CountryName }).ToList();
            model.ParentStep = checkoutViewModel.ParentStep ?? model.ParentStep;
            model.AccountAddressId = checkoutViewModel.AccountAddressId;
            switch (model.CurrentStep)
            {
                case CheckoutStep.BillingAddress:
                    model.BillingAddressId = null;
                    model.Addresses = _mapper.Map(AccountServices.GetAllValidAccountAddressesByAccountId(model.AccountId ?? 0).Where(u => (!model.BillingAddressId.HasValue || u.AddressID == model.BillingAddressId) && u.AddTypeBilling == true).ToList(), new List<AddressViewModel>());
                    break;
                case CheckoutStep.ShippingAddress:
                    model.ShippingAddressId = null;
                    model.Addresses = _mapper.Map(AccountServices.GetAllValidAccountAddressesByAccountId(model.AccountId ?? 0).Where(u => (!model.ShippingAddressId.HasValue || u.AddressID == model.ShippingAddressId) && u.AddTypeShipping == true && u.IsDeleted != true).ToList(), new List<AddressViewModel>());
                    break;
                case CheckoutStep.AddOrEditAddress:
                    model.AccountAddress = _mapper.Map(AccountServices.GetAccountAddressById(model.AccountAddressId ?? 0), new AddressViewModel());
                    break;
                case CheckoutStep.ShipmentRule:
                    var cartItems = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, Session.SessionID);
                    var parcelWeightInGrams = cartItems.Sum(i => (i.KitProductCartItems?.Sum(ki => (ki.SimpleProductMaster?.Weight ?? 0)) ?? (i.ProductMaster?.Weight ?? 0)));
                    model.ShippingRules = _tenantWebsiteService.GetShippingRulesByShippingAddress(CurrentTenantId, CurrentTenantWebsite.SiteID, model.ShippingAddressId.Value, parcelWeightInGrams);
                    break;
            }

            return model;
        }
    }
}