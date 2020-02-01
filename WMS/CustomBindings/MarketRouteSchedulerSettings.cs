using System.Linq;
using Ganedata.Core.Data;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using System;
using DevExpress.Web.Mvc;
using System.Web.Mvc;
using Ganedata.Core.Services;
using System.Collections;

namespace WMS.CustomBindings
{
    public class MarketRouteSchedulerSettings
    {
        static MVCxAppointmentStorage appointmentStorage;

        public static MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                {
                    appointmentStorage = new MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "MarketRouteScheduleId";
                    appointmentStorage.Mappings.Start = "StartTime";
                    appointmentStorage.Mappings.End = "EndTime";
                    appointmentStorage.Mappings.Subject = "Subject";
                    appointmentStorage.Mappings.Description = "Description";
                    appointmentStorage.Mappings.Location = "Location";
                    appointmentStorage.Mappings.AllDay = "AllDay";
                    appointmentStorage.Mappings.Type = "EventType";
                    appointmentStorage.Mappings.RecurrenceInfo = "RecurrenceInfo";
                    appointmentStorage.Mappings.ReminderInfo = "ReminderInfo";
                    appointmentStorage.Mappings.Label = "Label";
                    appointmentStorage.Mappings.Status = "Status";
                    appointmentStorage.Mappings.ResourceId = "WarehouseIDs";
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("Canceled", "IsCanceled"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("VehicleId", "VehicleId"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("TenentId", "TenentId"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("WarehouseId", "WarehouseId"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("RouteId", "RouteId"));
                }
                return appointmentStorage;
            }
        }

        static MVCxResourceStorage resourceStorage;

        public static MVCxResourceStorage ResourceStorage
        {
            get
            {
                if (resourceStorage == null)
                {
                    resourceStorage = new MVCxResourceStorage();
                    resourceStorage.Mappings.ResourceId = "WarehouseId";
                    resourceStorage.Mappings.Caption = "WarehouseName";
                }
                return resourceStorage;
            }
        }

        public static void UpdateEditableDataObject()
        {
            InsertAppointments(DataObject);
            UpdateAppointments(DataObject);
            DeleteAppointments(DataObject);
        }

        static void InsertAppointments(SchedulerDataObject dataObject)
        {
            var _marketRouteScheduleService = DependencyResolver.Current.GetService<MarketRouteScheduleService>();
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var newAppointments =
                SchedulerExtension
                    .GetAppointmentsToInsert<Ganedata.Core.Entities.Domain.MarketRouteSchedule>("RouteScheduler",
                        dataObject.FetchAppointments, dataObject.Resources,
                        AppointmentStorage, ResourceStorage);
            foreach (var appointment in newAppointments)
            {
                context.MarketRouteSchedules.Add(appointment);
            }
            context.SaveChanges();
        }

        static void UpdateAppointments(SchedulerDataObject dataObject)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var updAppointments =
                SchedulerExtension
                    .GetAppointmentsToUpdate<Ganedata.Core.Entities.Domain.MarketRouteSchedule>("RouteScheduler",
                        dataObject.FetchAppointments, dataObject.Resources,
                        AppointmentStorage, ResourceStorage);
            foreach (var appointment in updAppointments)
            {
                var origAppointment =
                    context.MarketRouteSchedules.FirstOrDefault(a => a.MarketRouteScheduleId == appointment.MarketRouteScheduleId);

                if (string.IsNullOrEmpty(appointment.WarehouseIDs) || !appointment.WarehouseIDs.Contains("<ResourceIds>"))
                {
                    int ResId = Convert.ToInt32(appointment.WarehouseId);
                    appointment.WarehouseIDs =
                        $"<ResourceIds>\r\n<ResourceId Type = \"System.Int32\" Value = \"{ResId}\" />\r\n</ResourceIds>";
                }

                context.Entry(origAppointment).CurrentValues.SetValues(appointment);
            }
            context.SaveChanges();
        }

        static void DeleteAppointments(SchedulerDataObject dataObject)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var delAppointments =
                SchedulerExtension
                    .GetAppointmentsToRemove<Ganedata.Core.Entities.Domain.MarketRouteSchedule>("RouteScheduler",
                        dataObject.FetchAppointments, dataObject.Resources,
                        AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {
                var delAppointment =
                    context.MarketRouteSchedules.FirstOrDefault(a => a.MarketRouteScheduleId == appointment.MarketRouteScheduleId);
                if (delAppointment != null)
                    delAppointment.IsCanceled = true;
            }

            context.SaveChanges();
        }

        public static IEnumerable GetResources()
        {
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            int tenantId = caCurrent.CurrentTenant().TenantId;
            return _currentDbContext.TenantWarehouses.Where(e => e.TenantId == tenantId && e.IsMobile == true && e.IsDeleted != true).ToList();
        }

        public static object FetchAppointmentsHelperMethod(FetchAppointmentsEventArgs args)
        {
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            int tenantId = caCurrent.CurrentTenant().TenantId;
            args.ForceReloadAppointments = true;
            DateTime startDate = args.Interval.Start;
            DateTime endDate = args.Interval.End;
            var res = _currentDbContext.MarketRouteSchedules.Where(m => ((m.StartTime >= startDate && m.StartTime <= endDate) || (m.EndTime >= startDate && m.EndTime <= endDate) ||
                                             (m.StartTime >= startDate && m.EndTime <= endDate) || (m.StartTime < startDate && m.EndTime > endDate) || (m.EventType > 0)) && m.IsCanceled != true).ToList();

