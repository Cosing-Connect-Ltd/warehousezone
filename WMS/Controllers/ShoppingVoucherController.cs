using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Web.Mvc;
using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class ShoppingVoucherController : BaseController
    {
        private readonly IShoppingVoucherService _shoppingVoucherService;
        private readonly IUserService _userService;

        public ShoppingVoucherController(ICoreOrderService orderService,
            IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IShoppingVoucherService shoppingVoucherService, IUserService userService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _shoppingVoucherService = shoppingVoucherService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            var results = _shoppingVoucherService.GetAllValidShoppingVouchers(CurrentTenantId);

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            ViewBag.ResultsCount = results.Count;
            return View(results);
        }

        public ActionResult ShoppingListPartial()
        {
            var result = _shoppingVoucherService.GetAllValidShoppingVouchers(CurrentTenantId);
            return PartialView("_GridPartial", result);
        }
        public ActionResult Create()
        {
            var model = new ShoppingVoucher();
            ViewBag.DiscountTypeList = StaticHelperExtensions.GetSelectList<ShoppingVoucherDiscountTypeEnum>();
            ViewBag.UserList = _userService.GetAllAuthUsers(CurrentTenantId).Select(m=> new SelectListItem() { Value = m.UserId.ToString(), Text = m.DisplayName}).ToList();
            return View("_CreateEdit", model);
        }
        public ActionResult Edit(int? id)
        {
            var model = new ShoppingVoucher();
            ViewBag.DiscountTypeList = StaticHelperExtensions.GetSelectList<ShoppingVoucherDiscountTypeEnum>();
            ViewBag.UserList = _userService.GetAllAuthUsers(CurrentTenantId).Select(m => new SelectListItem() { Value = m.UserId.ToString(), Text = m.DisplayName }).ToList();
            if (id.HasValue && id > 0)
            {
                model = _shoppingVoucherService.GetShoppingVoucherById(id.Value);
            }
            return View("_CreateEdit", model);
        }

        [HttpPost]
        public ActionResult SaveVoucher(ShoppingVoucher model)
        {
            model.TenantId = CurrentTenantId;
            _shoppingVoucherService.SaveShoppingVoucher(model, CurrentUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult DeleteVoucher(int id)
        {
            _shoppingVoucherService.DeleteShoppingVoucher(id, CurrentUserId);
            return Json(new { success = true });
        }
    }
}