﻿using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WarehouseEcommerce.Helpers;

namespace WarehouseEcommerce.Controllers
{
    public class ProductsController : BaseController
    {

        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsCurrencyRateServices _tenantServices;
        private readonly IProductLookupService _productlookupServices;
        private readonly ILookupServices _lookupServices;
        private readonly IProductServices _productServices;
        private readonly ICommonDbServices _commonDbServices;
        private readonly IMapper _mapper;
        private readonly IProductPriceService _productPriceService;
        private string[] Images = ConfigurationManager.AppSettings["ImageFormats"].Split(new char[] { ',' });

        public ProductsController(IProductServices productServices, IUserService userService, IProductLookupService productlookupServices, IProductPriceService productPriceService, ITenantsCurrencyRateServices tenantsCurrencyRateServices, IMapper mapper, ICommonDbServices commonDbServices, ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IActivityServices activityServices, ITenantsServices tenantServices)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices)
        {
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantsCurrencyRateServices;
            _productlookupServices = productlookupServices;
            _lookupServices = lookupServices;
            _productServices = productServices;
            _commonDbServices = commonDbServices;
            _mapper = mapper;
            _productPriceService = productPriceService;
        }
        // GET: Products

        public ActionResult list(string category, int? sort, string filter, string search, int? page, int? pagesize = 20, string values = "")
        {
            try
            {

                var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
                //ViewBag.groupId = group;
                ViewBag.Category = category;
                ViewBag.CurrentSort = sort;
                ViewBag.SortedValues = (sort ?? 1);
                ViewBag.pageList = new SelectList(from d in Enumerable.Range(1, 5) select new SelectListItem { Text = (d * 10).ToString(), Value = (d * 10).ToString() }, "Value", "Text", pagesize);
                ViewBag.searchString = search;
                ViewBag.CurrencySymbol = currencyyDetail.Symbol;
                ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
                var product = _productlookupServices.GetAllValidProductGroupAndDeptByName(CurrentTenantWebsite.SiteID, category);

                if (!string.IsNullOrEmpty(search))
                {
                    page = 1;
                }
                else
                {
                    search = filter;
                }
                ViewBag.CurrentFilter = search;
                if (!string.IsNullOrEmpty(search))
                {
                    product = product.Where(s => s.SKUCode.Contains(search) || s.Name.Contains(search));
                }
                product = _productlookupServices.FilterProduct(product, values);
                switch ((SortProductTypeEnum)(sort ?? 1))
                {
                    case SortProductTypeEnum.PriceByDesc:
                        product = product.OrderByDescending(s => s.SellPrice);
                        break;
                    case SortProductTypeEnum.NameByAsc:
                        product = product.OrderBy(s => s.Name);
                        break;
                    case SortProductTypeEnum.PriceByAsc:
                        product = product.OrderBy(s => s.SellPrice);
                        break;
                    default:  // Name ascending 
                        product = product.OrderBy(s => s.Name);
                        break;
                }

                int pageSize = pagesize ?? 10;
                int pageNumber = (page ?? 1);
                var pageedlist = product.ToPagedList(pageNumber, pageSize);
                page = pageNumber = pageNumber > pageedlist.PageCount ? pageedlist.PageCount : pageNumber;
                var data = product.ToPagedList((pageNumber == 0 ? 1 : pageNumber), pageSize);
                if (data.Count > 0)
                {
                    data.ToList().ForEach(u =>
                    u.SellPrice = (Math.Round((_productPriceService.GetProductPriceThresholdByAccountId(u.ProductId, null).SellPrice) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value), 2))

                    );
                }


                return View(data);
            }
            catch (Exception ex)
            {

                return Content("Issue of getting data  " + ex.Message);

            }

        }

        public ActionResult ProductDetails(string sku, int? productId = null)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            product.SellPrice = Math.Round((product.SellPrice ?? 0) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value), 2);
            if (product.GroupedProduct)
            {
                return RedirectToAction("GroupedProductDetail", "Products", new { sku = sku });
            }
            else if (product.Kit)
            {
                return RedirectToAction("KitProductDetail", "Products", new { sku = sku });
            }
            return View(product);
        }

        public ActionResult GroupedProductDetail(string sku)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            product.SellPrice = Math.Round((product.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2);
            var productTabs = product.ProductKitItems.Where(u => u.ProductKitType == ProductKitTypeEnum.Grouped && u.IsDeleted != true).Select(u => u.ProductKitTypeId).ToList();
            ViewBag.GroupedTabs = _productlookupServices.GetProductKitTypes(productTabs);
            return View(product);
        }
        public ActionResult KitProductDetail(string sku)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            product.SellPrice = Math.Round((product.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2);
            return View(product);
        }

        public JsonResult GetProductCategories()
        {

            var ProductCategories = _lookupServices.GetAllValidProductGroups((CurrentTenantId), 12);
            if (ProductCategories.Count() <= 0 || ProductCategories == null) return Json(false, JsonRequestBehavior.AllowGet);

            var data = (from pac in ProductCategories
                        select new
                        {
                            pac.ProductGroupId,
                            pac.ProductGroup
                        });
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);


        }

        public JsonResult searchProduct(string searchkey)
        {
            var model = (from product in _productlookupServices.GetAllValidProductGroupAndDeptByName(CurrentTenantWebsite.SiteID, ProductName: searchkey)
                         select new ProductSearchResult
                         {
                             Id = product.ProductId,
                             Name = product.Name,
                             Path = product.ProductFiles.FirstOrDefault(u => Images.Contains(u.FilePath)).FilePath,
                             SKUCode = product.SKUCode,
                             SearchKey = searchkey
                         }).OrderBy(u => u.Id).Take(10).ToList();


            model.ForEach(x => x.Path = ConfigurationManager.AppSettings["BaseFilePath"] + (string.IsNullOrEmpty(x.Path) ? "/UploadedFiles/Products/no_image.gif" : x.Path));


            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddToCart()
        {
            ViewBag.cart = true;

            return View();

        }

        public ActionResult Category()
        {
            ViewBag.cart = true;

            return View();

        }
        public ActionResult CategoryDetail()
        {
            ViewBag.cart = true;

            return View();

        }

        public PartialViewResult _CartItemsPartial(int? ProductId, decimal? qty, bool? Remove, bool? details)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            if (CurrentUser?.UserId <= 0)
            {
                ViewBag.PlaceOrder = true;
            }
            ViewBag.cart = true;
            if (!ProductId.HasValue)
            {

                var models = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();
                var data = models.FirstOrDefault(u => u.CurrencyId == currencyyDetail.Id);
                if (data == null)
                {
                    foreach (var item in models)
                    {
                        var product = _productServices.GetProductMasterById(item.ProductId);
                        item.Price = Math.Round(((product.SellPrice ?? 0) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
                        item.CurrencySign = currencyyDetail.Symbol;
                        item.CurrencyId = currencyyDetail.Id;
                        GaneCartItemsSessionHelper.UpdateCartItemsSession("", item, false, false);
                    }
                    models = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();

                }
                ViewBag.CurrencySymbol = currencyyDetail.Symbol;
                ViewBag.TotalQty = Math.Round(models.Sum(u => u.TotalAmount), 2);
                return PartialView(models);

            }
            else
            {
                if (Remove == true)
                {
                    GaneCartItemsSessionHelper.RemoveCartItemSession(ProductId ?? 0);
                    var models = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();
                    ViewBag.TotalQty = Math.Round(models.Sum(u => u.TotalAmount), 2);
                    models.ForEach(u => u.Price = Math.Round((u.Price) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value), 2));
                    models.ForEach(u => u.CurrencySign = currencyyDetail.Symbol);
                    return PartialView(models);

                }
                else if (qty.HasValue && !details.HasValue)
                {
                    var model = new OrderDetail();
                    var Product = _productServices.GetProductMasterById(ProductId ?? 0);
                    model.ProductMaster = Product;
                    model.Qty = qty.HasValue ? qty.Value : 1;
                    model.ProductId = ProductId ?? 0;
                    model.Price = Math.Round(((_productPriceService.GetProductPriceThresholdByAccountId(model.ProductId, null).SellPrice) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
                    model = _commonDbServices.SetDetails(model, null, "SalesOrders", "");
                    var Details = _mapper.Map(model, new OrderDetailSessionViewModel());
                    Details.Price = Math.Round(((Details.Price) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
                    Details.CurrencyId = currencyyDetail.Id;
                    GaneCartItemsSessionHelper.UpdateCartItemsSession("", Details, false);
                    var models = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();
                    ViewBag.TotalQty = Math.Round(models.Sum(u => u.TotalAmount), 2);
                    //models.ForEach(u => u.Price = (u.Price * (currencyyDetail.Rate ?? 0)));
                    models.ForEach(u => u.CurrencySign = currencyyDetail.Symbol);
                    return PartialView(models);
                }
                else
                {

                    var Product = _productServices.GetProductMasterById(ProductId ?? 0);
                    var model = new OrderDetail();
                    model.ProductMaster = Product;
                    model.Qty = qty.HasValue ? qty.Value : 1;
                    model.ProductId = ProductId ?? 0;
                    model.Price = Math.Round(((_productPriceService.GetProductPriceThresholdByAccountId(model.ProductId, null).SellPrice) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
                    model = _commonDbServices.SetDetails(model, null, "SalesOrders", "");
                    ViewBag.CartModal = false;
                    var Details = _mapper.Map(model, new OrderDetailSessionViewModel());
                    var ProductCheck = GaneCartItemsSessionHelper.GetCartItemsSession().FirstOrDefault(u => u.ProductId == ProductId);
                    Details.Price = Math.Round(((Details.Price) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2);
                    Details.CurrencyId = currencyyDetail.Id;
                    if (ProductCheck != null)
                    {
                        Details.Qty = ProductCheck.Qty + (qty.HasValue ? qty.Value : 1);
                    }
                    GaneCartItemsSessionHelper.UpdateCartItemsSession("", Details, false);
                    var models = GaneCartItemsSessionHelper.GetCartItemsSession() ?? new List<OrderDetailSessionViewModel>();
                    var cartedProduct = models.Where(u => u.ProductId == ProductId).ToList();
                    cartedProduct.ForEach(u => u.Price = Math.Round((u.Price) * (((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value)), 2));
                    cartedProduct.ForEach(u => u.CurrencySign = currencyyDetail.Symbol);
                    return PartialView(cartedProduct);
                }
            }

        }

        public void CurrencyChanged(int? CurrencyId)
        {

            if (CurrencyId.HasValue)
            {
                var detail = LookupServices.GetAllGlobalCurrencies().Where(c => (!CurrencyId.HasValue || c.CurrencyID == CurrencyId)).Select(u => new caCurrencyDetail
                {

                    Symbol = u.Symbol,
                    Id = u.CurrencyID,
                    CurrencyName = u.CurrencyName

                }).ToList();
                var getTenantCurrencies = _tenantServices.GetTenantCurrencies(CurrentTenantId).FirstOrDefault(u => u.CurrencyID == detail.FirstOrDefault()?.Id);
                detail.ForEach(c =>
                    c.Rate = _tenantServices.GetCurrencyRateByTenantid(getTenantCurrencies.TenantCurrencyID)
                );
                Session["CurrencyDetail"] = detail.FirstOrDefault();
            }
        }

        public int CartItemsCount()
        {
            return GaneCartItemsSessionHelper.GetCartItemsSession().Count;

        }

        public ActionResult PaymentInfo()
        {

            return View();
        }

        public PartialViewResult _DynamicFilters(string category, string productName)
        {
            ProductFilteringViewModel productFiltering = new ProductFilteringViewModel();
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;
            var products = _productlookupServices.GetAllValidProductGroupAndDeptByName(CurrentTenantWebsite.SiteID, category, ProductName: productName);
            
            productFiltering.Manufacturer = _productlookupServices.GetAllValidProductManufacturerGroupAndDeptByName(products).Select(u => u.Name).ToList();
            productFiltering.PriceInterval = _productlookupServices.AllPriceListAgainstGroupAndDept(products);
            productFiltering.AttributeValues = _productlookupServices.GetAllValidProductAttributeValuesByProductIds(products);
            productFiltering.subCategories = _productlookupServices.GetAllValidSubCategoriesByDepartmentAndGroup(products).ToList();
            productFiltering.Count = products.Count();

            return PartialView(productFiltering);
        }




    }

    public class ProductSearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Category { get; set; }
        public string SKUCode { get; set; }
        public string SearchKey { get; set; }
    }
}