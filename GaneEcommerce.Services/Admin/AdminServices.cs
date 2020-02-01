using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly IApplicationContext _applicationContext;

        public AdminServices(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }


        public TenantDepartments SaveTenantDepartment(TenantDepartments department, int userId)
        {
            if (department.DepartmentId < 1)
            {
                department.UpdateCreatedInfo(userId);
                _applicationContext.TenantDepartments.Add(department);
            }
            else
            {
                department.UpdateUpdatedInfo(userId);
                _applicationContext.Entry(department).State = EntityState.Modified;
                _applicationContext.SaveChanges();
            }
            return department;
        }

        public List<Locations> GetAllLocations(int tenantId, int warehouseId)
        {
            return _applicationContext.Locations.Where(a => a.WarehouseId == warehouseId && a.IsDeleted != true && a.TenentId == tenantId).ToList();
        }

        public IEnumerable<PalletTracking> GetPalletTrackingsbyProductId(int? productId, int TenantId, int WarehouseId)
        {
            return _applicationContext.PalletTracking.Where(u => (!productId.HasValue || u.ProductId == productId) && u.TenantId == TenantId && u.WarehouseId == WarehouseId && u.Status != Entities.Enums.PalletTrackingStatusEnum.Created);


        }
    }
}