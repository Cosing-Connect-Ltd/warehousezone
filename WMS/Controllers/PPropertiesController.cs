using System;
using System.Collections.Generic;
using System.Data;
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
    public class PPropertiesController : BaseController
    {
        public PPropertiesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices) : base(orderService, propertyService, accountServices, lookupServices)
        {

        }

        // GET: PProperties
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
           
            return View();
        }

        //public ActionResult PPropertiesGridview(int? id)
        //{
        //    ViewBag.Properties = PropertyService.GetAllValidProperties(id);
        //    if (id.HasValue)
        //    {
        //        ViewBag.DisableCallBacks = true;
        //    }

        //    return PartialView("_PropertiesListGridview");
        //}

        public ActionResult PPropertiesGridview(int? id)
        {
          
            if (id.HasValue)
            {
                
                ViewBag.DisableCallBacks = true;
            }
            

            var viewModel = GridViewExtension.GetViewModel("PPropertiesGridview");

            if (viewModel == null)
                viewModel = PPropertiesCustomBinding.CreatePPropertiesGridViewModel();

            return PPropertiesGridActionCore(viewModel,id);

        }
        public ActionResult PPropertiesGridActionCore(GridViewModel gridViewModel,int? id=null)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    PPropertiesCustomBinding.PPropertiesGetDataRowCount(args,id);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        PPropertiesCustomBinding.PPropertiesGetData(args,id);
                    })
            );
            return PartialView("_PropertiesListGridview", gridViewModel);
        }

        public ActionResult _PPropertiesGridViewsPaging(GridViewPagerState pager)
        {
          
            var viewModel = GridViewExtension.GetViewModel("PPropertiesGridview");
            viewModel.Pager.Assign(pager);
            return PPropertiesGridActionCore(viewModel);
        }

        public ActionResult _PPropertiesGridViewFiltering(GridViewFilteringState filteringState)
        {
           
            var viewModel = GridViewExtension.GetViewModel("PPropertiesGridview");
            viewModel.ApplyFilteringState(filteringState);
            return PPropertiesGridActionCore(viewModel);
        }
        public ActionResult _PPropertiesGridViewDataSorting(GridViewColumnState column, bool reset)
        {
           
            var viewModel = GridViewExtension.GetViewModel("PPropertiesGridview");
            viewModel.ApplySortingState(column, reset);
            return PPropertiesGridActionCore(viewModel);
        }




        // GET: PProperties/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.PropertyId = id;
            ViewBag.Guid = Guid.NewGuid();
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //var pProperty = PropertyService.GetPropertyById(id.Value);
            //if (pProperty == null)
            //{
            //    return HttpNotFound();
            //}
            return View();
        }

        // GET: PProperties/Create
        public ActionResult Create(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var landlords = PropertyService.GetAllValidPropertyLandlords().Select(m => new SelectListItem() { Value=m.PLandlordId.ToString(), Text = m.LandlordFullname + "(" + m.LandlordCode + ")"});
            ViewBag.CurrentLandlordId = new SelectList(landlords, "Value", "Text", id);
            ViewBag.ReturnUrl = Url.Action("Edit", "PLandlords", new { id });
            return View();
        }

        // POST: PProperties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "PPropertyId,PropertyCode,AddressLine1,AddressLine2,AddressLine3,AddressLine4,AddressLine5,AddressPostcode,PropertyStatus,IsVacant,DateAvailable,DateAdded,PropertyBranch,TenancyMonths,SiteId,SyncRequiredFlag,LetDate,CurrentLandlordId")]
            PProperty pProperty, FormCollection form)
        {
            if (!caSession.AuthoriseSession())
            {
                return Redirect((string) Session["ErrorUrl"]);
            }
            if (ModelState.IsValid)
            {
                pProperty.IsVacant = !pProperty.LetDate.HasValue;
                pProperty.DateAdded = DateTime.UtcNow;
                PropertyService.SavePProperty(pProperty, CurrentUserId);
                return RedirectToAction("Index");
            }

            var landlords = PropertyService.GetAllValidPropertyLandlords().Select(m => new SelectListItem()
            {
                Value = m.PLandlordId.ToString(),
                Text = m.LandlordFullname + "(" + m.LandlordCode + ")"
            });
            ViewBag.CurrentLandlordId = new SelectList(landlords, "Value", "Text", pProperty.CurrentLandlordId);

            return View(pProperty);
        }

        // GET: PProperties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PProperty pProperty = PropertyService.GetPropertyById(id.Value);
            if (pProperty == null)
            {
                return HttpNotFound();
            }
            var landlords = PropertyService.GetAllValidPropertyLandlords().Select(m => new SelectListItem() { Value = m.PLandlordId.ToString(), Text = m.LandlordFullname + "(" + m.LandlordCode + ")" });
            ViewBag.CurrentLandlordId = new SelectList(landlords, "Value", "Text", pProperty.CurrentLandlordId);

            return View(pProperty);
        }

        // POST: PProperties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PPropertyId,PropertyCode,AddressLine1,AddressLine2,AddressLine3,AddressLine4,AddressLine5,AddressPostcode,PropertyStatus,IsVacant,DateAvailable,DateAdded,PropertyBranch,TenancyMonths,SiteId,SyncRequiredFlag,LetDate,CurrentLandlordId")] PProperty pProperty)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                PropertyService.SavePProperty(pProperty, CurrentUserId); 
                return RedirectToAction("Index");
            }
            var landlords = PropertyService.GetAllValidPropertyLandlords().Select(m => new SelectListItem() { Value = m.PLandlordId.ToString(), Text = m.LandlordFullname + "(" + m.LandlordCode + ")" });
            ViewBag.CurrentLandlordId = new SelectList(landlords, "Value", "Text", pProperty.CurrentLandlordId);

            return View(pProperty);
        }

        // GET: PProperties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PProperty pProperty = PropertyService.GetPropertyById(id.Value);
            
            if (pProperty == null)
            {
                return HttpNotFound();
            }
            return View(pProperty);
        }

        // POST: PProperties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            PropertyService.DeletePropertyById(id);
            return RedirectToAction("Index");
        }
         
    }
}
