using AutoMapper;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class UISettingsController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUISettingServices _uiSettingServices;
        private readonly IMapper _mapper;

        public UISettingsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUISettingServices uiSettingServices, IMapper mapper, ITenantWebsiteService tenantWebsiteService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantWebsiteService = tenantWebsiteService;
            _uiSettingServices = uiSettingServices;
            _mapper = mapper;
        }

        public ActionResult SettingsBar()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var uiSettings = _uiSettingServices.GetWarehouseUISettings(CurrentTenantId, CurrentTenant.Theme);

            if (uiSettings == null)
            {
                uiSettings = new List<UISettingViewModel>();
            }
            var NewUISettingItems = _uiSettingServices.GetWarehouseUISettingItems(CurrentTenantId, CurrentTenant.Theme)
                                    .Where(k => !uiSettings.Select(s => s.UISettingItem.Id).Contains(k.Id));

            uiSettings.AddRange(NewUISettingItems.Select(k => new UISettingViewModel { UISettingItem = k }).ToList());

            uiSettings.Sort((item1, item2) => item1.UISettingItem.DisplayOrder.CompareTo(item2.UISettingItem.DisplayOrder));

            return PartialView("SettingsBar", uiSettings);
        }


        [HttpGet]
        public ActionResult WebsiteUISettings(int siteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var tenantWebsite = _tenantWebsiteService.GetTenantWebSiteBySiteId(siteId);

            ViewBag.SiteName = tenantWebsite.SiteName;

            var uiSettings = _uiSettingServices.GetWebsiteUISettings(CurrentTenantId, siteId, tenantWebsite.Theme);

            if (uiSettings == null)
            {
                uiSettings = new List<UISettingViewModel>();
            }
            var NewUISettingItems = _uiSettingServices.GetWebsiteUISettingItems(CurrentTenantId, tenantWebsite.Theme)
                                    .Where(k => !uiSettings.Select(s => s.UISettingItem.Id).Contains(k.Id));

            uiSettings.AddRange(NewUISettingItems.Select(k => new UISettingViewModel { UISettingItem = k, SiteId = siteId }).ToList());

            uiSettings.Sort((item1, item2) => item1.UISettingItem.DisplayOrder.CompareTo(item2.UISettingItem.DisplayOrder));

            return View("WebsiteUISettings", uiSettings);
        }

        [HttpPost, ActionName("WebsiteUISettings")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitWebsiteUISettings(List<UISettingViewModel> uiSettings)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                _uiSettingServices.Save(uiSettings, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", "TenantWebsites");
            }

            return View(uiSettings);
        }

        [HttpGet]
        public JsonResult GetWarehouseUISetting()
        {
            var uiSettings = _uiSettingServices.GetWarehouseUISettings(CurrentTenantId, CurrentTenant.Theme);

            if (uiSettings == null)
            {
                uiSettings = new List<UISettingViewModel>();
            }
            var NewUISettingItems = _uiSettingServices.GetWarehouseUISettingItems(CurrentTenantId, CurrentTenant.Theme)
                                    .Where(k => !uiSettings.Select(s => s.UISettingItem.Id).Contains(k.Id));

            uiSettings.AddRange(NewUISettingItems.Select(k => new UISettingViewModel { UISettingItem = k }).ToList());

            return Json(uiSettings.ToDictionary(t => t.UISettingItem.Key, t => t.Value), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Save(List<UISettingViewModel> uiSettings)
        {
            if (!caSession.AuthoriseSession()) { throw new AuthenticationException(); }

            if (ModelState.IsValid)
            {
                await _uiSettingServices.Save(uiSettings, CurrentUserId, CurrentTenantId);
            }
        }
    }
}