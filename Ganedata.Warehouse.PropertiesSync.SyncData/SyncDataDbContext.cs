using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.SyncData
{
     

    public class SyncDataDbContext : DbContext, ISyncDataDbContext
    {
        public SyncDataDbContext() : base("SyncDataDbContext")
        {
        }

        public DbSet<PTenant> PTenants { get; set; }
        public DbSet<PLandlord> PLandlords { get; set; }
        public DbSet<PProperty> PProperties { get; set; }
        public DbSet<PSyncHistory> PSyncHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
 
    }
}