            return res;

        }
        public static SchedulerDataObject DataObject
        {
            get
            {
                SchedulerDataObject sdo = new SchedulerDataObject();
                sdo.Resources = GetResources();
                sdo.FetchAppointments = FetchAppointmentsHelperMethod;
                return sdo;
            }
        }

        public static SchedulerSettings GetSchedulerSettings()
        {
            SchedulerSettings settings = new SchedulerSettings();
            settings.Name = "RouteScheduler";
            settings.CallbackRouteValues = new { Controller = "MarketRouteSchedules", Action = "SchedulerPartial" };
            settings.EditAppointmentRouteValues = new { Controller = "MarketRouteSchedules", Action = "SchedulerPartialEditAppointment" };
            settings.CustomActionRouteValues = new { Controller = "MarketRouteSchedules", Action = "CreateAppointment" };

            settings.OptionsBehavior.ShowFloatingActionButton = false;
            settings.Storage.Appointments.Assign(AppointmentStorage);
            settings.Storage.Resources.Assign(ResourceStorage);
            settings.Storage.EnableReminders = true;
            settings.GroupType = SchedulerGroupType.Resource;

            // event handler for Availabilities
            settings.HtmlTimeCellPrepared += Scheduler_HtmlTimeCellPrepared;
            settings.Views.TimelineView.Enabled = false;
            settings.Views.WeekView.Enabled = false;
            // Day View
            settings.Views.DayView.Styles.ScrollAreaHeight = 640;
            settings.Views.DayView.WorkTime.Start = new TimeSpan(7, 0, 0);
            settings.Views.DayView.WorkTime.End = new TimeSpan(20, 0, 0);
            settings.Views.DayView.ShowWorkTimeOnly = true;
            settings.Views.DayView.ResourcesPerPage = 5;
            // Work Days View
            settings.WorkDays.Add(WeekDays.Saturday);
            settings.Views.WorkWeekView.Styles.ScrollAreaHeight = 640;
            settings.Views.WorkWeekView.WorkTime.Start = new TimeSpan(7, 0, 0);
            settings.Views.WorkWeekView.WorkTime.End = new TimeSpan(20, 0, 0);
            settings.Views.WorkWeekView.ShowWorkTimeOnly = true;
            settings.Views.WorkWeekView.ResourcesPerPage = 1;
            // Month View
            settings.Views.MonthView.ResourcesPerPage = 1;
            settings.Storage.Appointments.ResourceSharing = true;
            settings.OptionsCustomization.AllowAppointmentDragBetweenResources = UsedAppointmentType.None;
            settings.OptionsCustomization.AllowAppointmentDrag = UsedAppointmentType.NonRecurring;
            settings.OptionsCustomization.AllowAppointmentDelete = UsedAppointmentType.NonRecurring;
            settings.OptionsCustomization.AllowAppointmentConflicts = AppointmentConflictsMode.Forbidden;

            SchedulerCompatibility.Base64XmlObjectSerialization = false;
            settings.ClientSideEvents.AppointmentDeleting = "OnAppointmentDeleting";
            settings.ClientSideEvents.EndCallback = "OnAppointmentEndCallBack";
            settings.ClientSideEvents.BeginCallback = "OnAppointmentBeginCallback";
            settings.AppointmentFormShowing += Scheduler_AppointmentFormShowing;

            settings.PopupMenuShowing = (sender, e) =>
            {
                if (e.Menu.MenuId == SchedulerMenuItemId.AppointmentMenu)
                {
                    DevExpress.Web.MenuItem item =
                        e.Menu.Items.FindByName("DeleteAppointment") as DevExpress.Web.MenuItem;
                    item.Text = "Cancel Route";

                    DevExpress.Web.MenuItem statusMenu =
                        e.Menu.Items.FindByName(SchedulerMenuItemId.StatusSubMenu.ToString());
                    if (statusMenu != null) statusMenu.Visible = false;

                    DevExpress.Web.MenuItem labelMenu =
                        e.Menu.Items.FindByName(SchedulerMenuItemId.LabelSubMenu.ToString());
                    if (labelMenu != null) labelMenu.Text = "Priority";

                    //Hide items temerary as delete button not working. Need to establish if its bug in DevExpress 17.1 release
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
                //Console.WriteLine(e.ToString());

            };

            return settings;
        }

        static void Scheduler_HtmlTimeCellPrepared(object handler, ASPxSchedulerTimeCellPreparedEventArgs e)
        {
            //var rid = e.Resource.Id.ToString();
            //var Interval = e.Interval;

            //e.Cell.BackColor = e.Cell.BackColor;
            ////e.Cell.Style.Add("color", System.Drawing.ColorTranslator.ToHtml(ColorHelper.InvertColor(e.Cell.BackColor)));
            //e.Cell.Style.Add("text-align", "center");
            //e.Cell.Controls.Add(new LiteralControl("N/A"));

        }

        static void Scheduler_AppointmentFormShowing(object handler, AppointmentFormEventArgs e)
        {
            // Console.WriteLine("Got it!");
        }

        protected void Storage_FilterAppointment(object sender, PersistentObjectCancelEventArgs e)
        {
            //e.Cancel = ((Appointment)e.Object). == statusID;
        }
    }
}