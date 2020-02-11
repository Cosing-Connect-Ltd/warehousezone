using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class AssetLog
    {
        [Key]
        [Display(Name = "Log Id")]
        public Guid AssetLogId { get; set; }
        [Display(Name = "Terminal Id")]
        public int TerminalId { get; set; }
        [Display(Name = "Asset Id")]
        public int? AssetId { get; set; }
        public DateTime DateCreated { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string piAddress { get; set; }
        public string uuid { get; set; }
        public int major { get; set; }
        public int minor { get; set; }
        public short measuredPower { get; set; }
        public short rssi { get; set; }
        public double accuracy { get; set; }
        public string proximity { get; set; }
        public string address { get; set; }
        [ForeignKey("AssetId")]
        public virtual Assets Assets { get; set; }
        [ForeignKey("TerminalId")]
        public virtual Terminals Terminals { get; set; }
        public string Ipv4 { get; set; }
        public object Ipv6 { get; set; }
        public DateTime? SeenTime { get; set; }
        public long SeenEpoch { get; set; }
        public string Ssid { get; set; }
        public string Os { get; set; }
        public string ClientMac { get; set; }
        public string Manufacturer { get; set; }


    }
}