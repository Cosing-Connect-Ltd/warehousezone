using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System.Linq;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class AccountSectorsController : BaseController
    {
        private readonly IAccountSectorService _accountSectorService;
        public AccountSectorsController(ICoreOrderService orderService,
                                        IPropertyService propertyService,
                                        IAccountServices accountServices,
                                        ILookupServices lookupServices,
                                        IAccountSectorService accountSectorService) :
            base(orderService, propertyService, accountServices, lookupServices)
        {
            _accountSectorService = accountSectorService;
        }
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public PartialViewResult _AccountSectors()
        {
            var model = _accountSectorService.GetAll();
            return  PartialView(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        [HttpPost]
        public ActionResult Create(AccountSector accountSector)
        {
            _accountSectorService.Save(accountSector.Name);
            return View("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var model = _accountSectorService.GetAll().FirstOrDefault(u => u.Id == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AccountSector accountSector)
        {
            _accountSectorService.Save(accountSector.Name, accountSector.Id);
            return View("Index");

        }
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            _accountSectorService.Delete((id ?? 0));
            return RedirectToAction("Index");
        }
    }
}