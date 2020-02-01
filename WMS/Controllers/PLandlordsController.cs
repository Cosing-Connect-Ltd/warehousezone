using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class PLandlordsController : BaseController
    {
        public PLandlordsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices) : base(orderService, propertyService, accountServices, lookupServices)
        {

        }
        // GET: PLandlords
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        //public ActionResult PLandlordsGridview()
        //{
        //    ViewBag.LandLords = PropertyService.GetAllValidPropertyLandlords().ToList();
        //    return PartialView("_LandlordsListGridview");
        //}
        public ActionResult PLandlordsGridview()
        {


            var viewModel = GridViewExtension.GetViewModel("_LandlordsListGridview");

            if (viewModel == null)
                viewModel = PLandlordsCustomBinding.CreatePlandlordGridViewModel();

            return PlandlordsGridActionCore(viewModel);

        }
        public ActionResult PlandlordsGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    PLandlordsCustomBinding.PlandlordGetDataRowCount(args);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        PLandlordsCustomBinding.PlandlordGetData(args);
                    })
            );
            return PartialView("_LandlordsListGridview", gridViewModel);
        }

        public ActionResult _PlandlordGridViewsPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("PLandlordsGridview");
            viewModel.Pager.Assign(pager);
            return PlandlordsGridActionCore(viewModel);
        }

        public ActionResult _PlandlordGridViewFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("PLandlordsGridview");
            viewModel.ApplyFilteringState(filteringState);
            return PlandlordsGridActionCore(viewModel);
        }
        public ActionResult _PlandlordGridViewDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("PLandlordsGridview");
            viewModel.ApplySortingState(column, reset);
            return PlandlordsGridActionCore(viewModel);
        }








        // GET: PLandlords/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pLandlord = PropertyService.GetPropertyLandlordById(id??0);
            if (pLandlord == null)
            {
                return HttpNotFound();
            }
            return View(pLandlord);
        }

        // GET: PLandlords/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        // POST: PLandlords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PLandlordId,LandlordCode,LandlordFullname,LandlordSalutation,LandlordStatus,LandlordAdded,LandlordNotes1,LandlordNotes2,UserNotes1,UserNotes2,SiteId,SyncRequiredFlag,AddressLine1,AddressLine2,AddressLine3,AddressLine4,AddressPostcode,HomeTelephone,WorkTelephone1,WorkTelephone2,WorkTelephoneFax,MobileNumber,Email")] PLandlord pLandlord)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
             
            if (ModelState.IsValid)
            {
                PropertyService.SavePLandlord(pLandlord, CurrentUserId); 
                return RedirectToAction("Index");
            }

            return View(pLandlord);
        }

        // GET: PLandlords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLandlord pLandlord = PropertyService.GetPropertyLandlordById(id??0);
            if (pLandlord == null)
            {
                return HttpNotFound();
            }
            return View(pLandlord);
        }

        // POST: PLandlords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PLandlordId,LandlordCode,LandlordFullname,LandlordSalutation,LandlordStatus,LandlordAdded,LandlordNotes1,LandlordNotes2,UserNotes1,UserNotes2,SiteId,SyncRequiredFlag,AddressLine1,AddressLine2,AddressLine3,AddressLine4,AddressPostcode,HomeTelephone,WorkTelephone1,WorkTelephone2,WorkTelephoneFax,MobileNumber,Email")] PLandlord pLandlord)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                PropertyService.SavePLandlord(pLandlord, CurrentUserId);
                
                return RedirectToAction("Index");
            }
            return View(pLandlord);
        }

        // GET: PLandlords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pLandlord = PropertyService.GetPropertyLandlordById(id ?? 0);
            if (pLandlord == null)
            {
                return HttpNotFound();
            }
            return View(pLandlord);
        }

        // POST: PLandlords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            PropertyService.DeletePropertyLandlord(id);
            return RedirectToAction("Index");
        }
         
    }
}
