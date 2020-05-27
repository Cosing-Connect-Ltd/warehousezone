using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            if (uiSettings == null)
            {
                uiSettings = new List<UISettingViewModel>();
            }

            // Getting new UI setting items to show in page
            var NewUISettingItems = _uiSettingServices.GetWebsiteUISettingItems(CurrentTenantId, CurrentTenantWebsite.Theme)
                                    .Where(k => !uiSettings.Select(s => s.UISettingItem.Id).Contains(k.Id));

            uiSettings.AddRange(NewUISettingItems.Select(k => new UISettingViewModel { UISettingItem = k, SiteId = CurrentTenantWebsite.SiteID })
                                                .ToList());

            uiSettings.Sort((item1, item2) => item1.UISettingItem.DisplayOrder.CompareTo(item2.UISettingItem.DisplayOrder));

            return PartialView("SettingsBar", uiSettings);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Save(List<UISettingViewModel> uiSettings)
        {
            if (ModelState.IsValid)
            {
                await _uiSettingServices.Save(uiSettings, CurrentUserId, CurrentTenantId);
            }
        }

        [HttpGet]
        public JsonResult GetWebsiteUISetting()
        {
            var uiSettings = _uiSettingServices.GetWebsiteUISettings(CurrentTenantId, CurrentTenantWebsite.SiteID, CurrentTenantWebsite.Theme)
                .ToDictionary(t => t.UISettingItem.Key, t => t.Value);

            return Json(uiSettings, JsonRequestBehavior.AllowGet);
        }
    }
}