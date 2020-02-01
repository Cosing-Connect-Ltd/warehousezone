using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ProductAccountsController : BaseController
    {
        public ProductAccountsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices) : base(orderService, propertyService, accountServices, lookupServices)
        {
        }
        // GET: ProductAccounts
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult IsCodeAvailable(int AccountID, string ProdAccCode, int ProdAccCodeID = 0)
       {

            caUser user = caCurrent.CurrentUser();
            if (ProdAccCodeID == 0)
            {
                var find = AccountServices.GetAllProductAccountCodesByAccount(AccountID).Count(a => a.ProdAccCode == ProdAccCode);
                if (find == 0)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }

            else
            {
                var find = AccountServices.GetAllProductAccountCodesByAccount(AccountID).Count(a => a.ProdAccCode == ProdAccCode && a.ProdAccCodeID != ProdAccCodeID);
                if (find == 0)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);

            }


        }
    }
}