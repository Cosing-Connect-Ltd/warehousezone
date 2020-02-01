using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class OrderProcessDetailController : BaseController
    {
        public OrderProcessDetailController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices) : base(orderService, propertyService, accountServices, lookupServices)
        {

        }
        // GET: OrderProcessDetail
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _OPDByOrderProcess(int? Id)
        {
            if (Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.orderid = Id;
            ViewBag.setName = "GridviewOPD" + Id.ToString();
            ViewBag.routeValues = new { Controller = "OrderProcessDetail", Action = "_OPDByOrderProcess", Id = ViewBag.orderid };

            var model = OrderService.GetOrderProcessDetailByOrderProcessId(Id.Value).ToList();

            return PartialView("_OPDList", model);

        }
    }
}