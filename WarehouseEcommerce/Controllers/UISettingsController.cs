﻿using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WarehouseEcommerce.Controllers;

namespace WMS.Controllers
{
    public class UISettingsController : BaseController
    {
        private readonly IUISettingServices _uiSettingServices;

        public UISettingsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUISettingServices uiSettingServices, ITenantsCurrencyRateServices tenantsCurrencyRateServices)
            : base(orderService, propertyService, accountServices, lookupServices, tenantsCurrencyRateServices)
        {
            _uiSettingServices = uiSettingServices;
        }

        [HttpGet]
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
            }
        }

        [HttpGet]
        public JsonResult GetUISettingValues()
        {
            var uiSettings = _uiSettingServices.GetWebsiteUISettings(CurrentTenantId, CurrentTenantWebsite.SiteID, CurrentTenantWebsite.Theme);

            return Json(uiSettings.ToDictionary(t => t.UISettingItem.Key,
                                                     t => new
                                                     {
                                                         t.UISettingItem.DefaultValue,
                                                         t.Value
                                                     }), JsonRequestBehavior.AllowGet);
        }

        public ContentResult AppStyle(string filePath)
        {
            var cssContent = _uiSettingServices.GetWebsiteCustomStylesContent(Server.MapPath(filePath), Request.Browser, CurrentTenantId, CurrentTenantWebsite.SiteID, CurrentTenantWebsite.Theme);

            return Content(cssContent, "text/css");
        }
    }
}