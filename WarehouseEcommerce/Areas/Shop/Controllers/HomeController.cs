﻿using AutoMapper;
using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Web.Mvc;
using WarehouseEcommerce.Helpers;
using WarehouseEcommerce.ViewModels;

namespace WarehouseEcommerce.Areas.Shop.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsServices _tenantServices;
        private readonly ILookupServices _lookupServices;
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productlookupServices;
        public HomeController(ICoreOrderService orderService, IMapper mapper, IProductServices productServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices, IUserService userService, IActivityServices activityServices, ITenantsServices tenantServices)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices)
        {
            _lookupServices = lookupServices;
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
            _productServices = productServices;

        }
        public ActionResult Index()
        {
            //if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            //ViewBag.TopProduct = _productServices.GetProductByCategory((CurrentTenantId == 0 ? tenantId : CurrentTenantId), 12, TopProduct:true).ToList();
            //ViewBag.OnSaleProduct= _productServices.GetProductByCategory((CurrentTenantId == 0 ? tenantId : CurrentTenantId), 6, OnSaleProduct: true).ToList();
            ViewBag.ProductGroups = new SelectList(_lookupServices.GetAllValidProductGroups((CurrentTenantId), 12), "ProductGroupId", "ProductGroup", ViewBag.groupId);
            return View();
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
                productMasterList = _productServices.GetProductByCategory(CurrentTenantId, 6, SpecialProduct: true).ToList(),

            };
            specialProduct.productMasterList.ForEach(u => u.SellPrice = (Math.Round((u.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2)));
            specialProduct.CurrencySign = currencyyDetail.Symbol;
            if (specialProduct.productMasterList != null)
            {
                var prdouctIds = specialProduct.productMasterList.Select(u => u.ProductId).ToList();


                specialProduct.ProductFilesList = _productServices.GetProductFilesByTenantId((CurrentTenantId)).Where(u => prdouctIds.Contains(u.ProductId)).ToList();
            }

            return PartialView(specialProduct);

        }
        public PartialViewResult _TopProductPartial()
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            var TopProduct = new ProductDetailViewModel
            {
                productMasterList = _productServices.GetProductByCategory((CurrentTenantId), 12, TopProduct: true).ToList(),

            };
            TopProduct.productMasterList.ForEach(u => u.SellPrice = (Math.Round(((u.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0)), 2)));
            TopProduct.CurrencySign = currencyyDetail.Symbol;
            if (TopProduct.productMasterList != null)
            {
                var prdouctIds = TopProduct.productMasterList.Select(u => u.ProductId).ToList();
                TopProduct.ProductFilesList = _productServices.GetProductFilesByTenantId((CurrentTenantId)).Where(u => prdouctIds.Contains(u.ProductId)).ToList();
            }


            return PartialView(TopProduct);

        }
        public PartialViewResult _OnSalePartial()
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            var onsale = new ProductDetailViewModel
            {
                productMasterList = _productServices.GetProductByCategory(CurrentTenantId, 6, OnSaleProduct: true).ToList(),

            };
            onsale.productMasterList.ForEach(u => u.SellPrice = (Math.Round((u.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2)));
            onsale.CurrencySign = currencyyDetail.Symbol;
            if (onsale.productMasterList != null)
            {
                var prdouctIds = onsale.productMasterList.Select(u => u.ProductId).ToList();
                onsale.ProductFilesList = _productServices.GetProductFilesByTenantId((CurrentTenantId)).Where(u => prdouctIds.Contains(u.ProductId)).ToList();
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
                productMasterList = _productServices.GetProductByCategory((CurrentTenantId), 2, BestSellerProduct: true).ToList()
            };
            BestSellerProduct.productMasterList.ForEach(u => u.SellPrice = (Math.Round((u.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2)));
            BestSellerProduct.CurrencySign = currencyyDetail.Symbol;
            if (BestSellerProduct.productMasterList != null)
            {
                var prdouctIds = BestSellerProduct.productMasterList.Select(u => u.ProductId).ToList();
                BestSellerProduct.ProductFilesList = _productServices.GetProductFilesByTenantId((CurrentTenantId)).Where(u => prdouctIds.Contains(u.ProductId)).ToList();
            }


            return PartialView(BestSellerProduct);

        }
        public PartialViewResult _NewsLetterPartial()
        {

            return PartialView();

        }
        public PartialViewResult _TopProductBannerPartial()
        {

            return PartialView();

        }
        public PartialViewResult _ImageBlockPartial()
        {

            return PartialView();

        }

        public PartialViewResult _HorizontalNavbarPartial()
        {
            var productDepartments = _lookupServices.GetAllValidTenantDepartments(CurrentTenantId).Take(8).ToList();
            ViewBag.departments = productDepartments;
            var productGroups = _lookupServices.GetAllValidProductGroups(CurrentTenantId).ToList();
            ViewBag.productGroup = productGroups;
            ViewBag.UserName = CurrentUser.UserFirstName + " " + CurrentUser.UserLastName;
            return PartialView();
        }

        public ActionResult _GetProductByGroupsAndDepartment(int? DepartmentId, int? productgroupId)
        {
            return RedirectToAction("ProductCategories", "Products", new { productGroupId = productgroupId, departmentId = DepartmentId });
        }

        public ActionResult ReturnPath(int productId, bool status)
        {
            string path = GetPathAgainstProductId(productId, status);

            return Content(path);
        }
    }
}