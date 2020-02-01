using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class ScanSourceProductController : BaseController
    {
        private DataImportFactory _importFactory;

        public ScanSourceProductController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _importFactory = new DataImportFactory();
        }
        // GET: ScanSourceProduct
        public ActionResult Index()

        {
            DateTime startTime = DateTime.Now;
            bool status = _importFactory.ImportScanSourceProduct(CurrentTenantId, CurrentUserId);
            DateTime endTime = DateTime.Now;
            if (status) { ViewBag.Success = string.Format("Data Import SucessFully - {0} - {1}", startTime, endTime); } else { ViewBag.Warning = "Data not Imported"; }
            return View();
        }
    }
}