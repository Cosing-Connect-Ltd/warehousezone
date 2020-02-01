using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IEmployeeServices
    {
        Resources GetByEmployeeId(int employeeId);
        IEnumerable<Resources> GetAllEmployees(int tenantId);
        IEnumerable<Resources> GetAllEmployeesWithoutResourceLinks(int tenantId, int? includeResourceId = null);
        IQueryable<Resources> GetAllEmployeesByLocation(int tenantId, int locationId);
        IEnumerable<Resources> SearchByEmployee(string employeeName, int tenantId);
        int InsertEmployee(Resources employee);
        void UpdateEmployee(Resources employee);
        void Delete(Resources employee);
        void DeleteByEmployeeId(int employeeId);
        IQueryable<Resources> GetAllActiveAppointmentResourceses(int tenantId);
        Resources GetAppointmentResourceById(int resourceId);
        Resources AddResourceJobTypes(Resources employee, List<int> JobTypeIds);
        Resources UpdateResourceJobTypes(Resources employee, List<int> jobTypeIds);
        void DeleteAppointmentResource(int resourceId, int userId);
        List<int> GetResourceJobTypeIds(int resourceId);
        IQueryable<ResourceRequests> GetAllResourceRequests(int tenantId);
        ResourceRequests GetResourceHolidayRequestById(int requestId);
        ResourceRequests AddResourceHolidayRequest(ResourceRequestsViewModel request, int tenantId, int userId);
        List<HolidayResponseSync> GetUserHolidays(int userId, DateTime? reqDate = null, bool includeIsDeleted = false);
        HolidayResponseSync AddResourceHolidaySync(HolidayRequestSync request, int tenantId, int userId);
        ResourceRequests UpdateResourceHolidayRequest(ResourceRequestsViewModel request, int userId);
        ResourceRequests DeleteResourceHolidayRequestById(int requestId, int userId);
        HolidayRequestResult UpdateResourceHolidayRequestStatus(ApproveRequestViewModel model, int userId);
        HolidayRequestResult CountResourceHolidayRequestStatus(ApproveRequestViewModel model, int userId);

        bool IsUserAvailable(int userID);
        List<int> GetEmployeeYears(int CurrentWarehouseId);

        double GetHolidaysCountByEmployeeId(int EmployeeId, int Year);



    }

}

