using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface ITooltipServices
    {
        IQueryable<TooltipViewModel> GetAll(int tenantId);
        Tooltip GetById(int tooltipId);
        Task<IEnumerable<Tooltip>> GetTooltipsByKey(string[] key, int tenantId);
        void Delete(int id, int userId);
        Tooltip Save(Tooltip tooltip, int userId);
    }
}
