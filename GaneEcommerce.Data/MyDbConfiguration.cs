using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.IO;

namespace Ganedata.Core.Data
{
    public class MyDbConfiguration : DbConfiguration
    {
        public MyDbConfiguration()
        {
            SetTransactionHandler(SqlProviderServices.ProviderInvariantName, () => new CommitFailureHandler());
            SetExecutionStrategy(SqlProviderServices.ProviderInvariantName, () => new SqlAzureExecutionStrategy());

            //generate EF compiled model on startup

            var modelStoreDir = AppDomain.CurrentDomain.BaseDirectory + @"ModelCache";
            if (!Directory.Exists(modelStoreDir))
            {
                Directory.CreateDirectory(modelStoreDir);
            }
            else
            {
                // delete old model if exists
                DirectoryInfo dir = new DirectoryInfo(modelStoreDir);

                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }
            }

            this.SetModelStore(new DefaultDbModelStore(modelStoreDir));

        }
    }
}