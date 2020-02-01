using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.SyncData
{
    public interface ISyncDataDbContext
    {
        DbSet<PTenant> PTenants { get; set; }
        DbSet<PLandlord> PLandlords { get; set; }
        DbSet<PProperty> PProperties { get; set; }
        DbSet<PSyncHistory> PSyncHistories { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbEntityEntry Entry(object entity);
    }
}