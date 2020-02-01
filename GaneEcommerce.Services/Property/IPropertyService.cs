using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;

namespace Ganedata.Core.Services
{
    public interface IPropertyService
    {
        IQueryable<PProperty> GetAllValidProperties(int? filterLandlordId = null, bool includeArchived = false);
        IQueryable<PLandlord> GetAllValidPropertyLandlords();
        IQueryable<PTenant> GetAllPropertyTenants(int? id);
        IEnumerable<PTenant> GetAllCurrentTenants(int filterByPropertyId = 0);
        IEnumerable<PTenant> GetAppointmentRecipientTenants(int filterByOrderId = 0);
        IEnumerable<PTenantViewModelForTenantFlag> GetAllTenantsToUpdateIsCurrentTenant(int filterByPropertyId = 0);
        IEnumerable<PTenant> GetAllEmailRecipientTenantsForOrder(int orderId);
        PTenant GetPropertyTenantById(int pTenantId);
        PProperty GetPropertyById(int pPropertyId);
        PLandlord GetPropertyLandlordById(int pPropertyLandlordId);

        PProperty SavePProperty(PProperty property, int userId);
        PLandlord SavePLandlord(PLandlord landlord, int userId);
        PTenant SavePTenant(PTenant ptenant, int userId);
        void DeletePropertyLandlord(int pLandlordId);
        void DeletePropertyById(int pProrpertyId);

        PTenant CreatePropertyTenant(PTenant tenant, int userId);
        PTenant UpdatePropertyTenant(PTenant tenant, int userId);
        void DeletePropertyTenant(int pTenantId);
        Task UpdateCurrentTenancyFlags();

        Task UpdateAllPropertyTenantsFlags();
    }
}