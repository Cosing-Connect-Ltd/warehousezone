using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface ITooltipServices
    {
        Task<IEnumerable<Tooltip>> GetTooltipsDetailByKey(string[] key, int tenantId);
    }
}
