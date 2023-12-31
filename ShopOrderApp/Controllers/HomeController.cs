﻿using AutoMapper;
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
using PagedList;
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
        private readonly IProductLookupService _productLookupService;

        public HomeController(IProductLookupService productLookupService, IProductServices productServices, ISalesOrderService salesOrderService, ICoreOrderService orderService, IAccountServices accountServices, ILookupServices lookupServices,
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
            _productLookupService = productLookupService;
        }
        public ActionResult Index()
        {

            return View();
            //return RedirectToAction("Login", "User");
        }

        public ActionResult _DeliveriesResult(string searchText, PalletDispatchStatusEnum palletDispatchStatus, int? page, int pageSize = 10)
        {
            var orderprocess = _palletingService.GetAllPalletsDispatchs().Where(c => c.DispatchStatus == palletDispatchStatus
            && (string.IsNullOrEmpty(searchText) || c.OrderProcess.Order.OrderNumber.Contains(searchText)) && (palletDispatchStatus == PalletDispatchStatusEnum.Created || c.MarketVehicleID == VechileId));
            var check = orderprocess.Count();
            ViewBag.searchText = searchText;
            ViewBag.status = palletDispatchStatus;
            var orderComplete = orderprocess.OrderByDescending(U => U.DateCreated);
            ViewBag.pageSize = pageSize;
            var orders = orderComplete.ToPagedList((page ?? 1), pageSize);
            return PartialView(orders);
        }

        public ActionResult UnloadTruck()
        {
            return View();
        }
        public JsonResult AssigningDispatchToDelivery(int id)
        {
            AssigningDispatchToDelivery assigningDispatchToDelivery = new AssigningDispatchToDelivery();
            var pallets = _palletingService.GetAllPalletByDispatchId(id);
            assigningDispatchToDelivery.VechileId = pallets.FirstOrDefault().PalletsDispatch?.MarketVehicleID;
            assigningDispatchToDelivery.Palletdetails = pallets.Select(u => new Palletdetails
            {
                Id = u.PalletID,
                PalletNumber = u.PalletNumber,
                ScannedOnLoad = u.ScannedOnLoading,
            }).ToList();

            assigningDispatchToDelivery.TruckDetails = _productLookupService.GetAllTrucks(CurrentTenantId == 0 ? 1 : CurrentTenantId).Select(u => new TruckDetail
            {
                Id = u.Id,
                TruckName = u.Name
            }).ToList();
            return Json(assigningDispatchToDelivery, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult SubmitTruckLoad(string scannedIds, int? truckId, int dispatchId, PalletDispatchStatusEnum palletDispatchStatus)
        {
            truckId = truckId == null ? VechileId : 0;
            return Json(_palletingService.LoadPalletOnTruck(scannedIds, truckId.Value, dispatchId, palletDispatchStatus), JsonRequestBehavior.AllowGet);
        }

    }
}