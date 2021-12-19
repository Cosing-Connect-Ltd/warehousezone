using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Entities.Enums;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class PalletTrackingController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productLookupService;


        public PalletTrackingController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, IProductLookupService productLookupService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _productLookupService = productLookupService;
        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }


        public ActionResult _PalletTrackingListDetail(int pId)
        {
            var pallets = (from p in _productServices.GetAllPalletTrackings(CurrentTenantId, CurrentWarehouseId)
                           where p.ProductId== pId && p.RemainingCases>0 && (p.Status==PalletTrackingStatusEnum.Active || p.Status==PalletTrackingStatusEnum.Hold)
                           select new
                {
                    p.PalletTrackingId,
                    p.ProductId,
                    p.OrderId,
                    p.ProductMaster.Name,
                    p.ProductMaster.SKUCode,
                    p.PalletSerial,
                    p.ExpiryDate,
                    p.RemainingCases,
                    p.TotalCases,
                    p.BatchNo,
                    p.Comments,
                    Status = p.Status.ToString(),
                    p.DateCreated,
                    p.DateUpdated,
                    p.ProductMaster.ProductGroup.ProductGroup,
                    p.ProductMaster.TenantDepartment.DepartmentName
                }).ToList();
            return PartialView(pallets);
        }


        public ActionResult _PalletTrackingList()
        {

            var viewModel = GridViewExtension.GetViewModel("_PalletTrackingListGridView");

            if (viewModel == null)
                viewModel = PalletTrackingListCustomBinding.CreateGetPalletTrackingGridViewModel();

            return PalletTrackingGridActionCore(viewModel);
        }


        public ActionResult _PalletTrackingListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("_PalletTrackingListGridView");
            viewModel.Pager.Assign(pager);
            return PalletTrackingGridActionCore(viewModel);
        }

        public ActionResult _PalletTrackingListFiltering(GridViewFilteringState filteringState)

        {
            var viewModel = GridViewExtension.GetViewModel("_PalletTrackingListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return PalletTrackingGridActionCore(viewModel);
        }

        public ActionResult _PalletTrackingGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("_PalletTrackingListGridView");
            viewModel.ApplySortingState(column, reset);
            return PalletTrackingGridActionCore(viewModel);
        }

        public ActionResult PalletTrackingGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    PalletTrackingListCustomBinding.GetPalletTrackingDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        PalletTrackingListCustomBinding.GetGetPalletTrackingData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_PalletTrackingList", gridViewModel);
        }


        public ActionResult _PalletTrackingInventoryTransactions(int Id)
        {
            ViewBag.Id = Id;

            var data = (from opd in _productServices.GetInventoryTransactionsByPalletTrackingId(Id)
                        .OrderByDescending(x => x.DateCreated)
                        select new
                        {
                            opd.InventoryTransactionId,
                            opd.ProductMaster.Name,
                            opd.Quantity,
                            opd.DateCreated,
                            OrderNumber = opd.Order != null ? opd.Order.OrderNumber : "",
                            opd.InventoryTransactionTypeId,
                            opd.LastQty
                        }).ToList();

            return PartialView(data);

        }


        public ActionResult Create()
        {

            return View();
        }
        [HttpPost]

        public JsonResult Create(int ProductId,string ExpiryDate,int TotalCase,string BatchNo,string Comments ,int NoOfPallets,string orderNumber)
        {
            if (ModelState.IsValid)
            {
                PalletTracking palletTracking = new PalletTracking();
                palletTracking.ProductId = ProductId;
                DateTime expiryDate = DateTime.Today;
                if (DateTime.TryParse(ExpiryDate, out expiryDate))
                {
                    palletTracking.ExpiryDate = expiryDate;
                }
                palletTracking.BatchNo = BatchNo;
                //palletTracking.OrderNumber = orderNumber;
                palletTracking.TotalCases = TotalCase;
                palletTracking.RemainingCases = TotalCase;
                palletTracking.Comments = Comments;
                palletTracking.Status = Ganedata.Core.Entities.Enums.PalletTrackingStatusEnum.Created;
                palletTracking.DateCreated = DateTime.UtcNow;
                palletTracking.DateUpdated = DateTime.UtcNow;
                palletTracking.TenantId = CurrentTenantId;
                palletTracking.WarehouseId = CurrentWarehouseId;
                string palletTrackingIds =_productServices.CreatePalletTracking(palletTracking,NoOfPallets);
                return Json(palletTrackingIds,JsonRequestBehavior.AllowGet);
            }

           
            return Json(false,JsonRequestBehavior.AllowGet);
        }



        public ActionResult SalesOrderSearch()
        {
            ViewBag.OrderAuth = true;
            return View();
        }
        public ActionResult OrderAuthzComboBoxPartial()
        {
            ViewBag.OrderAuth = true;
            return PartialView("OrderAuthComboBoxPartial");
        }

        public JsonResult SyncDate(int palletTrackingId)
        {
            bool status = _productServices.SyncDate(palletTrackingId);
            return Json(status, JsonRequestBehavior.AllowGet);

        }
        public JsonResult AddOrderId(int? OrderId, int palletTrackingId,int? type)
        {
            bool status = _productServices.AddOrderId(OrderId, palletTrackingId,type);
            return Json(status, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPalletbyPalletId(int palletTrackingId)
        {
            var status = _productServices.GetPalletbyPalletId(palletTrackingId)?.Status;
            return Json(status, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetProductCasePerPallet(int ProductId)
        {
            return Json(_productServices.GetProductMasterById(ProductId).CasesPerPallet.HasValue? _productServices.GetProductMasterById(ProductId).CasesPerPallet:0, JsonRequestBehavior.AllowGet);

        }

        public ActionResult PalletSummary()
        {
            return View();
        }

    }
}