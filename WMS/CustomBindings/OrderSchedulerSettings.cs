using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.Mvc;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class OrderSchedulerSettings
    {
        static DevExpress.Web.Mvc.MVCxAppointmentStorage appointmentStorage;
        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                var CurrentTenant = caCurrent.CurrentTenant();
                var appointmentServices = DependencyResolver.Current.GetService<IAppointmentsService>();

                var slaPriorities = appointmentServices.GetSlaPriorities(CurrentTenant.TenantId);

                if (appointmentStorage == null)
                {
                    appointmentStorage = new MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "Id";
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
                    appointmentStorage.Mappings.ResourceId = "ResourceIDs";
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("Canceled", "IsCanceled"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("PalletDispatchId", "PalletDispatchId"));
                    appointmentStorage.CustomFieldMappings.Add(new ASPxAppointmentCustomFieldMapping("TenentId", "TenentId"));

                    // clear existing lables and create new ones
                    appointmentStorage.Labels.Clear();
                    AppointmentLabelCollection customLables = new AppointmentLabelCollection();

                    foreach (var sla in slaPriorities)
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

        static MVCxResourceStorage resourceStorage;
        public static MVCxResourceStorage ResourceStorage
        {
            get
            {
                if (resourceStorage == null)
                {
                    resourceStorage = new MVCxResourceStorage();
                    resourceStorage.Mappings.ResourceId = "Id";
                    resourceStorage.Mappings.Caption = "Name";
                    //resourceStorage.Mappings.Color = "Color";

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

        private static void InsertAppointments(SchedulerDataObject dataObject)
        {
            var currentTenant = caCurrent.CurrentTenant();
            var appointmentServices = DependencyResolver.Current.GetService<IAppointmentsService>();

            var newAppointments = SchedulerExtension.GetAppointmentsToInsert<OrderSchedule>("Scheduler", dataObject.FetchAppointments, dataObject.Resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in newAppointments)
            {
                appointmentServices.CreateOrderScheduleAppointment(appointment);
            }
        }
        private static void UpdateAppointments(SchedulerDataObject dataObject)
        {
            var currentTenant = caCurrent.CurrentTenant();
            var currentUser = caCurrent.CurrentUser();
            var appointmentServices = DependencyResolver.Current.GetService<IAppointmentsService>();
            var _palletingService = DependencyResolver.Current.GetService<IPalletingService>();

            var updAppointments = SchedulerExtension.GetAppointmentsToUpdate<OrderSchedule>("Scheduler", dataObject.FetchAppointments, dataObject.Resources,
                AppointmentStorage, ResourceStorage);

            foreach (var appointment in updAppointments)
            {
                if (appointment.ResourceIDs == null) { continue; };
                var origAppointment = appointmentServices.GetAllActiveOrdersAppointments(currentTenant.TenantId).FirstOrDefault(a => a.Id == appointment.Id);

                if (!appointment.ResourceIDs.Contains("<ResourceIds>"))
                {


                    int ResId = Convert.ToInt32(appointment.ResourceIDs);
                    if (ResId > 0)
                    {
                        appointment.MarketVehicleId = ResId;
                    }

                    appointment.MarketVehicleId = ResId;
                    appointment.ResourceIDs =
                        $"<ResourceIds>\r\n<ResourceId Type = \"System.Int32\" Value = \"{ResId}\" />\r\n</ResourceIds>";
                }

                else
                {
                    ResourceIdCollection resources = new ResourceIdCollection();
                    AppointmentResourceIdCollectionXmlPersistenceHelper.ObjectFromXml(resources, appointment.ResourceIDs);

                    if (resources.ToList().Contains(origAppointment.MarketVehicleId))
                    {
                        appointment.MarketVehicleId = origAppointment.MarketVehicleId;
                    }
                    else
                    {
                        appointment.MarketVehicleId = Convert.ToInt32(resources.FirstOrDefault().ToString());
                    }

                }

               var status=appointmentServices.UpdateOrderScheduleAppointment(appointment);
                if (status)
                {
                    var order = _palletingService.UpdatePalletsDispatchStatus((appointment.PalletDispatchId.HasValue?appointment.PalletDispatchId.Value:0), (appointment.MarketVehicleId.HasValue? appointment.MarketVehicleId : 0), currentUser.UserId);

                }
            }

        }

        private static void DeleteAppointments(SchedulerDataObject dataObject)
        {
            var currentTenant = caCurrent.CurrentTenant();
            var currentUser = caCurrent.CurrentUser();
            var appointmentServices = DependencyResolver.Current.GetService<IAppointmentsService>();
            var palletservice = DependencyResolver.Current.GetService<IPalletingService>();

            var delAppointments = SchedulerExtension.GetAppointmentsToRemove<OrderSchedule>("Scheduler", dataObject.FetchAppointments, dataObject.Resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {

                // set IsCanceled flag for appointment
                if (appointment != null)
                {
                    appointmentServices.DeleteOrderScheduleAppointment(appointment);
                }

                //get order against appointment
                var palletdispatch = palletservice.GetPalletsDispatchByDispatchId((appointment.PalletDispatchId??0));

                // set order status for rescheduling
                if (palletdispatch != null)
                {
                    palletservice.UpdatePalletsDispatchStatus((appointment.PalletDispatchId ?? 0),null, currentUser.UserId, true);
                }

                //cancel notification queues
                //appointmentServices.CancelNotificationQueuesforAppointment(appointment.AppointmentId);
            }
        }

        public static IEnumerable GetResources()
        {
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            int CurrentTenantId = caCurrent.CurrentTenant().TenantId;
            var resources = _currentDbContext.MarketVehicles.AsNoTracking().Where(a => a.IsDeleted != true && a.TenantId == CurrentTenantId).ToList();
            return resources;
        }

        public static object FetchAppointmentsHelperMethod(FetchAppointmentsEventArgs args)
        {
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            int CurrentTenantId = caCurrent.CurrentTenant().TenantId;
            args.ForceReloadAppointments = true;
            DateTime startDate = args.Interval.Start;
            DateTime endDate = args.Interval.End;
            return _currentDbContext.OrderSchedule.AsNoTracking().Where(m => ((m.StartTime >= startDate && m.StartTime <= endDate) || (m.EndTime >= startDate && m.EndTime <= endDate) ||
                                             (m.StartTime >= startDate && m.EndTime <= endDate) || (m.StartTime < startDate && m.EndTime > endDate) || (m.EventType > 0)) && m.IsCanceled != true).ToList();
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
            settings.CallbackRouteValues = new { Controller = "OrderSchedule", Action = "SchedulerPartial" };
            settings.EditAppointmentRouteValues = new { Controller = "OrderSchedule", Action = "SchedulerPartialEditAppointment" };
            settings.CustomActionRouteValues = new { Controller = "OrderSchedule", Action = "CreateAppointment" };
            //settings.OptionsCustomization.AllowAppointmentEdit = UsedAppointmentType.None;
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
            settings.Views.DayView.Styles.ScrollAreaHeight = 500;
            settings.Views.DayView.WorkTime.Start = new TimeSpan(7, 0, 0);
            settings.Views.DayView.WorkTime.End = new TimeSpan(20, 0, 0);
            settings.Views.DayView.ShowWorkTimeOnly = true;
            settings.Views.DayView.ResourcesPerPage = 5;
            // Work Days View
            settings.WorkDays.Add(WeekDays.Saturday);
            settings.Views.WorkWeekView.Styles.ScrollAreaHeight = 500;
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