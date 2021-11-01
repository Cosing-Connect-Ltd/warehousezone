using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.Reports;

namespace ShopOrderApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ISalesOrderService _salesServices;
        private readonly IAccountServices _accountServices;
        private readonly IProductServices _productServices;
        private readonly IUserService _userService;
        private readonly IPalletingService _palletingService;
        private readonly IMapper _mapper;
        private readonly ICommonDbServices _commonDbServices;
        private readonly ITenantsServices _tenantServices;


        public HomeController(IProductServices productServices, ISalesOrderService salesOrderService, ICoreOrderService orderService, IAccountServices accountServices, ILookupServices lookupServices,
             IUserService userService, IPalletingService palletingService, IMapper mapper, ICommonDbServices commonDbServices, ITenantsServices tenantsServices)
            : base(orderService, accountServices, lookupServices)
        {
            _salesServices = orderService;
            _accountServices = accountServices;
            _productServices = productServices;
            _mapper = mapper;
            _userService = userService;
            _palletingService = palletingService;
            _commonDbServices = commonDbServices;
            _tenantServices = tenantsServices;
        }
        public ActionResult Index()
        {
            return RedirectToAction("Login", "User");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult NewDirectSalesOrder(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var model = _productServices.GetProductForDepartment();
            ViewBag.orderId = id;
            if (id.HasValue)
            {
                ViewBag.Quantities = OrderService.GetAllValidOrderDetailsByOrderId(id ?? 0).ToList();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult NewDirectSalesOrder(List<NewSalesProductMap> newSalesProducts, int? orderId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var model = _commonDbServices.CreateNewSaleOrders(newSalesProducts);
            model.OrderNumber = GenerateNextOrderNumber(InventoryTransactionTypeEnum.SalesOrder);
            if (!orderId.HasValue)
            {
                var order = OrderService.CreateNewDirectSalesOrder(model, CurrentTenantId, CurrentUserId, CurrentWarehouseId);
                return Json(order.OrderID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var order = OrderService.EditNewDirectSalesOrder(model, orderId ?? 0, CurrentTenantId, CurrentUserId, CurrentWarehouseId);
                return Json(order.OrderID, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult PlaceOrder(int id)
        {
            var model = OrderService.GetAllOrderById(id);
            return View(model);
        }

        #region SalesOrder

        public ActionResult SalesOrderPrint(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderService.UpdateOrders(id, CurrentUserId);
            var report = CreateSalesOrderPrint(id);

            return View(report);
        }
        public SalesOrderPrint CreateSalesOrderPrint(int id)
        {
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);

            var report = new SalesOrderPrint();
            report.paramOrderId.Value = id;
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }


            report.RequestParameters = false;
            report.FooterMsg1.Text = config.SoReportFooterMsg1;
            report.FooterMsg2.Text = config.SoReportFooterMsg2;
            return report;
        }

        #endregion SalesOrder
    }
}