using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WMS.CustomBindings;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class InventoryTransactionController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _lookupServices;
        private readonly IPurchaseOrderService _purchaseOrderService;

        public InventoryTransactionController(ICoreOrderService orderService, IPurchaseOrderService purchaseOrderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, IProductLookupService productLookupService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _productLookupService = productLookupService;
            _lookupServices = lookupServices;
            _purchaseOrderService = purchaseOrderService;


        }
        // GET: InventoryTransaction
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        public ActionResult Create()
        {

            if (Request.UrlReferrer != null)
            {
                if (Request.UrlReferrer.AbsolutePath.ToLower().Contains("edit"))
                {
                    ViewBag.back = 1;
                }

                if (Request.UrlReferrer.AbsolutePath.ToLower().Contains("products"))
                {
                    ViewBag.back = 2;
                }
            }
            var allOrders = OrderService.GetAllOrders(CurrentTenantId, CurrentWarehouseId);
            ViewBag.OrderID = new SelectList(allOrders, "OrderId", "OrderNumber", allOrders.FirstOrDefault());
            return View();
        }

        [HttpPost]
        public ActionResult Create(InventoryTransaction model, string back)
        {
            try
            {
                InventoryTransactionTypeEnum transportType = model.InventoryTransactionTypeId;
                int productId = (int)Session["pId"];
                decimal qty = model.Quantity;
                int orderId = model.OrderID ?? default(int);

                Inventory.StockTransaction(productId, transportType, qty, orderId);

                if (back == "1")
                    return Redirect(Url.Action("Edit", "Products", new { id = Session["pId"] }) + "#product-inventory");
                return RedirectToAction("Index");
            }
            catch (Exception exp)
            {
                var allOrders = OrderService.GetAllOrders(CurrentTenantId, CurrentWarehouseId);

                ModelState.AddModelError("", exp.Message);
                ViewBag.OrderID = new SelectList(allOrders, "OrderId", "OrderNumber", allOrders.FirstOrDefault(a => a.OrderID == model.OrderID));
                return View(model);
            }
        }

        public ActionResult _InventoryTransList()
        {
            var viewModel = GridViewExtension.GetViewModel("_InventoryTransListGridView");

            if (viewModel == null)
                viewModel = InventoryListCustomBinding.CreateInventoryGridViewModel();

            return InventoryGridActionCore(viewModel);
        }


        public ActionResult _InventoryTransListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("_InventoryTransListGridView");
            viewModel.Pager.Assign(pager);
            return InventoryGridActionCore(viewModel);
        }

        public ActionResult _InventoryTransListFiltering(GridViewFilteringState filteringState)

        {
            var viewModel = GridViewExtension.GetViewModel("_InventoryTransListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return InventoryGridActionCore(viewModel);
        }

        public ActionResult _InventoryGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("_InventoryTransListGridView");
            viewModel.ApplySortingState(column, reset);
            return InventoryGridActionCore(viewModel);
        }

        public ActionResult InventoryGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    InventoryListCustomBinding.InventoryGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        InventoryListCustomBinding.InventoryGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );

            ViewData["TransactionTypesList"] = Enum.GetNames(typeof(InventoryTransactionTypeEnum)).ToList();

            return PartialView("_InventoryTransList", gridViewModel);
        }



        #region Goods Return

        public ActionResult GoodReturnsIndex()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public ActionResult GoodsReturn()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            Guid guid = Guid.NewGuid();
            ViewBag.DeliveryNo = GaneStaticAppExtensions.GenerateDateRandomNo();
            ViewBag.groupToken = guid.ToString();


            return View();
        }
        public ActionResult _GoodsReturn()
        {
            var pid = Request.Params["pid"];
            if (pid == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            int id = int.Parse(pid);
            var product = _productServices.GetProductMasterById(id);

            var prodLocations = _productServices.GetAllProductLocationsFromMaps(id);
            var locations = _productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId);

            var locs = locations.Where(a => prodLocations == null || prodLocations.Count() < 1 || prodLocations.Contains(a.LocationId)).ToList();
            ViewBag.Locations = new SelectList(locs, "LocationId", "LocationWithCode", prodLocations.FirstOrDefault());

            ViewBag.QuantityEnabled = true;
            if (product == null)
                return HttpNotFound();
            ViewBag.ProductName = product.NameWithCode;

            int orderid = int.Parse(Request.Params["order"]);

            if (product.Serialisable)
            {
                return PartialView("_AddSerial");
            }
            else
            {
                ViewBag.Title = "Save";
                return PartialView("_RecProduct", new InventoryTransaction { ProductId = pid.AsInt(), OrderID = orderid });
            }
        }

        public JsonResult _IsOrderValid(string order)
        {
            var corder = OrderService.GetValidSalesOrderByOrderNumber(order, CurrentTenantId);

            if (corder == null) return Json(false, JsonRequestBehavior.AllowGet);
            var model = new { orderid = corder.OrderID, Products = corder.OrderDetails.Select(p => new { Id = p.ProductMaster.ProductId, Name = p.ProductMaster.NameWithCode + ($"( Qty: {p.Qty}, Processed:{p.ProcessedQty}, Returned: { p.ReturnedQty  }  )") }), accountId = corder?.Account?.CompanyName };
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        public JsonResult _IsQuantityValid(int order, int product, int quantity)
        {

            var processedQuantity = OrderService.GetOrderProcessesDetailsForOrderProduct(order, product);
            var returnedQuantity = OrderService.GetAllReturnsForOrderProduct(order, product);

            if (processedQuantity == null || processedQuantity.Count == 0)
                return Json(new { error = true, errorcode = 1 }, JsonRequestBehavior.AllowGet);

            decimal totalDispatched = processedQuantity.Sum(a => a.QtyProcessed);
            decimal totalReturned = returnedQuantity.Sum(a => a.Quantity);

            if (quantity + totalReturned > totalDispatched)
                return Json(new { error = true, errorcode = 2 }, JsonRequestBehavior.AllowGet);

            return Json(new { error = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult _ReturnNon_SerProduct(int? order, int product, int quantity, InventoryTransactionTypeEnum type, string groupToken = null, int? locationid = null, int? lineId = null, string deliveryNumber = null)
        {
            string orderNumber = "";
            if (order.HasValue)
            {
                GoodsReturnRequestSync goodsReturnRequestSync = new GoodsReturnRequestSync
                {
                    OrderId = order ?? 0,
                    ProductId = product,
                    Quantity = quantity,
                    InventoryTransactionType = type,
                    deliveryNumber = deliveryNumber,
                    LocationId = locationid ?? 0,
                    tenantId = CurrentTenantId,
                    warehouseId = CurrentWarehouseId,
                    userId = CurrentUserId

                };
                Inventory.StockTransaction(goodsReturnRequestSync, groupToken);
            }
            else
            {
                orderNumber = GenerateNextOrderNumber((InventoryTransactionTypeEnum)type);
                GoodsReturnRequestSync goodsReturnRequestSync = new GoodsReturnRequestSync
                {
                    OrderId = order ?? 0,
                    ProductId = product,
                    Quantity = quantity,
                    InventoryTransactionType = type,
                    LocationId = locationid ?? 0,
                    tenantId = CurrentTenantId,
                    warehouseId = CurrentWarehouseId,
                    OrderDetailID = lineId,
                    deliveryNumber = deliveryNumber,
                    OrderNumber = orderNumber,
                    userId = CurrentUserId

                };

                Inventory.StockTransaction(goodsReturnRequestSync, groupToken);

            }
            if (type == InventoryTransactionTypeEnum.Returns || type == InventoryTransactionTypeEnum.Wastage || type == InventoryTransactionTypeEnum.WastedReturn)
            {
                return Json(new { orderid = order ?? 0, productId = product, orderNumber = orderNumber, groupToken = groupToken }, JsonRequestBehavior.AllowGet);

            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _GRList()
        {
            var pid = int.Parse(!string.IsNullOrEmpty(Request.Params["prodId"]) ? Request.Params["prodId"] : "0");
            var orderNumber = Request.Params["ordernumber"];
            var groupToken = Request.Params["groupToken"];
            var orderId = int.Parse(!string.IsNullOrEmpty(Request.Params["orderId"]) ? Request.Params["orderId"] : "0");
            var InventorytType = int.Parse(!string.IsNullOrEmpty(Request.Params["InType"]) ? Request.Params["InType"] : "0");

            if (pid > 0 && (!string.IsNullOrEmpty(orderNumber) || orderId > 0) && InventorytType > 0)
            {

                var model = _productServices.GetInventoryTransactionsReturns(pid, orderId, orderNumber, InventorytType, groupToken).Select(md => new
                {
                    md.InventoryTransactionId,
                    LocationName = _productLookupService.GetLocationById(md.LocationId ?? 0) != null ? _productLookupService.GetLocationById(md.LocationId ?? 0).LocationName : "",
                    md.Quantity,
                    SerialNo = md?.ProductSerial?.SerialNo,
                    Product = _productServices.GetProductMasterById(md.ProductId).Name
                }).ToList();
                return PartialView(model);
            }
            return PartialView();
        }

        public JsonResult _GRRemove(int id)
        {

            var sList = Session["grList"] as List<InventoryTransaction>;
            sList.Remove(sList.FirstOrDefault(a => a.InventoryTransactionId == id));
            _productServices.DeleteInventoryTransactionById(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public ActionResult _GoodsReturnGridView()
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "goodsreturngridview");
            if (viewModel == null)
                viewModel = GoodsReturnCustomBinding.CreateGoodsReturnGridViewModel();

            return _GoodsReturnGridActionCore(viewModel);


        }


        public ActionResult _GoodsReturnPaging(GridViewPagerState pager, int? id)
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "goodsreturngridview");
            viewModel.Pager.Assign(pager);
            return _GoodsReturnGridActionCore(viewModel);
        }

        public ActionResult _GoodsReturnFiltering(GridViewFilteringState filteringState, int? id)

        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "goodsreturngridview");
            viewModel.ApplyFilteringState(filteringState);
            return _GoodsReturnGridActionCore(viewModel);
        }

        public ActionResult _GoodsReturnSorting(GridViewColumnState column, bool reset, int? id)
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "goodsreturngridview");
            viewModel.ApplySortingState(column, reset);
            return _GoodsReturnGridActionCore(viewModel);
        }

        public ActionResult _GoodsReturnGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    GoodsReturnCustomBinding.GoodsReturnDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        GoodsReturnCustomBinding.GoodsReturnData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_GoodsReturnGridView", gridViewModel);
        }





        public ActionResult _GoodsReturnDetails(int ProcessId)
        {
            ViewBag.processId = ProcessId;

            var data = (from opd in OrderService.GetOrderProcessDetailsByProcessId(ProcessId)
                        select new
                        {
                            opd.ProductMaster.Name,
                            opd.QtyProcessed,
                            opd.ProductMaster.SKUCode,
                            opd.DateCreated,
                            opd.OrderProcessDetailID
                        }).ToList();
            return PartialView(data);

        }

        #endregion
        #region Wasted Goods Return
        public ActionResult WastedGoodsReturn()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var products = (from prds in _productServices.GetAllValidProductMasters(CurrentTenantId).ToList()
                            select new
                            {
                                prds.ProductId,
                                prds.SKUCode,
                                prds.NameWithCode
                            }).ToList();
            //ViewBag.wrProducts = new SelectList(products, "ProductId", "NameWithCode");
            Guid guid = Guid.NewGuid();
            ViewBag.groupToken = guid.ToString();
            return View();
        }
        public ActionResult _WastageGoodsReturn()
        {

            var pid = Request.Params["pid"];
            var losslyR = Request.Params["looslyRetrun"];
            if (pid == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            int id = int.Parse(pid);
            var product = _productServices.GetProductMasterById(id);

            var prodLocations = _productServices.GetAllProductLocationsFromMaps(id);
            var locations = _productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId);

            var locs = locations.Where(a => prodLocations == null || prodLocations.Count() < 1 || prodLocations.Contains(a.LocationId)).ToList();
            ViewBag.Locations = new SelectList(locs, "LocationId", "LocationWithCode", prodLocations.FirstOrDefault());
            ViewBag.WastageReasons = new SelectList(_productLookupService.GetAllWastageReasons(), "Id", "Description");
            int orderid = int.Parse(!string.IsNullOrEmpty(Request.Params["order"]) ? Request.Params["order"] : "0");
            ViewBag.QuantityEnabled = true;
            if (product == null)
                return HttpNotFound();
            ViewBag.ProductName = product.NameWithCode;
            if (product.Serialisable && losslyR == "false")
            {
                return PartialView("_AddSerial");
            }
            else if (product.ProcessByPallet && losslyR == "false" && caCurrent.CurrentWarehouse().EnableGlobalProcessByPallet)
            {
                ViewBag.PalletPerCase = _productServices.GetProductMasterById(id).ProductsPerCase;
                //decimal caseperpallet = (ViewBag.RequiredQuantity / ViewBag.PalletPerCase);
                ViewBag.cases = ViewBag.PalletPerCase;
                return PartialView("_AddPalletes");
            }
            else
            {
                ViewBag.Title = "Save";
                return PartialView("_RecProduct", new InventoryTransaction { ProductId = pid.AsInt(), OrderID = orderid });
            }
        }


        public ActionResult WastageIndex()
        {
            return View();
        }
        public ActionResult _WastageGridView()
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "wastagegridview");
            if (viewModel == null)
                viewModel = WastageCustomBinding.CreateGoodsReturnGridViewModel();

            return _WastageGridActionCore(viewModel);


        }


        public ActionResult _WastagePaging(GridViewPagerState pager, int? id)
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "wastagegridview");
            viewModel.Pager.Assign(pager);
            return _WastageGridActionCore(viewModel);
        }

        public ActionResult _WastageFiltering(GridViewFilteringState filteringState, int? id)

        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "wastagegridview");
            viewModel.ApplyFilteringState(filteringState);
            return _WastageGridActionCore(viewModel);
        }

        public ActionResult _WastageSorting(GridViewColumnState column, bool reset, int? id)
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "wastagegridview");
            viewModel.ApplySortingState(column, reset);
            return _WastageGridActionCore(viewModel);
        }

        public ActionResult _WastageGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    WastageCustomBinding.WastageDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        WastageCustomBinding.WastageData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_WastageGridView", gridViewModel);
        }





        public ActionResult _WastageDetails(int ProcessId)
        {
            ViewBag.processId = ProcessId;

            var data = (from opd in OrderService.GetOrderProcessDetailsByProcessId(ProcessId)
                        select new
                        {
                            opd.ProductMaster.Name,
                            opd.QtyProcessed,
                            opd.DateCreated,
                            opd.OrderProcessDetailID
                        }).ToList();
            return PartialView(data);

        }







        public ActionResult _WGRList()
        {
            var pid = int.Parse(!string.IsNullOrEmpty(Request.Params["prodId"]) ? Request.Params["prodId"] : "0");
            var orderNumber = Request.Params["ordernumber"];
            var groupToken = Request.Params["groupToken"];
            var orderId = int.Parse(!string.IsNullOrEmpty(Request.Params["orderId"]) ? Request.Params["orderId"] : "0");
            var InventorytType = int.Parse(!string.IsNullOrEmpty(Request.Params["InType"]) ? Request.Params["InType"] : "0");

            if (pid > 0 && (!string.IsNullOrEmpty(orderNumber) || orderId > 0) && InventorytType > 0)
            {

                var model = _productServices.GetInventoryTransactionsReturns(pid, orderId, orderNumber, InventorytType, groupToken).Select(md => new
                {
                    md.InventoryTransactionId,
                    LocationName = _productLookupService.GetLocationById(md.LocationId ?? 0) != null ? _productLookupService.GetLocationById(md.LocationId ?? 0).LocationName : "",
                    md.Quantity,
                    SerialNo = md?.ProductSerial?.SerialNo,
                    Product = _productServices.GetProductMasterById(md.ProductId).Name
                }).ToList();
                return PartialView(model);
            }
            return PartialView();

        }

        #endregion


        #region goodsandWastageReturn
        public JsonResult _VerifyPalleteReturns(string serial, int? pid, int? type)
        {
            bool status = false;
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;
            var recored = OrderService.GetVerifedPallet(serial, pid ?? 0, CurrentTenantId, warehouseId, type);
            if (recored != null)
            {
                if (recored.Comments == "[#Expired#]")
                {
                    return Json("Expired", JsonRequestBehavior.AllowGet);
                }
                List<string> values = new List<string>();
                values.Add(recored.PalletTrackingId.ToString());
                values.Add(recored.PalletSerial);
                values.Add(recored.RemainingCases.ToString());
                values.Add(recored.TotalCases.ToString());
                return Json(values, JsonRequestBehavior.AllowGet);
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult _SubmitPalleteSerials(List<string> serialList, int? pid, int? orderId, InventoryTransactionTypeEnum? type, int? palletTrackingId, string groupToken, string deliveryNumber = null)
        {

            var _inventoryTransactionsService = DependencyResolver.Current.GetService<IOrderService>();
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;
            List<PalleTrackingProcess> palleTrackingProcessList = new List<PalleTrackingProcess>();
            ViewBag.QuantityEnabled = true;
            if (orderId.HasValue)
            {

                foreach (var pallet in serialList)
                {
                    PalleTrackingProcess palleTrackingProcess = new PalleTrackingProcess();
                    decimal quantity = 0;
                    var palletdData = pallet.Split(new string[] { "#+#", "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (palletdData.Length >= 2)
                    {
                        int PalletTrackingId = 0;

                        if (!string.IsNullOrEmpty(palletdData[0]))
                        {
                            PalletTrackingId = int.Parse(palletdData[0]);
                            palleTrackingProcess.PalletTrackingId = PalletTrackingId;
                        }
                        if (!string.IsNullOrEmpty(palletdData[1]))
                        {
                            quantity = decimal.Parse(palletdData[1]);
                            palleTrackingProcess.ProcessedQuantity = quantity;
                        }
                    }

                    palleTrackingProcessList.Add(palleTrackingProcess);
                }
                GoodsReturnRequestSync goodsReturnRequestSync = new GoodsReturnRequestSync
                {
                    PalleTrackingProcess = palleTrackingProcessList,
                    ProductId = pid ?? 0,
                    OrderId = orderId ?? 0,
                    PalletTrackingId = palletTrackingId,
                    InventoryTransactionType = type,
                    tenantId = CurrentTenantId,
                    warehouseId = warehouseId,
                    userId = CurrentUserId,
                    deliveryNumber = deliveryNumber

                };
                _purchaseOrderService.ProcessPalletTrackingSerial(goodsReturnRequestSync, groupToken, true);
                return Json(new { orderid = orderId ?? 0, productId = pid ?? 0, orderNumber = "", groupToken = groupToken }, JsonRequestBehavior.AllowGet);
            }
            else if (serialList == null && type == InventoryTransactionTypeEnum.AdjustmentIn)
            {
                _inventoryTransactionsService.AddGoodsReturnPallet(serialList, "", pid ?? 0, type ?? 0, 0, orderId, CurrentTenantId, CurrentWarehouseId, CurrentUserId, palletTrackigId: palletTrackingId ?? 0);
            }

            else
            {

                if (serialList != null)
                {
                    string orderNumber = GenerateNextOrderNumber((InventoryTransactionTypeEnum)type);
                    if ((!orderId.HasValue && type == InventoryTransactionTypeEnum.Wastage) || type == InventoryTransactionTypeEnum.AdjustmentIn || type == InventoryTransactionTypeEnum.AdjustmentOut)
                    {
                        foreach (var pallet in serialList)
                        {
                            PalleTrackingProcess palleTrackingProcess = new PalleTrackingProcess();
                            decimal quantity = 0;
                            var palletdData = pallet.Split(new string[] { "#+#", "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                            if (palletdData.Length >= 2)
                            {
                                int PalletTrackingId = 0;

                                if (!string.IsNullOrEmpty(palletdData[0]))
                                {
                                    PalletTrackingId = int.Parse(palletdData[0]);
                                    palleTrackingProcess.PalletTrackingId = PalletTrackingId;
                                }
                                if (!string.IsNullOrEmpty(palletdData[1]))
                                {
                                    quantity = decimal.Parse(palletdData[1]);
                                    palleTrackingProcess.ProcessedQuantity = quantity;
                                }
                            }

                            palleTrackingProcessList.Add(palleTrackingProcess);
                        }
                        GoodsReturnRequestSync goodsReturnRequestSync = new GoodsReturnRequestSync
                        {
                            PalleTrackingProcess = palleTrackingProcessList,
                            ProductId = pid ?? 0,
                            OrderId = orderId ?? 0,
                            PalletTrackingId = palletTrackingId,
                            InventoryTransactionType = type,
                            tenantId = CurrentTenantId,
                            warehouseId = warehouseId,
                            OrderNumber = orderNumber,
                            userId = CurrentUserId,
                            deliveryNumber = deliveryNumber
                        };
                        int result = _purchaseOrderService.ProcessPalletTrackingSerial(goodsReturnRequestSync, groupToken, true);
                        if (result > 0)
                        {
                            return Json(new { orderid = orderId ?? 0, productId = pid ?? 0, orderNumber = orderNumber, groupToken = groupToken }, JsonRequestBehavior.AllowGet);

                        }
                    }

                    else
                    {

                        foreach (var pallet in serialList)
                        {
                            PalleTrackingProcess palleTrackingProcess = new PalleTrackingProcess();
                            decimal quantity = 0;
                            var palletdData = pallet.Split(new string[] { "#+#", "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                            if (palletdData.Length >= 2)
                            {
                                int PalletTrackingId = 0;

                                if (!string.IsNullOrEmpty(palletdData[0]))
                                {
                                    PalletTrackingId = int.Parse(palletdData[0]);
                                    palleTrackingProcess.PalletTrackingId = PalletTrackingId;
                                }
                                if (!string.IsNullOrEmpty(palletdData[1]))
                                {
                                    quantity = decimal.Parse(palletdData[1]);
                                    palleTrackingProcess.ProcessedQuantity = quantity;
                                }
                            }

                            palleTrackingProcessList.Add(palleTrackingProcess);
                        }

                        GoodsReturnRequestSync goodsReturnRequestSync = new GoodsReturnRequestSync
                        {
                            PalleTrackingProcess = palleTrackingProcessList,
                            ProductId = pid ?? 0,
                            OrderId = orderId ?? 0,
                            PalletTrackingId = palletTrackingId,
                            InventoryTransactionType = type,
                            tenantId = CurrentTenantId,
                            warehouseId = warehouseId,
                            OrderNumber = orderNumber,
                            userId = CurrentUserId,
                            deliveryNumber = deliveryNumber

                        };

                        _purchaseOrderService.ProcessPalletTrackingSerial(goodsReturnRequestSync, groupToken, true);
                    }

                    return Json(new { orderid = orderId ?? 0, productId = pid ?? 0, orderNumber = orderNumber, groupToken = groupToken }, JsonRequestBehavior.AllowGet);

                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);

        }


        public ActionResult _InventoryAdjustmentPartial()
        {
            if (!string.IsNullOrEmpty(Request.Params["inventoryId"]))
            {
                int inventoryId = int.Parse(Request.Params["inventoryId"]);
                var inventoryTransaction = _productServices.GetInventoryTransactionById(inventoryId);
                ViewBag.productId = inventoryTransaction.ProductMaster.ProductId;
                ViewBag.ProductDescription = inventoryTransaction.ProductMaster.Description;

                var transactionTypes = from InventoryTransactionTypeEnum d in Enum.GetValues(typeof(InventoryTransactionTypeEnum))
                                       where d == InventoryTransactionTypeEnum.AdjustmentIn || d == InventoryTransactionTypeEnum.AdjustmentOut
                                       select new { InventoryTransactionTypeId = (int)d, InventoryTransactionTypeName = d.ToString() };
                ViewBag.InventoryTransactionTypeId = new SelectList(transactionTypes, "InventoryTransactionTypeId", "InventoryTransactionTypeName");
                ViewBag.CurrentQuantity = inventoryTransaction.Quantity;
                inventoryTransaction.Quantity = 0;
                return View(inventoryTransaction);

            }
            return View();
        }



        #endregion
    }
}