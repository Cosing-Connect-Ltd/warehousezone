using Ganedata.Core.Models;
using Ganedata.Core.Services;
using LazyCache;
using System.Collections.Generic;
using System.Web.Mvc;
using WarehouseEcommerce.Controllers;

namespace WMS.Controllers
{
    public class UISettingsController : BaseController
    {
        private readonly IUISettingServices _uiSettingServices;
        private readonly IAppCache _cache;

        public UISettingsController(ICoreOrderService orderService,
                                    IPropertyService propertyService,
                                    IAccountServices accountServices,
                                    ILookupServices lookupServices,
                                    IUISettingServices uiSettingServices,
                                    ITenantsCurrencyRateServices tenantsCurrencyRateServices,
                                    ITenantWebsiteService tenantWebsiteService,
                                    IAppCache cache)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices, tenantWebsiteService)
        {
            _uiSettingServices = uiSettingServices;
            _cache = cache;
        }

        public ActionResult SettingsBar()
        {
            var uiSettings = _uiSettingServices.GetWebsiteUISettings(CurrentTenantId, CurrentTenantWebsite.SiteID, CurrentTenantWebsite.Theme);

            return PartialView("SettingsBar", uiSettings);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Save(List<UISettingViewModel> uiSettings)
        {
            if (ModelState.IsValid)
            {
                _uiSettingServices.Save(uiSettings, CurrentUserId, CurrentTenantId, CurrentTenantWebsite.SiteID);

                ClearStyleCache(CurrentTenantWebsite.SiteID);
            }
        }

        [HttpGet]
        public JsonResult GetUISettingValues()
        {
            var uiSettings = _uiSettingServices.GetWebsiteUISettings(CurrentTenantId, CurrentTenantWebsite.SiteID, CurrentTenantWebsite.Theme);

            return Json(uiSettings, JsonRequestBehavior.AllowGet);
        }

        public ContentResult AppStyle(string filePath)
        {
            var cssContent = _uiSettingServices.GetWebsiteCustomStylesContent(Server.MapPath(filePath),
                                                                                Request.Browser.Type,
                                                                                Request.Browser.MajorVersion,
                                                                                CurrentTenantId,
                                                                                CurrentTenantWebsite.SiteID,
                                                                                CurrentTenantWebsite.Theme);

            return Content(cssContent, "text/css");
        }

        public void ClearStyleCache(int? siteId)
        {
            _cache.Remove(siteId + "-ui-settings-variable");
            _cache.Remove(siteId + "-ui-settings-fixed");
        }
    }
}