﻿using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Compatibility;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class ShiftScheduleService : IShiftScheduleService
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public ShiftScheduleService(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public ShiftSchedule GetShiftSchedule(int employeeId, DateTime date, int tenantId)
        {
            var shiftSchedules = _currentDbContext.ShiftSchedules.Where(s => s.IsCanceled != true &&
                                                                             s.ResourceId == employeeId &&
                                                                             s.TenantId == tenantId &&
                                                                             EntityFunctions.TruncateTime(s.StartTime) <= date.Date).ToList();

            var shiftSchedule = shiftSchedules.FirstOrDefault(s => (s.StartTime.Date == date.Date || (s.StartTime.Date < date.Date && s.EndTime.Date >= date.Date)) && s.Type == 0);

            if (shiftSchedule != null)
            {
                return shiftSchedule;
            }

            var schedule = StaticAppointmentFactory.CreateAppointment(AppointmentType.Pattern);

            var shiftSchedulesRecurrenceInfos = shiftSchedules.Where(s => s.Type == 1 && s.RecurrenceInfo != null)
                                                              .Select(s => {
                                                                            schedule.RecurrenceInfo.FromXml(s.RecurrenceInfo);
                                                                            schedule.Start = s.StartTime;
                                                                            schedule.End = s.EndTime;
                                                                            return new {
                                                                                         ShiftSchedule = s,
                                                                                         OccurrenceCalculator = OccurrenceCalculator.CreateInstance(schedule.RecurrenceInfo),
                                                                                         Schedule = schedule
                                                                            };
            });

            shiftSchedule = shiftSchedulesRecurrenceInfos.Where(s => s.OccurrenceCalculator.FindNextOccurrenceTimeAfter(date.Date, schedule).Date == date.Date).Select(s => s.ShiftSchedule).FirstOrDefault();

            return shiftSchedule;
        }

        [Obsolete]
        public IEnumerable<ShiftScheduleViewModel> GetShiftSchedules(int locationId, DateTime fromDate, DateTime toDate)
        {
            var shiftSchedules = _currentDbContext.ShiftSchedules.Where(s => s.IsCanceled != true &&
                                                                             s.LocationsId == locationId &&
                                                                             EntityFunctions.TruncateTime(s.StartTime) <= toDate.Date).ToList();

            var shiftScheduleViewModels = shiftSchedules.Where(s => s.StartTime.Date >= fromDate.Date && s.Type == 0)
                                                        .Select(s => new ShiftScheduleViewModel
                                                        {
                                                            Date = s.StartTime.Date,
                                                            EmployeeId = s.ResourceId,
                                                            ExpectedHours = s.ExpectedHours,
                                                            Id = s.Id,
                                                            LocationId = s.LocationsId,
                                                            TimeBreaks = s.TimeBreaks,
                                                            StartTime = s.StartTime,
                                                            EndTime = s.EndTime,
                                                            EmployeeName = s.Resources.Name,
                                                            IsFlexibleWorkingAllowed = s.Resources.IsFlexibleWorkingAllowed,
                                                            IsOvertimeWorkingAllowed = s.Resources.IsOvertimeWorkingAllowed,
                                                            AttendanceGracePeriodInMinutes = s.Resources.AttendanceGracePeriodInMinutes
                                                        })
                                                        .ToList();

            var schedule = StaticAppointmentFactory.CreateAppointment(AppointmentType.Pattern);

            var recurrenceShiftSchedules = shiftSchedules.Where(s => s.Type == 1 && s.RecurrenceInfo != null)
                                                              .SelectMany(s => {
                                                                  schedule.RecurrenceInfo.FromXml(s.RecurrenceInfo);
                                                                  schedule.Start = s.StartTime;
                                                                  schedule.End = s.EndTime;
                                                                  var occurrenceCalculator = OccurrenceCalculator.CreateInstance(schedule.RecurrenceInfo);

                                                                  var occurences = occurrenceCalculator.CalcOccurrences(new TimeInterval { Start = fromDate.Date, End = toDate.Date.AddDays(1) }, schedule);

                                                                  return occurences.Select(o => new ShiftScheduleViewModel {
                                                                    EmployeeId = s.ResourceId,
                                                                    ExpectedHours = s.ExpectedHours,
                                                                    Id = s.Id,
                                                                    LocationId = s.LocationsId,
                                                                    TimeBreaks = s.TimeBreaks,
                                                                    Date = o.Start.Date,
                                                                    StartTime = o.Start.Date + s.StartTime.TimeOfDay,
                                                                    EndTime = o.Start.Date + (s.EndTime.Date - s.StartTime.Date) + s.EndTime.TimeOfDay,
                                                                    EmployeeName = s.Resources.Name
                                                                  });
                                                              });

            shiftScheduleViewModels.AddRange(recurrenceShiftSchedules);

            return shiftScheduleViewModels;
        }

        public bool Create(ShiftSchedule shiftSchedule)
        {
            _currentDbContext.ShiftSchedules.Add(shiftSchedule);
            int result = _currentDbContext.SaveChanges();

            return result > 0;

        }

        public bool Update(ShiftSchedule shiftScheduleData)
        {
            var shiftSchedule = _currentDbContext.ShiftSchedules.Find(shiftScheduleData.Id);

            if (shiftSchedule != null)
            {
                shiftSchedule.StartTime = shiftScheduleData.StartTime;
                shiftSchedule.EndTime = shiftScheduleData.EndTime;
                shiftSchedule.Subject = shiftScheduleData.Subject;
                shiftSchedule.Description = shiftScheduleData.Description;
                shiftSchedule.RecurrenceInfo = shiftScheduleData.RecurrenceInfo;
                shiftSchedule.ResourceId = shiftScheduleData.ResourceId;
                shiftSchedule.TimeBreaks = shiftScheduleData.TimeBreaks;
                shiftSchedule.Type = shiftScheduleData.Type;
                _currentDbContext.SaveChanges();
            }

            return true;
        }

        public bool Delete(ShiftSchedule shiftScheduleData)
        {
            var shiftSchedule = _currentDbContext.ShiftSchedules.Find(shiftScheduleData.Id);
            if (shiftSchedule != null)
            {
                shiftSchedule.IsCanceled = true;
                _currentDbContext.Entry(shiftSchedule).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }

            return true;
        }
    }
}