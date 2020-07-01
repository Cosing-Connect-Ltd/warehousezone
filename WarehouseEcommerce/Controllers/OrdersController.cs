using AutoMapper;
using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Entities.Domain;
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
using WarehouseEcommerce.Helpers;

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
                                ITenantWebsiteService tenantWebsiteService)
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

        public ActionResult GetAddress(int? accountId, int? accountAddressId, int? billingAddressId, int? shippingAddressId, int? deliveryMethodId, int? collectionPointId, int? step, int? parentStep, int? shipmentRuleId)
        {
            if (GaneCartItemsSessionHelper.GetCartItemsSession().Count <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (CurrentUser?.UserId <= 0)
            {
                return RedirectToAction("Login", "User", new { PlaceOrder = true });
            }

            var currentStep = step != null ? (CheckoutStep)step : CheckoutStep.BillingAddress;

            ViewBag.cart = true;
            ViewBag.AccountIds = accountId = CurrentUser?.AccountId;
            ViewBag.Countries = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName");
            ViewBag.ShippingAddressId = shippingAddressId <= 0 ? null : shippingAddressId;
            ViewBag.BillingAddressId = billingAddressId <= 0 ? null : billingAddressId;
            ViewBag.ShipmentRuleId = shipmentRuleId;
            ViewBag.CurrentStep = currentStep;
            ViewBag.DeliveryMethodId = deliveryMethodId;
            ViewBag.CollectionPointId = collectionPointId;

            switch (currentStep)
            {
                case CheckoutStep.BillingAddress:
                    ViewBag.Addresses = AccountServices.GetAllValidAccountAddressesByAccountId(accountId ?? 0).Where(u => u.AddTypeBilling == true).ToList();
                    break;
                case CheckoutStep.ShippingAddress:
                    ViewBag.Addresses = AccountServices.GetAllValidAccountAddressesByAccountId(accountId ?? 0).Where(u => u.AddTypeShipping == true).ToList();
                    break;
                case CheckoutStep.EditAddress:
                    ViewBag.ParentStep = parentStep != null ? (CheckoutStep)parentStep : CheckoutStep.BillingAddress;
                    var model = AccountServices.GetAccountAddressById(accountAddressId ?? 0);
                    return View(model);
                case CheckoutStep.ShipmentRule:
                    var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
                    ViewBag.CurrencySymbol = currencyyDetail.Symbol;
                    var cartItems = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();
                    var parcelWeightInGrams = cartItems.Sum(i => (i.KitProductCartItems?.Sum(ki => (ki.SimpleProductMaster?.Weight ?? 0)) ?? (i.ProductMaster?.Weight ?? 0)));
                    var shippingRules = _tenantWebsiteService.GetShippingRulesByShippingAddress(CurrentTenantId, CurrentTenantWebsite.SiteID, shippingAddressId.Value, parcelWeightInGrams);
                    shippingRules.ForEach(r =>
                    {
                        r.Price = Math.Round(((r.Price) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
                    });

                    ViewBag.ShippingRules = shippingRules;

                    break;
            }

            return View();
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

            var cartItems = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();

            var WarehouseProductAvailabilities = cartItems.Select(a =>
                                                            {
                                                                var inventoryStocks = _productServices.GetAllInventoryStocksByProductId(a.ProductId);
                                                                return new
                                                                {
                                                                    WarehouseProductAvailability = inventoryStocks.Select(i => new
                                                                    {
                                                                        IsAvailable = i.Available >= a.Qty,
                                                                        i.WarehouseId,
                                                                        a.ProductMaster
                                                                    })
                                                                };
                                                            })
                                                    .SelectMany(a => a.WarehouseProductAvailability);

            var warehousesByDistance = distances.Destinations.Select((x, i) =>
                                                                    {
                                                                        var warehouse = warehouses[i];
                                                                        return new
                                                                        {
                                                                            warehouse?.WarehouseName,
                                                                            warehouse?.WarehouseId,
                                                                            warehouse?.PostalCode,
                                                                            warehouse?.AddressLine1,
                                                                            warehouse?.AddressLine2,
                                                                            warehouse?.AddressLine3,
                                                                            warehouse?.AddressLine4,
                                                                            warehouse?.City,
                                                                            warehouse?.GlobalCountry?.CountryName,
                                                                            warehouse?.ContactNumbers,
                                                                            Distance = distances.Rows[0].Elements[i],
                                                                            IsCartProductsAvailable = WarehouseProductAvailabilities.All(w => w.WarehouseId == warehouse?.WarehouseId && w.IsAvailable)
                                                                        };
                                                                    })
                                                                .OrderByDescending(w => w.IsCartProductsAvailable)
                                                                .ThenBy(w => w.Distance.Distance.Value)
                                                                .ToList();

            return Json(warehousesByDistance, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _PaymentAndShipmentMethods()
        {
            return PartialView();
        }




        public ActionResult SaveAddress(AccountAddresses accountAddresses, int? deliveryMethodId, int? billingAddressId)
        {
            accountAddresses.Name = "Ecommerce";
            if (accountAddresses.AccountID <= 0)
            {
                accountAddresses.AccountID = caCurrent.CurrentWebsiteUser().AccountId ?? 0;
            }
            AccountServices.SaveAccountAddress(accountAddresses, CurrentUserId == 0 ? 1 : CurrentUserId);
            if (accountAddresses.AddTypeShipping == true)
            {
                return RedirectToAction("GetAddress", new { accountId = accountAddresses.AccountID, billingAddressId, deliveryMethodId, step = (int)CheckoutStep.ShippingAddress });
            }

            return RedirectToAction("GetAddress", new { AccountId = accountAddresses.AccountID });

        }

        public ActionResult RemoveShippingAddress(int accountAddressId, int billingAddressId, int accountId, int deliveryMethodId)
        {
            var accountAddresses = AccountServices.DeleteAccountAddress(accountAddressId, (CurrentUserId == 0 ? 1 : CurrentUserId));
            return RedirectToAction("GetAddress", new { accountId, billingAddressId, deliveryMethodId, step = (int)CheckoutStep.ShippingAddress });
        }

        public ActionResult ConfirmOrder(int accountId, int? shippingAddressId, int? shipmentRuleId, int deliveryMethodId, int paymentTypeId, int? collectionPointId)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.cart = true;
            ViewBag.CartModal = true;
            ViewBag.paymentMethod = paymentTypeId;
            List<AccountAddresses> accountAddresses = new List<AccountAddresses>();
            if ((DeliveryMethod)deliveryMethodId == DeliveryMethod.ToShipmentAddress)
            {
                accountAddresses.Add(AccountServices.GetAccountAddressById(shippingAddressId.Value));
                var shippingRule = _tenantWebsiteService.GetWebsiteShippingRulesById(shipmentRuleId.Value);
                shippingRule.Price = Math.Round(((shippingRule.Price) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
                ViewBag.ShippingRule = shippingRule;
            }
            accountAddresses.Add(AccountServices.GetAllValidAccountAddressesByAccountId(accountId).FirstOrDefault(u => u.AddTypeBilling == true));
            ViewBag.Addresses = accountAddresses;
            var models = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();
            var orders = OrderService.CreateShopOrder(accountAddresses.FirstOrDefault().AccountID, _mapper.Map(models, new List<OrderDetail>()), CurrentTenantId, CurrentUserId, CurrentWarehouseId, CurrentTenantWebsite.SiteID);
            ViewBag.TotalQty = Math.Round(((models.Sum(u => u.TotalAmount)) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;
            ViewBag.DeliveryMethodId = deliveryMethodId;
            if ((DeliveryMethod)deliveryMethodId == DeliveryMethod.ToPickupPoint)
            {
                ViewBag.CollectionPoint = _tenantLocationServices.GetActiveTenantLocationById(collectionPointId.Value);
            }
            ViewBag.RetUrl = _paypalReturnUrl;
            ViewBag.PAYPALURL = _paypalUrl;
            ViewBag.PayPalIpnUrl = _paypalIpnUrl;
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
                await client.PostAsync(_paypalUrl, new StringContent(string.Empty));
                var response = await client.PostAsync(_paypalUrl, new FormUrlEncodedContent(ipn));
                var responseString = await response.Content.ReadAsStringAsync();
                return (responseString == "VERIFIED");
            }

        }

        public async Task<ActionResult> SagePay()
        {
            SagepayResponse resp = await GetSagePayToken();
            ViewBag.AuthCode = resp.merchantSessionKey;
            return View();
        }

        private async Task<SagepayResponse> GetSagePayToken()
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
                return JsonConvert.DeserializeObject<SagepayResponse>(responseString);
            }
        }
    }

    public class SagepayVendor
    {
        public string vendorName { get; set; }

    }

    public class SagepayResponse
    {
        public string merchantSessionKey { get; set; }
        public DateTime expiry { get; set; }

    }
}