using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        public string GetWarehouseCustomStylesContent(string filePath, string browserType, int browserVersion, int tenantId, WarehouseThemeEnum warehouseTheme)
        {
            var uiSettings = GetWarehouseUISettings(tenantId, warehouseTheme);

            return GetCustomStylesContent(filePath, browserType, browserVersion, uiSettings);
        }

        public string GetWebsiteCustomStylesContent(string filePath, string browserType, int browserVersion, int tenantId, int siteId, WebsiteThemeEnum websiteTheme)
        {
            var uiSettings = GetWebsiteUISettings(tenantId, siteId, websiteTheme);

            return GetCustomStylesContent(filePath, browserType, browserVersion, uiSettings);
        }

        private string GetCustomStylesContent(string filePath, string browserType, int browserVersion, List<UISettingViewModel> uiSettings)
        {
            var cssContent = File.ReadAllText(filePath);

            uiSettings = uiSettings.Where(item => !string.IsNullOrEmpty(item.Value) && !string.IsNullOrWhiteSpace(item.Value))
                                    .Select(item => {
                                        item.Value = string.IsNullOrEmpty(item.Value) || string.IsNullOrWhiteSpace(item.Value) ? item.UISettingItem.DefaultValue : item.Value;
                                        return item;
                                    }).ToList();

            ApplyCSSVariableCustomStyles(browserType, browserVersion, uiSettings.Where(t => string.IsNullOrEmpty(t.UISettingItem.Selector) ).ToList(), ref cssContent);

            ApplySelectorBasedCustomStyles(uiSettings.Where(t => !string.IsNullOrEmpty(t.UISettingItem.Selector)).ToList(), ref cssContent);

            return cssContent;
        }

        private static void ApplyCSSVariableCustomStyles(string browserType, int browserVersion, List<UISettingViewModel> uiSettings, ref string cssContent)
        {
            browserType = browserType.ToUpper();

            var isCSSVariablesSupported = !(browserType.Contains("IE") ||
                                            browserType.Contains("INTERNETEXPLORER") ||
                                            (browserType.Contains("FIREFOX") && browserVersion < 31) ||
                                            (browserType.Contains("CHROME") && browserVersion < 49) ||
                                            (browserType.Contains("SAFARI") && browserVersion < 9) ||
                                            (browserType.Contains("OPERA") && browserVersion < 36));

            foreach (var item in uiSettings)
            {
                if (!isCSSVariablesSupported)
                {
                    cssContent = Regex.Replace(cssContent, @"\b" + $"var\\({item.UISettingItem.Key}\\)" + @"(?!\w)", item.Value, RegexOptions.IgnoreCase);
                    continue;
                }

                cssContent = Regex.Replace(cssContent, @"" + item.UISettingItem.Key + @":(.*);", $"{item.UISettingItem.Key}: {item.Value};", RegexOptions.IgnoreCase);
            }
        }

        private static void ApplySelectorBasedCustomStyles(List<UISettingViewModel> uiSettings, ref string cssContent)
        {
            foreach (var item in uiSettings)
            {
                cssContent += $" {item.UISettingItem.Selector} {{ {item.UISettingItem.Key}: {item.Value}; }}";
            }
        }
    }
}