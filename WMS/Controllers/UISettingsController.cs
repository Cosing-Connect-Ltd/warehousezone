using AutoMapper;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class UISettingsController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUISettingServices _uiSettingServices;
        private readonly IMapper _mapper;

        public UISettingsController(ICoreOrderService orderService,
                                    IPropertyService propertyService,
                                    IAccountServices accountServices,
                                    ILookupServices lookupServices,
                                    IUISettingServices uiSettingServices,
                                    IMapper mapper,
                                    ITenantWebsiteService tenantWebsiteService)
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

            return PartialView("SettingsBar", uiSettings);
        }


        [HttpGet]
        public ActionResult WebsiteUISettings(int siteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var tenantWebsite = _tenantWebsiteService.GetTenantWebSiteBySiteId(siteId);

            ViewBag.SiteName = tenantWebsite.SiteName;

            var uiSettings = _uiSettingServices.GetWebsiteUISettings(CurrentTenantId, siteId, tenantWebsite.Theme);

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Save(List<UISettingViewModel> uiSettings)
        {
            if (ModelState.IsValid)
            {
                _uiSettingServices.Save(uiSettings, CurrentUserId, CurrentTenantId);
            }
        }

        [HttpGet]
        public JsonResult GetUISettingValues()
        {
            var uiSettings = _uiSettingServices.GetWarehouseUISettings(CurrentTenantId, CurrentTenant.Theme);

            return Json(uiSettings, JsonRequestBehavior.AllowGet);
        }

        public ContentResult AppStyle(string filePath)
        {
            var cssContent = _uiSettingServices.GetWarehouseCustomStylesContent(Server.MapPath(filePath),
                                                                                Request.Browser.Type,
                                                                                Request.Browser.MajorVersion,
                                                                                CurrentTenantId,
                                                                                CurrentTenant.Theme);

            return Content(cssContent, "text/css");
        }
    }
}