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
        private readonly IProductServices _productServices;

        public ShoppingVoucherController(ICoreOrderService orderService,
            IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IShoppingVoucherService shoppingVoucherService, IUserService userService, IProductServices productServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _shoppingVoucherService = shoppingVoucherService;
            _userService = userService;
            _productServices = productServices;
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
        public ActionResult RewardTriggersListPartial()
        {
            var result = _shoppingVoucherService.GetAllValidRewardTriggers(CurrentTenantId);
            return PartialView("_TriggersGridPartial", result);
        }
        public ActionResult Create()
        {
            var model = new ShoppingVoucher();
            LoadVoucherDropdownData();
            return View("_CreateEdit", model);
        }     
        public ActionResult CreateRewardTrigger()
        {
            var model = new RewardPointTrigger();
            LoadVoucherDropdownData(true);
            return View("_TriggersCreateEdit", model);
        }

        public ActionResult Edit(int? id)
        {
            var model = new ShoppingVoucher();
            LoadVoucherDropdownData();
            if (id.HasValue && id > 0)
            {
                model = _shoppingVoucherService.GetShoppingVoucherById(id.Value);
            }
            return View("_CreateEdit", model);
        }
        public ActionResult EditRewardTrigger(int? id)
        {
            var model = new RewardPointTrigger();
            LoadVoucherDropdownData(true);
            if (id.HasValue && id > 0)
            {
                model = _shoppingVoucherService.GetRewardTriggerById(id.Value);
            }
            return View("_TriggersCreateEdit", model);
        }
        private void LoadVoucherDropdownData(bool rewardPointLookups = false)
        {
            ViewBag.DiscountTypeList = StaticHelperExtensions.GetSelectList<ShoppingVoucherDiscountTypeEnum>();
            var usersList = _userService.GetAllAuthUsers(CurrentTenantId).Select(m => new SelectListItem() { Value = m.UserId.ToString(), Text = m.DisplayNameWithEmail }).ToList();
            usersList.Insert(0, new SelectListItem() { Text = "Select an User" });
            ViewBag.UserList = usersList;

            var products = _productServices.GetAllValidProductMastersForSelectList(CurrentTenantId).ToList();
            products.Insert(0, new SelectListItem() { Text = "Select a Product" });
            ViewBag.Products = products;
            if (rewardPointLookups)
            {
                ViewBag.TriggerTypeList = StaticHelperExtensions.GetSelectList<VoucherDiscountTriggerEnum>();
                ViewBag.VouchersList = _shoppingVoucherService.GetAllValidShoppingVouchers(CurrentTenantId)
                    .Select(m => new SelectListItem()
                    {
                        Text = m.VoucherTitle +"("+ m.VoucherCode+")",
                        Value = m.ShoppingVoucherId.ToString()
                    }).ToList();
            }
        }

        [HttpPost]
        public ActionResult SaveVoucher(ShoppingVoucher model)
        {
            if (model.DiscountType == ShoppingVoucherDiscountTypeEnum.FreeProduct && (model.RewardProductId == null || model.RewardProductId<1) )
            {
                ViewBag.ErrorMessage = "Please select a reward Product";
                LoadVoucherDropdownData();
                return View("_CreateEdit", model);
            }

            model.TenantId = CurrentTenantId;
            _shoppingVoucherService.SaveShoppingVoucher(model, CurrentUserId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SaveRewardTrigger(RewardPointTrigger model)
        {
            if (model.TriggerType ==  VoucherDiscountTriggerEnum.OnReachingPoints && model.LoyaltyPointToTrigger == null)
            {
                ViewBag.ErrorMessage = "Please enter a valid reward point";
                LoadVoucherDropdownData(true);
                return View("_TriggersCreateEdit", model);
            }

            _shoppingVoucherService.SaveRewardTrigger(model, CurrentUserId);
            return Redirect((Url.Action("Index")+ "#RewardPointTriggers"));
        }

        [HttpPost]
        public JsonResult DeleteVoucher(int id)
        {
            _shoppingVoucherService.DeleteShoppingVoucher(id, CurrentUserId);
            return Json(new { success = true });
        }

        public ActionResult UpdateAllUsersWithPersonalReferralCode()
        {
            _userService.UpdateAllUsersWithPersonalReferralCode();
            return RedirectToAction("Index");
        }
    }
}