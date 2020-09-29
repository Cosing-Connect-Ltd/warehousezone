using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class PalletsController : BaseController
    {
        private readonly IPalletingService _palletingService;
        private readonly IMarketServices _marketServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly IGaneConfigurationsHelper _helper;
        private readonly ITenantsServices _tenantServices;
        private string _uploadDirectory = "~/UploadedFiles/Pallets/";
        string UploadProductManufacturerDirectory = "~/UploadedFiles/ProductManufacturers/";
        string UploadTenantDepartmentDirectory = "~/UploadedFiles/TenantDepartment/";
        string UploadProductGroupsDirectory = "~/UploadedFiles/ProductGroups/";
        public PalletsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantsServices tenantsServices, IPalletingService palletingService, IMarketServices marketServices, IEmployeeServices employeeServices, IGaneConfigurationsHelper helper) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _palletingService = palletingService;
            _marketServices = marketServices;
            _employeeServices = employeeServices;
            _helper = helper;
            _tenantServices = tenantsServices;
        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

        public ActionResult _Pallets(int? type, int? PalletsDispatchID)
        {
            ViewBag.Type = type;
            ViewBag.PalletsDispatchID = PalletsDispatchID;
            var viewModel = GridViewExtension.GetViewModel("PalletsListGridView" + ViewBag.Type + PalletsDispatchID);
            if (viewModel == null)
                viewModel = PalletsCustomBinding.CreatePalletGridViewModel();

            return _PalletsGridActionCore(viewModel, type, false, PalletsDispatchID);

        }
        public ActionResult _PalletsGridActionCore(GridViewModel gridViewModel, int? type, bool status = false, int? PalletsDispatchID = null)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    PalletsCustomBinding.GetPalletDataRowCount(args, type, status, PalletsDispatchID);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        PalletsCustomBinding.GetPalletData(args, type, status, PalletsDispatchID);
                    })
            );
            return PartialView("_Pallets", gridViewModel);
        }

        public ActionResult _PalletsGridViewsPaging(GridViewPagerState pager, int? type, bool status = false, int? PalletsDispatchID = null)
        {
            ViewBag.Type = type;
            ViewBag.PalletsDispatchID = PalletsDispatchID;
            var viewModel = GridViewExtension.GetViewModel("PalletsListGridView" + ViewBag.Type + PalletsDispatchID);
            viewModel.Pager.Assign(pager);
            return _PalletsGridActionCore(viewModel, type, status, PalletsDispatchID);
        }

        public ActionResult _PalletsGridViewFiltering(GridViewFilteringState filteringState, int? type, bool status = false, int? PalletsDispatchID = null)
        {
            ViewBag.Type = type;
            ViewBag.PalletsDispatchID = PalletsDispatchID;
            var viewModel = GridViewExtension.GetViewModel("PalletsListGridView" + ViewBag.Type + PalletsDispatchID);
            viewModel.ApplyFilteringState(filteringState);
            return _PalletsGridActionCore(viewModel, type, status, PalletsDispatchID);
        }
        public ActionResult _PalletsGridViewDataSorting(GridViewColumnState column, bool reset, int? type, bool status = false, int? PalletsDispatchID = null)
        {
            ViewBag.Type = type;
            ViewBag.PalletsDispatchID = PalletsDispatchID;
            var viewModel = GridViewExtension.GetViewModel("PalletsListGridView" + ViewBag.Type + PalletsDispatchID);
            viewModel.ApplySortingState(column, reset);
            return _PalletsGridActionCore(viewModel, type, status, PalletsDispatchID);
        }


        public ActionResult _PalletItemsList(int palletId)
        {

            //setname and routevalues are required to reuse order detail list.
            ViewBag.PalletId = palletId;
            ViewBag.setName = palletId;
            ViewBag.route = new { Controller = "Pallets", Action = "_PalletItemsList", palletId = palletId };
            var model = _palletingService.GetFulFillmentPalletProductsForPallet(palletId);
            return PartialView("_PalletItemsList", model);
        }


        public ActionResult _PalletsDispatch(int? type, int? ProcessId)
        {
            ViewBag.ProcessId = ProcessId;
            var dispatchPallets = _palletingService.GetAllPalletsDispatch(null, null, ProcessId);

            return PartialView("_PalletsDispatch", dispatchPallets);
        }



        public ActionResult _PalletDetails(int? PalletsDispatchID)
        {
            ViewBag.Type = PalletsDispatchID;
            ViewBag.detial = true;
            var viewModel = GridViewExtension.GetViewModel("PalletsListGridView" + ViewBag.Type);
            if (viewModel == null)
                viewModel = PalletsCustomBinding.CreatePalletGridViewModel();
            return _PalletsGridActionCore(viewModel, PalletsDispatchID, true);
        }

        private PalletDispatchInfoViewModel GetPalletModel(int palletId)
        {
            ViewBag.PalletId = palletId;
            ViewBag.setName = "gvPalletDispatchDetails" + palletId;
            ViewBag.route = new { Controller = "Pallets", Action = "_PalletDispatchInfo", palletId = palletId };

            var pallet = _palletingService.GetFulfillmentPalletById(palletId);
            var model = new PalletDispatchInfoViewModel() { PalletID = palletId };
            if (pallet.DateCompleted.HasValue)
            {
                model = _palletingService.GetPalletDispatchDetailByPallet(palletId);
                model.IsDispatched = true;
            }
            return model;
        }

        public ActionResult _PalletDispatchInfo(int palletId)
        {
            var model = GetPalletModel(palletId);
            if (model == null)
            { ViewBag.RowColor = true; }
            else { ViewBag.RowColor = false; }
            return PartialView("_PalletDispatchInfo", model);
        }
        public ActionResult _PalletDispatchInfoImages(int palletId)
        {
            var model = GetPalletModel(palletId);
            return PartialView("_PalletDispatchInfoImages", model);
        }

        // int? accountid = null
        public ActionResult PalletEditor(string id = null, int? OrderProcessId = null, int? DispatchId = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var palletId = 0;
            if (!string.IsNullOrEmpty(id) && !OrderProcessId.HasValue)
            {
                //var pallet = _palletingService.GetFulfillmentPalletByNumber(id);
                var pallet = _palletingService.GetFulfillmentPalletById(int.Parse(id));
                //accountid = pallet.RecipientAccountID;
                //OrderProcessId=pallet.RecipientAccountID;
                palletId = pallet.PalletID;
            }

            var model = new PalletGenerateViewModel
            {
                AllCurrentPallets = _palletingService.GetAllPallets(2, PalletStatusEnum.Active, orderProcessId: OrderProcessId).Select(m => new SelectListItem() { Text = m.PalletNumber, Value = m.PalletID.ToString() }).ToList(),
                SelectedOrderProcessId = OrderProcessId ?? 0,
                SelectedPalletID = palletId,

            };
            model.OrderProcesses = OrderService.GetOrderProcessByOrderProcessId(OrderProcessId ?? 0);
            model.dispatchId = DispatchId;
            return View("PalletEditor", model);
        }

        public ActionResult _GetNewPallet(PalletGenerateViewModel data)
        {

            var pallet = _palletingService.CreateNewPallet(data.SelectedOrderProcessId, CurrentUserId);

            var model = new PalletGenerateViewModel
            {
                AllCurrentPallets = _palletingService.GetAllPallets(5, orderProcessId: data.SelectedOrderProcessId).Select(m => new SelectListItem() { Text = m.PalletNumber, Value = m.PalletID.ToString() }).ToList(),
                NextPalletNumber = pallet.PalletNumber,
                SelectedPalletID = pallet.PalletID
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _GetPalletDetails(PalletGenerateViewModel data)
        {
            var pallet = _palletingService.GetFulfillmentPalletById(data.SelectedPalletID);

            var model = new PalletGenerateViewModel();
            if (pallet != null)
            {
                model.NextPalletNumber = pallet.PalletNumber;
                model.SelectedPalletID = pallet.PalletID;
                model.SelectedOrderProcessId = pallet.OrderProcessID ?? 0;
                model.IsCompleted = pallet.DateCompleted.HasValue;
                model.PalletDateCompleted = pallet.DateCompleted.HasValue ? pallet.DateCompleted.Value.ToString("dd/MM/yyyy HH:mm") : "";
            };

            model.SelectedOrderProcessId = data.SelectedOrderProcessId;

            return PartialView("_PalletDetails", model);
        }
        public ActionResult _PalletOrderDetails(int Id, int? orderProcessId)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            //ViewBag.OrderNumber = order.OrderNumber;
            ViewBag.setname = "gvSODetails" + orderProcessId;
            ViewBag.route = new { Controller = "Pallets", Action = "_PalletOrderDetails", Id = ViewBag.orderid };
            var order = OrderService.GetOrderById(Id);
            ViewBag.Groups = _tenantServices.GetAllTenantConfig(CurrentTenantId)?.FirstOrDefault(u => u.EnableTimberProperties)?.EnableTimberProperties;
            return PartialView("_PalletOrderDetails", OrderService.GetPalletOrdersDetails(Id, CurrentTenantId, true));
        }

        public ActionResult _AddOrderItemsToPallet()
        {
            var orderDetailId = int.Parse(Request.Params["orderDetailID"]);
            var orderProcessDetail = OrderService.GetOrderProcessDetailById(orderDetailId);
            var productId = int.Parse(Request.Params["productID"]);
            var productName = Request.Params["productName"];
            var palletNumber = Request.Params["currentPalletNumber"];
            var processedQuantity = orderProcessDetail.QtyProcessed - orderProcessDetail.PalletedQuantity;

            return PartialView("_PalletItemDetail", new PalletGenerateViewModel() { NextPalletNumber = palletNumber, OrderDetailID = orderDetailId, ProcessedQuantity = processedQuantity, ProductID = productId, ProductName = productName });
        }

        public ActionResult AddProcessedProductsToPallet(PalletProductAddViewModel model)
        {
            model.DateCreated = DateTime.UtcNow;
            model.CreatedBy = CurrentUserId;
            var item = _palletingService.AddFulFillmentPalletProduct(model);
            return Json(item.PalletProductID, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddAllProcessedProductsToPallet(int orderProcessId, int? palletId)
        {
            Pallet pallet = null;
            if (palletId == null || palletId == 0)
            {
                pallet = _palletingService.CreateNewPallet(orderProcessId, CurrentUserId);

                palletId = pallet.PalletID;
            }

            _palletingService.AddFulFillmentPalletAllOrderProducts(orderProcessId, palletId.Value, CurrentUserId);

            var allCurrentPallets = _palletingService.GetAllPallets(5, orderProcessId: orderProcessId);

            var model = new PalletGenerateViewModel
            {
                AllCurrentPallets = allCurrentPallets.Select(m => new SelectListItem() { Text = m.PalletNumber, Value = m.PalletID.ToString() }).ToList(),
                NextPalletNumber = allCurrentPallets.FirstOrDefault(a => a.PalletID == palletId)?.PalletNumber ?? pallet?.PalletNumber,
                SelectedPalletID = palletId ?? pallet.PalletID
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public string ExportPalletItemsList(PalletOrderProductsCollection model)
        {
            return "<html><body><form id='frmPrintPallets'>" + _helper.GetActionResultHtml(this, "_PalletItemsPrintList", model) + "</form></body></html>";
        }

        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["UploadedPalletEvidences"] == null)
            {
                Session["UploadedPalletEvidences"] = new List<string>();
            }
            var files = Session["UploadedPalletEvidences"] as List<string>;

            foreach (var file in UploadControl)
            {
                var fileToken = Guid.NewGuid().ToString();
                var ext = new FileInfo(file.FileName).Extension;
                var fileName = fileToken + ext;
                SaveFile(file, fileName);
                files.Add(file.FileName);
            }
            Session["UploadedPalletEvidences"] = files;

            return null;
        }
        private void SaveFile(DevExpress.Web.UploadedFile file, string fileName)
        {
            if (!Directory.Exists(Server.MapPath(_uploadDirectory)))
                Directory.CreateDirectory(Server.MapPath(_uploadDirectory));
            string resFileName = Server.MapPath(_uploadDirectory + @"/" + fileName);
            file.SaveAs(resFileName);
        }

        public ActionResult DispatchPallets(PalletDispatchViewModel model)
        {
            model.AllVehicles = _marketServices.GetAllValidMarketVehicles(CurrentTenantId).MarketVehicles
                .Select(m => new SelectListItem() { Text = m.Name, Value = m.Id.ToString() });
            model.AllDrivers = _employeeServices.GetAllEmployees(CurrentTenantId)
                .Select(m => new SelectListItem() { Text = m.Name, Value = m.ResourceId.ToString() });
            model.AllSentMethods = _palletingService.GetAllSentMethods()
                .Select(m => new SelectListItem() { Text = m.ToString(), Value = ((int)m).ToString() });
            model.DispatchRefrenceNumber = GaneStaticAppExtensions.GenerateDateRandomNo();
            model.AllDpdServices = _palletingService.GetAllDpdServices().
                Select(m => new SelectListItem() { Text = m.NetworkDescription.ToString(), Value = m.NetworkCode.ToString() });
            ViewBag.ControllerName = "Pallets";
            Session["UploadedPalletEvidences"] = null;
            return PartialView("_PalletDisptachDetail", model);
        }
        public ActionResult SavePalletsDispatch(PalletDispatchViewModel model)
        {
            ViewBag.ControllerName = "Pallets";
            if (Session["UploadedPalletEvidences"] != null)
            {
                var filelist = Session["UploadedPalletEvidences"] as List<string>;
                model.ProofOfDeliveryImageFilenames = string.Join(",", filelist);
            }
            else
            {
                model.ProofOfDeliveryImageFilenames = "";
            }
            var result = _palletingService.DispatchPallets(model, CurrentUserId);
            if (!string.IsNullOrEmpty(result))
            {
                TempData["Error"] = result;
            }
            return RedirectToAction("Index", "Pallets");
        }
        public ActionResult EditDispatchPallets(int PalletsDispatchID)
        {
            PalletDispatchViewModel model = new PalletDispatchViewModel();
            var PalletDispatch = _palletingService.GetPalletsDispatchByDispatchId(PalletsDispatchID);
            model.PalletDispatchId = PalletsDispatchID;
            model.MarketVehicleID = PalletDispatch.MarketVehicleID;
            model.DeliveryMethod = PalletDispatch.DeliveryMethod;
            model.TrackingReference = PalletDispatch.TrackingReference;
            model.CustomVehicleModel = PalletDispatch.CustomVehicleModel;
            model.MarketVehicleDriverID = PalletDispatch.VehicleDriverResourceID;
            model.CustomDriverDetails = PalletDispatch.CustomDriverDetails;
            model.DispatchNotes = PalletDispatch.DispatchNotes;
            model.NetworkCode = PalletDispatch.NetworkCode;
            model.ProofOfDeliveryImageFilenames = PalletDispatch.ProofOfDeliveryImageFilenames;
            model.AllVehicles = _marketServices.GetAllValidMarketVehicles(CurrentTenantId).MarketVehicles
                .Select(m => new SelectListItem() { Text = m.Name, Value = m.Id.ToString() });
            model.AllDrivers = _employeeServices.GetAllEmployees(CurrentTenantId)
                .Select(m => new SelectListItem() { Text = m.Name, Value = m.ResourceId.ToString() });
            model.AllSentMethods = _palletingService.GetAllSentMethods()
                .Select(m => new SelectListItem() { Text = m.ToString(), Value = ((int)m).ToString() });
            model.DispatchRefrenceNumber = PalletDispatch.DispatchReference;
            model.AllDpdServices = _palletingService.GetAllDpdServices().
                Select(m => new SelectListItem() { Text = m.NetworkDescription.ToString(), Value = m.NetworkCode.ToString() });
            Session["UploadedPalletEvidences"] = null;
            ViewBag.ControllerName = "Pallets";
            if (!string.IsNullOrEmpty(PalletDispatch.ProofOfDeliveryImageFilenames))
            {

                var filePaths = PalletDispatch.ProofOfDeliveryImageFilenames.Split(',').ToList();
                if (filePaths.Count() > 0)
                {
                    List<string> files = new List<string>();
                    Session["UploadedPalletEvidences"] = files;
                    ViewBag.Files = files;
                    foreach (var file in filePaths)
                    {
                        DirectoryInfo dInfo = new DirectoryInfo(file);
                        files.Add(dInfo.Name);
                    }
                }
            }





            return PartialView("_PalletDisptachDetail", model);
        }
        public JsonResult DeletePallet(int palletId)
        {
            var status = _palletingService.DeletePallet(palletId);

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _PalletItemsPrintList(int id)
        {
            var palletItems = OrderService.GetAllProductsByOrdersInPallet(id);
            if (palletItems != null)
            {
                ViewBag.CallBackAction = "PalletItemsPrintList";
                ViewBag.ExportCallBackAction = "PalletItemsExportTo";
                return PartialView("_PalletItemsPrintList", palletItems);
            }
            return null;
        }

        public ActionResult Print(int id)
        {
            var palletItems = OrderService.GetAllProductsByOrdersInPallet(id);
            if (palletItems != null)
            {
                //ViewBag.CallBackAction = "_PalletItemsPrintList";
                //ViewBag.ExportCallBackAction = "PalletItemsExportTo";
                //var htmlString = ExportPalletItemsList(palletItems);
                //var featuresModel = FeaturesOptions.CreateDefault();
                //featuresModel.Html = htmlString;
                //ViewBag.PalletId = id;
                return View("PalletPrint", palletItems);
            }
            return null;
        }

        public ActionResult PalletItemsExportTo(HtmlEditorExportFormat format, string callbackAction, string exportCallbackAction)
        {
            return HtmlEditorExtension.Export(new HtmlEditorSettings(), format);
        }

        public JsonResult DeletePalletProduct(int PalletProductId)
        {

            int result = _palletingService.DeletePalletProduct(PalletProductId, CurrentUserId);
            if (result > 0)
            {
                return Json(new { palletId = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult PalletDispatchCheck(int OrderProcessId)
        {
            var orderDetail = OrderService.GetPalletOrdersDetails(OrderProcessId, CurrentTenantId, true);
            bool status = false;
            if (orderDetail.Count > 0)
            {
                foreach (var item in orderDetail)
                {
                    if (item.QtyProcessed < item.Qty || (item.orderProcessstatusId.HasValue && (OrderProcessStatusEnum)item.orderProcessstatusId == OrderProcessStatusEnum.Active) || (item.orderstatusId.HasValue && ((OrderStatusEnum)item.orderstatusId == OrderStatusEnum.Active) || (OrderStatusEnum)item.orderstatusId == OrderStatusEnum.BeingPicked || (OrderStatusEnum)item.orderstatusId == OrderStatusEnum.Hold)

                        )
                    {
                        status = false;
                        return Json(status, JsonRequestBehavior.AllowGet);
                    }
                    status = true;
                }

            }
            return Json(status, JsonRequestBehavior.AllowGet);

        }

        public JsonResult MarkedOrderProcessAsDispatch(int OrderProcessId)
        {

            bool status = _palletingService.MarkedOrderProcessAsDispatch(OrderProcessId);
            return Json(status, JsonRequestBehavior.AllowGet);

        }

        public JsonResult _RemoveProofOfDeliveryFile(string filename, bool tenantDepartment = false, bool TenantGroup = false, bool Maufacturer=false,int?Id=null)
        {
            if (tenantDepartment)
            {
                var files = Session["UploadTenantDepartmentImage"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == filename);
                files.Remove(filetoremove);
                if (Request.UrlReferrer.AbsolutePath.Contains("Edit"))
                {
                    string FilePath = Server.MapPath(UploadTenantDepartmentDirectory.Replace("~", "") + Id.ToString() + "/" + filename);
                    if (System.IO.File.Exists(FilePath))
                    {
                        System.IO.File.Delete(FilePath);
                    }
                    //System.IO.File.Delete(Server.MapPath(UploadDirectory + Session["pId"].ToString() + "/" + filename));
                }
                else
                {
                    System.IO.File.Delete(Server.MapPath(UploadTenantDepartmentDirectory + "TempFiles" + "/" + filename));
                }
                if (files.Count <= 0)
                {
                    Session["UploadTenantDepartmentImage"] = null;
                }
                var cfiles = files.Select(a => a).ToList();
                return Json(new { files = cfiles.Count == 0 ? null : cfiles });
            }
            else if (TenantGroup)
            {
                var files = Session["uploadProductGroup"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == filename);
                files.Remove(filetoremove);
                if (Request.UrlReferrer.AbsolutePath.Contains("Edit"))
                {
                    string FilePath = Server.MapPath(UploadProductGroupsDirectory.Replace("~", "") + Id.ToString() + "/" + filename);
                    if (System.IO.File.Exists(FilePath))
                    {
                        System.IO.File.Delete(FilePath);
                    }
                    //System.IO.File.Delete(Server.MapPath(UploadDirectory + Session["pId"].ToString() + "/" + filename));
                }
                else
                {
                    System.IO.File.Delete(Server.MapPath(UploadProductGroupsDirectory + "TempFiles" + "/" + filename));
                }
                if (files.Count <= 0)
                {
                    Session["uploadProductGroup"] = null;
                }
                var cfiles = files.Select(a => a).ToList();
                return Json(new { files = cfiles.Count == 0 ? null : cfiles });
            }
            else if (Maufacturer)
            {
                var files = Session["UploadProductManufacturerImage"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == filename);
                files.Remove(filetoremove);
                if (Request.UrlReferrer.AbsolutePath.Contains("Edit"))
                {
                    string FilePath = Server.MapPath(UploadProductManufacturerDirectory.Replace("~", "") + Id.ToString() + "/" + filename);
                    if (System.IO.File.Exists(FilePath))
                    {
                        System.IO.File.Delete(FilePath);
                    }
                    //System.IO.File.Delete(Server.MapPath(UploadDirectory + Session["pId"].ToString() + "/" + filename));
                }
                else
                {
                    System.IO.File.Delete(Server.MapPath(UploadProductManufacturerDirectory + "TempFiles" + "/" + filename));
                }
                if (files.Count <= 0)
                {
                    Session["UploadProductManufacturerImage"] = null;
                }
                var cfiles = files.Select(a => a).ToList();
                return Json(new { files = cfiles.Count == 0 ? null : cfiles });

            }
            else
            {
                var files = Session["UploadedPalletEvidences"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == filename);
                files.Remove(filetoremove);
                if (files.Count <= 0)
                {
                    Session["UploadedPalletEvidences"] = null;
                }
                var cfiles = files.Select(a => a).ToList();
                return Json(new { files = cfiles.Count == 0 ? null : cfiles });
            }
        }
    }
}