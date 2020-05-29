using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System.Collections.Generic;

namespace Ganedata.Core.Services
{
    public interface IUISettingServices
    {
        List<UISettingViewModel> GetWebsiteUISettings(int tenantId, int siteId, WebsiteThemeEnum websiteTheme);
        List<UISettingViewModel> GetWarehouseUISettings(int tenantId, WarehouseThemeEnum warehouseTheme);
        List<UISettingItemViewModel> GetWebsiteUISettingItems(int tenantId, WebsiteThemeEnum websiteTheme);
        List<UISettingItemViewModel> GetWarehouseUISettingItems(int tenantId, WarehouseThemeEnum warehouseTheme);
        List<UISettingViewModel> Save(List<UISettingViewModel> uiSettingData, int userId, int tenantId);
    }
}
