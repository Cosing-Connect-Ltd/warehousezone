using Ganedata.Core.Services;
using System;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers.TA
{
    public class ShiftsController : BaseController
    {
        public ShiftsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        { }

        public ActionResult Index(string message = "")
        {
            if (!caSession.AuthoriseSession()) return Redirect((string)Session["ErrorUrl"]);

            try
            {
                if (!String.IsNullOrWhiteSpace(message))
                    ViewBag.Message = message;

                return View();
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                return View("_EmptyResult");
            }
        }

        public ActionResult SchedulerPartial()
        {
            return PartialView("_SchedulerPartial", ShiftSchedulerSettings.DataObject);

        }

        public ActionResult ShiftSchedulePartialEdit()
        {
            try
            {
                ShiftSchedulerSettings.UpdateEditableDataObject();
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }

            return PartialView("_SchedulerPartial", ShiftSchedulerSettings.DataObject);
        }
    }
}