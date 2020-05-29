using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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

            return _mapper.Map<List<UISettingViewModel>>(uiSettings);
        }

        public List<UISettingViewModel> GetWarehouseUISettings(int tenantId, WarehouseThemeEnum warehouseTheme)
        {
            var uiSettings = GetUISettingsQuery(tenantId)
                            .Where(s => s.UISettingItem.WarehouseThemeId == (int)warehouseTheme)
                            .ToList();

            return _mapper.Map<List<UISettingViewModel>>(uiSettings);
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
            var uisettingItems = GetUISettingItems(tenantId)
                                .Where(i => i.WarehouseThemeId == (int)warehouseTheme);

            return _mapper.Map<List<UISettingItemViewModel>>(uisettingItems);
        }

        public List<UISettingItemViewModel> GetWebsiteUISettingItems(int tenantId, WebsiteThemeEnum websiteTheme)
        {
            var uisettingItems = GetUISettingItems(tenantId)
                                .Where(i => i.WebsiteThemeId == (int)websiteTheme);

            return _mapper.Map<List<UISettingItemViewModel>>(uisettingItems);
        }

        private IQueryable<UISettingItem> GetUISettingItems(int tenantId)
        {
            return _currentDbContext.UISettingItems
                        .AsNoTracking()
                        .Where(s => s.IsDeleted != true && s.TenantId == tenantId);
        }

        public List<UISettingViewModel> Save(List<UISettingViewModel> uiSettingData, int userId, int tenantId)
        {
            uiSettingData.ForEach( s => {

                if (s.Id != 0)
                {
                    var previousUISetting = _currentDbContext.UISettings.FirstOrDefault(t => t.Id == s.Id && t.Value != s.Value);
                    if (previousUISetting == null)
                    {

                        return;
                    }

                    previousUISetting.IsDeleted = true;
                    previousUISetting.UpdateUpdatedInfo(userId);
                    s.Id = 0;
                }

                var uiSetting = new UISetting
                                    {
                                        SiteId = s.SiteId,
                                        TenantId = tenantId,
                                        UISettingItemId = s.UISettingItem.Id,
                                        Value = s.Value
                                    };

                uiSetting.UpdateCreatedInfo(userId);

                _currentDbContext.UISettings.Add(uiSetting);

                _currentDbContext.SaveChanges();

                s.Id = uiSetting.Id;

            });

            return uiSettingData;
        }
    }
}