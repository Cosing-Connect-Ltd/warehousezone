using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ActivityController : BaseController
    {
        private readonly IActivityServices _activityServices;

        public ActivityController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IActivityServices activityServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _activityServices = activityServices;
        }

        // GET: /Activity/
        public ActionResult Index(string q, int page = 1, int ppitems = 5)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();

        }

        // GET: /Activity/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuthActivity authactivity = _activityServices.GetActivityById((int)id);
            if (authactivity == null)
            {
                return HttpNotFound();
            }
            return View(authactivity);
        }

        // GET: /Activity/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ActivityName,ActivityController,ActivityAction,IsActive,ExcludePermission,RightNav")] AuthActivity authactivity)
        {
            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();

                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _activityServices.SaveActivity(authactivity, user.UserId, tenant.TenantId);
                return RedirectToAction("Index");
            }

            return View(authactivity);
        }

        // GET: /Activity/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuthActivity authactivity = _activityServices.GetActivityById((int)id);
            if (authactivity == null)
            {
                return HttpNotFound();
            }

            return View(authactivity);
        }

        // POST: /Activity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuthActivity authactivity)
        {
            if (ModelState.IsValid)
            {
                caTenant tenant = caCurrent.CurrentTenant();
                caUser user = caCurrent.CurrentUser();

                bool status = _activityServices.UpdateActivity(authactivity, user.UserId, tenant.TenantId);

                if (status == false)
                {
                    ViewBag.Error = "Problem updating record. Please contact support";
                    return View(authactivity);
                }


                return RedirectToAction("Index");
            }

            return View(authactivity);
        }

        // GET: /Activity/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuthActivity authactivity = _activityServices.GetActivityById((int)id);
            if (authactivity == null)
            {
                return HttpNotFound();
            }
            return View(authactivity);
        }

        // POST: /Activity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            AuthActivity authactivity = _activityServices.GetActivityById((int)id);

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            _activityServices.DeleteActivity(authactivity, user.UserId);

            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult _ActivityList()
        {
            var model = _activityServices.GetAllActivities(CurrentTenantId);
            return PartialView("__ActivityList", model);
        }
    }
}
