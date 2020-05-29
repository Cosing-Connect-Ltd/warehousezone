using Ganedata.Core.Entities.Domain;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IApiCredentialServices
    {
        IQueryable<ApiCredentials> GetAll(int tenantId);
        ApiCredentials GetById(int id, int tenantId);
        void Delete(int id, int userId);
        ApiCredentials Save(ApiCredentials apiCredential, int userId);
    }
}
