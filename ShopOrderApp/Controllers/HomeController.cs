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

            return View();
            //return RedirectToAction("Login", "User");
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

    }
}