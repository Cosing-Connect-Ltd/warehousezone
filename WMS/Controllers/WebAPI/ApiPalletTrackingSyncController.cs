using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    [EnableCorsAttribute("*", "*", "*")]
    public class ApiPallettrackingSyncController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IPalletingService _palletService;
        public ApiPallettrackingSyncController(ITerminalServices terminalServices, IPurchaseOrderService purchaseOrderService, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IMapper mapper, IPalletingService palletingService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _mapper = mapper;
            _palletService = palletingService;
            _purchaseOrderService = purchaseOrderService;
        }

        // call example through URI http://localhost:8005/api/sync/get-pallet-tracking?ReqDate=2014-11-23&SerialNo=920013c000814
        public IHttpActionResult GetPallettracking(DateTime reqDate, string serialNo)
        {
            bool includeArchived = true;
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            if (reqDate.Date == new DateTime(2000, 01, 01)) { includeArchived = false; }

            var result = new PalletTrackingSyncCollection();

            var allPallets = ProductServices.GetAllPalletTrackings(terminal.TenantId, terminal.WarehouseId, reqDate, includeArchived).ToList();
            var pallets = new List<PalletTrackingSync>();

            foreach (var p in allPallets)
            {
                var pallet = new PalletTrackingSync();
                var mappedPallet = _mapper.Map(p, pallet);
                pallets.Add(mappedPallet);
            }

            result.Count = pallets.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, pallets.Count(), terminal.TerminalId, TerminalLogTypeEnum.PalletTrackingSync).TerminalLogId;
            result.PalletTrackingSync = pallets;
            return Ok(result);
        }

        // POST http://localhost:8005/api/sync/post-pallet-tracking
        public IHttpActionResult PostPallettracking(PalletTrackingSyncCollection data)
        {
            data.SerialNo = data.SerialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(data.SerialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var TransactionLog = TerminalServices.CheckTransactionLog(data.TransactionLogId, terminal.TerminalId);

            if (TransactionLog == true)
            {
                return Conflict();
            }

            var mobileLocation = TerminalServices.GetMobileLocationByTerminalId(terminal.TerminalId);
            if (mobileLocation != null)
            {
                terminal.WarehouseId = mobileLocation.WarehouseId;
            }

            var results = new PalletTrackingSyncCollection();

            if (data.PalletTrackingSync != null && data.PalletTrackingSync.Any())
            {

                foreach (var item in data.PalletTrackingSync)
                {
                    item.DateUpdated = DateTime.UtcNow;
                    var pallet = ProductServices.SavePalletTrackings(item, terminal.TenantId);

                    results.PalletTrackingSync.Add(pallet);
                    results.Count += 1;
                }
            }

            TerminalServices.CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, data.PalletTrackingSync.Count, terminal.TerminalId, TerminalLogTypeEnum.PostPalletTracking);

            return Ok(results);
        }

        //GET http://localhost:8005/api/sync/verify-pallet-tracking
        public IHttpActionResult VerifyPallettracking(VerifyPalletTrackingSync data)
        {
            var serialNo = data.SerialNo.Trim().ToLower();
            var terminal = TerminalServices.GetTerminalBySerial(serialNo);
            if (terminal == null)
            {
                return Unauthorized();
            }
            var pallet = ProductServices.GetVerifedPalletAsync(data);
            var result = _mapper.Map<PalletTracking, PalletTrackingSync>(pallet);
            if (result == null)
            {
                result = new PalletTrackingSync();
                result.Comments = "Pallet Not Found";
            }
            TerminalServices.CreateTerminalLog(DateTime.UtcNow, terminal.TenantId, (pallet == null ? 0 : 1), terminal.TerminalId, TerminalLogTypeEnum.PalletTrackingSync);
            return Ok(result);
        }
        [HttpGet]
        public IHttpActionResult VerifyPallet(string serial, int productId, int shopId, int type)
        {
            VerifyPalletTrackingSync data = new VerifyPalletTrackingSync();
            data.PalletSerial = serial;
            data.InventoryTransactionType = type;
            data.ProductId = productId;
            data.WarehouseId = shopId;
            data.TenantId = shopId;
            PalletTracking verifedPallet = ProductServices.GetVerifedPalletAsync(data);
            //this._purchaseOrderService.GetVerifedPallet(serial, productId, 1, shopId);
            //
            PalletTrackingSync palletTrackingSync = new PalletTrackingSync();
            if (verifedPallet != null)
            {
                palletTrackingSync.PalletTrackingId = verifedPallet.PalletTrackingId;
                palletTrackingSync.ProductId = verifedPallet.ProductId;
                palletTrackingSync.RemainingCases = verifedPallet.RemainingCases;
                palletTrackingSync.TotalCases = verifedPallet.TotalCases;
            }
            return Ok(palletTrackingSync);
        }
        public IHttpActionResult SubmitPalleteSerials(SubmitPalletSerial submitPalletSerial)
        {
            GoodsReturnRequestSync serials = new GoodsReturnRequestSync();
            int shopId = submitPalletSerial.ShopId;
            submitPalletSerial.InventoryTransactionType = OrderService.GetOrderById(submitPalletSerial.OrderId).InventoryTransactionTypeId;
            List<PalleTrackingProcess> palleTrackingProcessList = new List<PalleTrackingProcess>();
            foreach (string str in submitPalletSerial.PalletSerial)
            {
                PalleTrackingProcess palleTrackingProcess = new PalleTrackingProcess();
                string[] strArray = str.Split(new string[] { "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length >= 2)
                {
                    if (!string.IsNullOrEmpty(strArray[0]))
                    {
                        int num = int.Parse(strArray[0]);
                        palleTrackingProcess.PalletTrackingId = num;
                    }
                    if (!string.IsNullOrEmpty(strArray[1]))
                    {
                        Decimal num = Decimal.Parse(strArray[1]);
                        palleTrackingProcess.ProcessedQuantity = num;
                    }
                }
                palleTrackingProcessList.Add(palleTrackingProcess);
            }
            serials.PalleTrackingProcess = palleTrackingProcessList;
            serials.ProductId = submitPalletSerial.ProductId;
            serials.OrderId = submitPalletSerial.OrderId;
            serials.deliveryNumber = GaneStaticAppExtensions.GenerateDateRandomNo();
            serials.OrderDetailID = submitPalletSerial.OrderDetailID;
            serials.tenantId = shopId;
            serials.warehouseId = shopId;
            serials.userId = submitPalletSerial.UserId;
            if (submitPalletSerial.InventoryTransactionType != InventoryTransactionTypeEnum.PurchaseOrder)
            {
                serials.InventoryTransactionType = submitPalletSerial.InventoryTransactionType;
            }
            var ordernumber = OrderService.GetOrderById(submitPalletSerial.OrderId).OrderNumber;
            this._purchaseOrderService.ProcessPalletTrackingSerial(serials);
            return Ok(ordernumber);
        }
        [HttpPost]
        public IHttpActionResult PostOrderProcessSimple(OrderProcessFull OrderProcessFull)
        {
            InventoryTransaction model = new InventoryTransaction();
            model.TenentId = 1;
            model.WarehouseId = OrderProcessFull.ShopId;
            model.CreatedBy = OrderProcessFull.UserId;
            model.OrderID = new int?(OrderProcessFull.OrderId);
            model.ProductId = OrderProcessFull.ProductId;
            model.Quantity = OrderProcessFull.Quantity;
            int num = 2;
            int? cons_type = new int?();
            string delivery = GaneStaticAppExtensions.GenerateDateRandomNo().ToString();
            Inventory.StockTransaction(model, (InventoryTransactionTypeEnum)num, cons_type, delivery, new int?(OrderProcessFull.OrderDetailId));
            return (IHttpActionResult)this.Ok<bool>(true);
        }

        public IHttpActionResult GetSalesOrders(string orderNumber)
        {
            var orders = OrderService.GetAllOrdersByTenantId(tenantId: 1).Where(u => (string.IsNullOrEmpty(orderNumber) || u.OrderNumber.Contains(orderNumber)) && u.OrderStatusID == OrderStatusEnum.Active && u.IsDeleted != true).OrderByDescending(u => u.OrderID).Take(10).Select(u => new
            {
                u.OrderID,
                u.OrderNumber,
                u.Account.CompanyName,
                u.Account.AccountCode


            }).ToList();
            return Ok(orders);
        }

        [HttpGet]
        public IHttpActionResult CanAutoComplete(int orderId, int userId)
        {
            OrderService.UpdateOrderStatus(orderId, OrderStatusEnum.Complete, userId);
            return Ok(true);
        }


        //[HttpPost]
        //public async IHttpActionResult SavePalletsDispatch(PalletDispatchViewModel model)
        //{
        //    ViewBag.ControllerName = "Pallets";
        //    if (Session["UploadedPalletEvidences"] != null)
        //    {
        //        var filelist = Session["UploadedPalletEvidences"] as List<string>;
        //        model.ProofOfDeliveryImageFilenames = string.Join(",", filelist);
        //    }
        //    else
        //    {
        //        model.ProofOfDeliveryImageFilenames = "";
        //    }
        //    var result = await _palletingService.DispatchPallets(model, CurrentUserId);
        //    if (!string.IsNullOrEmpty(result))
        //    {
        //        TempData["Error"] = result;
        //    }
        //    return RedirectToAction("Index", "Pallets");
        //}
    }
}