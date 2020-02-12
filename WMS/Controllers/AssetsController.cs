using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class AssetsController : BaseController
    {
        private readonly IAssetServices _assetServices;
        public AssetsController(IAssetServices assetServices, ICoreOrderService orderService, IPurchaseOrderService purchaseOrderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, IProductLookupService productLookupService) : base(orderService, propertyService, accountServices, lookupServices)
        {

            _assetServices = assetServices;


        }

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult _assetListGridView()
        {
            var viewModel = GridViewExtension.GetViewModel("Assets");

            if (viewModel == null)
                viewModel = AssetListCustomBinding.CreateAssetListGridViewModel();
            return _AssetListGridActionCore(viewModel);
        }

        public ActionResult _AssetListPaging(GridViewPagerState pager)
        {

            var viewModel = GridViewExtension.GetViewModel("Assets");
            viewModel.Pager.Assign(pager);
            return _AssetListGridActionCore(viewModel);
        }


        public ActionResult _AssetListFiltering(GridViewFilteringState filteringState)
        {

            var viewModel = GridViewExtension.GetViewModel("Assets");

            viewModel.ApplyFilteringState(filteringState);
            return _AssetListGridActionCore(viewModel);
        }

        public ActionResult _AssetListSorting(GridViewColumnState column, bool reset)
        {

            var viewModel = GridViewExtension.GetViewModel("Assets");
            viewModel.ApplySortingState(column, reset);
            return _AssetListGridActionCore(viewModel);
        }


        public ActionResult _AssetListGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(new GridViewCustomBindingGetDataRowCountHandler(args => { AssetListCustomBinding.AssetListGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId); }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        AssetListCustomBinding.AssetListGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_assetListGridView", gridViewModel);
        }

        // GET: Assets/Details/5
        public ActionResult Details(int? id = null)
        {
            var assets = _assetServices.GetAssetById(id ?? 0);
            return View(assets);
        }

        [ValidateInput(false)]
        public ActionResult _AssetLog(int? id)
        {

            ViewData["id"] = id;
            Session["AssetId"] = id;
            var viewModel = GridViewExtension.GetViewModel("_AssetLog");

            if (viewModel == null)
                viewModel = AssetListCustomBinding.CreateAssetListGridViewModel();

            return _AssetLogListGridActionCore(viewModel, id);




        }
        public ActionResult _AssetLogListPaging(GridViewPagerState pager)
        {
            int? AssetId = null;
            if (Session["AssetId"] != null)
            {
                AssetId = Convert.ToInt32(Session["AssetId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("_AssetLog");
            viewModel.Pager.Assign(pager);
            return _AssetLogListGridActionCore(viewModel, AssetId);
        }


        public ActionResult _AssetLogListFiltering(GridViewFilteringState filteringState)
        {
            int? AssetId = null;
            if (Session["AssetId"] != null)
            {
                AssetId = Convert.ToInt32(Session["AssetId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("_AssetLog");

            viewModel.ApplyFilteringState(filteringState);
            return _AssetLogListGridActionCore(viewModel, AssetId);
        }

        public ActionResult _AssetLogListSorting(GridViewColumnState column, bool reset)
        {
            int? AssetId = null;
            if (Session["AssetId"] != null)
            {
                AssetId = Convert.ToInt32(Session["AssetId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("_AssetLog");
            viewModel.ApplySortingState(column, reset);
            return _AssetLogListGridActionCore(viewModel, AssetId);
        }


        public ActionResult _AssetLogListGridActionCore(GridViewModel gridViewModel, int? AssetId)
        {
            gridViewModel.ProcessCustomBinding(new GridViewCustomBindingGetDataRowCountHandler(args => { AssetListCustomBinding.AssetLogListGetDataRowCount(args, AssetId); }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        AssetListCustomBinding.AssetLogListGetData(args, AssetId);
                    })
            );
            return PartialView("_AssetLog", gridViewModel);
        }



        // GET: Assets/Create
        public ActionResult Create()
        {
            return View();
        }




        // POST: Assets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AssetId,AssetName,AssetDescription,AssetCode,AssetTag,IsActive")] Assets assets)
        {
            if (ModelState.IsValid)
            {
                assets.TenantId = CurrentTenantId;
                assets.CreatedBy = CurrentUserId;
                assets.DateCreated = DateTime.UtcNow;
                _assetServices.SaveAsset(assets);
                return RedirectToAction("Index");
            }

            return View(assets);
        }

        // GET: Assets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assets assets = _assetServices.GetAssetById(id ?? 0);
            if (assets == null)
            {
                return HttpNotFound();
            }
            return View(assets);
        }

        // POST: Assets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AssetId,AssetName,AssetDescription,AssetCode,AssetTag,IsActive")] Assets assets)
        {
            if (ModelState.IsValid)
            {
                assets.TenantId = CurrentTenantId;
                assets.UpdatedBy = CurrentUserId;
                assets.DateUpdated = DateTime.UtcNow;
                _assetServices.UpdateAsset(assets);
                return RedirectToAction("Index");
            }
            return View(assets);
        }

        // GET: Assets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var assets = _assetServices.GetAssetById(id ?? 0);
            //db.Assets.Find(id);
            if (assets == null)
            {
                return HttpNotFound();
            }
            return View(assets);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _assetServices.RemoveAsset(id);
            return RedirectToAction("Index");
        }

    }
}
