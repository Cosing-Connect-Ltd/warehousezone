using DevExpress.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Dialogs;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.Web.Mvc;
using DevExpress.XtraScheduler;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using StackExchange.Profiling.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI.WebControls;

namespace WMS.CustomBindings
{
    public class ShiftSchedulerSettings
    {
        private static MVCxAppointmentStorage appointmentStorage;
        public static MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                {
                    appointmentStorage = new MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "Id";
                    appointmentStorage.Mappings.Start = "StartTime";
                    appointmentStorage.Mappings.End = "EndTime";
                    appointmentStorage.Mappings.Subject = "Subject";
                    appointmentStorage.Mappings.Description = "Description";
                    appointmentStorage.Mappings.Type = "Type";
                    appointmentStorage.Mappings.RecurrenceInfo = "RecurrenceInfo";
                    appointmentStorage.Mappings.ResourceId = "ResourceId";
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("Canceled", "IsCanceled"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("TenantId", "TenantId"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("LocationId", "LocationId"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("TimeBreaks", "TimeBreaks"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("ResourceId", "ResourceId"));
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
                    resourceStorage.Mappings.ResourceId = "ResourceId";
                    resourceStorage.Mappings.Caption = "Name";
                }
                return resourceStorage;
            }
        }

        public static void UpdateEditableDataObject()
        {
            InsertShiftSchedules(DataObject);
            UpdateShiftSchedules(DataObject);
            DeleteShiftSchedules(DataObject);
        }

        private static void InsertShiftSchedules(SchedulerDataObject dataObject)
        {

            var shiftScheduleService = DependencyResolver.Current.GetService<IShiftScheduleService>();
            var currentTenant = caCurrent.CurrentTenant();
            var currentWarehouse = caCurrent.CurrentWarehouse();
            var shiftSchedules = SchedulerExtension.GetAppointmentsToInsert<ShiftSchedule>("Scheduler",
                                                                                           dataObject.FetchAppointments,
                                                                                           dataObject.Resources,
                                                                                           AppointmentStorage,
                                                                                           ResourceStorage);

            foreach (var shiftSchedule in shiftSchedules)
            {
                shiftSchedule.TenantId = currentTenant.TenantId;
                shiftSchedule.LocationsId = currentWarehouse.WarehouseId;
                shiftScheduleService.Create(shiftSchedule);
            }
        }
        private static void UpdateShiftSchedules(SchedulerDataObject dataObject)
        {
            var shiftScheduleService = DependencyResolver.Current.GetService<IShiftScheduleService>();

            var shiftSchedules = SchedulerExtension.GetAppointmentsToUpdate<ShiftSchedule>("Scheduler",
                                                                                           dataObject.FetchAppointments,
                                                                                           dataObject.Resources,
                                                                                           AppointmentStorage,
                                                                                           ResourceStorage);
            foreach (var shiftSchedule in shiftSchedules)
            {
                shiftScheduleService.Update(shiftSchedule);
            }
        }

        private static void DeleteShiftSchedules(SchedulerDataObject dataObject)
        {
            var shiftScheduleService = DependencyResolver.Current.GetService<IShiftScheduleService>();

            var shiftSchedules = SchedulerExtension.GetAppointmentsToRemove<ShiftSchedule>("Scheduler",
                                                                                           dataObject.FetchAppointments,
                                                                                           dataObject.Resources,
                                                                                           AppointmentStorage,
                                                                                           ResourceStorage);

            foreach (var shiftSchedule in shiftSchedules)
            {
                if (shiftSchedule != null)
                {
                    shiftScheduleService.Delete(shiftSchedule);
                }
            }
        }

        public static IEnumerable GetResources()
        {
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            int currentTenantId = caCurrent.CurrentTenant().TenantId;
            var warehouseId = caCurrent.CurrentWarehouse().WarehouseId;
            var resources = _currentDbContext.EmployeeShifts_Stores.AsNoTracking().Where(a => a.Resources.IsDeleted != true &&
                                                                                  a.Resources.IsActive &&
                                                                                  a.TenantLocations.WarehouseId == warehouseId &&
                                                                                  a.Resources.TenantId == currentTenantId)
                                                                                 .Select(r => r.Resources)
                                                                                 .OrderBy(a => a.FirstName)
                                                                                 .ThenBy(a => a.SurName)
                                                                                 .ToList();
            return resources;
        }

        public static object FetchAppointmentsHelperMethod(FetchAppointmentsEventArgs args)
        {
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            int currentTenantId = caCurrent.CurrentTenant().TenantId;
            args.ForceReloadAppointments = true;
            var startDate = args.Interval.Start;
            var endDate = args.Interval.End;
            var tt = _currentDbContext.ShiftSchedules.AsNoTracking().Where(m => ((m.StartTime >= startDate && m.StartTime <= endDate) ||
                                                                                (m.EndTime >= startDate && m.EndTime <= endDate) ||
                                                                                (m.StartTime >= startDate && m.EndTime <= endDate) ||
                                                                                (m.StartTime < startDate && m.EndTime > endDate) ||
                                                                                (m.Type > 0)) &&
                                                                                m.TenantId == currentTenantId &&
                                                                                m.IsCanceled != true)
                                                                  .ToList();

            return tt;
        }

        public static SchedulerDataObject DataObject
        {
            get
            {
                var schedulerDataObject = new SchedulerDataObject();
                schedulerDataObject.Resources = GetResources();
                schedulerDataObject.FetchAppointments = FetchAppointmentsHelperMethod;
                return schedulerDataObject;
            }
        }

        public static SchedulerSettings GetSchedulerSettings(HtmlHelper customHtml)
        {
            var settings = new SchedulerSettings();

            settings.Name = "scheduler";
            settings.CallbackRouteValues = new { Controller = "Shifts", Action = "SchedulerPartial" };
            settings.EditAppointmentRouteValues = new { Controller = "Shifts", Action = "ShiftSchedulePartialEdit" };
            settings.OptionsCustomization.AllowAppointmentEdit = UsedAppointmentType.All;
            settings.OptionsCustomization.AllowAppointmentCreate = UsedAppointmentType.All;
            settings.OptionsBehavior.ShowFloatingActionButton = false;
            settings.Storage.Appointments.Assign(AppointmentStorage);
            settings.Storage.Resources.Assign(ResourceStorage);
            settings.Storage.EnableReminders = false;
            settings.GroupType = SchedulerGroupType.Resource;

            // event handler for Availabilities
            settings.Views.TimelineView.Enabled = false;
            settings.Views.WeekView.Enabled = false;
            // Day View
            settings.Views.DayView.Styles.ScrollAreaHeight = 500;
            settings.Views.DayView.WorkTime.Start = new TimeSpan(7, 0, 0);
            settings.Views.DayView.WorkTime.End = new TimeSpan(20, 0, 0);
            settings.Views.DayView.ShowWorkTimeOnly = true;
            settings.Views.DayView.ResourcesPerPage = 7;
            // Work Days View
            settings.WorkDays.Add(WeekDays.Saturday);
            settings.Views.WorkWeekView.Styles.ScrollAreaHeight = 500;
            settings.Views.WorkWeekView.WorkTime.Start = new TimeSpan(7, 0, 0);
            settings.Views.WorkWeekView.WorkTime.End = new TimeSpan(20, 0, 0);
            settings.Views.WorkWeekView.ShowWorkTimeOnly = true;
            settings.Views.WorkWeekView.ResourcesPerPage = 1;
            // Month View
            settings.Views.MonthView.ResourcesPerPage = 1;
            settings.Storage.Appointments.ResourceSharing = false;
            SchedulerCompatibility.Base64XmlObjectSerialization = false;

            settings.PopupMenuShowing = (sender, e) =>
            {
                var newAppointmentMenuItem = e.Menu.Items.FindByName(SchedulerMenuItemId.NewAppointment.ToString());
                if (newAppointmentMenuItem != null)
                {
                    newAppointmentMenuItem.Text = "New Shift";
                }

                var newAllDayEventMenuItem = e.Menu.Items.FindByName(SchedulerMenuItemId.NewAllDayEvent.ToString());
                if (newAllDayEventMenuItem != null)
                {
                    newAllDayEventMenuItem.Visible = false;
                }

                var newRecurringAppointmentMenuItem = e.Menu.Items.FindByName(SchedulerMenuItemId.NewRecurringAppointment.ToString());
                if (newRecurringAppointmentMenuItem != null)
                {
                    newRecurringAppointmentMenuItem.Text = "New Recurring Shift";
                }

                var newRecurringEventMenuItem = e.Menu.Items.FindByName(SchedulerMenuItemId.NewRecurringEvent.ToString());
                if (newRecurringEventMenuItem != null)
                {
                    newRecurringEventMenuItem.Visible = false;
                }

                var labelSubMenuItem = e.Menu.Items.FindByName(SchedulerMenuItemId.LabelSubMenu.ToString());
                if (labelSubMenuItem != null)
                {
                    labelSubMenuItem.Visible = false;
                }

                var statusSubMenuItem = e.Menu.Items.FindByName(SchedulerMenuItemId.StatusSubMenu.ToString());
                if (statusSubMenuItem != null)
                {
                    statusSubMenuItem.Visible = false;
                }
            };

            settings.PrepareAppointmentFormPopupContainer = (sender, e) =>
            {
                MVCxScheduler schedule = sender as MVCxScheduler;
                if (schedule.SelectedAppointments.Count > 0)
                {
                    e.Popup.HeaderText = "Edit Shift" + (string.IsNullOrEmpty(schedule.SelectedAppointments[0].Subject) ? "" : ": " + schedule.SelectedAppointments[0].Subject);
                }
                else
                {
                    e.Popup.HeaderText = "New Shift";
                }
            };


            settings.OptionsForms.RecurrenceFormName = "shiftRecurrenceForm";

            settings.AppointmentFormShowing = (sender, e) => {
                var scheduler = sender as MVCxScheduler;
                if (scheduler != null)
                    e.Container = new CustomAppointmentTemplateContainer(scheduler);
            };

            settings.OptionsForms.SetAppointmentFormTemplateContent(c => {
                var container = (CustomAppointmentTemplateContainer)c;

                var schedule = new ShiftSchedule()
                {
                    Id = container.Appointment.Id == null ? -1 : (int)container.Appointment.Id,
                    Subject = container.Appointment.Subject,
                    StartTime = container.Appointment.Start,
                    EndTime = container.Appointment.End,
                    Description = container.Appointment.Description,
                    Type = (int)container.Appointment.Type,
                    ResourceId = container.Appointment.ResourceId == ResourceEmpty.Id ? 1 : (int)container.Appointment.ResourceId,
                    TimeBreaks = (TimeSpan?)container.CustomFields["TimeBreaks"],
                    LocationsId = (int)(container.CustomFields["LocationId"] ?? caCurrent.CurrentWarehouse().WarehouseId),
                    TenantId = (int)(container.CustomFields["TenantId"] ?? caCurrent.CurrentTenant().TenantId),
                    RecurrenceInfo = container.Appointment.RecurrenceInfo?.ToXml()
                };

                customHtml.ViewBag.DeleteButtonEnabled = container.CanDeleteAppointment;
                customHtml.ViewBag.IsRecurring = container.Appointment.IsRecurring;
                customHtml.ViewBag.AppointmentRecurrenceFormSettings = CreateAppointmentRecurrenceFormSettings(container);

                customHtml.ViewBag.ResourceDataSource = container.ResourceDataSource;
                customHtml.ViewBag.TimeBreaksDataSource = new List<TimeBreaksItem> {
                    new TimeBreaksItem { Value = null, Text= "No Breaks"},
                    new TimeBreaksItem { Value = TimeSpan.FromMinutes(15), Text= "15 Minutes"},
                    new TimeBreaksItem { Value = TimeSpan.FromMinutes(30), Text= "30 Minutes"},
                    new TimeBreaksItem { Value = TimeSpan.FromMinutes(45), Text= "45 Minutes"},
                    new TimeBreaksItem { Value = TimeSpan.FromMinutes(60), Text= "1 Hour"},
                    new TimeBreaksItem { Value = TimeSpan.FromMinutes(75), Text= "1 Hour 15 Mins"},
                    new TimeBreaksItem { Value = TimeSpan.FromMinutes(90), Text= "1 Hour 30 Mins"},
                    new TimeBreaksItem { Value = TimeSpan.FromMinutes(105), Text= "1 Hour 45 Mins"},
                    new TimeBreaksItem { Value = TimeSpan.FromMinutes(120), Text= "2 Hours"}
                };

                customHtml.RenderPartial("_ShiftScheduleFormPartial", schedule);
            });

            return settings;
        }

        static AppointmentRecurrenceFormSettings CreateAppointmentRecurrenceFormSettings(CustomAppointmentTemplateContainer container)
        {
            var settings = new AppointmentRecurrenceFormSettings();
            settings.Name = "shiftRecurrenceForm";
            settings.Width = Unit.Percentage(100);
            settings.IsRecurring = container.Appointment.IsRecurring;
            settings.DayNumber = container.RecurrenceDayNumber;
            settings.End = container.RecurrenceEnd;
            settings.Month = container.RecurrenceMonth;
            settings.OccurrenceCount = container.RecurrenceOccurrenceCount;
            settings.Periodicity = container.RecurrencePeriodicity;
            settings.RecurrenceRange = container.RecurrenceRange;
            settings.Start = container.Start;
            settings.WeekDays = container.RecurrenceWeekDays;
            settings.WeekOfMonth = container.RecurrenceWeekOfMonth;
            settings.RecurrenceType = container.RecurrenceType;
            settings.IsFormRecreated = container.IsFormRecreated;
            return settings;
        }
    }

    class TimeBreaksItem
    {
        public TimeSpan? Value { get; set; }
        public string Text { get; set; }
    }

    public class CustomAppointmentTemplateContainer : AppointmentFormTemplateContainer
    {
        public CustomAppointmentTemplateContainer(MVCxScheduler scheduler)
            : base(scheduler)
        {
        }

        public new IEnumerable ResourceDataSource
        {
            get { return ShiftSchedulerSettings.GetResources(); }
        }
    }
}