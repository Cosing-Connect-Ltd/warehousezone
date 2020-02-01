using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}