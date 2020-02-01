using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public interface IOperLogsServices
    {
        void Insert(OperLogs operLogs);
    }
}