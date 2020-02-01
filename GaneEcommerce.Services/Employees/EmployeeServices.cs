using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{


    public class EmployeeServices : IEmployeeServices
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IResourceHolidayServices _resourceHolidayServices;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        //constructor
        public EmployeeServices(IApplicationContext currentDbContext, IResourceHolidayServices resourceHolidayServices, IUserService userService, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _resourceHolidayServices = resourceHolidayServices;
            _userService = userService;
            _mapper = mapper;
        }

        //methods
        public Resources GetByEmployeeId(int employeeId)
        {
            return _currentDbContext.Resources.Find(employeeId);
        }

        public IEnumerable<Resources> GetAllEmployees(int tenantId)
        {
            return _currentDbContext.Resources.Where(x => x.TenantId.Equals(tenantId) && x.IsDeleted != true);
        }

        public IEnumerable<Resources> GetAllEmployeesWithoutResourceLinks(int tenantId, int? includeResourceId = null)
        {
            return _currentDbContext.Resources.Where(x => x.TenantId.Equals(tenantId) && x.IsDeleted != true && (x.AuthUser != null || x.ResourceId == includeResourceId));
        }

        public IQueryable<Resources> GetAllEmployeesByLocation(int tenantId, int locationId)
        {
            var result = _currentDbContext.Resources.Where(x => x.TenantId.Equals(tenantId) && x.EmployeeShifts_Stores.Any(z => z.WarehouseId == locationId) && x.IsDeleted != true)
                .Include(x => x.EmployeeRoles)
                .Include(x => x.EmployeeRoles.Select(y => y.Roles));
            return result;

        }

        public IEnumerable<Resources> SearchByEmployee(string employeeName, int tenantId)
        {
            return _currentDbContext.Resources.Where(s => s.TenantId.Equals(tenantId) && s.FirstName.ToLower().Contains(employeeName.ToLower()) || s.SurName.ToLower().Contains(employeeName.ToLower()) && s.IsDeleted != true);
        }

        public int InsertEmployee(Resources employee)
        {
            _currentDbContext.Entry(employee).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return employee.ResourceId;
        }

        public void UpdateEmployee(Resources employee)
        {
            _currentDbContext.Resources.Attach(employee);
            _currentDbContext.Entry(employee).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }
        public void Delete(Resources employee)
        {
            _currentDbContext.Resources.Attach(employee);
            _currentDbContext.Resources.Remove(employee);
            _currentDbContext.SaveChanges();
        }


        public Resources AddResourceJobTypes(Resources employee, List<int> jobTypeIds)
        {
            Resources resource = new Resources();

            if (employee.ResourceId > 0)
            {
                // add new job types if selected any
                if (jobTypeIds != null && jobTypeIds.Count > 0)
                {
                    resource = _currentDbContext.Resources.First(m => m.ResourceId == employee.ResourceId);
                    var newJobTypes = jobTypeIds.Where(m => !resource.JobTypes.Select(j => j.JobTypeId).Contains(m));
                    foreach (var item in newJobTypes)
                    {
                        var jobType = _currentDbContext.JobTypes.Find(item);
                        resource.JobTypes.Add(jobType);
                        _currentDbContext.Entry(resource).State = EntityState.Modified;
                    }
                }

                //save db changes
                _currentDbContext.SaveChanges();
            }

            return resource;
        }

        public Resources UpdateResourceJobTypes(Resources employee, List<int> jobTypeIds)
        {
            Resources resource = new Resources();

            if (employee.ResourceId > 0)
            {
                // remove old job types
                resource = _currentDbContext.Resources.First(m => m.ResourceId == employee.ResourceId);
                if (jobTypeIds != null && jobTypeIds.Count > 0)
                {
                    var removedJobTypes = resource.JobTypes.Where(m => !jobTypeIds.Contains(m.JobTypeId)).ToList();
                    foreach (var item in removedJobTypes)
                    {
                        var jobType = _currentDbContext.JobTypes.Find(item.JobTypeId);
                        resource.JobTypes.Remove(jobType);
                    }
                }
                else
                {
                    var removedJobTypes = resource.JobTypes.ToList();
                    foreach (var item in removedJobTypes)
                    {
                        var jobType = _currentDbContext.JobTypes.Find(item.JobTypeId);
                        resource.JobTypes.Remove(jobType);
                    }

                }

                // add new job types if selected any
                if (jobTypeIds != null && jobTypeIds.Count > 0)
                {
                    var newJobTypes = jobTypeIds.Where(m => !resource.JobTypes.Select(j => j.JobTypeId).Contains(m));
                    foreach (var item in newJobTypes)
                    {
                        var jobType = _currentDbContext.JobTypes.Find(item);
                        resource.JobTypes.Add(jobType);
                    }
                }

                //save db changes
                _currentDbContext.SaveChanges();
            }

            return resource;
        }

        public void DeleteAppointmentResource(int resourceId, int userId)
        {
            Resources item = _currentDbContext.Resources.Find(resourceId);
            item.IsDeleted = true;
            item.DateUpdated = DateTime.UtcNow;
            item.UpdatedBy = userId;

            _currentDbContext.Entry(item).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public void DeleteByEmployeeId(int employeeId)
        {
            var employee = GetByEmployeeId(employeeId);
            _currentDbContext.Resources.Remove(employee);
            _currentDbContext.SaveChanges();
        }
        public IQueryable<Resources> GetAllActiveAppointmentResourceses(int tenantId)
        {
            var resources = _currentDbContext.Resources.AsNoTracking().Where(a => a.IsDeleted != true && a.TenantId == tenantId);
            return resources;
        }

        public Resources GetAppointmentResourceById(int resourceId)
        {
            return _currentDbContext.Resources.FirstOrDefault(a => a.ResourceId == resourceId && a.IsDeleted != true);
        }

        public List<int> GetResourceJobTypeIds(int resourceId)
        {
            List<int> jobTypeIds = new List<int>();

            Resources resource = _currentDbContext.Resources.FirstOrDefault(a => a.IsActive && a.ResourceId == resourceId && a.IsDeleted != true);

            jobTypeIds = resource?.JobTypes.Where(a => a.IsDeleted != true)?.Select(x => x.JobTypeId)?.ToList();

            return jobTypeIds;
        }

        public IQueryable<ResourceRequests> GetAllResourceRequests(int tenantId)
        {
            return _currentDbContext.ResourceHolidays.Where(m => m.TenantId == tenantId && m.IsDeleted != true);
        }

        public ResourceRequests GetResourceHolidayRequestById(int requestId)
        {
            return _currentDbContext.ResourceHolidays.Find(requestId);
        }
        public ResourceRequests AddResourceHolidayRequest(ResourceRequestsViewModel resourceRequests, int tenantId, int userId)
        {
            resourceRequests.RequestedDate = DateTime.UtcNow;
            resourceRequests.Label = 1;
            resourceRequests.AllDay = false;
            resourceRequests.EventType = 0;
            resourceRequests.Status = 1;
            resourceRequests.RequestNotes = resourceRequests.RequestNotes;
            resourceRequests.RequestStatus = ResourceRequestStatusEnum.Created;
            resourceRequests.Notes = resourceRequests.Notes;

            var request = _mapper.Map<ResourceRequests>(resourceRequests);
            request.DateCreated = DateTime.UtcNow;
            _currentDbContext.ResourceHolidays.Add(request);
            request.TenantId = tenantId;
            request.CreatedBy = userId;
            _currentDbContext.SaveChanges();
            return request;
        }

        public List<HolidayResponseSync> GetUserHolidays(int userId, DateTime? reqDate = null, bool includeIsDeleted = false)
        {
            var results = new List<HolidayResponseSync>();
            var resourceId = _userService.GetResourceIdByUserId(userId);
            var holidays = _currentDbContext.ResourceHolidays.Where(m => m.ResourceId == resourceId && (includeIsDeleted || m.IsDeleted != true) && (!reqDate.HasValue || (m.DateUpdated ?? m.DateCreated) >= reqDate)).ToList();
            foreach (var result in holidays)
            {
                var response = new HolidayResponseSync
                {
                    RequestedDate = result.RequestedDate,
                    IsAllDay = result.AllDay == true,
                    HolidayReason = result.HolidayReason,
                    StartDate = result.StartDate,
                    RequestNotes = result.Notes,
                    EndDate = result.EndDate,
                    HolidayRequestId = result.Id,
                    UserId = userId,
                    IsDeleted = result.IsDeleted,
                    RequestStatus = result.RequestStatus,
                    ActionReason = result.ActionReason,
                    ActionedBy = result.ActionedBy,

                };

                results.Add(response);
            }

            return results;
        }

        public HolidayResponseSync AddResourceHolidaySync(HolidayRequestSync model, int tenantId, int userId)
        {

            var request = new ResourceRequests();
            request.RequestedDate = model.RequestedDate;
            request.AllDay = model.IsAllDay == true;
            request.HolidayReason = model.HolidayReason;
            request.StartDate = model.StartDate;
            request.EndDate = model.EndDate;
            request.ResourceId = _userService.GetResourceIdByUserId(userId);
            request.RequestType = ResourceRequestTypesEnum.AnnualHoliday;
            request.RequestStatus = ResourceRequestStatusEnum.Created;
            request.DateCreated = DateTime.UtcNow;
            _currentDbContext.ResourceHolidays.Add(request);
            request.TenantId = tenantId;
            request.CreatedBy = userId;
            _currentDbContext.SaveChanges();

            var response = new HolidayResponseSync();
            _mapper.Map(model, response);
            return response;
        }

        public ResourceRequests UpdateResourceHolidayRequest(ResourceRequestsViewModel resourceRequests, int userId)
        {
            var newResourceRequest = GetResourceHolidayRequestById(resourceRequests.Id);
            newResourceRequest.UpdatedBy = userId;
            newResourceRequest.DateUpdated = DateTime.UtcNow;
            newResourceRequest.ResourceId = resourceRequests.ResourceId;
            newResourceRequest.StartDate = resourceRequests.StartDate;
            newResourceRequest.EndDate = resourceRequests.EndDate;
            newResourceRequest.HolidayReason = resourceRequests.HolidayReason;
            newResourceRequest.RequestStatus = resourceRequests.RequestStatus;
            newResourceRequest.ActionReason = resourceRequests.ActionReason;
            newResourceRequest.RequestType = resourceRequests.RequestType;

            newResourceRequest.Notes = resourceRequests.Notes;
            _currentDbContext.Entry(newResourceRequest).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return newResourceRequest;
        }
        public ResourceRequests DeleteResourceHolidayRequestById(int requestId, int userId)
        {
            var request = GetResourceHolidayRequestById(requestId);
            request.IsDeleted = true;
            request.UpdatedBy = userId;
            request.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(request).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return request;
        }
        public HolidayRequestResult UpdateResourceHolidayRequestStatus(ApproveRequestViewModel model, int userId)
        {

            var request = GetResourceHolidayRequestById(model.ResourceHolidayRequestId);
            request.UpdatedBy = userId;
            request.DateUpdated = DateTime.UtcNow;

            var response = new HolidayRequestResult(); ;

            if (model.IsApproved)
            {
                request.RequestStatus = ResourceRequestStatusEnum.Accepted;
                request.ActionedBy = userId;
                request.ActionReason = null;
                _currentDbContext.Entry(request).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return new HolidayRequestResult();

            }
            else
            {
                request.ActionReason = model.Reason;
                request.RequestStatus = ResourceRequestStatusEnum.Declined;
                _currentDbContext.Entry(request).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }

            response.HasWarning = false;

            return response;
        }

        public HolidayRequestResult CountResourceHolidayRequestStatus(ApproveRequestViewModel model, int userId)
        {

            var request = GetResourceHolidayRequestById(model.ResourceHolidayRequestId);

            var response = new HolidayRequestResult(); ;
            response = _resourceHolidayServices.VerifyHolidayList(model.ResourceHolidayRequestId, request.ResourceId, request.StartDate.Value, request.EndDate, request.EventType);



            return response;
        }

        public bool IsUserAvailable(int userId)
        {
            return _currentDbContext.Resources.Any(u => u.AuthUserId == userId && u.IsDeleted != true);

        }

        public List<int> GetEmployeeYears(int CurrentWarehouseId)
        {
            List<int> years = new List<int>();
            var currentYear = DateTime.UtcNow.Year;
            var previousfouryear = DateTime.UtcNow.AddYears(-4).Year;
            for (int i = previousfouryear; i <= currentYear; i++)
            {
                years.Add(i);

            }

            var yearsort = years.OrderByDescending(u => u).ToList();
            return yearsort;
        }

        public double GetHolidaysCountByEmployeeId(int EmployeeId, int Year)
        {
            List<double> TotalDays = new List<double>();
            var TotalHolidays = _currentDbContext.ResourceHolidays.Where(u => u.ResourceId == EmployeeId && u.AllDay == true && u.StartDate.Value.Year >= Year && u.EndDate.Value.Year <= Year && u.IsDeleted != true).ToList();
            foreach (var holiday in TotalHolidays)
            {
                var days = (holiday.EndDate - holiday.StartDate).Value.TotalDays;
                TotalDays.Add(days);
            }
            return TotalDays.Sum();
        }
    }
}