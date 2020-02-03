using System;
using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Services
{
    public interface IAppointmentsService
    {
        IEnumerable<Appointments> GetAllAppointments(DateTime? filterByDate = null, int resourceId = 0, bool includeCancelled = false);
        IEnumerable<Resources> GetAllResources(int TenantId,DateTime? filterByDate = null);
        Appointments GetAppointmentById(int appointmentId);
        Resources GetAppointmentResourceById(int appointmentResourceId);
        Appointments GetMostRecentAppointmentForOrder(int orderId);
        Appointments CreateAppointment(string start, string end, string subject, string resourceId, int orderId,int joblabel, int tenantId);
        void UpdateAllAppointmentSubjects();
        List<SLAPriorit> GetSlaPriorities(int tenantId);
        IQueryable<Appointments> GetAllActiveAppointments(int tenantId);
        bool CreateAppointment(Appointments appointment);
        bool UpdateAppointment(Appointments appointment);
        bool DeleteAppointment(Appointments appointment);
        bool CancelNotificationQueuesforAppointment(int appointmentId);

        //------------------- orders Schedule--------------------------------
        IEnumerable<OrderSchedule> GetAllOrderSchedule(DateTime? filterByDate = null, int resourceId = 0, bool includeCancelled = false);
        IEnumerable<MarketVehicle> GetAllMarketVehicle(int TenantId, DateTime? filterByDate = null);
        OrderSchedule GetOrderScheduleId(int Id);
        MarketVehicle GetMarketVehicleById(int VechicleId);
        IQueryable<OrderSchedule> GetAllActiveOrdersAppointments(int tenantId);
        OrderSchedule CreateOrderScheduleAppointment(string start, string end, string subject, string resourceId, int joblabel, int tenantId, int palletDispatchId);
        bool CreateOrderScheduleAppointment(OrderSchedule appointment);
        bool UpdateOrderScheduleAppointment(OrderSchedule appointment);
        bool DeleteOrderScheduleAppointment(OrderSchedule appointment);
       


    }
}