using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Unity;
using WarehouseEcommerce.Helpers;
using WebGrease.Css.Extensions;

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

        public ProductsController(IProductServices productServices, IUserService userService, IProductLookupService productlookupServices, ITenantWebsiteService tenantWebsiteService, IProductPriceService productPriceService, ITenantsCurrencyRateServices tenantsCurrencyRateServices, IMapper mapper, ICommonDbServices commonDbServices, ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IActivityServices activityServices, ITenantsServices tenantServices)
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
            _tenantWebsiteService = tenantWebsiteService;
        }
        // GET: Products

        public ActionResult list(string category, int? sort, string filter, string search, int? page, int? pagesize = 10, string values = "")
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
                    data.ToList().ForEach(u => u.SellPrice = (Math.Round((_productPriceService.GetProductPriceThresholdByAccountId(u.ProductId, null).SellPrice) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value), 2)));
                    if (CurrentUserId <= 0)
                    {
                        if (GaneWishListItemsSessionHelper.GetWishListItemsSession().Count > 0)
                        {
                            foreach (var item in GaneWishListItemsSessionHelper.GetWishListItemsSession())
                            {
                                var wistlist = GaneWishListItemsSessionHelper.GetWishListItemsSession().ToList().Select(u => new WebsiteWishListItem()
                                {
                                    ProductMaster = _productServices.GetProductMasterById(item.ProductId),

                                }).FirstOrDefault();
                                var list = data.FirstOrDefault(u => u.ProductId == item.ProductId);
                                if (list.WebsiteCartItems.Count <= 0)
                                {
                                    list.WebsiteWishListItems.Add(wistlist);
                                }


                            }
                        }
                    }



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

            var selectedProduct = product;

            ViewBag.BaseProduct = product;

            if (product.ProductType == ProductKitTypeEnum.ProductByAttribute)
            {
                selectedProduct = GetSelectedProductByAttribute(productId, product);
            }

            ViewBag.Category = _tenantWebsiteService.CategoryAndSubCategoryBreedCrumb(CurrentTenantWebsite.SiteID, product.ProductId);
            ViewBag.SubCategory = _tenantWebsiteService.CategoryAndSubCategoryBreedCrumb(CurrentTenantWebsite.SiteID, SubCategory: ViewBag.Category);
            if (ViewBag.SubCategory != null && !string.IsNullOrEmpty(ViewBag.SubCategory))
            {
                var category = ViewBag.SubCategory;
                ViewBag.SubCategory = ViewBag.Category;
                ViewBag.Category = category;

            }
            selectedProduct.SellPrice = Math.Round((selectedProduct.SellPrice ?? 0) * ((!currencyyDetail.Rate.HasValue || currencyyDetail.Rate <= 0) ? 1 : currencyyDetail.Rate.Value), 2);
            if (product.ProductType == ProductKitTypeEnum.Grouped)
            {
                return RedirectToAction("GroupedProductDetail", "Products", new { sku = sku });
            }
            else if (product.ProductType == ProductKitTypeEnum.Kit)
            {
                return RedirectToAction("KitProductDetail", "Products", new { sku = sku });
            }

            return View(selectedProduct);
        }

        public ActionResult GroupedProductDetail(string sku)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            product.SellPrice = Math.Round((product.SellPrice ?? 0) * (currencyyDetail.Rate ?? 0), 2);
            var productTabs = product.ProductKitItems.Where(u => u.ProductKitType == ProductKitTypeEnum.Grouped && u.IsActive == true).Select(u => u.ProductKitTypeId).ToList();
            ViewBag.GroupedTabs = _productlookupServices.GetProductKitTypes(productTabs);
            return View(product);
        }
        public ActionResult KitProductDetail(string sku)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;
            ViewBag.SiteDescription = caCurrent.CurrentTenantWebSite().SiteDescription;
            var product = _productServices.GetProductMasterByProductCode(sku, CurrentTenantId);
            product.SellPrice = Math.Round((product.SellPrice ?? 0) * ((currencyyDetail.Rate <= 0 || !currencyyDetail.Rate.HasValue || currencyyDetail == null) ? 1 : currencyyDetail.Rate.Value), 2);
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

        public PartialViewResult _CartItemsPartial(int? productId = null)
        {
            var SessionKey = HttpContext.Session.SessionID;
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            var models = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, SessionKey).Where(u => (!productId.HasValue || u.ProductId == productId));
            ViewBag.CurrencySymbol = currencyyDetail.Symbol;

            return PartialView(models.ToList());
        }

        public JsonResult AddWishListItem(int ProductId, bool isNotfication)
        {

            var ProductCheck = GaneWishListItemsSessionHelper.GetWishListItemsSession().FirstOrDefault(u => u.ProductId == ProductId);
            if (ProductCheck == null)
            {
                var Product = _productServices.GetProductMasterById(ProductId);
                var model = new OrderDetail();
                model.ProductMaster = Product;
                model.Qty = 0;
                model.ProductId = ProductId;
                model = _commonDbServices.SetDetails(model, null, "SalesOrders", "");
                ViewBag.CartModal = false;
                var Details = _mapper.Map(model, new OrderDetailSessionViewModel());
                Details.isNotfication = isNotfication;
                GaneWishListItemsSessionHelper.UpdateWishListItemsSession("", Details, false);
                if (CurrentUserId > 0)
                {
                    _tenantWebsiteService.AddOrUpdateWishListItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, (GaneWishListItemsSessionHelper.GetWishListItemsSession()));
                }
                return Json(_tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).Count(), JsonRequestBehavior.AllowGet);

            }
            if (CurrentUserId > 0)
            {
                _tenantWebsiteService.AddOrUpdateWishListItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, (GaneWishListItemsSessionHelper.GetWishListItemsSession()));
            }
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

        public PartialViewResult _wishlistItems(int? ProductId)
        {
            if (ProductId.HasValue)
            {
                if (CurrentUserId > 0)
                {
                    var count = _tenantWebsiteService.RemoveWishListItem((ProductId ?? 0), CurrentTenantWebsite.SiteID, CurrentUserId);
                    return PartialView(_tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).ToList());
                }

                var data = GaneWishListItemsSessionHelper.GetWishListItemsSession().ToList().Select(u => new WebsiteWishListItem()
                {
                    ProductMaster = _productServices.GetProductMasterById(u.ProductId),

                });
                return PartialView(data.ToList());
            }
            else
            {
                if (CurrentUserId > 0)
                {
                    var model = _tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).ToList();
                    return PartialView(model);
                }
                var data = GaneWishListItemsSessionHelper.GetWishListItemsSession().ToList().Select(u => new WebsiteWishListItem()
                {
                    ProductMaster = _productServices.GetProductMasterById(u.ProductId),

                });
                return PartialView(data.ToList());

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
            ViewBag.BaseProduct = product;
            ViewBag.Quantity = quantity;
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

            return PartialView(selectedProduct);
        }

        private ProductMaster GetSelectedProductByAttribute(int? productId, ProductMaster product)
        {
            ProductMaster selectedProduct;
            var relatedProducts = _productServices.GetAllProductInKitsByKitProductId(product.ProductId).Where(k => k.IsActive == true);
            selectedProduct = relatedProducts.FirstOrDefault(p => p.IsDeleted != true && p.IsActive == true && (p.ProductId == productId || (productId == null && p.ProductId != product.ProductId))) ?? product;
            ViewBag.AvailableAttributes = relatedProducts
                                            .SelectMany(a => a.ProductAttributeValuesMap.Where(p => p.IsDeleted != true).Select(k => k.ProductAttributeValues))
                                            .GroupBy(a => a.ProductAttributes)
                                            .ToDictionary(g => g.Key, g => g.OrderBy(av => av.AttributeValueId)
                                                                                            .GroupBy(av => av.AttributeValueId)
                                                                                            .Select(av => av.First())
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

        public  JsonResult EditCartItem(int productId, decimal quantity)
        {
            var cartItem = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, Session.SessionID).FirstOrDefault(u => u.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Qty = quantity;
            }
            var cartedItem = _tenantWebsiteService.AddOrUpdateCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, Session.SessionID,productId,quantity);
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AddCartItem(int productId, decimal quantity)
        {
            var currencyyDetail = Session["CurrencyDetail"] as caCurrencyDetail;
            var cartItem = _tenantWebsiteService.GetAllValidCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, Session.SessionID).FirstOrDefault(u => u.ProductId == productId);
            if (cartItem != null)
            {
                quantity=cartItem.Qty += quantity;
            }
           
            var cartedItem = _tenantWebsiteService.AddOrUpdateCartItems(CurrentTenantWebsite.SiteID, CurrentUserId, CurrentTenantId, Session.SessionID, productId, quantity, currencyyDetail.Rate, currencyyDetail.Id);
            return Json(cartedItem.ProductId,JsonRequestBehavior.AllowGet);

        }

        public JsonResult RemoveCartItem(int productId)
        {
            _tenantWebsiteService.RemoveCartItem(productId, CurrentTenantWebsite.SiteID, CurrentUserId, Session.SessionID);
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddKitProductCartItem(List<KitProductCartSession> kitProductCartItems)
        {

            return _CartItemsPartial();
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