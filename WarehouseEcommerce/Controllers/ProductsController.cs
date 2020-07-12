using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            _lookupServices = lookupServices;
            _productServices = productServices;
            _commonDbServices = commonDbServices;
            _mapper = mapper;
            _productPriceService = productPriceService;
            _tenantWebsiteService = tenantWebsiteService;
        }
        // GET: Products

        public ActionResult list(string category, int? sort, string filter, string search, int? page, int? pagesize = 10, string values = "")
        {
            try
            {
                //ViewBag.groupId = group;
                ViewBag.Category = category;
                ViewBag.CurrentSort = sort;
                ViewBag.SortedValues = (sort ?? 1);
                ViewBag.pageList = new SelectList(from d in Enumerable.Range(1, 5) select new SelectListItem { Text = (d * 10).ToString(), Value = (d * 10).ToString() }, "Value", "Text", pagesize);
                ViewBag.searchString = search;
                ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
                var product = _tenantWebsiteService.GetAllValidProductWebsiteSearch(CurrentTenantWebsite.SiteID, category);
                ViewBag.Categories = _tenantWebsiteService.CategoryAndSubCategoryBreedCrumb(CurrentTenantWebsite.SiteID, Category: category);
                ViewBag.SubCategory = _tenantWebsiteService.CategoryAndSubCategoryBreedCrumb(CurrentTenantWebsite.SiteID, SubCategory: ViewBag.Category);
                if (ViewBag.SubCategory != null && !string.IsNullOrEmpty(ViewBag.SubCategory))
                {
                    var catg = ViewBag.SubCategory;
                    ViewBag.SubCategory = ViewBag.Category;
                    ViewBag.Categories = catg;

                }
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
                    data.ToList().ForEach(u => u.SellPrice = (Math.Round((_tenantWebsiteService.GetPriceForProduct(u.ProductId,CurrentTenantWebsite.SiteID)), 2)));




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
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);

            var selectedProduct = product;

            if (product?.ProductType == ProductKitTypeEnum.ProductByAttribute)
            {
                selectedProduct = GetSelectedProductByAttribute(productId, product);
            }

            if (selectedProduct == null)
            {
                return RedirectToAction("List", "Products", new { sku = sku });
            }

            product.ProductFiles = product.ProductFiles.Where(p => p.IsDeleted != true).ToList();
            selectedProduct.ProductFiles = selectedProduct.ProductFiles.Where(p => p.IsDeleted != true).ToList();

            ViewBag.BaseProduct = product;
            ViewBag.Category = _tenantWebsiteService.CategoryAndSubCategoryBreedCrumb(CurrentTenantWebsite.SiteID, product.ProductId);
            ViewBag.SubCategory = _tenantWebsiteService.CategoryAndSubCategoryBreedCrumb(CurrentTenantWebsite.SiteID, SubCategory: ViewBag.Category);

            if (ViewBag.SubCategory != null && !string.IsNullOrEmpty(ViewBag.SubCategory))
            {
                var category = ViewBag.SubCategory;
                ViewBag.SubCategory = ViewBag.Category;
                ViewBag.Category = category;

            }
            selectedProduct.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(selectedProduct.ProductId, CurrentTenantWebsite.SiteID),2);
            if (product.ProductType == ProductKitTypeEnum.Grouped)
            {
                return RedirectToAction("GroupedProductDetail", "Products", new { sku = sku });
            }
            else if (product.ProductType == ProductKitTypeEnum.Kit)
            {
                return RedirectToAction("KitProductDetail", "Products", new { sku = sku });
            }

            selectedProduct.ProductAttributeValuesMap = selectedProduct.ProductAttributeValuesMap.Where(p => p.IsDeleted != true).ToList();

            return View(selectedProduct);
        }

        public ActionResult GroupedProductDetail(string sku)
        {
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            product.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(product.ProductId, CurrentTenantWebsite.SiteID), 2);
            var productTabs = product.ProductKitItems.Where(u => u.ProductKitType == ProductKitTypeEnum.Grouped && u.IsActive == true).Select(u => u.ProductKitTypeId).ToList();
            ViewBag.GroupedTabs = _productlookupServices.GetProductKitTypes(productTabs);
            return View(product);
        }
        public ActionResult KitProductDetail(string sku)
        {
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            product.SellPrice = Math.Round(_tenantWebsiteService.GetPriceForProduct(product.ProductId, CurrentTenantWebsite.SiteID), 2);
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
            var model = (from product in _tenantWebsiteService.GetAllValidProductWebsiteSearch(CurrentTenantWebsite.SiteID, ProductName: searchkey)
                         select new ProductSearchResult
                         {
                             Id = product.ProductId,
                             Name = product.Name,
                             Path = product.ProductFiles.FirstOrDefault(u => Images.Contains(u.FilePath)).FilePath,
                             SKUCode = product.SKUCode,
                             SearchKey = searchkey
                         }).OrderBy(u => u.Id).Take(10).ToList();


            model.ForEach(x => x.Path = CurrentTenantWebsite.BaseFilePath + (string.IsNullOrEmpty(x.Path) ? "/UploadedFiles/Products/no_image.gif" : x.Path));


            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddToCart()
        {
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

            model.ShippmentAddresses = _mapper.Map(AccountServices.GetAllValidAccountAddressesByAccountId(CurrentUser.AccountId ?? 0).Where(u => u.AddTypeShipping == true && u.IsDeleted != true).ToList(), new List<AddressViewModel>());

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

            model.ShippingAddressId = Session["shippingAddressId"] != null ?(int)Session["shippingAddressId"] : (int?)null;

            var postCode = Session["shippingAddressPostCode"] != null ? (string)Session["shippingAddressPostCode"] : null;
            model.ShippingAddressPostCode = postCode;

            if (!string.IsNullOrEmpty(model.ShippingAddressPostCode?.Trim()))
            {
                model.WebsiteCartItems.ForEach(c =>
                {
                    c.IsAvailableForDelivery = GetDeliveryAvailabilityStatus(c.ProductId, c.Quantity, postCode);
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

        private bool? GetDeliveryAvailabilityStatus(int productId, decimal quantity, string postCode)
        {
            // TODO: Complete the Delivery availability status
            return string.IsNullOrEmpty(postCode?.Trim()) ? (bool?)null : true;
        }

        public JsonResult AddWishListItem(int ProductId, bool isNotfication)
        {
            var Product = _productServices.GetProductMasterById(ProductId);
            var model = new OrderDetail();
            model.ProductMaster = Product;
            model.Qty = 0;
            model.ProductId = ProductId;
            model = _commonDbServices.SetDetails(model, null, "SalesOrders", "");
            ViewBag.CartModal = false;
            var details = _mapper.Map(model, new OrderDetailSessionViewModel());
            details.isNotfication = isNotfication;
            _tenantWebsiteService.AddOrUpdateWishListItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, new List<OrderDetailSessionViewModel>() { details });
            return Json(_tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).Count(), JsonRequestBehavior.AllowGet);


        }

        public ActionResult WishList()
        {
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
        public JsonResult ChangeWishListStatus(int productId,bool notification)
        {

            return Json(
                _tenantWebsiteService.ChangeWishListStatus(productId, notification, CurrentTenantWebsite.SiteID,
                    CurrentUserId), JsonRequestBehavior.AllowGet);

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
            var products = _tenantWebsiteService.GetAllValidProductWebsiteSearch(CurrentTenantWebsite.SiteID, category, ProductName: productName);

            productFiltering.Manufacturer = _tenantWebsiteService.GetAllValidProductManufacturerGroupAndDeptByName(products).Select(u => u.Name).ToList();
            productFiltering.PriceInterval = _tenantWebsiteService.AllPriceListAgainstGroupAndDept(products);
            productFiltering.AttributeValues = _tenantWebsiteService.GetAllValidProductAttributeValuesByProductIds(products);
            productFiltering.subCategories = _productlookupServices.GetAllValidSubCategoriesByDepartmentAndGroup(products).ToList();
            productFiltering.Count = products.Count();

            return PartialView(productFiltering);
        }


        public JsonResult RemoveWishList(int ProductId)
        {
            var count = _tenantWebsiteService.RemoveWishListItem(ProductId, CurrentTenantWebsite.SiteID, CurrentUserId);
            GaneWishListItemsSessionHelper.RemoveWishListSession(ProductId);
            count = Math.Max(count, GaneWishListItemsSessionHelper.GetWishListItemsSession().Count);
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _KitProductAttributeDetail(string skuCode, decimal? quantity = null, int? productId = null)
        {
            var product = _productServices.GetProductMasterByProductCode(skuCode, CurrentTenantId);
            var selectedProduct = product;
            IEnumerable<SelectListItem> squares = Enumerable.Range(1, (quantity.HasValue ? Convert.ToInt32(quantity) : 1)).Select(u => new SelectListItem
            {
                Text = u.ToString(),
                Value = u.ToString()
            }
                ).ToList();
            ViewBag.QuantityList = new SelectList(squares, "Text", "Value");
            if (product.ProductType == ProductKitTypeEnum.ProductByAttribute)
            {
                selectedProduct = GetSelectedProductByAttribute(productId, product);
            }

            product.ProductFiles = product.ProductFiles.Where(p => p.IsDeleted != true).ToList();
            selectedProduct.ProductFiles = selectedProduct.ProductFiles.Where(p => p.IsDeleted != true).ToList();

            ViewBag.BaseProduct = product;
            ViewBag.Quantity = quantity;

            return PartialView(selectedProduct);
        }

        private ProductMaster GetSelectedProductByAttribute(int? productId, ProductMaster product)
        {
            ProductMaster selectedProduct;
            var relatedProducts = _productServices.GetAllProductInKitsByKitProductId(product.ProductId).Where(k => k.IsActive == true && k.ProductType != ProductKitTypeEnum.ProductByAttribute && k.IsDeleted != true);

            relatedProducts.ForEach(r => r.ProductAttributeValuesMap = r.ProductAttributeValuesMap.Where(p => p.IsDeleted != true).ToList());

            selectedProduct = relatedProducts.FirstOrDefault(p => p.ProductId == productId || productId == null);

            ViewBag.AvailableAttributes = relatedProducts
                                            .SelectMany(a => a.ProductAttributeValuesMap.Where(p => p.IsDeleted != true).Select(k => k.ProductAttributeValues))
                                            .GroupBy(a => a.ProductAttributes).OrderBy(u=>u.Key.SortOrder)
                                            .ToDictionary(g => g.Key, g => g.OrderBy(av => av.SortOrder)
                                                                                            .GroupBy(av => av.AttributeValueId)
                                                                                            .Select(av => av.First()).OrderBy(u=>u.ProductAttributes.SortOrder)
                                                                                            .ToList());
            ViewBag.RelatedProducts = relatedProducts;
            return selectedProduct;
        }

        public ActionResult _ProductByAttributeSelector(string skuCode, int? quantity = null, int? productId = null)
        {
            var product = _productServices.GetProductMasterByProductCode(skuCode, CurrentTenantId);
            var selectedProduct = product;
            ViewBag.BaseProduct = product;
            ViewBag.Quantity = quantity;
            if (product.ProductType == ProductKitTypeEnum.ProductByAttribute)
            {
                selectedProduct = GetSelectedProductByAttribute(productId, product);
            }

            return PartialView(selectedProduct);
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
                    IsAvailable = GetDeliveryAvailabilityStatus(c.ProductId, c.Quantity, postCode)
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