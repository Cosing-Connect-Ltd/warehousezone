using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public class TooltipServices : ITooltipServices
    {
        private readonly IApplicationContext _currentDbContext;

        public TooltipServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public async Task<IEnumerable<Tooltip>> GetTooltipsByKey(string[] keys, int tenantId)
        {
            if(keys == null || keys.Length == 0)
            {
                return null;
            }

            var tooltips = await _currentDbContext.Tooltips
                                    .Where(t => keys.Contains(t.Key) && t.IsDeleted != true &&
                                            (t.Localization == CultureInfo.CurrentCulture.Name || string.IsNullOrEmpty(t.Localization)) &&
                                            (t.TenantId == tenantId || t.TenantId == 0 || t.TenantId == null))
                                    .GroupBy(t => t.Key)
                                    .Select(g => g.OrderByDescending(t => t.Localization)
                                                  .ThenByDescending(t => t.TenantId)
                                                  .FirstOrDefault())
                                    .ToListAsync();

            return tooltips;
        }

        public Tooltip GetById(int id, int tenantId, bool isSuperUser)
        {
            return _currentDbContext.Tooltips
                .Include(t => t.Tenant)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id && (x.TenantId == tenantId || isSuperUser) && x.IsDeleted != true);
        }


        public IQueryable<TooltipViewModel> GetAll(int tenantId, bool isSuperUser)
        {
            var tooltips = _currentDbContext.Tooltips
                .AsNoTracking().Where(t => (t.TenantId == tenantId || isSuperUser) && t.IsDeleted != true)
                .Select(t => new TooltipViewModel
                {
                    Id = t.Id,
                    Key = t.Key,
                    Title = t.Title,
                    Localization = t.Localization,
                    Description = t.Description,
                    TenantName = t.Tenant.TenantName,
                    TenantId = t.TenantId,
                })
                .OrderBy(x => x.Key);

            return tooltips;
        }

        public Tooltip Save(Tooltip tooltipData, int userId)
        {
            var tooltip = _currentDbContext.Tooltips.FirstOrDefault(t => t.Id == tooltipData.Id);

            if (tooltip == null || tooltipData.Id < 1)
            {

                _currentDbContext.Tooltips.Add(tooltipData);
                tooltipData.UpdateCreatedInfo(userId);
                _currentDbContext.SaveChanges();

            }
            else
            {
                tooltip.Key = tooltipData.Key.Trim();
                tooltip.Title = tooltipData.Title;
                tooltip.Description = tooltipData.Description;
                tooltip.TenantId = tooltipData.TenantId;
                tooltip.UpdatedBy = userId;
                tooltip.Localization = tooltipData.Localization;

                _currentDbContext.SaveChanges();
            }

            return tooltipData;
        }

        public void Delete(int id, int userId)
        {
            var tooltip = _currentDbContext.Tooltips.Find(id);
            tooltip.UpdateUpdatedInfo(userId);
            tooltip.IsDeleted = true;

            _currentDbContext.Tooltips.Attach(tooltip);
            var entry = _currentDbContext.Entry(tooltip);
            entry.Property(e => e.IsDeleted).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;

            _currentDbContext.SaveChanges();
        }
    }
}