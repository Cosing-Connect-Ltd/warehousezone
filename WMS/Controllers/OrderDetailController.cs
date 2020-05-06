using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using WMS.Helpers;
using AutoMapper;

namespace WMS.Controllers
{
    public class OrderDetailController : BaseController
    {
        public IStockTakeApiService StockTakeApiService;
        private readonly ICommonDbServices _commonDbServices;
        private readonly IProductServices _productServices;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly IProductPriceService _productPriceService;
        private readonly ICoreOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderDetailController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IStockTakeApiService stockTakeApiService,
            ICommonDbServices commonDbServices, IProductServices productServices, ITenantLocationServices tenantLocationServices, IProductPriceService productPriceService, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            StockTakeApiService = stockTakeApiService;
            _commonDbServices = commonDbServices;
            _productServices = productServices;
            _tenantLocationServices = tenantLocationServices;
            _productPriceService = productPriceService;
            _orderService = orderService;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">POID</param>
        /// <returns></returns>
        public ActionResult Create()
        {

            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var allProducts = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList();

            ViewBag.Products = new SelectList(allProducts, "ProductId", "NameWithCode");

            var products = new SelectList(allProducts, "ProductId", "NameWithCode").ToList();
            products.Insert(0, new SelectListItem() { Value = "0", Text = "Other / New Product" });

            ViewBag.Products = new SelectList(products);

            ViewBag.GlobalWarranties = new SelectList(LookupServices.GetAllTenantWarrenties(CurrentTenantId), "WarrantyID", "WarrantyName");
            var taxes = (from gtax in LookupServices.GetAllValidGlobalTaxes().Where(a => a.CountryID == CurrentTenant.CountryID)
                         select new
                         {
                             TaxId = gtax.TaxID,
                             TaxName = gtax.TaxName + " - " + gtax.PercentageOfAmount + " %"

                         }).ToList();
            ViewBag.GlobalTaxes = new SelectList(taxes, "TaxId", "TaxName");
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "POID,WarehouseId,ExpectedDate,CancelDate,Notes,ProductId,Qty,Price,SupplierCode,OrderID")] OrderDetail podetail)
        {
            // get properties of tenant
            if (ModelState.IsValid)
            {
                OrderService.SaveOrderDetail(podetail, CurrentTenantId, CurrentUserId);
                return RedirectToAction("Create", new { id = podetail.OrderID });
            }

            var supplierid = OrderService.GetOrderById(podetail.OrderID).AccountID;
            ViewBag.Supplierid = supplierid;
            ViewBag.productId = 0;
            var pr = OrderService.GetAllOrderDetailsForOrderAccount(supplierid.Value, podetail.OrderID, CurrentTenantId)
              .Select(p => new
              {
                  ProductID = p.ProductId,
                  Description = string.Format("{0} - {1}", p.ProductMaster.Name.Length >= 120 ? p.ProductMaster.Name.Substring(0, 120) : p.ProductMaster.Name, p.ProductMaster.SKUCode)
              }).ToList();

            var prod = pr.Distinct();
            ViewBag.ProductId = new SelectList(prod, "ProductID", "Description");

            ViewBag.WarehouseId = new SelectList(_tenantLocationServices.GetAllTenantLocations(CurrentTenantId), "WarehouseId", "WarehouseName");
            ViewBag.POID = podetail.OrderID;
            ViewBag.PoDetail = OrderService.GetAllValidOrderDetailsByOrderId(podetail.OrderID);
            ViewBag.Groups = new SelectList(from p in LookupServices.GetAllValidProductGroups(CurrentTenantId)
                                            where (p.TenentId == CurrentTenantId)
                                            select new
                                            {
                                                ProductGroupId = p.ProductGroupId,
                                                ProductGroup = p.ProductGroup
                                            }, "ProductGroupId", "ProductGroup");
            return View(podetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PODetailID,POID,WarehouseId,ExpectedDate,CancelDate,Notes,ProductId,Qty,Price,SupplierCode")] OrderDetail podetail)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                OrderService.SaveOrderDetail(podetail, CurrentTenantId, CurrentUserId);
            }
            return RedirectToAction("Create", "PODetail", new { id = podetail.OrderID });

        }

        public ActionResult RemoveProduct(int PODetailID, int POID)
        {
            OrderService.RemoveOrderDetail(PODetailID, CurrentTenantId, CurrentUserId);
            return RedirectToAction("Create", new { id = POID });

        }

        public JsonResult jsonpricehistory(int pid)
        {
            try
            {
                var prod = _productServices.GetPriceHistoryForProduct(pid, CurrentTenantId);
                return Json(prod, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, isValid = false, isException = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult IsProductAvailable(int ProductId, int POID)
        {
            var result = OrderService.GetOrderDetailsForProduct(POID, ProductId, CurrentTenantId).Count();
            return Json(result <= 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _OrderDetails(int? Id, string pageSessionToken)
        {
            ViewBag.cases = false;
            ViewBag.setName = "gridViewOrdDet";
            ViewBag.routeValues = new { Controller = "OrderDetail", Action = "_OrderDetails" };

            if (!String.IsNullOrEmpty(Request.Params["AccountId"]) && GaneStaticAppExtensions.ParseToNullableInt(Request.Params["AccountId"]) > 0)
            {
                var accountId = (int.Parse(Request.Params["AccountId"]));
                ViewBag.CurrencySymbol = AccountServices.GetAccountsById(accountId).GlobalCurrency.Symbol;
            }

            if (Id.HasValue && Id > 0)
            {
                ViewBag.setName = "gridViewOrdDetEdit";
                ViewBag.OrderId = Id.Value;
                ViewBag.routeValues = new { Controller = "OrderDetail", Action = "_OrderDetails", id = Id };
                var order = OrderService.GetOrderById(Id.Value);
                if (order != null)
                {
                    var sessionODList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(pageSessionToken);
                    if (sessionODList == null || sessionODList.Count < 1 && !GaneOrderDetailsSessionHelper.IsDictionaryContainsKey(pageSessionToken))
                    {
                        var detailItems = order.OrderDetails.OrderBy(o => o.OrderDetailID).ToList();
                        sessionODList = _mapper.Map(detailItems, sessionODList);
                    }
                    GaneOrderDetailsSessionHelper.SetOrderDetailSessions(pageSessionToken, sessionODList);
                    ViewBag.CurrencySymbol = order?.AccountCurrency?.Symbol;

                }
            }

            var model = GaneOrderDetailsSessionHelper.GetOrderDetailSession(pageSessionToken) ?? new List<OrderDetailSessionViewModel>();
            return PartialView("_OrderDetails", model);
        }

        public JsonResult _SetValue(int? Id)
        {
            Session["dId"] = Id;
            return Json(string.Empty);
        }
        [HttpPost]
        public JsonResult _SaveDetail(OrderDetail model, ProductDetailRequest productRequest)
        {
            if (productRequest != null && productRequest.IsNewProduct && productRequest.ProductName != null)
            {
                var product = StockTakeApiService.CreateProductOnStockTake(productRequest);
                model.ProductId = product.Result.ProductId;
            }

            model = _commonDbServices.SetDetails(model, productRequest.AccountId, Request.UrlReferrer.AbsolutePath, productRequest.ProcessByType);

            if (Request.UrlReferrer.AbsolutePath.Contains("SalesOrder") && model.ProductId > 0 && model.OrderDetailID < 1)
            {
                var minPrice = _productPriceService.GetProductPriceThresholdByAccountId(model.ProductId, productRequest != null ? productRequest.AccountId : 0);
                if (model.Price < minPrice.MinimumThresholdPrice)
                {
                    minPrice.Success = false;
                    model.OrderDetailStatusId = (int)OrderStatusEnum.AwaitingAuthorisation;
                    if (productRequest.ThresholdAcknowledged)
                    {
                        UpdatedOrderDetails(model, productRequest.CaseQuantity, productRequest.PageSessionToken);
                        return Json(new { error = "" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { error = minPrice.FailureMessage, Threshold = minPrice }, JsonRequestBehavior.AllowGet);
                }
            }

            UpdatedOrderDetails(model, productRequest.CaseQuantity, productRequest.PageSessionToken, productRequest.IsTransferAdd);
            return Json(new { error = "" }, JsonRequestBehavior.AllowGet);
        }

        private void UpdatedOrderDetails(OrderDetail model, decimal? casequantity, string pageGuid, bool isTransferAdd = false)
        {
            if (!Request.UrlReferrer.AbsolutePath.Contains("ProcessOrder"))
            {
                if (Request.UrlReferrer.AbsolutePath.Contains("TransferOrder"))
                {
                    var details = _mapper.Map(model, new OrderDetailSessionViewModel());
                    details.CaseQuantity = casequantity;

                    GaneOrderDetailsSessionHelper.UpdateOrderDetailSession(pageGuid, details, false, isTransferAdd);
                }
                else
                {
                    var details = _mapper.Map(model, new OrderDetailSessionViewModel());
                    details.CaseQuantity = casequantity;
                    GaneOrderDetailsSessionHelper.UpdateOrderDetailSession(pageGuid, details, false);
                }
            }
            else
            {
                OrderService.SaveOrderDetail(model, CurrentTenantId, CurrentUserId);
            }
        }


        public PartialViewResult _OrderDetail(int? Id, string pageSessionToken)
        {
            ViewBag.cases = false;
            DateTime? exp_dte = null;
            var orderId = 0;
            int? accountId = 0;
            if (!string.IsNullOrEmpty(Request.Params["OrderID"]))
            {
                orderId = int.Parse(Request.Params["OrderID"]);
            }
            if (Request.Params["exp_date"] != "")
            {
                exp_dte = DateTime.Parse(Request.Params["exp_date"].ToString());
            }

            var value = int.Parse(Request.Params["did"] ?? "0");
            if (Request.Params["rec_qty"] != null)
                ViewBag.RecQty = Request.Params["rec_qty"].ToString();

            //ViewBag.Products = new SelectList(_productServices.GetAllValidProductMasters(CurrentTenantId).ToList(), "ProductId", "NameWithCode");
            ViewBag.ProductAccounts = new SelectList(new List<ProductAccountCodes>(), "ProdAccCodeID", "ProdAccCode");

            if (!string.IsNullOrEmpty(Request.Params["account"]))
            {
                 accountId = int.Parse(Request.Params["account"]);
               
                var data =
                (from pac in AccountServices.GetAllProductAccountCodesByAccount(accountId??0)
                 select new
                 {
                     pac.ProdAccCodeID,
                     pac.ProdAccCode
                 }).ToList();
                ViewBag.ProductAccounts = new SelectList(data, "ProdAccCodeID", "ProdAccCode");
                //ViewBag.OrderingNotes = AccountServices.GetAllProductAccountCodesByAccount(accountId)?.FirstOrDefault()?.ProdOrderingNotes;
            }
            if (!string.IsNullOrEmpty(Request.Params["product"]))
            {
                var productid = int.Parse(Request.Params["product"]);
                var product = _productServices.GetProductMasterById(productid);
                if (product != null)
                {
                    ViewBag.ProductCurrentPrice = product.SellPrice;
                }
            }

            ViewBag.GlobalWarranties = new SelectList(LookupServices.GetAllTenantWarrenties(CurrentTenantId), "WarrantyID", "WarrantyName");

            var taxes = (from gtax in LookupServices.GetAllValidGlobalTaxes(CurrentTenant.CountryID)
                         select new
                         {
                             TaxId = gtax.TaxID,
                             TaxName = gtax.TaxName + " - " + gtax.PercentageOfAmount + " %"

                         }).ToList();

            ViewBag.GlobalTaxes = new SelectList(taxes, "TaxId", "TaxName");

            if (value == 0)
            {
                ViewBag.IsTransferOrderAdd = true;
                return PartialView("Create", new OrderDetailSessionViewModel
                {
                    ExpectedDate = exp_dte,
                    OrderID = !string.IsNullOrEmpty(Request.Params["oid"]) ? (int.Parse(Request.Params["oid"])) : orderId,
                    TenentId = CurrentTenantId,
                    WarehouseId = CurrentWarehouseId
                });
            }
            else
            {
                if (Request.UrlReferrer.AbsolutePath.Contains("ProcessOrder"))
                {
                    var cObject = OrderService.GetOrderDetailsById(value);
                    ViewBag.productId = cObject.ProductId;
                    var product = _productServices.GetProductMasterById(cObject.ProductId);

                    ViewBag.cases = product?.ProcessByCase;
                    ViewBag.processcase = product?.ProductsPerCase == null ? 1 : product.ProductsPerCase;
                    if (product?.ProcessByCase != null && product?.ProcessByCase == true)
                    {
                        ViewBag.caseProcess = (cObject.Qty / (product?.ProductsPerCase == null ? 1 : product.ProductsPerCase));
                    }
                    var cOrderSessionViewModel = _mapper.Map(cObject, new OrderDetailSessionViewModel());
                    
                    return PartialView("Create", cOrderSessionViewModel);
                }

                var odList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(pageSessionToken);

                var model = odList.FirstOrDefault(a => a.OrderDetailID == value && a.IsDeleted != true);
                if (model != null)
                {
                    ViewBag.productId = model.ProductId;
                    var product = _productServices.GetProductMasterById(model.ProductId);
                    ViewBag.cases = product?.ProcessByCase;
                    ViewBag.processcase = product?.ProductsPerCase == null ? 1 : product.ProductsPerCase;
                    if (product?.ProcessByCase != null && product?.ProcessByCase == true)
                    {
                        ViewBag.caseProcess = (model.Qty / (product?.ProductsPerCase == null ? 1 : product.ProductsPerCase));
                    }
                    ViewBag.BuyingPrice =Math.Round((_productPriceService.GetProductPriceThresholdByAccountId(model.ProductId, accountId ?? 0).MinimumSellPrice??0),2);
                    ViewBag.percentageMargin = product.PercentMargin;
                }
                if (model == null)
                {
                    model = new OrderDetailSessionViewModel() { OrderID = orderId };
                    ViewBag.IsTransferOrderAdd = true;
                }

                return PartialView("Create", model);
            }
        }

        public JsonResult _ProductPriceDetail()
        {
            int check;

            if (!string.IsNullOrEmpty(Request.Params["ProductId"]) && string.IsNullOrEmpty(Request.Params["AccountId"]))
            {
                var prodId = (int.Parse(Request.Params["ProductId"]));
                var products = _productServices.GetProductMasterById(prodId);
                var prices = new
                {
                    Price = 0,
                    products.ProcessByCase,
                    products.ProcessByPallet,
                    products.CasesPerPallet,
                    products.ProductsPerCase,
                    products.AllowModifyPrice,
                    products.AllowZeroSale,
                    products.BuyPrice,
                    products.EnableWarranty,
                    percentageMargin = products.PercentMargin,
                    //Converting LandedCost into percentage and adding it into PercentMargin
                    PercentMargin = Math.Round((_productPriceService.GetProductPriceThresholdByAccountId(prodId, 0).MinimumSellPrice ?? 0), 2),
                    ProductsPerPallet = products.CasesPerPallet * products.ProductsPerCase,
                    ShowPriceAlert = CurrentWarehouse.ShowPriceAlertInSalesOrder
                };


                return Json(prices, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(Request.Params["ProductId"]) || string.IsNullOrEmpty(Request.Params["AccountId"]))
            {
                return Json(new { Price = 0 }, JsonRequestBehavior.AllowGet);
            }
            if (!int.TryParse(Request.Params["ProductId"].ToString(), out check))
            {
                return Json(new { Price = 0 }, JsonRequestBehavior.AllowGet);
            }
            var productId = (int.Parse(Request.Params["ProductId"]));
            var accountId = (int.Parse(Request.Params["AccountId"]));
            var product = _productServices.GetProductMasterById(productId);

            var price = new
            {
                Price = Request.UrlReferrer.AbsoluteUri.Contains("PurchaseOrder") ? _productPriceService.GetLastPurchaseProductPriceForAccount(productId, accountId) : _productPriceService.GetLastSoldProductPriceForAccount(productId, accountId),
                product.ProcessByCase,
                product.ProcessByPallet,
                product.CasesPerPallet,
                product.ProductsPerCase,
                product.AllowModifyPrice,
                product.AllowZeroSale,
                ProductsPerPallet = product.CasesPerPallet * product.ProductsPerCase,
                product.BuyPrice,
                product.EnableWarranty,
                percentageMargin = product.PercentMargin,
                //Converting LandedCost into percentage and adding it into PercentMargin
                PercentMargin = Math.Round((_productPriceService.GetProductPriceThresholdByAccountId(productId, accountId).MinimumSellPrice??0),2),
                purchaseOrder = Request.UrlReferrer.AbsoluteUri.Contains("PurchaseOrder") ? true : false,
                ShowPriceAlert = CurrentWarehouse.ShowPriceAlertInSalesOrder
            };

            return Json(price, JsonRequestBehavior.AllowGet);
        }

        public JsonResult _RemoveProduct(int? Id, string pageSessionToken)
        {
            var Qunatity = _orderService.QunatityForOrderDetail(Id ?? 0);
            if (Qunatity > 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(pageSessionToken) && Id.HasValue)
            {
                GaneOrderDetailsSessionHelper.RemoveOrderDetailSession(pageSessionToken, Id.Value);
            }

            return Json(string.Empty);
        }

        public JsonResult _GetProductAccounts(int productid, int? accountid = null)
        {
            var allValidAccounts = AccountServices.GetAllValidProductAccountCodes(productid, accountid);

            if (allValidAccounts.Count() <= 0 || allValidAccounts == null) return Json(false, JsonRequestBehavior.AllowGet);

            var data = (from pac in allValidAccounts
                        select new
                        {
                            pac.ProdAccCodeID,
                            pac.ProdAccCode
                        });
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);

        }


        public JsonResult IsQunatityProcessed(decimal Qty, int OrderDetailId = 0, bool isdeleted = false)
        {


            var Qunatity = _orderService.QunatityForOrderDetail(OrderDetailId);

            if (Qty >= Qunatity)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductLargeDataComboBoxPartial(int? ProductId = null)
        {
            if (ProductId.HasValue)
            {
                ViewBag.productId = ProductId;
            }

            return PartialView("ProductLargeDataComboBoxPartial");
        }

        public JsonResult IsAllowZeroSale(int productid)
        {
            var ZeroPriceCheck = _productServices.GetProductMasterById(productid)?.AllowZeroSale;
            return Json(new{ zeroprice=ZeroPriceCheck ?? false,ShowPriceAlert=CurrentWarehouse.ShowPriceAlertInSalesOrder}, JsonRequestBehavior.AllowGet);

        }
        public ActionResult EditProductLargeCombobox(int ProductId)
        {

            return ProductLargeDataComboBoxPartial(ProductId);
        }


    }
}
