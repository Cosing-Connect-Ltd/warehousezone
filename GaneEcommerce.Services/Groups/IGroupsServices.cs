using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IGroupsServices
    {
        IEnumerable<Groups> GetAllGroups(int tenantId);
        IEnumerable<Groups> SearchGroupsByName(string groupsName, int tenantId);
        Groups GetByGroupsId(int groupsId);
        int Insert(Groups groups, int userId);
        void Update(Groups groups, int userId);
        void Delete(Groups groups, int userId);
        int GetResourceGroupIds(int resourceId);
    }
}
