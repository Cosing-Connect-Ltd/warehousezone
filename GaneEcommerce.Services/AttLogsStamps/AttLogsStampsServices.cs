using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class AttLogsStampsServices : IAttLogsStampsServices
    {
        private readonly IApplicationContext _currentDbContext;

        public AttLogsStampsServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public IQueryable<AttLogsStamps> GetAllAttLogsStamps(int terminalId, TnALogsStampType tnAlogStampType)
        {
            return _currentDbContext.AttLogsStamps.Where(x => x.TerminalId == terminalId && x.TnALogsStampType == tnAlogStampType);
        }

        public void Insert(AttLogsStamps attLogsStamps)
        {
            _currentDbContext.AttLogsStamps.Add(attLogsStamps);
            _currentDbContext.SaveChanges();

        }
    }
}