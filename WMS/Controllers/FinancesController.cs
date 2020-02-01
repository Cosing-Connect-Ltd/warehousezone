using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class FinancesController : BaseController
    {
        private readonly InvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public FinancesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, InvoiceService invoiceService, IMapper mapper) 
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        public ActionResult Index(int? id=null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id.HasValue)
            {
                ViewBag.SelectedAccountID = id ?? 0;
            }
            return View();
        }

        public ActionResult _AccountTransactionsForm(int? id = null)
        {
            ViewData["AccountTransactionID"] = id;
            var model = AccountServices.GetAccountTransactionById(id ?? 0);
            ViewBag.PaymentModes = AccountServices.GetAllAccountPaymentModesSelectList();
            ViewBag.Accounts = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId), "AccountID", "AccountNameCode");
            return PartialView("_AccountTransactionCreateEdit", model);
        }

        public ActionResult _AccountTransactionsGrid(int? accountId)
        {
            var viewModel = GridViewExtension.GetViewModel("gridviewAccountTransactions"+accountId);
            ViewBag.accountId = accountId;
            if (viewModel == null)
                viewModel = FinancialTransactionsCustomBinding.CreateFinancialTransactionsGridViewModel();

            return AccountTransactionsGridActionCore(viewModel, accountId);

        }

        public ActionResult _AccountTransactionsListPaging(GridViewPagerState pager, int? accountId)
        {
            ViewBag.accountId = accountId;
            var viewModel = GridViewExtension.GetViewModel("gridviewAccountTransactions" + accountId);
            viewModel.Pager.Assign(pager);
            return AccountTransactionsGridActionCore(viewModel,accountId);
        }

        public ActionResult _AccountTransactionsListFiltering(GridViewFilteringState filteringState, int? accountId)

        {
            ViewBag.accountId = accountId;
            var viewModel = GridViewExtension.GetViewModel("gridviewAccountTransactions" + accountId);
            viewModel.ApplyFilteringState(filteringState);
            return AccountTransactionsGridActionCore(viewModel,accountId);
        }

        public ActionResult _AccountTransactionsDataSorting(GridViewColumnState column, bool reset, int? accountId)
        {
            ViewBag.accountId = accountId;
            var viewModel = GridViewExtension.GetViewModel("gridviewAccountTransactions" + accountId);
            viewModel.ApplySortingState(column, reset);
            return AccountTransactionsGridActionCore(viewModel,accountId);
        }
        public ActionResult AccountTransactionsGridActionCore(GridViewModel gridViewModel, int? accountId)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    FinancialTransactionsCustomBinding.FinancialTransactionsGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId, accountId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        FinancialTransactionsCustomBinding.FinancialTransactionsGetData(args, CurrentTenantId, CurrentWarehouseId, accountId);
                    })
            );
             
            return PartialView("_AccountTransactionsGridPartial", gridViewModel);
        }

        public ActionResult SaveAccountTransaction(AccountTransactionViewModel model)
        {
            _invoiceService.SaveAccountTransaction(_mapper.Map<AccountTransactionViewModel, AccountTransaction>(model), CurrentTenantId, CurrentUserId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChequeDetail()
        {
            var accounttransId = int.Parse(!string.IsNullOrEmpty(Request.Params["accounttransId"]) ? Request.Params["accounttransId"] : "0");
            //var Details = int.Parse(!string.IsNullOrEmpty(Request.Params["detail"]) ? Request.Params["detail"] : "0");
            if (accounttransId > 0)
            {
                
                var model = _invoiceService.GetaccountTransactionFiles(accounttransId, CurrentTenantId);
                return PartialView("_Chequedetail", model);
            }
            return View("Index");
        }
    }
}