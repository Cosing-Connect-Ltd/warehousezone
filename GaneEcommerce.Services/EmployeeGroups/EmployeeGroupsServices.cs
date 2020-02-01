using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class EmployeeGroupsServices : IEmployeeGroupsServices
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public EmployeeGroupsServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public IEnumerable<EmployeeGroups> GetEmployeeGroupsByEmployeeId(int employeeId)
        {
            return _currentDbContext.EmployeeGroups.Where(s => s.ResourceId == employeeId && s.IsDeleted != true);
        }

        public void DeleteEmployeeGroupsByEmployeeId(int employeeId)
        {
            var groups = GetEmployeeGroupsByEmployeeId(employeeId).ToList();
            foreach (var item in groups)
            {
                _currentDbContext.EmployeeGroups.Remove(item);
                _currentDbContext.SaveChanges();
            }
        }

        public void UpdateEmployeeGroups_ByEmployeeId(int employeeId, int groupsId)
        {
            //do inserts
            EmployeeGroups insert = new EmployeeGroups()
            {
                ResourceId = employeeId,
                GroupsId = groupsId,
            };

            _currentDbContext.EmployeeGroups.Add(insert);
            _currentDbContext.SaveChanges();
        }

        public void Insert(EmployeeGroups employeeGroups)
        {
            _currentDbContext.EmployeeGroups.Add(employeeGroups);
            _currentDbContext.SaveChanges();

        }
    }
}