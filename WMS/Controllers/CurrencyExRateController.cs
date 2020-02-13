using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class CurrencyExRateController : BaseController
    {
        public readonly ITenantsCurrencyRateServices _tenantsCurrencyRateServices;

        public CurrencyExRateController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantsCurrencyRateServices = tenantsCurrencyRateServices;
        }
        // GET: CurrencyExRate
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GlobalCurrency currencies = _tenantsCurrencyRateServices.GetCurrencyNameById((int)id);
            if (currencies == null)
            {
                return HttpNotFound();
            }
            return View(currencies);
        }

        public ActionResult CurrencyExList()
        {

            var viewModel = DevExpress.Web.Mvc.GridViewExtension.GetViewModel("CurrencyExRate");

            if (viewModel == null)
                viewModel = CurrencyExRateCustomBinding.CreateCurrencyExRateListViewModel();

            return CurrencyExRateListGridActionCore(viewModel);
        }

        public ActionResult CurrencyExRateListGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(new GridViewCustomBindingGetDataRowCountHandler(args => { CurrencyExRateCustomBinding.CurrencyExRateGetDataRowCount(args, CurrentTenantId); }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        CurrencyExRateCustomBinding.CurrencyExRateGetData(args, CurrentTenantId);
                    })
            );

            return PartialView("CurrencyExList", gridViewModel);
        }

        public ActionResult LatestCurrencyExRateData()
        {

            var currencyExchangeRates = _tenantsCurrencyRateServices.GetTenantCurrenciesExRates(CurrentTenantId);
            var getLastUpdated = currencyExchangeRates.OrderByDescending(i => i.DateUpdated);
            return PartialView("_LatestCurrencyExRateData", getLastUpdated);
        }
        public ActionResult CurrencyExRateListPaging(GridViewPagerState pager)
        {

            var viewModel = GridViewExtension.GetViewModel("CurrencyExRate");
            if (viewModel == null)
            {
                viewModel = CurrencyExRateCustomBinding.CreateCurrencyExRateListViewModel();
            }
            viewModel.Pager.Assign(pager);
            return CurrencyExRateListGridActionCore(viewModel);
        }


        public ActionResult CurrencyExRateListFiltering(GridViewFilteringState filteringState)
        {

            var viewModel = GridViewExtension.GetViewModel("CurrencyExRate");
            if (viewModel == null)
            {
                viewModel = CurrencyExRateCustomBinding.CreateCurrencyExRateListViewModel();
            }
            viewModel.ApplyFilteringState(filteringState);
            return CurrencyExRateListGridActionCore(viewModel);
        }

        public ActionResult CurrencyExRateListSorting(GridViewColumnState column, bool reset)
        {

            var viewModel = GridViewExtension.GetViewModel("CurrencyExRate");
            if (viewModel == null)
            {
                viewModel = CurrencyExRateCustomBinding.CreateCurrencyExRateListViewModel();
            }
            viewModel.ApplySortingState(column, reset);
            return CurrencyExRateListGridActionCore(viewModel);
        }
    }
}