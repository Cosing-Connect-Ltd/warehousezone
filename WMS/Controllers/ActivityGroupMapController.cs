using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ActivityGroupMapController : BaseController
    {
        private readonly IActivityServices _activityServices;

        public ActivityGroupMapController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IActivityServices activityServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _activityServices = activityServices;
        }
        // GET: /ActivityGroupMap/
        public ActionResult Index(string q, int page = 1, int ppitems = 5)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }


        // GET: /ActivityGroupMap/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AuthActivityGroupMap authactivitygroupmap = _activityServices.GetActivityGroupMapById((int)id);
            if (authactivitygroupmap == null)
            {
                return HttpNotFound();
            }
            return View(authactivitygroupmap);
        }

        // GET: /ActivityGroupMap/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            ViewBag.ActivityId = new SelectList(LookupServices.GetAllActiveActivities(), "ActivityId", "ActivityName");
            ViewBag.ActivityGroupId = new SelectList(LookupServices.GetAllActiveActivityGroups(), "ActivityGroupId", "ActivityGroupName");
            return View();
        }

        // POST: /ActivityGroupMap/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ActivityGroupMapId,ActivityId,ActivityGroupId,IsActive,IsDeleted")] AuthActivityGroupMap authactivitygroupmap)
        {
            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();
                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _activityServices.SaveActivityGroupMap(authactivitygroupmap, user.UserId, tenant.TenantId);
                return RedirectToAction("Index");
            }

            ViewBag.ActivityId = new SelectList(LookupServices.GetAllActiveActivities(), "ActivityId", "ActivityName", authactivitygroupmap.ActivityId);
            ViewBag.ActivityGroupId = new SelectList(LookupServices.GetAllActiveActivityGroups(), "ActivityGroupId", "ActivityGroupName", authactivitygroupmap.ActivityGroupId);
            return View(authactivitygroupmap);
        }

        // GET: /ActivityGroupMap/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AuthActivityGroupMap authactivitygroupmap = _activityServices.GetActivityGroupMapById((int)id);
            if (authactivitygroupmap == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityId = new SelectList(LookupServices.GetAllActiveActivities(), "ActivityId", "ActivityName", authactivitygroupmap.ActivityId);
            ViewBag.ActivityGroupId = new SelectList(LookupServices.GetAllActiveActivityGroups(), "ActivityGroupId", "ActivityGroupName", authactivitygroupmap.ActivityGroupId);
            return View(authactivitygroupmap);
        }

        // POST: /ActivityGroupMap/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ActivityGroupMapId,ActivityId,ActivityGroupId,IsActive")] AuthActivityGroupMap authactivitygroupmap)
        {
            if (ModelState.IsValid)
            {

                caTenant tenant = caCurrent.CurrentTenant();

                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _activityServices.UpdateActivityGroupMap(authactivitygroupmap, user.UserId, tenant.TenantId);
                return RedirectToAction("Index");
            }
            ViewBag.ActivityId = new SelectList(LookupServices.GetAllActiveActivities(), "ActivityId", "ActivityName", authactivitygroupmap.ActivityId);
            ViewBag.ActivityGroupId = new SelectList(LookupServices.GetAllActiveActivityGroups(), "ActivityGroupId", "ActivityGroupName", authactivitygroupmap.ActivityGroupId);
            return View(authactivitygroupmap);
        }

        // GET: /ActivityGroupMap/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            AuthActivityGroupMap authactivitygroupmap = _activityServices.GetActivityGroupMapById((int)id);
            if (authactivitygroupmap == null)
            {
                return HttpNotFound();
            }
            return View(authactivitygroupmap);
        }

        // POST: /ActivityGroupMap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AuthActivityGroupMap authactivitygroupmap = _activityServices.GetActivityGroupMapById((int)id);

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            _activityServices.DeleteActivityGroupMap(authactivitygroupmap, user.UserId);
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult _AGroupList()
        {
            var model = _activityServices.GetActivityGroupMapsForGroupList();
            return PartialView("__AGroupList", model.ToList());
        }

    }
}
