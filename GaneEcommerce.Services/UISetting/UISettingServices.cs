using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace Ganedata.Core.Services
{
    public class UISettingServices : IUISettingServices
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;

        public UISettingServices(IApplicationContext currentDbContext, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _mapper = mapper;
        }

        public List<UISettingViewModel> GetWebsiteUISettings(int tenantId, int siteId, WebsiteThemeEnum websiteTheme)
        {
            var uiSettings = GetUISettingsQuery(tenantId)
                            .Where(s => s.SiteId == siteId && s.UISettingItem.WebsiteThemeId == (int)websiteTheme)
                            .ToList();

            var missingUISettingItems = GetUISettingItemsQuery(tenantId)
                                        .Where(k => k.WebsiteThemeId == (int)websiteTheme)
                                        .ToList();

            uiSettings = AddMissingUISettingItems(uiSettings, missingUISettingItems, siteId);

            uiSettings.Sort((item1, item2) => item1.UISettingItem.DisplayOrder.CompareTo(item2.UISettingItem.DisplayOrder));

            return _mapper.Map<List<UISettingViewModel>>(uiSettings);
        }

        public List<UISettingViewModel> GetWarehouseUISettings(int tenantId, WarehouseThemeEnum warehouseTheme)
        {
            var uiSettings = GetUISettingsQuery(tenantId)
                            .Where(s => s.UISettingItem.WarehouseThemeId == (int)warehouseTheme)
                            .ToList();

            var missingUISettingItems = GetUISettingItemsQuery(tenantId)
                                        .Where(i => i.WarehouseThemeId == (int)warehouseTheme)
                                        .ToList();

            uiSettings = AddMissingUISettingItems(uiSettings, missingUISettingItems, null);

            return _mapper.Map<List<UISettingViewModel>>(uiSettings);
        }

        private List<UISetting> AddMissingUISettingItems(List<UISetting> uiSettings, IEnumerable<UISettingItem> newUISettingItems, int? siteId)
        {
            if (uiSettings == null)
            {
                uiSettings = new List<UISetting>();
            }

            uiSettings.AddRange(newUISettingItems.Where(i => !uiSettings.Select(s => s.UISettingItem.Id).Contains(i.Id))
                                                 .Select(k =>
                                                        new UISetting
                                                        {
                                                            UISettingItem = k,
                                                            Value = k.DefaultValue,
                                                            SiteId = siteId
                                                        })
                                                 .ToList());

            uiSettings.Sort((item1, item2) => item1.UISettingItem.DisplayOrder.CompareTo(item2.UISettingItem.DisplayOrder));

            return uiSettings;
        }

        private IQueryable<UISetting> GetUISettingsQuery(int tenantId)
        {
            return _currentDbContext.UISettings
                                    .Include(u => u.UISettingItem)
                                    .AsNoTracking()
                                    .Where(s => s.TenantId == tenantId &&
                                                s.IsDeleted != true &&
                                                s.UISettingItem.IsDeleted != true);
        }

        public List<UISettingItemViewModel> GetWarehouseUISettingItems(int tenantId, WarehouseThemeEnum warehouseTheme)
        {
            var uisettingItems = GetUISettingItemsQuery(tenantId)
                                .Where(i => i.WarehouseThemeId == (int)warehouseTheme);

            return _mapper.Map<List<UISettingItemViewModel>>(uisettingItems);
        }

        public List<UISettingItemViewModel> GetWebsiteUISettingItems(int tenantId, WebsiteThemeEnum websiteTheme)
        {
            var uisettingItems = GetUISettingItemsQuery(tenantId)
                                .Where(i => i.WebsiteThemeId == (int)websiteTheme);

            return _mapper.Map<List<UISettingItemViewModel>>(uisettingItems);
        }

        private IQueryable<UISettingItem> GetUISettingItemsQuery(int tenantId)
        {
            return _currentDbContext.UISettingItems
                        .AsNoTracking()
                        .Where(s => s.IsDeleted != true && s.TenantId == tenantId);
        }

        public void Save(List<UISettingViewModel> uiSettingData, int userId, int tenantId, int? siteId = null)
        {
            if (siteId == null)
            {
                siteId = uiSettingData.FirstOrDefault()?.SiteId;
            }

            var oldUISettings = _currentDbContext.UISettings.Where(t => t.TenantId == tenantId && t.SiteId == siteId && t.IsDeleted != true).ToList();

            uiSettingData.ForEach(dataItem =>
            {

                var oldUISetting = oldUISettings.FirstOrDefault(t => t.UISettingItemId == dataItem.UISettingItem.Id);
                if (oldUISetting?.Value == dataItem.Value)
                {
                    return;
                }

                if (oldUISetting != null && oldUISetting.Value != dataItem.Value)
                {
                    oldUISetting.IsDeleted = true;
                    oldUISetting.UpdateUpdatedInfo(userId);
                }

                if (dataItem.Value != dataItem.UISettingItem.DefaultValue && oldUISetting?.Value != dataItem.Value)
                {
                    var uiSetting = new UISetting
                    {
                        SiteId = dataItem.SiteId,
                        TenantId = tenantId,
                        UISettingItemId = dataItem.UISettingItem.Id,
                        Value = dataItem.Value
                    };

                    uiSetting.UpdateCreatedInfo(userId);

                    _currentDbContext.UISettings.Add(uiSetting);
                }

            });

            _currentDbContext.SaveChanges();
        }
        public string GetWarehouseCustomStylesContent(string filePath, HttpBrowserCapabilitiesBase browser, int tenantId, WarehouseThemeEnum warehouseTheme)
        {
            var uiSettings = GetWarehouseUISettings(tenantId, warehouseTheme);

            return GetCustomStylesContent(filePath, browser, uiSettings);
        }

        public string GetWebsiteCustomStylesContent(string filePath, HttpBrowserCapabilitiesBase browser, int tenantId, int siteId, WebsiteThemeEnum websiteTheme)
        {
            var uiSettings = GetWebsiteUISettings(tenantId, siteId, websiteTheme);

            return GetCustomStylesContent(filePath, browser, uiSettings);
        }

        private string GetCustomStylesContent(string filePath, HttpBrowserCapabilitiesBase browser, List<UISettingViewModel> uiSettings)
        {

            var appCSS = System.IO.File.ReadAllText(filePath);

            var isCSSVariablesSupported = !(browser.Type.ToUpper().Contains("IE") ||
                                            browser.Type.ToUpper().Contains("INTERNETEXPLORER") ||
                                            (browser.Type.ToUpper().Contains("FIREFOX") && browser.MajorVersion < 31) ||
                                            (browser.Type.ToUpper().Contains("CHROME") && browser.MajorVersion < 49) ||
                                            (browser.Type.ToUpper().Contains("SAFARI") && browser.MajorVersion < 9) ||
                                            (browser.Type.ToUpper().Contains("OPERA") && browser.MajorVersion < 36));

            foreach (var item in uiSettings)
            {
                var itemValue = string.IsNullOrEmpty(item.Value) ? item.UISettingItem.DefaultValue : item.Value;
                var itemKey = item.UISettingItem.Key;

                if (!isCSSVariablesSupported)
                {
                    appCSS = ReplaceString(appCSS, $"var({itemKey})", itemValue, StringComparison.OrdinalIgnoreCase);
                }

                appCSS = ReplaceString(appCSS, $"{itemKey}: {item.UISettingItem.DefaultValue};", $"{itemKey}: {itemValue};", StringComparison.OrdinalIgnoreCase);
                appCSS = ReplaceString(appCSS, $"{itemKey}: ;", $"{itemKey}: {itemValue};", StringComparison.OrdinalIgnoreCase);
            }

            return appCSS;
        }

        private static string ReplaceString(string originalString, string oldValue, string newValue, StringComparison comparison)
        {
            var sb = new StringBuilder();

            int previousIndex = 0;
            int index = originalString.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(originalString.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = originalString.IndexOf(oldValue, index, comparison);
            }
            sb.Append(originalString.Substring(previousIndex));

            return sb.ToString();
        }
    }
}