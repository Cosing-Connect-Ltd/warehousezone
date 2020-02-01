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

        Appointments CreateAppointment(string start, string end, string subject, string resourceId, int orderId,
            int joblabel, int tenantId);

        void UpdateAllAppointmentSubjects();

        List<SLAPriorit> GetSlaPriorities(int tenantId);
        IQueryable<Appointments> GetAllActiveAppointments(int tenantId);
        bool CreateAppointment(Appointments appointment);
        bool UpdateAppointment(Appointments appointment);
        bool DeleteAppointment(Appointments appointment);
        bool CancelNotificationQueuesforAppointment(int appointmentId);
    }
}