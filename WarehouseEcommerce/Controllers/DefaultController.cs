using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseEcommerce.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Shop/Default
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult _SocialMediaAccountsPartial()
        {
            return PartialView();
        }
    }
}