using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Data;

namespace Ganedata.Core.Services 
{
    public class AttLogsServices : IAttLogsServices
    {
        private readonly IApplicationContext _currentDbContext;
        public AttLogsServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public int Insert(AttLogs attLogs)
        {
            _currentDbContext.Entry(attLogs).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return attLogs.Id;
        }
    }
}