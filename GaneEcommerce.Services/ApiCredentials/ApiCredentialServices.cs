using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class ApiCredentialServices : IApiCredentialServices
    {
        private readonly IApplicationContext _currentDbContext;

        public ApiCredentialServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public ApiCredentials GetById(int id, int tenantId)
        {
            return _currentDbContext.GlobalApis
                                    .AsNoTracking()
                                    .FirstOrDefault(x => x.Id == id && x.TenantId == tenantId && x.IsDeleted != true);
        }


        public IQueryable<ApiCredentials> GetAll(int tenantId)
        {
            var apiCredentials = _currentDbContext.GlobalApis
                                                    .AsNoTracking()
                                                    .Where(t => t.TenantId == tenantId && t.IsDeleted != true);

            return apiCredentials;
        }

        public ApiCredentials Save(ApiCredentials apiCredentialData, int userId)
        {
            var apiCredential = _currentDbContext.GlobalApis.FirstOrDefault(t => t.Id == apiCredentialData.Id);

            if (apiCredential == null || apiCredentialData.Id < 1)
            {

                _currentDbContext.GlobalApis.Add(apiCredentialData);
                apiCredentialData.UpdateCreatedInfo(userId);
                _currentDbContext.SaveChanges();
            }
            else
            {
                apiCredential.ApiKey = apiCredentialData.ApiKey;
                apiCredential.ApiTypes = apiCredentialData.ApiTypes;
                apiCredential.ApiUrl = apiCredentialData.ApiUrl;
                apiCredential.UserName = apiCredentialData.UserName;
                apiCredential.Password = apiCredentialData.Password;
                apiCredential.AccountNumber = apiCredentialData.AccountNumber;
                apiCredential.SiteID = apiCredentialData.SiteID;
                apiCredential.DefaultWarehouseId = apiCredentialData.DefaultWarehouseId;
                apiCredential.ExpiryDate = apiCredentialData.ExpiryDate;
                _currentDbContext.SaveChanges();
            }

            return apiCredentialData;
        }

        public void Delete(int id, int userId)
        {
            var apiCredential = _currentDbContext.GlobalApis.Find(id);
            apiCredential.UpdateUpdatedInfo(userId);
            apiCredential.IsDeleted = true;

            _currentDbContext.GlobalApis.Attach(apiCredential);
            var entry = _currentDbContext.Entry(apiCredential);
            entry.Property(e => e.IsDeleted).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;

            _currentDbContext.SaveChanges();
        }
    }
}