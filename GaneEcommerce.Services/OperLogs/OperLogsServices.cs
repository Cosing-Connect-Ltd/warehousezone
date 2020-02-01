using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class OperLogsServices : IOperLogsServices
    {
        private readonly IApplicationContext _currentDbContext;
        //constructor
        public OperLogsServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public void Insert(OperLogs operLogs)
        {
            _currentDbContext.OperLogs.Add(operLogs);
            _currentDbContext.SaveChanges();
        }
    }
}