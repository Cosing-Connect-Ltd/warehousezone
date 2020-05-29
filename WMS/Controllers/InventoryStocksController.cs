using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Models;
using DevExpress.Web.Mvc;
using WMS.CustomBindings;
using WarehouseEcommerce.Helpers;

namespace WMS.Controllers
{
    public class InventoryStocksController : BaseController
    {
        private readonly IProductServices _productService;
        private readonly IAdminServices _adminServices;
        private readonly IActivityServices _activityServices;
        private readonly ILookupServices _lookupServices;

        public InventoryStocksController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productService, IAdminServices adminServices, IActivityServices activityServices) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productService = productService;
            _adminServices = adminServices;
            _activityServices = activityServices;
            _lookupServices = lookupServices;
        }
        // GET: InventoryStocks
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var warehouses = _activityServices.GetAllPermittedWarehousesForUser(CurrentUserId, CurrentTenantId, CurrentUser.SuperUser == true, true);
            warehouses.Insert(0, new WarehousePermissionViewModel() { WId = 0, WName = "All Locations" });
            ViewBag.InventoryWarehouseId = new SelectList(warehouses, "WId", "WName", 0);
            return View();
        }



        //stock adjustments controller
        public ActionResult InventoryAdjustments(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var product = _productService.GetProductMasterById(id);

            if (product == null) return RedirectToAction("Index", "Products");

            ViewBag.ProductId = product.ProductId;
            ViewBag.ProductName = product.Name;
            var inventoryStock = product.InventoryStocks.FirstOrDefault(m => m.TenantId == CurrentTenantId &&
                                                            m.WarehouseId == caCurrent.CurrentWarehouse().WarehouseId);
            if (inventoryStock != null)
            {
                ViewBag.CurrentQuantity = inventoryStock.InStock;
            }
            ViewBag.ProductDescription = product.Description;

            var transactionTypes = from InventoryTransactionTypeEnum d in Enum.GetValues(typeof(InventoryTransactionTypeEnum))
                                   where d == InventoryTransactionTypeEnum.AdjustmentIn || d == InventoryTransactionTypeEnum.AdjustmentOut
                                   select new { InventoryTransactionTypeId = (int)d, InventoryTransactionTypeName = d.ToString() };

            ViewBag.InventoryTransactionTypeId = new SelectList(transactionTypes, "InventoryTransactionTypeId", "InventoryTransactionTypeName");

            ViewBag.Groups = new SelectList(from p in LookupServices.GetAllValidProductGroups(CurrentTenantId)
                                            where (p.TenentId == CurrentTenantId && p.IsDeleted != true)
                                            select new
                                            {
                                                p.ProductGroupId,
                                                p.ProductGroup
                                            }, "ProductGroupId", "ProductGroup");

            string referrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "/";

            // route to appropriate controller / action
            ViewBag.RController = "InventoryStocks";

            if (referrer.Contains("Products"))
            {
                ViewBag.RController = "Products";
            }

            return View();
        }

        public ActionResult InventoryAdjustmentsSerial(int id)
        {
            var product = _productService.GetProductMasterById(id);
            return PartialView("_InventoryAdjustmentsSerial", product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InventoryAdjustments(StockAdjustSerialsRequest model, string ReturnController = "")
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var productId = model.ProductId;

            var product = _productService.GetProductMasterById(productId);

            if (product == null)
            {
                ViewBag.Error = $"Product with id {productId} could not be found";
                return View();
            }

            ViewBag.productId = product.ProductId;
            ViewBag.ProductName = product.Name;
            ViewBag.ProductDescription = product.Description;

            var transactionTypes = from InventoryTransactionTypeEnum d in Enum.GetValues(typeof(InventoryTransactionTypeEnum))
                                   where d == InventoryTransactionTypeEnum.AdjustmentIn || d == InventoryTransactionTypeEnum.AdjustmentOut
                                   select new { InventoryTransactionTypeId = (int)d, InventoryTransactionTypeName = d.ToString() };

            ViewBag.InventoryTransactionTypeId = new SelectList(transactionTypes, "InventoryTransactionTypeId", "InventoryTransactionTypeName");

            ViewBag.Groups = new SelectList(from p in LookupServices.GetAllValidProductGroups(CurrentTenantId)
                                            where (p.TenentId == CurrentTenantId && p.IsDeleted != true)
                                            select new
                                            {
                                                p.ProductGroupId,
                                                p.ProductGroup
                                            }, "ProductGroupId", "ProductGroup");

            if (!product.Serialisable && (!model.Quantity.HasValue || model.InventoryTransactionTypeId < InventoryTransactionTypeEnum.PurchaseOrder))
            {
                ViewBag.Error = "Please specify the quantity and transaction type to complete the stock adjustment";
                return View();
            }

            if (product.Serialisable && (model.SerialItems == null))
            {
                ViewBag.Error = "Please specify product serials to continue";
                return View();
            }

            if (product.Serialisable)
            {
                var currentQuantity = model.SerialItems.Count;
                var productInventory = _productService.GetInventoryStockByProductTenantLocation(productId, CurrentWarehouseId);

                if (productInventory == null)
                {
                    Inventory.StockRecalculate(product.ProductId, CurrentWarehouseId, CurrentTenantId, CurrentUserId);
                }

                foreach (var serial in model.SerialItems)
                {
                    var productSerial = _productService.GetProductSerialBySerialCode(serial, CurrentTenantId);
                    if (productSerial == null)
                    {
                        productSerial = new ProductSerialis() { ProductId = productId, SerialNo = serial, CurrentStatus = InventoryTransactionTypeEnum.AdjustmentIn, BuyPrice = 0, WarrantyID = 1, DateCreated = DateTime.UtcNow, CreatedBy = CurrentUserId, UpdatedBy = CurrentUserId, TenentId = CurrentTenantId, WarehouseId = CurrentWarehouseId };
                    }
                    _productService.SaveProductSerial(productSerial, CurrentUserId);

                    var res = Inventory.StockTransaction(productId, model.InventoryTransactionTypeId, 1, null, null, model.InventoryTransactionRef, productSerial.SerialID);
                }
            }

            if (!product.Serialisable && ModelState.IsValid && model.Quantity.HasValue && model.Quantity > 0)
            {
                if (!Inventory.StockTransaction(model.ProductId, model.InventoryTransactionTypeId, model.Quantity ?? 0, null, null, model.InventoryTransactionRef, null))
                {
                    ViewBag.Error = "Sorry, Some error occured During Processing, Please Contact Support";
                    return View();
                }
            }

            if (ReturnController != "")
            {
                return RedirectToAction("Index", ReturnController);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }


        // GET: InventoryStocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var inventoryStock = _productService.GetInventoryStockByProductTenantLocation(id.Value, CurrentWarehouseId);
            if (inventoryStock == null)
            {
                return HttpNotFound();
            }
            return View(inventoryStock);
        }


        //[ValidateInput(false)]
        //public ActionResult _InventoryList()
        //{

        //    var model = _productService.GetAllInventoryStocksList(CurrentTenantId, id ?? 0);

        //    return PartialView("__InventoryList", model.ToList());
        //}
        [ValidateInput(false)]
        public ActionResult _InventoryList(int? id)
        {
            Session["InventoryId"] = id;
            var viewModel = GridViewExtension.GetViewModel("Inventory");

            if (viewModel == null)
                viewModel = InventoryStocksListCustomBinding.CreateInventoryStocksListGridViewModel();

            return _InventoryStocksGridActionCore(viewModel, id ?? 0);
        }

        public ActionResult _InventoryStocksPaging(GridViewPagerState pager)
        {
            int InventoryId = 0;
            if (Session["InventoryId"] != null)
            {
                InventoryId = Convert.ToInt32(Session["InventoryId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("Inventory");
            viewModel.Pager.Assign(pager);
            return _InventoryStocksGridActionCore(viewModel, InventoryId);
        }


        public ActionResult _InventoryStocksFiltering(GridViewFilteringState filteringState)
        {
            int InventoryId = 0;
            if (Session["InventoryId"] != null)
            {
                InventoryId = Convert.ToInt32(Session["InventoryId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("Inventory");

            viewModel.ApplyFilteringState(filteringState);
            return _InventoryStocksGridActionCore(viewModel, InventoryId);
        }

        public ActionResult _InventoryStocksSorting(GridViewColumnState column, bool reset)
        {
            int InventoryId = 0;
            if (Session["InventoryId"] != null)
            {
                InventoryId = Convert.ToInt32(Session["InventoryId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("Inventory");
            viewModel.ApplySortingState(column, reset);
            return _InventoryStocksGridActionCore(viewModel, InventoryId);
        }


        public ActionResult _InventoryStocksGridActionCore(GridViewModel gridViewModel, int id)
        {

            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    InventoryStocksListCustomBinding.InventoryStocksListGetDataRowCount(args, CurrentTenantId, id);
                }),
                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        InventoryStocksListCustomBinding.InventoryStocksListGetData(args, CurrentTenantId, id);
                    })
            );

            return PartialView("__InventoryList", gridViewModel);
        }


        [ValidateInput(false)]
        public ActionResult _InventoryLocationsList(int? productid, int? id = null)
        {
            if (productid < 1) return HttpNotFound();
            var model = _productService.GetAllInventoryStocksList(CurrentTenantId, id ?? 0, productid ?? 0).ToList();
            ViewBag.ProductId = productid;
            return PartialView("__InventoryLocationsList", model);
        }

        public ActionResult MoveStock(int? id)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            GaneStockMovementItemsSessionHelper.ClearSessionStockMovement();
            if (id == null)
            {
                var LocationsList = _adminServices.GetAllLocations(CurrentTenantId, CurrentWarehouseId);
                ViewBag.FromLocations = new SelectList(LocationsList, "LocationId", "LocationCode");
                ViewBag.ToLocations = new SelectList(LocationsList, "LocationId", "LocationCode");
                return View();
            }
            var prd = _productService.GetProductMasterById(id.Value);

            if (prd == null)
            {
                return HttpNotFound();
            }

            var Locations = _adminServices.GetAllLocations(CurrentTenantId, CurrentWarehouseId);

            ViewBag.FromLocations = new SelectList(Locations, "LocationId", "LocationCode");
            ViewBag.ToLocations = new SelectList(Locations, "LocationId", "LocationCode");
            ViewBag.Serials = new List<string>();
            var model = new StockMovementViewModel
            {
                ProductId = prd.ProductId
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult MoveStock(StockMovementViewModel data, bool BulkStock = false)
        {
            StockMovementCollectionViewModel model = new StockMovementCollectionViewModel();
            model.StockMovements = new List<StockMovementViewModel>();

            if (BulkStock)
            {
                var bulkdata = GaneStockMovementItemsSessionHelper.GetStockMovementsSession();
                model.StockMovements.AddRange(bulkdata);

            }
            else
            {
                data.UserId = CurrentUserId;
                data.TenentId = CurrentTenantId;
                data.WarehouseId = CurrentWarehouseId;
                model.StockMovements.Add(data);
            }

            var result = _lookupServices.UpdateStockMovement(model);
            if (result)
            {
                TempData["Success"] = "Product Moved Successfully";
            }
            else
            {
                TempData["Error"] = "Product not moved, qunatity not found against this product.";
            }
            if (BulkStock)
            {
                return RedirectToAction("MoveStock", new { id = string.Empty });
            }
            else
            {
                return RedirectToAction("MoveStock", new { id = data.ProductId });
            }
        }


        public ActionResult _StockMovementList()
        {
            var model = GaneStockMovementItemsSessionHelper.GetStockMovementsSession();
            return PartialView(model);

        }
        public JsonResult SaveMoveStock(StockMovementViewModel stockmovement)
        {
            stockmovement.WarehouseId = CurrentWarehouseId;
            stockmovement.TenentId = CurrentTenantId;
            stockmovement.UserId = CurrentUserId;
            stockmovement.FromLocationName = _lookupServices.GetLocationById((stockmovement.FromLocation ?? 0), CurrentTenantId)?.LocationName;
            stockmovement.ToLocationName = _lookupServices.GetLocationById((stockmovement.ToLocation ?? 0), CurrentTenantId)?.LocationName;
            stockmovement.ProductName = _productService.GetProductMasterById(stockmovement.ProductId)?.Name;
            GaneStockMovementItemsSessionHelper.UpdateStockMovementItemsSession(stockmovement);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetProductInformation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productId = int.Parse(id);
            var product = _productService.GetProductMasterById(productId);
            if (product == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (product.ProcessByPallet)
            {
                if (product.ProcessByPallet && caCurrent.CurrentWarehouse().EnableGlobalProcessByPallet) { product.ProcessByPallet = true; } else { product.ProcessByPallet = false; }


            }
            return Json(new { ProductName = product.Name, ProcessByPallet = product.ProcessByPallet, IsSerialised = product.Serialisable, ExistingSerials = product.ProductSerialization.Select(m => new { m.SerialID, m.SerialNo }) }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AdjustmentDetail()
        {
            var ProductId = int.Parse(!string.IsNullOrEmpty(Request.Params["prodId"]) ? Request.Params["prodId"] : "0");
            var Details = int.Parse(!string.IsNullOrEmpty(Request.Params["detail"]) ? Request.Params["detail"] : "0");
            if (ProductId > 0 && Details > 0)
            {
                var model = _productService.AllocatedProductDetail(ProductId, CurrentWarehouseId, Details);
                return PartialView("_InventoryAdjustmentDetailGridView", model);
            }
            return View("Index");
        }


        //public ActionResult StockCheck(int ProductId, decimal inStock,bool serialize)
        //{
        //    return Content(_lookupServices.CheckStockIssue(ProductId, inStock,serialize).ToString());
        //}


    }
}
