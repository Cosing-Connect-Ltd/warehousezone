using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ActivityGroupController : BaseController
    {
        private readonly IActivityServices _activityServices;

        public ActivityGroupController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IActivityServices activityServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _activityServices = activityServices;
        }
        // GET: /ActivityGroup/
        public ActionResult Index(string q, int page = 1, int ppitems = 5)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();

        }

        // GET: /ActivityGroup/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AuthActivityGroup authactivitygroup = _activityServices.GetActivityGroupById((int)id);
            if (authactivitygroup == null)
            {
                return HttpNotFound();
            }
            return View(authactivitygroup);
        }

        // GET: /ActivityGroup/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ActivityGroupName,ActivityGroupDetail,ActivityGroupParentId,IsActive")] AuthActivityGroup authactivitygroup)
        {
            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();
                // get properties of user
                caUser user = caCurrent.CurrentUser();

                int res = _activityServices.SaveActivityGroup(authactivitygroup, user.UserId, tenant.TenantId);
                return RedirectToAction("Index");
            }

            return View(authactivitygroup);
        }

        // GET: /ActivityGroup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AuthActivityGroup authactivitygroup = _activityServices.GetActivityGroupById((int)id);
            if (authactivitygroup == null)
            {
                return HttpNotFound();
            }
            return View(authactivitygroup);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityGroupId,ActivityGroupName,ActivityGroupDetail,IsActive")] AuthActivityGroup authactivitygroup)
        {
            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();

                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _activityServices.UpdateActivityGroup(authactivitygroup, user.UserId, tenant.TenantId);
                return RedirectToAction("Index");
            }
            return View(authactivitygroup);
        }

        // GET: /ActivityGroup/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AuthActivityGroup authactivitygroup = _activityServices.GetActivityGroupById((int)id);
            if (authactivitygroup == null)
            {
                return HttpNotFound();
            }
            return View(authactivitygroup);
        }

        // POST: /ActivityGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuthActivityGroup authactivitygroup = _activityServices.GetActivityGroupById((int)id);

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            _activityServices.DeleteActivityGroup(authactivitygroup, user.UserId);
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult _ActivityGroupList()
        {
            var model = _activityServices.GetAllActivityGroups(CurrentTenantId);
            return PartialView("__ActivityGroupList", model);
        }
    }
}
