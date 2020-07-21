using AutoMapper;
using Ganedata.Core.Data.Migrations;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WarehouseEcommerce.Helpers;
using WarehouseEcommerce.ViewModels;

namespace WarehouseEcommerce.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsServices _tenantServices;
        private readonly IAccountServices _accountServices;
        private readonly IGaneConfigurationsHelper _configurationsHelper;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        string baseUrl = "";
        public UserController(ICoreOrderService orderService, IMapper mapper, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices, IUserService userService, IActivityServices activityServices, ITenantsServices tenantServices, IGaneConfigurationsHelper configurationsHelper, ITenantWebsiteService tenantWebsiteService)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices, tenantWebsiteService)
        {
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
            _accountServices = accountServices;
            _configurationsHelper = configurationsHelper;
            _tenantWebsiteService = tenantWebsiteService;

        }



        // GET: /User/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuthUser authuser = _userService.GetAuthUserById((int)id);
            if (authuser == null)
            {
                return HttpNotFound();
            }
            return View(authuser);
        }


        //[RequireHttps]
        public ActionResult Login(bool? PlaceOrder)
        {

            ViewBag.PlaceOrders = PlaceOrder;
            return View();
        }

        public ActionResult LoginUser(string UserName, string UserPassword, bool? PlaceOrder)
        {
            // string redirect to hold the redirect path
            string RedirectController = "user";
            string ReditectAction = "login";

            if (ModelState.IsValid)
            {

                caUser user = new caUser();
                bool status;
                status = user.AuthoriseWebsiteUser(UserName, UserPassword);

                if (status)
                {
                    Session["cawebsiteUser"] = user;


                    RedirectController = "Home";
                    ReditectAction = "Index";


                    // store login id into session
                    AuthUserLogin Logins = new AuthUserLogin();
                    Session["CurrentUserLogin"] = _userService.SaveAuthUserLogin(Logins, user.UserId, user.TenantId);
                    _tenantWebsiteService.UpdateUserIdInCartItem(Session.SessionID, user.UserId, CurrentTenantWebsite.SiteID);

                }
                else
                {
                    ViewBag.Error = "Wrong user information";
                    ReditectAction = "login";
                    return View("Login");
                }
            }
            if (PlaceOrder == true)
            {
                RedirectController = "Orders";
                ReditectAction = "Checkout";
                return RedirectToAction(ReditectAction, RedirectController, new { AccountId = CurrentUser.AccountId });
            }
            if (Session["LastUrlFrom"] != null)
            {
                var url = Session["LastUrlFrom"].ToString();
                Session["LastUrlFrom"] = null;
                if (!url.Contains("error"))
                {
                    return Redirect(url);
                }
            }

            return RedirectToAction(ReditectAction, RedirectController);

        }


        public ActionResult Create(bool? PlaceOrder)
        {
            ViewBag.Placeorder = PlaceOrder;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateUser(UsersViewModel user, bool? PlaceOrder)
        {
            baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            if (ModelState.IsValid)
            {
                AuthUser authuser = new AuthUser();
                Account account = new Account();
                account.AccountCode = user.FirstName + GaneStaticAppExtensions.GenerateRandomNo();
                account.CompanyName = account.AccountCode;
                account.RegNo = "";
                account.VATNo = "";
                account.AccountStatusID = AccountStatusEnum.Active;
                var accountModel = _accountServices.SaveAccount(account, null, null, 1, 1, 1, 1, null, null, CurrentUserId, CurrentTenantId, null);
                authuser.UserPassword = GaneStaticAppExtensions.GetMd5(user.Password);

                authuser.UserEmail = user.Email;
                authuser.UserFirstName = user.FirstName;
                authuser.UserLastName = user.LastName;
                authuser.UserPassword = GaneStaticAppExtensions.GetMd5(user.Password);
                authuser.UserName = user.Email;
                authuser.SiteId = CurrentTenantWebsite.SiteID;

                authuser.IsActive = false;
                authuser.AccountId = accountModel.AccountID;
                _userService.SaveAuthUser(authuser, CurrentUserId, CurrentTenantId);
                string confirmationLink = Url.Action("ConfirmUsers", "User", new { confirmationValue = GaneStaticAppExtensions.HashPassword(authuser.UserId.ToString()) + "_" + authuser.UserId.ToString(), placeOrder = GaneStaticAppExtensions.HashPassword(PlaceOrder.HasValue ? PlaceOrder.HasValue.ToString() : "") });
                confirmationLink = baseUrl + confirmationLink;
                confirmationLink = "<a href='" + confirmationLink + "' class=btn btn-primary>Activate Account</a>";
                _configurationsHelper.CreateTenantEmailNotificationQueue("Activate your account", null, sendImmediately: true, worksOrderNotificationType: WorksOrderNotificationTypeEnum.EmailConfirmation, TenantId: CurrentTenantId, accountId: accountModel.AccountID, UserEmail: authuser.UserEmail, confirmationLink: confirmationLink, userId: authuser.UserId, siteId: CurrentTenantWebsite.SiteID);
                return Json(true, JsonRequestBehavior.AllowGet);

            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmUser(string confirmationValue, string placeOrder)
        {
            bool? placeOrders = null;
            if (!string.IsNullOrEmpty(confirmationValue))
            {
                string[] data = confirmationValue.Split(new string[] { "_", "_" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (data.Length > 0)
                {
                    bool status = GaneStaticAppExtensions.ValidatePassword(data[data.Length - 1], data[0]);
                    var UserId = int.Parse(data[data.Length - 1]);
                    var authUsers = _userService.GetAuthUserById(UserId);
                    if (authUsers != null)
                    {
                        authUsers.IsActive = status;

                    }
                    _userService.UpdateAuthUser(authUsers, CurrentUserId, CurrentTenantId);


                }
                placeOrders = GaneStaticAppExtensions.ValidatePassword("True", placeOrder);
            }

            return RedirectToAction("Login", new { PlaceOrder = placeOrders });
        }

        public ActionResult ConfirmUsers(string confirmationValue, string placeOrder)
        {
            bool? placeOrders = null;
            if (!string.IsNullOrEmpty(confirmationValue))
            {
                string[] data = confirmationValue.Split(new string[] { "_", "_" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (data.Length > 0)
                {
                    bool status = GaneStaticAppExtensions.ValidatePassword(data[data.Length - 1], data[0]);
                    var UserId = int.Parse(data[data.Length - 1]);
                    var authUsers = _userService.GetAuthUserById(UserId);
                    authUsers.UserPassword = null;
                    if (authUsers != null)
                    {
                        authUsers.IsActive = status;

                    }
                    _userService.UpdateAuthUser(authUsers, CurrentUserId, CurrentTenantId);


                }
                placeOrders = GaneStaticAppExtensions.ValidatePassword("True", placeOrder);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult logout()
        {

            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            HttpContext.Session.Abandon();
            return RedirectToAction("Index", "Home");

        }
        public ActionResult Account()
        {
            var AccountDetailViewModel = new AccountDetailViewModel()
            {
                AuthUser = _userService.GetAuthUserById(CurrentUserId),
                OrderHistory = _orderService.GetOrdersHistory(CurrentUserId, CurrentTenantWebsite.SiteID).Take(10).ToList(),
                WebsiteWishList = _tenantWebsiteService.GetAllValidWishListItemsList(CurrentTenantWebsite.SiteID, CurrentUserId).ToList()


            };

            return View(AccountDetailViewModel);
        }
        public ActionResult _AccountSideBar()
        {
            return PartialView();
        }
        public ActionResult OrderHistory()
        {
            return View();
        }
        public ActionResult WishList()
        {
            return View();
        }
        public JsonResult LoginUsers(string UserName, string UserPassword, bool? PlaceOrder, string Popup)
        {
            if (ModelState.IsValid)
            {

                caUser user = new caUser();
                bool status;
                status = user.AuthoriseWebsiteUser(UserName, UserPassword);
                if (status)
                {
                    Session["cawebsiteUser"] = user;
                    AuthUserLogin Logins = new AuthUserLogin();
                    Session["CurrentUserLogin"] = _userService.SaveAuthUserLogin(Logins, user.UserId, user.TenantId);
                    _tenantWebsiteService.UpdateUserIdInCartItem(Session.SessionID, user.UserId, CurrentTenantWebsite.SiteID);
                    if (PlaceOrder.HasValue)
                    {
                        return Json(new { status, AccountId = user.AccountId }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status, Name = user.UserFirstName + " " + user.UserLastName }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateUser(int UserId, string FirstName, string LastName)
        {
            var User = _userService.GetAuthUserById(UserId);
            User.UserFirstName = FirstName;
            User.UserLastName = LastName;
            _userService.UpdateAuthUser(User, CurrentUserId, CurrentTenantId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsUserAvailableForSite(string Email)
        {
            if (!string.IsNullOrEmpty(Email)) Email = Email.Trim();

            int result = _userService.IsUserNameExistsForSite(Email, CurrentTenantWebsite.SiteID);

            if (result > 0)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}
