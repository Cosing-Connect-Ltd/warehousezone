using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderSchedule
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int Label { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int EventType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public int? MarketVehicleId { get; set; }
        public string ResourceIDs { get; set; }
        public int? PalletDispatchId { get; set; }
        public int? TenentId { get; set; }
        public bool IsCanceled { get; set; }
        public string CancelReason { get; set; }
        [ForeignKey("MarketVehicleId")]
        public virtual MarketVehicle Resources { get; set; }
        [ForeignKey("PalletDispatchId")]
        public virtual PalletsDispatch PalletsDispatches { get; set; }
    }
}