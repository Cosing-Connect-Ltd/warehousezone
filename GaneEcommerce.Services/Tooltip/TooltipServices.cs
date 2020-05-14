using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
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

        public async Task<IEnumerable<Tooltip>> GetTooltipsDetailByKey(string[] keys, int tenantId)
        {
            var tooltips = await _currentDbContext.Tooltips
                                    .Where(t => keys.Contains(t.Key) &&
                                            (t.Localization == CultureInfo.CurrentCulture.Name || string.IsNullOrEmpty(t.Localization)) &&
                                            (t.TenantId == tenantId || t.TenantId == 0 || t.TenantId == null))
                                    .GroupBy(t => t.Key)
                                    .Select(g => g.OrderByDescending(t => t.Localization)
                                                  .ThenByDescending(t => t.TenantId)
                                                  .FirstOrDefault())
                                    .ToListAsync();

            return tooltips;

        }
    }
}