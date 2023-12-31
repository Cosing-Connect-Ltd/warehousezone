﻿using AutoMapper;
using Ganedata.Core.Services;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using WarehouseEcommerce.Models;
using WarehouseEcommerce.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Domain;

namespace WarehouseEcommerce.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsServices _tenantServices;
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productlookupServices;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IProductPriceService _productPriceService;
        public HomeController(ICoreOrderService orderService, IMapper mapper, IProductLookupService productlookupServices, IProductServices productServices, IPropertyService propertyService,
            IAccountServices accountServices, ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices, IUserService userService, IActivityServices activityServices,
            ITenantsServices tenantServices, ITenantWebsiteService tenantWebsiteService, IProductPriceService productPriceService)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices, tenantWebsiteService)
        {
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
            _productServices = productServices;
            _productlookupServices = productlookupServices;
            _tenantWebsiteService = tenantWebsiteService;
            _productPriceService = productPriceService;

        }

        // GET: Home
        public ActionResult Index()
        {
            if (HttpContext.Session["caErrors"] != null)
            {
                return RedirectToAction("Index", "Error");
            }

            ViewBag.Title = CurrentTenantWebsite.SiteName;

            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup", ViewBag.groupId);
            Session["CheckoutViewModel"] = null;
            return View();
        }

        public ActionResult page(string pageUrl, string blogDetail = null)
        {
            ViewBag.BlogDetail = blogDetail;
            var content = _tenantWebsiteService.GetWebsiteContentByUrl(CurrentTenantWebsite.SiteID, pageUrl);
            if (string.IsNullOrEmpty(pageUrl) || content == null) { RedirectToAction("Index"); }
            ViewBag.BlogList = _tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, CurrentTenantWebsite.SiteID).Where(u => u.Id != content.Id && u.Type == ContentType.post).OrderByDescending(u => u.DateCreated).Take(7).ToList();

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

            var tenantWebsite = ViewBag.TenantWebsite as TenantWebsites;

            var model = new ContactUsViewModel
            {
                WebsiteContactAddress = tenantWebsite.WebsiteContactAddress,
                WebsiteContactEmail = tenantWebsite.WebsiteContactEmail,
                WebsiteContactPhone = tenantWebsite.WebsiteContactPhone
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Contact(ContactUsViewModel model)
        {
            var tenantWebsite = ViewBag.TenantWebsite as TenantWebsites;

            if (ModelState.IsValid && tenantWebsite?.WebsiteContactEmail != null)
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress(tenantWebsite?.WebsiteContactEmail));
                message.From = new MailAddress(model.Email);
                message.Subject = $"Customer Request - {model.Name}";
                message.Body = $"<p><strong>Name:</strong> {model.Name} <br/> <strong> Email: </strong> {model.Email} <br/> <strong> Message: </strong> {model.Message}</p>";
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
                        return RedirectToAction("Contact", "Home", new { area = "" });
                    }


                    Session["success"] = 1;
                    return RedirectToAction("Contact", "Home", new { area = "" });
                }
            }
            return View(model);
        }

        public PartialViewResult _TopHeaderPartial()
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup");
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            ViewBag.Symbol = currencyyDetail.Symbol;
            ViewBag.CurrencyName = currencyyDetail.CurrencyName;
            return PartialView();
        }
        public PartialViewResult _UserMenuPartial()
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;

            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup");
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            ViewBag.Symbol = currencyyDetail.Symbol;
            ViewBag.CurrencyName = currencyyDetail.CurrencyName;
            return PartialView();
        }
        public PartialViewResult _CartMenuPartial()
        {
            var currencyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup");
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            ViewBag.Symbol = currencyDetail.Symbol;
            ViewBag.CurrencyName = currencyDetail.CurrencyName;
            return PartialView();
        }

        public PartialViewResult _VerticalNavBarPartial()
        {
            var productCategories = _lookupServices.GetAllValidProductGroups((CurrentTenantId), 12);
            return PartialView(productCategories);
        }

        public PartialViewResult _SpecialProductPartial()
        {
            var tenantWebsite = ViewBag.TenantWebsite as TenantWebsites;
            var specialProduct = new ProductViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantWebsite.SiteID, CurrentTenantId, 6,string.Empty, tagId: tenantWebsite?.FeaturedTagId).ToList(),

            };

            specialProduct.productMasterList.ForEach(u => u.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(u.ProductId, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId) ?? 0, 2));
            specialProduct.FeaturedText = tenantWebsite?.HomeFeaturedProductText;

            return PartialView(specialProduct);

        }

        public PartialViewResult _OurBrands()
        {
            var tenantWebsite = ViewBag.TenantWebsite as TenantWebsites;
            var specialProduct = new OurBrandsViewModel
            {
                Manufacturers = _tenantWebsiteService.GetWebsiteProductManufacturers(CurrentTenantWebsite.SiteID).Where(o => o.ShowInOurBrands).OrderBy(b => b.SortOrder).ToList()
            };

            specialProduct.OurBrandsText = tenantWebsite?.HomeOurBrandsText;

            return PartialView(specialProduct);

        }
        public PartialViewResult _TopProductPartial()
        {

            var topProduct = new ProductViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantWebsite.SiteID, (CurrentTenantId), 12, "TopProduct").ToList(),
            };
            topProduct.productMasterList.ForEach(u => u.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(u.ProductId, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId) ?? 0, 2));

            if (topProduct.productMasterList != null)
            {
                var prdouctIds = topProduct.productMasterList.Select(u => u.ProductId).ToList();
            }

            return PartialView(topProduct);

        }
        public PartialViewResult _OnSalePartial()
        {
            var onSale = new ProductViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantWebsite.SiteID, CurrentTenantId, 6, "OnSaleProduct").ToList(),
            };
            onSale.productMasterList.ForEach(u => u.SellPrice = (Math.Round(_tenantWebsiteService.GetPriceForProduct(u.ProductId, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId) ?? 0, 2)));

            if (onSale.productMasterList != null)
            {
                var prdouctIds = onSale.productMasterList.Select(u => u.ProductId).ToList();
            }

            return PartialView(onSale);

        }

        public PartialViewResult _TopCategoryPartial()
        {
            var tenantWebsite = ViewBag.TenantWebsite as TenantWebsites;
            var topCategory = _tenantWebsiteService.GetAllValidWebsiteNavigationTopCategory(CurrentTenantId, CurrentTenantWebsite.SiteID).OrderBy(u => u.SortOrder).Take(6).ToList();
            ViewBag.TopCategoryText = tenantWebsite?.HomeTopCategoryText;
            return PartialView(topCategory);
        }



        public PartialViewResult _BestSellerPartial()
        {
            var BestSellerProduct = new ProductViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantWebsite.SiteID, (CurrentTenantId), 2, "BestSellerProduct").ToList()
            };
            BestSellerProduct.productMasterList.ForEach(u => u.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(u.ProductId, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId) ?? 0, 2));

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

        public PartialViewResult _SubscribePartial()
        {
            var websiteLayoutSettings = _tenantWebsiteService.GetWebsiteLayoutSettingsInfoBySiteId(CurrentTenantWebsite.SiteID);
            return PartialView(websiteLayoutSettings ?? new WebsiteLayoutSettings { ShowSubscriptionPanel = false });
        }



        public PartialViewResult _NewsLetterPartial()
        {
            return PartialView();
        }
        public PartialViewResult _TopProductBannerPartial()
        {
            var categories = _tenantWebsiteService.GetAllValidWebsiteNavigationCategory(CurrentTenantId, CurrentTenantWebsite.SiteID).OrderBy(u => u.SortOrder).Take(4).ToList();
            return PartialView(categories);

        }
        public PartialViewResult _ImageBlockPartial()
        {
            return PartialView();
        }

        public PartialViewResult _HorizontalNavbarPartial()
        {
            var navigation = _tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, CurrentTenantWebsite.SiteID).Where(u => u.ShowInNavigation == true).OrderBy(u => u.SortOrder).ToList();
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;

            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup");
            ViewBag.Symbol = currencyyDetail.Symbol;
            ViewBag.CurrencyName = currencyyDetail.CurrencyName;
            return PartialView(navigation);
        }

        public PartialViewResult _BlogPartial()
        {
            var data = _tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, CurrentTenantWebsite.SiteID).OrderByDescending(u => u.DateCreated).Where(u => u.Type == ContentType.post).Take(3).ToList();
            return PartialView(data);
        }

        public PartialViewResult _DeliveryInfoPartial()
        {
            var data = _tenantWebsiteService.GetAllValidWebsiteDeliveryNavigations(CurrentTenantId, CurrentTenantWebsite.SiteID).OrderBy(u => u.SortOrder).ToList();
            return PartialView(data);
        }



        public ActionResult _TopCategoryProductsPartial(int NavigationId)
        {
            var model = _tenantWebsiteService.GetProductByNavigationId(NavigationId).Take(5).ToList();
            return PartialView(model);
        }
        public ActionResult ReturnPath(int productId, bool status)
        {
            string path = GetPathAgainstProductId(productId, status);
            return Content(path);
        }
        public PartialViewResult _FooterPartialArea(bool university = false)
        {
            ViewBag.FooterNavigation = _tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, CurrentTenantWebsite.SiteID).Where(u => u.ShowInFooter == true && (u.Type == WebsiteNavigationType.Content || u.Type == WebsiteNavigationType.Link)).ToList();

            if (university)
            {
                var data = _tenantWebsiteService.GetAllActiveTenantWebSites(CurrentTenantId).FirstOrDefault(u => u.SiteID == CurrentTenantWebsite.SiteID);
                return PartialView(data);
            }

            var productManufacturer = _lookupServices.GetAllValidProductManufacturer(CurrentTenantId);
            return PartialView(productManufacturer.ToList());
        }

        public PartialViewResult _SocialMediaAccountsPartial()
        {
            return PartialView();
        }
        public PartialViewResult _defaultPartial()
        {
            var contentPage = _tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, CurrentTenantWebsite.SiteID).FirstOrDefault(u => u.IsHomePage == true);
            return PartialView(contentPage);
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult Blog()
        {
            var data = _tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, CurrentTenantWebsite.SiteID).Where(u => u.Type == ContentType.post).OrderByDescending(u => u.DateCreated).Take(12).ToList();
            return View(data);
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