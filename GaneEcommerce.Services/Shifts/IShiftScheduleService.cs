using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;

namespace Ganedata.Core.Services
{
    public interface IShiftScheduleService
    {
        ShiftSchedule GetShiftSchedule(int employeeId, DateTime date, int tenantId);
        IEnumerable<ShiftScheduleViewModel> GetShiftSchedules(int locationId, DateTime fromDate, DateTime toDate);
        bool Create(ShiftSchedule shiftSchedule);
        bool Update(ShiftSchedule shiftSchedule);
        bool Delete(ShiftSchedule shiftSchedule);
    }
}