using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Services;
using System.Net;
using System.Web.Mvc;
using WarehouseEcommerce.ViewModels;

namespace WarehouseEcommerce.Areas.Shop.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsServices _tenantServices;
        private readonly IAccountServices _accountServices;
        private readonly IGaneConfigurationsHelper _configurationsHelper;
        string baseUrl = "";
        public UserController(ICoreOrderService orderService, IMapper mapper, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IActivityServices activityServices, ITenantsServices tenantServices, IGaneConfigurationsHelper configurationsHelper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
            _accountServices = accountServices;
            _configurationsHelper = configurationsHelper;
            
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
                    Session["cawebsiteUser"]  = user;


                    RedirectController = "Home";
                    ReditectAction = "Index";


                    // store login id into session
                    AuthUserLogin Logins = new AuthUserLogin();
                    Session["CurrentUserLogin"] = _userService.SaveAuthUserLogin(Logins, user.UserId, user.TenantId);

                    
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
                ReditectAction = "GetAddress";
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
            //if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Placeorder = PlaceOrder;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsersViewModel usersViewModel)
        {
            //if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            AuthUser authuser = new AuthUser();

            if (ModelState.IsValid)
            {
                Account account = new Account();
                account.AccountCode = string.IsNullOrEmpty(usersViewModel.CompanyName) ? usersViewModel.FirstName + GaneStaticAppExtensions.GenerateRandomNo() : usersViewModel.CompanyName;
                account.CompanyName = account.AccountCode;
                account.RegNo = usersViewModel.RegNumber;
                account.VATNo = usersViewModel.VATNumber;
                var accountModel = _accountServices.SaveAccount(account, null, null, 1, 1, 1, 1, 1, null, null, CurrentUserId, CurrentTenantId, null);

                // change user password into MD5 hash value
                string password = usersViewModel.Password;
                usersViewModel.Password = GaneStaticAppExtensions.GetMd5(usersViewModel.Password);
                authuser.UserEmail = usersViewModel.Email;
                authuser.UserFirstName = usersViewModel.FirstName;
                authuser.UserLastName = usersViewModel.LastName;
                authuser.UserPassword = usersViewModel.Password;
                authuser.UserName = usersViewModel.Email;
                authuser.IsActive = false;
                authuser.AccountId = accountModel.AccountID;
                _userService.SaveAuthUser(authuser, CurrentUserId, CurrentTenantId);
                string confirmationLink = Url.Action("ConfirmUser", "User", new { confirmationValue = GaneStaticAppExtensions.HashPassword(authuser.UserId.ToString()) + "_" + authuser.UserId.ToString(), placeOrder = GaneStaticAppExtensions.HashPassword(usersViewModel.PlaceOrder.HasValue? usersViewModel.PlaceOrder.HasValue.ToString():"")});
                confirmationLink = baseUrl + confirmationLink;
                confirmationLink = "<a href='" + confirmationLink + "' class=btn btn-primary>Activate Account</a>";
                _configurationsHelper.CreateTenantEmailNotificationQueue("Activate your account", null, sendImmediately: true, worksOrderNotificationType: WorksOrderNotificationTypeEnum.EmailConfirmation, TenantId: CurrentTenantId, accountId: accountModel.AccountID, UserEmail: authuser.UserEmail, confirmationLink: confirmationLink, userId: authuser.UserId);
                if (usersViewModel.PlaceOrder == true)
                {
                    return RedirectToAction("LoginUser", new { UserName = usersViewModel.Email, UserPassword = password, PlaceOrder = usersViewModel.PlaceOrder });
                }
                return RedirectToAction("Login");
            }

            return View(usersViewModel);
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
                    _userService.UpdateAuthUser(authUsers,CurrentUserId, CurrentTenantId);


                }
                placeOrders = GaneStaticAppExtensions.ValidatePassword("True", placeOrder);
            }

            return RedirectToAction("Login", new { PlaceOrder = placeOrders });
        }

        public ActionResult logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "User");

        }

    }
}
