using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class ActivityGroupMapWithNames
    {
        public int ActivityGroupMapId { get; set; }
        public string ActivityName { get; set; }
        public string GroupName { get; set; }

    }

    public class AuthPermissionsViewModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ActivityName { get; set; }
        public int SortOrder { get; set; }
        public int ActivityGroupId { get; set; }
    }

    public class WarehousePermissionViewModel
    {
        public int WId { get; set; }
        public string WName { get; set; }
    }
}