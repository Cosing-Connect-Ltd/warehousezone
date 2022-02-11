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
        public ActionResult Index(int pageSize = 10, int pageNo = 1)
        {
            var orderprocess = _palletingService.GetAllPalletsDispatchs().Where(c=>c.DispatchStatus==PalletDispatchStatusEnum.Created);
            var check = orderprocess.Count();
            var orderComplete=orderprocess.OrderByDescending(U=>U.DateCreated);
            ViewBag.pageSize=pageSize;
            ViewBag.pageNo=pageNo;
            var orders=orderComplete.ToPagedList(pageNo,pageSize);
            return View(orders);
            //return RedirectToAction("Login", "User");
        }
        public JsonResult AssigningDispatchToDelivery(int id)
        {
            AssigningDispatchToDelivery assigningDispatchToDelivery = new AssigningDispatchToDelivery();
            assigningDispatchToDelivery.Palletdetails = _palletingService.GetAllPalletByDispatchId(id).Select(u => new Palletdetails
            {
                Id = u.PalletID,
                PalletNumber = u.PalletNumber
            }).ToList();
            assigningDispatchToDelivery.TruckDetails = _productLookupService.GetAllTrucks(CurrentTenantId==0?1:CurrentTenantId).Select(u => new TruckDetail
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
        public JsonResult SubmitTruckLoad(string scannedIds, int truckId, int dispatchId)
        {

            return Json(_palletingService.LoadPalletOnTruck(scannedIds,truckId, dispatchId),JsonRequestBehavior.AllowGet);
        }

    }
}