using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;

namespace Ganedata.Core.Services
{
    public class VehicleInspectionService : IVehicleInspectionService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IEmployeeServices _employeeServices;
        private readonly IUserService _userService;

        public VehicleInspectionService(IApplicationContext currentDbContext, IEmployeeServices employeeServices, IUserService userService)
        {
            _currentDbContext = currentDbContext;
            _employeeServices = employeeServices;
            _userService = userService;
        }
        public List<InspectionCheckListViewModel> GetAllInspectionCheckLists(int tenantId)
        {
            return _currentDbContext.VehicleInspectionCheckLists.Select(m => new InspectionCheckListViewModel()
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                VehicleInspectionTypeId = m.VehicleInspectionTypeId,
                VehicleInspectionTypeName = m.VehicleInspectionType.TypeName
            })
                .ToList();
        }

        public List<VehicleDriverViewModel> GetAllVehicleDrivers(int tenantId)
        {
            var allResources = _employeeServices.GetAllEmployees(tenantId);
            return allResources.Select(m => new VehicleDriverViewModel() { Id = m.ResourceId, Name = m.Name }).ToList();
        }

        public List<VehicleInspectionViewModel> GetAllVehicleInspections(int tenantId)
        {
            return _currentDbContext.VehicleInspections.Where(m => m.TenantId == tenantId && m.IsDeleted != true).OrderByDescending(x => x.InspectionDateTime).ToList()
                .Select(x => new VehicleInspectionViewModel()
                {
                    Id = x.Id,
                    ReportedToUserId = x.ReportedToUserId,
                    ReportedToName = x.ReportedToUser != null ? x.ReportedToUser.Name : "",
                    VehicleDriverId = x.VehicleDriverId,
                    DriverName = x.VehicleDriver != null ? x.VehicleDriver.Name : "",
                    FleetNumber = x.FleetNumber,
                    DriverSignature = x.DriverSignature,
                    InspectionDateTime = x.InspectionDateTime,
                    MarketVehicleId = x.MarketVehicleId,
                    VehicleName = x.MarketVehicle != null ? x.MarketVehicle.VehicleIdentifier : "",
                    MileageReading = x.MileageReading,
                    Notes = x.Notes,
                    RectifiedDateTime = x.RectifiedDateTime,
                    RectifiedSignature = x.RectifiedSignature,
                    RectifiedUserId = x.RectifiedUserId,
                    RectifiedUserName = x.RectifiedUser != null ? x.RectifiedUser.Name : ""
                }).ToList();
        }


        public VehicleInspectionViewModel GetVehicleInspectionById(int vehicleInspectionId)
        {
            var x = _currentDbContext.VehicleInspections.FirstOrDefault(m => m.Id == vehicleInspectionId);

            if (x == null) return new VehicleInspectionViewModel();

            return new VehicleInspectionViewModel()
            {
                Id = x.Id,
                ReportedToUserId = x.ReportedToUserId,
                VehicleDriverId = x.VehicleDriverId,
                FleetNumber = x.FleetNumber,
                DriverSignature = x.DriverSignature,
                InspectionDateTime = x.InspectionDateTime,
                MarketVehicleId = x.MarketVehicleId,
                MileageReading = x.MileageReading,
                Notes = x.Notes,
                RectifiedDateTime = x.RectifiedDateTime,
                RectifiedSignature = x.RectifiedSignature,
                RectifiedUserId = x.RectifiedUserId,
                CheckedInspectionIds = x.ConfirmedChecklists.Select(m => m.VehicleInspectionCheckListId).ToList()
            };
        }
        public VehicleInspection SaveInspection(VehicleInspection inspection, int userId, List<int> checkedList, bool apiCall = false)
        {
            if (inspection.Id > 0)
            {
                var m = _currentDbContext.VehicleInspections.First(x => x.Id == inspection.Id);
                m.DriverSignature = inspection.DriverSignature;
                m.FleetNumber = inspection.FleetNumber;
                var resourceId = _userService.GetResourceIdByUserId(inspection.VehicleDriverId ?? 0);

                if (apiCall == true && resourceId > 0)
                {
                    m.VehicleDriverId = resourceId;
                }
                else
                {
                    inspection.VehicleDriverId = null;
                }

                m.MileageReading = inspection.MileageReading;
                m.NilDefect = inspection.NilDefect;
                m.RectifiedUserId = inspection.RectifiedUserId;
                m.RectifiedDateTime = inspection.RectifiedDateTime;
                m.ReportedToUserId = inspection.ReportedToUserId;
                m.Notes = inspection.Notes;
                m.UpdateUpdatedInfo(userId);
                m.CreatedBy = m.CreatedBy;
                m.DateCreated = m.DateCreated;

                var deletedList = m.ConfirmedChecklists.Where(s => !checkedList.Contains(s.VehicleInspectionCheckListId)).ToList();
                foreach (var item in deletedList)
                {
                    _currentDbContext.Entry(item).State = EntityState.Deleted;
                }
                _currentDbContext.SaveChanges();

                var newItems = checkedList.Where(c => !m.ConfirmedChecklists.Select(s => s.VehicleInspectionCheckListId).Contains(c));
                foreach (var item in newItems)
                {
                    _currentDbContext.VehicleInspectionConfirmedLists.Add(new VehicleInspectionConfirmedList()
                    {
                        VehicleInspectionCheckListId = item,
                        VehicleInspection = m
                    });
                }
                _currentDbContext.SaveChanges();
            }
            else
            {
                inspection.InspectionDateTime = DateTime.UtcNow;
                inspection.UpdateCreatedInfo(userId);
                var resourceId = _userService.GetResourceIdByUserId(inspection.VehicleDriverId ?? 0);

                if (apiCall == true && resourceId > 0)
                {
                    inspection.VehicleDriverId = resourceId;
                }
                else
                {
                    inspection.VehicleDriverId = null;
                }

                foreach (var item in checkedList)
                {
                    inspection.ConfirmedChecklists.Add(new VehicleInspectionConfirmedList()
                    {
                        VehicleInspectionCheckListId = item,
                        VehicleInspection = inspection
                    });
                }

                _currentDbContext.Entry(inspection).State = EntityState.Added;
                _currentDbContext.SaveChanges();
            }

            return inspection;
        }

        public void DeleteInspection(int inspectionId, int userId)
        {
            var inspection = _currentDbContext.VehicleInspections.First(m => m.Id == inspectionId);
            if (inspection != null)
            {
                inspection.IsDeleted = true;
                inspection.UpdateUpdatedInfo(userId);
                _currentDbContext.Entry(inspection).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
        }
    }
}