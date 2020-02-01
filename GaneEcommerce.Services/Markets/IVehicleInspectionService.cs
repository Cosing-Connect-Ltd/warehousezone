using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;

namespace Ganedata.Core.Services
{
    public interface IVehicleInspectionService
    {
        List<InspectionCheckListViewModel> GetAllInspectionCheckLists(int tenantId);
        List<VehicleDriverViewModel> GetAllVehicleDrivers(int tenantId);
        List<VehicleInspectionViewModel> GetAllVehicleInspections(int tenantId);
        VehicleInspectionViewModel GetVehicleInspectionById(int vehicleInspectionId);
        VehicleInspection SaveInspection(VehicleInspection inspection, int userId, List<int> checkedList, bool apiCall = false);
        void DeleteInspection(int inspectionId, int userId);
    }
}