using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public interface IAttLogsStampsServices
    {
        IQueryable<AttLogsStamps> GetAllAttLogsStamps(int terminalId, TnALogsStampType logType);
        void Insert(AttLogsStamps attLogsStamps);
    }
}