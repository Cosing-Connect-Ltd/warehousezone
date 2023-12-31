﻿using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WarehouseEcommerce.Helpers;
using WarehouseEcommerce.ViewModels;

namespace WarehouseEcommerce.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsCurrencyRateServices _tenantServices;
        private readonly IProductLookupService _productlookupServices;
        private readonly IProductServices _productServices;
        private readonly ICommonDbServices _commonDbServices;
        private readonly IMapper _mapper;
        private readonly IProductPriceService _productPriceService;
        private readonly ITenantWebsiteService _tenantWebsiteService;

        private string[] Images = ConfigurationManager.AppSettings["ImageFormats"].Split(new char[] { ',' });

        public ProductsController(IProductServices productServices,
                                    IUserService userService,
                                    IProductLookupService productlookupServices,
                                    ITenantWebsiteService tenantWebsiteService,
                                    IProductPriceService productPriceService,
                                    ITenantsCurrencyRateServices tenantsCurrencyRateServices,
                                    IMapper mapper,
                                    ICommonDbServices commonDbServices,
                                    ICoreOrderService orderService,
                                    IPropertyService propertyService,
                                    IAccountServices accountServices,
                                    ILookupServices lookupServices,
                                    IActivityServices activityServices)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices, tenantWebsiteService)
        {
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantsCurrencyRateServices;
            _productlookupServices = productlookupServices;
            _productServices = productServices;
            _commonDbServices = commonDbServices;
            _mapper = mapper;
            _productPriceService = productPriceService;
            _tenantWebsiteService = tenantWebsiteService;
        }

        // GET: Products

        public ActionResult list(string category, int? categoryId, string previousSearch, string search, int? page, int? pageSize = 12, string filters = "", SortProductTypeEnum sort = SortProductTypeEnum.Recommended)
        {
            try
            {
                ViewBag.Title = CurrentTenantWebsite.SiteName;

                var model = new ProductListViewModel
                {
                    CurrentCategoryName = category,
                    CurrentCategoryId = categoryId,
                    CurrentSort = sort,
                    CurrentFilters = filters
                };

                if (categoryId != null || !string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(previousSearch) || !string.IsNullOrEmpty(filters))
                {
                    var categoryInfo = _tenantWebsiteService.GetAllValidWebsiteNavigationCategory(CurrentTenantId, CurrentTenantWebsite.SiteID)
                                                            .FirstOrDefault(c => c.Id == categoryId);

                    model.Category = categoryInfo?.Parent ?? categoryInfo;
                    model.SubCategory = categoryInfo?.Parent != null ? categoryInfo : null;

                    if (model.Category != null)
                    {
                        ViewBag.Title += $": {model.Category.Name}";
                    }

                    if (model.SubCategory != null)
                    {
                        ViewBag.Title += $": {model.SubCategory.Name}";
                    }

                    var products = _tenantWebsiteService.GetWebsiteProducts(CurrentTenantWebsite.SiteID, category, string.Empty, categoryId);

                    if (!string.IsNullOrEmpty(search))
                    {
                        page = 1;
                    }
                    else
                    {
                        search = previousSearch;
                    }

                    if (categoryId != null)
                    {
                        model.DynamicFilters = GetDynamicFiltersModel(products.Select(p => p.ProductId).ToList(), categoryId);
                    }

                    model.CurrentSearch = search;

                    if (!string.IsNullOrEmpty(search))
                    {
                        products = products.Where(s => s.SKUCode.Contains(search) || s.Name.Contains(search));
                    }

                    products = _productlookupServices.ApplyFixedFilters(products, filters, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId);

                    if (categoryId == null)
                    {
                        model.DynamicFilters = GetDynamicFiltersModel(products.Select(p => p.ProductId).ToList(), categoryId);
                    }

                    model.DynamicFilters.Attributes = _tenantWebsiteService.GetAllValidProductAttributeValuesByProductIds(products);

                    products = _productlookupServices.ApplyAttributeFilters(products, filters, CurrentTenantWebsite.SiteID);

                    if (products.Any())
                    {
                        products.ForEach(u => u.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(u.ProductId, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId) ?? 0, 2));
                    }

                    IEnumerable<ProductMaster> prods;

                    switch (sort)
                    {
                        case SortProductTypeEnum.PriceByDesc:
                            prods = products.ToList().OrderByDescending(s => s.SellPrice);
                            break;
                        case SortProductTypeEnum.PriceByAsc:
                            prods = products.ToList().OrderBy(s => s.SellPrice);
                            break;
                        case SortProductTypeEnum.NameByDesc:
                            prods = products.ToList().OrderByDescending(s => s.Name);
                            break;
                        case SortProductTypeEnum.NameByAsc:
                            prods = products.ToList().OrderBy(s => s.Name);
                            break;
                        default:
                            prods = products;
                            break;
                    }

                    pageSize = pageSize ?? 12;
                    int pageNumber = (page ?? 1);
                    var productsPagedList = prods.ToPagedList(pageNumber, pageSize.Value);
                    page = pageNumber = pageNumber > productsPagedList.PageCount ? productsPagedList.PageCount : pageNumber;
                    var pagedProductsList = prods.ToPagedList((pageNumber == 0 ? 1 : pageNumber), pageSize.Value);
                    
                    model.DynamicFilters.FilteredCount = prods.Count();
                    model.Products = pagedProductsList;
                }
                else
                {
                    model = new ProductListViewModel
                    {
                        DynamicFilters = GetDynamicFiltersModel(null, categoryId)
                    };
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return Content("Issue of getting data  " + ex.Message);
            }
        }

        public ActionResult Brands()
        {
            ViewBag.Title = CurrentTenantWebsite.SiteName + " Brands";
            try
            {
                var manufactuters = _tenantWebsiteService.GetWebsiteProductManufacturers(CurrentTenantWebsite.SiteID);

                return View(manufactuters);
            }
            catch (Exception ex)
            {
                return Content("Issue of getting data  " + ex.Message);
            }
        }

        public ActionResult ProductDetails(string sku, int? productId = null, string filters = "")
        {
            var baseProduct = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);

            if (baseProduct.ProductType == ProductKitTypeEnum.Grouped)
            {
                return RedirectToAction("GroupedProductDetail", "Products", new { sku = sku, filters = filters });
            }
            else if (baseProduct.ProductType == ProductKitTypeEnum.Kit)
            {
                return RedirectToAction("KitProductDetail", "Products", new { sku = sku });
            }

            var model = new ProductDetailViewModel();

            if (baseProduct?.ProductType == ProductKitTypeEnum.ProductByAttribute)
            {
                model = GetSelectedChildProduct(productId, baseProduct);
            }
            else
            {
                model.Product = baseProduct;
            }

            if (model == null)
            {
                return RedirectToAction("List", "Products", new { sku = sku });
            }

            model.Product.ProductFiles = model.Product.ProductFiles.Where(p => p.IsDeleted != true).ToList();

            if (baseProduct?.ProductType == ProductKitTypeEnum.ProductByAttribute && baseProduct.ProductId != model.Product.ProductId)
            {
                baseProduct.ProductFiles.Where(p => p.IsDeleted != true && !model.Product.ProductFiles.Select(f => Path.GetFileNameWithoutExtension(f.FilePath)).Contains(Path.GetFileNameWithoutExtension(p.FilePath))).ForEach(p =>
                {
                    model.Product.ProductFiles.Add(p);
                });

                model.Product.Description = string.IsNullOrEmpty(model.Product.Description?.Trim()) ? baseProduct.Description : model.Product.Description;
            }

            model.ParentProductId = baseProduct.ProductId;
            model.ParentProductName = baseProduct.Name;
            model.ParentProductType = baseProduct.ProductType;
            model.ParentProductSKUCode = baseProduct.SKUCode;

            SetProductViewModelCategoryInfo(baseProduct, model);

            model.Prices = _tenantWebsiteService.GetPricesForProducts(new List<int> { model.Product.ProductId }, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId).FirstOrDefault();

            model.AvailableProductCount = InventoryAvailabilityExtensions.GetAvailableProductCount(model.Product, CurrentTenantWebsite.SiteID);

            model.RelatedProducts = _productServices.GetRelatedProductsByProductId(model.Product.ProductId, CurrentTenantId, CurrentTenantWebsite.SiteID, baseProduct.ProductId);

            model.RelatedProducts.ForEach(u => u.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(u.ProductId, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId) ?? 0, 2));

            return View(model);
        }

        private void SetProductViewModelCategoryInfo(ProductMaster product, BaseProductViewModel model)
        {
            var categoryInfo = _tenantWebsiteService.GetProductCategoryByProductId(CurrentTenantWebsite.SiteID, product.ProductId);

            model.Category = categoryInfo?.Parent?.Name ?? categoryInfo?.Name;
            model.CategoryId = categoryInfo?.Parent?.Id ?? categoryInfo?.Id ?? 0;
            model.SubCategory = categoryInfo?.Parent != null ? categoryInfo.Name : null;
            model.SubCategoryId = categoryInfo?.Parent != null ? categoryInfo.Id : 0;
        }

        public ActionResult GroupedProductDetail(string sku, string filters = "")
        {
            var model = new GroupedProductViewModel();

            model.Product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            var productTabs = model.Product.ProductKitItems.Where(u => u.ProductKitType == ProductKitTypeEnum.Grouped && u.IsActive == true).Select(u => u.ProductKitTypeId).ToList();

            var allKitProducts = model.Product.ProductKitItems.Where(u => u.ProductKitType == ProductKitTypeEnum.Grouped &&
                                                                           u.KitProductMaster.IsDeleted != true &&
                                                                           u.ProductMaster.IsDeleted != true &&
                                                                           u.IsDeleted != true);

            var allCalculatableProducts = allKitProducts.Where(k => k.ProductKitTypes != null && k.ProductKitTypes.UseInParentCalculations == true)
                                                                        .Select(p => p.KitProductMaster)
                                                                        .ToList();

            model.DynamicFilters = GetDynamicFiltersModel(allCalculatableProducts.Select(p => p.ProductId).ToList(), null, true);

            var isFirstTab = true;
            model.GroupedTabs = new List<ChildProductsViewModel>();
                
                _productlookupServices.GetProductKitTypes(productTabs)
                .ForEach(ProductKitType =>
                {
                    var kitProducts = allKitProducts.Where(u => (u.ProductKitTypeId == ProductKitType.Id || ((u.ProductKitTypeId == null || u.ProductKitTypeId == 0) && isFirstTab)))
                                                                   .GroupBy(m => m.KitProductMaster)
                                                                   .Select(m => m.Key);

                    List<ProductMaster> kitProductsList;

                    if (isFirstTab)
                    {
                        var filteredKitProducts = _productlookupServices.ApplyFixedFilters(kitProducts.AsQueryable(), filters, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId);

                        model.DynamicFilters.Attributes = _tenantWebsiteService.GetAllValidProductAttributeValuesByProductIds(filteredKitProducts);

                        filteredKitProducts = _productlookupServices.ApplyAttributeFilters(filteredKitProducts, filters, CurrentTenantWebsite.SiteID);

                        kitProductsList = filteredKitProducts.ToList();
                    }
                    else
                    {
                        kitProductsList = kitProducts.ToList();
                    }

                    var filtersDic = _productlookupServices.ReadFiltersString(filters);                    
                    var inStockOnly = filtersDic != null && filtersDic.TryGetValue("instockonly", out List<string> inStockData) && inStockData.Any(s => Convert.ToBoolean(s));

                    var finalKitProducts = kitProductsList.Select(p => new
                    {
                        Product = p,
                        AvailableProductCount = InventoryAvailabilityExtensions.GetAvailableProductCount(p, CurrentTenantWebsite.SiteID),
                    })
                    .Where(p => !inStockOnly || p.AvailableProductCount > 0)
                    .OrderByDescending(p => p.AvailableProductCount != null && p.AvailableProductCount > 0);

                    isFirstTab = false;

                    model.GroupedTabs.Add(
                        new ChildProductsViewModel
                        {
                            Products = finalKitProducts.Select(p => p.Product).ToList(),
                            ProductsAvailableCounts = finalKitProducts.ToDictionary(p => p.Product.ProductId, p => p.AvailableProductCount),
                            Prices = _tenantWebsiteService.GetPricesForProducts(finalKitProducts.Select(p => p.Product.ProductId).ToList(), CurrentTenantWebsite.SiteID, CurrentUser?.AccountId)
                                                      .GroupBy(p => p.ProductId)
                                                      .ToDictionary(p => p.Key, p => p.First()),
                            ProductKitType = ProductKitType
                        });                    
                });

            model.DynamicFilters.FilteredCount = model.GroupedTabs?.Where(g => g.ProductKitType.UseInParentCalculations == true).Sum(a => a.Products.Count()) ?? 0;

            SetProductViewModelCategoryInfo(model.Product, model);

            model.RelatedProducts = _productServices.GetRelatedProductsByProductId(model.Product.ProductId, CurrentTenantId, CurrentTenantWebsite.SiteID);

            model.RelatedProducts.ForEach(u => u.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(u.ProductId, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId) ?? 0, 2));

            return View(model);
        }

        public ActionResult KitProductDetail(string sku)
        {
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            product.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(product.ProductId, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId) ?? 0, 2);
            return View(product);
        }

        public JsonResult GetProductCategories()
        {
            var productCategories = _lookupServices.GetAllValidProductGroups((CurrentTenantId), 12);
            if (productCategories.Count() <= 0 || productCategories == null) return Json(false, JsonRequestBehavior.AllowGet);

            var data = (from pac in productCategories
                        select new
                        {
                            pac.ProductGroupId,
                            pac.ProductGroup
                        });
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult searchProduct(string searchkey)
        {
            var model = _tenantWebsiteService.SearchWebsiteProducts(CurrentTenantWebsite.SiteID, 10, searchkey);

            model.ForEach(x => x.DefaultImage = CurrentTenantWebsite.BaseFilePath + (string.IsNullOrEmpty(x.DefaultImage) ? "/UploadedFiles/Products/no_image.gif" : x.DefaultImage));

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddToCart()
        {
            ViewBag.Title = CurrentTenantWebsite.SiteName + " Shopping Basket";
            ViewBag.cart = true;
            Session["CheckoutViewModel"] = null;
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

        public PartialViewResult _CartItemsPartial(int? cartId = null)
        {
            var model = new WebsiteCartItemsViewModel { WebsiteCartItems = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, HttpContext.Session.SessionID, cartId).ToList() };

            model.ShippmentAddresses = _mapper.Map(_accountServices.GetAllValidAccountAddressesByAccountIdOrSessionKey(CurrentUser.AccountId ?? 0, Session.SessionID).Where(u => u.AddTypeShipping == true && u.IsDeleted != true).ToList(), new List<AddressViewModel>());

            model.ShowCartPopUp = cartId.HasValue;
            model.ShowLoginPopUp = CurrentUserId == 0;

            model.IsCollectionAvailable = CurrentTenantWebsite.IsCollectionAvailable;
            model.IsDeliveryAvailable = CurrentTenantWebsite.IsDeliveryAvailable;

            InitiateShippmentAddress(ref model);

            InitiateCollectionPoint(ref model);

            return PartialView(model);
        }

        private void InitiateShippmentAddress(ref WebsiteCartItemsViewModel model)
        {
            if (Session["shippingAddressId"] == null && Session["shippingAddressPostCode"] == null)
            {
                var firstAddress = model.ShippmentAddresses.FirstOrDefault();
                Session["shippingAddressId"] = firstAddress?.AddressID;
                Session["shippingAddressPostCode"] = firstAddress?.PostCode;
            }

            model.ShippingAddressId = Session["shippingAddressId"] != null ? (int)Session["shippingAddressId"] : (int?)null;

            var postCode = Session["shippingAddressPostCode"] != null ? (string)Session["shippingAddressPostCode"] : null;
            model.ShippingAddressPostCode = postCode;

            if (!string.IsNullOrEmpty(model.ShippingAddressPostCode?.Trim()))
            {
                model.WebsiteCartItems.ForEach(c =>
                {
                    c.IsAvailableForDelivery = GetDeliveryAvailabilityStatus(c.ProductId, ((double)c.Quantity * c.KitProductCartItems?.Sum(ki => (ki.SimpleProductMaster?.Weight ?? 0)) ?? (c.ProductMaster?.Weight ?? 0)), postCode);
                });
            }
        }

        private void InitiateCollectionPoint(ref WebsiteCartItemsViewModel model)
        {
            if (Session["collectionPointId"] != null)
            {
                model.CollectionPointId = (int)Session["collectionPointId"];
                model.CollectionPointName = (string)Session["collectionPointName"];
                model.CollectionPointAddress = (string)Session["collectionPointAddress"];
                model.WebsiteCartItems.ForEach(c =>
                {
                    c.IsAvailableForCollection = GetCollectionAvailabilityStatus(c.ProductId, c.Quantity, (int)Session["collectionPointId"]);
                });
            }
        }

        private bool? GetCollectionAvailabilityStatus(int productId, decimal quantity, int? collectionPointId)
        {
            return collectionPointId == null ?
                                    (bool?)null :
                                    _productServices.GetAllInventoryStocksByProductId(productId)
                                                         .Any(i => i.IsActive == true &&
                                                                   i.IsDeleted != false &&
                                                                   i.WarehouseId == collectionPointId &&
                                                                   i.Available >= quantity);
        }

        private bool? GetDeliveryAvailabilityStatus(int productId, double parcelWeightInGrams, string postCode)
        {
            var availableShipmentRules = _tenantWebsiteService.GetShippingRulesByPostCode(CurrentTenantId, CurrentTenantWebsite.SiteID, postCode, parcelWeightInGrams);
            return string.IsNullOrEmpty(postCode?.Trim()) ? (bool?)null : availableShipmentRules.Count > 0;
        }

        public JsonResult AddWishListItem(int productId)
        {
            ViewBag.CartModal = false;

            var itemsCount = _tenantWebsiteService.AddOrUpdateWishListItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, productId);
            return Json(itemsCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNotifyListItem(int productId, string emailId)
        {
            ViewBag.CartModal = false;
            var itemsCount = _tenantWebsiteService.AddOrUpdateNotifyListItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, productId, emailId);
            
            return Json(itemsCount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsProductInWishList(int productId)
        {
            return Json(InventoryAvailabilityExtensions.IsProductInWishList(productId, CurrentUserId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsProductInNotifyList(int productId)
        {
            return Json(InventoryAvailabilityExtensions.IsProductInNotifyList(productId, CurrentUserId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WishList()
        {
            ViewBag.Title = CurrentTenantWebsite.SiteName + " WishList";
            if (CurrentUserId > 0)
            {
                var model = _tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).ToList();
                return View(model);
            }
            var data = GaneWishListItemsSessionHelper.GetWishListItemsSession().ToList().Select(u => new WebsiteWishListItem()
            {
                ProductMaster = _productServices.GetProductMasterById(u.ProductId),
            });
            return View(data.ToList());
        }

        public PartialViewResult _wishlistItems(int? productId)
        {
            if (productId.HasValue)
            {
                var count = _tenantWebsiteService.RemoveWishListItem((productId ?? 0), CurrentTenantWebsite.SiteID, CurrentUserId);
                return PartialView(_tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).ToList());
            }
            else
            {
                var model = _tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).ToList();
                return PartialView(model);
            }
        }

        public JsonResult GetWishlistNotificationStatus(int productId)
        {
            return Json(
                _tenantWebsiteService.GetWishlistNotificationStatus(productId, CurrentTenantWebsite.SiteID,
                    CurrentUserId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeWishListStatus(int productId, bool notification)
        {
            return Json(
                _tenantWebsiteService.ChangeWishListStatus(productId, notification, CurrentTenantWebsite.SiteID,
                    CurrentUserId), JsonRequestBehavior.AllowGet);
        }

        public void CurrencyChanged(int? currencyId)
        {
            if (currencyId.HasValue)
            {
                var detail = _lookupServices.GetAllGlobalCurrencies().Where(c => (!currencyId.HasValue || c.CurrencyID == currencyId)).Select(u => new caCurrencyDetail
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
            return _tenantWebsiteService.GetAllValidCartItemsList(CurrentTenantWebsite.SiteID, CurrentUserId, Session.SessionID).Count();
        }

        public ActionResult PaymentInfo()
        {
            return View();
        }

        public ProductDynamicFilteringViewModel GetDynamicFiltersModel(List<int> productIds, int? categoryId, bool isProductChildsFilter = false)
        {
            var productFiltering = new ProductDynamicFilteringViewModel();

            if (productIds != null && productIds.Count() > 0)
            {
                productFiltering.Brands = _tenantWebsiteService.GetAllValidProductManufacturers(productIds);
                productFiltering.Types = _productlookupServices.GetAllValidSubCategoriesByDepartmentAndGroup(productIds);
                (productFiltering.MinPrice, productFiltering.MaxPrice) = _tenantWebsiteService.GetAvailablePricesRange(productIds, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId);

                productFiltering.TotalCount = !isProductChildsFilter ? productIds.Count() : 0;
            }

            productFiltering.Categories = !isProductChildsFilter ?
                                           _productlookupServices.GetWebsiteNavigationCategoriesList(categoryId, CurrentTenantWebsite.SiteID)
                                           .OrderBy(o => o.SortOrder)
                                           .ToList() :
                                           new List<WebsiteNavigation>();

            return productFiltering;
        }

        public JsonResult RemoveWishListItem(int ProductId)
        {
            var count = _tenantWebsiteService.RemoveWishListItem(ProductId, CurrentTenantWebsite.SiteID, CurrentUserId);

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveNotifyListItem(int ProductId)
        {
            var count = _tenantWebsiteService.RemoveNotifyItem(ProductId, CurrentTenantWebsite.SiteID, CurrentUserId);

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _KitProductAttributeDetail(string skuCode, decimal? quantity = null, int? productId = null)
        {
            var baseProduct = _productServices.GetProductMasterByProductCode(skuCode, CurrentTenantId);
            baseProduct.ProductFiles = baseProduct.ProductFiles.Where(p => p.IsDeleted != true).ToList();

            IEnumerable<SelectListItem> squares = Enumerable.Range(1, (quantity.HasValue ? Convert.ToInt32(quantity) : 1))
                                                            .Select(u => new SelectListItem
                                                            {
                                                                Text = u.ToString(),
                                                                Value = u.ToString()
                                                            })
                                                            .ToList();

            ViewBag.QuantityList = new SelectList(squares, "Text", "Value");

            var model = new ProductDetailViewModel();
            if (baseProduct.ProductType == ProductKitTypeEnum.ProductByAttribute)
            {
                model = GetSelectedChildProduct(productId, baseProduct);
            }
            else
            {
                model.Product = baseProduct;
            }

            model.ParentProductType = baseProduct.ProductType;
            model.ParentProductSKUCode = baseProduct.SKUCode;
            model.Quantity = quantity;

            return PartialView(model);
        }

        private ProductDetailViewModel GetSelectedChildProduct(int? productId, ProductMaster product)
        {
            var relatedProducts = _productServices.GetAllProductByAttributeKitsByProductId(product.ProductId);

            var selectedProduct = relatedProducts.FirstOrDefault(p => p.ProductId == productId || productId == null);

            if (selectedProduct != null)
            {
                selectedProduct.ProductAttributeValuesMap = selectedProduct.ProductAttributeValuesMap.Where(p => p.IsDeleted != true).ToList();
            }

            var productDetailViewModel = new ProductDetailViewModel
            {
                Product = selectedProduct ?? product,
                AvailableAttributes = GetProductAttributes(relatedProducts, selectedProduct ?? product)
            };

            return productDetailViewModel;
        }

        private List<ProductDetailAttributeViewModel> GetProductAttributes(IEnumerable<ProductMaster> relatedProducts, ProductMaster selectedProduct)
        {
            var availableAttributes = relatedProducts
                                    .SelectMany(a => a.ProductAttributeValuesMap.Where(p => p.IsDeleted != true).Select(k => k.ProductAttributeValues))
                                    .GroupBy(a => a.ProductAttributes).Where(u => u.Key.IsDeleted != true).OrderBy(u => u.Key.SortOrder)
                                    .ToDictionary(g => g.Key, g => g.OrderBy(av => av.SortOrder)
                                                                                    .GroupBy(av => av.AttributeValueId)
                                                                                    .Select(av => av.First()).OrderBy(u => u.ProductAttributes.SortOrder)
                                                                                    .ToList());

            var processedProductAttributes = new List<ProductDetailAttributeViewModel>();

            foreach (var item in availableAttributes)
            {
                var attribute = item.Key;
                var selectedAttributeValue = selectedProduct.ProductAttributeValuesMap.FirstOrDefault(a => a.IsDeleted != true && a.ProductAttributeValues.AttributeId == attribute.AttributeId)?.ProductAttributeValues;

                var attributeViewModel = new ProductDetailAttributeViewModel();
                attributeViewModel.Name = attribute.AttributeName;
                attributeViewModel.SelectedValue = selectedAttributeValue?.Value;

                foreach (var attributeValue in item.Value)
                {
                    var attributeRelatedProducts = relatedProducts.Where(p => p.SKUCode != selectedProduct.SKUCode &&
                                                                              p.ProductAttributeValuesMap.Any(m => m.IsDeleted != true &&
                                                                                                                   m.ProductAttributeValues.AttributeId == attribute.AttributeId &&
                                                                                                                   m.ProductAttributeValues.AttributeValueId == attributeValue.AttributeValueId));

                    if (attributeRelatedProducts.Count() > 1)
                    {
                        foreach (var currentProductAttribute in selectedProduct.ProductAttributeValuesMap.Where(am => am.IsDeleted != true && am.ProductAttributeValues.AttributeId != attribute.AttributeId))
                        {
                            var resultQuery = attributeRelatedProducts.Where(p => p.ProductAttributeValuesMap.Any(m => m.IsDeleted != true && m.ProductAttributeValues.AttributeId == currentProductAttribute.ProductAttributeValues.AttributeId &&
                                                                                                              m.ProductAttributeValues.AttributeValueId == currentProductAttribute.AttributeValueId));

                            if (resultQuery.Count() == 1)
                            {
                                attributeRelatedProducts = resultQuery;
                                break;
                            }

                            if (resultQuery.Count() != 0)
                            {
                                attributeRelatedProducts = resultQuery;
                            }
                        }
                    }

                    var isAttributeAvailableWithCurrentSelection = true;

                    var attribiteStatusCheckQuery = relatedProducts.Where(m => m.SKUCode != selectedProduct.SKUCode &&
                                                                                m.ProductAttributeValuesMap.Any(p => p.IsDeleted != true &&
                                                                                                                     p.ProductAttributeValues.AttributeId == attribute.AttributeId &&
                                                                                                                     p.ProductAttributeValues.AttributeValueId == attributeValue.AttributeValueId));

                    foreach (var currentProductAttribute in selectedProduct.ProductAttributeValuesMap.Where(m => m.ProductAttributeValues.AttributeId != attribute.AttributeId))
                    {
                        attribiteStatusCheckQuery = attribiteStatusCheckQuery.Where(m => m.ProductAttributeValuesMap.Any(p => p.IsDeleted != true &&
                                                                                                                              p.ProductAttributeValues.AttributeId == currentProductAttribute.ProductAttributeValues.AttributeId &&
                                                                                                                              p.ProductAttributeValues.AttributeValueId == currentProductAttribute.AttributeValueId));

                        isAttributeAvailableWithCurrentSelection = attribiteStatusCheckQuery.Any();
                    }

                    var isSelectedAttributeValue = attributeValue.AttributeValueId == selectedAttributeValue?.AttributeValueId;

                    attributeViewModel.AttributeValues.Add(new ProductDetailAttributeValueViewModel
                    {
                        IsAvailableWithCurrentSelection = (isSelectedAttributeValue || isAttributeAvailableWithCurrentSelection),
                        IsSelected = attributeValue.AttributeValueId == selectedAttributeValue?.AttributeValueId,
                        RelatedProductId = attributeRelatedProducts.FirstOrDefault()?.ProductId ?? selectedProduct.ProductId,
                        Value = attributeValue.Value,
                        Color = !string.IsNullOrEmpty(attributeValue.Color?.Trim()) ? attributeValue.Color : attributeValue.Value,
                        IsColorTyped = attribute.IsColorTyped
                    });
                }

                processedProductAttributes.Add(attributeViewModel);
            }

            return processedProductAttributes;
        }

        public ActionResult _ProductByAttributeSelector(string skuCode, int? quantity = null, int? productId = null)
        {
            var baseProduct = _productServices.GetProductMasterByProductCode(skuCode, CurrentTenantId);
            var model = new ProductDetailViewModel();

            if (baseProduct.ProductType == ProductKitTypeEnum.ProductByAttribute)
            {
                model = GetSelectedChildProduct(productId, baseProduct);
            }
            else
            {
                model.Product = baseProduct;
            }

            model.Prices = _tenantWebsiteService.GetPricesForProducts(new List<int> { model.Product.ProductId }, CurrentTenantWebsite.SiteID, CurrentUser?.AccountId).FirstOrDefault();
            model.ParentProductId = baseProduct.ProductId;
            model.ParentProductType = baseProduct.ProductType;
            model.ParentProductSKUCode = baseProduct.SKUCode;
            model.Quantity = quantity;

            return PartialView(model);
        }

        public JsonResult EditCartItem(int cartId, decimal quantity)
        {
            var result = _tenantWebsiteService.UpdateCartItemQuantity(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, Session.SessionID, cartId, quantity);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCartItem(int productId, decimal quantity)
        {
            var cartItemProductId = _tenantWebsiteService.AddOrUpdateCartItem(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, Session.SessionID, productId, quantity);
            return Json(cartItemProductId, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveCartItem(int cartId)
        {
            var result = _tenantWebsiteService.RemoveCartItem(cartId, CurrentTenantWebsite.SiteID, CurrentUserId, Session.SessionID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCartItemsCollectionAvailability(int id, string name, string address)
        {
            Session["collectionPointId"] = id;
            Session["collectionPointName"] = name;
            Session["collectionPointAddress"] = address;

            var websiteCartItems = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, HttpContext.Session.SessionID).ToList();

            var itemAvailabilities = websiteCartItems.Select(c =>
                {
                    return new
                    {
                        Id = c.Id.ToString(),
                        IsAvailable = GetCollectionAvailabilityStatus(c.ProductId, c.Quantity, id)
                    };
                });

            return Json(itemAvailabilities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCartItemsDeliveryAvailability(int? id, string postCode)
        {
            Session["shippingAddressId"] = id;
            Session["shippingAddressPostCode"] = postCode;

            var websiteCartItems = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, HttpContext.Session.SessionID).ToList();

            var itemAvailabilities = websiteCartItems.Select(c =>
            {
                return new
                {
                    Id = c.Id.ToString(),
                    IsAvailable = GetDeliveryAvailabilityStatus(c.ProductId, (c.Weight??0), postCode)
                };
            });

            return Json(itemAvailabilities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddKitProductCartItem(int productId, List<KitProductCartSession> kitProductCartItems)
        {
            var cartItemProductId = _tenantWebsiteService.AddOrUpdateCartItem(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, Session.SessionID, productId, 1, kitProductCartItems);
            return Json(cartItemProductId, JsonRequestBehavior.AllowGet);
        }
    }
}