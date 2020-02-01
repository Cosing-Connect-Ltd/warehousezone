using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class TerminalGeoLocation
    {
        [Key]
        public Guid Id { get; set; }
        public int TerminalId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime Date { get; set; }
        public int? LoggedInUserId { get; set; }
        public int TenantId { get; set; }
        [ForeignKey("TerminalId")]
        public virtual Terminals Terminals { get; set; }
        [ForeignKey("LoggedInUserId")]
        public virtual AuthUser AuthUser { get; set; }

    }
}