using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.Mvc;
using DevExpress.XtraScheduler;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;

namespace WMS.Controllers
{
    public class MarketRouteScheduleControllerSchedulerSettingsTest
    {
        private static readonly ApplicationContext AppointmentContext = ApplicationContext.SingletonInstance;
        static DevExpress.Web.Mvc.MVCxAppointmentStorage _appointmentStorage;
        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (_appointmentStorage == null)
                {
                    _appointmentStorage = new DevExpress.Web.Mvc.MVCxAppointmentStorage();
                    _appointmentStorage.Mappings.AppointmentId = "AppointmentId";
                    _appointmentStorage.Mappings.Start = "StartTime";
                    _appointmentStorage.Mappings.End = "EndTime";
                    _appointmentStorage.Mappings.Subject = "Subject";
                    _appointmentStorage.Mappings.Description = "Description";
                    _appointmentStorage.Mappings.Location = "Location";
                    _appointmentStorage.Mappings.AllDay = "AllDay";
                    _appointmentStorage.Mappings.Type = "EventType";
                    _appointmentStorage.Mappings.RecurrenceInfo = "RecurrenceInfo";
                    _appointmentStorage.Mappings.ReminderInfo = "ReminderInfo";
                    _appointmentStorage.Mappings.Label = "Label";
                    _appointmentStorage.Mappings.Status = "Status";
                    _appointmentStorage.Mappings.ResourceId = "VehicleIDs";
                    _appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("MarketRouteId", "MarketRouteId"));
                    _appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("MarketId", "MarketId"));
                    _appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("VehicleId", "VehicleId"));
                    _appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("TenentId", "TenentId"));

                    // clear existing lables and create new ones
                    _appointmentStorage.Labels.Clear();
                    AppointmentLabelCollection customLables = new AppointmentLabelCollection();

                    foreach (var sla in AppointmentContext.Markets)
                    {
                        var label = new AppointmentLabel
                        {
                            DisplayName = sla.Name,
                            MenuCaption = sla.Name,
                            Color = Color.FromArgb(new Random().Next(0,255),0,4)
                        };
                        customLables.Add(label);
                    }

                    _appointmentStorage.Labels.AddRange(customLables);

                    // change status
                    _appointmentStorage.Statuses.Clear();

                    AppointmentStatusCollection customStatuses = new AppointmentStatusCollection();

                    AppointmentStatus busy = new AppointmentStatus();
                    busy.Color = Color.Red;
                    busy.MenuCaption = "Busy";
                    busy.DisplayName = "Busy";
                    customStatuses.Add(busy);

                    _appointmentStorage.Statuses.AddRange(customStatuses);

                }
                return _appointmentStorage;
            }
        }

        static DevExpress.Web.Mvc.MVCxResourceStorage resourceStorage;
        public static DevExpress.Web.Mvc.MVCxResourceStorage ResourceStorage
        {
            get
            {
                if (resourceStorage == null)
                {
                    resourceStorage = new DevExpress.Web.Mvc.MVCxResourceStorage();
                    resourceStorage.Mappings.ResourceId = "VehicleID";
                    resourceStorage.Mappings.Caption = "Name";
                }
                return resourceStorage;
            }
        }

        public static void UpdateEditableDataObject()
        {
            InsertAppointments();
            UpdateAppointments();
            DeleteAppointments();
        }

        private static void InsertAppointments()
        {
            var appointments = AppointmentContext.MarketRouteSchedules.ToList();
            var markets = AppointmentContext.Markets.ToList();

            var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<MarketRouteSchedule>("Scheduler", appointments, markets,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in newAppointments)
            {
                AppointmentContext.MarketRouteSchedules.Add(appointment);
            }
            AppointmentContext.SaveChanges();
        }
        private static void UpdateAppointments()
        {
            var appointments = AppointmentContext.MarketRouteSchedules.ToList();
            var markets = AppointmentContext.Markets.ToList();

            var updAppointments = SchedulerExtension.GetAppointmentsToUpdate<MarketRouteSchedule>("Scheduler", appointments, markets,
                AppointmentStorage, ResourceStorage);

            foreach (var appointment in updAppointments)
            {
                var origAppointment = appointments.FirstOrDefault(a => a.MarketRouteScheduleId == appointment.MarketRouteScheduleId);

                if (!appointment.VehicleIDs.Contains("<VehicleIds>"))
                {
                    int ResId = Convert.ToInt32(appointment.VehicleIDs);
                    origAppointment.VehicleId = ResId;
                    origAppointment.VehicleIDs =
                        $"<VehicleIds>\r\n<VehicleId Type = \"System.Int32\" Value = \"{ResId}\" />\r\n</VehicleIds>";
                }
                else
                {
                    origAppointment.VehicleIDs = appointment.VehicleIDs;
                }

                AppointmentContext.Entry(origAppointment);

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

                    AppointmentContext.MarketRouteSchedules.Attach(origAppointment);
                    var entry = AppointmentContext.Entry<MarketRouteSchedule>(origAppointment);
                    entry.Property(e => e.AllDay).IsModified = true;
                    entry.Property(e => e.VehicleId).IsModified = true;
                    entry.Property(e => e.VehicleIDs).IsModified = true;
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

                }
            }
            AppointmentContext.SaveChanges();
        }

        private static void DeleteAppointments()
        {
            var appointments = AppointmentContext.MarketRouteSchedules.ToList();
            var markets = AppointmentContext.Markets.ToList();

            var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<Ganedata.Core.Entities.Domain.MarketRouteSchedule>("Scheduler", appointments, markets,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {
                // set IsCanceled flag for appointment
                if (appointment != null)
                {
                    var delAppointment = AppointmentContext.MarketRouteSchedules.FirstOrDefault(a => a.MarketRouteScheduleId == appointment.MarketRouteScheduleId);
                    if (delAppointment != null)

                        delAppointment.IsCanceled = true;
                    AppointmentContext.Entry(delAppointment).State = EntityState.Modified;
                }
            }

            AppointmentContext.SaveChanges();
        }

        public static SchedulerSettings GetSchedulerSettings()
        {
            SchedulerSettings settings = new SchedulerSettings();
            settings.Name = "Scheduler";
            settings.CallbackRouteValues = new { Controller = "MarketRouteSchedules", Action = "SchedulerPartial" };
            settings.EditAppointmentRouteValues = new { Controller = "MarketRouteSchedules", Action = "SchedulerPartialEditAppointment" };
            settings.CustomActionRouteValues = new { Controller = "MarketRouteSchedules", Action = "CreateAppointment" };

            settings.Storage.Appointments.Assign(AppointmentStorage);
            settings.Storage.Resources.Assign(ResourceStorage);
            settings.Storage.EnableReminders = true;
            settings.GroupType = SchedulerGroupType.Resource;

            // event handler for Availabilities
            settings.HtmlTimeCellPrepared += Scheduler_HtmlTimeCellPrepared;
            settings.Views.TimelineView.Enabled = false;
            settings.Views.WeekView.Enabled = false;
            // Day View
            settings.Views.DayView.Styles.ScrollAreaHeight = 610;
            settings.Views.DayView.WorkTime.Start = new TimeSpan(7, 0, 0);
            settings.Views.DayView.WorkTime.End = new TimeSpan(20, 0, 0);
            settings.Views.DayView.ShowWorkTimeOnly = true;
            settings.Views.DayView.ResourcesPerPage = 5;
            // Work Days View
            settings.WorkDays.Add(WeekDays.Saturday);
            settings.Views.WorkWeekView.Styles.ScrollAreaHeight = 610;
            settings.Views.WorkWeekView.WorkTime.Start = new TimeSpan(7, 0, 0);
            settings.Views.WorkWeekView.WorkTime.End = new TimeSpan(20, 0, 0);
            settings.Views.WorkWeekView.ShowWorkTimeOnly = true;
            settings.Views.WorkWeekView.ResourcesPerPage = 1;
            // Month View
            settings.Views.MonthView.ResourcesPerPage = 1;
            settings.Storage.Appointments.ResourceSharing = true;
            SchedulerCompatibility.Base64XmlObjectSerialization = false;
            settings.ClientSideEvents.AppointmentDeleting = "OnAppointmentDeleting";
            settings.ClientSideEvents.EndCallback = "OnAppointmentEndCallBack";
            settings.ClientSideEvents.AppointmentClick = "function(s,e){ OnAppointmentEventsClick(s,e); }";
            settings.ClientSideEvents.BeginCallback = "OnBeginCallback";
            settings.AppointmentFormShowing += Scheduler_AppointmentFormShowing;

            settings.PopupMenuShowing = (sender, e) =>
            {
                if (e.Menu.MenuId == SchedulerMenuItemId.AppointmentMenu)
                {
                    DevExpress.Web.MenuItem item =
                        e.Menu.Items.FindByName("DeleteAppointment") as DevExpress.Web.MenuItem;
                    item.Text = "Cancel Appointment";

                    DevExpress.Web.MenuItem statusMenu =
                        e.Menu.Items.FindByName(SchedulerMenuItemId.StatusSubMenu.ToString());
                    if (statusMenu != null) statusMenu.Visible = false;

                    DevExpress.Web.MenuItem labelMenu =
                        e.Menu.Items.FindByName(SchedulerMenuItemId.LabelSubMenu.ToString());
                    if (labelMenu != null) labelMenu.Text = "Priority";

                    if (item != null) item.Visible = false;
                    if (labelMenu != null) labelMenu.Visible = false;
                }

                else
                {
                    e.Menu.Items.Clear();
                }
            };

            settings.AppointmentFormShowing = (sender, e) =>
            {
            };

            return settings;
        }

        static void Scheduler_HtmlTimeCellPrepared(object handler, ASPxSchedulerTimeCellPreparedEventArgs e)
        {
            

        }

        static void Scheduler_AppointmentFormShowing(object handler, AppointmentFormEventArgs e)
        {
            
        }

        protected void Storage_FilterAppointment(object sender, PersistentObjectCancelEventArgs e)
        {
         
        }
    }
}