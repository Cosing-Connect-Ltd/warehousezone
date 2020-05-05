using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Services;
using System.Linq;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ReturnOrdersController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IVanSalesService _vanSalesService;
        private readonly IMapper _mapper;
        private readonly ILookupServices _lookupServices;

        public ReturnOrdersController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, 
            IVanSalesService vanSalesService, IMapper mapper) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _vanSalesService = vanSalesService;
            _mapper = mapper;
            _lookupServices = LookupServices;
        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

      
        public ActionResult _ReturnOrders()
        {
            var viewModel = GridViewExtension.GetViewModel("_ReturnOrderListGridView");

            if (viewModel == null)
                viewModel = OrdersCustomBinding.CreateSalesOrdersGridViewModel();

            return _ReturnOrdersGridActionCore(viewModel);


        }
        public ActionResult _ReturnOrdersOrdersPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("_ReturnOrderListGridView");
            viewModel.Pager.Assign(pager);
            return _ReturnOrdersGridActionCore(viewModel);
        }

        public ActionResult _ReturnOrdersFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("_ReturnOrderListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return _ReturnOrdersGridActionCore(viewModel);
        }

        public ActionResult _ReturnOrdersSorting(GridViewColumnState column, bool reset)
        {

            var viewModel = GridViewExtension.GetViewModel("_ReturnOrderListGridView");
            viewModel.ApplySortingState(column, reset);
            return _ReturnOrdersGridActionCore(viewModel);
        }
        public ActionResult _ReturnOrdersGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.ReturnOrderGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.ReturnOrderGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            ViewData["TransactionTypesList"] = _lookupServices.GetAllInventoryTransactionTypes().Select(x => x.InventoryTransactionTypeName).ToList();
            return PartialView("_ReturnOrders", gridViewModel);
        }


    }
}