using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace WMS.Controllers.TA
{
    public class RolesController : BaseController
    {
        readonly IRolesServices _rolesServices;
        private readonly IMapper _mapper;

        public RolesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IRolesServices roleServices, IMapper mapper) 
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _rolesServices = roleServices;
            _mapper = mapper;
        }

        public ActionResult Index(string message = "")
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                if (!String.IsNullOrWhiteSpace(message))
                    ViewBag.Message = message;

                return View("_Listing", _mapper.Map(_rolesServices.GetAllRoles(tenant.TenantId), new List<RolesViewModel>()));
            }
            catch (Exception e)
            {
                var err = e.Message;
                return View("_EmptyResult");
            }
        }

        public ActionResult RolesListPartial()
        {
            return PartialView("_RolesListGridPartial",
                _mapper.Map(_rolesServices.GetAllRoles(CurrentTenantId), new List<RolesViewModel>()));
        }

        [HttpPost]
        public ActionResult SearchRolesByName(string rolesName)
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                return PartialView("_Listing", _mapper.Map(_rolesServices.SearchRolesByName(rolesName, tenant.TenantId), new List<RolesViewModel>()));
            }
            catch (Exception e)
            {
                var err = e.Message;
                return View("_ErrorResult");
            }
        }

        public ActionResult Add()
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                return View("_CreateEdit", new RolesViewModel());
            }
            catch (Exception e)
            {
                var err = e.Message;
                return View("_EmptyResult");
            }
        }

        public ActionResult Details(int id, string message = "")
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                if (!String.IsNullOrWhiteSpace(message))
                    ViewBag.Message = message;

                var result = _rolesServices.GetByRolesId(id);

                return View("_CreateEdit", _mapper.Map(result, new RolesViewModel()));
            }
            catch (Exception e)
            {
                var err = e.Message;
                return View("_EmptyResult");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Submit(RolesViewModel roles)
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                if (ModelState.IsValid)
                {
                    if (roles.Id <= 0)
                    {
                        //insert
                        roles.CreatedBy = user.UserId;
                        roles.UpdatedBy = user.UserId;
                        roles.DateCreated = DateTime.UtcNow;
                        roles.DateUpdated = DateTime.UtcNow;
                        roles.TenantId = tenant.TenantId;
                        roles.Id = _rolesServices.Insert(_mapper.Map<Roles>(roles), CurrentUserId);

                        ViewBag.Message = $"Successfully Added on {DateTime.Now}.";
                    }
                    else
                    {
                        //update
                        Roles newRole = _rolesServices.GetByRolesId(roles.Id);
                        newRole.RoleName = roles.RoleName;
                        newRole.TenantId = tenant.TenantId;
                        newRole.UpdatedBy = user.UserId;
                        newRole.DateUpdated = DateTime.UtcNow;
                        _rolesServices.Update(_mapper.Map<Roles>(newRole), CurrentUserId);
                        ViewBag.Message = $"Successfully Updated on {DateTime.Now}.";
                    }

                    return RedirectToAction("Index", new { message = ViewBag.Message });
                }
                else //ModelState.IsValid is not valid
                {
                    return View("_CreateEdit", _mapper.Map(_rolesServices.GetByRolesId(roles.Id), new RolesViewModel()));
                }
            }
            catch (Exception e)
            {
                var err = e.Message;
                return View("_ErrorResult");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Delete(int Id)
        {
            if (!caSession.AuthoriseSession()) return Json(new { success = false });

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            try
            {
                _rolesServices.Delete(_mapper.Map<Roles>(new RolesViewModel { Id = Id }), CurrentUserId);
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                var err = e.Message;
            }

            return Json(new { success = false });
        }
    }
}