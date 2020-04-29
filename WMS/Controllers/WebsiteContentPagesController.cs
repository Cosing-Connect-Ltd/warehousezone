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
    public class WebsiteContentPagesController : Controller
    {
        private ApplicationContext db = new ApplicationContext();

        // GET: WebsiteContentPages
        public ActionResult Index()
        {
            var websiteContentPages = db.WebsiteContentPages.Include(w => w.TenantWebsites);
            return View(websiteContentPages.ToList());
        }

        // GET: WebsiteContentPages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteContentPages websiteContentPages = db.WebsiteContentPages.Find(id);
            if (websiteContentPages == null)
            {
                return HttpNotFound();
            }
            return View(websiteContentPages);
        }

        // GET: WebsiteContentPages/Create
        public ActionResult Create()
        {
            ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName");
            return View();
        }

        // POST: WebsiteContentPages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SiteID,Title,MetaTitle,MetaDescription,Contant,SortOrder,IsActive,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] WebsiteContentPages websiteContentPages)
        {
            if (ModelState.IsValid)
            {
                db.WebsiteContentPages.Add(websiteContentPages);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName", websiteContentPages.SiteID);
            return View(websiteContentPages);
        }

        // GET: WebsiteContentPages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteContentPages websiteContentPages = db.WebsiteContentPages.Find(id);
            if (websiteContentPages == null)
            {
                return HttpNotFound();
            }
            ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName", websiteContentPages.SiteID);
            return View(websiteContentPages);
        }

        // POST: WebsiteContentPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,SiteID,Title,MetaTitle,MetaDescription,Contant,SortOrder,IsActive,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] WebsiteContentPages websiteContentPages)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(websiteContentPages).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName", websiteContentPages.SiteID);
        //    return View(websiteContentPages);
        //}

        // GET: WebsiteContentPages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteContentPages websiteContentPages = db.WebsiteContentPages.Find(id);
            if (websiteContentPages == null)
            {
                return HttpNotFound();
            }
            return View(websiteContentPages);
        }

        // POST: WebsiteContentPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WebsiteContentPages websiteContentPages = db.WebsiteContentPages.Find(id);
            db.WebsiteContentPages.Remove(websiteContentPages);
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
