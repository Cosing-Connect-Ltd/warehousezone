using DevExpress.Web.Mvc;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ProductSerialController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productLookupService;


        public ProductSerialController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, IProductLookupService productLookupService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _productLookupService = productLookupService;
        }
        // GET: ProductSerial
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _ProductSerialList()
        {

            var viewModel = GridViewExtension.GetViewModel("_ProductSerialListGridView");

            if (viewModel == null)
                viewModel = ProductSerialListCustomBinding.CreateProductSerialGridViewModel();

            return ProductSerialGridActionCore(viewModel);
        }


        public ActionResult _ProductSerialListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("_ProductSerialListGridView");
            viewModel.Pager.Assign(pager);
            return ProductSerialGridActionCore(viewModel);
        }

        public ActionResult _ProductSerialListFiltering(GridViewFilteringState filteringState)

        {
            var viewModel = GridViewExtension.GetViewModel("_ProductSerialListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return ProductSerialGridActionCore(viewModel);
        }

        public ActionResult _ProductSerialGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("_ProductSerialListGridView");
            viewModel.ApplySortingState(column, reset);
            return ProductSerialGridActionCore(viewModel);
        }

        public ActionResult ProductSerialGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ProductSerialListCustomBinding.GetPoductSerialDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ProductSerialListCustomBinding.GetProductSerialData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_ProdcutSerialList", gridViewModel);
        }

        public ActionResult _ProductSerialInventoryTransactions(int Id)
        {
            ViewBag.Id = Id;

            var data = (from opd in _productServices.GetInventoryTransactionsByProductSerialId(Id)
                        .OrderByDescending(x => x.DateCreated)
                        select new
                        {
                            opd.InventoryTransactionId,
                            opd.ProductMaster.Name,
                            opd.Quantity,
                            opd.DateCreated,
                            OrderNumber = opd.Order != null ? opd.Order.OrderNumber : "",
                            opd.InventoryTransactionType.InventoryTransactionTypeName,
                            opd.LastQty
                        }).ToList();

            return PartialView(data);

        }
    }
}