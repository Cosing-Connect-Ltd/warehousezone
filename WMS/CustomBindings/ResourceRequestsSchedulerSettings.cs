using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.Mvc;
using DevExpress.XtraScheduler;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class ResourceRequestsSchedulerSettings
    {
        static DevExpress.Web.Mvc.MVCxAppointmentStorage appointmentStorage;
        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                var AppointmentContext = DependencyResolver.Current.GetService<IApplicationContext>();

                if (appointmentStorage == null)
                {
                    appointmentStorage = new DevExpress.Web.Mvc.MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "Id";
                    appointmentStorage.Mappings.Start = "StartDate";
                    appointmentStorage.Mappings.End = "EndDate";
                    appointmentStorage.Mappings.Subject = "HolidayReason";
                    appointmentStorage.Mappings.Description = "Notes";
                    appointmentStorage.Mappings.Location = "Location";
                    appointmentStorage.Mappings.AllDay = "AllDay";
                    appointmentStorage.Mappings.Type = "EventType";
                    appointmentStorage.Mappings.RecurrenceInfo = "RecurrenceInfo";
                    appointmentStorage.Mappings.ReminderInfo = "ReminderInfo";
                    appointmentStorage.Mappings.Label = "Label";
                    appointmentStorage.Mappings.Status = "Status";
                    appointmentStorage.Mappings.ResourceId = "ResourceId";

                    // clear existing lables and create new ones
                    appointmentStorage.Labels.Clear();
                    AppointmentLabelCollection customLables = new AppointmentLabelCollection();

                    foreach (var sla in AppointmentContext.SLAPriorities)
                    {
                        var label = new AppointmentLabel
                        {
                            DisplayName = sla.Priority,
                            MenuCaption = sla.Priority,
                            Color = Color.FromName(sla.Colour)
                        };
                        customLables.Add(label);
                    }

                    appointmentStorage.Labels.AddRange(customLables);

                    // change status
                    appointmentStorage.Statuses.Clear();

                    AppointmentStatusCollection customStatuses = new AppointmentStatusCollection();

                    AppointmentStatus busy = new AppointmentStatus();
                    busy.Color = Color.Red;
                    busy.MenuCaption = "Busy";
                    busy.DisplayName = "Busy";
                    customStatuses.Add(busy);

                    appointmentStorage.Statuses.AddRange(customStatuses);

                }
                return appointmentStorage;
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
                    resourceStorage.Mappings.ResourceId = "ResourceId";
                    resourceStorage.Mappings.Caption = "Name";
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
            var AppointmentContext = DependencyResolver.Current.GetService<IApplicationContext>();

            var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<Ganedata.Core.Entities.Domain.ResourceRequests>("Scheduler", dataObject.FetchAppointments, dataObject.Resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in newAppointments)
            {
                AppointmentContext.ResourceHolidays.Add(appointment);
                var empService = DependencyResolver.Current.GetService<IEmployeeServices>();

                empService.AddResourceHolidayRequest(new ResourceRequestsViewModel()
                {
                    StartDate = appointment.StartDate,
                    AllDay = appointment.AllDay,
                    EndDate = appointment.EndDate,
                    EventType = appointment.EventType,
                    HolidayReason = appointment.HolidayReason,
                    Location = appointment.Location,
                    Label = appointment.Label,
                    RequestType = (ResourceRequestTypesEnum)appointment.EventType,
                    ResourceName = appointment.Resources.Name
                },
                    caCurrent.CurrentTenant().TenantId, caCurrent.CurrentUser().UserId);
            }
            AppointmentContext.SaveChanges();
        }

        static void UpdateAppointments(SchedulerDataObject dataObject)
        {
            var AppointmentContext = DependencyResolver.Current.GetService<IApplicationContext>();

            var updAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToUpdate<Ganedata.Core.Entities.Domain.ResourceRequests>("Scheduler", dataObject.FetchAppointments, dataObject.Resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in updAppointments)
            {
                var origAppointment = AppointmentContext.ResourceHolidays.FirstOrDefault(a => a.Id == appointment.Id);
                AppointmentContext.Entry(origAppointment).CurrentValues.SetValues(appointment);

                var empService = DependencyResolver.Current.GetService<IEmployeeServices>();
                //TODO: Have to establish 1-1 relationship between Actual Appointments Model. Check with Shoaib
                empService.UpdateResourceHolidayRequest(new ResourceRequestsViewModel()
                {
                    StartDate = appointment.StartDate,
                    AllDay = appointment.AllDay,
                    EndDate = appointment.EndDate,
                    EventType = appointment.EventType,
                    HolidayReason = appointment.HolidayReason,
                    Location = appointment.Location,
                    Label = appointment.Label,
                    RequestType = appointment.RequestType,
                    ResourceId = appointment.ResourceId,
                }, caCurrent.CurrentUser().UserId);
            }
            AppointmentContext.SaveChanges();
        }

        static void DeleteAppointments(SchedulerDataObject dataObject)
        {
            var AppointmentContext = DependencyResolver.Current.GetService<IApplicationContext>();

            var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<Ganedata.Core.Entities.Domain.ResourceRequests>("Scheduler", dataObject.FetchAppointments, dataObject.Resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {
                var delAppointment = AppointmentContext.ResourceHolidays.FirstOrDefault(a => a.Id == appointment.Id);
                if (delAppointment != null)
                {
                    delAppointment.IsDeleted = true;
                }
                AppointmentContext.Entry(delAppointment).State = EntityState.Modified;
            }
            AppointmentContext.SaveChanges();
        }

        public static IEnumerable GetResources()
        {
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            return _currentDbContext.Resources.Where(x => x.IsActive == true && x.IsDeleted != true).ToList();
        }

        public static object FetchAppointmentsHelperMethod(FetchAppointmentsEventArgs args)
        {
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            args.ForceReloadAppointments = true;
            DateTime startDate = args.Interval.Start;
            DateTime endDate = args.Interval.End;
            return _currentDbContext.ResourceHolidays.Where(m => ((m.StartDate >= startDate && m.StartDate <= endDate) || (m.EndDate >= startDate && m.EndDate <= endDate) ||
                                             (m.StartDate >= startDate && m.EndDate <= endDate) || (m.StartDate < startDate && m.EndDate > endDate) || (m.EventType > 0))
                                             && m.IsDeleted != true && m.RequestStatus == ResourceRequestStatusEnum.Accepted).ToList();
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
            settings.Name = "Scheduler";
            settings.CallbackRouteValues = new { Controller = "ResourceRequests", Action = "SchedulerPartial" };
            settings.EditAppointmentRouteValues = new { Controller = "ResourceRequests", Action = "SchedulerPartialEditAppointment" };
            settings.ActiveViewType = SchedulerViewType.Agenda;
            settings.Views.AgendaView.DayCount = 7;

            settings.OptionsBehavior.ShowFloatingActionButton = false;
            settings.Storage.Appointments.Assign(AppointmentStorage);
            settings.Storage.Resources.Assign(ResourceStorage);
            settings.Storage.EnableReminders = true;

            settings.Views.DayView.Enabled = false;
            settings.Views.WorkWeekView.Enabled = false;
            settings.Views.WeekView.Enabled = true;
            settings.Views.MonthView.Enabled = true;
            settings.Views.TimelineView.Enabled = false;

            settings.Views.AgendaView.Enabled = true;
            settings.Views.AgendaView.AppointmentDisplayOptions.ShowResource = true;

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
            settings.Storage.Appointments.ResourceSharing = false;
            //SchedulerCompatibility.Base64XmlObjectSerialization = false;
            //settings.ClientSideEvents.AppointmentDeleting = "OnAppointmentDeleting";
            //settings.ClientSideEvents.EndCallback = "OnAppointmentEndCallBack";
            //settings.ClientSideEvents.AppointmentClick = "function(s,e){ OnAppointmentEventsClick(s,e); }";
            //settings.ClientSideEvents.BeginCallback = "OnBeginCallback";
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