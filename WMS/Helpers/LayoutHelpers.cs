using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WMS.Helpers
{
    public class LayoutHelpers
    {

        // Helper to handle right navigation and permissions display
        public static string NavigationMenus()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var activityService = DependencyResolver.Current.GetService<IActivityServices>();
            var tenantService = DependencyResolver.Current.GetService<ITenantsServices>();

            if (HttpContext.Current.Session["caUser"] != null)
            {
                var user = (caUser)HttpContext.Current.Session["caUser"];
                var currentWarehouseId = (int)HttpContext.Current.Session["CurrentWarehouseId"];
                var userModules = tenantService.GetAllTenantModules(user.TenantId).Select(m => m.ModuleId).ToList();

#if DEBUG
                //if (tenantService.GetAllTenants().Count() > 1)
                //{
                //    if (user.SuperUser == true)
                //    {
                //        userModules = tenantService.GetAllTenantModules(0).Select(m => m.ModuleId).ToList();
                //    }
                //    else
                //    {
                //        userModules = tenantService.GetAllTenantModules(user.TenantId).Select(m => m.ModuleId).ToList();
                //    }
                //}
#endif
                if (user.SuperUser != true)
                {
                    var Groups = activityService.GetDistinctActivityGroupsForNavigation(user.UserId, currentWarehouseId, userModules).ToList();

                    var counter = 1;

                    var navigation = activityService.GetDistinctActivityPermissionsForUserNavigation(user.UserId, currentWarehouseId, userModules).ToList();

                    foreach (var grp in Groups)
                    {
                        stringBuilder.Append(string.Format("<h3 class=\"item{0}\"> <i class=\"nav-icon {1}\"></i><span>" + grp.ActivityGroupName + "</span></h3>", counter, grp.GroupIcon));
                        stringBuilder.Append("<div>");
                        stringBuilder.Append("<ul>");

                        var navByGroup = navigation.Where(x => x.ActivityGroupId == grp.ActivityGroupId);

                        foreach (var perm in navByGroup)
                        {
                            stringBuilder.Append("<li><a href='/" + perm.Controller + "/" + perm.Action + "'>" + perm.ActivityName + "</a></li>");
                        }
                        stringBuilder.Append("</ul>");
                        stringBuilder.Append("</div>");
                        counter++;
                    }

                    var navigationNoGroup = activityService.GetDistinctActivityNonGroupsForNavigation(user.UserId, currentWarehouseId, userModules).ToList();

                    if (navigationNoGroup.Any())
                    {
                        stringBuilder.Append("<h3>Miscellaneous</h3>");
                        stringBuilder.Append("<div>");
                        stringBuilder.Append("<ul>");

                        foreach (var perm2 in navigationNoGroup)
                        {
                            stringBuilder.Append("<li><a href='/" + perm2.Controller + "/" + perm2.Action + "'>" + perm2.ActivityName + "</a></li>");
                        }
                        stringBuilder.Append("</ul>");
                        stringBuilder.Append("</div>");
                    }
                }
                else
                {
                    var superGroup = activityService.GetDistinctActivityGroupsForSuperUserNavigation(userModules).ToList();

                    var counter = 1;

                    var nav = activityService.GetDistinctActivitiesForSuperUserNavigation(userModules).ToList();

                    foreach (var grp in superGroup)
                    {

                        stringBuilder.Append(string.Format("<h3 class=\"item{0}\"> <i class=\"nav-icon {1}\"></i><span>" + grp.ActivityGroupName + "</span></h3>", counter, grp.GroupIcon));
                        stringBuilder.Append("<div>");
                        stringBuilder.Append("<ul>");
                        var navByGroup = nav.Where(x => x.ActivityGroupId == grp.ActivityGroupId);
                        foreach (var perm in navByGroup)
                        {
                            stringBuilder.Append("<li><a href='/" + perm.Controller + "/" + perm.Action + "'>" + perm.ActivityName + "</a></li>");
                        }
                        stringBuilder.Append("</ul>");
                        stringBuilder.Append("</div>");
                        counter++;
                    }

                    var supernav2 = activityService.GetDistinctActivityNonGroupsForSuperUserNavigation(userModules).ToList();

                    if (supernav2.Any())
                    {
                        stringBuilder.Append("<h3>Miscellaneous</h3>");
                        stringBuilder.Append("<div>");
                        stringBuilder.Append("<ul>");

                        foreach (var perm2 in supernav2)
                        {
                            stringBuilder.Append("<li><a href='/" + perm2.Controller + "/" + perm2.Action + "'>" + perm2.ActivityName + "</a></li>");
                        }
                        stringBuilder.Append("</ul>");
                        stringBuilder.Append("</div>");
                    }
                }
            }

            return stringBuilder.ToString();
        }

        // helper to handle warehouses dropdown data
        public static string WarehDropdown(bool includeMobileLocations = false)
        {
            var dropdown = "";
            var curent = "";
            StringBuilder stringBuilder = new StringBuilder();

            var activityService = DependencyResolver.Current.GetService<IActivityServices>();

            if (HttpContext.Current.Session["caUser"] != null)
            {
                var user = (caUser)HttpContext.Current.Session["caUser"];

                var currentWarehouseId = (int)HttpContext.Current.Session["CurrentWarehouseId"];

                stringBuilder.Append("<form name=\"wareh-select-form\" method=\"post\" id=\"wareh-select-form\" action=\"/user/WarehChange\">");
                stringBuilder.Append("<select name=\"wareh-select\" id=\"wareh-select\">");

                var wareh = activityService.GetAllPermittedWarehousesForUser(user.UserId, user.TenantId, user.SuperUser == true, includeMobileLocations);

                foreach (var w in wareh)
                {
                    if (w.WId.Equals(currentWarehouseId)) { curent = "selected"; }
                    stringBuilder.Append($"<option {curent} value=\"{w.WId}\">{w.WName}</option>");
                    curent = "";
                }

                stringBuilder.Append("</select>");
                stringBuilder.Append("<input id=\"wareh-submit\" type=\"submit\" value=\"Change\" name=\"submit\" />");
                stringBuilder.Append("</form>");
            }

            dropdown = stringBuilder.ToString();
            return dropdown;
        }


        //helper to validate activity
        //accepts two string parameters and return true or false

        public static Boolean ActivityValidator(string Controller, string Action)
        {
            bool Status = false;
            int CurrentWarehouseId;
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            if (HttpContext.Current.Session["caUser"] != null)
            {
                // get properties of user
                caUser user = (caUser)HttpContext.Current.Session["caUser"];

                //check if user is super user
                if (user.SuperUser == true)
                {
                    Status = true;
                    return Status;
                }

                // assign warehouse id from session
                CurrentWarehouseId = (int)HttpContext.Current.Session["CurrentWarehouseId"];

                // check in permissions if this activity is available
                ICollection<AuthPermission> permissons = user.AuthPermissions;
                // checking permissions
                if (permissons.Any(c => c.AuthActivity.ActivityController.Equals(Controller, StringComparison.OrdinalIgnoreCase) && c.AuthActivity.ActivityAction.Equals(Action, StringComparison.OrdinalIgnoreCase)
                && c.WarehouseId == CurrentWarehouseId && c.IsActive == true && c.IsDeleted != true))
                {
                    Status = true;
                }


                AuthActivity CurrentActivity = context.AuthActivities.AsNoTracking().Where(e => e.ActivityController.Trim().ToLower() == Controller.Trim().ToLower()
                            && e.ActivityAction.Trim().ToLower() == Action.Trim().ToLower() && e.IsActive == true && e.IsDeleted != true && e.ExcludePermission == true).FirstOrDefault();

                if (CurrentActivity != null)
                {
                    Status = true;
                }
            }

            return Status;
        }

        // get user login status using this helper
        // returns true or false aginast current user in sesssion
        public static bool UserLoginStatus()
        {
            bool status = false;

            if (HttpContext.Current.Session["caUser"] != null)
            {
                caUser user = (caUser)HttpContext.Current.Session["caUser"];
                if (user.AuthUserStatus == true) status = true;
            }
            return status;
        }

        public static bool IsSuperUser()
        {
            bool isSuperUser = false;

            if (HttpContext.Current.Session["caUser"] != null)
            {
                // get properties of user
                caUser user = (caUser)HttpContext.Current.Session["caUser"];
                if (user.AuthUserStatus == true) isSuperUser = user.SuperUser == true;
            }
            return isSuperUser;
        }

        public static string GetCurrentUserName()
        {
            string userName = "";

            if (HttpContext.Current.Session["caUser"] != null)
            {
                // get properties of user
                caUser user = (caUser)HttpContext.Current.Session["caUser"];
                if (user.AuthUserStatus == true) userName = user.UserName;
            }
            return userName;
        }
        public static List<SelectListItem> MailMergeVariablesList => GetMailMergeVariableEnumList<MailMergeVariableEnum>();

        public static List<SelectListItem> GetMailMergeVariableEnumList<T>()
        {
            var enumValues = Enum.GetValues(typeof(MailMergeVariableEnum))
                .Cast<MailMergeVariableEnum>()
                .Select(d => Tuple.Create(((int)d).ToString(), d.ToString()))
                .ToList();
            var results = enumValues.Select(enu => new SelectListItem() { Text = enu.Item2.ToString(), Value = enu.Item1 }).ToList();
            return results;
        }

        public static string GetUserName(int? userId)
        {
            if (!userId.HasValue) return "";
            var helperService = DependencyResolver.Current.GetService<IUserService>();
            return helperService.GetAuthUserById(userId.Value)?.UserName;
        }

        public static string GetStStatusString(int statusCode)
        {
            var helperService = DependencyResolver.Current.GetService<IGaneConfigurationsHelper>();
            return helperService.GetStStatusString(statusCode);
        }

        public static bool GetStStatus(int id)
        {
            var helperService = DependencyResolver.Current.GetService<IGaneConfigurationsHelper>();
            return helperService.GetStStatus(id);
        }
        public static string GetDeviceLastIp(string serial)
        {
            var helperService = DependencyResolver.Current.GetService<IGaneConfigurationsHelper>();
            return helperService.GetDeviceLastIp(serial);
        }
        public static string GetDeviceLastPingDate(string serial)
        {
            var helperService = DependencyResolver.Current.GetService<IGaneConfigurationsHelper>();
            return helperService.GetDeviceLastPingDate(serial);
        }
        public static bool GetDeviceCurrentStatus(string serial)
        {
            var helperService = DependencyResolver.Current.GetService<IGaneConfigurationsHelper>();
            return helperService.GetDeviceCurrentStatus(serial);
        }

        public static bool ActiveStocktake(int warehouseId)
        {
            var helperService = DependencyResolver.Current.GetService<IGaneConfigurationsHelper>();
            return helperService.ActiveStocktake(warehouseId);
        }


    }
}
