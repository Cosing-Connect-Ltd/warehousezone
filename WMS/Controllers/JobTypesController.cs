using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Data;
using WMS.Helpers;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class JobTypesController : BaseController
    {
        private readonly IEmployeeServices _employeeServices;

        public JobTypesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IEmployeeServices employeeServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _employeeServices = employeeServices;
        }
        // GET: JobTypes
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }


        // GET: JobTypes/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var resources = (from res in _employeeServices.GetAllActiveAppointmentResourceses(CurrentTenantId)
                             select new
                             {
                                 res.ResourceId,
                                 res.FirstName
                             }).ToList();
            ViewBag.Resources = new MultiSelectList(resources, "ResourceId", "FirstName");
            return View();
        }

        // POST: JobTypes/Create
        [HttpPost]
        public ActionResult Create(JobType model, List<int> resourceIds)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LookupServices.CreateJobType(model, CurrentTenantId, CurrentUserId, resourceIds);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Some error occured");
                return View(model);
            }
        }

        // GET: JobTypes/Edit/5
        public ActionResult Edit(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = LookupServices.GetJobTypeById(id, CurrentTenantId);

            SetUpdatedJobTypeViewBagItems(model);

            return View(model);
        }

        // POST: JobTypes/Edit/5
        [HttpPost]
        public ActionResult Edit(JobType model, List<int> ResourceIds)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    LookupServices.SaveJobType(model, CurrentTenantId, CurrentUserId, ResourceIds);
                    return RedirectToAction("Index");
                }
                else
                {
                    SetUpdatedJobTypeViewBagItems(model);
                    return View();
                }

            }
            catch (Exception exp)
            {
                SetUpdatedJobTypeViewBagItems(model);
                ModelState.AddModelError("", exp.Message);
                return View();
            }
        }

        // GET: JobTypes/Delete/5
        public ActionResult Delete(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View(LookupServices.GetJobTypeById(id, CurrentTenantId));
        }

        // POST: JobTypes/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                LookupServices.DeleteJobType(id, CurrentTenantId);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message.ToString();
                return View();
            }
        }
        public ActionResult _JobTypeList()
        {
            var data = (from jTypes in LookupServices.GetAllJobTypes(CurrentTenantId)
                        select new
                        {
                            Resources = jTypes.AppointmentResources.Select(a => a.Name),
                            jTypes.Name,
                            jTypes.Description,
                            jTypes.JobTypeId
                        }).ToList();

            return PartialView(data);

        }

        private void SetUpdatedJobTypeViewBagItems(JobType model)
        {
            var resources = (from res in _employeeServices.GetAllActiveAppointmentResourceses(CurrentTenantId)
                             select new
                             {
                                 res.ResourceId,
                                 res.FirstName
                             }).ToList();
            ViewBag.Resources = new MultiSelectList(resources, "ResourceId", "FirstName", model.AppointmentResources.Select(a => a.ResourceId));
        }
    }
}
