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

        public Tooltip GetById(int tooltipId)
        {
            return _currentDbContext.Tooltips.AsNoTracking().FirstOrDefault(x => x.TooltipId == tooltipId && x.IsDeleted != true);
        }


        public IQueryable<TooltipViewModel> GetAll(int tenantId)
        {
            var model = _currentDbContext.Tooltips.AsNoTracking().Where(t => (t.TenantId == tenantId || t.TenantId == 0 || t.TenantId == null) && t.IsDeleted != true)
                .Select(t => new TooltipViewModel
                {
                    TooltipId = t.TooltipId,
                    Key = t.Key,
                    Title = t.Title,
                    Localization = t.Localization,
                    Description = t.Description,
                    TenantId = t.TenantId,
                })
                .OrderBy(x => x.Key);

            return model;
        }

        public Tooltip Save(Tooltip tooltip, int userId)
        {
            var account = _currentDbContext.Tooltips.FirstOrDefault(t => t.TooltipId == tooltip.TooltipId);

            if (account == null || tooltip.TooltipId < 1)
            {

                _currentDbContext.Tooltips.Add(tooltip);
                tooltip.UpdateCreatedInfo(userId);
                _currentDbContext.SaveChanges();

            }
            else
            {
                account.Key = tooltip.Key.Trim();
                account.Title = tooltip.Title.Trim();
                account.Description = tooltip.Description.Trim();
                account.TenantId = tooltip.TenantId;
                account.UpdatedBy = userId;
                account.Localization = tooltip.Localization;

                _currentDbContext.SaveChanges();
            }

            return tooltip;
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