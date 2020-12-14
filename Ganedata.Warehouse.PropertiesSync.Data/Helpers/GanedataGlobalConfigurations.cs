using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Ganedata.Warehouse.PropertiesSync.Data.Helpers
{
    public class 
        GanedataGlobalConfigurations
    {
        public static int WarehouseSyncSiteId {
            get
            {
                if (!_WarehouseSyncSiteId.HasValue)
                {
                    _WarehouseSyncSiteId =
                        string.IsNullOrEmpty(WebConfigurationManager.AppSettings["WarehouseSyncSiteID"]) ? 0
                            : Int32.Parse(WebConfigurationManager.AppSettings["WarehouseSyncSiteID"]);
                }
                return _WarehouseSyncSiteId.Value;
            }
            set { _WarehouseSyncSiteId = value; }
        }
        private static int? _WarehouseSyncSiteId { get; set; }

        private static List<string> _EntityConnections { get; set; }

        public static List<string> EntityConnections
        {
            get
            {
                if (_EntityConnections == null)
                {
                    var connections = new List<string>();
                    foreach (ConnectionStringSettings conn in WebConfigurationManager.ConnectionStrings)
                    {
                        if (conn.Name.ToLower().Contains("entities"))
                        {
                            connections.Add(conn.Name);
                        }
                    }
                    _EntityConnections = connections;
                }

                return _EntityConnections;
            }
            set { _EntityConnections = value; }
        }

        public static string WarehouseStoreBaseUri => WebConfigurationManager.AppSettings["WarehouseStoreBaseUri"];
        public static int WarehouseSyncIntervalSeconds => WebConfigurationManager.AppSettings["WarehouseSyncIntervalSeconds"]!=null? int.Parse(WebConfigurationManager.AppSettings["WarehouseSyncIntervalSeconds"]):60*15;

    }

}
