using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class DirectSalesController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IVanSalesService _vanSalesService;
        private readonly IMapper _mapper;

        public DirectSalesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices,
            IVanSalesService vanSalesService, IMapper mapper) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _vanSalesService = vanSalesService;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

        private DirectSalesViewModel LoadLookups(DirectSalesViewModel model)
        {
            var allTaxes = LookupServices.GetAllValidGlobalTaxes().ToList();
            var allWarranties = LookupServices.GetAllTenantWarrenties(CurrentTenantId).ToList();

            model.AllAccounts = AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Customer)
                .Select(m => new SelectListItem { Text = m.CompanyName, Value = m.AccountID.ToString() }).ToList();
            //model.AllAccounts.Insert(0, new SelectListItem { Text = "None", Value = "0" });

            model.AllProducts = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList()
                .Select(m => new SelectListItem { Text = m.NameWithCode, Value = m.ProductId.ToString() }).ToList();
            model.AllTaxes = allTaxes.Select(m => new SelectListItem { Text = m.TaxName, Value = m.TaxID.ToString() })
                .ToList();
            model.TaxDataHelper =
                Newtonsoft.Json.JsonConvert.SerializeObject(allTaxes.Select(m => new { m.TaxID, m.PercentageOfAmount }));
            model.AllWarranties = allWarranties
                .Select(m => new SelectListItem { Text = m.WarrantyName, Value = m.WarrantyID.ToString() }).ToList();
            model.WarrantyDataHelper =
                Newtonsoft.Json.JsonConvert.SerializeObject(
                    allWarranties.Select(m => new { m.WarrantyID, m.IsPercent, m.PercentageOfPrice, m.FixedPrice }));
            return model;
        }

        public ActionResult CreateDirectSales()
        {
            if (!caSession.AuthoriseSession())
            { return Redirect((string)Session["ErrorUrl"]); }
            var model = LoadLookups(new DirectSalesViewModel());
            ViewBag.discountPercentage = 100;
            return View(model);
        }
        public ActionResult EditDirectSales(int? id)
        {
            if (!caSession.AuthoriseSession())
            { return Redirect((string)Session["ErrorUrl"]); }
            caTenant tenant = caCurrent.CurrentTenant();
            string pageToken = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order Order = OrderService.GetOrderById(id.Value);
            if (Order == null)
            {
                return HttpNotFound();
            }
            if (Order.OrderStatusID != OrderStatusEnum.AwaitingAuthorisation)
            {
                TempData["ErrorAwaitingAuthorization"] = $"Order is not available to modify because order status not matched with required status.";
                return RedirectToAction("AwaitingAuthorisation", "Order");

            }

            ViewBag.AllAccounts = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Customer).ToList(), "AccountID", "CompanyName", Order.AccountID);

            if (string.IsNullOrEmpty(pageToken))
            {
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
            }
            var odList = OrderService.GetAllValidOrderDetailsByOrderId(id.Value).ToList();

            GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, _mapper.Map(odList, new List<OrderDetailSessionViewModel>()));
            return View(Order);

        }

        public ActionResult DirectSalesPreviewPartial(int? id = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new DirectSalesViewModel();
            if (id.HasValue)
            {
                model = OrderService.GetDirectSalesModelByOrderId(id.Value);
            }
            else
            {
                var paymentModes = from AccountPaymentModeEnum d in Enum.GetValues(typeof(AccountPaymentModeEnum))
                                   select new { ID = (int)d, Name = d.ToString() };
                model.AllPaymentModes = new SelectList(paymentModes, "Value", "Text", paymentModes.FirstOrDefault()).ToList();

            }

            return PartialView("_DirectSalesPreviewPartial", model);
        }

        public ActionResult SaveDirectSales(DirectSalesViewModel model)
        {
            try
            {

                OrderService.CreateDirectSalesOrder(model, CurrentTenantId, CurrentUserId, CurrentWarehouseId);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;

                return View("CreateDirectSales", LoadLookups(new DirectSalesViewModel()));
            }
            //var response = AutoMapper.Mapper.Map<Order, OrderViewModel>(order);
            return AnchoredOrderIndex("DirectSales", "Index", ViewBag.Fragment as string);
        }
        [HttpPost]
        public ActionResult EditDirectSales(Order order, string PageSession, DateTime? InvoiceDate)
        {
            var items = GaneOrderDetailsSessionHelper.GetOrderDetailSession(PageSession);

            var orders = OrderService.SaveDirectSalesOrder(order, CurrentTenantId, CurrentWarehouseId, CurrentUserId, _mapper.Map(items, new List<OrderDetail>()), null);
            TempData["SuccessDS"] = $"Order Updated Successfully";


            return RedirectToAction("AwaitingAuthorisation", "Order");

        }

        public ActionResult VanSalesCashReport()
        {
            ViewBag.MobileLocations = LookupServices.GetAllWarehousesForTenant(CurrentTenantId, null, true).Select(x => new SelectListItem() { Text = x.WarehouseName, Value = x.WarehouseId.ToString() }).ToList();
            return View("VanSalesCashReport");
        }

        public ActionResult _VanSalesCashReport()
        {
            var startDate = Request.Params["StartDate"].AsDateTime();
            DateTime? endDate = Request.Params["EndDate"].AsDateTime();
            if (endDate.HasValue)
            {
                endDate = endDate.Value.AddHours(24);
            }
            var warehouseId = Request.Params["MobileLocationID"].AsInt();
            var results = _vanSalesService.GetAllVanSalesDailyReports(CurrentTenantId, warehouseId, startDate, endDate);
            return PartialView("_VanSalesCashReport", results);
        }
    }
}