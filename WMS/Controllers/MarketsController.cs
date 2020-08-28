using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class MarketsController : BaseController
    {
        private readonly IAccountServices _accountServices;
        private readonly IMarketServices _marketServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly IMapper _mapper;

        public MarketsController(ICoreOrderService orderService,
            IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IMarketServices marketServices, IEmployeeServices employeeServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _accountServices = accountServices;
            _marketServices = marketServices;
            _employeeServices = employeeServices;
            _mapper = mapper;
        }
        // GET: Appointments
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var results = _marketServices.GetAllValidMarkets(CurrentTenantId);
            return View(results);
        }

        public ActionResult MarketsListPartial()
        {
            var results = _marketServices.GetAllValidMarkets(CurrentTenantId);
            return PartialView("_GridPartial", results);
        }

        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new MarketViewModel();
            if (id.HasValue && id > 0)
            {
                model = _marketServices.GetMarketById(id.Value);
            }
            return View("_CreateEdit", model);
        }

        [HttpPost]
        public ActionResult SaveMarket(MarketViewModel model)
        {
            model.TenantId = CurrentTenantId;
            var market = _mapper.Map(model, new Market());
            _marketServices.SaveMarket(market, CurrentUserId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _marketServices.DeleteMarket(id, CurrentUserId);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.InnerException });
            }
        }

        #region Routes

        public ActionResult MarketCustomers(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new MarketCustomersViewModel() { MarketId = id };


            if (id > 0)
            {
                ViewBag.MarketName = _marketServices.GetMarketById(id).Name;
                model = _marketServices.GetMarketCustomersById(id, CurrentTenantId, null);
                model.MarketCustomerEntries = Newtonsoft.Json.JsonConvert.SerializeObject(model.SelectedCustomers);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveMarketCustomers(MarketCustomersViewModel model)
        {
            model.MarketCustomerAccounts = (List<SelectedAccountViewModel>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.MarketCustomerEntries, typeof(List<SelectedAccountViewModel>));

            _marketServices.SaveMarketCustomer(model, CurrentUserId, CurrentTenantId);

            return RedirectToAction("MarketCustomers", new { id = model.MarketId });
        }


        public ActionResult SearchAvailable(int id)
        {
            return PartialView("_SearchAvailable", id);
        }

        [HttpPost]
        public ActionResult SearchAvailable(int id, string query)
        {
            var model = new MarketCustomersViewModel() { MarketId = id };

            if (query != null)
            {
                model = _marketServices.GetMarketCustomersById(id, CurrentTenantId, query);
            }

            return PartialView("_SearchAvailableResult", model);
        }
        #endregion

        #region Market Stock Levels

        public ActionResult EditStockLevels(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var market = _marketServices.GetMarketById(id);
            ViewBag.MarketName = market.Name;
            ViewBag.MarketId = market.Id;
            return View();
        }

        public ActionResult _StockLevelsPartial(int id)
        {
            var model = _marketServices.GetAllStockLevelsForMarket(id, CurrentTenantId);
            ViewBag.MarketId = id;
            return PartialView("_StockLevelsPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult UpdateProductLevels(MVCxGridViewBatchUpdateValues<MarketProductLevelViewModel, int> updateValues)
        {
            int marketId = Convert.ToInt32(Request.Params["MarketId"]);
            foreach (var product in updateValues.Update)
            {
                if (updateValues.IsValid(product))
                    _marketServices.UpdateProductLevelsForMarkets(marketId, product.ProductID,
                       product.MinStockQuantity, CurrentUserId);
            }
            return _StockLevelsPartial(marketId);
        }

        #endregion

    }
}