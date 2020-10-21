using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class OrderProcessController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IGaneConfigurationsHelper _ganeConfigurationsHelper;

        public OrderProcessController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, IGaneConfigurationsHelper ganeConfigurationsHelper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _ganeConfigurationsHelper = ganeConfigurationsHelper;
        }

        // GET: /RecevePO/
        public ActionResult Index()
        {
            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();

        }

        public ActionResult ReceiveList()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();

        }
        public ActionResult Create(string delivery = "", int id = 0, int receivepo = 0)
        {

            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find PO of given id
            Order NewOrder = OrderService.GetOrderById(id);
            if (NewOrder == null)
            {
                return HttpNotFound();
            }

            //1 get all podetail of given po
            var podetail = NewOrder.OrderDetails.Where(x => x.WarehouseId == CurrentWarehouseId && x.IsDeleted != true);
            // 2 get all receive po detail
            //var receivepodetail = po.ReceivePOs.Select(x=>x.ReceivePODetails);
            var receivepodetail = NewOrder.OrderProcess.Where(x => x.WarehouseId == CurrentWarehouseId && x.IsDeleted != true).Select(x => x.OrderProcessID);
            //declare list of objects for view
            List<OrderProcessDetail> ReceivePOLine = new List<OrderProcessDetail>();

            foreach (var item in podetail)
            {
                //////////// // create ReceivePOLine object ///////////////////////////////
                OrderProcessDetail line = new OrderProcessDetail();
                //assigning values
                line.OrderProcessDetailID = item.OrderDetailID;
                line.ProductId = item.ProductId;
                line.QtyProcessed = OrderService.GetAllOrderProcessesByOrderDetailId(item.OrderDetailID, CurrentWarehouseId).Sum(x => (decimal?)x.QtyProcessed) ?? 0;
            } // end of loop

            ViewBag.rpodetail = ReceivePOLine;
            ViewBag.RPO = receivepo;
            ViewBag.delivery = delivery;

            return View(NewOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int POID, string DeliveryNO, int[] product, decimal[] qty, decimal[] qtyrec, int[] line, string Serialstemp)
        {

            if (qty.Any(m => m >= 0))
            {
                //TODO: inventory type should be passed to create order process
                OrderService.CreateOrderProcess(POID, DeliveryNO, product, qty, qtyrec, line, Serialstemp, CurrentUserId,
                    CurrentTenantId, CurrentWarehouseId);

                Inventory.StockRecalculateAll(CurrentWarehouseId, CurrentTenantId, CurrentUserId);
            }

            _ganeConfigurationsHelper.IsPOComplete(POID, CurrentUserId, CurrentWarehouseId);

            return RedirectToAction("Index");

        }

        public ActionResult Edit(int id = 0)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var rp = OrderService.GetOrderProcessByOrderProcessId(id);

            if (rp == null)
            {
                return HttpNotFound();
            }

            var recevivepodetail = rp.OrderProcessDetail.Where(x => x.IsDeleted != true);


            List<OrderProcessDetail> ReceivePOLine = new List<OrderProcessDetail>();

            foreach (var item in recevivepodetail)
            {
                OrderProcessDetail line = new OrderProcessDetail();
                line.OrderProcessDetailID = item.OrderProcessDetailID;
                line.ProductId = item.ProductId;
                line.QtyProcessed = item.QtyProcessed;

                if (line != null)
                    ReceivePOLine.Add(line);

            }
            ////////////PODetail lines that are not in ReceiveList. ////////////////////////////

            var detail = OrderService.GetAllValidOrderDetailsByOrderId(rp.OrderID??0);
            var rdetail = from r in rp.OrderProcessDetail where (r.IsDeleted != true) select r.OrderProcessDetailID;

            var podetail = from d in detail
                           where !(rdetail)
                           .Contains(d.OrderDetailID)
                           select d;

            List<OrderProcessDetail> PDLine = new List<OrderProcessDetail>();

            foreach (var item in podetail)
            {
                OrderProcessDetail line = new OrderProcessDetail();

                line.OrderProcessDetailID = item.OrderDetailID;
                line.ProductId = item.ProductId;
                line.QtyProcessed = OrderService.GetAllOrderProcessesByOrderDetailId(item.OrderDetailID, CurrentWarehouseId).Sum(x => (decimal?)x.QtyProcessed) ?? 0;
            }

            ViewBag.rpodetail = ReceivePOLine;
            ViewBag.pdetail = PDLine;

            return View(rp);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int OrderProcessId, int[] product, decimal[] qty, decimal[] qtyrec, int[] line)
        {
           var orderProcess = OrderService.SaveOrderProcess(OrderProcessId, product, qty, qtyrec, line, CurrentUserId, CurrentTenantId,
                CurrentWarehouseId);

            _ganeConfigurationsHelper.IsPOComplete(orderProcess.OrderID, CurrentUserId, CurrentWarehouseId);

            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult _RcvList()
        {
            var model = OrderService.GetOrderProcessByWarehouseId(CurrentWarehouseId);
            return PartialView("_RcvList", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult RDetail(int ReceivePOId)
        {
            ViewData["ReceivePOId"] = ReceivePOId;

            var model = OrderService.GetOrderProcessDetailByOrderProcessId(ReceivePOId)
                .Select(x => new
                {
                    x.OrderProcessDetailID,
                    GTIN = x.ProductMaster.SKUCode,
                    QtyReceived = x.QtyProcessed,

                });
            return PartialView("RDetail", model.ToList());
        }

        public ActionResult _ProcessByOrder(int? Id)
        {
            ViewBag.orderid = Id;
            var model = OrderService.GetOrderProcessByWarehouseId(CurrentWarehouseId).ToList();
            ViewBag.setName = "GridviewPDet"+Id.ToString();
            ViewBag.routeValues = new { Controller = "OrderProcess", Action = "_ProcessByOrder", Id= ViewBag.orderid };
            return PartialView("_ProcessList", model);
        }

    }
}