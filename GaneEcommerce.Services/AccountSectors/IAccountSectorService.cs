using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;

namespace Ganedata.Core.Services
{
    public interface IAccountSectorService
    {
        AccountSector Save(string name, int? id = null);
        bool Delete(int? id = null);
        IEnumerable<AccountSector> GetAll();
    }
}
