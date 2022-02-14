using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    public static class caSession
    {
        public static bool AuthoriseSession()
        {
            bool Status = false;
            string ControllerName = "";
            string ActionName = "";
            int CurrentWarehouseId = 0;

            HttpContext.Current.Session["ErrorUrl"] = "~/error";

            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("action"))
                {
                    ControllerName = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
                    ActionName = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();

                }
            }

            if (HttpContext.Current.Session["caTenant"] == null)
            {
                Uri Url = HttpContext.Current.Request.Url;
                caTenant ca = new caTenant();

                if (ca.AuthorizeTenant(Url) == true)
                {
                    HttpContext.Current.Session["caTenant"] = ca;
                }
            }


            // check conditions

            if (HttpContext.Current.Session["caTenant"] == null)
            {
                // set error details
                caError error = new caError();
                error.ErrorTtile = "Client not validated";
                error.ErrorMessage = "Sorry, system is unable to validate client";
                error.ErrorDetail = "Either client is not registered, inactive or ambiguous, please contact support";
                error.ErrorController = ControllerName;
                error.ErrorAction = ActionName;

                HttpContext.Current.Session["caError"] = error;
                HttpContext.Current.Session["ErrorUrl"] = "~/error";
            }

            else
            {
                if (HttpContext.Current.Session["caUser"] == null)
                {
                    HttpContext.Current.Session["ErrorUrl"] = "~/user/login";
                    if (HttpContext.Current.Session["LastUrlFrom"] == null)
                    {
                        HttpContext.Current.Session["LastUrlFrom"] = HttpContext.Current.Request.RawUrl;
                    }
                }
                else
                {
                    caTenant tenant = (caTenant)HttpContext.Current.Session["caTenant"];
                    caUser user = (caUser)HttpContext.Current.Session["caUser"];

                    if (tenant.TenantId.Equals(user.TenantId))
                    {
                        if (user.SuperUser == true)
                        {
                            if (HttpContext.Current.Session["CurrentWarehouseId"] == null)
                            {
                                HttpContext.Current.Session["CurrentWarehouseId"] = tenant.TenantLocations.Where(x => x.IsDeleted != true && x.IsActive == true).FirstOrDefault().WarehouseId;
                            }
                            Status = true;
                            return Status;
                        }

                        if (HttpContext.Current.Session["CurrentWarehouseId"] == null)
                        {
                            if (user.AuthPermissions.Any())
                            {
                                CurrentWarehouseId = user.AuthPermissions.Where(x => x.IsDeleted != true && x.IsActive != false).FirstOrDefault().WarehouseId;
                            }
                            HttpContext.Current.Session["CurrentWarehouseId"] = CurrentWarehouseId;

                        }
                        else
                        {
                            CurrentWarehouseId = (int)HttpContext.Current.Session["CurrentWarehouseId"];
                        }

                        ICollection<AuthActivity> CurrentActivity = context.AuthActivities.AsNoTracking().Where(e => e.ActivityController.Trim().ToLower() == ControllerName.Trim().ToLower()
                            && e.ActivityAction.Trim().ToLower() == ActionName.Trim().ToLower() && e.IsActive == true && e.IsDeleted != true).ToList();

                        if (CurrentActivity.Count() == 0 || CurrentActivity.Count() > 1)
                        {
                            caError error = new caError();

                            if (CurrentActivity.Count() == 0)
                            {

                                error.ErrorTtile = "No authorisation for requested resources";
                                error.ErrorMessage = "Sorry, requested activity is not registered, and cannot be allowed to view";
                                error.ErrorDetail = "Problem getting activity, Activity is not registerd or inactive";
                                error.ErrorController = ControllerName;
                                error.ErrorAction = ActionName;

                            }

                            if (CurrentActivity.Count() > 1)
                            {
                                error.ErrorTtile = "No authorisation for requested resources";
                                error.ErrorMessage = "Sorry, Duplicate Entry for This Activity";
                                error.ErrorDetail = "Activity is found more then once, please contact support for assistence";
                                error.ErrorController = ControllerName;
                                error.ErrorAction = ActionName;

                            }

                            HttpContext.Current.Session["caError"] = error;
                            HttpContext.Current.Session["ErrorUrl"] = "~/error";
                        }

                        else
                        {
                            int ThisActivity = CurrentActivity.First().ActivityId;
                            ICollection<AuthPermission> permissons = user.AuthPermissions;

                            if (permissons.Any(c => c.ActivityId == ThisActivity && c.WarehouseId == CurrentWarehouseId && c.IsActive == true
                                && c.IsDeleted == false) || CurrentActivity.First().ExcludePermission == true)
                            {
                                Status = true;

                                if (user.AuthUserStatus)
                                {
                                    AuthUserLoginActivity LoginActivity = new AuthUserLoginActivity();

                                    LoginActivity.ActivityId = ThisActivity;
                                    LoginActivity.UserLoginId = (int)HttpContext.Current.Session["CurrentUserLoginId"];
                                    LoginActivity.WarehouseId = CurrentWarehouseId;
                                    LoginActivity.DateCreated = DateTime.UtcNow;
                                    LoginActivity.TenantId = tenant.TenantId;

                                    context.AuthUsersLoginActivities.Add(LoginActivity);
                                    context.SaveChanges();
                                }
                            }
                            else
                            {
                                caError error = new caError();
                                error.ErrorTtile = "No authorisation for requested resources";
                                error.ErrorMessage = "Sorry, you dont have permissions to access this activity, Please contact Admin";
                                error.ErrorDetail = "This Activity is not authorised for current User";
                                error.ErrorController = ControllerName;
                                error.ErrorAction = ActionName;
                                HttpContext.Current.Session["caError"] = error;
                                HttpContext.Current.Session["ErrorUrl"] = "~/error";
                            }
                        }

                    }
                    else
                    {
                        caError error = new caError();
                        error.ErrorTtile = "Unable to validate user against client";
                        error.ErrorMessage = "Sorry, system is unable to validate user against client";
                        error.ErrorDetail = "System cannot verify user association with client. Please contact sypport.";
                        error.ErrorController = ControllerName;
                        error.ErrorAction = ActionName;
                        HttpContext.Current.Session["caError"] = error;
                        HttpContext.Current.Session["ErrorUrl"] = "~/error";
                    }
                }
            }

            return Status;
        }

        public static bool AuthorizeVechile(string id)
        {
            caTenant ca = new caTenant();
            var VechileId = ca.AuthorizeVechile(id);
            if (VechileId > 0)
            {
                HttpContext.Current.Session["VechileId"] = VechileId;
                return true;
            }
            return false;



        }
    }
}