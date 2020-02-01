using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Ganedata.Core.Data
{
    public interface IDbContext
    {
        //IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        void Dispose();
        DbChangeTracker ChangeTracker { get; }
        Database Database { get; }
        DbContextConfiguration Configuration { get; }

    }
}
