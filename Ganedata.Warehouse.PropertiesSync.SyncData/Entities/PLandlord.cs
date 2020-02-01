using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Warehouse.PropertiesSync.SyncData.Entities
{
    public class PLandlord : ContactInfo
    {
        [Key]
        public int PLandlordId { get; set; }
        public string LandlordCode { get; set; }
        public string LandlordFullname { get; set; }
        public string LandlordSalutation { get; set; }
        public string LandlordStatus { get; set; }

        public DateTime? LandlordAdded { get; set; }

        public string LandlordNotes1 { get; set; }
        public string LandlordNotes2 { get; set; }

        public string UserNotes1 { get; set; }
        public string UserNotes2 { get; set; }
        public int SiteId { get; set; }
        public bool SyncRequiredFlag { get; set; }
    }
}