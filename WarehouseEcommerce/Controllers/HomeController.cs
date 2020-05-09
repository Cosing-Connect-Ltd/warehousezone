using AutoMapper;
using Ganedata.Core.Services;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using WarehouseEcommerce.Helpers;
using WarehouseEcommerce.Models;
using WarehouseEcommerce.ViewModels;
using Ganedata.Core.Entities.Enums;

namespace WarehouseEcommerce.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsServices _tenantServices;
        private readonly ILookupServices _lookupServices;
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productlookupServices;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        public HomeController(ICoreOrderService orderService, IMapper mapper, IProductLookupService productlookupServices, IProductServices productServices, IPropertyService propertyService,
            IAccountServices accountServices, ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices, IUserService userService, IActivityServices activityServices,
            ITenantsServices tenantServices, ITenantWebsiteService tenantWebsiteService)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices)
        {
            _lookupServices = lookupServices;
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
            _productServices = productServices;
            _productlookupServices = productlookupServices;
            _tenantWebsiteService = tenantWebsiteService;

        }

        // GET: Home
        public ActionResult Index()
        {
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup", ViewBag.groupId);
            return View();
        }

        public ActionResult page(string pageUrl)
        {
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup", ViewBag.groupId);
            var content = _tenantWebsiteService.GetWebsiteContentByUrl(CurrentTenantWebsite.SiteID, pageUrl);
            return View(content);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public PartialViewResult _TopHeaderPartial()
        {
            caCurrent caUser = new caCurrent();
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CartItemCount = GaneCartItemsSessionHelper.GetCartItemsSession().Count;
            ViewBag.CartItems = GaneCartItemsSessionHelper.GetCartItemsSession().ToList();
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup");
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            ViewBag.Symbol = currencyyDetail.Symbol;
            ViewBag.CurrencyName = currencyyDetail.CurrencyName;
            return PartialView();
        }
        public PartialViewResult _UserMenuPartial()
        {
            caCurrent caUser = new caCurrent();
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CartItemCount = GaneCartItemsSessionHelper.GetCartItemsSession().Count;
            ViewBag.CartItems = GaneCartItemsSessionHelper.GetCartItemsSession().ToList();
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup");
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            ViewBag.Symbol = currencyyDetail.Symbol;
            ViewBag.CurrencyName = currencyyDetail.CurrencyName;
            return PartialView();
        }
        public PartialViewResult _CartMenuPartial()
        {
            caCurrent caUser = new caCurrent();
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CartItemCount = GaneCartItemsSessionHelper.GetCartItemsSession().Count;
            ViewBag.CartItems = GaneCartItemsSessionHelper.GetCartItemsSession().ToList();
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup");
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            ViewBag.Symbol = currencyyDetail.Symbol;
            ViewBag.CurrencyName = currencyyDetail.CurrencyName;
            return PartialView();
        }

        public PartialViewResult _VerticalNavBarPartial()
        {
            var ProductCategories = _lookupServices.GetAllValidProductGroups((CurrentTenantId), 12);
            return PartialView(ProductCategories);
        }

        public PartialViewResult _SpecialProductPartial()
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            var specialProduct = new ProductDetailViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantWebsite.SiteID,CurrentTenantId, 6, SpecialProduct: true).ToList(),

            };
            specialProduct.productMasterList.ForEach(u => u.SellPrice = (Math.Round((u.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2)));
            specialProduct.CurrencySign = currencyyDetail.Symbol;
            if (specialProduct.productMasterList != null)
            {
                var prdouctIds = specialProduct.productMasterList.Select(u => u.ProductId).ToList();
            }

            return PartialView(specialProduct);

        }
        public PartialViewResult _TopProductPartial()
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            var TopProduct = new ProductDetailViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantWebsite.SiteID,(CurrentTenantId), 12, TopProduct: true).ToList(),
            };
            TopProduct.productMasterList.ForEach(u => u.SellPrice = (Math.Round(((u.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0)), 2)));
            TopProduct.CurrencySign = currencyyDetail.Symbol;
            if (TopProduct.productMasterList != null)
            {
                var prdouctIds = TopProduct.productMasterList.Select(u => u.ProductId).ToList();
            }

            return PartialView(TopProduct);

        }
        public PartialViewResult _OnSalePartial()
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            var onsale = new ProductDetailViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantWebsite.SiteID,CurrentTenantId, 6, OnSaleProduct: true).ToList(),
            };
            onsale.productMasterList.ForEach(u => u.SellPrice = (Math.Round((u.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2)));
            onsale.CurrencySign = currencyyDetail.Symbol;
            if (onsale.productMasterList != null)
            {
                var prdouctIds = onsale.productMasterList.Select(u => u.ProductId).ToList();
            }

            return PartialView(onsale);

        }

        public PartialViewResult _TopCategoryPartial()
        {
            var TopCategory = _lookupServices.GetAllValidProductGroups((CurrentTenantId), 6);
            return PartialView(TopCategory);
        }



        public PartialViewResult _BestSellerPartial()
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            var BestSellerProduct = new ProductDetailViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantWebsite.SiteID, (CurrentTenantId), 2, BestSellerProduct: true).ToList()
            };
            BestSellerProduct.productMasterList.ForEach(u => u.SellPrice = (Math.Round((u.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2)));
            BestSellerProduct.CurrencySign = currencyyDetail.Symbol;
            if (BestSellerProduct.productMasterList != null)
            {
                var prdouctIds = BestSellerProduct.productMasterList.Select(u => u.ProductId).ToList();
            }

            return PartialView(BestSellerProduct);

        }

        public PartialViewResult _SliderPartial()
        {
            var slides = _tenantWebsiteService.GetAllValidWebsiteSlider(CurrentTenantId, CurrentTenantWebsite.SiteID).ToList();
            return PartialView(slides);
        }



        public PartialViewResult _NewsLetterPartial()
        {
            return PartialView();
        }
        public PartialViewResult _TopProductBannerPartial()
        {
            var categories = _tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, CurrentTenantWebsite.SiteID).Where(x => x.Type == WebsiteNavigationType.Category).ToList();
            return PartialView(categories);

        }
        public PartialViewResult _ImageBlockPartial()
        {
            return PartialView();
        }

        public PartialViewResult _HorizontalNavbarPartial()
        {
            var navigation = _tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, CurrentTenantWebsite.SiteID).ToList();
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CartItemCount = GaneCartItemsSessionHelper.GetCartItemsSession().Count;
            ViewBag.CartItems = GaneCartItemsSessionHelper.GetCartItemsSession().ToList();
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup");
            ViewBag.Symbol = currencyyDetail.Symbol;
            ViewBag.CurrencyName = currencyyDetail.CurrencyName;
            return PartialView(navigation);
        }



        public ActionResult _TopCategoryProductsPartial(int? ProductGroupId)
        {
            var model = _productlookupServices.GetAllValidProductGroupById(ProductGroupId).Take(5).ToList();
            return PartialView(model);
        }
        public ActionResult ReturnPath(int productId, bool status)
        {
            string path = GetPathAgainstProductId(productId, status);
            return Content(path);
        }
        public PartialViewResult _FooterPartialArea()
        {
            var productManufacturer = _lookupServices.GetAllValidProductManufacturer(CurrentTenantId);
            return PartialView(productManufacturer.ToList());
        }

        public PartialViewResult _SocialMediaAccountsPartial()
        {
            return PartialView();
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
        public ActionResult DataProtection()
        {
            return View();
        }

    }
}