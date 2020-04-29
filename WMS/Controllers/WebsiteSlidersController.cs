using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;

namespace WMS.Controllers
{
    public class WebsiteSlidersController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: WebsiteSliders
        public ActionResult Index()
        {
            var websiteSliders = db.WebsiteSliders.Include(w => w.TenantWebsites);
            return View(websiteSliders.ToList());
        }

        // GET: WebsiteSliders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteSlider websiteSlider = db.WebsiteSliders.Find(id);
            if (websiteSlider == null)
            {
                return HttpNotFound();
            }
            return View(websiteSlider);
        }

        // GET: WebsiteSliders/Create
        public ActionResult Create()
        {
            ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName");
            return View();
        }

        // POST: WebsiteSliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SiteID,Image,IamgeAltTag,Text,ButtonText,ButtonLinkUrl,SortOrder,IsActive,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] WebsiteSlider websiteSlider)
        {
            if (ModelState.IsValid)
            {
                db.WebsiteSliders.Add(websiteSlider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName", websiteSlider.SiteID);
            return View(websiteSlider);
        }

        // GET: WebsiteSliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteSlider websiteSlider = db.WebsiteSliders.Find(id);
            if (websiteSlider == null)
            {
                return HttpNotFound();
            }
            ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName", websiteSlider.SiteID);
            return View(websiteSlider);
        }

        // POST: WebsiteSliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SiteID,Image,IamgeAltTag,Text,ButtonText,ButtonLinkUrl,SortOrder,IsActive,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] WebsiteSlider websiteSlider)
        {
            if (ModelState.IsValid)
            {
             //   db.Entry(websiteSlider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName", websiteSlider.SiteID);
            return View(websiteSlider);
        }

        // GET: WebsiteSliders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteSlider websiteSlider = db.WebsiteSliders.Find(id);
            if (websiteSlider == null)
            {
                return HttpNotFound();
            }
            return View(websiteSlider);
        }

        // POST: WebsiteSliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WebsiteSlider websiteSlider = db.WebsiteSliders.Find(id);
            db.WebsiteSliders.Remove(websiteSlider);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
