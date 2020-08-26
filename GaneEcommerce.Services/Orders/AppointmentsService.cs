using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public class AppointmentsService : IAppointmentsService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IOrderService _orderService;

        public AppointmentsService(IApplicationContext currentDbContext, IOrderService orderService)
        {
            _currentDbContext = currentDbContext;
            _orderService = orderService;
        }
        public IEnumerable<Appointments> GetAllAppointments(DateTime? filterByDate = null, int resourceId = 0, bool includeCancelled = false)
        {
            return _currentDbContext.Appointments.Where(m => (includeCancelled || !m.IsCanceled) && (filterByDate == null || DbFunctions.TruncateTime((DateTime?)m.StartTime) == DbFunctions.TruncateTime(filterByDate)) && (resourceId == 0 || m.ResourceId == resourceId))
                .OrderBy(m => m.Orders.PPropertyId).ThenBy(m => m.StartTime);
        }
        public IEnumerable<Resources> GetAllResources(int TenantId, DateTime? filterByDate = null)
        {
            var resources = _currentDbContext.Resources.Where(a => a.IsDeleted != true && a.TenantId == TenantId).ToList();
            if (filterByDate.HasValue)
            {
                var appointments = GetAllAppointments(filterByDate).Select(m => m.ResourceId);
                resources.RemoveAll(m => !appointments.Contains(m.ResourceId));
            }
            return resources.OrderBy(m => m.Name);
        }

        public Appointments GetAppointmentById(int appointmentId)
        {
            return _currentDbContext.Appointments.Find(appointmentId);
        }

        public Resources GetAppointmentResourceById(int appointmentResourceId)
        {
            return _currentDbContext.Resources.Find(appointmentResourceId);
        }

        public Appointments GetMostRecentAppointmentForOrder(int orderId)
        {
            return _currentDbContext.Appointments.Where(x => x.OrderId == orderId).OrderByDescending(m => m.StartTime)
                .FirstOrDefault();
        }

        public Appointments CreateAppointment(string start, string end, string subject, string resourceId, int orderId, int joblabel, int tenantId)
        {
            var order = _orderService.GetOrderById(orderId);
            var resIds = $"<ResourceIds>\r\n<ResourceId Type = \"System.Int32\" Value = \"{resourceId}\" />\r\n</ResourceIds>";
            var newAppt = new Appointments()
            {
                StartTime = ParseDate(start),
                EndTime = ParseDate(end),
                Subject = order.OrderNumber + " : " + Math.Round(TimeSpan.FromMinutes(Convert.ToInt64(order.ExpectedHours)).TotalHours, 2) + "Hours, " + (order.SLAPriority != null ? order.SLAPriority.Priority + ": " : ": ") +
                order.PProperties.AddressLine1 + ">" + order.JobType.Name + "-" + order?.JobSubType?.Name,
                Description = string.Join("\n", order.OrderNotes.Select(x => x.Notes + "(" + (x.DateUpdated ?? x.DateCreated).ToString("dd/MM/yyyy HH:mm") + "-" + _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserId == x.CreatedBy)?.DisplayName + ")")),
                ResourceId = Convert.ToInt32(resourceId),
                ResourceIDs = resIds,
                OrderId = orderId,
                Label = joblabel,
                TenentId = tenantId
            };
            _currentDbContext.Appointments.Add(newAppt);
            _currentDbContext.SaveChanges();
            return newAppt;
        }

        public void UpdateAllAppointmentSubjects()
        {
            var appts = _currentDbContext.Appointments.Where(m => string.IsNullOrEmpty(m.Description));
            foreach (var item in appts)
            {
                var subjectTxts = item.Subject.Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                subjectTxts.RemoveAll(m => m.Trim().Length < 1);

                var subjects = subjectTxts.Take(subjectTxts.Count - 1);
                var notes = subjectTxts[subjectTxts.Count - 1];
                item.Subject = string.Join(" ", subjects);
                item.Description = notes;
                _currentDbContext.Entry(item).State = EntityState.Modified;
            }
            _currentDbContext.SaveChanges();
        }

        private DateTime ParseDate(string utcDateString)
        {
            DateTime utcDate = new DateTime(1970, 1, 1);
            utcDate = utcDate.AddMilliseconds(System.Convert.ToDouble(utcDateString));
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDate, DateTimeKind.Utc), TimeZoneInfo.Local);
        }

        public List<SLAPriorit> GetSlaPriorities(int tenantId)
        {
            return _currentDbContext.SLAPriorities.Where(x => x.TenantId == tenantId).ToList();
        }

        public IQueryable<Appointments> GetAllActiveAppointments(int tenantId)
        {
            return _currentDbContext.Appointments.AsNoTracking().Where(x => x.IsCanceled != true && x.Orders.OrderStatusID != OrderStatusEnum.Complete);
        }

        public bool CreateAppointment(Appointments appointment)
        {
            bool success = false;

            _currentDbContext.Appointments.Add(appointment);
            int res = _currentDbContext.SaveChanges();

            if (res > 0) success = true;

            return success;

        }

        public bool UpdateAppointment(Appointments appointment)
        {
            bool success = false;

            var origAppointment = _currentDbContext.Appointments.Find(appointment.AppointmentId);

            _currentDbContext.Entry(origAppointment);

            if (origAppointment != null)
            {
                origAppointment.AllDay = appointment.AllDay;
                origAppointment.StartTime = appointment.StartTime;
                origAppointment.EndTime = appointment.EndTime;
                origAppointment.Subject = appointment.Subject;
                origAppointment.Description = appointment.Description;
                origAppointment.Location = appointment.Location;
                origAppointment.EventType = appointment.EventType;
                origAppointment.RecurrenceInfo = appointment.RecurrenceInfo;
                origAppointment.ReminderInfo = appointment.ReminderInfo;
                origAppointment.Label = appointment.Label;
                origAppointment.Status = appointment.Status;
                origAppointment.ResourceIDs = appointment.ResourceIDs;
                origAppointment.ResourceId = appointment.ResourceId;

                if (appointment.ResourceId != null)
                {
                    origAppointment.ResourceId = appointment.ResourceId;
                }

                _currentDbContext.Appointments.Attach(origAppointment);
                var entry = _currentDbContext.Entry<Appointments>(origAppointment);
                entry.Property(e => e.AllDay).IsModified = true;
                entry.Property(e => e.ResourceIDs).IsModified = true;
                entry.Property(e => e.StartTime).IsModified = true;
                entry.Property(e => e.EndTime).IsModified = true;
                entry.Property(e => e.Subject).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.Location).IsModified = true;
                entry.Property(e => e.EventType).IsModified = true;
                entry.Property(e => e.RecurrenceInfo).IsModified = true;
                entry.Property(e => e.ReminderInfo).IsModified = true;
                entry.Property(e => e.Label).IsModified = true;
                entry.Property(e => e.Status).IsModified = true;

                if (appointment.ResourceId != null)
                {
                    entry.Property(e => e.ResourceId).IsModified = true;
                }

            }

            int res = _currentDbContext.SaveChanges();

            if (res > 0) success = true;

            return success;


        }

        public bool DeleteAppointment(Appointments appointment)
        {
            bool success = false;

            var delAppointment = _currentDbContext.Appointments.Find(appointment.AppointmentId);
            if (delAppointment != null)

                delAppointment.IsCanceled = true;
            _currentDbContext.Entry(delAppointment).State = EntityState.Modified;
            int res = _currentDbContext.SaveChanges();

            if (res > 0) success = true;

            return success;
        }

        public bool CancelNotificationQueuesforAppointment(int appointmentId)
        {
            bool success = false;

            var notificationsInQueue = _currentDbContext.TenantEmailNotificationQueues.Where(m => m.AppointmentId == appointmentId && !m.ActualProcessingTime.HasValue);
            foreach (var item in notificationsInQueue)
            {
                item.IsNotificationCancelled = true;
                _currentDbContext.Entry(item).State = EntityState.Modified;
            }

            int res = _currentDbContext.SaveChanges();

            if (res > 0) success = true;

            return success;
        }

        //------------------- orders Schedule--------------------------------
        public IEnumerable<OrderSchedule> GetAllOrderSchedule(DateTime? filterByDate = null, int resourceId = 0, bool includeCancelled = false)
        {
            return _currentDbContext.OrderSchedule.Where(m => m.IsCanceled != true && (filterByDate == null || DbFunctions.TruncateTime((DateTime?)m.StartTime) == DbFunctions.TruncateTime(filterByDate)) && (resourceId == 0 || m.MarketVehicleId == resourceId))
               .OrderBy(m => m.PalletDispatchId).ThenBy(m => m.StartTime);
        }
        public IQueryable<OrderSchedule> GetAllActiveOrdersAppointments(int tenantId)
        {
            return _currentDbContext.OrderSchedule.AsNoTracking().Where(x => x.IsCanceled != true);
        }
        public IEnumerable<MarketVehicle> GetAllMarketVehicle(int TenantId, DateTime? filterByDate = null)
        {
            var resources = _currentDbContext.MarketVehicles.Where(a => a.IsDeleted != true && a.TenantId == TenantId).ToList();
            if (filterByDate.HasValue)
            {
                var appointments = GetAllOrderSchedule(filterByDate).Select(m => m.MarketVehicleId);
                resources.RemoveAll(m => !appointments.Contains(m.Id));
            }
            return resources.OrderBy(m => m.Name);

        }
        public OrderSchedule GetOrderScheduleId(int Id)
        {
            return _currentDbContext.OrderSchedule.Find(Id);
        }
        public MarketVehicle GetMarketVehicleById(int VechicleId)
        {
            return _currentDbContext.MarketVehicles.Find(VechicleId);
        }

        public OrderSchedule CreateOrderScheduleAppointment(string start, string end, string subject, string resourceId, int joblabel, int tenantId, int palletDispatchId)
        {
            var palletDispatch = _currentDbContext.PalletsDispatches.FirstOrDefault(u => u.PalletsDispatchID == palletDispatchId);
            var resIds = $"<ResourceIds>\r\n<ResourceId Type = \"System.Int32\" Value = \"{resourceId}\" />\r\n</ResourceIds>";
            var newAppt = new OrderSchedule()
            {
                StartTime = ParseDate(start),
                EndTime = ParseDate(end),
                Subject = palletDispatch.DispatchReference + ":" + palletDispatch?.OrderProcess?.Order?.OrderNumber + ":" + palletDispatch?.OrderProcess?.Order?.Account?.CompanyName,
                Description = string.Join("\n", _currentDbContext.AuthUsers.FirstOrDefault(m => m.TenantId == tenantId)?.DisplayName),
                MarketVehicleId = Convert.ToInt32(resourceId),
                ResourceIDs = resIds,
                PalletDispatchId = palletDispatchId,
                Label = joblabel,
                TenentId = tenantId
            };
            _currentDbContext.OrderSchedule.Add(newAppt);
            _currentDbContext.SaveChanges();
            return newAppt;
        }

        public bool CreateOrderScheduleAppointment(OrderSchedule appointment)
        {
            bool success = false;

            _currentDbContext.OrderSchedule.Add(appointment);
            int res = _currentDbContext.SaveChanges();

            if (res > 0) success = true;

            return success;
        }
        public bool UpdateOrderScheduleAppointment(OrderSchedule appointment)
        {
            bool success = false;

            var origAppointment = _currentDbContext.OrderSchedule.Find(appointment.Id);

            _currentDbContext.Entry(origAppointment);

            if (origAppointment != null)
            {
                origAppointment.AllDay = appointment.AllDay;
                origAppointment.StartTime = appointment.StartTime;
                origAppointment.EndTime = appointment.EndTime;
                origAppointment.Subject = appointment.Subject;
                origAppointment.Description = appointment.Description;
                origAppointment.Location = appointment.Location;
                origAppointment.EventType = appointment.EventType;
                origAppointment.RecurrenceInfo = appointment.RecurrenceInfo;
                origAppointment.ReminderInfo = appointment.ReminderInfo;
                origAppointment.Label = appointment.Label;
                origAppointment.Status = appointment.Status;
                origAppointment.ResourceIDs = appointment.ResourceIDs;

                if (appointment.ResourceIDs != null)
                {
                    origAppointment.MarketVehicleId = appointment.MarketVehicleId;
                }


                _currentDbContext.OrderSchedule.Attach(origAppointment);
                var entry = _currentDbContext.Entry<OrderSchedule>(origAppointment);
                entry.Property(e => e.AllDay).IsModified = true;
                entry.Property(e => e.ResourceIDs).IsModified = true;
                entry.Property(e => e.StartTime).IsModified = true;
                entry.Property(e => e.EndTime).IsModified = true;
                entry.Property(e => e.Subject).IsModified = true;
                entry.Property(e => e.Description).IsModified = true;
                entry.Property(e => e.Location).IsModified = true;
                entry.Property(e => e.EventType).IsModified = true;
                entry.Property(e => e.RecurrenceInfo).IsModified = true;
                entry.Property(e => e.ReminderInfo).IsModified = true;
                entry.Property(e => e.Label).IsModified = true;
                entry.Property(e => e.Status).IsModified = true;

                if (appointment.MarketVehicleId != null)
                {
                    entry.Property(e => e.MarketVehicleId).IsModified = true;
                }

            }

            int res = _currentDbContext.SaveChanges();

            if (res > 0) success = true;

            return success;

        }
        public bool DeleteOrderScheduleAppointment(OrderSchedule appointment)
        {
            bool success = false;

            var delAppointment = _currentDbContext.OrderSchedule.Find(appointment.Id);
            if (delAppointment != null)
            {
                delAppointment.IsCanceled = true;
            }
            _currentDbContext.Entry(delAppointment).State = EntityState.Modified;
            int res = _currentDbContext.SaveChanges();

            if (res > 0) success = true;

            return success;
        }


    }
}