using Ganedata.Warehouse.PropertiesSync.Data;
using Ganedata.Warehouse.PropertiesSync.Services.Implementations;
using Ganedata.Warehouse.PropertiesSync.Services.Interfaces;
using System.Windows.Forms;
using Unity;
using Unity.Lifetime;

namespace Ganedata.Warehouse.PropertiesSyncServiceTester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var container = new UnityContainer();
            container.RegisterType<IRepository, IRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITenantsService, PTenantsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISiteSyncService, SiteSyncService>(new ContainerControlledLifetimeManager());
             
            Application.Run(new SyncTester());
        }
    }
    
}
