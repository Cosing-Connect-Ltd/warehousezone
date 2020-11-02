using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Compatibility;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                                                                             s.StartTime.Date <= date.Date).ToList();

            var shiftSchedule = shiftSchedules.FirstOrDefault(s => (s.StartTime == date || (s.StartTime.Date < date && s.EndTime.Date >= date)) && s.Type == 0);

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

            shiftSchedule = shiftSchedulesRecurrenceInfos.Where(s => s.OccurrenceCalculator.FindNextOccurrenceTimeAfter(date, schedule).Date == date.Date).Select(s => s.ShiftSchedule).FirstOrDefault();

            return shiftSchedule;
        }

        public IEnumerable<ShiftScheduleViewModel> GetShiftSchedules(int locationId, DateTime fromDate, DateTime toDate)
        {
            var shiftSchedules = _currentDbContext.ShiftSchedules.Where(s => s.IsCanceled != true &&
                                                                             s.LocationsId == locationId &&
                                                                             s.StartTime.Date <= toDate.Date).ToList();

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
                                                            EmployeeName = s.Resources.Name
                                                        })
                                                        .ToList();

            var schedule = StaticAppointmentFactory.CreateAppointment(AppointmentType.Pattern);

            var recurrenceShiftSchedules = shiftSchedules.Where(s => s.Type == 1 && s.RecurrenceInfo != null)
                                                              .SelectMany(s => {
                                                                  schedule.RecurrenceInfo.FromXml(s.RecurrenceInfo);
                                                                  schedule.Start = s.StartTime;
                                                                  schedule.End = s.EndTime;
                                                                  var occurrenceCalculator = OccurrenceCalculator.CreateInstance(schedule.RecurrenceInfo);

                                                                  var occurences = occurrenceCalculator.CalcOccurrences(new TimeInterval { Start = fromDate, End = toDate }, schedule.RecurrencePattern);

                                                                  return occurences.Select(o => new ShiftScheduleViewModel {
                                                                    EmployeeId = s.ResourceId,
                                                                    ExpectedHours = s.ExpectedHours,
                                                                    Id = s.Id,
                                                                    LocationId = s.LocationsId,
                                                                    TimeBreaks = s.TimeBreaks,
                                                                    Date = o.Start.Date,
                                                                    StartTime = s.StartTime,
                                                                    EndTime = s.EndTime,
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