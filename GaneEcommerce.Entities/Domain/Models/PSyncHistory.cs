using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class PSyncHistory
    {
        [Key]
        public int PSyncHistoryId { get; set; }
        public DateTime SyncStartTime { get; set; }

        public DateTime ImportCompletedTime { get; set; }
        public DateTime SyncCompletedTime { get; set; }

        public int TenantsSynced { get; set; }
        public int LandlordsSynced { get; set; }
        public int PropertiesSynced { get;set; }
    }
}