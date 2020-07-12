using AutoMapper;
using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
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

        public ActionResult MobileCheckout()
        {
            var checkoutViewModel = new CheckoutViewModel();

            checkoutViewModel.CurrentStep = CheckoutStep.DeliveryMethod;

            checkoutViewModel.StepsHistory.Add(checkoutViewModel.CurrentStep.Value);

            Session["CheckoutViewModel"] = checkoutViewModel;

            return RedirectToAction("Checkout");
        }

        public ActionResult Checkout(CheckoutStep? step)
        {
            if (CurrentUser?.UserId <= 0)
            {
                return RedirectToAction("Login", "User", new { PlaceOrder = true });
            }

            var checkoutViewModel = (CheckoutViewModel)Session["CheckoutViewModel"];

            if(checkoutViewModel == null || checkoutViewModel.CurrentStep == null || (!CurrentTenantWebsite.IsCollectionAvailable && !CurrentTenantWebsite.IsDeliveryAvailable))
            {
                return RedirectToAction("AddToCart", "Products", new { PlaceOrder = true });
            }

            if(step != null)
            {
                checkoutViewModel.CurrentStep = step.Value;
            }
            else {
                checkoutViewModel.noTrackStep = true;
            }

            return View(checkoutViewModel);
        }

        public PartialViewResult _CheckoutProcessPartial(CheckoutViewModel checkoutViewModel, CheckoutStep? step)
        {
            checkoutViewModel.CurrentStep = step ?? checkoutViewModel.CurrentStep;
            var model = SetCheckoutProcessViewModel(checkoutViewModel);

            Session["CheckoutViewModel"] = model;
            return PartialView(model);
        }

        public ActionResult GoToPreviousStep()
        {
            var checkoutViewModel = (CheckoutViewModel)Session["CheckoutViewModel"];

            if (checkoutViewModel.StepsHistory?.Count > 0) {
                checkoutViewModel.StepsHistory.RemoveAt(checkoutViewModel.StepsHistory.Count - 1);
            }

            if (checkoutViewModel == null || checkoutViewModel.StepsHistory?.Count == 0)
            {
                return RedirectToAction("AddToCart", "Products", new { PlaceOrder = true });
            }

            checkoutViewModel.CurrentStep = checkoutViewModel.StepsHistory.LastOrDefault();

            Session["CheckoutViewModel"] = checkoutViewModel;

            return RedirectToAction("Checkout");
        }

        public ActionResult ProceedToCheckout(DeliveryMethod deliveryMethod, int? destinationId)
        {
            var checkoutViewModel = new CheckoutViewModel();

            checkoutViewModel.SetInitialStep(deliveryMethod);

            checkoutViewModel.DeliveryMethodId = (int)deliveryMethod;

            if (deliveryMethod == DeliveryMethod.ToPickupPoint) {
                if (!CurrentTenantWebsite.IsCollectionAvailable)
                {
                    return RedirectToAction("AddToCart", "Products", new { PlaceOrder = true });
                }
                checkoutViewModel.CurrentStep = destinationId > 0 ? CheckoutStep.BillingAddress : CheckoutStep.CollectionPoint;
                checkoutViewModel.CollectionPointId = destinationId;
            }
            else if (deliveryMethod == DeliveryMethod.ToShipmentAddress)
            {
                if (!CurrentTenantWebsite.IsDeliveryAvailable)
                {
                    return RedirectToAction("AddToCart", "Products", new { PlaceOrder = true });
                }
                checkoutViewModel.CurrentStep = destinationId > 0 ? CheckoutStep.ShipmentRule : CheckoutStep.ShippingAddress;
                checkoutViewModel.ShippingAddressId = destinationId;
            }

            Session["CheckoutViewModel"] = checkoutViewModel;

            return RedirectToAction("Checkout", new { step = (int)checkoutViewModel.CurrentStep });
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

        public ActionResult SaveAddress(CheckoutViewModel checkoutViewModel)
        {
            var accountAddresses = _mapper.Map(checkoutViewModel.AccountAddress, new AccountAddresses());
            accountAddresses.Name = "Ecommerce";
            if (accountAddresses.AccountID <= 0)
            {
                accountAddresses.AccountID = caCurrent.CurrentWebsiteUser().AccountId ?? 0;
            }

            AccountServices.SaveAccountAddress(accountAddresses, CurrentUserId == 0 ? 1 : CurrentUserId);

            checkoutViewModel = (CheckoutViewModel)Session["CheckoutViewModel"];

            if (accountAddresses.AddressID > 0 && checkoutViewModel?.CurrentStep == CheckoutStep.AddOrEditAddress)
            {
                return RedirectToAction("GoToPreviousStep");
            }

            var nextStep = CheckoutStep.ShipmentRule;

            if (accountAddresses.AddTypeBilling == true)
            {
                checkoutViewModel.BillingAddressId = accountAddresses.AddressID;
                nextStep = CheckoutStep.PaymentMethod;
            }

            if (accountAddresses.AddTypeShipping == true)
            {
                checkoutViewModel.ShippingAddressId = accountAddresses.AddressID;
            }

            if (checkoutViewModel.StepsHistory?.LastOrDefault() == CheckoutStep.AddOrEditAddress)
            {
                checkoutViewModel.StepsHistory.RemoveAt(checkoutViewModel.StepsHistory.Count - 1);
            }

            Session["CheckoutViewModel"] = checkoutViewModel;

            return RedirectToAction("Checkout", new { step = nextStep });
        }

        public ActionResult RemoveShippingAddress(int accountAddressId)
        {
            AccountServices.DeleteAccountAddress(accountAddressId, (CurrentUserId == 0 ? 1 : CurrentUserId));
            return RedirectToAction("Checkout");
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

        public ActionResult PayPal()
        {
            var checkOutModel = Session["CheckoutViewModel"] == null
                ? new CheckoutViewModel()
                : Session["CheckoutViewModel"] as CheckoutViewModel;
            return View(checkOutModel);
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
            var checkOutModel = Session["CheckoutViewModel"] == null
                ? new CheckoutViewModel()
                : Session["CheckoutViewModel"] as CheckoutViewModel;
            return View(checkOutModel);
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
                    CreateOrder(responseString);
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
            model.BillingAddressId = checkoutViewModel.BillingAddressId <= 0 ? null : (model.BillingAddressId ?? checkoutViewModel.BillingAddressId);
            model.ShipmentRuleId = checkoutViewModel.ShipmentRuleId ?? (model.ShipmentRuleId ?? checkoutViewModel.ShipmentRuleId);
            model.DeliveryMethodId = checkoutViewModel.DeliveryMethodId ?? (model.DeliveryMethodId ?? checkoutViewModel.DeliveryMethodId);
            model.CollectionPointId = checkoutViewModel.CollectionPointId ?? (model.CollectionPointId ?? checkoutViewModel.CollectionPointId);
            model.Countries = _lookupServices.GetAllGlobalCountries().Select(u => new CountryViewModel { CountryId = u.CountryID, CountryName = u.CountryName }).ToList();
            model.CurrencySymbol = checkoutViewModel.CurrencySymbol ?? (Session["CurrencyDetail"] != null ? (Session["CurrencyDetail"] as caCurrencyDetail).Symbol : string.Empty);
            model.AccountAddressId = checkoutViewModel.AccountAddressId;
            model.StepsHistory = checkoutViewModel.StepsHistory != null && checkoutViewModel.StepsHistory.Count > 0 ? checkoutViewModel.StepsHistory : model.StepsHistory;

            SetStepHistory(model);

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

        private static void SetStepHistory(CheckoutViewModel model)
        {
            if (model.noTrackStep != true && model.CurrentStep != null && model.StepsHistory.LastOrDefault() != model.CurrentStep)
            {
                model.StepsHistory.Add(model.CurrentStep.Value);
            }

            model.noTrackStep = null;
        }

        public ActionResult CreateOrder(string sagePayReponse=null, string paypalTransactionId=null)
        {
            var checkoutModel = new CheckoutViewModel();
            if (Session["CheckoutViewModel"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            checkoutModel= Session["CheckoutViewModel"] as CheckoutViewModel;
            if (!string.IsNullOrEmpty(sagePayReponse))
            {
                checkoutModel.SagePayPaymentResponse = JsonConvert.DeserializeObject<SagePayPaymentResponseViewModel>(sagePayReponse);
                Session["CheckoutViewModel"] = OrderService.CreateShopOrder(checkoutModel, CurrentTenantId, CurrentUserId, CurrentWarehouseId, CurrentTenantWebsite.SiteID);
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(paypalTransactionId))
            {
                checkoutModel.SagePayPaymentResponse.transactionId = paypalTransactionId;
                Session["CheckoutViewModel"] = OrderService.CreateShopOrder(checkoutModel, CurrentTenantId, CurrentUserId, CurrentWarehouseId, CurrentTenantWebsite.SiteID);
                return Json(true,JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);




        }
    }
}