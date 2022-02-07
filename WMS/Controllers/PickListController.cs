using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class PickListController : BaseController
    {
        // GET: PickList           
        public PickListController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, InvoiceService invoiceService)
           : base(orderService, propertyService, accountServices, lookupServices)
        {

        }
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        public ActionResult CcpeReport()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        public ActionResult _CcpeReportListPartial(DateTime? startDate, DateTime? endDate)
        {
           
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (!startDate.HasValue && !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-30);
                endDate = DateTime.Now;
            }
            ViewBag.SDate = startDate;
            ViewBag.EDate = endDate;
            var model=CCPECustomBindings.GetDataForCCPE(startDate.Value, endDate.Value);
            return PartialView(model);
        }
    }
}