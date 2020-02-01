using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;


namespace WMS.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;
        private readonly ITenantsServices _tenantServices;

        public UserController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IActivityServices activityServices, ITenantsServices tenantServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _userService = userService;
            _activityServices = activityServices;
            _tenantServices = tenantServices;
        }
        // GET: /User/
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
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

        // GET: /User/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,UserPassword,UserFirstName,UserLastName,UserEmail,IsActive")] AuthUser authuser)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {

                // change user password into MD5 hash value
                authuser.UserPassword = GaneStaticAppExtensions.GetMd5(authuser.UserPassword);

                _userService.SaveAuthUser(authuser, CurrentUserId, CurrentTenantId);

                return RedirectToAction("Index");
            }

            return View(authuser);
        }

        // GET: /User/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: /User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,UserName,UserPassword,UserFirstName,UserLastName,UserEmail,IsActive")] AuthUser authuser)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();

                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _userService.UpdateAuthUser(authuser, user.UserId, tenant.TenantId);

                return RedirectToAction("Index");
            }

            return View(authuser);
        }

        // GET: /User/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            AuthUser authuser = _userService.GetAuthUserById(id);
            // get properties of user
            caUser user = caCurrent.CurrentUser();

            _userService.DeleteAuthUser(authuser, user.UserId);
            return RedirectToAction("Index");
        }


        //[RequireHttps]
        public ActionResult Login()
        {
            if (caSession.AuthoriseSession())
            {
                return Redirect("~/home");
            }
            else if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }
            else if (Session["caTenant"] == null)
            {
                return Redirect((string)Session["ErrorUrl"]);
            }

            return View();
        }


        //[RequireHttps]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string UserPassword)
        {
            // string redirect to hold the redirect path
            string RedirectController = "user";
            string ReditectAction = "login";

            if (Session["caTenant"] == null)
            {
                TempData["Error"] = "Security token was expired, Enter credentials again";
            }

            else if (ModelState.IsValid)
            {

                caUser user = new caUser();
                bool status;
                status = user.AuthoriseUser(UserName, UserPassword);
                if (status)
                {
                    Session["caUser"] = user;
                    RedirectController = "home";
                    ReditectAction = "Index";

                    // store login id into session
                    AuthUserLogin Logins = new AuthUserLogin();
                    Session["CurrentUserLoginId"] = _userService.SaveAuthUserLogin(Logins, user.UserId, user.TenantId);

                    if (!caSession.AuthoriseSession())
                    {
                        return Redirect((string)Session["ErrorUrl"]);
                    }
                }
                else
                {
                    ViewBag.Error = "Wrong user information";
                    ReditectAction = "login";
                    return View();
                }
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

        public ActionResult logout()
        {
            Session.Clear();
            Session.Abandon();
            return View();

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehChange(FormCollection form)
        {
            int id = Convert.ToInt32(form["wareh-select"]);

            var requestUrl = Request.UrlReferrer.ToString();

            //set session of current warehouse
            Session["CurrentWarehouseId"] = id;

            // return back to the origional refferal page
            return Redirect(requestUrl);

        }


        //change user permissions  quick
        public ActionResult UserPermissions(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            string Checked = "";
            StringBuilder stringBuilder = new StringBuilder();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            caTenant tenant = caCurrent.CurrentTenant();
            caUser user = caCurrent.CurrentUser();

            var userModules = _tenantServices.GetAllTenantModules(user.TenantId).Select(m => m.ModuleId).ToList();

            ICollection<TenantLocations> WareHouse = tenant.TenantLocations.Where(x => x.IsMobile != true).ToList();
            stringBuilder.Append(string.Format("<input type=\"hidden\" name=\"UserId\" value=\"{0}\" />", id));

            foreach (TenantLocations vh in WareHouse)
            {
                stringBuilder.Append(string.Format("<div id=\"page-wrap{0}\" class=\"page-wrap form-horizontal\">", vh.WarehouseId));
                stringBuilder.Append("<ul>");
                stringBuilder.Append("<li>");
                stringBuilder.Append(string.Format("<input type=\"checkbox\" name=\"wareh\" id=\"wareh-{0}\" value=\"{0}\">", vh.WarehouseId, vh.WarehouseName));
                stringBuilder.Append(string.Format("<label for=\"wareh-{0}\">{1}</label>", vh.WarehouseId, vh.WarehouseName));

                stringBuilder.Append("<ul class=\"wareh-ul\">");

                var Groups = _activityServices.GetDistinctActivityGroupsForPerm(userModules);

                foreach (var Grp in Groups)
                {
                    stringBuilder.Append("<li class=\"group-li\">");
                    stringBuilder.Append(string.Format("<input type=\"checkbox\" name=\"group-{0}-{1}\" id=\"group-{0}-{1}\" value=\"{1}\">", vh.WarehouseId, Grp.ActivityGroupId));
                    stringBuilder.Append(string.Format("<label for=\"group-{0}-{1}\">{2}</label>", vh.WarehouseId, Grp.ActivityGroupId, Grp.ActivityGroupName));
                    stringBuilder.Append("<ul class=\"group-ul\">");

                    var nav = _activityServices.GetAuthActivitiesForPermByGroup(Grp, userModules,CurrentTenantId);

                    foreach (var perm in nav)
                    {
                        if (_activityServices.PermCheckByActivityId(perm.ActivityId, (int)id, vh.WarehouseId))
                        {
                            Checked = "checked";
                        }

                        stringBuilder.Append("<li class=\"col-lg-2 col-md-3 col-sm-4 pull-left\">");
                        stringBuilder.Append(string.Format("<input type=\"checkbox\" name=\"perm\" id=\"{0}-{1}\" {2} value=\"{0}-{1}\">", vh.WarehouseId, perm.ActivityId, Checked));
                        stringBuilder.Append(string.Format("<label for=\"{0}-{1}\">{2}</label>", vh.WarehouseId, perm.ActivityId, perm.ActivityName));
                        stringBuilder.Append("</li>");

                        Checked = "";
                    }

                    stringBuilder.Append("</ul>");
                    stringBuilder.Append("</li>");
                }

                // check the activities which are not in any group
                var nav2 = _activityServices.GetAuthActivitiesForPermNoGroup(userModules);

                if (nav2.Count() > 0)
                {
                    stringBuilder.Append("<li>");
                    stringBuilder.Append(string.Format("<input type=\"checkbox\" name=\"Misc-{0}\" id=\"Misc-{0}\" value=\"{0}\">", vh.WarehouseId));
                    stringBuilder.Append(string.Format("<label for=\"Misc-{0}\">Miscellaneous</label>", vh.WarehouseId));
                    stringBuilder.Append("<ul>");

                    foreach (var perm2 in nav2)
                    {
                        if (_activityServices.PermCheckByActivityId(perm2.ActivityId, (int)id, vh.WarehouseId))
                        {
                            Checked = "checked";
                        }

                        stringBuilder.Append("<li>");
                        stringBuilder.Append(string.Format("<input type=\"checkbox\" name=\"perm\" id=\"{0}-{1}\" {2} value=\"{0}-{1}\">", vh.WarehouseId, perm2.ActivityId, Checked));
                        stringBuilder.Append(string.Format("<label for=\"{0}-{1}\">{2}</label>", vh.WarehouseId, perm2.ActivityId, perm2.ActivityName));
                        stringBuilder.Append("</li>");

                        Checked = "";
                    }

                    stringBuilder.Append("</ul>");
                    stringBuilder.Append("</li>");
                }

                stringBuilder.Append("</ul>");
                stringBuilder.Append("</li>");
                stringBuilder.Append("</ul>");
                stringBuilder.Append("</div>");
            }

            stringBuilder.Append("<div class=\"page-wrap-btn\">");
            stringBuilder.Append("</div>");

            ViewBag.permissions = stringBuilder.ToString();
            return View();
        }


        [HttpPost]
        public ActionResult UserPermissions(int? id, string[] perm)
        {
            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            List<AuthPermission> Permissions = new List<AuthPermission>();
            HashSet<int> wareh = new HashSet<int>();
            if (perm != null)
            {
                foreach (var chk in perm)
                {
                    String[] exploded = chk.Split('-');
                    wareh.Add(Convert.ToInt32(exploded[0]));

                    AuthPermission Permission = new AuthPermission();

                    Permission.WarehouseId = Convert.ToInt32(exploded[0]);
                    Permission.ActivityId = Convert.ToInt32(exploded[1]);
                    Permission.UserId = Convert.ToInt32(id);
                    Permission.TenantId = tenant.TenantId;
                    Permission.DateCreated = DateTime.UtcNow;
                    Permission.DateUpdated = DateTime.UtcNow;
                    Permission.CreatedBy = user.UserId;
                    Permission.UpdatedBy = user.UserId;
                    Permission.IsActive = true;
                    Permission.IsDeleted = false;

                    // add each permission in list
                    Permissions.Add(Permission);

                }

                // assign default permissions to the users for each warehouse
                foreach (var ware in wareh)
                {
                    // get list of activities which are excluded permissions
                    // these activites are to be allowed by default with each user
                    List<AuthActivity> ExActivities = new List<AuthActivity>();
                    ExActivities = _activityServices.GetExcludedActivities();

                    foreach (var Activity in ExActivities)
                    {
                        AuthPermission Permission = new AuthPermission();

                        Permission.WarehouseId = Convert.ToInt32(ware);
                        Permission.ActivityId = Activity.ActivityId;
                        Permission.UserId = Convert.ToInt32(id);
                        Permission.TenantId = tenant.TenantId;
                        Permission.DateCreated = DateTime.UtcNow;
                        Permission.DateUpdated = DateTime.UtcNow;
                        Permission.CreatedBy = user.UserId;
                        Permission.UpdatedBy = user.UserId;
                        Permission.IsActive = true;
                        Permission.IsDeleted = false;

                        // add each permission in list
                        Permissions.Add(Permission);
                    }
                }
            }

            // remove previous permissions against user if any in database context
            List<AuthPermission> OldPermissions = new List<AuthPermission>();
            OldPermissions = _activityServices.GetPermissionsByUserId((int)id);

            //remove old permissions
            _activityServices.RemoveAuthPermissions(OldPermissions);

            // add new premissions in database context
            _activityServices.AddAuthPermissions(Permissions);

            // update user dateUpdated and Updated by
            AuthUser NewUser = _userService.GetAuthUserById(Convert.ToInt32(id));

            NewUser.DateUpdated = DateTime.UtcNow;
            NewUser.UpdatedBy = user.UserId;

            _userService.UpdateAuthUserForPermissions(NewUser, user.UserId, tenant.TenantId);

            // return back to the origional refferal page
            return RedirectToAction("Index");

        }

        //check if user name already exist in database
        public JsonResult IsUserAvailable(string UserName)
        {
            if (!String.IsNullOrEmpty(UserName)) UserName = UserName.Trim();

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            int result = _userService.IsUserNameExists(UserName, tenant.TenantId);

            if (result > 0)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult _UserList()
        {
            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            var model = _userService.GetAllAuthUsersForGrid(tenant.TenantId);

            return PartialView("__UserList", model);
        }
    }
}
