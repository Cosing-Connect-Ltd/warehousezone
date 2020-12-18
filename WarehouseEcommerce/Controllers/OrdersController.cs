using AutoMapper;
using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
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
using Ganedata.Core.Models.AdyenPayments;
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
        private readonly IMapper _mapper;
        private readonly IGaneConfigurationsHelper _configurationsHelper;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IAdyenPaymentService _adyenPaymentService;
        private readonly string _paypalClientId;
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
                                ITenantWebsiteService tenantWebsiteService, IAdyenPaymentService adyenPaymentService
                                )
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices, tenantWebsiteService)
        {
            _paypalClientId = ConfigurationManager.AppSettings["PaypalClientId"] ?? "";
            _productServices = productServices;
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
            _productlookupServices = productlookupServices;
            _mapper = mapper;
            _configurationsHelper = configurationsHelper;
            _tenantLocationServices = tenantLocationServices;
            _tenantWebsiteService = tenantWebsiteService;
            _adyenPaymentService = adyenPaymentService;
        }
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MobileCheckout()
        {
            var checkoutViewModel = new CheckoutViewModel();

            if (CurrentTenantWebsite.IsCollectionAvailable && CurrentTenantWebsite.IsDeliveryAvailable)
            {
                checkoutViewModel.CurrentStep = CheckoutStep.DeliveryMethod;
            }
            else
            {
                checkoutViewModel.CurrentStep = CurrentTenantWebsite.IsDeliveryAvailable ? CheckoutStep.ShippingAddress : CheckoutStep.CollectionPoint;
                checkoutViewModel.DeliveryMethodId = CurrentTenantWebsite.IsDeliveryAvailable ? (int)DeliveryMethod.ToShipmentAddress : (int)DeliveryMethod.ToPickupPoint;
            }

            checkoutViewModel.StepsHistory.Add(checkoutViewModel.CurrentStep.Value);

            Session["CheckoutViewModel"] = checkoutViewModel;

            return RedirectToAction("Checkout");
        }

        public ActionResult Checkout(CheckoutStep? step)
        {
            var checkoutViewModel = (CheckoutViewModel)Session["CheckoutViewModel"];

            if (checkoutViewModel == null || checkoutViewModel.CurrentStep == null || (!CurrentTenantWebsite.IsCollectionAvailable && !CurrentTenantWebsite.IsDeliveryAvailable))
            {
                return RedirectToAction("AddToCart", "Products", new { PlaceOrder = true });
            }

            if (step != null)
            {
                checkoutViewModel.CurrentStep = step.Value;
            }
            else
            {
                checkoutViewModel.noTrackStep = true;
            }

            ViewBag.Title = $"{CurrentTenantWebsite.SiteName} Checkout";

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

            if (checkoutViewModel.StepsHistory?.Count > 0)
            {
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

            if (deliveryMethod == DeliveryMethod.ToPickupPoint)
            {
                if (!CurrentTenantWebsite.IsCollectionAvailable)
                {
                    return RedirectToAction("AddToCart", "Products", new { PlaceOrder = true });
                }
                checkoutViewModel.CurrentStep = destinationId > 0 ? CheckoutStep.PaymentDetails : CheckoutStep.CollectionPoint;
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

            if (!dataImportFactory.IsValidUkPostcode(postCode)) {
                return Json("InvalidPostCode", JsonRequestBehavior.AllowGet);
            }

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
                                                                                      (!string.IsNullOrEmpty(warehouse?.City?.Trim()) ? $", {warehouse?.City}" : string.Empty),
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

        public JsonResult GetDeliveryAvailabilityStatus(string postCode)
        {
            var dataImportFactory = new DataImportFactory();

            if (!dataImportFactory.IsValidUkPostcode(postCode))
            {
                return Json("InvalidPostCode", JsonRequestBehavior.AllowGet);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
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
            else
            {
                accountAddresses.SessionId = HttpContext.Session.SessionID;
            }

            accountAddresses = _accountServices.SaveAccountAddress(accountAddresses, CurrentUserId == 0 ? 1 : CurrentUserId);

            checkoutViewModel = (CheckoutViewModel)Session["CheckoutViewModel"];

            var nextStep = CheckoutStep.ShipmentRule;

            if (accountAddresses.AddTypeBilling == true)
            {
                checkoutViewModel.BillingAddressId = accountAddresses.AddressID;
                checkoutViewModel.IsAddressSameForBilling = false;
                nextStep = CheckoutStep.PaymentDetails;
            }

            if (accountAddresses.AddTypeShipping == true)
            {
                checkoutViewModel.ShippingAddressId = accountAddresses.AddressID;
            }


            if (accountAddresses.AddressID > 0 && checkoutViewModel?.CurrentStep == CheckoutStep.AddOrEditAddress)
            {
                Session["CheckoutViewModel"] = checkoutViewModel;
                return RedirectToAction("GoToPreviousStep");
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
            _accountServices.DeleteAccountAddress(accountAddressId, (CurrentUserId == 0 ? 1 : CurrentUserId));
            return RedirectToAction("Checkout");
        }

        public ActionResult ConfirmOrder()
        {
            var model = Session["CheckoutViewModel"] as CheckoutViewModel;
            model = _tenantWebsiteService.SetCheckOutProcessModel(model, CurrentTenantWebsite.SiteID, CurrentTenantId, CurrentUserId, Session.SessionID);
            ViewBag.cart = true;
            ViewBag.CartModal = true;
            ViewBag.paymentMethod = model.PaymentMethodId;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmOrder(CheckoutViewModel data)
        {
            var model = Session["CheckoutViewModel"] as CheckoutViewModel;
            model = _tenantWebsiteService.SetCheckOutProcessModel(model, CurrentTenantWebsite.SiteID, CurrentTenantId, CurrentUserId, Session.SessionID);

            if (CurrentUser?.AccountId == null)
            {
                model.Email = data.Email;

                if (data.CreateAccount == true)
                {
                    CreateNewUserAccount(data, model);
                }
            }

            try
            {
                Session["CheckoutViewModel"] = model;
                if (Session["_AdyenPaylink"] == null)
                {
                    var paymentLink = await _adyenPaymentService.GenerateOrderPaymentLink(
                        new AdyenCreatePayLinkRequestModel()
                        {
                            Amount = new AdyenAmount() {Value = model.TotalOrderAmount},
                            MerchantAccount = AdyenPaymentService.AdyenMerchantAccountName,
                            OrderDescription = model.CartItems.First().ProductMaster.Description,
                            PaymentReference = Guid.NewGuid().ToString("N"),
                            ShopperUniqueReference = model.OrderNumber + "_" + DateTime.Now.ToFileTime()
                        });
                    Session["_AdyenPaylink"] = paymentLink.Url;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(model.OrderNumber), "Order ");
                return View("ConfirmOrder", model);
            }

            switch (model.PaymentMethodId)
            {
                case (int)PaymentMethodEnum.PayPal:
                    return RedirectToAction("PayPal");
                case (int)PaymentMethodEnum.SagePay:
                    return RedirectToAction("SagePay");
                default:
                    return null;
            }
        }

        private void CreateNewUserAccount(CheckoutViewModel data, CheckoutViewModel model)
        {
            var user = _userService.GetAuthUserByUserName(data.Email, CurrentTenantId);
            if (user != null)
            {
                model.AccountId = user.AccountId;
                return;
            }

            model.UserFirstName = data.UserFirstName;
            model.UserLastName = data.UserLastName;
            var accountId = _accountServices.CreateNewAccountForEcommerceUser(data.UserFirstName + GaneStaticAppExtensions.GenerateRandomNo(), CurrentUserId, CurrentTenantId);
            var userId = _userService.CreateNewEcommerceUser(data.Email, data.UserFirstName, data.UserLastName, data.UserPassword, accountId, CurrentTenantWebsite.SiteID, CurrentTenantId, CurrentUserId);
            SendUserRegistrationNotification(userId, data.Email, accountId);

            model.AccountId = accountId;
        }

        private void SendUserRegistrationNotification(int userId, string userEmail, int accountId)
        {
            var baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            string confirmationLink = Url.Action("ConfirmUsers", "User", new { confirmationValue = GaneStaticAppExtensions.HashPassword(userId.ToString()) + "_" + userId.ToString(), placeOrder = "" });
            confirmationLink = baseUrl + confirmationLink;
            confirmationLink = "<a href='" + confirmationLink + "' class=btn btn-primary>Activate Account</a>";
            _configurationsHelper.CreateTenantEmailNotificationQueue("Activate your account", null, sendImmediately: true, worksOrderNotificationType: WorksOrderNotificationTypeEnum.EmailConfirmation, TenantId: CurrentTenantId, accountId: accountId, UserEmail: userEmail, confirmationLink: confirmationLink, userId: userId, siteId: CurrentTenantWebsite.SiteID);
        }

        public JsonResult SetPaymentDetails(int paymentMethodId, bool isAddressSameForBilling)
        {
            var model = new CheckoutViewModel();
            if (Session["CheckoutViewModel"] != null)
            {
                model = Session["CheckoutViewModel"] as CheckoutViewModel;
            }

            model.PaymentMethodId = paymentMethodId;
            model.IsAddressSameForBilling = isAddressSameForBilling;
            if (isAddressSameForBilling)
            {
                model.BillingAddressId = model.ShippingAddressId;
            }

            Session["CheckoutViewModel"] = model;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetApiAddressAsync(string postCode)
        {
            DataImportFactory dataImportFactory = new DataImportFactory();
            var addresses = await dataImportFactory.GetAddressByPostCodeAsync(postCode);

            return Json(addresses, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PayPal()
        {
            var checkOutModel = Session["CheckoutViewModel"] == null
                ? new CheckoutViewModel()
                : Session["CheckoutViewModel"] as CheckoutViewModel;

            checkOutModel.PaypalClientId = _paypalClientId;
            return View(checkOutModel);
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
                return RedirectToAction("Index", "Home");
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

            model.UserFirstName = CurrentUser?.UserFirstName;
            model.UserLastName = CurrentUser?.UserLastName;
            model.Email = CurrentUser?.UserEmail;
            model.CurrentStep = checkoutViewModel.CurrentStep ?? model.CurrentStep;
            model.AccountId = checkoutViewModel.AccountId ?? (model.AccountId ?? CurrentUser.AccountId);
            model.ShippingAddressId = checkoutViewModel.ShippingAddressId <= 0 ? null : (model.ShippingAddressId ?? checkoutViewModel.ShippingAddressId);
            model.BillingAddressId = checkoutViewModel.BillingAddressId <= 0 ? null : (model.BillingAddressId ?? checkoutViewModel.BillingAddressId);
            model.ShipmentRuleId = checkoutViewModel.ShipmentRuleId ?? (model.ShipmentRuleId ?? checkoutViewModel.ShipmentRuleId);
            model.DeliveryMethodId = checkoutViewModel.DeliveryMethodId ?? (model.DeliveryMethodId ?? checkoutViewModel.DeliveryMethodId);
            model.CollectionPointId = checkoutViewModel.CollectionPointId ?? (model.CollectionPointId ?? checkoutViewModel.CollectionPointId);
            model.Countries = _lookupServices.GetAllGlobalCountries().Select(u => new CountryViewModel { CountryId = u.CountryID, CountryName = u.CountryName }).ToList();
            model.AccountAddressId = checkoutViewModel.AccountAddressId;
            model.StepsHistory = checkoutViewModel.StepsHistory != null && checkoutViewModel.StepsHistory.Count > 0 ? checkoutViewModel.StepsHistory : model.StepsHistory;

            if (model.DeliveryMethodId != null)
            {
                model.IsAddressSameForBilling = model.IsAddressSameForBilling ?? model.DeliveryMethodId == (int)DeliveryMethod.ToShipmentAddress;
            }

            if (model.IsAddressSameForBilling == true)
            {
                model.BillingAddressId = model.ShippingAddressId;
            }

            SetStepHistory(model);

            switch (model.CurrentStep)
            {
                case CheckoutStep.PaymentDetails:
                    model.Addresses = _mapper.Map(_accountServices.GetAllValidAccountAddressesByAccountIdOrSessionKey(model.AccountId ?? 0, Session.SessionID).Where(u => (model.IsAddressSameForBilling == true || !model.BillingAddressId.HasValue || u.AddressID == model.BillingAddressId) && u.AddTypeBilling == true).ToList(), new List<AddressViewModel>());
                    model.AccountAddress = model.Addresses.FirstOrDefault();
                    model.BillingAddressId = model.AccountAddress?.AddressID;
                    model.PaymentMethodId = (int)PaymentMethodEnum.PayPal;
                    break;
                case CheckoutStep.ShippingAddress:
                    model.ShippingAddressId = null;
                    model.Addresses = _mapper.Map(_accountServices.GetAllValidAccountAddressesByAccountIdOrSessionKey(model.AccountId ?? 0, Session.SessionID).Where(u => (!model.ShippingAddressId.HasValue || u.AddressID == model.ShippingAddressId) && u.AddTypeShipping == true && u.IsDeleted != true).ToList(), new List<AddressViewModel>());
                    break;
                case CheckoutStep.AddOrEditAddress:
                    model.AccountAddress = _mapper.Map(_accountServices.GetAccountAddressById(model.AccountAddressId ?? 0), new AddressViewModel());
                    break;
                case CheckoutStep.ShipmentRule:
                    var cartItems = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, Session.SessionID);
                    var parcelWeightInGrams = cartItems.Sum(i => i.Weight);
                    model.ShippingRules = _tenantWebsiteService.GetShippingRulesByShippingAddressId(CurrentTenantId, CurrentTenantWebsite.SiteID, model.ShippingAddressId.Value, parcelWeightInGrams);
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

        public ActionResult CreateOrder(string sagePayReponse = null, string paypalTransactionId = null)
        {
            var checkoutModel = new CheckoutViewModel();
            if (Session["CheckoutViewModel"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            checkoutModel = Session["CheckoutViewModel"] as CheckoutViewModel;
            if (!string.IsNullOrEmpty(sagePayReponse))
            {
                checkoutModel.SagePayPaymentResponse = JsonConvert.DeserializeObject<SagePayPaymentResponseViewModel>(sagePayReponse);
                var order = base._orderService.CreateShopOrder(checkoutModel, CurrentTenantId, CurrentUserId, CurrentWarehouseId, CurrentTenantWebsite.SiteID);
                checkoutModel.OrderNumber = order.OrderNumber;
                _configurationsHelper.CreateTenantEmailNotificationQueue("Order Confirmed", _mapper.Map(order, new OrderViewModel()), sendImmediately: true, worksOrderNotificationType: WorksOrderNotificationTypeEnum.WebsiteOrderConfirmation, TenantId: CurrentTenantId, accountId: order.AccountID, UserEmail: CurrentUser.UserEmail, userId: CurrentUser.UserId, siteId: CurrentTenantWebsite.SiteID);
                Session["CheckoutViewModel"] = checkoutModel;
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(paypalTransactionId))
            {
                checkoutModel.SagePayPaymentResponse.transactionId = paypalTransactionId;
                var order = base._orderService.CreateShopOrder(checkoutModel, CurrentTenantId, CurrentUserId, CurrentWarehouseId, CurrentTenantWebsite.SiteID);
                checkoutModel.OrderNumber = order.OrderNumber;
                _configurationsHelper.CreateTenantEmailNotificationQueue("Order Confirmed", _mapper.Map(order, new OrderViewModel()), sendImmediately: true, worksOrderNotificationType: WorksOrderNotificationTypeEnum.WebsiteOrderConfirmation, TenantId: CurrentTenantId, accountId: order.AccountID, UserEmail: CurrentUser.UserEmail, userId: CurrentUser.UserId, siteId: CurrentTenantWebsite.SiteID);
                Session["CheckoutViewModel"] = checkoutModel;
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        #region Payments
        public ActionResult ReviewOrder()
        {
            var model = new ReviewOrderViewModel
            {
                CartItems = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, HttpContext.Session.SessionID).ToList(),
            };

            foreach (var item in model.CartItems)
            {
                model.RelatedProducts.AddRange(_productServices.GetRelatedProductsByProductId(item.ProductId, CurrentTenantId, CurrentTenantWebsite.SiteID));
            }

            return View("Payments/ReviewOrder", model);
        }
        #endregion
    }
}